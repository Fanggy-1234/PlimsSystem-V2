using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class Temp_Group
    {
        [Key]
        public string ID { get; set; }
        [Key]
        public string EmployeeID { get; set; }

    }
}
