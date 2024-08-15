namespace Plims.Models
{
	public class ResultGrpProductModel
	{
		public string ProductID { get; set; }
		public string ProductName { get; set; }
        public string SectionName { get; set; }
        public double STD { get; set; }
		public double Actual { get; set; }
		public double Diff { get; set; }

	}
}
