using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_EmployeeGroupList
    {
        [Key]
        public string GroupID { get; set; }
        public int PlantID { get; set; }
        public string EmployeeIDs { get; set; }
        public string EmployeeNames { get; set; }
        public int Status { get; set; }
    }
}
