using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_User
    {
        [Key]
        public int ID { get; set; }
        public string UserEmpID { get; set; }
        public string UserName { get; set; }
        public string UserLastName { get; set; }
        public string UserPassword { get; set; }
        public string UserEmail { get; set; }
        public int PlantID { get; set; }
        public string PlantName { get; set; }

        //public string LineName { get; set; }
        //public string SectionName { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> PasswordLastUpdate { get; set; }
        public string Lineconcern { get; set; }
    }
}
