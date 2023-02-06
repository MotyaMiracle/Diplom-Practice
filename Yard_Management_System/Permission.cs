using Microsoft.EntityFrameworkCore;
namespace Yard_Management_System
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public string Name { get; set; } = "";
        public bool Access { get; set; }
        public List<Role> Roles { get; set; } = new();
        public List<PermissionRole> PermissionRoles { get; set; } = new();
    }
}
