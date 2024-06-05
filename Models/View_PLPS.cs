using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_PLPS
    {
        [Key]
        public string PLPSID { get; set; }
        public string PlantName { get; set; }
        public string ProductName { get; set; }
        public string SectionName { get; set; }
        public string LineName { get; set; }
        public string Size { get; set; }
        public Nullable<int> QTYPerQRCode { get; set; }
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
        public int FormularID { get; set; }
        public string FormularName { get; set; }
        public string Delaytime { get; set; }



    }
}
