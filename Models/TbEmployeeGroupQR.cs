using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbEmployeeGroupQR
    {
        [Key]
        public int ID { get; set; }
        public string GroupID { get; set; }
        public string EmployeeID { get; set; }
        public int PlantID { get; set; }
        public string  LineID { get; set; }
        public string SectionID { get; set; }
        public int Status { get; set; }

        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }

    }
}
