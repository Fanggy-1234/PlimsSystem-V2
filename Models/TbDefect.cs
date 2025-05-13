using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbDefect
    {
        [Key]
        public int DefectID { get; set; }
        public string DefectName { get; set; }
        public int DefectPlant { get; set; }
        public int DefectLine { get; set; }
        public int DefectSection { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
