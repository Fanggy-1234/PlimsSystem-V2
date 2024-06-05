using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbFormular
    {
        [Key]
        public int FormularID { get; set; }
        public string FormularName { get; set; }
        public string Detail { get; set; }
    }
}
