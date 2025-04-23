using Microsoft.AspNetCore.Mvc;
using DATNBACK_END.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

namespace DATNBACK_END.Controllers
{
    public class AdminController : Controller
    {
        private readonly IPlayerService _playerService;

        public AdminController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        // Trang chính: Đăng nhập & Đăng ký
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Đăng nhập với Google
        [HttpPost]
        public IActionResult LoginWithGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback")
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Xử lý callback từ Google
        [HttpGet]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                ViewBag.Error = "Không thể đăng nhập bằng Google.";
                return RedirectToAction("Index");
            }

            // Lấy thông tin user từ Google
            var email = result.Principal.FindFirst(c => c.Type == "email")?.Value;
            var name = result.Principal.FindFirst(c => c.Type == "name")?.Value;

            if (email == null)
            {
                ViewBag.Error = "Không thể lấy thông tin email.";
                return RedirectToAction("Index");
            }

            // Xử lý đăng nhập hoặc tạo tài khoản
            var response = await _playerService.LoginWithGoogle(email, name);
            if (!response.status)
            {
                ViewBag.Error = response.message;
                return RedirectToAction("Index");
            }

            ViewBag.Success = "Đăng nhập thành công!";
            return RedirectToAction("Index");
        }

        // Xử lý đăng nhập thông thường
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var response = await _playerService.Login(email, password);
            if (!response.status)
            {
                ViewBag.Error = response.message;
                return RedirectToAction("Index");
            }

            ViewBag.Success = "Đăng nhập thành công!";
            return RedirectToAction("Index");
        }

    }
}
