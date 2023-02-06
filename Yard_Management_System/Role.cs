using Microsoft.EntityFrameworkCore;
using System.Security;

namespace Yard_Management_System
{
    public class Role
    {
        public int RoleId { get; set; }
        public string? Title { get; set; }
        public List<Permission> Permissions { get; set; } = new();
        public List<PermissionRole> PermissionRoles { get; set; } = new();
    }
}
