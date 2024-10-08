﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plims.Models
{
    public class View_DailyReportSummary
    {

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }
        [Key]
        public int PlantID { get; set; }
        public string PlantName { get; set; }
        [Key]
        public string LineID { get; set; }
        public string LineName { get; set; }
        [Key]
        public string QRCode { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeLastName { get; set; }
        public string ShiftName { get; set; }
        public string Prefix { get; set; }
        [Key]
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        [Key]
        public string SectionID { get; set; }
        public string SectionName { get; set; }
        public decimal CountQty { get; set; }  //int
        public decimal MinusQty { get; set; }  //int
        public decimal DefectQty { get; set; }  //int
        public decimal FG_Count_Qty { get; set; }  //int
        [Column(TypeName = "decimal(18,3)")]
        public decimal YieldDefect { get; set; }
       // public int QtyPerQR { get; set; }
        public decimal QtyPerQR { get; set; }
    //    public string Reason { get; set; }
     //   public string Note { get; set; }
     //   public string IncentiveID { get; set; }
    //    public string IncentiveName { get; set; }
        public decimal STD { get; set; }  //int  std
        public decimal EFFSTD { get; set; }  //int  std
      // public decimal Min { get; set; }
     //   public decimal Max { get; set; }
        public string Grade { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Rate { get; set; } //
     //   public string ClockIn { get; set; }
      //  public string ClockOut { get; set; }
      //  public DateTime CurrentDateTime { get; set; }
      //  public TimeSpan CurrentTime { get; set; }
        public decimal DiffHours { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal PcsPerHr { get; set; }
       // public decimal PiecePerHr { get; set; }// change from int
        public decimal EffManPerSTD { get; set; }
        public Decimal FGQty { get; set; } //int

        [Column(TypeName = "decimal(18,3)")]
        public decimal wage { get; set; }

        public decimal TotalDefect { get; set; }

        public decimal ActualFG { get; set; }
        public decimal FGAdjust { get; set; } //int

    }
}
