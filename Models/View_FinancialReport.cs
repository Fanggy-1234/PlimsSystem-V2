using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Math;
using System;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plims.Models
{
    public class View_FinancialReport
    {

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }
        [Key]
        public int PlantID { get; set; }
        //public string PlantName { get; set; }
        [Key]
        public string LineID { get; set; }
        public string LineName { get; set; }
        [Key]
        public string SectionID { get; set; }
        public string SectionName { get; set; }
        [Key]
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public int QTYPerQRCode { get; set; }
        public int FormularID { get; set; }
        public string QRCode { get; set; }
        public string EmployeeName { get; set; }

        public string ShiftName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string  Reason { get; set; }
        public string  Note { get; set; }
        public int TotalCountQty { get; set; }
        public int TotalFGQty { get; set; }
        public int TotalQty { get; set; }
        public int Defect { get; set; }
        public decimal ProductSTD { get; set; }
        public decimal PercentSTD { get; set; }
        public decimal PercentYield { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public DateTime CurrentTime { get; set; }   // time
        public string TransactionDateTime { get; set; }
        public decimal hourSinceClockIn { get; set; }
        public decimal ShiftHour { get; set; }

        public decimal PiecePerHr { get; set; }
        public string IncentiveID { get; set; }
        public string IncentiveName { get; set; }
        public decimal Rate { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public string Grade { get; set; }
        public decimal STDincentive { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Incentive { get; set; }
        public decimal EffManPerSTD { get; set; }
    }
}
