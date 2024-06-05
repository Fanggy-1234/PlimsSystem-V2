using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbShift
    {

        [Key]
        public int ShiftID { get; set; }
        public string Prefix { get; set; }
        public string ShiftName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        [Key]
        public int PlantID { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
