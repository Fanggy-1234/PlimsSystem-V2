using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbService
    {
        [Key]
        public string ServicesID { get; set; }
        public string ServicesName { get; set; }
        [Key]
        public int PlantID { get; set; }
        public string LineID { get; set; }
        public string SectionID { get; set; }
        
        public decimal ServicesRate { get; set; }
        public int ServicesStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
