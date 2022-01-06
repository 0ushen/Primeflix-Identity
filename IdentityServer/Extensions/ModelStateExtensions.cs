using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityServer.Extensions
{
    public static class ModelStateExtensions
    {
        public static bool HasErrors(this ModelStateDictionary modelState, string[] keys = null)
        {
            if (modelState is null)
                return false;

            if (!keys.IsNullOrEmpty())
            {
                return modelState.Where(x => keys != null && keys.Contains(x.Key))
                                 .Any(x => x.Value.Errors.Any());
            }

            return modelState.Any(x => x.Value.Errors.Any());
        }

        public static bool HasErrors(this ModelStateDictionary modelState, string key)
        {
            return HasErrors(modelState, new[] { key });
        }
    }
}