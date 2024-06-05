using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class GeneratingQRCode
    {
        [Display(Name = "Enter QRCode Text")]
        public string QRCodeText { get; set; }
    }
}
