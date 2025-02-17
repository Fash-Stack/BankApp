using System.Text.RegularExpressions;
using Newtonsoft.Json;
using ConsoleTables;

namespace BankApp;

public class BankManager : IBankManger, IBankMenu
{
    readonly List<BankService> services = LoadBankService();
     
    public BankManager()
    {
        services = LoadBankService();
    }

    decimal? AccountLimit = 500000;
    public void CreatAccount()
    {
        BankService bankService = new BankService();

        Console.WriteLine("Enter your first name");
        bankService.FirstName = Console.ReadLine();

        Console.WriteLine("Enter your surname");
        bankService.Surname = Console.ReadLine();

        Console.WriteLine("Enter last name");
        bankService.LastName = Console.ReadLine();

        bankService.FullName = bankService.Surname +" " + bankService.FirstName +" "  + bankService.LastName;
        
        Console.WriteLine("Enter your mobile number");
        bankService.MobileNumber = Console.ReadLine()!;

        if (bankService.MobileNumber.Length != 11)
        {
            Console.WriteLine("Phone Number must not be greater or less than 11 figures");
            return;
        }

        string numberMustNot = @"^\d+$";
        if (!Regex.IsMatch(bankService.MobileNumber, numberMustNot))
        {
            Console.WriteLine("Phone Number can not contain an alphabet or special character");
            return;
        }


        Console.WriteLine("Enter your email (optional)");
        bankService.Email = Console.ReadLine();

        Console.WriteLine("Enter password (6 digits)");
        bankService.Password = Console.ReadLine()!;

        if (bankService.Password.Length != 6)
        {
            Console.WriteLine("Password must not be greater or less than 6 figures");
            return;
        }

        string passwordMustNot = @"^\d+$";
        if (!Regex.IsMatch(bankService.Password, passwordMustNot))
        {
            Console.WriteLine("Password can not contain an alphabet or special character");
            return;
        }

        Console.WriteLine("Enter transaction pin (4 digits): ");
        bankService.Pin = int.Parse(Console.ReadLine()!);

        bankService.AccountNumber = $"{bankService.MobileNumber.Trim().Substring(1 , 10)}";
        Console.WriteLine($"Your account number is: {bankService.AccountNumber}");

        bankService.CreatedAt = DateTime.Now;
        
        services.Add(bankService);
        SaveBankService(services);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Account sucessfully created...");
        Console.ResetColor();
        Console.WriteLine();
    }

    public void LogIn()
    {
        if (services.Count == 0)
        {
            Console.WriteLine("No account in the record");
            return;
        }

        Console.Write("Enter mobile number: ");
        string mobileNumber = Console.ReadLine()!;

        var findMobileNumber = services.FirstOrDefault(f => f.MobileNumber == mobileNumber);

        Console.Write("Enter your password: ");
        string password = Console.ReadLine()!;

        if (findMobileNumber is null || findMobileNumber.Password != password)
        {
            Console.WriteLine("Invalid mobile number or password");
            return;
        }

        Console.WriteLine("Login sucessful! Welcome");
        bool running = true;
        while (running)
        {
            Console.WriteLine();        
            Console.WriteLine("1. Deposit Money\n2. Withdraw Money\n3. Check Balance\n4. View Account Information\n5. Account Limits\n6. Transfer Money\n7. Transaction History\n8. Account type\n9. Inward transaction\n10. Log out");
            Console.Write("Enter your choice: ");
            bool input = int.TryParse(Console.ReadLine(), out int userInput);

            switch (userInput)
            {
                case 1:
                    DepositMoney();
                    break;
                case 2:
                    WithdrawMoney();
                    break;
                case 3:
                    AccountBalance();
                    break;
                case 4:
                    ViewAccountInformation();
                    break;
                case 5:
                    AccountLimits();
                    break;
                case 6:
                    TransferToOtherBank();
                    break;
                case 7:
                    TransactionHistory();
                    break;
                case 8:
                    AccountType();
                    break;
                case 9:
                    InwardTransaction();
                    break;
                case 10:
                    running = false;
                    Console.WriteLine();
                    Console.WriteLine("Loging out...");
                    break;
                default:
                    Console.WriteLine("Invalid input! Read the instruction carefully.");
                    break;
            }
        }
    }
    public void AccountBalance()
    {
        Console.Write("Enter your account number: ");
        string accountNumber = Console.ReadLine()!;

        var checkAccountNumber = services.FirstOrDefault(f => f.AccountNumber == accountNumber);
        Console.Write("Enter your pin (4 digit): ");
        int pin = int.Parse(Console.ReadLine()!);

        if (checkAccountNumber is null || checkAccountNumber.Pin != pin)
        {
            Console.WriteLine("Incorrect account number or pin"); 
            return;
        }
        
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine();
        Console.WriteLine("Your account balnce is: " + checkAccountNumber.AccountBalance);
        Console.WriteLine();
        Console.WriteLine("-----------------------------------------");
    }

