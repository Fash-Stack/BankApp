namespace BankApp;

public class BankMenu
{
    public IBankMenu BankService;
    public BankMenu()
    {
        BankService = new BankManager();
    }
    public void Menu()
    {
        bool running = true;
        
        while (running)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("===Welcome to Fash Bank Plc===");
            Console.ResetColor();
            Console.WriteLine("1. Create Account\n2. Log in\n3. Exit");
            Console.Write("Enter your choice: ");
            bool choice = int.TryParse(Console.ReadLine()!, out int userChoice);

            if (userChoice == 1)
            {
                BankService.CreatAccount();
            }
            else if (userChoice == 2)
            {
                BankService.LogIn();
            }
            else if (userChoice == 3)
            {
                running = false;
                Console.WriteLine();
                Console.WriteLine("Exiting Application...");
            }
            else 
            {
                Console.WriteLine("Invalid input! Please read the instruction carefully");
            }
        }
    }
}
