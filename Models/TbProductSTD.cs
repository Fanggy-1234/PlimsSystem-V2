using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbProductSTD
    {
        [Key]
        public string ProductSTDID { get; set; }
        [Key]
        public int PlantID { get; set; }
        public string LineID { get; set; }
        //public int ProdID { get; set; }
        public string ProductID { get; set; }
        public string SectionID { get; set; }
        public decimal STD { get; set; }
        public decimal PercentSTD { get; set; }
        public decimal PercentYield { get; set; }
        public decimal YieldIncentive { get; set; }

        public decimal EFFSTD { get; set; }

        public string Unit { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
