using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbPackage
    {
         
        [Key]
        public int PackageID { get; set; }
        public string PackageName { get; set; }
        public int PackageQTY { get; set; }
        public string Detail { get; set; }
    }
}
