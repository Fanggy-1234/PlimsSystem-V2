namespace Plims.Models
{
    public class ResultGrpLineOverviewModel
    {


        public string LineID { get; set; }
        public string LineName { get; set; }

        //1st Graph
        public decimal EffSTD { get; set; } //View_EFFReport.PercentSTD
        public decimal EffLine { get; set; } //View_EFFReport.EFF3

        //2nd Graph
        public decimal YieldSTD { get; set; } //View_EFFReport.PercentYield
        public decimal YieldDefect { get; set; } //View_EFFReport.Defect

    }
}
