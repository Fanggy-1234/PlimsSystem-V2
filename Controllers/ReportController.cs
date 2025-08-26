using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Plims.Data;
using Plims.Models;
using Plims.ViewModel;
using IronBarCode;
using System.Data;
using QRCoder;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.InkML;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Dynamic;
using System.Web.WebPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DocumentFormat.OpenXml.Office2016.Drawing.Charts;

using Newtonsoft.Json;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Plims.Controllers
{
    public class ReportController : Controller
    {
        private readonly AppDbContext db;

        public ReportController(AppDbContext _db)
        {
            db = _db;
        }

        [HttpGet]
        public async Task<ActionResult> EmployeeDashBaord(ViewModelReport model)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.DefaultStartDate = DateTime.Now.ToString("dd-MM-yyyy");
                ViewBag.DefaultEndDate = DateTime.Now.ToString("dd-MM-yyyy");

                if (model.filter == 0)
                {
                    model.StartDate = DateTime.Today;
                    model.EndDate = DateTime.Today;
                }

                List<View_DailyReportSummary> view_DailyReportSummaryData = new List<View_DailyReportSummary>();

                try
                {
                    view_DailyReportSummaryData = await GetEmployeeDashboardDataAsync(
                        PlantID, model.StartDate, model.EndDate, model.FilterYear, model.FilterMonth,
                        model.FilterLine, model.FilterProduct, model.FilterPoint);
                }
                catch
                {
                    view_DailyReportSummaryData = new List<View_DailyReportSummary>();
                    TempData["AlertMessage"] = "Working function is currently in use. Please try again later.";
                }

                var mymodel = new ViewModelReport
                {
                    view_DailyReportSummary = view_DailyReportSummaryData,
                };

                var currentYear = DateTime.Now.Year;
                var yearList = new List<SelectListItem>
                {
                    new SelectListItem { Value = currentYear.ToString(), Text = currentYear.ToString() },
                    new SelectListItem { Value = (currentYear - 1).ToString(), Text = (currentYear - 1).ToString() }
                };
                ViewBag.varYear = new SelectList(yearList, "Value", "Text");

                var monthList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "January" },
                    new SelectListItem { Value = "2", Text = "February" },
                    new SelectListItem { Value = "3", Text = "March" },
                    new SelectListItem { Value = "4", Text = "April" },
                    new SelectListItem { Value = "5", Text = "May" },
                    new SelectListItem { Value = "6", Text = "June" },
                    new SelectListItem { Value = "7", Text = "July" },
                    new SelectListItem { Value = "8", Text = "August" },
                    new SelectListItem { Value = "9", Text = "September" },
                    new SelectListItem { Value = "10", Text = "October" },
                    new SelectListItem { Value = "11", Text = "November" },
                    new SelectListItem { Value = "12", Text = "December" }
                };
                ViewBag.varMonth = new SelectList(monthList, "Value", "Text");

                var lineList = await db.TbLine
                                    .Where(x => x.PlantID == PlantID && x.Status == 1)
                                    .Select(x => new SelectListItem 
                                    { 
                                        Value = x.LineID, 
                                        Text = x.LineName 
                                    })
                                    .Distinct()
                                    .OrderBy(x => x.Text)
                                    .AsNoTracking()
                                    .ToListAsync();
                ViewBag.varLine = new SelectList(lineList, "Value", "Text");

                var productList = await db.TbProduct
                                        .Where(x => x.PlantID == PlantID && x.Status == 1)
                                        .Select(x => new SelectListItem 
                                        { 
                                            Value = x.ProductID, 
                                            Text = x.ProductName 
                                        })
                                        .Distinct()
                                        .OrderBy(x => x.Text)
                                        .AsNoTracking()
                                        .ToListAsync();
                ViewBag.varProduct = new SelectList(productList, "Value", "Text");

                var sectionList = await db.TbSection
                                        .Where(x => x.PlantID == PlantID && x.Status == 1)
                                        .Select(x => new SelectListItem 
                                        { 
                                            Value = x.SectionID, 
                                            Text = x.SectionName 
                                        })
                                        .Distinct()
                                        .OrderBy(x => x.Text)
                                        .AsNoTracking()
                                        .ToListAsync();
                ViewBag.varPoint = new SelectList(sectionList, "Value", "Text");

                var sumGrpEmp = from count in mymodel.view_DailyReportSummary
                                where (model.FilterYear == 0 || count.TransactionDate.Year == model.FilterYear) &&
                                    (model.FilterMonth == 0 || count.TransactionDate.Month == model.FilterMonth) &&
                                    (model.FilterLine == null || count.LineID == model.FilterLine) &&
                                    (model.FilterProduct == null || count.ProductID == model.FilterProduct) &&
                                    (model.FilterPoint == null || count.SectionID == model.FilterPoint) &&
                                    (model.StartDate == DateTime.MinValue || count.TransactionDate >= model.StartDate) && (model.EndDate == DateTime.MinValue || count.TransactionDate <= model.EndDate)
                                group count by count.QRCode into grouped
                                select new
                                {
                                    QRCode = grouped.Key,
                                    Cnt = grouped.Count()
                                };

                var sumEmployeeDict = sumGrpEmp.ToDictionary(item => item.QRCode, item => item.Cnt);
                ViewBag.SumEmployee = sumEmployeeDict.Count();

                var resultGrpProduct = (from summary in mymodel.view_DailyReportSummary
                                        where (model.FilterYear == 0 || summary.TransactionDate.Year == model.FilterYear) &&
                                            (model.FilterMonth == 0 || summary.TransactionDate.Month == model.FilterMonth) &&
                                            (model.FilterLine == null || summary.LineID == model.FilterLine) &&
                                            (model.FilterProduct == null || summary.ProductID == model.FilterProduct) &&
                                            (model.FilterPoint == null || summary.SectionID == model.FilterPoint) &&
                                            (model.StartDate == DateTime.MinValue || summary.TransactionDate >= model.StartDate) && (model.EndDate == DateTime.MinValue || summary.TransactionDate <= model.EndDate) &&
                                            (summary.PlantID == PlantID)
                                        group summary by new { summary.ProductID, summary.ProductName, summary.SectionName, summary.STD } into grouped
                                        select new ResultGrpProductModel
                                        {
                                            ProductID = grouped.Key.ProductID,
                                            ProductName = grouped.Key.ProductName,
                                            SectionName = grouped.Key.SectionName,
                                            STD = Convert.ToDouble(grouped.Key.STD),
                                            Actual = Convert.ToDouble(grouped.Sum(x => x.FGQty) / grouped.Sum(x => x.DiffHours)),
                                            Diff = Convert.ToDouble(((grouped.Sum(x => x.FGQty) / grouped.Sum(x => x.DiffHours)) - grouped.Key.STD) / grouped.Key.STD * 100)
                                        }).ToList();

                var sumGrpGrade = (from count in mymodel.view_DailyReportSummary
                                where (model.FilterYear == 0 || count.TransactionDate.Year == model.FilterYear) &&
                                        (model.FilterMonth == 0 || count.TransactionDate.Month == model.FilterMonth) &&
                                        (model.FilterLine == null || count.LineID == model.FilterLine) &&
                                        (model.FilterProduct == null || count.ProductID == model.FilterProduct) &&
                                        (model.FilterPoint == null || count.SectionID == model.FilterPoint) &&
                                        (model.StartDate == DateTime.MinValue || count.TransactionDate >= model.StartDate) && (model.EndDate == DateTime.MinValue || count.TransactionDate <= model.EndDate) &&
                                        (count.PlantID == PlantID)
                                group count by count.Grade into grouped
                                select new
                                {
                                    Grade = grouped.Key,
                                    CountSum = grouped.Count()
                                }).ToList();

                int sumOfCounts = sumGrpGrade.Sum(item => item.CountSum);

                var resultGrpGrade = (from count in mymodel.view_DailyReportSummary
                                    where (model.FilterYear == 0 || count.TransactionDate.Year == model.FilterYear) &&
                                            (model.FilterMonth == 0 || count.TransactionDate.Month == model.FilterMonth) &&
                                            (model.FilterLine == null || count.LineID == model.FilterLine) &&
                                            (model.FilterProduct == null || count.ProductID == model.FilterProduct) &&
                                            (model.FilterPoint == null || count.SectionID == model.FilterPoint) &&
                                            (model.StartDate == DateTime.MinValue || count.TransactionDate >= model.StartDate) && (model.EndDate == DateTime.MinValue || count.TransactionDate <= model.EndDate)
                                    group count by count.Grade into grouped
                                    select new ResultGrpGradeModel
                                    {
                                        Grade = grouped.Key,
                                        Cnt = grouped.Count(),
                                        FGQty = grouped.Sum(x => x.FGQty),
                                        DiffHours = grouped.Sum(x => x.DiffHours),
                                        PcsPerHr = grouped.Sum(x => x.FGQty) / grouped.Sum(x => x.DiffHours),
                                        Percent = Math.Round((grouped.Count() / (double)sumOfCounts) * 100.00, 2)
                                    }).OrderBy(x => x.Grade).ToList();

                List<string> grades = resultGrpGrade.Select(x => x.Grade).ToList();
                List<int> counts = resultGrpGrade.Select(x => x.Cnt).ToList();
                List<double> percents = resultGrpGrade.Select(x => x.Percent).ToList();

                //Set data pie
                List<object[]> chartData = new List<object[]>();
                for (int i = 0; i < grades.Count; i++)
                {
                    chartData.Add(new object[] { grades[i], percents[i], grades[i] });
                }

                string chartDataJson = JsonConvert.SerializeObject(chartData);

                ViewBag.ChartDataJson = chartDataJson;

                string chartDataString = string.Join(",", chartData.Select(data => $"[{string.Join(",", data.Select(x => "\"" + x + "\""))}]"));
                ViewBag.GrdJoin = chartDataString;

                mymodel = new ViewModelReport
                {
                    view_PermissionMaster = db.View_PermissionMaster.ToList(),
                    view_DailyReportSummary = mymodel.view_DailyReportSummary.Where(x => x.TransactionDate >= model.StartDate && x.TransactionDate <= model.EndDate).ToList(),
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    FilterYear = model.FilterYear,
                    FilterMonth = model.FilterMonth,
                    FilterLine = string.IsNullOrEmpty(model.FilterLine) ? null : model.FilterLine,
                    FilterProduct = string.IsNullOrEmpty(model.FilterProduct) ? null : model.FilterProduct,
                    FilterPoint = string.IsNullOrEmpty(model.FilterPoint) ? null : model.FilterPoint,
                    ResultGrpProduct = resultGrpProduct,
                    ResultGrpGrade = resultGrpGrade
                };

                ViewBag.VBRoleEmployeeDashBaord = db.View_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(23)).Select(x => x.RoleAction).FirstOrDefault();

                ////Set Refresh Time
                int Valuesetup = db.TbSetup.Where(x => x.PlantID == PlantID).Select(x => x.Valuesetup).FirstOrDefault();
                ViewBag.SetTime = Valuesetup * 60000; //Change minute to millisecond

                return View(mymodel);
            }
        }

        private async Task<List<View_DailyReportSummary>> GetEmployeeDashboardDataAsync(
            int plantId, DateTime startDate, DateTime endDate,int filterYear, int filterMonth,
            string lineId, string productId, string sectionId,
            CancellationToken ct = default)
        {

            if (filterYear > 0 || filterMonth > 0)
            {
                if (filterYear > 0 && filterMonth > 0)
                {
                    startDate = new DateTime(filterYear, filterMonth, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                }
                else if (filterYear > 0)
                {
                    startDate = new DateTime(filterYear, 1, 1);
                    endDate = new DateTime(filterYear, 12, 31);
                }
            }

            var chunks = BuildDateChunks(startDate, endDate, DAILY_REPORT_CHUNK_DAYS);

            var prevTimeout = db.Database.GetCommandTimeout();
            db.Database.SetCommandTimeout(TimeSpan.FromSeconds(180));

            try
            {
                var buffer = new List<View_DailyReportSummary>(capacity: 4096);
                foreach (var (cs, ce) in chunks)
                {
                    var q = db.View_DailyReportSummary
                        .AsNoTracking()
                        .Where(x => x.PlantID == plantId
                                    && x.Grade != "X"
                                    && x.TransactionDate >= cs
                                    && x.TransactionDate <= ce);

                    if (!string.IsNullOrEmpty(lineId)) q = q.Where(x => x.LineID == lineId);
                    if (!string.IsNullOrEmpty(productId)) q = q.Where(x => x.ProductID == productId);
                    if (!string.IsNullOrEmpty(sectionId)) q = q.Where(x => x.SectionID == sectionId);

                    var list = await q.ToListAsync(ct);
                    if (list.Count > 0) buffer.AddRange(list);
                }

                var dedup = buffer
                    .GroupBy(x => new { x.TransactionDate, x.PlantID, x.LineID, x.SectionID, x.ProductID, x.QRCode, x.Prefix })
                    .Select(g => g.First())
                    .OrderBy(x => x.TransactionDate).ThenBy(x => x.LineID).ThenBy(x => x.SectionID).ThenBy(x => x.ProductID).ThenBy(x => x.QRCode)
                    .ToList();

                return dedup;
            }
            finally
            {
                db.Database.SetCommandTimeout(prevTimeout);
            }
        }



        [HttpGet]
        public ActionResult EmployeeDashBaordCurr(ViewModelReport model)
        {


            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {

                ViewBag.DefaultStartDate = DateTime.Now.ToString("dd-MM-yyyy");
                ViewBag.DefaultEndDate = DateTime.Now.ToString("dd-MM-yyyy");

                if (model.filter == 0)
                {
                    //model.StartDate = DateTime.Now;
                    //model.EndDate = DateTime.Now;
                    model.StartDate = DateTime.Today;
                    model.EndDate = DateTime.Today;

                }

                var sect = db.View_DailyReportSummary.Where(x => x.PlantID.Equals(PlantID)).ToList();

                var varYear = from a in db.View_DailyReportSummary
                              group a by new { a.TransactionDate.Year } into g
                              select new SelectListItem
                              {
                                  Value = $"{g.Key.Year}",
                                  Text = $"{g.Key.Year}"
                              };
                ViewBag.varYear = new SelectList(varYear, "Value", "Text");

                var varMonth = from a in db.View_DailyReportSummary
                               group a by new { a.TransactionDate.Month } into g
                               select new SelectListItem
                               {
                                   Text = ($"{g.Key.Month}" == "1") ? "January" :
                                           ($"{g.Key.Month}" == "2") ? "February" :
                                           ($"{g.Key.Month}" == "3") ? "March" :
                                           ($"{g.Key.Month}" == "4") ? "April" :
                                           ($"{g.Key.Month}" == "5") ? "May" :
                                           ($"{g.Key.Month}" == "6") ? "June" :
                                           ($"{g.Key.Month}" == "7") ? "July" :
                                           ($"{g.Key.Month}" == "8") ? "August" :
                                           ($"{g.Key.Month}" == "9") ? "September" :
                                           ($"{g.Key.Month}" == "10") ? "October" :
                                           ($"{g.Key.Month}" == "11") ? "November" :
                                           ($"{g.Key.Month}" == "12") ? "December" :
                                   $"{g.Key.Month}",
                                   Value = $"{g.Key.Month}"
                               };

                ViewBag.varMonth = new SelectList(varMonth, "Value", "Text");

                var varLine = from a in db.View_DailyReportSummary
                              where a.PlantID.Equals(PlantID)
                              group a by new { a.LineID, a.LineName } into g
                              select new SelectListItem
                              {
                                  Value = $"{g.Key.LineID}",
                                  Text = $"{g.Key.LineName}"
                              };
                ViewBag.varLine = new SelectList(varLine, "Value", "Text");

                var varProduct = from a in db.View_DailyReportSummary
                                 where a.PlantID.Equals(PlantID)
                                 group a by new { a.ProductID, a.ProductName } into g
                                 select new SelectListItem
                                 {
                                     Value = $"{g.Key.ProductID}",
                                     Text = $"{g.Key.ProductName}"
                                 };
                ViewBag.varProduct = new SelectList(varProduct, "Value", "Text");

                var varPoint = from a in db.View_DailyReportSummary
                               where a.PlantID.Equals(PlantID)
                               group a by new { a.SectionID, a.SectionName } into g
                               select new SelectListItem
                               {
                                   Value = $"{g.Key.SectionID}",
                                   Text = $"{g.Key.SectionName}"
                               };
                ViewBag.varPoint = new SelectList(varPoint, "Value", "Text");


                /////////////////// 1 Count Employee
                var sumGrpEmp = from count in db.View_DailyReportSummary
                                where (model.FilterYear == 0 || count.TransactionDate.Year == model.FilterYear) &&
                                      (model.FilterMonth == 0 || count.TransactionDate.Month == model.FilterMonth) &&
                                      (model.FilterLine == null || count.LineID == model.FilterLine) &&
                                      (model.FilterProduct == null || count.ProductID == model.FilterProduct) &&
                                      (model.FilterPoint == null || count.SectionID == model.FilterPoint) &&
                                      (model.StartDate == DateTime.MinValue || count.TransactionDate >= model.StartDate) && (model.EndDate == DateTime.MinValue || count.TransactionDate <= model.EndDate) &&
                                      (count.PlantID == PlantID)
                                group count by count.QRCode into grouped
                                select new
                                {
                                    QRCode = grouped.Key,
                                    Cnt = grouped.Count()
                                };


                // var sumEmployeeDict = sumGrpEmp.ToDictionary(item => item.QRCode, item => item.Cnt);
                var sumEmployeeDict = sumGrpEmp.Count();
                ViewBag.SumEmployee = sumEmployeeDict; // sumEmployeeDict.Count();


                /////////////////// 2 Group Product 
                var resultGrpProduct = (from summary in db.View_DailyReportSummary
                                        where (model.FilterYear == 0 || summary.TransactionDate.Year == model.FilterYear) &&
                                              (model.FilterMonth == 0 || summary.TransactionDate.Month == model.FilterMonth) &&
                                              (model.FilterLine == null || summary.LineID == model.FilterLine) &&
                                              (model.FilterProduct == null || summary.ProductID == model.FilterProduct) &&
                                              (model.FilterPoint == null || summary.SectionID == model.FilterPoint) &&
                                              (model.StartDate == DateTime.MinValue || summary.TransactionDate >= model.StartDate) && (model.EndDate == DateTime.MinValue || summary.TransactionDate <= model.EndDate) &&
                                               (summary.PlantID == PlantID)
                                        group summary by new { summary.ProductID, summary.ProductName, summary.SectionName, summary.STD } into grouped
                                        select new ResultGrpProductModel
                                        {
                                            ProductID = grouped.Key.ProductID,
                                            ProductName = grouped.Key.ProductName,
                                            SectionName = grouped.Key.SectionName,
                                            STD = Convert.ToDouble(grouped.Key.STD),
                                            Actual = Convert.ToDouble(grouped.Sum(x => x.PcsPerHr)),
                                            Diff = Convert.ToDouble((grouped.Sum(x => x.PcsPerHr) * 100) / grouped.Key.STD)
                                        }).ToList();


                /////////////////// 3 Group Grad 
                var sumGrpGrade = (from count in db.View_DailyReportSummary
                                   where (model.FilterYear == 0 || count.TransactionDate.Year == model.FilterYear) &&
                                         (model.FilterMonth == 0 || count.TransactionDate.Month == model.FilterMonth) &&
                                         (model.FilterLine == null || count.LineID == model.FilterLine) &&
                                         (model.FilterProduct == null || count.ProductID == model.FilterProduct) &&
                                         (model.FilterPoint == null || count.SectionID == model.FilterPoint) &&
                                         (model.StartDate == DateTime.MinValue || count.TransactionDate >= model.StartDate) && (model.EndDate == DateTime.MinValue || count.TransactionDate <= model.EndDate) &&
                                          (count.PlantID == PlantID)
                                   group count by count.Grade into grouped
                                   select new
                                   {
                                       Grade = grouped.Key,
                                       CountSum = grouped.Count()
                                   }).ToList();

                int sumOfCounts = sumGrpGrade.Sum(item => item.CountSum);



                var resultGrpGrade = (from count in db.View_DailyReportSummary
                                      where (model.FilterYear == 0 || count.TransactionDate.Year == model.FilterYear) &&
                                            (model.FilterMonth == 0 || count.TransactionDate.Month == model.FilterMonth) &&
                                            (model.FilterLine == null || count.LineID == model.FilterLine) &&
                                            (model.FilterProduct == null || count.ProductID == model.FilterProduct) &&
                                            (model.FilterPoint == null || count.SectionID == model.FilterPoint) &&
                                            (model.StartDate == DateTime.MinValue || count.TransactionDate >= model.StartDate) && (model.EndDate == DateTime.MinValue || count.TransactionDate <= model.EndDate) &&
                                             (count.PlantID == PlantID)
                                      group count by count.Grade into grouped
                                      select new ResultGrpGradeModel
                                      {
                                          Grade = grouped.Key,
                                          //PcsPerHr = 0,                      
                                          Cnt = grouped.Count(),
                                          PcsPerHr = Math.Round(grouped.Sum(g => g.PcsPerHr), 2),
                                          //PcsPerHr = grouped.Count() != 0 ? (grouped.Sum(g => g.FGQty) / sumEmployeeDict.Count() ) : 0,
                                          Percent = Math.Round((grouped.Count() / (double)sumOfCounts) * 100.00, 2)
                                      }).ToList();


                /////////////////// 4 Show Chart Pie
                //Create separate lists for Grade, Cnt, and Percent
                List<string> grades = resultGrpGrade.Select(x => x.Grade).ToList();
                List<int> counts = resultGrpGrade.Select(x => x.Cnt).ToList();
                List<double> percents = resultGrpGrade.Select(x => x.Percent).ToList();

                //Set data pie
                List<object[]> chartData = new List<object[]>();
                for (int i = 0; i < grades.Count; i++)
                {
                    chartData.Add(new object[] { grades[i], percents[i], grades[i] });
                }

                string chartDataJson = JsonConvert.SerializeObject(chartData);

                // Pass chartDataJson to the ViewBag
                ViewBag.ChartDataJson = chartDataJson;

                // Convert chartData to a string
                string chartDataString = string.Join(",", chartData.Select(data => $"[{string.Join(",", data.Select(x => "\"" + x + "\""))}]"));
                ViewBag.GrdJoin = chartDataString;


                var mymodel = new ViewModelReport
                {
                    view_PermissionMaster = db.View_PermissionMaster.ToList(),
                    view_DailyReportSummary = db.View_DailyReportSummary.Where(x => x.PlantID == PlantID && x.TransactionDate >= model.StartDate && x.TransactionDate <= model.EndDate).ToList(),
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    FilterYear = model.FilterYear,
                    FilterMonth = model.FilterMonth,
                    FilterLine = string.IsNullOrEmpty(model.FilterLine) ? null : model.FilterLine,
                    FilterProduct = string.IsNullOrEmpty(model.FilterProduct) ? null : model.FilterProduct,
                    FilterPoint = string.IsNullOrEmpty(model.FilterPoint) ? null : model.FilterPoint,
                    ResultGrpProduct = resultGrpProduct,
                    ResultGrpGrade = resultGrpGrade
                };

                ViewBag.VBRoleEmployeeDashBaord = db.View_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(23)).Select(x => x.RoleAction).FirstOrDefault();

                ////Set Refrsh Time
                int Valuesetup = db.TbSetup.Where(x => x.PlantID == PlantID).Select(x => x.Valuesetup).FirstOrDefault();
                ViewBag.SetTime = Valuesetup * 60000; //Change minute to millisecond

                //Set Refrsh Time
                //int Valuesetup = db.TbSetup.Where(x => x.PlantID == PlantID).Select(x => x.Valuesetup).FirstOrDefault();
                // ViewBag.SetTime = Valuesetup * 60; //Change minute to millisecond
                // String Refreshtime = Convert.ToString(Valuesetup * 60000);
                //  Response.Headers.Add("Refresh", Refreshtime);


                return View(mymodel);

            }

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter(ViewModelReport mymodel)
        {
           
            if (mymodel.StartDate != DateTime.MinValue || mymodel.EndDate != DateTime.MinValue || mymodel.FilterYear != 0 || mymodel.FilterMonth != 0 || mymodel.FilterLine != null || mymodel.FilterProduct != null || mymodel.FilterPoint != null)
            {
                mymodel.filter = 1;
                return RedirectToAction("EmployeeDashBaord", mymodel);
            }
            else
            {
                mymodel.StartDate = DateTime.Today;
                mymodel.EndDate = DateTime.Today;
                return RedirectToAction("EmployeeDashBaord", mymodel);

            }


        }

        private async Task<(SelectList Year, SelectList Month, SelectList Line, SelectList Product, SelectList Point)>CreateDropdownListsAsync(int plantId)
        {
            var plantData = await db.View_EFFReport
                                .Where(x => x.PlantID == plantId && x.TransactionDate.Year > 2000)
                                .Select(x => new { 
                                    x.TransactionDate, 
                                    x.LineID, 
                                    x.LineName, 
                                    x.ProductID, 
                                    x.ProductName, 
                                    x.SectionID, 
                                    x.SectionName 
                                })
                                .AsNoTracking()
                                .ToListAsync();

            var yearList = plantData
                        .GroupBy(a => a.TransactionDate.Year)
                        .Select(g => new SelectListItem { Value = $"{g.Key}", Text = $"{g.Key}" })
                        .OrderByDescending(x => x.Value)
                        .ToList();

            var monthList = plantData
                        .GroupBy(a => a.TransactionDate.Month)
                        .Select(g => new SelectListItem
                        {
                            Value = $"{g.Key}",
                            Text = g.Key switch
                            {
                                1 => "January", 2 => "February", 3 => "March", 4 => "April",
                                5 => "May", 6 => "June", 7 => "July", 8 => "August",
                                9 => "September", 10 => "October", 11 => "November", 12 => "December",
                                _ => $"{g.Key}"
                            }
                        })
                        .OrderBy(x => int.Parse(x.Value))
                        .ToList();

            var lineList = plantData
                        .GroupBy(a => new { a.LineID, a.LineName })
                        .Select(g => new SelectListItem { Value = g.Key.LineID, Text = g.Key.LineName })
                        .OrderBy(x => x.Text)
                        .ToList();

            var productList = plantData
                            .GroupBy(a => new { a.ProductID, a.ProductName })
                            .Select(g => new SelectListItem { Value = g.Key.ProductID, Text = g.Key.ProductName })
                            .OrderBy(x => x.Text)
                            .ToList();

            var pointList = plantData
                        .GroupBy(a => new { a.SectionID, a.SectionName })
                        .Select(g => new SelectListItem { Value = g.Key.SectionID, Text = g.Key.SectionName })
                        .OrderBy(x => x.Text)
                        .ToList();

            return (
                new SelectList(yearList, "Value", "Text"),
                new SelectList(monthList, "Value", "Text"),
                new SelectList(lineList, "Value", "Text"),
                new SelectList(productList, "Value", "Text"),
                new SelectList(pointList, "Value", "Text")
            );
        }

        [HttpGet]
        public async Task<ActionResult> OverviewDashBoard(ViewModelReport model)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.DefaultStartDate = DateTime.Today.ToString("dd-MM-yyyy");
                ViewBag.DefaultEndDate = DateTime.Today.ToString("dd-MM-yyyy");

                if (model.filter == 0 || model.StartDate == DateTime.MinValue || model.EndDate == DateTime.MinValue)
                {
                    model.StartDate = DateTime.Today;
                    model.EndDate = DateTime.Today;
                }

                List<View_EFFReport> view_EFFReportData = new List<View_EFFReport>();
                List<View_DailyReportSummary> view_DailyReportSummaryData = new List<View_DailyReportSummary>();

                try
                {
                    view_EFFReportData = await GetOverviewEFFDataAsync(PlantID, model.StartDate, model.EndDate, model.FilterLine, model.FilterPoint);
                    view_DailyReportSummaryData = await GetOverviewDailyReportDataAsync(PlantID, model.StartDate, model.EndDate, model.FilterLine, model.FilterPoint);
                }
                catch
                {
                    view_EFFReportData = new List<View_EFFReport>();
                    view_DailyReportSummaryData = new List<View_DailyReportSummary>();
                    TempData["AlertMessage"] = "Working function is currently in use. Please try again later.";
                }

                var mymodel = new ViewModelReport
                {
                    view_EFFReport = view_EFFReportData,
                    view_DailyReportSummary = view_DailyReportSummaryData,
                };

                var (yearDropdown, monthDropdown, lineDropdown, productDropdown, pointDropdown) = await CreateDropdownListsAsync(PlantID);

                ViewBag.varYear = yearDropdown;
                ViewBag.varMonth = monthDropdown;
                ViewBag.varLine = lineDropdown;
                ViewBag.varProduct = productDropdown;
                ViewBag.varPoint = pointDropdown;
                // var varYear = from a in db.View_EFFReport
                //             where a.PlantID == PlantID
                //             group a by new { a.TransactionDate.Year } into g
                //             select new SelectListItem
                //             {
                //                 Value = $"{g.Key.Year}",
                //                 Text = $"{g.Key.Year}"
                //             };
                // ViewBag.varYear = new SelectList(varYear, "Value", "Text");

                // var varMonth = from a in db.View_EFFReport
                //             where a.PlantID == PlantID
                //             group a by new { a.TransactionDate.Month } into g
                //             select new SelectListItem
                //             {
                //                 Text = ($"{g.Key.Month}" == "1") ? "January" :
                //                         ($"{g.Key.Month}" == "2") ? "February" :
                //                         ($"{g.Key.Month}" == "3") ? "March" :
                //                         ($"{g.Key.Month}" == "4") ? "April" :
                //                         ($"{g.Key.Month}" == "5") ? "May" :
                //                         ($"{g.Key.Month}" == "6") ? "June" :
                //                         ($"{g.Key.Month}" == "7") ? "July" :
                //                         ($"{g.Key.Month}" == "8") ? "August" :
                //                         ($"{g.Key.Month}" == "9") ? "September" :
                //                         ($"{g.Key.Month}" == "10") ? "October" :
                //                         ($"{g.Key.Month}" == "11") ? "November" :
                //                         ($"{g.Key.Month}" == "12") ? "December" :
                //                 $"{g.Key.Month}",
                //                 Value = $"{g.Key.Month}"
                //             };

                // ViewBag.varMonth = new SelectList(varMonth, "Value", "Text");

                // var varLine = from a in db.View_EFFReport
                //             where a.PlantID.Equals(PlantID)
                //             group a by new { a.LineID, a.LineName } into g
                //             select new SelectListItem
                //             {
                //                 Value = $"{g.Key.LineID}",
                //                 Text = $"{g.Key.LineName}"
                //             };
                // ViewBag.varLine = new SelectList(varLine, "Value", "Text");

                // var varProduct = from a in db.View_EFFReport
                //                 where a.PlantID.Equals(PlantID)
                //                 group a by new { a.ProductID, a.ProductName } into g
                //                 select new SelectListItem
                //                 {
                //                     Value = $"{g.Key.ProductID}",
                //                     Text = $"{g.Key.ProductName}"
                //                 };
                // ViewBag.varProduct = new SelectList(varProduct, "Value", "Text");

                // var varPoint = from a in db.View_EFFReport
                //             where a.PlantID.Equals(PlantID)
                //             group a by new { a.SectionID, a.SectionName } into g
                //             select new SelectListItem
                //             {
                //                 Value = $"{g.Key.SectionID}",
                //                 Text = $"{g.Key.SectionName}"
                //             };
                // ViewBag.varPoint = new SelectList(varPoint, "Value", "Text");

                /////////////////// 1 Count Employee
                var sumGrpEmp = from count in mymodel.view_DailyReportSummary
                                where (model.FilterYear == 0 || count.TransactionDate.Year == model.FilterYear) &&
                                    (model.FilterMonth == 0 || count.TransactionDate.Month == model.FilterMonth) &&
                                    (model.FilterLine == null || count.LineID == model.FilterLine) &&
                                    (model.FilterProduct == null || count.ProductID == model.FilterProduct) &&
                                    (model.FilterPoint == null || count.SectionID == model.FilterPoint) &&
                                    (model.StartDate == DateTime.MinValue || count.TransactionDate >= model.StartDate) && (model.EndDate == DateTime.MinValue || count.TransactionDate <= model.EndDate)
                                group count by count.QRCode into grouped
                                select new
                                {
                                    QRCode = grouped.Key,
                                    Cnt = grouped.Count()
                                };

                var sumEmployeeDict = sumGrpEmp.ToDictionary(item => item.QRCode, item => item.Cnt);
                ViewBag.SumEmployee = sumEmployeeDict.Count();

                var resultGrpProductOverview = (from summary in mymodel.view_EFFReport
                                                where (model.FilterYear == 0 || summary.TransactionDate.Year == model.FilterYear) &&
                                                    (model.FilterMonth == 0 || summary.TransactionDate.Month == model.FilterMonth) &&
                                                    (model.FilterLine == null || summary.LineID == model.FilterLine) &&
                                                    (model.FilterProduct == null || summary.ProductID == model.FilterProduct) &&
                                                    (model.FilterPoint == null || summary.SectionID == model.FilterPoint) &&
                                                    (model.StartDate == DateTime.MinValue || summary.TransactionDate >= model.StartDate) &&
                                                    (model.EndDate == DateTime.MinValue || summary.TransactionDate <= model.EndDate) &&
                                                    (summary.PlantID == PlantID)
                                                group summary by new { summary.ProductID, summary.ProductName, summary.SectionID, summary.SectionName } into grouped
                                                select new ResultGrpProductOverviewModel
                                                {
                                                    //Display Box
                                                    SumEmp = grouped.Sum(x => x.CountQRCode),
                                                    SumFG = grouped.Sum(x => x.FinishGood),
                                                    CapHr = grouped.Sum(x => x.EFF3) != 0 ? (grouped.Sum(x => x.FinishGood) - grouped.Sum(x => x.TotalDefect)) / grouped.Sum(x => x.EFF3) : 0,
                                                    EFFhr1 = grouped.Sum(x => x.FinishGood) / grouped.Sum(x => x.EFF1),
                                                    EFFhr2 = grouped.Sum(x => x.FinishGood) / grouped.Sum(x => x.EFF2),
                                                    EFFhr3 = grouped.Sum(x => x.FinishGood) / grouped.Sum(x => x.EFF3),
                                                    TotalDefect = grouped.Sum(x => x.TotalDefect),

                                                    //1st Graph
                                                    EffSTD = grouped.Average(x => x.STD),
                                                    EffLine = grouped.Sum(x => x.ValueEFF3),

                                                    //2nd Graph
                                                    YieldSTD = grouped.Average(x => x.PercentYield),
                                                    YieldDefect = (grouped.Sum(x => x.FinishGood) - grouped.Sum(x => x.TotalDefect)) / grouped.Sum(x => x.FinishGood) * 100,

                                                    //Table
                                                    ProductName = grouped.Key.ProductName,
                                                    SectionName = grouped.Key.SectionName,
                                                    EffTarget = grouped.Max(x => x.EFFSTD),
                                                    EffAct = grouped.Max(x => x.WorkinghourACT),
                                                    DiffEff = ((grouped.Sum(x => x.FinishGood) / grouped.Sum(x => x.EFF1)) - grouped.Max(x => x.EFFSTD)) / grouped.Max(x => x.EFFSTD) * 100,
                                                    YieldTarget = grouped.Max(x => x.PercentYield),
                                                    YieldActual = (grouped.Sum(x => x.FinishGood) - grouped.Sum(x => x.TotalDefect)) / grouped.Sum(x => x.FinishGood) * 100,
                                                    DiffYield = grouped.Max(x => x.PercentYield) - ((grouped.Sum(x => x.FinishGood) - grouped.Sum(x => x.TotalDefect)) / grouped.Sum(x => x.FinishGood) * 100)
                                                }).ToList();

                if (resultGrpProductOverview.Count == 0)
                {
                    ViewBag.SumEmployee = 0;
                    ViewBag.SumCapHr = 0;
                    ViewBag.SumEFFhr1 = 0;
                    ViewBag.SumEFFhr2 = 0;
                    ViewBag.SumEFFhr3 = 0;
                    ViewBag.DefectAll = 0;
                }
                else
                {
                    ViewBag.SumCapHr = resultGrpProductOverview.Sum(x => x.CapHr);
                    ViewBag.SumEFFhr1 = resultGrpProductOverview.Sum(x => x.EFFhr1);
                    ViewBag.SumEFFhr2 = resultGrpProductOverview.Sum(x => x.EFFhr2);
                    ViewBag.SumEFFhr3 = resultGrpProductOverview.Sum(x => x.EFFhr3);
                    ViewBag.DefectAll = resultGrpProductOverview.Sum(x => x.TotalDefect);
                }

                /////////////////// 2 Group Bar Chart Line Overview
                var resultGrpLineOverviewtest = (from summary in mymodel.view_EFFReport
                                                where (model.FilterYear == 0 || summary.TransactionDate.Year == model.FilterYear) &&
                                                    (model.FilterMonth == 0 || summary.TransactionDate.Month == model.FilterMonth) &&
                                                    (model.FilterLine == null || summary.LineID == model.FilterLine) &&
                                                    (model.FilterProduct == null || summary.ProductID == model.FilterProduct) &&
                                                    (model.FilterPoint == null || summary.SectionID == model.FilterPoint) &&
                                                    (model.StartDate == DateTime.MinValue || summary.TransactionDate >= model.StartDate) &&
                                                    (model.EndDate == DateTime.MinValue || summary.TransactionDate <= model.EndDate) &&
                                                        (summary.PlantID == PlantID)
                                                select new ResultGrpLineOverviewModel
                                                {
                                                    LineID = summary.LineID
                                                }).ToList();

                var resultGrpLineOverview = (from summary in mymodel.view_EFFReport
                                            where (model.FilterYear == 0 || summary.TransactionDate.Year == model.FilterYear) &&
                                                (model.FilterMonth == 0 || summary.TransactionDate.Month == model.FilterMonth) &&
                                                (model.FilterLine == null || summary.LineID == model.FilterLine) &&
                                                (model.FilterProduct == null || summary.ProductID == model.FilterProduct) &&
                                                (model.FilterPoint == null || summary.SectionID == model.FilterPoint) &&
                                                (model.StartDate == DateTime.MinValue || summary.TransactionDate >= model.StartDate) &&
                                                (model.EndDate == DateTime.MinValue || summary.TransactionDate <= model.EndDate) &&
                                                    (summary.PlantID == PlantID)
                                            group summary by new { summary.LineID, summary.LineName } into grouped
                                            select new ResultGrpLineOverviewModel
                                            {
                                                //Title
                                                LineID = grouped.Key.LineID,
                                                LineName = grouped.Key.LineName,

                                                //1st Graph
                                                EffSTD = grouped.Average(x => x.EFFSTD),
                                                EffLine = grouped.Sum(x => x.EFFhr1),

                                                //2nd Graph
                                                YieldSTD = grouped.Average(x => x.PercentYield),
                                                YieldDefect = (grouped.Sum(x => x.FinishGood) - grouped.Sum(x => x.TotalDefect)) / grouped.Sum(x => x.FinishGood) * 100
                                            }).ToList();

                //Create separate lists Bar 1
                List<string> lineNameEff = resultGrpLineOverview.Select(x => x.LineName).ToList();
                List<decimal> effSTD = resultGrpLineOverview.Select(x => x.EffSTD).ToList();
                List<decimal> effLine = resultGrpLineOverview.Select(x => x.EffLine).ToList();

                if (lineNameEff.Count == 1 && resultGrpProductOverview.Count > 0)
                {
                    lineNameEff = resultGrpProductOverview.Select(x => x.SectionName).ToList();
                    effSTD = resultGrpProductOverview.Select(x => x.EffTarget).ToList();
                    effLine = resultGrpProductOverview.Select(x => x.EFFhr1).ToList();

                    ViewBag.xAxisTitle = "Section";
                }
                else ViewBag.xAxisTitle = "Line";

                //Set data bar 1
                List<object[]> chartDataEff = new List<object[]>();
                for (int i = 0; i < lineNameEff.Count; i++)
                {
                    string color = "";
                    if (effLine[i] >= effSTD[i])
                    {
                        color = "#5cd65c";
                    }
                    else
                    {
                        color = "#F44336";
                    }
                    chartDataEff.Add(new object[] { lineNameEff[i], effSTD[i], effLine[i], "#33c7ff", @color, null, null });
                }

                string chartDataJsonEff = JsonConvert.SerializeObject(chartDataEff);

                // Pass chartDataJson to the ViewBag
                ViewBag.ChartDataJsonEff = chartDataJsonEff;

                //Create separate lists Bar 2
                List<string> lineNameYield = resultGrpLineOverview.Select(x => x.LineName).ToList();
                List<decimal> yieldSTD = resultGrpLineOverview.Select(x => x.YieldSTD).ToList();
                List<decimal> yieldDefect = resultGrpLineOverview.Select(x => x.YieldDefect).ToList();

                //Set data bar 1
                List<object[]> chartDataYield = new List<object[]>();
                for (int i = 0; i < lineNameYield.Count; i++)
                {
                    string color = "";
                    if (yieldDefect[i] >= yieldSTD[i])
                    {
                        color = "#5cd65c";
                    }
                    else
                    {
                        color = "#F44336";
                    }
                    chartDataYield.Add(new object[] { lineNameYield[i], yieldSTD[i], yieldDefect[i], "#33c7ff", @color, null, null });
                }

                string chartDataJsonYield = JsonConvert.SerializeObject(chartDataYield);

                // Pass chartDataJson to the ViewBag
                ViewBag.ChartDataJsonYield = chartDataJsonYield;

                mymodel = new ViewModelReport
                {
                    view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                    view_EFFReport = mymodel.view_EFFReport.ToList(),
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    FilterYear = model.FilterYear,
                    FilterMonth = model.FilterMonth,
                    FilterLine = model.FilterLine,
                    FilterProduct = model.FilterProduct,
                    FilterPoint = model.FilterPoint,
                    ResultGrpProductOverviewModel = resultGrpProductOverview
                };

                ViewBag.VBRoleEmployeeDashBaord = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(23)).Select(x => x.RoleAction).FirstOrDefault();

                ////Set Refrsh Time
                int Valuesetup = db.TbSetup.Where(x => x.PlantID == PlantID).Select(x => x.Valuesetup).FirstOrDefault();
                ViewBag.SetTime = Valuesetup * 60000; //Change minute to millisecond

                return View(mymodel);
            }
        }

        private const int DAILY_REPORT_CHUNK_DAYS = 7;
        private const int EFFICIENCY_CHUNK_DAYS = 7;

        private static IEnumerable<(DateTime Start, DateTime End)> BuildDateChunks(DateTime start, DateTime end, int chunkDays)
        {
            if (start == DateTime.MinValue || end == DateTime.MinValue)
            {
                var today = DateTime.Today;
                yield return (today, today);
                yield break;
            }
            if (end < start) (start, end) = (end, start);

            var cursor = start.Date;
            var hardEnd = end.Date;
            while (cursor <= hardEnd)
            {
                var chunkEnd = cursor.AddDays(chunkDays - 1);
                if (chunkEnd > hardEnd) chunkEnd = hardEnd;
                yield return (cursor, chunkEnd);
                cursor = chunkEnd.AddDays(1);
            }
        }

        private async Task<List<View_EFFReport>> GetOverviewEFFDataAsync(
            int plantId, DateTime startDate, DateTime endDate,
            string lineId, string sectionId,
            CancellationToken ct = default)
        {
            var chunks = BuildDateChunks(startDate, endDate, EFFICIENCY_CHUNK_DAYS);

            var prevTimeout = db.Database.GetCommandTimeout();
            db.Database.SetCommandTimeout(TimeSpan.FromSeconds(180));

            try
            {
                var buffer = new List<View_EFFReport>(capacity: 4096);
                foreach (var (cs, ce) in chunks)
                {
                    var q = db.View_EFFReport
                        .AsNoTracking()
                        .Where(x => x.PlantID == plantId
                                    && x.TransactionDate >= cs
                                    && x.TransactionDate <= ce);

                    if (!string.IsNullOrEmpty(lineId)) q = q.Where(x => x.LineID == lineId);
                    if (!string.IsNullOrEmpty(sectionId)) q = q.Where(x => x.SectionID == sectionId);

                    var list = await q.ToListAsync(ct);
                    if (list.Count > 0) buffer.AddRange(list);
                }

                var dedup = buffer
                    .GroupBy(x => new { x.TransactionDate, x.PlantID, x.LineID, x.SectionID, x.ProductID, x.Prefix })
                    .Select(g => g.First())
                    .OrderBy(x => x.TransactionDate).ThenBy(x => x.LineID).ThenBy(x => x.SectionID).ThenBy(x => x.ProductID).ThenBy(x => x.Prefix)
                    .ToList();

                return dedup;
            }
            finally
            {
                db.Database.SetCommandTimeout(prevTimeout);
            }
        }

        private async Task<List<View_DailyReportSummary>> GetOverviewDailyReportDataAsync(
            int plantId, DateTime startDate, DateTime endDate,
            string lineId, string sectionId,
            CancellationToken ct = default)
        {
            var chunks = BuildDateChunks(startDate, endDate, DAILY_REPORT_CHUNK_DAYS);

            var prevTimeout = db.Database.GetCommandTimeout();
            db.Database.SetCommandTimeout(TimeSpan.FromSeconds(180));

            try
            {
                var buffer = new List<View_DailyReportSummary>(capacity: 4096);
                foreach (var (cs, ce) in chunks)
                {
                    var q = db.View_DailyReportSummary
                        .AsNoTracking()
                        .Where(x => x.PlantID == plantId
                                    && x.TransactionDate >= cs
                                    && x.TransactionDate <= ce);

                    if (!string.IsNullOrEmpty(lineId)) q = q.Where(x => x.LineID == lineId);
                    if (!string.IsNullOrEmpty(sectionId)) q = q.Where(x => x.SectionID == sectionId);

                    var list = await q.ToListAsync(ct);
                    if (list.Count > 0) buffer.AddRange(list);
                }

                var dedup = buffer
                    .GroupBy(x => new { x.TransactionDate, x.PlantID, x.LineID, x.SectionID, x.ProductID, x.QRCode, x.Prefix })
                    .Select(g => g.First())
                    .OrderBy(x => x.TransactionDate).ThenBy(x => x.LineID).ThenBy(x => x.SectionID).ThenBy(x => x.ProductID).ThenBy(x => x.QRCode)
                    .ToList();

                return dedup;
            }
            finally
            {
                db.Database.SetCommandTimeout(prevTimeout);
            }
        }



     


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FilterOverview(ViewModelReport mymodel)
        {

            if (mymodel.StartDate != DateTime.MinValue || mymodel.EndDate != DateTime.MinValue || mymodel.FilterYear != 0 || mymodel.FilterMonth != 0 || mymodel.FilterLine != null || mymodel.FilterProduct != null || mymodel.FilterPoint != null)
            {
                mymodel.filter = 1;
                //int Valuesetup = db.TbSetup.Where(x => x.PlantID == 6).Select(x => x.Valuesetup).FirstOrDefault();
               // ViewBag.SetTime = Valuesetup * 60000;
                return RedirectToAction("OverviewDashBoard", mymodel);

                // return RedirectToAction("OverviewDashBoard", mymodel);
            }
            else
            {
                mymodel.StartDate = DateTime.Today;
                mymodel.EndDate = DateTime.Today;
                return RedirectToAction("OverviewDashBoard", mymodel);

            }


        }



    }
}
