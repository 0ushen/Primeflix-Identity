using System.Security.Claims;
using Duende.IdentityServer.Services;
using IdentityServer.Extensions;
using IdentityServer.Identity;
using IdentityServer.Interfaces;
using IdentityServer.ViewModels.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

public class AuthController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IIdentityServerInteractionService _interactionService;
    private readonly INotificationService _notificationService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IIdentityServerInteractionService interactionService,
        INotificationService notificationService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _interactionService = interactionService;
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId)
    {
        await _signInManager.SignOutAsync();

        var logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);

        return string.IsNullOrEmpty(logoutRequest.PostLogoutRedirectUri)
            ? RedirectToAction("Index", "Home")
            : Redirect(logoutRequest.PostLogoutRedirectUri);
    }

    [HttpGet]
    public async Task<IActionResult> Login(string returnUrl)
    {
        var externalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();

        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl,
            ExternalProviders = externalProviders
        });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        vm.ExternalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();

        if (!ModelState.IsValid)
            return View(vm);

        var result = await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, false, false);

        if (result.Succeeded)
        {
            return Redirect(vm.ReturnUrl);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Too much failed attempts");
        }
        else if(result.IsNotAllowed)
        {
            ModelState.AddModelError(string.Empty, "User is not allowed to login");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Wrong username or password");
        }

        return View(vm);
    }

    [HttpGet]
    public IActionResult Register(string returnUrl)
    {
        return View(new RegisterViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var user = new ApplicationUser(vm.Username)
        {
            Email = vm.Email
        };
        var result = await _userManager.CreateAsync(user, vm.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(vm);
        }

        await _signInManager.SignInAsync(user, false);

        return Redirect(vm.ReturnUrl);

    }

    public ActionResult ExternalLogin(string provider, string returnUrl)
    {
        var redirectUri = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });

        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUri);

        return Challenge(properties, provider);
    }

    public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return RedirectToAction(nameof(Login));

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

        if (result.Succeeded)
            return Redirect(returnUrl);

        var username = info.Principal.FindFirst(ClaimTypes.Name)?.Value.Replace(" ", "_");

        return View("ExternalRegister", new ExternalRegisterViewModel
        {
            Username = username,
            ReturnUrl = returnUrl
        });
    }

    public async Task<IActionResult> ExternalRegister(ExternalRegisterViewModel vm)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction("Login");
        }

        var user = new ApplicationUser(vm.Username)
        {
            Email = info.Principal.FindFirst(ClaimTypes.Email)?.Value
        };
        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(vm);
        }

        result = await _userManager.AddLoginAsync(user, info);

        if (!result.Succeeded)
        {
            return View(vm);
        }

        await _signInManager.SignInAsync(user, false);

        return Redirect(vm.ReturnUrl);
    }

    public ActionResult RetrievePassword(string returnUrl) => View("RetrievePassword", new RetrievePasswordViewModel
    {
        ReturnUrl = returnUrl
    });

    [HttpPost]
    public async Task<ActionResult> RetrievePassword(RetrievePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View("RetrievePassword", model);

        var vm = new RetrievePasswordResultViewModel
        {
            ReturnUrl = model.ReturnUrl
        };

        var user = await _userManager.FindByEmailAsync(model.Email);

        try
        {
            if (user == null)
                return View("RetrievePasswordResult", vm);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.AbsoluteAction(nameof(ResetPassword), "Auth", new
            {
                userId = user.Id, 
                token,
                returnUrl = model.ReturnUrl
            });

            await _notificationService.SendChangePasswordLinkAsync(user.Email, url);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, $"An error occured while retrieving user's password");

            return View("RetrievePassword", model);
        }

        return View("RetrievePasswordResult", vm);
    }

    [HttpGet]
    public async Task<ActionResult> ResetPassword(string userId, string token, string returnUrl)
    {
        if (string.IsNullOrEmpty(token))
            return NotFound();

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return NotFound();

        var model = new ResetPasswordViewModel
        {
            UserId = userId,
            Token = token
        };

        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());

            if (user == null)
                throw new Exception($"Cant find user with id {model.UserId}");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                return View("ResetPasswordResult", new ResetPasswordResultViewModel
                {
                    ReturnUrl = model.ReturnUrl
                });
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, "There was an error durring the password reset");
        }

        return View(model);
    }
}
