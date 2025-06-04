
using DATNBACK_END.Models;
using DATNBACK_END.Services;
using PayPal.Api;

namespace DATNBACK_END.Service
{
    public class PaypalService : IPaypalService
    {
        private readonly PaypalConfig _config;
        private readonly IPlayerService _playerService;

        public PaypalService(IConfiguration configuration, IPlayerService playerService)
        {
            var paypalConfig = configuration.GetSection("PayPal");
            _config = new PaypalConfig
            {
                ClientId = paypalConfig["ClientID"],
                ClientSecret = paypalConfig["ClientSecret"],
                Mode = paypalConfig["Mode"]
            };
            _playerService = playerService;
        }

        private APIContext GetAPIContext()
        {
            var config = new Dictionary<string, string>
            {
                { "mode", _config.Mode }
            };
            var accessToken = new OAuthTokenCredential(_config.ClientId, _config.ClientSecret, config).GetAccessToken();
            return new APIContext(accessToken) { Config = config };
        }

        public async Task<string> CreatePayment(decimal amount, string currency, string description)
        {
            try
            {
                var apiContext = GetAPIContext();
                var payment = new Payment
                {
                    intent = "sale",
                    payer = new Payer { payment_method = "paypal" },
                    transactions = new List<Transaction>
                    {
                        new Transaction
                        {
                            amount = new Amount
                            {
                                total = amount.ToString("F2"),
                                currency = currency
                            },
                            description = description
                        }
                    },
                    redirect_urls = new RedirectUrls
                    {
                        cancel_url = "http://localhost:7102/Admin/Recharge",
                        return_url = "http://localhost:7102/Admin/ExecutePayment"
                    }
                    // redirect_urls = new RedirectUrls
                    // {
                    //     cancel_url = "https://localhost:7010/Admin/Recharge", // URL hủy giao dịch
                    //     return_url = "https://localhost:7010/Admin/ExecutePayment" // URL xử lý sau khi thanh toán
                    // }
                };

                var createdPayment = payment.Create(apiContext);
                var approvalUrl = createdPayment.links.FirstOrDefault(x => x.rel == "approval_url")?.href;

                if (approvalUrl == null)
                {
                    throw new Exception("Approval URL không được tạo. Kiểm tra cấu hình PayPal.");
                }

                return approvalUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating payment: {ex.Message}");
                throw;
            }
        }

        public async Task<string> ExecutePayment(string paymentId, string payerId, string email)
        {
            try
            {
                var apiContext = GetAPIContext();
                var paymentExecution = new PaymentExecution { payer_id = payerId };
                var payment = new Payment { id = paymentId };

                var executedPayment = payment.Execute(apiContext, paymentExecution);

                if (executedPayment.state.ToLower() == "approved")
                {
                    var amount = decimal.Parse(executedPayment.transactions[0].amount.total);
                    Console.WriteLine($"Số tiền đã được duyệt: {amount}");
                    await _playerService.Updatemoney(email, amount);
                }
                else
                {
                    throw new Exception("Thanh toán không được duyệt.");
                }

                return executedPayment.state;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing payment: {ex.Message}");
                throw;
            }
        }
    }
}
