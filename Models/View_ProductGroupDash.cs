﻿using System.ComponentModel.DataAnnotations;

namespace Plims.Models
{
    public class View_ProductGroupDash
    {

        public DateTime TransactionDate { get; set; }
        [Key]
        public int PlantID { get; set; }
        public string PlantName { get; set; }
        [Key]
        public string LineID { get; set; }
        public string LineName { get; set; }
        [Key]
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        [Key]
        public string SectionID { get; set; }
        public string SectionName { get; set; }
        public decimal STD { get; set; }
        public decimal Actual { get; set; }
        public decimal Diff { get; set; }
    }
}