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

    public AccountController(IUserApiService userApiService)
    {
        _userApiService = userApiService;

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
        var response = await _userApiService.LoginAsync(loginModel);
        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Invalid email or password";
            ModelState.AddModelError("", "Invalid email or password");
            return View(loginModel);
        }

        var content = await response.Content.ReadFromJsonAsync<LoginResponseModel>();
        if (content == null || string.IsNullOrEmpty(content.Token))
        {
            TempData["ErrorMessage"] = "Invalid response from server";
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
