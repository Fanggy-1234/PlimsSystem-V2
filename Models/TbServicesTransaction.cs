using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbServicesTransaction
    {
        [Key]
        public int TransactionNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public string EmployeeID { get; set; }
        public int Plant { get; set; }
        public int Shift { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public string Line { get; set; }
        public string SectionID { get; set; }
        public string SectionName { get; set; }
        public string ServicesName { get; set; }
        public string ServicesID { get; set; }
        public string WorkingStatus { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public string Remark { get; set; }
        public string StatusClocktime { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
