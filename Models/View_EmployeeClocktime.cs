﻿using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_EmployeeClocktime
    {
        [Key]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }
        [Key]
        public Nullable<int> TransactionNo { get; set; }

        [Key]
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public int Status { get; set; }
        public int PlantID { get; set; }
        public string PlantName { get; set; }

        public string LineID { get; set; }
        public string LineName { get; set; }
        public string SectionID { get; set; }
        public string SectionName { get; set; }
        public int ShiftID { get; set; }
        public string ShiftName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }


        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public string WorkingStatus { get; set; }
        public int QRCodePerEmployee { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }

        public string Remark { get; set; }
        public string BreakFlag { get; set; }

       // public string qrcode { get; set; }

        //  public Nullable<int> QRCodePerUnit { get; set; }




    }
}
