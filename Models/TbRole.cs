using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbRole
    {
        [Key]
        public int ID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int RoleStatus { get; set; }
        public int PlantID { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
