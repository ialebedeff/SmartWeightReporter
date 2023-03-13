namespace Entities
{
    public class AuthenticationData
    {
        public AuthenticationData() { } 
        public AuthenticationData(IdentityData user)
        {
            User = user;
        }
        public IdentityData User { get; set; } = null!;
    }
}