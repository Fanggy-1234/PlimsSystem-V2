using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbPageMaster
    {
        [Key]
        public int PageNo { get; set; }
        public string PageName { get; set; }
    }
}
