using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Role : IdentityRole<int> { }
}