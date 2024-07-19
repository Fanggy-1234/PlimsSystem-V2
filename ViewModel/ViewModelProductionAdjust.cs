using Plims.Models;

namespace Plims.ViewModel
{
    public class ViewModelProductionAdjust
    {

        public IEnumerable<TbPlant> tbPlants { get; set; }
        public IEnumerable<TbLine> tbLine { get; set; }
        public IEnumerable<TbProduct> tbProduct { get; set; }

        public IEnumerable<TbSection> tbSection { get; set; }

        public IEnumerable<TbShift> tbShift { get; set; }

        public IEnumerable<TbEmployeeMaster> tbEmployeeMaster { get; set; }

        public IEnumerable<View_PermissionMaster> view_PermissionMaster { get; set; }

        public IEnumerable<View_ProductionTransactionAdjust> view_ProductionTransactionAdjust { get; set; }

        public IEnumerable<View_ProductionTransactionAj> view_ProductionTransactionAj { get; set; }
       
    }
}
