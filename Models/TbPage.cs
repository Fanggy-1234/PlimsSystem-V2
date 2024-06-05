using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbPage
    {
        [Key]
        public int PageID { get; set; }
        public string PageName { get; set; }
        public int PageStatus { get; set; }
        public int PlantID { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
    
    }
}
