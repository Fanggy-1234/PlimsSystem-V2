using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbLine
    {
        [Key]
        public int ID { get; set; }

        public string LineID { get; set; }
        public string LineName { get; set; }
        public int Status { get; set; }

        public int PlantID { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
