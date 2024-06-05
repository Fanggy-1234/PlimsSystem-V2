using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_EFFReport
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }
        //[Key]
        public int PlantID { get; set; }
        //[Key]
        public string LineID { get; set; }
        public string LineName { get; set; }
        //[Key]
        public string ProductID { get; set; }
        public string ProductName { get; set; }

        public int CountQRCode { get; set; }
        //[Key]
      //  public string StartTime { get; set; }
        //[Key]
      //  public string EndTime { get; set; }

        //[Key]
        public string SectionID { get; set; }
        public string SectionName { get; set; }
        public decimal ActualworkhourService { get; set; }
        public decimal ProductSTD { get; set; }
        public decimal PercentSTD { get; set; }
        public decimal PercentYield { get; set; }
        public decimal WorkinghourSTD { get; set; } // decimal
        public int FinishGood { get; set; }
        public int Defect { get; set; }
      public string Unit { get; set; }
        public decimal EFF1 { get; set; } // decimal
        public decimal WorkinghourACT { get; set; } // decimal
        public decimal Servicehour { get; set; } // decimal
        public decimal Supporthour { get; set; } // decimal
        public decimal EFF2 { get; set; } // decimal
        public decimal EFF3 { get; set; }  // decimal
        public decimal EFFhr1 { get; set; }  // decimal
        public decimal EFFhr2 { get; set; }  // decimal
        public decimal EFFhr3 { get; set; }  // decimal
        public decimal YieldDefect { get; set; }

        public decimal MEDEFF3 { get; set; }
        public decimal ValueEFF3 { get; set; }

        public decimal KPIh3 { get; set; }
        public decimal MEDh3 { get; set; }
        public decimal ValEffh3 { get; set; }
        public decimal KPIh1 { get; set; }
        public decimal MEDh1 { get; set; }

        public decimal ValEffh1 { get; set; }


    }
}
