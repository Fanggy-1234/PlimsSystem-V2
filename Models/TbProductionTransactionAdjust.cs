using System;
using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbProductionTransactionAdjust
    {
        [Key]
        public int TransactionID { get; set; }
        public DateTime TransactionDate { get; set; }
        public int PlantID { get; set; }
        public string LineID { get; set; }
        public string SectionID { get; set; }
        public string Prefix { get; set; }
        public string Type { get; set; }
        public int QTY { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }

    }
}
