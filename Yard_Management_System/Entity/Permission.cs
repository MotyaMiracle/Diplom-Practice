using Microsoft.EntityFrameworkCore;

namespace Yard_Management_System.Entity
{
    public class Permission
    {
        public Guid PermissionId { get; set; }
        public string Name { get; set; } = "";
        public bool Access { get; set; }
        public List<PermissionRole> PermissionRoles { get; set; } = new();
    }
}
