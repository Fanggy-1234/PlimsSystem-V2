using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_EmployeeAdjustLine
    {

        [Key]
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateBy { get; set; }
        public string PlantName { get; set; }
        [Key]
        public int PlantID { get; set; }
        [Key]
        public string LineID { get; set; }
        public string FromLine { get; set; }
        public string LineName { get; set; }
        public string FromSectionID { get; set; }
        public string FromSection { get; set; }
        [Key]
        public string SectionID { get; set; }
        public string SectionName { get; set; }
        public string ShiftName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public string WorkingStatus { get; set; }
        public string Remark { get; set; }

        //  public Nullable<int> QRCodePerUnit { get; set; }
        public Nullable<int> QRCodePerEmployee { get; set; }
        public Nullable<int> TransactionNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> TransactionDate { get; set; }
    }
}
