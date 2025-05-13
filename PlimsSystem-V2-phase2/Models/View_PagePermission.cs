using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_PagePermission
    {
        [Key]
        public int PermissionID { get; set; }
        public string PageName { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleAction { get; set; }
        [Key]
        public int PlantID { get; set; }
    }
}
