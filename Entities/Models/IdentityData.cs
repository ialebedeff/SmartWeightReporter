using System.Security.Claims;

namespace Entities
{
    public class IdentityData
    {
        public string? Name { get; set; }
        public string? AuthenticationType { get; set; } 
        public bool IsAuthenticated { get; set; } 
        public IEnumerable<ClaimData>? Claims { get; set; }
        public IdentityData(ClaimsPrincipal claimsIdentity) 
        {
            AuthenticationType = claimsIdentity.Identity.AuthenticationType;
            IsAuthenticated = claimsIdentity.Identity.IsAuthenticated;
            Name = claimsIdentity.Identity.Name;

            Claims = claimsIdentity.Claims.Select(
                claim => new ClaimData(claim));
        }
        public IdentityData() { }
    }
}