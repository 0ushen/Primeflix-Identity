using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.ViewModels.Auth;

public class ExternalRegisterViewModel : IdentityViewModel
{
    public string Username { get; set; }
}