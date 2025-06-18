namespace DATNBACK_END.Services;
public interface IPaypalService{
    Task<string> CreatePayment(decimal amount, string currency, string description);
    Task<string> ExecutePayment(string paymentId,string payerId,string email);
}