using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using UserManagement.Services;
using UserManagement.ViewModels;
using static UserManagement.ViewModels.LoginViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace UserManagement.Controllers;

public class AccountController: Controller
{
    private readonly IUserApiService _userApiService;
    private readonly IEmailService _emailService;
    private readonly IAuthService _authService;

    public AccountController(IUserApiService userApiService, IEmailService emailService, IAuthService authService)
    {
        _userApiService = userApiService;
        _emailService = emailService;
        _authService = authService;
    }

    public async Task<IActionResult> Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(User user)
    {
       
        if (ModelState.IsValid)
        {
            var (success, message) = await _userApiService.CreateUserAsync(user);

            if (success)
            {
                TempData["success"] = "User Register Successfully!"; 
                return RedirectToAction("Login");
            }

            TempData["error"] = "A user with this email is already registered.";
            return View(user);
        }
        return View(user);

    }

    // GET : Login Page
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginModel)
    {
        if (!ModelState.IsValid)
            return View(loginModel);

        // Call the API Login endpoint
        var response = await _authService.LoginAsync(loginModel);
        if (!response.IsSuccessStatusCode)
        {
            TempData["error"] = "Invalid email or password";
            ModelState.AddModelError("", "Invalid email or password");
            return View(loginModel);
        }

        var content = await response.Content.ReadFromJsonAsync<LoginResponseModel>();
        if (content == null || string.IsNullOrEmpty(content.Token))
        {
            TempData["error"] = "Invalid response from server";
            return View(loginModel);
        }

        // Set AuthToken cookie
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = loginModel.RememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddHours(24)
        };

        Response.Cookies.Append("AuthToken", content.Token, cookieOptions);

        // 1. Decode the JWT to get the claims
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(content.Token);

        // 2. ClaimsIdentity using the claims from the JWT
        var claims = jwtToken.Claims; 
        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        // 3. Set authentication properties for the cookie
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = loginModel.RememberMe,
            ExpiresUtc = jwtToken.ValidTo 
        };

        // 4. Sign in the user using the Cookie Authentication scheme
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity),             
            authProperties);                                

        var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        return roleClaim switch
        {
            "Admin" => RedirectToAction("Index", "Home"), 
            "User" => RedirectToAction("Index", "Home"), 
            _ => RedirectToAction("Login", "Account")      
        };
    }

     // GET: Account/ForgotPassword
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var response = await _authService.ForgotPasswordAsync(model.Email, baseUrl);

        if (response.Success)
        {
            var resetLink = $"{baseUrl}/Account/ResetPassword?userId={response.UserId}&token={response.Token}";

            // Send email from MVC using Razor view
            await _emailService.SendPasswordResetEmail(model.Email, resetLink);

            TempData["success"] = "A password reset link has been sent to your email.";
            return RedirectToAction("Login");
        }

        TempData["error"] = response.Message;
        return View(model);
    }

    public async Task<IActionResult> ResetPassword(int userId, string token)
    {
        if (userId == 0 || string.IsNullOrEmpty(token))
        {
            return BadRequest("Invalid password reset link.");
        }

        var isValid = await _authService.ValidateResetTokenAsync(userId, token);

        if (!isValid)
        {
            TempData["error"] = "Reset link is invalid or expired.";
            return RedirectToAction("ForgotPassword");
        }

        var model = new ResetPasswordViewModel
        {
            UserId = userId,
            Token = token
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.ResetPasswordAsync(model);

        if (result == "User not found." || result == "Invalid or expired token.")
        {
            TempData["error"] = result;
            return View(model);
        }

        TempData["success"] = result;
        return RedirectToAction("Login");
    }


    public async Task<IActionResult> Logout(){ 
        Response.Cookies.Delete("AuthToken");
        Response.Cookies.Delete(".AspNetCore.Cookies");
        return RedirectToAction("Login","Account");
    }

    [AllowAnonymous] 
    public IActionResult AccessDenied()
    {
        return View();
    }

}
