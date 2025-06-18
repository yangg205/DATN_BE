using DATNBACK_END.Models;
using DATNBACK_END.Service;
using DATNBACK_END.Services;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Encodings;

namespace DATNBACK_END.Controllers;

public class AdminController : Controller
{
    private readonly IPlayerService _playerservice;
    public AdminController(IPlayerService playerService)
    {
        _playerservice = playerService;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";
        try
        {
            //lay email nguoi choi tu Session 
            var email = HttpContext.Session.GetString("email");
            if (string.IsNullOrEmpty(email))
            {
                //chuyen huong ve login neu chua login
                return RedirectToAction("LoginandRegister");
            }
            var player = await _playerservice.GetPlayersByEmail(email);
            if (player == null)
            {
                return RedirectToAction("LoginandRegister");
            }
            ViewData["UserName"] = player.Name;
            ViewData["UserBalance"] = player.Total_Money;
            return View();
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
    [HttpGet]
    public IActionResult LoginandRegister()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(Player player)
    {
        try
        {
            if (string.IsNullOrEmpty(player.Email) || string.IsNullOrEmpty(player.Password))
            {
                throw new Exception("du lieu trong");
            }
            var login = await _playerservice.Login(player.Email, player.Password);
            // System.Console.WriteLine(email,password);
            Console.WriteLine($"Login status: {login.status}");
            if (login.status == true)
            {
                //luu email vao session
                HttpContext.Session.SetString("email", player.Email);
                Console.WriteLine($"Session Email: {player.Email}");
                return RedirectToAction("Index");
            }
            ViewBag.message = login.message;
            return View("LoginAndRegister");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] Dangky player)
    {
        try
        {
            if (string.IsNullOrEmpty(player.Email) || string.IsNullOrEmpty(player.Password) || string.IsNullOrEmpty(player.Repassword))
            {
                return Json(new { status = false, message = "Dữ liệu không hợp lệ" });
            }

            var registerResult = await _playerservice.RegisterPendingPlayer(player.Email, player.Password, player.Repassword);

            return Json(registerResult);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return Json(new { status = false, message = "Đăng ký thất bại. Vui lòng thử lại." });
        }
    }

    [HttpGet]
    public IActionResult Recharge()
    {
        var email = HttpContext.Session.GetString("email");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("LoginAndRegister");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> StartRecharge(decimal amount, [FromServices] IPaypalService paypalService)
    {
        var email = HttpContext.Session.GetString("email");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("LoginAndRegister");
        }
        try
        {
            // Gọi PayPal Service để tạo thanh toán
            var approvalUrl = await paypalService.CreatePayment(amount, "USD", "Nạp tiền vào tài khoản game");
            Console.WriteLine("Approval URL: " + approvalUrl);
            if (!string.IsNullOrEmpty(approvalUrl))
            {
                return Redirect(approvalUrl); // Chuyển hướng tới PayPal
            }
            else
            {
                ViewBag.ErrorMessage = "Không thể tạo thanh toán. Vui lòng thử lại.";
                return View("Recharge");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            ViewBag.ErrorMessage = "Có lỗi xảy ra khi tạo thanh toán.";
            return View("Recharge");
        }
    }


    [HttpGet]
    public async Task<IActionResult> ExecutePayment(string token, [FromServices] IPaypalService paypalService)
{
        // Lấy email từ Session
    
    Console.WriteLine($"Controller ExecutePayment: token={token}");
    var email = HttpContext.Session.GetString("email");
    if (string.IsNullOrEmpty(email))
    {
        // Nếu không có email, chuyển hướng về trang đăng nhập/đăng ký
        return RedirectToAction("LoginAndRegister");
    }
     Console.WriteLine($"Controller ExecutePayment: email from session={email}");
    
    
    // Kiểm tra xem token (Order ID) có hợp lệ không
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Thiếu thông tin giao dịch PayPal. Vui lòng thử lại.";
            return RedirectToAction("Index"); // Hoặc về trang nạp tiền
        }

    try
    {
        // Gọi paypalService.ExecutePayment.
        // paymentId trong IPaypalService sẽ nhận giá trị của 'token' (là Order ID).
        // PayerID không còn cần thiết cho Orders API v2, nhưng vẫn cần truyền vào để khớp chữ ký.
        // Bạn có thể truyền một chuỗi rỗng hoặc null cho PayerID vì nó không được sử dụng.
        var state = await paypalService.ExecutePayment(token, null, email); // PayerID được truyền là null hoặc string.Empty

        if (state.ToLower() == "completed" || state.ToLower() == "approved") // Kiểm tra trạng thái "completed" hoặc "approved"
        {
            TempData["SuccessMessage"] = "Nạp tiền thành công! Số tiền đã được cộng vào tài khoản của bạn.";
        }
        else
        {
            TempData["ErrorMessage"] = $"Thanh toán không thành công. Trạng thái: {state}.";
        }
        return RedirectToAction("Index"); // Chuyển hướng về trang Index hoặc một trang thông báo kết quả
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error executing PayPal payment: {ex.Message}");
        TempData["ErrorMessage"] = "Có lỗi xảy ra khi xử lý thanh toán PayPal. Vui lòng liên hệ hỗ trợ.";
        return RedirectToAction("Index"); // Hoặc về trang lỗi
    }
}

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear(); // Xóa session

        // Chuyển hướng đến URL LoginAndRegister
        return Redirect("/Admin/LoginandRegister");
    }
}



