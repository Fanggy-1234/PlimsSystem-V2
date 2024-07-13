using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plims.Models
{
    public class View_ProductionTransactionAj
    {
        [Key]
        public int TransactionNo { get; set; }
  
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }
      
        public int PlantID { get; set; }
     
        public string LineID { get; set; }
       
        public string SectionID { get; set; }
        
        public string ProductID { get; set; }
        
        public string QRCode { get; set; }
        public int Qty { get; set; }

        [Column(TypeName = "decimal(28,8)")]
        public decimal QtyPerQR { get; set; }
        public string DataType { get; set; }

        public string Prefix { get; set; }
    }
}
