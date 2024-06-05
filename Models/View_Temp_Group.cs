using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_Temp_Group
    {
        [Key]
        public string ID { get; set; }
        [Key]
        public string EmployeeID { get; set; }

        public string EmployeeName { get; set; }

        public string EmployeeLastName { get; set; }

    }
}
