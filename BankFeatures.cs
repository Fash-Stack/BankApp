namespace BankApp;

public class BankService
{
    public string? FirstName { get; set; }
    public string? Surname { get; set; }
    public string? LastName { get; set;} = null;
    public string? FullName { get; set; }
    public string? MobileNumber { get; set; }
    public string? Email { get; set; } = null;
    public string? Password { get; set;}
    public int? Pin { get; set; }
    public string? AccountNumber { get; set;}
    public decimal AccountBalance { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
}

