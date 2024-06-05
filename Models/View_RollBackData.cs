using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_RollBackData
    {
        [Key]
        public long RunningNumber { get; set; }
        public DateTime ProductionDate { get; set; }
        public int PlantID { get; set; }
        public string LineID { get; set; }
        public string SectionID { get; set; }
        public string ProductID { get; set; }
        public string QRCode { get; set; }

        public string EmployeeID { get; set; }

        public string LineName { get; set; }

        public string SectionName { get; set; }
        public string ProductName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeLastName  { get; set; }
        public string ShiftName  { get; set; }
        public string StartTime  { get; set; }
        public string EndTime  { get; set; }


    }
}
