namespace Plims.Models
{
	public class ResultGrpProductModel
	{
		public string ProductID { get; set; }
		public string ProductName { get; set; }
        public string SectionName { get; set; }
        public int STD { get; set; }
		public int Actual { get; set; }
		public int Diff { get; set; }

	}
}
