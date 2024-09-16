using Plims.Models;
using System.ComponentModel.DataAnnotations;

namespace Plims.ViewModel
{
    public class ViewModelDaily
    {

        public IEnumerable<TbPlant> tbPlants { get; set; }
        public IEnumerable<TbLine> tbLine { get; set; }
        public IEnumerable<TbProduct> tbProduct { get; set; }
        public IEnumerable<TbSection> tbSection { get; set; }
        public IEnumerable<TbEmployeeMaster> tbEmployeeMaster { get; set; }
        public IEnumerable<TbShift> tbShift { get; set; }

        public IEnumerable<View_DailyReportSummary> view_DailyReportSummary { get; set; }
        public IEnumerable<View_PermissionMaster> view_PermissionMaster { get; set; }

    }
}
