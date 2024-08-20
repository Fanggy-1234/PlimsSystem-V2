using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_EmployeeGroupWorking
    {
      
        public int ID { get; set; }
        [Key]
        public string GroupID { get; set; }
        [Key]
        public string EmployeeID { get; set; }
        [Key]
        public int PlantID { get; set; }
        public int Status { get; set; }
        [Key]
        public int TransactionNo { get; set; }
        [Key]
        public DateTime TransactionDate { get; set; }

        public int Shift { get; set; }
        public string Prefix { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string Line { get; set; }
        public string LineName { get; set; }
        public string Section { get; set; }
        public string SectionName { get; set; }
        public string WorkingStatus { get; set; }

        public string ClockIn { get; set; }

        public string ClockOut { get; set; }

        public string Breakflag { get; set; }
        public string Remark { get; set; }



    }
}
