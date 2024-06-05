using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_Incentive
    {
        [Key]
        public string IncentiveID { get; set; }
        public string IncentiveName { get; set; }
        public string PlantName { get; set; }
        public string LineName { get; set; }
        public string ProductName { get; set; }
        public string SectionName { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> STD { get; set; }
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public Nullable<decimal> Min { get; set; }
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public Nullable<decimal> Max { get; set; }
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public Nullable<decimal> Rate { get; set; }
        public string Grade { get; set; }

        public int PlantID { get; set; }
        public string LineID { get; set; }

        //public int ProdID { get; set; }
        public string ProductID { get; set; }
        public string SectionID { get; set; }
    }
}
