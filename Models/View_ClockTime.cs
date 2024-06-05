using System.ComponentModel.DataAnnotations;
namespace Plims.Models
{
    public class View_ClockTime
    {

        [Key]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }
        [Key]
        public string EmployeeID { get; set; }
        [Key]
        public int PlantID { get; set; }
        [Key]
        public string ShiftName { get; set; }
        public string Prefix { get; set; }
        public string EndTime { get; set; }
        public string StartTime { get; set; }

        public string ClockIn { get; set; }

        public string ClockOut { get; set; }
        [Key]
        public string LineID { get; set; }
        [Key]
        public string SectionID { get; set; }

        public string WorkingStatus { get; set; }
        [Key]
        public string Type { get; set; }
        public string Remark { get; set; }
    }
}
