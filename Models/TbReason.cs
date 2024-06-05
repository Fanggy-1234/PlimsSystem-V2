using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbReason
    {
        [Key]
        public string ReasonID { get; set; }
        public string ReasonName { get; set; }
        [Key]
        public int PlantID { get; set; }
        public string LineID { get; set; }
        //public int ProdID { get; set; }
        public string ProductID { get; set; }
        public string SectionID { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
