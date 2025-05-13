using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbUser
    {
        [Key]
        public int ID { get; set; }
        public string UserName { get; set; }
        public string UserLastName { get; set; }
        public string UserPassword { get; set; }
        public int UserPermission { get; set; }
        public string UserEmpID { get; set; }
        public int Status { get; set; }
        public string UserEmail { get; set; }
        public DateTime PasswordLastUpdate { get; set; }
        public int PlantID { get; set; }
        public string Lineconcern { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
