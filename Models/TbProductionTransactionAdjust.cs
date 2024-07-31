using System;
using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbProductionTransactionAdjust
    {
        [Key]
        public int TransactionID { get; set; }
        [Key]
        public DateTime TransactionDate { get; set; }
        [Key]
        public int PlantID { get; set; }
        [Key]
        public string LineID { get; set; }
        [Key]
        public string SectionID { get; set; }
        [Key]
        public string Prefix { get; set; }
        [Key]
        public string Type { get; set; }
        public decimal QTY { get; set; } //int
        public string Remark { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }

    }
}
