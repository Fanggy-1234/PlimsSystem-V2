using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_EmployeeGroup
    {
        [Key]
        public string GroupID { get; set; }
        [Key]
        public string EmployeeID { get; set; }
        [Key]
        public int PlantID { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeLastName { get; set; }

        public string LineID { get; set; }
        public string SectionID { get; set; }
    }
}
