using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_Reason
    {
        [Key]   
        public string ReasonID { get; set; }
        public string ReasonName { get; set; }
        public string PlantName { get; set; }
        public string LineName { get; set; }
        public string ProductName { get; set; }
        public string SectionName { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<int> PlantID { get; set; }
        public string LineID { get; set; }
        //public int ProdID { get; set; }
        public string ProductID { get; set; }
        public string SectionID { get; set; }
       
    }
}
