using System.ComponentModel.DataAnnotations.Schema;
using Yard_Management_System.Entity;

namespace Yard_Management_System
{
    public class PermissionRole
    {
        public Guid Id { get; set; }

        [ForeignKey(nameof(Permission))]
        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; }

        [ForeignKey(nameof(Role))]
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}
