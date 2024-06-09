using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plims.Migrations
{
    /// <inheritdoc />
    public partial class firstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_View_ServicesClocktime",
                table: "View_ServicesClocktime");

            migrationBuilder.DropPrimaryKey(
                name: "PK_View_PermissionMaster",
                table: "View_PermissionMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_View_PagePermission",
                table: "View_PagePermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbShift",
                table: "TbShift");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbService",
                table: "TbService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbSection",
                table: "TbSection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbRole",
                table: "TbRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbReason",
                table: "TbReason");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbProductSTD",
                table: "TbProductSTD");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbProduct",
                table: "TbProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbPLPS",
                table: "TbPLPS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbLine",
                table: "TbLine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbIncentiveMaster",
                table: "TbIncentiveMaster");

            migrationBuilder.DropColumn(
                name: "QRCodePerUnit",
                table: "View_ServicesClocktime");

            migrationBuilder.DropColumn(
                name: "EmployeeID",
                table: "View_PermissionMaster");

            migrationBuilder.DropColumn(
                name: "LineName",
                table: "View_PermissionMaster");

            migrationBuilder.DropColumn(
                name: "PlantName",
                table: "View_PermissionMaster");

            migrationBuilder.DropColumn(
                name: "UserPermission",
                table: "View_PermissionMaster");

            migrationBuilder.DropColumn(
                name: "QRCodePerUnit",
                table: "View_EmployeeClocktime");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "View_Employee");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "View_Employee");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                table: "View_Employee");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "View_Employee");

            migrationBuilder.RenameColumn(
                name: "SectionName",
                table: "View_PermissionMaster",
                newName: "RoleAction");

            migrationBuilder.RenameColumn(
                name: "Ser_PlantID",
                table: "TbService",
                newName: "PlantID");

            migrationBuilder.RenameColumn(
                name: "Ser_LineID",
                table: "TbService",
                newName: "SectionID");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "View_ServicesClocktime",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "SectionID",
                table: "View_ServicesClocktime",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServicesID",
                table: "View_ServicesClocktime",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "View_ServicesClocktime",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlantID",
                table: "View_ServicesClocktime",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "View_ServicesClocktime",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServicesName",
                table: "View_ServicesClocktime",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ServicesID",
                table: "View_Service",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "SectionID",
                table: "View_Service",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SectionName",
                table: "View_Service",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "View_Reason",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductID",
                table: "View_Reason",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReasonID",
                table: "View_Reason",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "ProductID",
                table: "View_ProductSTD",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ProductSTDID",
                table: "View_ProductSTD",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<decimal>(
                name: "EFFSTD",
                table: "View_ProductSTD",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "YieldIncentive",
                table: "View_ProductSTD",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductID",
                table: "View_PLPS",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PLPSID",
                table: "View_PLPS",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Delaytime",
                table: "View_PLPS",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FormularID",
                table: "View_PLPS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FormularName",
                table: "View_PLPS",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserEmpID",
                table: "View_PermissionMaster",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "View_PermissionMaster",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlantID",
                table: "View_PermissionMaster",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PageID",
                table: "View_PermissionMaster",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "View_PermissionMaster",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "RoleID",
                table: "View_PermissionMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "PermissionID",
                table: "View_PagePermission",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "PlantID",
                table: "View_PagePermission",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoleID",
                table: "View_PagePermission",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ProductID",
                table: "View_Incentive",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "IncentiveID",
                table: "View_Incentive",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "STD",
                table: "View_Incentive",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeLastName",
                table: "View_EmployeeMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EndTime",
                table: "View_EmployeeMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "View_EmployeeMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PlantID",
                table: "View_EmployeeMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SectionID",
                table: "View_EmployeeMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ShiftID",
                table: "View_EmployeeMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StartTime",
                table: "View_EmployeeMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "View_EmployeeClocktime",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "View_EmployeeClocktime",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "QRCodePerEmployee",
                table: "View_EmployeeClocktime",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "View_EmployeeClocktime",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BreakFlag",
                table: "View_EmployeeClocktime",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "View_EmployeeClocktime",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PlantID",
                table: "View_EmployeeClocktime",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "View_EmployeeClocktime",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SectionID",
                table: "View_EmployeeClocktime",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftID",
                table: "TbShift",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "PlantID",
                table: "TbShift",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                table: "TbShift",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ServicesID",
                table: "TbService",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "TbService",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PlantID",
                table: "TbSection",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "RoleID",
                table: "TbRole",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "TbRole",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "PlantID",
                table: "TbRole",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ProductID",
                table: "TbReason",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ProductID",
                table: "TbProductSTD",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "EFFSTD",
                table: "TbProductSTD",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "YieldIncentive",
                table: "TbProductSTD",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "ProductID",
                table: "TbProduct",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "TbProduct",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "PlantID",
                table: "TbProduct",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ProductID",
                table: "TbPLPS",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PLPSID",
                table: "TbPLPS",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "FormularID",
                table: "TbPLPS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlantID",
                table: "TbPermission",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlantID",
                table: "TbPage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "LineID",
                table: "TbLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "TbLine",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "PlantID",
                table: "TbLine",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                table: "TbIncentiveMaster",
                type: "decimal(18,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "ProductID",
                table: "TbIncentiveMaster",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Min",
                table: "TbIncentiveMaster",
                type: "decimal(18,3)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Max",
                table: "TbIncentiveMaster",
                type: "decimal(18,3)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "IncentiveID",
                table: "TbIncentiveMaster",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "STD",
                table: "TbIncentiveMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Section",
                table: "TbEmployeeTransaction",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Line",
                table: "TbEmployeeTransaction",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "BreakFlag",
                table: "TbEmployeeTransaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EndTime",
                table: "TbEmployeeTransaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Plant",
                table: "TbEmployeeTransaction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                table: "TbEmployeeTransaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "TbEmployeeTransaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartTime",
                table: "TbEmployeeTransaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "SectionID",
                table: "TbEmployeeMaster",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "LineID",
                table: "TbEmployeeMaster",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "GroupID",
                table: "TbEmployeeGroupQR",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "TbEmployeeGroupQR",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PlantID",
                table: "TbEmployeeGroupQR",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SectionID",
                table: "TbEmployeeGroupQR",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_View_ServicesClocktime",
                table: "View_ServicesClocktime",
                columns: new[] { "ID", "SectionID", "ServicesID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_View_PermissionMaster",
                table: "View_PermissionMaster",
                columns: new[] { "PageID", "PlantID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_View_PagePermission",
                table: "View_PagePermission",
                columns: new[] { "PermissionID", "PlantID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbShift",
                table: "TbShift",
                columns: new[] { "ShiftID", "PlantID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbService",
                table: "TbService",
                columns: new[] { "ServicesID", "PlantID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbSection",
                table: "TbSection",
                columns: new[] { "SectionID", "PlantID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbRole",
                table: "TbRole",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbReason",
                table: "TbReason",
                columns: new[] { "ReasonID", "PlantID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbProductSTD",
                table: "TbProductSTD",
                columns: new[] { "ProductSTDID", "PlantID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbProduct",
                table: "TbProduct",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbPLPS",
                table: "TbPLPS",
                columns: new[] { "PLPSID", "PlantID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbLine",
                table: "TbLine",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbIncentiveMaster",
                table: "TbIncentiveMaster",
                columns: new[] { "IncentiveID", "PlantID" });

            migrationBuilder.CreateTable(
                name: "TbEmployeeLeaveHoliday",
                columns: table => new
                {
                    TransactionNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftID = table.Column<int>(type: "int", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    LineID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockOut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbEmployeeLeaveHoliday", x => x.TransactionNo);
                });

            migrationBuilder.CreateTable(
                name: "TbFormular",
                columns: table => new
                {
                    FormularID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormularName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbFormular", x => x.FormularID);
                });

            migrationBuilder.CreateTable(
                name: "TbPackage",
                columns: table => new
                {
                    PackageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PackageQTY = table.Column<int>(type: "int", nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbPackage", x => x.PackageID);
                });

            migrationBuilder.CreateTable(
                name: "TbPageMaster",
                columns: table => new
                {
                    PageNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbPageMaster", x => x.PageNo);
                });

            migrationBuilder.CreateTable(
                name: "TbProductionPlan",
                columns: table => new
                {
                    TransactionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SizeMin = table.Column<int>(type: "int", nullable: false),
                    SizeMax = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    QRcodeperday = table.Column<int>(type: "int", nullable: false),
                    TotalPiecePerDay = table.Column<int>(type: "int", nullable: false),
                    QTY = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbProductionPlan", x => x.TransactionID);
                });

            migrationBuilder.CreateTable(
                name: "TbProductionTransaction",
                columns: table => new
                {
                    TransactionNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    LineID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    QtyPerQR = table.Column<int>(type: "int", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PackageRef = table.Column<int>(type: "int", nullable: false),
                    EmployeeRef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroupRef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbProductionTransaction", x => x.TransactionNo);
                });

            migrationBuilder.CreateTable(
                name: "TbProductionTransactionAdjust",
                columns: table => new
                {
                    TransactionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    LineID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QTY = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbProductionTransactionAdjust", x => x.TransactionID);
                });

            migrationBuilder.CreateTable(
                name: "TbServicesTransaction",
                columns: table => new
                {
                    TransactionNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Plant = table.Column<int>(type: "int", nullable: false),
                    Shift = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Line = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServicesName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServicesID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockOut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusClocktime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbServicesTransaction", x => x.TransactionNo);
                });

            migrationBuilder.CreateTable(
                name: "TbSetup",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Valuesetup = table.Column<int>(type: "int", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbSetup", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Temp_Group",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temp_Group", x => new { x.ID, x.EmployeeID });
                });

            migrationBuilder.CreateTable(
                name: "View_ClockTime",
                columns: table => new
                {
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    LineID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockOut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View_ClockTime", x => new { x.TransactionDate, x.PlantID, x.LineID, x.EmployeeID, x.SectionID, x.Type });
                });

            migrationBuilder.CreateTable(
                name: "View_DailyReport",
                columns: table => new
                {
                    TransactionNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    PlantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    QtyPerQR = table.Column<int>(type: "int", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View_DailyReport", x => x.TransactionNo);
                });

            migrationBuilder.CreateTable(
                name: "View_DailyReportSummary",
                columns: table => new
                {
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    LineID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountQty = table.Column<int>(type: "int", nullable: false),
                    DefectQty = table.Column<int>(type: "int", nullable: false),
                    FG_Count_Qty = table.Column<int>(type: "int", nullable: false),
                    YieldDefect = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    QtyPerQR = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    STD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PercentSTD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Min = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Max = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ClockIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockOut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiffHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PcePerHr = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PiecePerHr = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffManPerSTD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FGQty = table.Column<int>(type: "int", nullable: false),
                    wage = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TotalDefect = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualFG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FGAdjust = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View_DailyReportSummary", x => new { x.TransactionDate, x.PlantID, x.LineID, x.QRCode, x.ProductID, x.SectionID });
                });

            migrationBuilder.CreateTable(
                name: "View_EFFReport",
                columns: table => new
                {
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    LineID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountQRCode = table.Column<int>(type: "int", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActualworkhourService = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductSTD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PercentSTD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PercentYield = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WorkinghourSTD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinishGood = table.Column<int>(type: "int", nullable: false),
                    Defect = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EFF1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WorkinghourACT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Servicehour = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Supporthour = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EFF2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EFF3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EFFhr1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EFFhr2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EFFhr3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    YieldDefect = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MEDEFF3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValueEFF3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KPIh3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MEDh3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValEffh3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KPIh1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MEDh1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValEffh1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDefect = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "View_EmployeeAdjustBreak",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockOut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BreakFlag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QRCodePerEmployee = table.Column<int>(type: "int", nullable: true),
                    TransactionNo = table.Column<int>(type: "int", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View_EmployeeAdjustBreak", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "View_EmployeeAdjustLine",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    LineID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromLine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromSectionID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromSection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockOut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QRCodePerEmployee = table.Column<int>(type: "int", nullable: true),
                    TransactionNo = table.Column<int>(type: "int", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View_EmployeeAdjustLine", x => new { x.ID, x.SectionID, x.LineID, x.PlantID });
                });

            migrationBuilder.CreateTable(
                name: "View_EmployeeGroup",
                columns: table => new
                {
                    GroupID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View_EmployeeGroup", x => new { x.GroupID, x.EmployeeID, x.PlantID });
                });

            migrationBuilder.CreateTable(
                name: "View_EmployeeGroupList",
                columns: table => new
                {
                    GroupID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    EmployeeIDs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeNames = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View_EmployeeGroupList", x => x.GroupID);
                });

            migrationBuilder.CreateTable(
                name: "View_EmployeeLeaveHolidayClocktime",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockOut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QRCodePerEmployee = table.Column<int>(type: "int", nullable: true),
                    TransactionNo = table.Column<int>(type: "int", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View_EmployeeLeaveHolidayClocktime", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "View_FinancialReport",
                columns: table => new
                {
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    LineID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QTYPerQRCode = table.Column<int>(type: "int", nullable: false),
                    FormularID = table.Column<int>(type: "int", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalCountQty = table.Column<int>(type: "int", nullable: false),
                    TotalFGQty = table.Column<int>(type: "int", nullable: false),
                    TotalQty = table.Column<int>(type: "int", nullable: false),
                    Defect = table.Column<int>(type: "int", nullable: false),
                    ProductSTD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PercentSTD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PercentYield = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClockIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockOut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionDateTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hourSinceClockIn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShiftHour = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PiecePerHr = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncentiveID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IncentiveName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Min = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Max = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    STDincentive = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Incentive = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffManPerSTD = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View_FinancialReport", x => new { x.TransactionDate, x.PlantID, x.LineID, x.QRCode, x.ProductID, x.SectionID });
                });

            migrationBuilder.CreateTable(
                name: "View_ProductionPlan",
                columns: table => new
                {
                    TransactionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    PlantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SizeMin = table.Column<int>(type: "int", nullable: false),
                    SizeMax = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    QRcodeperday = table.Column<int>(type: "int", nullable: false),
                    TotalPiecePerDay = table.Column<int>(type: "int", nullable: false),
                    QTY = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View_ProductionPlan", x => x.TransactionID);
                });

            migrationBuilder.CreateTable(
                name: "View_ProductionTransaction",
                columns: table => new
                {
                    TransactionNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    LineID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    QtyPerQR = table.Column<int>(type: "int", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PackageRef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeRef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View_ProductionTransaction", x => x.TransactionNo);
                });

            migrationBuilder.CreateTable(
                name: "View_ProductionTransactionAdjust",
                columns: table => new
                {
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    LineID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TransactionID = table.Column<long>(type: "bigint", nullable: false),
                    PlantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkHr = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QtyPerQR = table.Column<int>(type: "int", nullable: false),
                    CountQty = table.Column<int>(type: "int", nullable: false),
                    FGInputQty = table.Column<int>(type: "int", nullable: false),
                    DefectQty = table.Column<int>(type: "int", nullable: false),
                    MinusQty = table.Column<int>(type: "int", nullable: false),
                    FG = table.Column<int>(type: "int", nullable: false),
                    TotalPiece = table.Column<int>(type: "int", nullable: false),
                    Yield = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View_ProductionTransactionAdjust", x => new { x.TransactionDate, x.PlantID, x.LineID, x.QRCode, x.ProductID, x.SectionID });
                });

            migrationBuilder.CreateTable(
                name: "View_RollBackData",
                columns: table => new
                {
                    RunningNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    LineID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndTime = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View_RollBackData", x => x.RunningNumber);
                });

            migrationBuilder.CreateTable(
                name: "View_Temp_Group",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeLastName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View_Temp_Group", x => new { x.ID, x.EmployeeID });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbEmployeeLeaveHoliday");

            migrationBuilder.DropTable(
                name: "TbFormular");

            migrationBuilder.DropTable(
                name: "TbPackage");

            migrationBuilder.DropTable(
                name: "TbPageMaster");

            migrationBuilder.DropTable(
                name: "TbProductionPlan");

            migrationBuilder.DropTable(
                name: "TbProductionTransaction");

            migrationBuilder.DropTable(
                name: "TbProductionTransactionAdjust");

            migrationBuilder.DropTable(
                name: "TbServicesTransaction");

            migrationBuilder.DropTable(
                name: "TbSetup");

            migrationBuilder.DropTable(
                name: "Temp_Group");

            migrationBuilder.DropTable(
                name: "View_ClockTime");

            migrationBuilder.DropTable(
                name: "View_DailyReport");

            migrationBuilder.DropTable(
                name: "View_DailyReportSummary");

            migrationBuilder.DropTable(
                name: "View_EFFReport");

            migrationBuilder.DropTable(
                name: "View_EmployeeAdjustBreak");

            migrationBuilder.DropTable(
                name: "View_EmployeeAdjustLine");

            migrationBuilder.DropTable(
                name: "View_EmployeeGroup");

            migrationBuilder.DropTable(
                name: "View_EmployeeGroupList");

            migrationBuilder.DropTable(
                name: "View_EmployeeLeaveHolidayClocktime");

            migrationBuilder.DropTable(
                name: "View_FinancialReport");

            migrationBuilder.DropTable(
                name: "View_ProductionPlan");

            migrationBuilder.DropTable(
                name: "View_ProductionTransaction");

            migrationBuilder.DropTable(
                name: "View_ProductionTransactionAdjust");

            migrationBuilder.DropTable(
                name: "View_RollBackData");

            migrationBuilder.DropTable(
                name: "View_Temp_Group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_View_ServicesClocktime",
                table: "View_ServicesClocktime");

            migrationBuilder.DropPrimaryKey(
                name: "PK_View_PermissionMaster",
                table: "View_PermissionMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_View_PagePermission",
                table: "View_PagePermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbShift",
                table: "TbShift");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbService",
                table: "TbService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbSection",
                table: "TbSection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbRole",
                table: "TbRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbReason",
                table: "TbReason");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbProductSTD",
                table: "TbProductSTD");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbProduct",
                table: "TbProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbPLPS",
                table: "TbPLPS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbLine",
                table: "TbLine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbIncentiveMaster",
                table: "TbIncentiveMaster");

            migrationBuilder.DropColumn(
                name: "SectionID",
                table: "View_ServicesClocktime");

            migrationBuilder.DropColumn(
                name: "ServicesID",
                table: "View_ServicesClocktime");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "View_ServicesClocktime");

            migrationBuilder.DropColumn(
                name: "PlantID",
                table: "View_ServicesClocktime");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "View_ServicesClocktime");

            migrationBuilder.DropColumn(
                name: "ServicesName",
                table: "View_ServicesClocktime");

            migrationBuilder.DropColumn(
                name: "SectionID",
                table: "View_Service");

            migrationBuilder.DropColumn(
                name: "SectionName",
                table: "View_Service");

            migrationBuilder.DropColumn(
                name: "EFFSTD",
                table: "View_ProductSTD");

            migrationBuilder.DropColumn(
                name: "YieldIncentive",
                table: "View_ProductSTD");

            migrationBuilder.DropColumn(
                name: "Delaytime",
                table: "View_PLPS");

            migrationBuilder.DropColumn(
                name: "FormularID",
                table: "View_PLPS");

            migrationBuilder.DropColumn(
                name: "FormularName",
                table: "View_PLPS");

            migrationBuilder.DropColumn(
                name: "RoleID",
                table: "View_PermissionMaster");

            migrationBuilder.DropColumn(
                name: "PlantID",
                table: "View_PagePermission");

            migrationBuilder.DropColumn(
                name: "RoleID",
                table: "View_PagePermission");

            migrationBuilder.DropColumn(
                name: "STD",
                table: "View_Incentive");

            migrationBuilder.DropColumn(
                name: "EmployeeLastName",
                table: "View_EmployeeMaster");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "View_EmployeeMaster");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "View_EmployeeMaster");

            migrationBuilder.DropColumn(
                name: "PlantID",
                table: "View_EmployeeMaster");

            migrationBuilder.DropColumn(
                name: "SectionID",
                table: "View_EmployeeMaster");

            migrationBuilder.DropColumn(
                name: "ShiftID",
                table: "View_EmployeeMaster");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "View_EmployeeMaster");

            migrationBuilder.DropColumn(
                name: "BreakFlag",
                table: "View_EmployeeClocktime");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "View_EmployeeClocktime");

            migrationBuilder.DropColumn(
                name: "PlantID",
                table: "View_EmployeeClocktime");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "View_EmployeeClocktime");

            migrationBuilder.DropColumn(
                name: "SectionID",
                table: "View_EmployeeClocktime");

            migrationBuilder.DropColumn(
                name: "PlantID",
                table: "TbShift");

            migrationBuilder.DropColumn(
                name: "Prefix",
                table: "TbShift");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "TbService");

            migrationBuilder.DropColumn(
                name: "PlantID",
                table: "TbSection");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "TbRole");

            migrationBuilder.DropColumn(
                name: "PlantID",
                table: "TbRole");

            migrationBuilder.DropColumn(
                name: "EFFSTD",
                table: "TbProductSTD");

            migrationBuilder.DropColumn(
                name: "YieldIncentive",
                table: "TbProductSTD");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "TbProduct");

            migrationBuilder.DropColumn(
                name: "PlantID",
                table: "TbProduct");

            migrationBuilder.DropColumn(
                name: "FormularID",
                table: "TbPLPS");

            migrationBuilder.DropColumn(
                name: "PlantID",
                table: "TbPermission");

            migrationBuilder.DropColumn(
                name: "PlantID",
                table: "TbPage");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "TbLine");

            migrationBuilder.DropColumn(
                name: "PlantID",
                table: "TbLine");

            migrationBuilder.DropColumn(
                name: "STD",
                table: "TbIncentiveMaster");

            migrationBuilder.DropColumn(
                name: "BreakFlag",
                table: "TbEmployeeTransaction");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "TbEmployeeTransaction");

            migrationBuilder.DropColumn(
                name: "Plant",
                table: "TbEmployeeTransaction");

            migrationBuilder.DropColumn(
                name: "Prefix",
                table: "TbEmployeeTransaction");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "TbEmployeeTransaction");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "TbEmployeeTransaction");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "TbEmployeeGroupQR");

            migrationBuilder.DropColumn(
                name: "PlantID",
                table: "TbEmployeeGroupQR");

            migrationBuilder.DropColumn(
                name: "SectionID",
                table: "TbEmployeeGroupQR");

            migrationBuilder.RenameColumn(
                name: "RoleAction",
                table: "View_PermissionMaster",
                newName: "SectionName");

            migrationBuilder.RenameColumn(
                name: "SectionID",
                table: "TbService",
                newName: "Ser_LineID");

            migrationBuilder.RenameColumn(
                name: "PlantID",
                table: "TbService",
                newName: "Ser_PlantID");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "View_ServicesClocktime",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "QRCodePerUnit",
                table: "View_ServicesClocktime",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ServicesID",
                table: "View_Service",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "View_Reason",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProductID",
                table: "View_Reason",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ReasonID",
                table: "View_Reason",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "ProductID",
                table: "View_ProductSTD",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ProductSTDID",
                table: "View_ProductSTD",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "ProductID",
                table: "View_PLPS",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "PLPSID",
                table: "View_PLPS",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "UserEmpID",
                table: "View_PermissionMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "View_PermissionMaster",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "View_PermissionMaster",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "PlantID",
                table: "View_PermissionMaster",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PageID",
                table: "View_PermissionMaster",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeID",
                table: "View_PermissionMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LineName",
                table: "View_PermissionMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlantName",
                table: "View_PermissionMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserPermission",
                table: "View_PermissionMaster",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PermissionID",
                table: "View_PagePermission",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "ProductID",
                table: "View_Incentive",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "IncentiveID",
                table: "View_Incentive",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "View_EmployeeClocktime",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "View_EmployeeClocktime",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "QRCodePerEmployee",
                table: "View_EmployeeClocktime",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "View_EmployeeClocktime",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "QRCodePerUnit",
                table: "View_EmployeeClocktime",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "View_Employee",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "View_Employee",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateBy",
                table: "View_Employee",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "View_Employee",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShiftID",
                table: "TbShift",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "ServicesID",
                table: "TbService",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "RoleID",
                table: "TbRole",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "ProductID",
                table: "TbReason",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ProductID",
                table: "TbProductSTD",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ProductID",
                table: "TbProduct",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ProductID",
                table: "TbPLPS",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "PLPSID",
                table: "TbPLPS",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "LineID",
                table: "TbLine",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                table: "TbIncentiveMaster",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");

            migrationBuilder.AlterColumn<int>(
                name: "ProductID",
                table: "TbIncentiveMaster",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Min",
                table: "TbIncentiveMaster",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Max",
                table: "TbIncentiveMaster",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IncentiveID",
                table: "TbIncentiveMaster",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Section",
                table: "TbEmployeeTransaction",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Line",
                table: "TbEmployeeTransaction",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "SectionID",
                table: "TbEmployeeMaster",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "LineID",
                table: "TbEmployeeMaster",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "GroupID",
                table: "TbEmployeeGroupQR",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_View_ServicesClocktime",
                table: "View_ServicesClocktime",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_View_PermissionMaster",
                table: "View_PermissionMaster",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_View_PagePermission",
                table: "View_PagePermission",
                column: "PermissionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbShift",
                table: "TbShift",
                column: "ShiftID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbService",
                table: "TbService",
                column: "ServicesID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbSection",
                table: "TbSection",
                column: "SectionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbRole",
                table: "TbRole",
                column: "RoleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbReason",
                table: "TbReason",
                column: "ReasonID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbProductSTD",
                table: "TbProductSTD",
                column: "ProductSTDID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbProduct",
                table: "TbProduct",
                column: "ProductID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbPLPS",
                table: "TbPLPS",
                column: "PLPSID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbLine",
                table: "TbLine",
                column: "LineID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbIncentiveMaster",
                table: "TbIncentiveMaster",
                column: "IncentiveID");
        }
    }
}
