using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbProduct
    {
        [Key]
        public int ID { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }

        public int PlantID { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
