namespace BankApp;

public interface IBankManger
{
    void AccountBalance();
    void DepositMoney();
    void WithdrawMoney();
    void TransferToOtherBank();
    void AccountLimits();
    void ViewAccountInformation();
    void TransactionHistory();
    void AccountType();
    void InwardTransaction();
}
