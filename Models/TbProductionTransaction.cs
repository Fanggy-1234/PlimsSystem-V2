using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class TbProductionTransaction
    {
        [Key]
        public int TransactionNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public int PlantID { get; set; }
        public string LineID { get; set; }
        public string SectionID { get; set; }
        //public int ProdID { get; set; }
        public string ProductID { get; set; }
        public string Prefix { get; set; }
        public string QRCode { get; set; }
        public int Qty { get; set; }
        public int QtyPerQR { get; set; }

        public string DataType { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
        public int PackageRef { get; set; }
        public string EmployeeRef { get; set; }
        public string GroupRef { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate{ get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
