using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_PermissionMaster
    {
        [Key]
        public int PageID { get; set; }
        public string PageName { get; set; }
        [Key]
        public string UserEmpID { get; set; }
        public string UserName { get; set; }
        public string UserLastName { get; set; }
        public string UserPassword { get; set; }
        public int ID { get; set; }
        public string UserEmail { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleAction { get; set; }

        public int Status { get; set; }
        [Key]
        public int PlantID { get; set; }
    }
}
