namespace DATNBACK_END.Service;
using DATNBACK_END.Models; // Chứa PaypalConfig
using DATNBACK_END.Services; // Chứa IPaypalService và IPlayerService

using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

using System; // Cần cho Guid, Exception
using System.Collections.Generic; // Cần cho List
using System.Linq; // Cần cho FirstOrDefault
using System.Threading.Tasks; // Cần cho Task

using Microsoft.Extensions.Configuration; // Cần để đọc cấu hình
using Newtonsoft.Json; // Cần để serialize object thành JSON để debug

public class PaypalService : IPaypalService
{
    private readonly PaypalConfig _config;
    private readonly IPlayerService _playerService;
    private readonly PayPalHttpClient _payPalClient;

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

        PayPalEnvironment environment;
        if (_config.Mode?.ToLower() == "sandbox")
        {
            environment = new SandboxEnvironment(_config.ClientId, _config.ClientSecret);
        }
        else
        {
            // Mặc định là Live nếu không phải Sandbox. Bạn nên có một cơ chế kiểm tra mode nghiêm ngặt hơn.
            environment = new LiveEnvironment(_config.ClientId, _config.ClientSecret);
        }
        _payPalClient = new PayPalHttpClient(environment);
    }

    public async Task<string> CreatePayment(decimal amount, string currency, string description)
    {
        try
        {
            var orderRequest = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE", // Ý định là thu tiền ngay lập tức
                ApplicationContext = new ApplicationContext()
                {
                    BrandName = "Night Reign", // Tên hiển thị trên trang PayPal
                    LandingPage = "BILLING", // Trang đích trên PayPal (trang thanh toán/hóa đơn)
                    ShippingPreference = "NO_SHIPPING", // Không yêu cầu địa chỉ giao hàng
                    UserAction = "PAY_NOW", // Nút trên PayPal sẽ là "Pay Now"
                    ReturnUrl = "https://datn-be-oygi.onrender.com/Admin/ExecutePayment", // URL sau khi thanh toán thành công
                    CancelUrl = "https://datn-be-oygi.onrender.com/Admin/Recharge" // URL nếu người dùng hủy
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest()
                    {
                        ReferenceId = Guid.NewGuid().ToString(), // ID duy nhất cho đơn vị mua hàng này
                        Description = description, // Mô tả giao dịch
                        AmountWithBreakdown = new AmountWithBreakdown()
                        {
                            CurrencyCode = currency, // Loại tiền tệ (USD, VND, EUR,...)
                            Value = amount.ToString("F2") // Tổng số tiền với 2 chữ số thập phân
                            // KHÔNG CÓ PHẦN 'Breakdown' hay 'Items' ở đây, chỉ là tổng tiền
                        }
                    }
                }
            };

            // Gửi yêu cầu tạo Order tới PayPal
            var request = new OrdersCreateRequest().RequestBody(orderRequest);
            var response = await _payPalClient.Execute(request);
            var createdOrder = response.Result<Order>(); // Kết quả trả về là đối tượng Order

            // Lấy URL phê duyệt để chuyển hướng người dùng
            var approvalUrl = createdOrder.Links?.FirstOrDefault(link => link.Rel?.ToLower() == "approve")?.Href;

            if (string.IsNullOrEmpty(approvalUrl))
            {
                throw new Exception("Approval URL không được tạo từ phản hồi của PayPal. Kiểm tra cấu hình hoặc phản hồi API.");
            }

            return approvalUrl;
        }
        catch (PayPalHttp.HttpException ex)
        {
            Console.WriteLine($"Error creating PayPal order (HTTP Error): {ex.Message}");
            // Log chi tiết lỗi từ PayPal (Status Code và Message của lỗi)
            Console.WriteLine($"PayPal HTTP Error Details: Status Code = {ex.StatusCode}, Message = {ex.Message}");
            throw; // Ném lại exception để Controller xử lý
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred during PayPal order creation: {ex.Message}");
            throw; // Ném lại exception
        }
    }

    public async Task<string> ExecutePayment(string paymentId, string payerId, string email)
    {
        try
        {
            Console.WriteLine($"ExecutePayment called: paymentId={paymentId}, payerId={payerId}, email={email}");

            // Gửi yêu cầu để thu tiền Order (capture)
            var request = new OrdersCaptureRequest(paymentId); // paymentId ở đây chính là Order ID
            request.RequestBody(new OrderActionRequest());

            var response = await _payPalClient.Execute(request);
            var executedOrder = response.Result<Order>();

            // Kiểm tra null ở các cấp độ
            if (executedOrder == null)
            {
                Console.WriteLine("Error: executedOrder is null after PayPal execution. No order details received.");
                throw new Exception("Không nhận được chi tiết giao dịch từ PayPal.");
            }
            Console.WriteLine($"PayPal Order Status: {executedOrder.Status}");

            // Log toàn bộ JSON để debug chi tiết nếu có vấn đề về dữ liệu trả về
            Console.WriteLine("--- Full executedOrder JSON Response ---");
            Console.WriteLine(JsonConvert.SerializeObject(executedOrder, Formatting.Indented));
            Console.WriteLine("---------------------------------------");

            if (executedOrder.PurchaseUnits == null || executedOrder.PurchaseUnits.Count == 0)
            {
                Console.WriteLine("Error: PurchaseUnits is null or empty in PayPal response. Cannot determine amount.");
                throw new Exception("Không có đơn vị mua hàng trong phản hồi của PayPal.");
            }

            var firstPurchaseUnit = executedOrder.PurchaseUnits[0];
            if (firstPurchaseUnit == null)
            {
                Console.WriteLine("Error: First PurchaseUnit is null in PayPal response.");
                throw new Exception("Đơn vị mua hàng đầu tiên bị thiếu trong phản hồi.");
            }

            // Lấy số tiền từ captures thay vì AmountWithBreakdown
            if (firstPurchaseUnit.Payments?.Captures == null || firstPurchaseUnit.Payments.Captures.Count == 0)
            {
                Console.WriteLine("CRITICAL ERROR: No captures found in PurchaseUnit. Cannot determine amount.");
                throw new Exception("Không tìm thấy thông tin capture trong phản hồi của PayPal.");
            }

            var firstCapture = firstPurchaseUnit.Payments.Captures[0];
            if (firstCapture == null || firstCapture.Amount == null || string.IsNullOrEmpty(firstCapture.Amount.Value))
            {
                Console.WriteLine("CRITICAL ERROR: Capture amount is null or empty. Cannot determine amount.");
                throw new Exception("Không thể xác định số tiền giao dịch từ PayPal (giá trị capture bị thiếu).");
            }

            // Chuyển đổi số tiền từ string sang decimal
            if (!decimal.TryParse(firstCapture.Amount.Value, out decimal amount))
            {
                Console.WriteLine($"CRITICAL ERROR: Could not parse amount '{firstCapture.Amount.Value}' to decimal.");
                throw new Exception("Không thể chuyển đổi số tiền giao dịch thành định dạng số.");
            }

            Console.WriteLine($"Số tiền đã được duyệt bởi PayPal: {amount}");

            // Cập nhật tiền cho người chơi trong game của bạn
            await _playerService.Updatemoney(email, amount);

            Console.WriteLine($"Đã cập nhật tiền cho người chơi {email}.");

            return executedOrder.Status;
        }
        catch (PayPalHttp.HttpException ex)
        {
            Console.WriteLine($"Error executing PayPal payment (HTTP Error): {ex.Message}");
            // Log chi tiết lỗi HTTP từ PayPal
            Console.WriteLine($"PayPal HTTP Error Details: Status Code = {ex.StatusCode}, Message = {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred during PayPal payment execution: {ex.Message}");
            throw;
        }
    }
}