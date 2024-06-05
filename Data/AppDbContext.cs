using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using Plims.Models;
using System.ComponentModel.DataAnnotations;

namespace Plims.Data
{
    

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TbPlant> TbPlant { get; set; }
        public DbSet<TbLine> TbLine { get; set; }
        public DbSet<TbProduct> TbProduct { get; set; }

        public DbSet<TbSection> TbSection { get; set; }

        public DbSet<TbReason> TbReason { get; set; }
        public DbSet<TbShift> TbShift { get; set; }
        public DbSet<TbIncentiveMaster> TbIncentiveMaster { get; set; }

        public DbSet<TbService> TbService { get; set; }
        public DbSet<TbDefect> TbDefect { get; set; }
        public DbSet<TbEmployeeMaster> TbEmployeeMaster { get; set; }

        public DbSet<TbEmployeeTransaction> TbEmployeeTransaction { get; set; }

        public DbSet<TbPage> TbPage { get; set; }
        public DbSet<TbPermission> TbPermission { get; set; }
        public DbSet<TbPLPS> TbPLPS { get; set; }

        public DbSet<TbRole> TbRole { get; set; }

        public DbSet<TbUser> TbUser { get; set; }
        public DbSet<TbProductSTD> TbProductSTD { get; set; }

        public DbSet<TbEmployeeGroupQR> TbEmployeeGroupQR { get; set; }
        public DbSet<TbServicesTransaction> TbServicesTransaction { get; set; }

        public DbSet<TbEmployeeLeaveHoliday> TbEmployeeLeaveHoliday { get; set; }

        public DbSet<TbSetup> TbSetup { get; set; }
        public DbSet<TbProductionTransaction> TbProductionTransaction { get; set; }
        public DbSet<TbFormular> TbFormular { get; set; }
        public DbSet<TbPackage> TbPackage { get; set; }

        public DbSet<TbPageMaster> TbPageMaster { get; set; }

        public DbSet<TbProductionPlan> TbProductionPlan { get; set; }
        public DbSet<View_Employee> View_Employee { get; set; }
        public DbSet<View_EmployeeMaster> View_EmployeeMaster { get; set; }
        public DbSet<View_Incentive> View_Incentive { get; set; }
        public DbSet<View_Service> View_Service { get; set; }

        public DbSet<View_Reason> View_Reason { get; set; }

        public DbSet<View_PLPS> View_PLPS { get; set; }

        public DbSet<View_User> View_User { get; set; }
        public DbSet<View_PermissionMaster> View_PermissionMaster { get; set; }
        public DbSet<View_PagePermission> View_PagePermission { get; set; }

        public DbSet<View_ProductSTD> View_ProductSTD { get; set; }

        public DbSet<View_EmployeeClocktime>  View_EmployeeClocktime { get; set; }
        public DbSet<View_ServicesClocktime> View_ServicesClocktime { get; set; }
        public DbSet<View_EmployeeGroup> View_EmployeeGroup { get; set; }
        public DbSet<View_EmployeeGroupList> View_EmployeeGroupList { get; set; }

        public DbSet<Temp_Group> Temp_Group { get; set; }
        public DbSet<View_Temp_Group> View_Temp_Group { get; set; }

        public DbSet<View_EmployeeLeaveHolidayClocktime> View_EmployeeLeaveHolidayClocktime { get; set; }
        public DbSet<View_EmployeeAdjustLine> View_EmployeeAdjustLine { get; set; }

        public DbSet<View_EmployeeAdjustBreak> View_EmployeeAdjustBreak { get; set; }
        public DbSet<View_ProductionTransaction> View_ProductionTransaction { get; set; }

        public DbSet<View_DailyReport> View_DailyReport { get; set; }
        public DbSet<View_DailyReportSummary> View_DailyReportSummary { get; set; }

        public DbSet<View_FinancialReport> View_FinancialReport { get; set; }

        public DbSet<View_EFFReport> View_EFFReport { get; set; }


