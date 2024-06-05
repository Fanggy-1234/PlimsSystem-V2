using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_ProductionPlan
    {
        [Key]
        public int TransactionID { get; set; }
        public DateTime PlanDate { get; set; }
        public int PlantID { get; set; }
        public string PlantName { get; set; }
        public string Prefix { get; set; }

        public string LineID { get; set; }
        public string LineName { get; set; }
        public string SectionID { get; set; }
        public string SectionName{ get; set; }

        //public int ProdID { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
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
