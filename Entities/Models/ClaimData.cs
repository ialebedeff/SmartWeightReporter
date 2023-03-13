using System.Security.Claims;

namespace Entities
{
    public class ClaimData
    {
        public string Issuer { get; set; } = null!;
        public string OriginalIssuer { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string ValueType { get; set; } = null!;

        public ClaimData() { }
        public ClaimData(Claim claim)
        {
            Issuer = claim.Issuer;
            OriginalIssuer = claim.OriginalIssuer;
            Type = claim.Type;
            Value = claim.Value;
            ValueType = claim.ValueType;

        }
    }
}