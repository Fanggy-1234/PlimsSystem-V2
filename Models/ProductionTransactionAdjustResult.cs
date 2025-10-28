using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plims.Models
{
    [Keyless]
    public class ProductionTransactionAdjustResult
    {
        public long TransactionID { get; set; }
        public DateTime TransactionDate { get; set; }
        public int PlantID { get; set; }
        public string PlantName { get; set; }
        public string LineID { get; set; }
        public string LineName { get; set; }
        public string SectionID { get; set; }
        public string SectionName { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string QRCode { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeLastName { get; set; }
        public string Prefix { get; set; }

        // ✅ แก้เป็น decimal?
        public decimal? QtyPerQR { get; set; }

        // SUM() คืน BIGINT → ใช้ long?
        [Column(TypeName = "decimal(18,3)")]
        public decimal? CountQty { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal? FGInputQty { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal? DefectQty { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal? MinusQty { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal? TotalPiece { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal? FG { get; set; }

        // ✅ แก้เป็น decimal?
        [Column(TypeName = "decimal(18,3)")]
        public decimal? Yield { get; set; }

        public decimal? WorkHr { get; set; }

        // SUM() adjust → BIGINT → ใช้ long?
        [Column(TypeName = "decimal(18,3)")]
        public decimal? FGAdjust { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal? DefectAdjust { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal? ALLDefect { get; set; }
        
    }
}
