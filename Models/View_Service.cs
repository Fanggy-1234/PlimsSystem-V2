using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_Service
    {
        [Key]
        public string ServicesID { get; set; }
        public string ServicesName { get; set; }
        public decimal ServicesRate { get; set; }
        public Nullable<int> ServicesStatus { get; set; }
        public string PlantName { get; set; }
        public string LineName { get; set; }
        public string SectionName { get; set; }
        public Nullable<int> PlantID { get; set; }
        public string LineID { get; set; }
        public string SectionID { get; set; }
    }
}
