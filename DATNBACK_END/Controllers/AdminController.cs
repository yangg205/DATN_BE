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
    public async Task<IActionResult> ExecutePayment(string paymentId, string token, string PayerID, [FromServices] IPaypalService paypalService)
    {
        var email = HttpContext.Session.GetString("email");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("LoginAndRegister");
        }

        if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(PayerID))
        {
            TempData["ErrorMessage"] = "Thiếu thông tin giao dịch. Vui lòng thử lại.";
            return RedirectToAction("Index");
        }

        try
        {
            var state = await paypalService.ExecutePayment(paymentId, PayerID, email);
            if (state.ToLower() == "approved")
            {
                TempData["SuccessMessage"] = "Nạp tiền thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Thanh toán không thành công.";
            }
            return RedirectToAction("Index"); // Chuyển hướng về trang Index
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi xử lý thanh toán.";
            return RedirectToAction("Index");
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



