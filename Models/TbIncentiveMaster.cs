using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plims.Models
{
    public class TbIncentiveMaster
    {
   
        [Key]
        public string IncentiveID { get; set; }
        public string IncentiveName { get; set; }
        [Key]
        public int PlantID { get; set; }
        public string LineID { get; set; }
        //public int ProdID { get; set; }
        public string ProductID { get; set; }
        public string SectionID { get; set; }
        public int STD { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal? Min { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal? Max { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Rate { get; set; }
        public string Grade { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
