using DocumentFormat.OpenXml.Spreadsheet;
using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbProductionPlan
    {
        [Key]
        public int TransactionID { get; set; }
        public DateTime  PlanDate { get; set; }
        public int PlantID { get; set; }
        public String Prefix { get; set; }
        //public int ShiftID { get; set; }
        public string LineID { get; set; }
        public string SectionID { get; set; }
        //public int ProdID { get; set; }
        public string ProductID { get; set; }
        public int SizeMin { get; set; }
        public int SizeMax { get; set; }
        public int Weight { get; set; }
        public int QRcodeperday { get; set; }
        public int TotalPiecePerDay { get; set; }
        public int QTY { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
