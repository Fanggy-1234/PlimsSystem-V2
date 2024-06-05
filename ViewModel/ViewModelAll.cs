using Microsoft.EntityFrameworkCore;
using Plims.Models;
using System.ComponentModel.DataAnnotations;

namespace Plims.ViewModel
{
    public class ViewModelAll
    {
        public IEnumerable<TbPlant> tbPlants { get; set; }
        public IEnumerable<TbLine> tbLine { get; set; }
        public IEnumerable<TbProduct> tbProduct { get; set; }
        public IEnumerable<TbService> tbService { get; set; }
        public IEnumerable<TbSection> tbSection { get; set; }
        public IEnumerable<TbIncentiveMaster> tbIncentiveMaster { get; set; }
        public IEnumerable<TbEmployeeMaster> tbEmployeeMaster { get; set; }

        public IEnumerable<TbUser> tbUser { get; set; }

        public IEnumerable<TbShift> tbShift { get; set; }
        public IEnumerable<TbEmployeeTransaction> tbEmployeeTransaction { get; set; }

        public IEnumerable<TbPermission> tbPermission { get; set; }
        public IEnumerable<TbRole> tbRole { get; set; }

        public IEnumerable<TbReason> tbReason { get; set; }

        public IEnumerable<TbPLPS> tbPLPS { get; set; }

        public IEnumerable<TbPage> tbPage { get; set; }

        public IEnumerable<TbProductSTD> tbProductSTD { get; set; }

        public IEnumerable<TbEmployeeGroupQR> tbEmployeeGroupQR { get; set; }

        public IEnumerable<Temp_Group> temp_Group { get; set; }

        public IEnumerable<TbServicesTransaction> tbServicesTransaction { get; set; }

        public IEnumerable<TbEmployeeLeaveHoliday> tbEmployeeLeaveHoliday{ get; set;}

        public IEnumerable<TbProductionTransaction> tbProductionTransaction { get; set; }

        public IEnumerable<TbProductionPlan> tbProductionPlan { get; set; }
        public IEnumerable<TbSetup> tbSetup { get; set; }
        public IEnumerable<TbFormular> tbFormular { get; set; }
        public IEnumerable<TbPackage> tbPackage { get; set; }

        public IEnumerable<TbPageMaster> tbPageMaster { get; set; }

        public IEnumerable<View_User> view_User { get; set; }
        public IEnumerable<View_Incentive> view_Incentive { get; set; }
       public IEnumerable<View_PLPS> view_PLPS { get; set; }

        public IEnumerable<View_EmployeeClocktime> view_EmployeeClocktime { get; set; }
        public IEnumerable<View_ServicesClocktime> view_ServicesClocktime { get; set; }
        public IEnumerable<View_Reason> view_Reason { get; set; }
        public IEnumerable<View_PermissionMaster> view_PermissionMaster { get; set; }
        public IEnumerable<View_Service> view_Service { get; set; }

        public IEnumerable<View_PagePermission> view_PagePermission { get; set; }
        public IEnumerable<View_EmployeeMaster> view_EmployeeMaster { get; set; }
        public IEnumerable<View_Employee> view_Employee { get; set; }
        public IEnumerable<View_ProductSTD> view_ProductSTD { get; set; }
        public IEnumerable<View_EmployeeGroup> view_EmployeeGroup { get; set; }
        public IEnumerable<View_EmployeeGroupList> view_EmployeeGroupList { get; set; }
        public IEnumerable<View_Temp_Group> view_Temp_Group { get; set; }

        public IEnumerable<View_EmployeeLeaveHolidayClocktime> view_EmployeeLeaveHolidayClocktime { get; set; }
        public IEnumerable<View_EmployeeAdjustLine> view_EmployeeAdjustLine { get; set; }
        public IEnumerable<View_EmployeeAdjustBreak> view_EmployeeAdjustBreak { get; set; }
        public IEnumerable<View_ProductionTransaction> view_ProductionTransaction { get; set; }

        public IEnumerable<View_DailyReport> view_DailyReport { get; set; }
        public IEnumerable<View_DailyReportSummary> view_DailyReportSummary { get; set; }

        public IEnumerable<View_FinancialReport> view_FinancialReport { get; set; }
        public IEnumerable<View_EFFReport> view_EFFReport { get; set; }

        public IEnumerable<View_ProductionPlan> view_ProductionPlan { get; set; }

        public IEnumerable<View_RollBackData> View_RollBackData { get; set; }

        public IEnumerable<View_ClockTime> view_ClockTime { get; set; }

        public IEnumerable<View_ProductionTransactionAdjust> view_ProductionTransactionAdjust { get; set; }

        public IEnumerable<TbProductionTransactionAdjust> tbProductionTransactionAdjust { get; set; }
        public List<ViewModelAll> Permissions { get; set; }
        public string PageName { get; set; }
        public string PermissionValue { get; set; }
        public string RoleID { get; set; }

        public string RoleName { get; set; }
        public string RoleNames { get; set; }

        public IFormFile ImageFile { get; set; }
        
        public string GroupID { get; set; }
        public string EmployeeID { get; set; }
        public List<Dictionary<string, string>> TableData { get; set; }

        [Display(Name = "Enter QRCode Text")]
        public string QRCodeText { get; set; }
        public bool InactiveStatus { get; set; }


        //Use in Financial Report

        public List<GroupedFinancialData> groupedData { get; set; }

    }

    //// Define the structure of each group
    public class GroupedFinancialData
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }
        public string QRCode { get; set; }
        public string EmployeeName { get; set; }

        public decimal TotalIncentive { get; set; }

        public string SectionName { get; set; }
 
    }


}
