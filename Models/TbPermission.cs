using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbPermission
    {
        [Key]
        public int PermissionID { get; set; }
        public int RoleID { get; set; }
        public int PageID { get; set; }
        public string RoleAction { get; set; }
        public int PlantID { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