    public void AccountType()
    {
        string accountType = "Savings";
        Console.WriteLine($"Account type: {accountType}");
    }

    public void DepositMoney()
    {
        Console.Write("Enter your account number: ");
        string accountNumber = Console.ReadLine()!;

        var checkAccountNumber = services.FirstOrDefault(d => d.AccountNumber == accountNumber);

        if (checkAccountNumber is null)
        {
            Console.WriteLine("Invalid input! Wrong Account");
            return;
        }

        Console.Write("Enter the amount you want to deposit: ");
        decimal depositedAmount;
        while (!decimal.TryParse(Console.ReadLine(), out depositedAmount) || depositedAmount <= 0)
        {
            Console.WriteLine("Invalid input! Please enter a valid deposit amount.");
        }

        if (depositedAmount <= 0 || (checkAccountNumber.AccountBalance + depositedAmount) > AccountLimit)
        {
            Console.WriteLine("Invalid amount or check account limits");
            return;
        }

        Console.Write("Enter your payment pin: ");
        int pin;
        while (!int.TryParse(Console.ReadLine(), out pin))
        {
            Console.WriteLine("Invalid PIN! Please enter a valid PIN.");
        }

        if (checkAccountNumber.Pin != pin)
        {
            Console.WriteLine("Incorrect PiN! Transaction failed");
            return;
        }

        checkAccountNumber.AccountBalance += depositedAmount;

        Transaction transaction = new Transaction
        {
            Date = DateTime.Now,
            Description = "Deposit",
            Amount = depositedAmount,
            Category = "Income",
            Status = "Successful"
        };

        checkAccountNumber.Transactions.Add(transaction);

        SaveBankService(services);

        Console.WriteLine($"You have sucessfully deposit the sum of: {depositedAmount}");
       
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine();
        Console.WriteLine($"Account Balance is: {checkAccountNumber.AccountBalance}");
        Console.WriteLine();
        Console.WriteLine("-----------------------------------------");
    }

    public void AccountLimits()
    {
        string accountLimit = "500,000";
        Console.WriteLine("Account Limits: {0}" + accountLimit);
    }

    public void TransferToOtherBank()
    {
        Console.Write("Enter your account number: ");
        string accountNumber = Console.ReadLine()!;

        var validateAccountNumber = services.FirstOrDefault(v => v.AccountNumber == accountNumber);

        Console.Write("Enter the repicent account number: ");
        string repicentAccountNumber = Console.ReadLine()!;

        if (repicentAccountNumber.Length != 10)
        {
            Console.WriteLine("Invalid account number");
            return;
        }

        Console.Write("Enter amount: ");
        decimal amount = decimal.Parse(Console.ReadLine()!);

        if (validateAccountNumber is null || amount <= 0 || amount > validateAccountNumber.AccountBalance)
        {
            Console.WriteLine("Insufficient funds!");
            return;
        }
        
        Console.Write("Enter payment pin: ");
        int pin = int.Parse(Console.ReadLine()!);

        if (validateAccountNumber is null || validateAccountNumber.Pin != pin)
        {
            Console.WriteLine("Access denied! Invalid payment pin");
            return;
        }

        var amountRemaining = validateAccountNumber.AccountBalance - amount;
        validateAccountNumber.AccountBalance = amountRemaining;

        Transaction transaction = new Transaction()
        {
            Date = DateTime.Now,
            Description = $"Transfer to {accountNumber}",
            Amount = amount,
            Category = "Expense",
            Status = "Successful"
        };
        
        validateAccountNumber.Transactions.Add(transaction);

        SaveBankService(services);

        Console.WriteLine($"You have sucessfully transfer the sum of: {amount}");
        
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine();
        Console.WriteLine($"Your account balance is: {amountRemaining}");
        Console.WriteLine();
        Console.WriteLine("-----------------------------------------");
    }

    public void WithdrawMoney()
    {
        Console.Write("Enter your account number: ");
        string accountNumber = Console.ReadLine()!;

        var validateAccountNumber = services.FirstOrDefault(v => v.AccountNumber == accountNumber);

        if (validateAccountNumber is null)
        {
            Console.WriteLine("Invalid account number!");
            return;
        }

        Console.Write("Enter the amount you want to withdraw: ");
        decimal amount = decimal.Parse(Console.ReadLine()!);

        if (amount <= 0 && amount < validateAccountNumber.AccountBalance)
        {
            Console.WriteLine("Insufficient funds!");
            return;
        }

        Console.Write("Enter you payment pin: ");
        int pin = int.Parse(Console.ReadLine()!);

        if (validateAccountNumber.Pin != pin)
        {
            Console.WriteLine("Invalid account niumber or payment pin");
            return;
        }

        var amountRemaining = validateAccountNumber.AccountBalance - amount;
        validateAccountNumber.AccountBalance = amountRemaining;

        Transaction transaction = new Transaction()
        {
            Date = DateTime.Now,
            Description = "Withdraw",
            Amount = amount,
            Category = "Expense",
            Status  = "Successful"
        };

        validateAccountNumber.Transactions.Add(transaction);

        SaveBankService(services);

        Console.WriteLine($"You have sucessfully withdraw the sum of: {amount}");

        Console.WriteLine("-----------------------------------------");
        Console.WriteLine();
        Console.WriteLine($"Your account balance is: {amountRemaining}");
        Console.WriteLine();
        Console.WriteLine("-----------------------------------------");
    }

