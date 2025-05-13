using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plims.Migrations
{
    /// <inheritdoc />
    public partial class Newdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tbShift",
                table: "tbShift");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbService",
                table: "tbService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbSection",
                table: "tbSection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbReason",
                table: "tbReason");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbProduct",
                table: "tbProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbPlant",
                table: "tbPlant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbLine",
                table: "tbLine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbIncentiveMasters",
                table: "tbIncentiveMasters");

            migrationBuilder.RenameTable(
                name: "tbShift",
                newName: "TbShift");

            migrationBuilder.RenameTable(
                name: "tbService",
                newName: "TbService");

            migrationBuilder.RenameTable(
                name: "tbSection",
                newName: "TbSection");

            migrationBuilder.RenameTable(
                name: "tbReason",
                newName: "TbReason");

            migrationBuilder.RenameTable(
                name: "tbProduct",
                newName: "TbProduct");

            migrationBuilder.RenameTable(
                name: "tbPlant",
                newName: "TbPlant");

            migrationBuilder.RenameTable(
                name: "tbLine",
                newName: "TbLine");

            migrationBuilder.RenameTable(
                name: "tbIncentiveMasters",
                newName: "TbIncentiveMaster");

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
                name: "PK_TbReason",
                table: "TbReason",
                column: "ReasonID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbProduct",
                table: "TbProduct",
                column: "ProductID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbPlant",
                table: "TbPlant",
                column: "PlantID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbLine",
                table: "TbLine",
                column: "LineID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TbIncentiveMaster",
                table: "TbIncentiveMaster",
                column: "IncentiveID");

            migrationBuilder.CreateTable(
                name: "TbDefect",
                columns: table => new
                {
                    DefectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DefectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefectPlant = table.Column<int>(type: "int", nullable: false),
                    DefectLine = table.Column<int>(type: "int", nullable: false),
                    DefectSection = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbDefect", x => x.DefectID);
                });

            migrationBuilder.CreateTable(
                name: "TbEmployeeMaster",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    LineID = table.Column<int>(type: "int", nullable: false),
                    SectionID = table.Column<int>(type: "int", nullable: false),
                    ShiftID = table.Column<int>(type: "int", nullable: false),
                    QRCodePerUnit = table.Column<int>(type: "int", nullable: false),
                    QRCodePerEmployee = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbEmployeeMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TbEmployeeTransaction",
                columns: table => new
                {
                    TransactionNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Shift = table.Column<int>(type: "int", nullable: false),
                    Line = table.Column<int>(type: "int", nullable: false),
                    Section = table.Column<int>(type: "int", nullable: false),
                    WorkingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClockOut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbEmployeeTransaction", x => x.TransactionNo);
                });

            migrationBuilder.CreateTable(
                name: "TbPage",
                columns: table => new
                {
                    PageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PageStatus = table.Column<int>(type: "int", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbPage", x => x.PageID);
                });

            migrationBuilder.CreateTable(
                name: "TbPermission",
                columns: table => new
                {
                    PermissionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    PageID = table.Column<int>(type: "int", nullable: false),
                    RoleAction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbPermission", x => x.PermissionID);
                });

            migrationBuilder.CreateTable(
                name: "TbPLPS",
                columns: table => new
                {
                    PLPSID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantID = table.Column<int>(type: "int", nullable: false),
                    LineID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    SectionID = table.Column<int>(type: "int", nullable: false),
                    STD = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PercentSTD = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PercentYield = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QTYPerQRCode = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbPLPS", x => x.PLPSID);
                });

            migrationBuilder.CreateTable(
                name: "TbRole",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleStatus = table.Column<int>(type: "int", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbRole", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "TbUser",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPermission = table.Column<int>(type: "int", nullable: false),
                    UserEmpID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordLastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Lineconcern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbUser", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbDefect");

            migrationBuilder.DropTable(
                name: "TbEmployeeMaster");

            migrationBuilder.DropTable(
                name: "TbEmployeeTransaction");

            migrationBuilder.DropTable(
                name: "TbPage");

            migrationBuilder.DropTable(
                name: "TbPermission");

            migrationBuilder.DropTable(
                name: "TbPLPS");

            migrationBuilder.DropTable(
                name: "TbRole");

            migrationBuilder.DropTable(
                name: "TbUser");

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
                name: "PK_TbReason",
                table: "TbReason");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbProduct",
                table: "TbProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbPlant",
                table: "TbPlant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbLine",
                table: "TbLine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TbIncentiveMaster",
                table: "TbIncentiveMaster");

            migrationBuilder.RenameTable(
                name: "TbShift",
                newName: "tbShift");

            migrationBuilder.RenameTable(
                name: "TbService",
                newName: "tbService");

            migrationBuilder.RenameTable(
                name: "TbSection",
                newName: "tbSection");

            migrationBuilder.RenameTable(
                name: "TbReason",
                newName: "tbReason");

            migrationBuilder.RenameTable(
                name: "TbProduct",
                newName: "tbProduct");

            migrationBuilder.RenameTable(
                name: "TbPlant",
                newName: "tbPlant");

            migrationBuilder.RenameTable(
                name: "TbLine",
                newName: "tbLine");

            migrationBuilder.RenameTable(
                name: "TbIncentiveMaster",
                newName: "tbIncentiveMasters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbShift",
                table: "tbShift",
                column: "ShiftID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbService",
                table: "tbService",
                column: "ServicesID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbSection",
                table: "tbSection",
                column: "SectionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbReason",
                table: "tbReason",
                column: "ReasonID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbProduct",
                table: "tbProduct",
                column: "ProductID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbPlant",
                table: "tbPlant",
                column: "PlantID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbLine",
                table: "tbLine",
                column: "LineID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbIncentiveMasters",
                table: "tbIncentiveMasters",
                column: "IncentiveID");
        }
    }
}
