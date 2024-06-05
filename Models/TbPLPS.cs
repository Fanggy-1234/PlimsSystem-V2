using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbPLPS
    {

        [Key]
        public string PLPSID { get; set; }
        [Key]
        public int PlantID { get; set; }
        public string LineID { get; set; }
        //public int ProdID { get; set; }
        public string ProductID { get; set; }
        public string SectionID { get; set; }
        public string Size { get; set; }
        public int QTYPerQRCode { get; set; }
        public string Unit { get; set; }
        public int FormularID { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
