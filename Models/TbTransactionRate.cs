using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbTransactionRate
    {

        [Key]
        public int TransactionNo { get; set; }

        [Key]
        public DateTime TransactionDate { get; set; }
        [Key]
        public int PlantID { get; set; }
        [Key]
        public string LineID { get; set; }
        [Key]
        public string ProductID { get; set; }
        [Key]
        public string SectionID { get; set; }
        [Key]
        public string EmployeeID { get; set; }

        [Key]
        public string Type { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal? Min { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal? Max { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Rate { get; set; }
        public string Grade { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
