using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbPlant
    {
        [Key]
        public int PlantID { get; set; }
        public string PlantName { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
