using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plims.Models
{
    public class View_ProductionTransactionAdjust
    {
        [Key]
        public long TransactionID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }
     
        public int PlantID { get; set; }
        public string PlantName { get; set; }
 
        public string LineID { get; set; }
        public string LineName { get; set; }
   
        public string QRCode { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeLastName { get; set; }
        public string Prefix { get; set; }
     
        public string ProductID { get; set; }
        public string ProductName { get; set; }
   
        public string SectionID { get; set; }
        public string SectionName { get; set; }

        public decimal WorkHr { get; set; }
        [Column(TypeName = "decimal(28,8)")]
        public decimal QtyPerQR { get; set; }

        public int CountQty { get; set; }
        public int FGInputQty { get; set; }  //int
        public int DefectQty { get; set; }  //int
        public int MinusQty { get; set; }  //int

        public decimal FG { get; set; }  //int
        public decimal TotalPiece { get; set; }

        public decimal Yield { get; set; }
        public decimal ALLDefect { get; set; }

    }
}
