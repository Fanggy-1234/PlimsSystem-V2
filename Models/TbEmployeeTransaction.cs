using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbEmployeeTransaction
    {
        [Key]
        public int TransactionNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }
        public string EmployeeID { get; set; }
        public int Shift { get; set; }

        public string Prefix { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public int Plant { get; set; }
        public string Line { get; set; }
        public string Section { get; set; }
        public string WorkingStatus { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public string BreakFlag { get; set; }
        public string? Remark { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
