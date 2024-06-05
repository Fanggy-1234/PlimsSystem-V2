using DocumentFormat.OpenXml.Office2010.Excel;
using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbSetup
    {

        [Key]
        public int ID { get; set; }
        public string  Name { get; set; }
        public int Valuesetup { get; set; }
        public int PlantID { get; set; }
        public string  CreateBy { get; set; }
        public DateTime  CreateDate { get; set; }
        public string  UpdateBy { get; set; }
        public DateTime  UpdateDate { get; set; }

    }
}
