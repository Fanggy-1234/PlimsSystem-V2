using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbEmployeeLeaveHoliday
    {
        [Key]
        public int TransactionNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public string EmployeeID { get; set; }
        public int ShiftID { get; set; }
        public int PlantID { get; set; }
        public string LineID { get; set; }
        public string SectionID { get; set; }
        public string WorkingStatus { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public string? Remark { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