        public DbSet<View_ProductionPlan> View_ProductionPlan { get; set; }
        public DbSet<View_RollBackData> View_RollBackData { get; set; }

        public DbSet<View_ClockTime> View_ClockTime { get; set; }



        public DbSet<View_ProductionTransactionAdjust> View_ProductionTransactionAdjust { get; set; }

        public DbSet<TbProductionTransactionAdjust> TbProductionTransactionAdjust { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<View_PermissionMaster>()
                .HasKey(v => new { v.PageID, v.UserEmpID });

            modelBuilder.Entity<Temp_Group>()
                .HasKey(v => new { v.ID, v.EmployeeID });

            modelBuilder.Entity<View_Temp_Group>()
              .HasKey(v => new { v.ID, v.EmployeeID });


            modelBuilder.Entity<View_EmployeeGroup>()
              .HasKey(v => new { v.GroupID, v.EmployeeID, v.PlantID });


            //ID and plant ID key
            //modelBuilder.Entity<TbLine>()
            // .HasKey(v => new { v.ID, v.LineID, v.PlantID });
            //modelBuilder.Entity<TbProduct>()
            //.HasKey(v => new { v.ID, v.ProductID, v.PlantID });
            modelBuilder.Entity<TbSection>()
           .HasKey(v => new { v.SectionID, v.PlantID });
            modelBuilder.Entity<TbShift>()
           .HasKey(v => new { v.ShiftID, v.PlantID });
            modelBuilder.Entity<TbReason>()
          .HasKey(v => new { v.ReasonID, v.PlantID });
            modelBuilder.Entity<TbProductSTD>()
        .HasKey(v => new { v.ProductSTDID, v.PlantID });
            modelBuilder.Entity<TbPLPS>()
       .HasKey(v => new { v.PLPSID, v.PlantID });
            modelBuilder.Entity<TbIncentiveMaster>()
            .HasKey(v => new { v.IncentiveID, v.PlantID });
            modelBuilder.Entity<TbService>()
          .HasKey(v => new { v.ServicesID, v.PlantID });


            //            modelBuilder.Entity<TbEmployeeMaster>()
            //.HasKey(v => new { v.ID, v.PlantID ,v.EmployeeID  });
            modelBuilder.Entity<View_EmployeeAdjustLine>()
                  .HasKey(v => new { v.ID, v.SectionID, v.LineID , v.PlantID });


            modelBuilder.Entity<View_ServicesClocktime>()
           .HasKey(v => new { v.ID, v.SectionID, v.ServicesID });

            modelBuilder.Entity<View_DailyReportSummary>()
        .HasKey(v => new { v.TransactionDate, v.PlantID, v.LineID, v.QRCode,v.ProductID, v.SectionID });

            modelBuilder.Entity<View_FinancialReport>()
.HasKey(v => new { v.TransactionDate, v.PlantID, v.LineID, v.QRCode, v.ProductID, v.SectionID });


            modelBuilder.Entity<View_ClockTime>()
.HasKey(v => new { v.TransactionDate, v.PlantID, v.LineID, v.EmployeeID, v.SectionID ,v.Type});

            modelBuilder.Entity<View_ProductionTransactionAdjust>()
  .HasKey(v => new { v.TransactionDate, v.PlantID, v.LineID, v.QRCode, v.ProductID, v.SectionID });

            //            modelBuilder.Entity<View_EFFReport>()
            //.HasKey(v => new { v.TransactionDate, v.PlantID, v.LineID,  v.ProductID, v.SectionID });

            modelBuilder.Entity<View_EFFReport>().HasNoKey();


            modelBuilder.Entity<View_PermissionMaster>()
 .HasKey(v => new { v.PageID, v.PlantID });

            modelBuilder.Entity<View_PagePermission>()
.HasKey(v => new { v.PermissionID, v.PlantID });
            // Other configurations if needed

            base.OnModelCreating(modelBuilder);
        }

    }

}