    public void InwardTransaction()
    {
        Console.Write("Enter your account number: ");
        string accountNumber = Console.ReadLine()!;

        var validateAccountNumber = services.FirstOrDefault(v => v.AccountNumber == accountNumber);

        if (validateAccountNumber == null)
        {
            Console.WriteLine("Account not found!");
            return;
        }

        Console.Write("Enter the recipient account number: ");
        string recipientAccountNumber = Console.ReadLine()!;

        var recipientAccount = services.FirstOrDefault(v => v.AccountNumber == recipientAccountNumber);

        if (recipientAccount == null)
        {
            Console.WriteLine("Recipient account not found!");
            return;
        }

        Console.Write("Enter amount: ");
        decimal amount = decimal.Parse(Console.ReadLine()!);

        if (amount <= 0 || amount > validateAccountNumber.AccountBalance)
        {
            Console.WriteLine("Insufficient funds or invalid amount.");
            return;
        }

        Console.Write("Enter payment pin: ");
        int pin = int.Parse(Console.ReadLine()!);

        if (validateAccountNumber.Pin != pin)
        {
            Console.WriteLine("Access denied! Invalid payment pin.");
            return;
        }

        // Deduct from sender
        validateAccountNumber.AccountBalance -= amount;
        ;
        // Add to recipient
        recipientAccount.AccountBalance += amount;

        Transaction transaction = new Transaction()
        {
            AccountNumber = validateAccountNumber.AccountNumber,
            Date = DateTime.Now,
            Description = $"Inward Transfer to {recipientAccountNumber}",
            Amount = amount,
            Category = "Transfer",
            Status = "Successful"
        };

        validateAccountNumber.Transactions.Add(transaction);

        SaveBankService(services);

        Console.WriteLine($"You have successfully transferred the sum of {amount} to {recipientAccountNumber}.");
        
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine();
        Console.WriteLine($"Your account balance is: {validateAccountNumber.AccountBalance}");
        Console.WriteLine();
        Console.WriteLine("-----------------------------------------");
    }

    public void ViewAccountInformation()
    {
        Console.WriteLine("Enter your first name");
        string userInput = Console.ReadLine()!;
        var info = services.Find(x => x.FirstName == userInput);

        if (info is null)
        {
            Console.WriteLine("Account does not exist!");
            return;
        }

        ConsoleTable table = new("Name", "Email", "Account Number", "Phone Number", "Created At");
        table.AddRow(info.FullName, string.IsNullOrWhiteSpace(info.Email) ? "N/A" : info.Email, info.AccountNumber, info.MobileNumber, info.CreatedAt);
        
        Console.WriteLine();
        table.Write(Format.Alternative);
        Console.WriteLine();
    }

    public void TransactionHistory()
    {
        Console.Write("Enter your account number: ");
        string accountNumber = Console.ReadLine()!;

        var validateAccountNumber = services.FirstOrDefault(v => v.AccountNumber == accountNumber);

        if (validateAccountNumber is null)
        {
            Console.WriteLine("Account number does not exist!");
            return;
        }

        Console.Write("Enter your pin (4 digits): ");
        int pin = int.Parse(Console.ReadLine()!);

        if (validateAccountNumber.Pin != pin)
        {
            Console.WriteLine("Invalid pin");
            return;
        }

        if (validateAccountNumber.Transactions.Count == 0)
        {
            Console.WriteLine("No transaction done");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("----TRANSACTION HISTORY----");

        ConsoleTable table = new("Description", "Amount", "Category", "Status", "Date"); 
        
        foreach (var transaction in validateAccountNumber.Transactions)
        {
            table.AddRow(transaction.Description, transaction.Amount, transaction.Category, transaction.Status, transaction.Date.ToString("dddd, MMMM dd, yyyy h:mm:ss tt"));
        }

        Console.WriteLine();
        table.Write(Format.Alternative);
        Console.WriteLine();
    }

    static List<BankService> LoadBankService()
    {
        string filePath = "BankService.json";

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }

        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<List<BankService>>(json) ?? new List<BankService>();
    }

    static void SaveBankService(List<BankService> service)
    {
        string json = JsonConvert.SerializeObject(service, Formatting.Indented);
        File.WriteAllText("BankService.json", json);
    }

}

