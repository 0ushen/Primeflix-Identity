using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Identity;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        
    }
    public ApplicationUser(string userName) : base(userName)
    {
    }
}
