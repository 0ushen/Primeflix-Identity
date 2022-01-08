using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.ViewModels.Auth;

public class IdentityViewModel
{
    [HiddenInput]
    [Required]
    public string? ReturnUrl { get; init; }
}