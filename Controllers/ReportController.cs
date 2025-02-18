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
        public ActionResult EmployeeDashBaord(ViewModelReport model)
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


                var mymodel = new ViewModelReport
                {
                    view_DailyReportSummary = db.View_DailyReportSummary.Where(x => x.PlantID == PlantID && x.Grade != "X" ).ToList(),
                  
                };



               // var sect = db.View_DailyReportSummary.Where(x=>x.PlantID.Equals(PlantID)).ToList();

                var varYear = from a in mymodel.view_DailyReportSummary
                              group a by new { a.TransactionDate.Year } into g
                              select new SelectListItem
                              {
                                  Value = $"{g.Key.Year}",
                                  Text = $"{g.Key.Year}"
                              };
                ViewBag.varYear = new SelectList(varYear, "Value", "Text");

                var varMonth = from a in mymodel.view_DailyReportSummary
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

                var varLine = from a in mymodel.view_DailyReportSummary
                              where a.PlantID.Equals(PlantID)
                              group a by new { a.LineID, a.LineName } into g
                              select new SelectListItem
                              {
                                  Value = $"{g.Key.LineID}",
                                  Text = $"{g.Key.LineName}"
                              };
                ViewBag.varLine = new SelectList(varLine, "Value", "Text");

                var varProduct = from a in mymodel.view_DailyReportSummary
                                 where a.PlantID.Equals(PlantID)
                                 group a by new { a.ProductID, a.ProductName } into g
                                 select new SelectListItem
                                 {
                                     Value = $"{g.Key.ProductID}",
                                     Text = $"{g.Key.ProductName}"
                                 };
                ViewBag.varProduct = new SelectList(varProduct, "Value", "Text");

                var varPoint = from a in mymodel.view_DailyReportSummary
                               where a.PlantID.Equals(PlantID)
                               group a by new { a.SectionID, a.SectionName } into g
                               select new SelectListItem
                               {
                                   Value = $"{g.Key.SectionID}",
                                   Text = $"{g.Key.SectionName}"
                               };
                ViewBag.varPoint = new SelectList(varPoint, "Value", "Text");


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
              //  var sumEmployeeDict = sumGrpEmp.Count();
                ViewBag.SumEmployee =   sumEmployeeDict.Count();


                /////////////////// 2 Group Product 
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
                                            //Diff = Convert.ToDouble((grouped.Sum(x => x.PcsPerHr) * 100) / grouped.Key.STD)
                                            Diff = Convert.ToDouble(((grouped.Sum(x => x.FGQty) / grouped.Sum(x => x.DiffHours)) - grouped.Key.STD) / grouped.Key.STD * 100)
                                        }).ToList();

                /////////////////// 3 Group Grad 

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
               // int sumOfCounts = sumGrpGrade.Count;


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
                                          //PcsPerHr = 0,                      
                                          Cnt = grouped.Count(),
                                          FGQty = grouped.Sum(x => x.FGQty),
                                          DiffHours = grouped.Sum(x => x.DiffHours),
                                          PcsPerHr = grouped.Sum(x => x.FGQty) / grouped.Sum(x => x.DiffHours),
                                          //PcsPerHr = Math.Round(grouped.Sum(g => g.PcsPerHr), 2),
                                          //PcsPerHr = grouped.Count() != 0 ? (grouped.Sum(g => g.FGQty) / sumEmployeeDict.Count() ) : 0,
                                          Percent = Math.Round((grouped.Count() / (double)sumOfCounts) * 100.00, 2)
                                      }).OrderBy(x => x.Grade).ToList();


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

                ////Set Refrsh Time
                int Valuesetup = db.TbSetup.Where(x => x.PlantID == PlantID).Select(x => x.Valuesetup).FirstOrDefault();
                ViewBag.SetTime = Valuesetup * 60000; //Change minute to millisecond

                return View(mymodel);

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

        [HttpGet]
        public ActionResult OverviewDashBoard(ViewModelReport model)
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


                var mymodel = new ViewModelReport
                {

                    view_EFFReport = db.View_EFFReport.Where(x => x.PlantID == PlantID).ToList(),
                    view_DailyReportSummary = db.View_DailyReportSummary.Where(x => x.PlantID == PlantID).ToList(),
                };
                var varYear = from a in mymodel.view_EFFReport
                              group a by new { a.TransactionDate.Year } into g
                              select new SelectListItem
                              {
                                  Value = $"{g.Key.Year}",
                                  Text = $"{g.Key.Year}"
                              };
                ViewBag.varYear = new SelectList(varYear, "Value", "Text");

                var varMonth = from a in mymodel.view_EFFReport
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

                var varLine = from a in mymodel.view_EFFReport
                              where a.PlantID.Equals(PlantID)
                              group a by new { a.LineID, a.LineName } into g
                              select new SelectListItem
                              {
                                  Value = $"{g.Key.LineID}",
                                  Text = $"{g.Key.LineName}"
                              };
                ViewBag.varLine = new SelectList(varLine, "Value", "Text");

                var varProduct = from a in mymodel.view_EFFReport
                                 where a.PlantID.Equals(PlantID)
                                 group a by new { a.ProductID, a.ProductName } into g
                                 select new SelectListItem
                                 {
                                     Value = $"{g.Key.ProductID}",
                                     Text = $"{g.Key.ProductName}"
                                 };
                ViewBag.varProduct = new SelectList(varProduct, "Value", "Text");

                var varPoint = from a in mymodel.view_EFFReport
                               where a.PlantID.Equals(PlantID)
                               group a by new { a.SectionID, a.SectionName } into g
                               select new SelectListItem
                               {
                                   Value = $"{g.Key.SectionID}",
                                   Text = $"{g.Key.SectionName}"
                               };
                ViewBag.varPoint = new SelectList(varPoint, "Value", "Text");


                /////////////////// 1 Product Overview
                ///

                //var resultGrpProductOverviewcheck = (from summary in db.View_EFFReport
                //                                where (model.FilterYear == 0 || summary.TransactionDate.Year == model.FilterYear) &&
                //                                      (model.FilterMonth == 0 || summary.TransactionDate.Month == model.FilterMonth) &&
                //                                      (model.FilterLine == null || summary.LineID == model.FilterLine) &&
                //                                      (model.FilterProduct == null || summary.ProductID == model.FilterProduct) &&
                //                                      (model.FilterPoint == null || summary.SectionID == model.FilterPoint) &&
                //                                      (model.StartDate == DateTime.MinValue || summary.TransactionDate >= model.StartDate) &&
                //                                      (model.EndDate == DateTime.MinValue || summary.TransactionDate <= model.EndDate) &&
                //                                       (summary.PlantID == PlantID) select(summary.CountQRCode)).ToString();


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
                //  var sumEmployeeDict = sumGrpEmp.Count();
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
                                                    //EFFhr1 = grouped.Sum(x => x.EFFhr1),
                                                    //EFFhr2 = grouped.Sum(x => x.EFFhr2),
                                                    //EFFhr3 = grouped.Sum(x => x.EFFhr3),
                                                    EFFhr1 = grouped.Sum(x => x.FinishGood) / grouped.Sum(x => x.EFF1),
                                                    EFFhr2 = grouped.Sum(x => x.FinishGood) / grouped.Sum(x => x.EFF2),
                                                    EFFhr3 = grouped.Sum(x => x.FinishGood) / grouped.Sum(x => x.EFF3),
                                                    TotalDefect = grouped.Sum(x => x.TotalDefect),

                                                    //1st Graph
                                                    EffSTD = grouped.Average(x => x.STD),
                                                    EffLine = grouped.Sum(x => x.ValueEFF3),

                                                    //2nd Graph
                                                    YieldSTD = grouped.Average(x => x.PercentYield),
                                                    YieldDefect = grouped.Max(x => x.YieldDefect),

                                                    //Table
                                                    ProductName = grouped.Key.ProductName,
                                                    SectionName = grouped.Key.SectionName,
                                                    EffTarget = grouped.Max(x => x.EFFSTD),
                                                    EffAct = grouped.Max(x => x.WorkinghourACT),
                                                    //DiffEff = (grouped.Sum(x => x.WorkinghourACT) - grouped.Max(x => x.EFFSTD)) / grouped.Max(x => x.EFFSTD) * 100,
                                                    DiffEff = ((grouped.Sum(x => x.FinishGood) / grouped.Sum(x => x.EFF1)) - grouped.Max(x => x.EFFSTD)) / grouped.Max(x => x.EFFSTD) * 100,
                                                    YieldTarget = grouped.Max(x => x.PercentYield),
                                                    YieldActual = grouped.Sum(x => x.YieldDefect),
                                                    DiffYield = grouped.Max(x => x.PercentYield) - grouped.Sum(x => x.YieldDefect) //YieldTarget - YieldActual
                                                }).ToList();


                //var resultGrpProductOverview = (from summary in db.View_EFFReport
                //                                    where (model.FilterYear == 0 || summary.TransactionDate.Year == model.FilterYear) &&
                //                                          (model.FilterMonth == 0 || summary.TransactionDate.Month == model.FilterMonth) &&
                //                                          (model.FilterLine == null || summary.LineID == model.FilterLine) &&
                //                                          (model.FilterProduct == null || summary.ProductID == model.FilterProduct) &&
                //                                          (model.FilterPoint == null || summary.SectionID == model.FilterPoint) &&
                //                                          (model.StartDate == DateTime.MinValue || summary.TransactionDate >= model.StartDate) &&
                //                                          (model.EndDate == DateTime.MinValue || summary.TransactionDate <= model.EndDate) &&
                //                                           (summary.PlantID == PlantID)
                //                                    group summary by new { summary.ProductID, summary.ProductName, summary.SectionID, summary.SectionName } into grouped
                //                                    select new ResultGrpProductOverviewModel
                //                                    {
                //                                        //Display Box
                //                                        SumEmp = grouped.Sum(x => x.CountQRCode),
                //                                        SumFG = grouped.Sum(x => x.FinishGood),
                //                                        CapHr = grouped.Sum(x => x.FinishGood) / grouped.Sum(x => x.EFF1),
                //                                        EFFhr1 = grouped.Average(x => x.EFFhr1),
                //                                        EFFhr2 = grouped.Average(x => x.EFFhr2),
                //                                        EFFhr3 = grouped.Average(x => x.EFFhr3),
                //                                        TotalDefect = grouped.Sum(x => x.TotalDefect),

                //                                        //1st Graph
                //                                        EffSTD = grouped.Average(x => x.STD),
                //                                        EffLine = grouped.Sum(x => x.EFF3),

                //                                        //2nd Graph
                //                                        YieldSTD = grouped.Average(x => x.PercentYield),
                //                                        YieldDefect = grouped.Sum(x => x.YieldDefect),

                //                                        //Table
                //                                        ProductName = grouped.Key.ProductName,
                //                                        SectionName = grouped.Key.SectionName,
                //                                        EffTarget = grouped.Average(x => x.STD),
                //                                        EffAct = grouped.Sum(x => x.EFFhr3),
                //                                        DiffEff = (grouped.Sum(x => x.EFFhr3)) - (grouped.Average(x => x.STD)),
                //                                        YieldTarget = grouped.Average(x => x.PercentYield),
                //                                        YieldActual = grouped.Sum(x => x.YieldDefect),
                //                                        DiffYield = (grouped.Sum(x => x.YieldDefect)) - (grouped.Average(x => x.PercentYield)) //YieldActual - YieldTarget
                //                                    }).ToList();


                if (resultGrpProductOverview.Count == 0)
                {
                    //ViewBag.SumEmployee = 0;
                    //ViewBag.AvgCapHr = 0;
                    //ViewBag.AvgEFFhr1 = 0;
                    //ViewBag.AvgEFFhr2 = 0;
                    //ViewBag.AvgEFFhr3 = 0;
                    //ViewBag.DefectAll = 0;

                    ViewBag.SumEmployee = 0;
                    ViewBag.SumCapHr = 0;
                    ViewBag.SumEFFhr1 = 0;
                    ViewBag.SumEFFhr2 = 0;
                    ViewBag.SumEFFhr3 = 0;
                    ViewBag.DefectAll = 0;
                }
                else
                {
                    //ViewBag.SumEmployee = resultGrpProductOverview.Sum(x => x.SumEmp);
                    //ViewBag.AvgCapHr = resultGrpProductOverview.Average(x => x.CapHr);
                    //ViewBag.AvgEFFhr1 = resultGrpProductOverview.Average(x => x.EFFhr1);
                    //ViewBag.AvgEFFhr2 = resultGrpProductOverview.Average(x => x.EFFhr2);
                    //ViewBag.AvgEFFhr3 = resultGrpProductOverview.Average(x => x.EFFhr3);
                    //ViewBag.DefectAll = resultGrpProductOverview.Average(x => x.TotalDefect);

                    // ViewBag.SumEmployee = resultGrpProductOverview.Sum(x => x.SumEmp);
                    ViewBag.SumCapHr = resultGrpProductOverview.Sum(x => x.CapHr);
                    ViewBag.SumEFFhr1 = resultGrpProductOverview.Sum(x => x.EFFhr1);
                    ViewBag.SumEFFhr2 = resultGrpProductOverview.Sum(x => x.EFFhr2);
                    ViewBag.SumEFFhr3 = resultGrpProductOverview.Sum(x => x.EFFhr3);
                    ViewBag.DefectAll = resultGrpProductOverview.Sum(x => x.TotalDefect);

                    //ViewBag.SumEmployee = resultGrpProductOverview.Sum(x => x.SumEmp);
                    //ViewBag.AvgCapHr = resultGrpProductOverview.Average(x => x.CapHr);
                    //ViewBag.AvgEFFhr1 = resultGrpProductOverview.Average(x => x.EFFhr1);
                    //ViewBag.AvgEFFhr2 = resultGrpProductOverview.Average(x => x.EFFhr2);
                    //ViewBag.AvgEFFhr3 = resultGrpProductOverview.Average(x => x.EFFhr3);
                    //ViewBag.DefectAll = resultGrpProductOverview.Average(x => x.TotalDefect);
                }

                /////////////////// 2 Group Bar Chart Line Overview
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
                                                 YieldDefect = grouped.Average(x => x.YieldDefect)

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
                    chartDataEff.Add(new object[] { lineNameEff[i], effSTD[i], effLine[i], "#33c7ff", @color, null , null });
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
                    view_PermissionMaster = db.View_PermissionMaster.Where(x=>x.PlantID.Equals(PlantID)).ToList(),
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


                //Set Refrsh Time
                //int Valuesetup = db.TbSetup.Where(x => x.PlantID == PlantID).Select(x => x.Valuesetup).FirstOrDefault();
                //ViewBag.SetTime = Valuesetup * 60; //Change minute to millisecond
                //String Refreshtime = Convert.ToString(Valuesetup * 60000);
                //Response.Headers.Add("Refresh", Refreshtime);

                return View(mymodel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FilterOverview(ViewModelReport mymodel)
        {

            if(mymodel.StartDate != DateTime.MinValue || mymodel.EndDate != DateTime.MinValue || mymodel.FilterYear !=0 || mymodel.FilterMonth !=0 || mymodel.FilterLine != null || mymodel.FilterProduct != null || mymodel.FilterPoint != null)
            {
                mymodel.filter = 1;
                return RedirectToAction("OverviewDashBoard", mymodel);
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
