using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_ProductSTD
    {
        [Key]
        public string ProductSTDID { get; set; }
        public string PlantName { get; set; }
        public string ProductName { get; set; }
        public string SectionName { get; set; }
        public string LineName { get; set; }
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public Nullable<decimal> STD { get; set; }
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public Nullable<decimal> PercentSTD { get; set; }
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public Nullable<decimal> PercentYield { get; set; }

        public Nullable<decimal> YieldIncentive { get; set; }
        public Nullable<decimal> EFFSTD { get; set; }
        public string Unit { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateBy { get; set; }
        public int PlantID { get; set; }
        public string LineID { get; set; }
        //public int ProdID { get; set; }
        public string ProductID { get; set; }
        public string SectionID { get; set; }
    }
}
