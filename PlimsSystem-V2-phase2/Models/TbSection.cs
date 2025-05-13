using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbSection
    {
        [Key]
        public string SectionID { get; set; }
        public string SectionName { get; set; }
        public string Delaytime { get; set; }
        [Key]
        public int PlantID { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
