using Microsoft.EntityFrameworkCore;
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
        public decimal? CountQty { get; set; }
        public decimal? FGInputQty { get; set; }
        public decimal? DefectQty { get; set; }
        public decimal? MinusQty { get; set; }
        public decimal? TotalPiece { get; set; }
        public decimal? FG { get; set; }

        // ✅ แก้เป็น decimal?
        public decimal? Yield { get; set; }
        public decimal? WorkHr { get; set; }

        // SUM() adjust → BIGINT → ใช้ long?
        public decimal? FGAdjust { get; set; }
        public decimal? DefectAdjust { get; set; }
        public decimal? ALLDefect { get; set; }
        
    }
}
