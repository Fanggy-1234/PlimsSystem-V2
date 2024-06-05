using Microsoft.EntityFrameworkCore;
using Plims.Models;
using System.ComponentModel.DataAnnotations;

namespace Plims.ViewModel
{
    public class ViewModelReport
    {

        /// Ref Start Var Temp for filter 
        public int FilterYear { get; set; }
        public int FilterMonth { get; set; }
        public string FilterLine { get; set; }
        public string FilterProduct { get; set; }
        public string FilterPoint { get; set; }

        public string FilterEmp { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        //public int filter { get; set; }

		public List<ResultGrpProductModel> ResultGrpProduct { get; set; }
		public List<ResultGrpGradeModel> ResultGrpGrade { get; set; }

        public List<ResultGrpProductOverviewModel> ResultGrpProductOverviewModel { get; set; }

        public List<ResultGrpLineOverviewModel> ResultGrpLineOverviewModel { get; set; }


        // Ref  view_PermissionMaster
        public IEnumerable<View_PermissionMaster> view_PermissionMaster { get; set; }

        // Ref Start View_DailyReportSummary
        public IEnumerable<View_DailyReportSummary> view_DailyReportSummary { get; set; }

        // Ref Start View_EFFReport
        public IEnumerable<View_EFFReport> View_EFFReport { get; set; }


    }
}
