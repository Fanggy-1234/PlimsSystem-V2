using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_DailyReport
    {

        [Key]
        public int TransactionNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime  TransactionDate { get; set; }
    public int PlantID { get; set; }
    public string  PlantName { get; set; }
    public string  LineID { get; set; }
    public string  LineName { get; set; }
    public string  ShiftName { get; set; } 
    public string  ProductID { get; set; } 
    public string  ProductName { get; set; }
    public string EmployeeID { get; set; }
    public string  EmployeeName { get; set; }
    public string SectionID { get; set; }
    public string SectionName { get; set; }
    public int Qty { get; set; }
    public int QtyPerQR { get; set; }
    public string DataType { get; set; }
    public string Reason { get; set; }
    public string Note { get; set; }



}
}
