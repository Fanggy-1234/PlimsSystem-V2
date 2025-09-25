using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace Plims.Models
{
    public class FinanceReportRow
    {

        public DateTime TransactionDate { get; set; }   // เดิม DateTime -> DateTime?
        public int PlantID { get; set; }           // เดิม int -> int?
        public string LineID { get; set; }
        public string LineName { get; set; }
        public string SectionID { get; set; }
        public string SectionName { get; set; }
        public string QRCode { get; set; }
        public string EmployeeName { get; set; }
        public decimal? Incentive { get; set; }         // เดิม decimal -> decimal?
        public string Type { get; set; }
    }
}
