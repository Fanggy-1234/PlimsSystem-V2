namespace Plims.Models
{
    public class ResultGrpProductOverviewModel
    {

        //Display Box
        public int SumEmp { get; set; } //View_EFFReport  => sum(CountQRCode)
		public int SumFG { get; set; }
		public decimal CapHr { get; set; } //View_EFFReport  => sum(FinidGood)/sum(CountQRCode)


        public decimal EFFhr1 { get; set; } //View_EFFReport.EFFhr1
        public decimal EFFhr2 { get; set; } //View_EFFReport.EFFhr2
        public decimal EFFhr3 { get; set; } //View_EFFReport.EFFhr3

        //1st Graph
        public decimal EffSTD { get; set; } //View_EFFReport.PercentSTD
        public decimal EffLine { get; set; } //View_EFFReport.EFF3


        //2nd Graph
        public decimal YieldSTD { get; set; } //View_EFFReport.PercentYield
        public decimal YieldDefect { get; set; } //View_EFFReport.Defect

        //Table
        public string ProductName { get; set; } //View_EFFReport.ProductName
        public string SectionName { get; set; } //View_EFFReport.ProductName
        public decimal EffTarget { get; set; } //View_EFFReport.PercentSTD
        public decimal EffAct { get; set; } //View_EFFReport.EFFhr3
        public decimal DiffEff { get; set; } //View_EFFReport => EFFhr3 - PercentSTD
        public decimal YieldTarget { get; set; } //View_EFFReport.PercentYield
        public decimal YieldActual { get; set; } //View_EFFReport.YieldDefect
        public decimal DiffYield { get; set; } //View_EFFReport => YieldDefect - PercentYield



    }
}
