﻿using Microsoft.AspNetCore.Mvc;
using Plims.Models;
using Plims.ViewModel;
using Plims.Data;
using System.Data;

using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using X.PagedList;


namespace Plims.Controllers
{
    public class EmployeeController : Controller
    {

        //Connect Model
        private readonly AppDbContext db;
        public EmployeeController(AppDbContext _db)
        {
            db = _db;
        }


        // GET: Employee
        public ActionResult UserInformation()
        {

            return View();
        }




        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Employee Clock In
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="EmployeeIDchk"></param>
        /// <param name="TransactionDate"></param>
        /// <param name="TransactionDateFillter"></param>
        /// <param name="action"></param>
        /// <returns></returns>

        [HttpGet]
        public ActionResult EmployeeClockIn(View_EmployeeClocktime obj, string[] EmployeeIDchk, string TransactionDate, string TransactionDateFillter, string action)//
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            var TransactionDateVar = DateTime.Today;

            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                view_EmployeeClocktime = db.View_EmployeeClocktime
                .Where(x => x.PlantID.Equals(PlantID))
                .OrderByDescending(x => x.TransactionDate)
                .ThenBy(x => x.SectionID)
                .ThenBy(x => x.ShiftID)
                .ThenBy(x => x.EmployeeID)
                .ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),

            };



            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }


          //if(EmployeeIDchk.Count() == 0) //(action == "Search" || action == "EmployeeClockIn")
            //{

            if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.SectionName) || !string.IsNullOrEmpty(TransactionDateFillter))
            {

                if (!string.IsNullOrEmpty(obj.EmployeeID))
                {
                    ViewBag.SelectedEmpID = obj.EmployeeID;
                    mymodel.view_EmployeeClocktime = mymodel.view_EmployeeClocktime.Where(p => p.EmployeeID == obj.EmployeeID).ToList();
                }
                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    ViewBag.SelectedLineName = obj.LineName;
                    mymodel.view_EmployeeClocktime = mymodel.view_EmployeeClocktime.Where(p => p.LineName == obj.LineName).ToList();
                }
                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    ViewBag.SelectedSectionName = obj.SectionName;
                    mymodel.view_EmployeeClocktime = mymodel.view_EmployeeClocktime.Where(p => p.SectionID == obj.SectionName).ToList();
                }
                if (!string.IsNullOrEmpty(TransactionDateFillter) && Convert.ToDateTime(TransactionDateFillter) != DateTime.Today)
                {
                    //DateTime datefillter = Convert.ToDateTime(TransactionDateFillter);
                    //ViewBag.SelectedTransactionDate = datefillter.ToString("yyyy-MM-dd");
                    //Employee.view_EmployeeClocktime = Employee.view_EmployeeClocktime.Where(p => p.TransactionDate.Equals(ViewBag.SelectedTransactionDate)).ToList();

                    if (DateTime.TryParse(TransactionDateFillter, out DateTime dateFilter))
                    {
                        ViewBag.SelectedTransactionDate = dateFilter.ToString("yyyy-MM-dd");
                        mymodel.view_EmployeeClocktime = mymodel.view_EmployeeClocktime.Where(p => p.TransactionDate.Date == dateFilter.Date).ToList();
                    }
                }
                else if (Convert.ToDateTime(TransactionDateFillter) == DateTime.Today)
                {
                    ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
                    mymodel.view_EmployeeClocktime = mymodel.view_EmployeeClocktime.Where(p => p.TransactionDate.Date.Equals(DateTime.Today)).ToList();

                }
                else
                {

                    //ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
                    mymodel.view_EmployeeClocktime = mymodel.view_EmployeeClocktime.Where(p => p.TransactionDate.Equals(DateTime.MinValue)).ToList();
                   // mymodel.view_EmployeeClocktime = mymodel.view_EmployeeClocktime.ToList();
                }
                ViewBag.VBRoleEmpClockIn = db.View_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();

                // mymodel.view_EmployeeClocktime = mymodel.view_EmployeeClocktime.Where(p => p.TransactionDate.Equals(DateTime.MinValue) || p.TransactionDate.Equals(DateTime.MinValue)).ToList();
                return View(mymodel);

            }
            else
            {
                ViewBag.VBRoleEmpClockIn = db.View_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();

                //ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
                mymodel.view_EmployeeClocktime = mymodel.view_EmployeeClocktime.Where(p => p.TransactionDate.Equals(DateTime.Today) || p.TransactionDate.Equals(DateTime.MinValue)).ToList();

                return View(mymodel);
            }

            //}
            //else
            //{
            //    if (obj.ClockIn == null || obj.ClockIn == "" || Convert.ToDateTime(TransactionDate) == DateTime.MinValue)
            //    {
            //        TempData["AlertMessage"] = "Please fill Time/Date Clockin";
            //        return RedirectToAction("EmployeeClockIn", "Employee");
            //    }

            //    // Create Function

            //    int datacnt = EmployeeIDchk.Count();
            //    for (int i = 0; i < datacnt; ++i)
            //    {

            //        string empid = EmployeeIDchk[i];
            //        DateTime clockoutvar;
            //        DateTime clockinvar;
            //        var empdbcheck = db.TbServicesTransaction.Where(x => x.TransactionDate.Equals(TransactionDateVar) && x.EmployeeID.Equals(empid) && x.ClockOut == "").ToList();
            //        if (empdbcheck.Count() != 0)
            //        {
            //            TempData["AlertMessage"] = "Please Services Clock out Employee ID :" + empid;
            //            return RedirectToAction("EmployeeClockIn");
            //        }


            //        var EmpTrans = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid) && x.Plant.Equals(PlantID) && x.TransactionDate == TransactionDateVar && x.Remark == "" && x.WorkingStatus == "Working").ToList();
            //        if (EmpTrans.Count() != 0)
            //        {

            //            var Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.Plant.Equals(PlantID) && x.TransactionDate == TransactionDateVar).SingleOrDefault();
            //            Empdb.ClockIn = obj.ClockIn.ToString();
            //            Empdb.UpdateBy = EmpID;//User.Identity.Name;
            //            Empdb.UpdateDate = DateTime.Now;
            //            db.SaveChanges();

            //        }
            //        else
            //        {

            //            var empdetails = db.TbEmployeeMaster.Where(x => x.EmployeeID == empid.Trim() && x.PlantID.Equals(PlantID)).SingleOrDefault();

            //            if (!string.IsNullOrEmpty(obj.ClockOut))
            //            {
            //                var startt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.StartTime).SingleOrDefault();
            //                var Endt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.EndTime).SingleOrDefault();
            //                var Prefixt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.Prefix).SingleOrDefault();

            //                //Case with clock out
            //                db.TbEmployeeTransaction.Add(new TbEmployeeTransaction()
            //                {
            //                    TransactionDate = Convert.ToDateTime(TransactionDateVar),
            //                    EmployeeID = empid,
            //                    Shift = empdetails.ShiftID,
            //                    StartTime = startt,
            //                    EndTime = Endt,
            //                    Plant = PlantID,
            //                    Line = empdetails.LineID,//obj.LineName,
            //                    Section = empdetails.SectionID,
            //                    WorkingStatus = "Working",
            //                    Prefix = Prefixt,
            //                    BreakFlag = "",
            //                    Remark = "",
            //                    ClockIn = obj.ClockIn,
            //                    CreateDate = DateTime.Now,
            //                    CreateBy = EmpID,//User.Identity.Name,
            //                    UpdateDate = DateTime.Now,
            //                    UpdateBy = EmpID//User.Identity.Name,
            //                });


            //            }
            //            else
            //            {
            //                var startt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.StartTime).SingleOrDefault();
            //                var Endt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.EndTime).SingleOrDefault();
            //                var Prefixt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.Prefix).SingleOrDefault();

            //                db.TbEmployeeTransaction.Add(new TbEmployeeTransaction()
            //                {

            //                    TransactionDate = Convert.ToDateTime(TransactionDateVar),
            //                    EmployeeID = empid,
            //                    Plant = PlantID,
            //                    Shift = empdetails.ShiftID,
            //                    StartTime = startt,
            //                    EndTime = Endt,
            //                    Prefix = Prefixt,
            //                    Line = empdetails.LineID,//obj.LineName,
            //                    Section = empdetails.SectionID,
            //                    ClockIn = obj.ClockIn,
            //                    ClockOut = "",
            //                    WorkingStatus = "Working",
            //                    BreakFlag = "",
            //                    Remark = "",
            //                    CreateDate = DateTime.Now,
            //                    CreateBy = EmpID,//User.Identity.Name,
            //                    UpdateDate = DateTime.Now,
            //                    UpdateBy = EmpID,//User.Identity.Name,
            //                });



            //            }
            //            db.SaveChanges();

            //        }
            //    }
            //    return RedirectToAction("EmployeeClockIn");
            //   // return View(mymodel);

            //}

        }




        [HttpGet]
        public ActionResult EmployeeClockInsave(string[] EmployeeIDchk, string ClockIn, DateTime TransactionDate)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            var TransactionDateVar = TransactionDate;// DateTime.Today;

            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).OrderBy(x => x.ShiftID).ThenBy(x=>x.EmployeeID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),

            };

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            ViewBag.VBRoleEmpClockIn = db.View_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();

            // Create Function


            string[] EmployeeIDchksplite = EmployeeIDchk[0].Split(",");
                   int datacnt = EmployeeIDchksplite.Count();
            for (int i = 0; i < datacnt; ++i)
                {
                
               // string empid = EmployeeIDchk[i];
                string empid = EmployeeIDchksplite[i];
                DateTime clockoutvar;
                    DateTime clockinvar;

                   // var empdbcheck = db.TbServicesTransaction.Where(x => x.EmployeeID.Equals(empid) && x.ClockOut == "" ).ToList();
                   // var empcheckduplicate = db.View_EmployeeClocktime.Where(x => x.EmployeeID.Equals(empid) && x.ClockIn == ClockIn && x.ClockIn != x.ClockOut && x.TransactionDate.Equals(TransactionDate)).ToList();

                        var empdetails = db.TbEmployeeMaster.Where(x => x.EmployeeID == empid.Trim() && x.PlantID.Equals(PlantID)).SingleOrDefault();
                        var startt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.StartTime).SingleOrDefault();
                        var Endt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.EndTime).SingleOrDefault();
                        var Prefixt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.Prefix).SingleOrDefault();
                    int counttran = db.TbEmployeeTransaction.Where(x => x.EmployeeID == empid.Trim() && x.Plant.Equals(PlantID) && x.ClockOut == "").ToList().Count;
                    if(counttran == 0)
                    {
                    db.TbEmployeeTransaction.Add(new TbEmployeeTransaction()
                    {
                        TransactionDate = Convert.ToDateTime(TransactionDateVar),
                        EmployeeID = empid,
                        Plant = PlantID,
                        Shift = empdetails.ShiftID,
                        StartTime = startt,
                        EndTime = Endt,
                        Prefix = Prefixt,
                        Line = empdetails.LineID,//obj.LineName,
                        Section = empdetails.SectionID,
                        ClockIn = ClockIn,
                        ClockOut = "",
                        WorkingStatus = "Working",
                        BreakFlag = "",
                        Remark = "",
                        CreateDate = DateTime.Now,
                        CreateBy = EmpID,//User.Identity.Name,
                        UpdateDate = DateTime.Now,
                        UpdateBy = EmpID,//User.Identity.Name,
                    });

                    db.SaveChanges();
                }
               

                  
                }

                ViewBag.VBRoleEmpClockIn = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();
                return RedirectToAction("EmployeeClockIn");
            

        }



        [HttpGet]
        public ActionResult EmployeeClockInsavecurrent(string[] EmployeeIDchk, string ClockIn, DateTime TransactionDate)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            var TransactionDateVar = TransactionDate;// DateTime.Today;

            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).OrderBy(x => x.ShiftID).ThenBy(x => x.EmployeeID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),

            };



            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }


            ViewBag.VBRoleEmpClockIn = db.View_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();
            if (EmployeeIDchk.Length == 0)
            {
                return RedirectToAction("EmployeeClockIn", "Employee");
                //return View(mymodel);

            }
            else
            {

                if (ClockIn == null || ClockIn == "" || Convert.ToDateTime(TransactionDate) == DateTime.MinValue)
                {
                    TempData["AlertMessage"] = "Please fill Time/Date Clockin";
                    return RedirectToAction("EmployeeClockIn", "Employee");
                }

                // Create Function

                int datacnt = EmployeeIDchk.Count();
                for (int i = 0; i < datacnt; ++i)
                {

                    string empid = EmployeeIDchk[i];
                    DateTime clockoutvar;
                    DateTime clockinvar;

                    var empdbcheck = db.TbServicesTransaction.Where(x => x.EmployeeID.Equals(empid) && x.ClockOut == "").ToList();
                    var empcheckduplicate = db.View_EmployeeClocktime.Where(x => x.EmployeeID.Equals(empid) && x.ClockIn == ClockIn && x.ClockIn != x.ClockOut && x.TransactionDate.Equals(TransactionDate)).ToList();

                    // Fetch data as much as possible using SQL
                    var data = db.View_EmployeeClocktime
                        .Where(x => x.EmployeeID.Equals(empid)
                                    && x.TransactionDate.Equals(TransactionDateVar) && x.ClockOut != "")
                        .ToList(); // This brings the data into memory

                    // Perform the DateTime comparison in memory
                    var empcheckduplicate2 = data.Where(x => DateTime.Parse(x.ClockIn) <= DateTime.Parse(ClockIn)
                                    && DateTime.Parse(ClockIn) <= DateTime.Parse(x.ClockOut)).ToList();

                    //Alert Clockout Service
                    if (empdbcheck.Count() != 0)
                    {

                        TempData["AlertMessage"] = "Please Services Clock out Employee ID :" + empid + " Date :" + empdbcheck.First().TransactionDate;
                        return RedirectToAction("EmployeeClockIn");
                    }

                    //Alert Duplicate transaction and Alert Clockin has Range clockin berfore
                    if (empcheckduplicate.Count != 0 && empcheckduplicate2.Count != 0)
                    {
                        TempData["AlertMessage"] = "Please Clock in Duplicate , Employee :" + empid + " Date :" + empcheckduplicate.First().TransactionDate;
                        return RedirectToAction("EmployeeClockIn");
                    }


                    var EmpTrans = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid) && x.ClockOut == "" && x.Plant.Equals(PlantID) && x.WorkingStatus == "Working").ToList();
                    var EmpTransSumtime = db.TbEmployeeTransaction
                                        .Where(x => x.EmployeeID.Equals(empid) && x.TransactionDate.Equals(TransactionDateVar) &&
                                        !string.IsNullOrEmpty(x.ClockOut) && x.Plant.Equals(PlantID))
                                        .Select(x => new
                                        {
                                            x.ClockIn,
                                            x.ClockOut,
                                            Duration = (DateTime.Parse(x.ClockOut) - DateTime.Parse(x.ClockIn)).TotalMinutes // Calculate the difference in minutes
                                        })
                                        .ToList();

                    foreach (var item in EmpTransSumtime)
                    {
                        if (item.Duration > 1140)
                        {
                            TempData["AlertMessage"] = "Please Check total time of Employee.";
                            return RedirectToAction("EmployeeClockIn");
                        }
                    }


                    if (EmpTrans.Count() != 0)
                    {
                        var EmpTransval = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid) && x.ClockOut == "" && x.Plant.Equals(PlantID) && x.WorkingStatus == "Working").Select(x => x.TransactionDate).FirstOrDefault();

                        TempData["AlertMessage"] = "Please Employee Clock out Employee ID :" + empid + " Date :" + EmpTransval;
                        return RedirectToAction("EmployeeClockIn");

                    }
                    else
                    {

                        var empdetails = db.TbEmployeeMaster.Where(x => x.EmployeeID == empid.Trim() && x.PlantID.Equals(PlantID)).SingleOrDefault();
                        var startt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.StartTime).SingleOrDefault();
                        var Endt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.EndTime).SingleOrDefault();
                        var Prefixt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.Prefix).SingleOrDefault();

                        db.TbEmployeeTransaction.Add(new TbEmployeeTransaction()
                        {
                            TransactionDate = Convert.ToDateTime(TransactionDateVar),
                            EmployeeID = empid,
                            Plant = PlantID,
                            Shift = empdetails.ShiftID,
                            StartTime = startt,
                            EndTime = Endt,
                            Prefix = Prefixt,
                            Line = empdetails.LineID,//obj.LineName,
                            Section = empdetails.SectionID,
                            ClockIn = ClockIn,
                            ClockOut = "",
                            WorkingStatus = "Working",
                            BreakFlag = "",
                            Remark = "",
                            CreateDate = DateTime.Now,
                            CreateBy = EmpID,//User.Identity.Name,
                            UpdateDate = DateTime.Now,
                            UpdateBy = EmpID,//User.Identity.Name,
                        });



                        db.SaveChanges();

                    }
                }

                ViewBag.VBRoleEmpClockIn = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();
                return RedirectToAction("EmployeeClockIn");
            }

        }




        [HttpPost]
        public IActionResult EmployeeClockInCheck(string[] EmployeeIDchk, string ClockIn, DateTime TransactionDate)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            var TransactionDateVar = TransactionDate;// DateTime.Today;

            //var mymodel = new ViewModelAll
            //{
            //    tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
            //    tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
            //    tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
            //    view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).OrderBy(x => x.ShiftID).ThenBy(x => x.EmployeeID).ToList(),
            //    tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
            //    view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),

            //};



            if (EmpID == null)
            {
                return Json("Login", "Home");
            }


            //ViewBag.VBRoleEmpClockIn = db.View_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();
            if (EmployeeIDchk.Length == 0)
            {

                 return Json(new { success = false, message = "Please select Employee" });
              //  return RedirectToAction("EmployeeClockIn", "Employee");
            }
            else
            {

                // Create Function

                int datacnt = EmployeeIDchk.Count();
                for (int i = 0; i < datacnt; i++)
                {

                    string empid = EmployeeIDchk[i];
                   //// DateTime clockoutvar;
                   // DateTime clockinvar;

                    var empdbcheck = db.TbServicesTransaction.Where(x => x.EmployeeID.Equals(empid) && x.ClockOut == "").ToList();
                    var empcheckduplicate = db.View_EmployeeClocktime.Where(x => x.EmployeeID.Equals(empid) && x.ClockIn == ClockIn && x.ClockIn != x.ClockOut && x.TransactionDate.Equals(TransactionDate)).ToList();

                    // Fetch data as much as possible using SQL
                    var data = db.View_EmployeeClocktime
                        .Where(x => x.EmployeeID.Equals(empid)
                                    && x.TransactionDate.Equals(TransactionDateVar) && x.ClockOut != "")
                        .ToList(); // This brings the data into memory

                    // Perform the DateTime comparison in memory
                    var empcheckduplicate2 = data.Where(x => DateTime.Parse(x.ClockIn) <= DateTime.Parse(ClockIn)
                                    && DateTime.Parse(ClockIn) <= DateTime.Parse(x.ClockOut)).ToList();

                    //Alert Clockout Service
                    if (empdbcheck.Count() != 0)
                    {

                        return Json(new { success = false, message = "Please Services Clock out Employee ID :" + empid + " Date :" + empdbcheck.First().TransactionDate });
                    }

                    //Alert Duplicate transaction and Alert Clockin has Range clockin berfore
                    if (empcheckduplicate.Count != 0 && empcheckduplicate2.Count != 0)
                    {
                        
                        return Json(new { success = false, message = "Please Clock in Duplicate , Employee :" + empid + " Date :" + empcheckduplicate.First().TransactionDate });
                    }


                    var EmpTrans = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid) && x.ClockOut == "" && x.Plant.Equals(PlantID) && x.WorkingStatus == "Working").ToList();
                    var EmpTransSumtime = db.TbEmployeeTransaction
                                        .Where(x => x.EmployeeID.Equals(empid) && x.TransactionDate.Equals(TransactionDateVar) &&
                                        !string.IsNullOrEmpty(x.ClockOut) && x.Plant.Equals(PlantID))
                                        .Select(x => new
                                        {
                                            x.ClockIn,
                                            x.ClockOut,
                                            Duration = (DateTime.Parse(x.ClockOut) - DateTime.Parse(x.ClockIn)).TotalMinutes // Calculate the difference in minutes
                                        })
                                        .ToList();

                    foreach (var item in EmpTransSumtime)
                    {
                        if (item.Duration > 1140)
                        {
                            
                           
                            return Json(new { success = false, message = "Please Check total time of Employee."});

                            }
                    }


                    if (EmpTrans.Count() != 0)
                    {
                        var EmpTransval = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid) && x.ClockOut == "" && x.Plant.Equals(PlantID) && x.WorkingStatus == "Working").Select(x => x.TransactionDate).FirstOrDefault();

                      
                        return Json(new { success = false, message = "Please Employee Clock out Employee ID :" + empid + " Date :" + EmpTransval });
                    }
                    else
                    {
                       // return Json(new { success = true });

                    }
                }

             
            }


           // ViewBag.VBRoleEmpClockIn = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();
           
            return Json(new { success = true });
        }




        //Function Employee Clock in Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function

        [HttpGet]
        public JsonResult EmployeeClockInEdit(int ID, DateTime TranDate, int TransactionNo)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            // var todayDate = DateTime.Today.ToString("yyyy-MM-dd");  // Use the format "yyyy-MM-dd" to match the expected format
            var EmplID = db.TbEmployeeMaster.Where(x => x.ID.Equals(ID)).Select(x => x.EmployeeID).SingleOrDefault();

            var Emps = db.TbEmployeeTransaction
             .Where(x => x.EmployeeID.Equals(EmplID) && x.Plant.Equals(PlantID) && x.TransactionNo.Equals(TransactionNo)).SingleOrDefault();


            return Json(Emps);
        }


        //4.  Function Employee Clock In Update Transaction
        [HttpPost]
        public ActionResult EmployeeClockInUpdate(TbEmployeeTransaction obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var Empdb = new TbEmployeeTransaction();

            var EmpTran = db.TbEmployeeTransaction.Where(x => x.TransactionNo.Equals(obj.TransactionNo) && x.EmployeeID.Equals(obj.EmployeeID) && x.TransactionDate == obj.TransactionDate).ToList();
            if (EmpTran.Count() != 0 )
            {
                //Update Transaction
                Empdb = db.TbEmployeeTransaction.Where(x => x.TransactionNo.Equals(obj.TransactionNo) && x.EmployeeID == obj.EmployeeID && x.TransactionDate == obj.TransactionDate).SingleOrDefault();
                if (obj.TransactionDate != DateTime.MinValue)
                {
                    Empdb.TransactionDate = obj.TransactionDate;
                }

                Empdb.ClockIn = obj.ClockIn;
                Empdb.UpdateBy = EmpID;// User.Identity.Name;
                Empdb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }
            else
            {
                string workst;

                if (obj.WorkingStatus == null)
                { workst = "Working"; }
                else
                { workst = obj.WorkingStatus; }
                var empdetails = db.TbEmployeeMaster.Where(x => x.EmployeeID == obj.EmployeeID.Trim()).SingleOrDefault();
                // Create Transaction
                // Insert new Line               
                db.TbEmployeeTransaction.Add(new TbEmployeeTransaction()
                {
                    TransactionNo = db.TbEmployeeTransaction.Count() + 1,
                    TransactionDate = DateTime.Today.Date,
                    EmployeeID = obj.EmployeeID,
                    Shift = 1,
                    Plant = PlantID,
                    Line = obj.Line,
                    Section = obj.Section,
                    WorkingStatus = workst,
                    ClockIn = obj.ClockIn,
                    CreateDate = DateTime.Now,
                    CreateBy = EmpID,//User.Identity.Name,
                    UpdateDate = DateTime.Now,
                    UpdateBy = EmpID,//User.Identity.Name,
                });
                db.SaveChanges();

            }
            ViewBag.VBRoleEmpClockIn = db.View_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();

            return RedirectToAction("EmployeeClockIn");

        }

        public ActionResult EmployeeClockInClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            //var Employee = new ViewModelAll
            //{
            //    tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
            //    tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
            //    tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
            //    view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).OrderBy(x => x.ShiftID).ThenBy(x => x.EmployeeID).ToList(),
            //    tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
            //    view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
            //};


            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                view_EmployeeClocktime = db.View_EmployeeClocktime
                .Where(x => x.PlantID.Equals(PlantID))
                .OrderByDescending(x => x.TransactionDate)
                .ThenBy(x => x.SectionID)
                .ThenBy(x => x.ShiftID)
                .ThenBy(x => x.EmployeeID)
                .ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),

            };



            ViewBag.VBRoleEmpClockIn = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();
            mymodel.view_EmployeeClocktime = mymodel.view_EmployeeClocktime.Where(p => p.TransactionDate == DateTime.Today || p.TransactionDate.Equals(DateTime.MinValue)).ToList();

            return View("EmployeeClockIn", mymodel);

        }






        //// 5. Function Employee Clock In Update Create Transaction
        //[HttpPost]
        //public ActionResult EmployeeClockInCreate(View_EmployeeClocktime obj)
        //{
        //    int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
        //    string EmpID = HttpContext.Session.GetString("UserEmpID");
        //    var Empdb = new TbEmployeeTransaction();

        //    var EmpTran = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(obj.EmployeeID) && x.TransactionDate == DateTime.Today.Date).ToList();
        //    if (EmpTran.Count() != 0)
        //    {
        //        //Update Transaction
        //        Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == obj.EmployeeID && x.TransactionDate == DateTime.Today.Date).SingleOrDefault();
        //        Empdb.ClockIn = obj.ClockIn.ToString();
        //        Empdb.UpdateBy = EmpID;//User.Identity.Name;
        //        Empdb.UpdateDate = DateTime.Now;
        //        db.SaveChanges();
        //        return RedirectToAction("EmployeeClockIn");
        //    }
        //    else
        //    {
        //        string workst;

        //        if (obj.WorkingStatus == null)
        //        { workst = "Working"; }
        //        else
        //        { workst = obj.WorkingStatus; }
        //        var empdetails = db.TbEmployeeMaster.Where(x => x.EmployeeID == obj.EmployeeID.Trim()).SingleOrDefault();
        //        // Create Transaction
        //        // Insert new Line               
        //        db.TbEmployeeTransaction.Add(new TbEmployeeTransaction()
        //        {
        //            TransactionNo = db.TbEmployeeTransaction.Count() + 1,
        //            TransactionDate = DateTime.Today.Date,
        //            EmployeeID = obj.EmployeeID,
        //            Shift = Convert.ToInt32(obj.ShiftName),
        //            Plant = PlantID,
        //            Line = obj.LineName,
        //            WorkingStatus = workst,
        //            ClockIn = obj.ClockIn.ToString(),
        //            CreateDate = DateTime.Now,
        //            CreateBy = EmpID,//User.Identity.Name,
        //            UpdateDate = DateTime.Now,
        //            UpdateBy = EmpID,//User.Identity.Name,
        //        });
        //        db.SaveChanges();

        //    }

        //    return RedirectToAction("EmployeeClockIn");


        //}

        // End Employee Clock in Function
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////




        /// <summary>
        /// Employee Clock out Function
        /// </summary>
        /// <returns></returns>


        [HttpGet]
        public ActionResult EmployeeClockOut(View_EmployeeClocktime obj, string[] EmployeeIDchk, string TransactionDateFillter)/*,string action*/
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            //var TransactionDateVar = obj.TransactionDate;
            var TransactionDateVar = DateTime.Today;

            if (string.IsNullOrEmpty(TransactionDateFillter))
            {
                TransactionDateFillter = DateTime.Today.ToString("yyyy-MM-dd");
            }

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID) && x.Remark != "Adjust").OrderByDescending(x => x.WorkingStatus).ThenBy(x => x.ShiftID).ThenBy(x => x.EmployeeID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.Plant.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //view_Employee = db.View_Employee.ToList()

            };


            ViewBag.VBRoleEmpClockOut = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();
            //if(EmployeeIDchk.Count() == 0 ) //(action == "Search" || action == "EmployeeClockOut")
            //{

            if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.SectionName) | TransactionDateFillter != null)
            {

                if (!string.IsNullOrEmpty(obj.EmployeeID))
                {
                    ViewBag.SelectedEmpID = obj.EmployeeID;
                    Employee.view_EmployeeClocktime = Employee.view_EmployeeClocktime.Where(p => p.EmployeeID == obj.EmployeeID).ToList();
                }
                if (!string.IsNullOrEmpty(obj.LineName))
                {

                    ViewBag.SelectedLineName = obj.LineName;
                    Employee.view_EmployeeClocktime = Employee.view_EmployeeClocktime.Where(p => p.LineName == obj.LineName).ToList();
                }
                if (!string.IsNullOrEmpty(obj.SectionName))
                {

                    ViewBag.SelectedSectionName = obj.SectionName;
                    Employee.view_EmployeeClocktime = Employee.view_EmployeeClocktime.Where(p => p.SectionID == obj.SectionName).ToList();
                }
                if (!string.IsNullOrEmpty(TransactionDateFillter) && Convert.ToDateTime(TransactionDateFillter) != DateTime.Today)
                {
                    //DateTime datefillter = Convert.ToDateTime(TransactionDateFillter);
                    //ViewBag.SelectedTransactionDate = datefillter.ToString("yyyy-MM-dd");
                    //Employee.view_EmployeeClocktime = Employee.view_EmployeeClocktime.Where(p => p.TransactionDate.Equals(ViewBag.SelectedTransactionDate)).ToList();

                    if (DateTime.TryParse(TransactionDateFillter, out DateTime dateFilter))
                    {
                        ViewBag.SelectedTransactionDate = dateFilter.ToString("yyyy-MM-dd");
                        Employee.view_EmployeeClocktime = Employee.view_EmployeeClocktime.Where(p => p.TransactionDate.Date == dateFilter.Date).ToList();
                    }
                }
                else if (Convert.ToDateTime(TransactionDateFillter) == DateTime.Today)
                {
                    ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
                    Employee.view_EmployeeClocktime = Employee.view_EmployeeClocktime.Where(p => p.TransactionDate.Equals(DateTime.Today)).ToList();

                }
                else
                {


                }

                return View(Employee);

            }
            else
            {

                // ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
                Employee.view_EmployeeClocktime = Employee.view_EmployeeClocktime.Where(p => p.TransactionDate.Equals(DateTime.Today) || p.TransactionDate.Equals(DateTime.MinValue)).ToList();
                return View(Employee);
            }

            //}
            //else
            //{
            //    if (obj.ClockOut == null || obj.ClockOut == "" )
            //    {
            //        TempData["AlertMessage"] = "Please fill data Date/Time! Clockout";
            //        return RedirectToAction("EmployeeClockOut", "Employee");
            //    }

            //    // Create Function
            //    int datacnt = EmployeeIDchk.Count();
            //    for (int i = 0; i < datacnt; ++i)
            //    {

            //        // 1. check TbEmployeeTransaction == Null ?
            //        var Empdb = new TbEmployeeTransaction();
            //        //var Emp = db.TbEmployeeMasters.Where(x => x.EmployeeID.Equals(EmployeeID[i]));
            //        string empid = EmployeeIDchk[i];
            //        var EmpClockView = db.TbEmployeeMaster.Where(x => x.ID.Equals(Convert.ToInt32(empid))).Select(x => x.EmployeeID).SingleOrDefault();
            //        //  var EmpTrancheck = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(EmpClockView) && (x.TransactionDate == TransactionDateVar || (x.TransactionDate == TransactionDateVar.AddDays(-1) && x.ClockOut == "")) && (x.Remark == null || x.Remark == "") && x.WorkingStatus == "Working").ToList();


            //         //   DateTime dateFilter = DateTime.Parse(TransactionDateFillter);&& x.TransactionDate.Equals(dateFilter)//
            //            var EmpClockNo = db.View_EmployeeClocktime.Where(x => x.ID.Equals(Convert.ToInt32(empid))&& x.ClockOut == "" ).Select(x => x.TransactionNo).SingleOrDefault();


            //        var EmpTrancheck = db.TbEmployeeTransaction.Where(x => x.TransactionNo.Equals(EmpClockNo) ).ToList();

            //        if (EmpTrancheck.Count() == 1)
            //        {
            //            var EmpTran = db.TbEmployeeTransaction.Where(x => x.TransactionNo.Equals(EmpClockNo)).SingleOrDefault();


            //            if (EmpTran != null)
            //            {
            //                //Update Transaction
            //                // Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(EmpClockView) && x.TransactionDate == TransactionDateVar && (x.Remark == null || x.Remark == "") && x.WorkingStatus.Equals("Working")).SingleOrDefault();
            //                EmpTran.ClockOut = obj.ClockOut.ToString();
            //                EmpTran.UpdateBy = EmpID;//User.Identity.Name;
            //                EmpTran.UpdateDate = DateTime.Now;
            //                db.SaveChanges();

            //            }
            //            else
            //            {

            //                TempData["AlertMessage"] = "Please Clock In !";

            //            }

            //        }
            //        else
            //        {
            //            TempData["AlertMessage"] = "Please Contact IT : Check data in database some transaction not clock out!";
            //            return RedirectToAction("EmployeeClockOut");

            //        }
            //    }


            //}
            // return RedirectToAction("EmployeeClockOut");

        }




        [HttpGet]
        public ActionResult EmployeeClockOutSave(View_EmployeeClocktime obj, string[] EmployeeIDchk, string TransactionDateFillter)/*,string action*/
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            //var TransactionDateVar = obj.TransactionDate;
            var TransactionDateVar = DateTime.Today;

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
               
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID) && x.Remark != "Adjust").OrderBy(x => x.ShiftID).ThenBy(x => x.EmployeeID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),


            };





            ViewBag.VBRoleEmpClockOut = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();
            if (EmployeeIDchk.Length == 0)
            {
                return RedirectToAction("EmployeeClockOut", "Employee");

            }
            else
            {
               


                // Create Function
                int datacnt = EmployeeIDchk.Count();
                for (int i = 0; i < datacnt; ++i)
                {

                    var Empdb = new TbEmployeeTransaction();
                    string empid = EmployeeIDchk[i];
                    var EmpClockView = db.TbEmployeeMaster.Where(x => x.ID.Equals(Convert.ToInt32(empid))).Select(x => x.EmployeeID).SingleOrDefault();
                    //var EmpProducttiondata = db.TbProductionTransaction.Where(x => x.TransactionDate.Equals(TransactionDateVar) && x.QRCode == EmpClockView && x.PlantID.Equals(PlantID)).ToList();
                    //if(EmpProducttiondata.Count() == 0)
                    //{
                    //    TempData["AlertMessage"] = "This Employee no data scan." + EmpClockView;
                    //    return RedirectToAction("EmployeeClockOut", "Employee");
                    //}
                    //Check time for use 
                    var EmpTransSumtime = db.TbEmployeeTransaction
                                      .Where(x => x.EmployeeID.Equals(EmpClockView) && x.TransactionDate.Equals(TransactionDateVar) &&
                                      !string.IsNullOrEmpty(x.ClockOut) && x.Plant.Equals(PlantID) )
                                      .Select(x => new
                                      {
                                          x.ClockIn,
                                          x.ClockOut,
                                          Duration = (DateTime.Parse(x.ClockOut) - DateTime.Parse(x.ClockIn)).TotalMinutes // Calculate the difference in minutes
                                      })
                                      .ToList();

                   
                    //   DateTime dateFilter = DateTime.Parse(TransactionDateFillter);&& x.TransactionDate.Equals(dateFilter)//
                    var EmpClockNo = db.View_EmployeeClocktime.Where(x => x.ID.Equals(Convert.ToInt32(empid)) && x.ClockIn != "" && x.ClockOut == "")
                                    .Select(x => new
                                    { x.TransactionNo ,
                                        x.ClockIn
                                    }).SingleOrDefault();

                    foreach (var item in EmpTransSumtime)
                    {

                        var outtime = obj.ClockOut;
                        var intime = EmpClockNo.ClockIn;
                        double diffoutin = (DateTime.Parse(outtime) - DateTime.Parse(intime)).TotalMinutes;
                        double absDuration = 1140 - Math.Abs(item.Duration);
                        if (diffoutin > absDuration)
                        {
                            TempData["AlertMessage"] = "Please Check total time of Employee.";
                            return RedirectToAction("EmployeeClockOut", "Employee");
                        }
                    }

                  
                    var EmpTrancheck = db.TbEmployeeTransaction.Where(x => x.TransactionNo.Equals(EmpClockNo.TransactionNo)).ToList();

                    if (EmpTrancheck.Count() == 1)
                    {
                        var EmpTran = db.TbEmployeeTransaction.Where(x => x.TransactionNo.Equals(EmpClockNo.TransactionNo)).SingleOrDefault();
                        //count stamp qty 
                        var qtyperht = db.TbProductionTransaction.Where(x=> x.PlantID.Equals(PlantID) &&  x.TransactionDate.Equals(obj.TransactionDate) && x.LineID.Equals(obj.LineID) && x.SectionID.Equals(obj.SectionID)).FirstOrDefault();
                        //Check incentive
                        var incentiverateGrade = db.TbIncentiveMaster.Where(x => x.PlantID.Equals(PlantID) && x.SectionID.Equals(EmpTran.Section) && x.LineID.Equals(EmpTran.Line))
                            .Select(x => new
                            {
                                x.Grade,
                                x.Rate,
                                x.ProductID,
                                x.Min,
                                x.Max
                            }).ToList();
                        // insert record

                        


                        if (EmpTran != null)
                        {
                            //check มีแล้วหรือยังถ้ามีแล้วไม่แอด 

                            foreach(var item in incentiverateGrade)
                            {

                                //var checkincentive = db.tbRateTransaction.ToList();



                                var checkincentive = db.tbRateTransaction.Where(x => x.PlantID.Equals(PlantID)
                                && x.TransactionDate.Equals(Convert.ToDateTime(EmpTran.TransactionDate))
                                && x.LineID.Equals(EmpTran.Line)
                                && x.Grade.Equals(item.Grade)
                                && x.Type.Equals("Employee")).ToList();


                                if (checkincentive.Count == 0 )
                                {
                                    // Table : TbTransactionRate  Create
                                    db.tbRateTransaction.Add(new TbRateTransaction()
                                    {
                                        TransactionDate = Convert.ToDateTime(EmpTran.TransactionDate),
                                        PlantID = PlantID,
                                        LineID = EmpTran.Line,
                                        SectionID = EmpTran.Section,
                                        ProductID = item.ProductID,
                                        Type = "Employee",
                                        Rate = item.Rate,
                                        Grade = item.Grade,
                                        Min = item.Min,
                                        Max = item.Max,
                                        CreateDate = DateTime.Now,
                                        CreateBy = EmpID,
                                        UpdateBy = EmpID,
                                        UpdateDate = DateTime.Now
                                    });
                                    db.SaveChanges();

                                }
                              

                            }



                            //Update Transaction
                            // Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(EmpClockView) && x.TransactionDate == TransactionDateVar && (x.Remark == null || x.Remark == "") && x.WorkingStatus.Equals("Working")).SingleOrDefault();
                            EmpTran.ClockOut = obj.ClockOut.ToString();
                            EmpTran.WorkingStatus = obj.WorkingStatus;
                            EmpTran.UpdateBy = EmpID;//User.Identity.Name;
                            EmpTran.UpdateDate = DateTime.Now;
                            db.SaveChanges();

                        }
                        else
                        {

                            TempData["AlertMessage"] = "Please Clock In !";

                        }

                    }
                    else
                    {
                        TempData["AlertMessage"] = "Check data in database some transaction not clock out!";
                        return RedirectToAction("EmployeeClockOut");

                    }
                }

            }

            return RedirectToAction("EmployeeClockOut");

        }





        //[HttpGet]
        // public ActionResult EmployeeClockOut(View_EmployeeClocktime obj, string[] EmployeeIDchk )
        //{
        //    int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
        //    string EmpID = HttpContext.Session.GetString("UserEmpID");
        //    //var TransactionDateVar = obj.TransactionDate;
        //    var TransactionDateVar = DateTime.Today;

        //    if (EmpID == null)
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }
        //    var Employee = new ViewModelAll
        //    {
        //        tbEmployeeMaster = db.TbEmployeeMaster.ToList(),
        //       tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
        //        tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
        //        tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
        //        view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
        //       tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
        //        tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.Plant.Equals(PlantID)).ToList(),
        //        view_PermissionMaster = db.View_PermissionMaster.ToList(),
        //        view_Employee = db.View_Employee.ToList()

        //    };





        //    ViewBag.VBRoleEmpClockOut = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();
        //    if (EmployeeIDchk.Length == 0)
        //    {

        //        if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.SectionName))
        //        {

        //            if (!string.IsNullOrEmpty(obj.EmployeeID))
        //            {
        //                ViewBag.SelectedEmpID = obj.EmployeeID;
        //                Employee.view_EmployeeClocktime = Employee.view_EmployeeClocktime.Where(p => p.EmployeeID == obj.EmployeeID).ToList();
        //            }
        //            if (!string.IsNullOrEmpty(obj.LineName))
        //            {

        //                ViewBag.SelectedLineName = obj.LineName;
        //                Employee.view_EmployeeClocktime = Employee.view_EmployeeClocktime.Where(p => p.LineName == obj.LineName).ToList();
        //            }
        //            if (!string.IsNullOrEmpty(obj.SectionName))
        //            {

        //                ViewBag.SelectedSectionName = obj.SectionName;
        //                Employee.view_EmployeeClocktime = Employee.view_EmployeeClocktime.Where(p => p.SectionID == obj.SectionName).ToList();
        //            }

        //            return View(Employee);

        //        }
        //        else
        //        {

        //             //&& (x.TransactionDate == DateTime.Now || x.TransactionDate == DateTime.Now.AddDays(-1)) && x.ClockIn != null && x.ClockOut == null
        //            return View(Employee);
        //        }

        //    }
        //    else
        //    {
        //        if (obj.ClockOut == null || obj.ClockOut == "")
        //        {
        //            return RedirectToAction("EmployeeClockOut", "Employee");
        //        }

        //        // Create Function
        //        int datacnt = EmployeeIDchk.Count();
        //        for (int i = 0; i < datacnt; ++i)
        //        {

        //            // 1. check TbEmployeeTransaction == Null ?
        //            var Empdb = new TbEmployeeTransaction();
        //            //var Emp = db.TbEmployeeMasters.Where(x => x.EmployeeID.Equals(EmployeeID[i]));
        //            string empid = EmployeeIDchk[i];
        //            var EmpClockView = db.TbEmployeeMaster.Where(x => x.ID.Equals(Convert.ToInt32(empid))).Select(x => x.EmployeeID).SingleOrDefault();
        //            var EmpTrancheck = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(EmpClockView) && (x.TransactionDate == TransactionDateVar || (x.TransactionDate == TransactionDateVar.AddDays(-1) && x.ClockOut == "")) && (x.Remark == null || x.Remark == "") && x.WorkingStatus == "Working").ToList();
        //            if (EmpTrancheck.Count() == 1)
        //            {
        //                var EmpTran = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(EmpClockView) && (x.TransactionDate == TransactionDateVar || (x.TransactionDate == TransactionDateVar.AddDays(-1) && x.ClockOut == "")) && (x.Remark == null || x.Remark == "") && x.WorkingStatus == "Working").SingleOrDefault();


        //                if (EmpTran != null)
        //                {
        //                    //Update Transaction
        //                    // Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(EmpClockView) && x.TransactionDate == TransactionDateVar && (x.Remark == null || x.Remark == "") && x.WorkingStatus.Equals("Working")).SingleOrDefault();
        //                    EmpTran.ClockOut = obj.ClockOut.ToString();
        //                    EmpTran.UpdateBy = EmpID;//User.Identity.Name;
        //                    EmpTran.UpdateDate = DateTime.Now;
        //                    db.SaveChanges();

        //                }
        //                else
        //                {

        //                    TempData["AlertMessage"] = "Please Clock In !";

        //                }

        //            }
        //            else 
        //            {
        //                TempData["AlertMessage"] = "Please Contact IT : Check data in database some transaction not clock out!";
        //                return RedirectToAction("EmployeeClockOut");

        //            }
        //        }


        //    }return RedirectToAction("EmployeeClockOut");

        //}








        //3.  Function Employee Clock in Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult EmployeeClockOutEdit(int ID, int TransactionNo)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            //    var todayDate = DateTime.Today.ToString("yyyy-MM-dd");  // Use the format "yyyy-MM-dd" to match the expected format
            var EmplID = db.TbEmployeeMaster.Where(x => x.ID.Equals(ID)).Select(x => x.EmployeeID).Max();

            //   DateTime parsedTodayDate = DateTime.ParseExact(todayDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var Emps = db.TbEmployeeTransaction
                .Where(x => x.TransactionNo.Equals(TransactionNo)).SingleOrDefault();


            return Json(Emps);
        }



        //4.  Function Employee Clock In Update Transaction
        [HttpPost]
        public ActionResult EmployeeClockOutUpdate(TbEmployeeTransaction obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            // 1. check TbEmployeeTransaction == Null ?
            var Empdb = new TbEmployeeTransaction();

            var EmpTran = db.TbEmployeeTransaction.Where(x => x.TransactionNo.Equals(obj.TransactionNo) ).ToList();
            if (EmpTran.Count() != 0 && obj.ClockOut != null)
            {
                //Update Transaction
                Empdb = db.TbEmployeeTransaction.Where(x => x.TransactionNo.Equals(obj.TransactionNo) ).SingleOrDefault();
                Empdb.ClockOut = obj.ClockOut;
                Empdb.UpdateBy = EmpID;//User.Identity.Name,
                Empdb.UpdateDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("EmployeeClockOut");
            }
            else
            {
                TempData["AlertMessage"] = "Please Clock In !";


            }

            return RedirectToAction("EmployeeClockOut");

        }


        public ActionResult EmployeeClockOutClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID) && x.Remark != "Adjust").OrderBy(x=>x.ShiftID).ThenBy(x=>x.EmployeeID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                // tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x =>  x.Plant.Equals(PlantID)),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                // view_Employee = db.View_Employee.ToList()

            };
            //  ViewBag.SelectedTransactionDate = DateTime.Today;
            ViewBag.VBRoleEmpClockOut = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();

            Employee.view_EmployeeClocktime = Employee.view_EmployeeClocktime.Where(p => p.TransactionDate.Equals(DateTime.Today) || p.TransactionDate.Equals(DateTime.MinValue)).ToList();
            return View("EmployeeClockOut", Employee);

        }



        //End Employee Clock out
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////



        /// <summary>
        /// Services Clock in Function
        /// </summary>
        /// <returns></returns>
        /// 

        [HttpGet]
        public ActionResult ServicesClockIn(View_ServicesClocktime obj, string[] EmployeeIDchk, string TableData, string LineID, string SectionSelect, string TransactionDateFillter) //  , string action
        {

            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            //var TransactionDateVar = obj.TransactionDate;
            var TransactionDateVar = DateTime.Today;

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID) && x.ServicesStatus.Equals(1)).ToList(),
                view_ServicesClocktime = db.View_ServicesClocktime.Where(x => x.PlantID.Equals(PlantID))
                .OrderByDescending(x => x.TransactionDate)
                .ThenBy(x => x.ShiftID).ThenBy(x=>x.EmployeeID)
                .ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                // tbServicesTransaction = db.TbServicesTransaction.Where(x => x.Plant.Equals(PlantID)),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                // view_Employee = db.View_Employee.ToList()
            };
            ViewBag.VBRoleServicesClockIn = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(18)).Select(x => x.RoleAction).FirstOrDefault();

            //if  (EmployeeIDchk.Count() == 0)  //(action == "Search" || action == "ServicesClockIn")

            //{

            if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.SectionName) || !string.IsNullOrEmpty(TransactionDateFillter))
            {
                if (!string.IsNullOrEmpty(obj.EmployeeID))
                {
                    ViewBag.SelectedEmpID = obj.EmployeeID;
                    Employee.view_ServicesClocktime = Employee.view_ServicesClocktime.Where(p => p.EmployeeID == obj.EmployeeID).ToList();
                }
                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    ViewBag.SelectedLineName = obj.LineName;
                    Employee.view_ServicesClocktime = Employee.view_ServicesClocktime.Where(p => p.LineName == obj.LineName).ToList();
                }
                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    ViewBag.SelectedSectionName = obj.SectionName;
                    Employee.view_ServicesClocktime = Employee.view_ServicesClocktime.Where(p => p.SectionID == obj.SectionName).ToList();
                }
                if (!string.IsNullOrEmpty(TransactionDateFillter) && Convert.ToDateTime(TransactionDateFillter) != DateTime.Today)
                {

                    if (DateTime.TryParse(TransactionDateFillter, out DateTime dateFilter))
                    {
                        ViewBag.SelectedTransactionDate = dateFilter.ToString("yyyy-MM-dd");
                        Employee.view_ServicesClocktime = Employee.view_ServicesClocktime.Where(p => p.TransactionDate.Date == dateFilter.Date).ToList();
                    }
                }
                else if (Convert.ToDateTime(TransactionDateFillter) == DateTime.Today)
                {
                    ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
                    Employee.view_ServicesClocktime = Employee.view_ServicesClocktime.Where(p => p.TransactionDate.Equals(DateTime.Today)).ToList();

                }
                else
                {
                    Employee.view_ServicesClocktime = Employee.view_ServicesClocktime.Where(p => p.TransactionDate.Equals(DateTime.MinValue)).ToList();

                }
                return View(Employee);
            }
            else
            {
                //ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
                Employee.view_ServicesClocktime = Employee.view_ServicesClocktime.Where(p => p.TransactionDate.Equals(DateTime.MinValue) || p.TransactionDate.Equals(DateTime.Today)).ToList();

                return View(Employee);
            }


        }



        [HttpGet]
        public ActionResult ServicesClockInSave(View_ServicesClocktime obj, string[] EmployeeIDchk, string TableData, string LineID, string SectionSelect, string TransactionDateFillter) //  , string action
        {

            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var TransactionDateVar = Convert.ToDateTime(TransactionDateFillter); // obj.TransactionDate;

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals("1")).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID) && x.ServicesStatus.Equals(1)).ToList(),
                view_ServicesClocktime = db.View_ServicesClocktime.Where(x => x.PlantID.Equals(PlantID)).OrderBy(x => x.ShiftID).ThenBy(x => x.EmployeeID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),

            };
            ViewBag.VBRoleServicesClockIn = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(18)).Select(x => x.RoleAction).FirstOrDefault();

            // Create Function
            //int datacnt = EmployeeIDchk.Count();
            List<TableDataRow> tableRows = JsonConvert.DeserializeObject<List<TableDataRow>>(TableData);

          


            string[] EmployeeIDchksplite = EmployeeIDchk[0].Split(",");
            int datacnt = EmployeeIDchksplite.Count();


            for (int i = 0; i < datacnt; ++i)
            {
                string empid = EmployeeIDchksplite[i];
                foreach (var itmservice in tableRows)
                {
                    var Empdb = new TbServicesTransaction();
                   // string empid = EmployeeIDchk[i];

                    var sectionsplit = itmservice.Section.Split(":").ToList();
                    var servicesplit = itmservice.Service.Split(":").ToList();
                    var remark = "";
                    if (SectionSelect == "All")
                    {
                        remark = "All";
                    }
                    else
                    {
                        remark = "";
                    }

                    ////////////////////////// select service normal case //////////////////////////////////

                    var empdbcheck = db.TbEmployeeTransaction
                    .Where(x => x.TransactionDate.Equals(TransactionDateVar) &&
                      (!string.IsNullOrEmpty(x.ClockIn) ||
                       !string.IsNullOrEmpty(x.ClockOut)) && x.EmployeeID.Equals(empid))
                    .ToList();
                    if (empdbcheck.Count() != 0)
                    {
                        foreach (var itm in empdbcheck)
                        {
                            var empidvar = itm.EmployeeID;
                            var clockinvar = Convert.ToDateTime(itm.ClockIn);
     

                            var clockoutvar = Convert.ToDateTime(itm.ClockOut);
                            var empdb = db.View_Employee.FirstOrDefault(x => x.EmployeeID.Equals(empidvar));

                            if (empdb != null)
                            {
                                var startTime = Convert.ToDateTime(empdb.StartTime);
                                var endTime = Convert.ToDateTime(empdb.EndTime);

                                TimeSpan timeSpan = endTime - startTime;
                                TimeSpan timeclockspan = clockoutvar - clockinvar;

                                var durationInMinutes = timeSpan.TotalMinutes;
                                var durationInMinutesclock = timeclockspan.TotalMinutes;
                                if (durationInMinutesclock > durationInMinutes)
                                {
                                    // Filter out the EmployeeID from view_EmployeeClocktime
                                    Employee.view_ServicesClocktime = Employee.view_ServicesClocktime
                                        .Where(x => !x.EmployeeID.Equals(empidvar))
                                        .ToList();
                                }
                            }

                        }

                    }
                    /////////////////////////////////////////////////////////////////////////////////////////
                    ///


                   // var EmpTran = db.TbServicesTransaction.Where(x => x.EmployeeID.Equals(empid) && x.TransactionDate == TransactionDateVar && x.Line.Equals(obj.LineID) && x.SectionID.Equals(sectionsplit[0].Trim()) && x.ServicesID.Equals(servicesplit[0].Trim())).ToList();
                    // check TbServicesTransaction == Null ?

                  //  if (EmpTran.Count() != 0)
                   // {

                        //Update Transaction


                        //Empdb = db.TbServicesTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.TransactionDate == TransactionDateVar && x.SectionID.Equals(sectionsplit[0].Trim()) && x.ServicesID.Equals(servicesplit[0].Trim())).SingleOrDefault();
                        //Empdb.ClockIn = obj.ClockIn.ToString();
                        //Empdb.Rate = -1;
                        //Empdb.UpdateBy = EmpID;
                        //Empdb.UpdateDate = DateTime.Now;
                       // db.SaveChanges();

                  //  }
                  //  else
                  //  {

                        var empdetails = db.TbEmployeeMaster.Where(x => x.EmployeeID == empid.Trim() && x.PlantID.Equals(PlantID)).SingleOrDefault();

                        // Insert new Line               
                        if (!string.IsNullOrEmpty(obj.ClockIn))
                        {
                            var startt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID)).Select(x => x.StartTime).SingleOrDefault();
                            var Endt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID)).Select(x => x.EndTime).SingleOrDefault();
                            var Prefixt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.Prefix).SingleOrDefault();
                            var serviceID = db.TbService.Where(x => x.ServicesName.Equals(servicesplit[1].Trim()) && x.ServicesStatus.Equals(1) && x.PlantID.Equals(PlantID) && LineID.Equals(obj.LineID) && x.SectionID.Equals(sectionsplit[0].Trim())).Select(x=>x.ServicesID).SingleOrDefault(); 
                            
                            if(serviceID == null)
                            {
                                 serviceID = db.TbService.Where(x => x.ServicesID.Equals(servicesplit[0].Trim()) && x.ServicesStatus.Equals(1) && x.PlantID.Equals(PlantID) && LineID.Equals(obj.LineID)).Select(x => x.ServicesID).SingleOrDefault();
                                
                            }
                            
                            db.TbServicesTransaction.Add(new TbServicesTransaction()
                            {
                                TransactionDate = Convert.ToDateTime(TransactionDateVar),
                                EmployeeID = empid,
                                Plant = PlantID,
                                Shift = empdetails.ShiftID,
                                Prefix = Prefixt,
                                StartTime = startt,
                                EndTime = Endt,
                                Line = obj.LineID,//obj.LineName,
                                SectionID = sectionsplit[0].Trim(),
                                SectionName = sectionsplit[1].Trim(),
                                ServicesID = serviceID,
                                ServicesName = servicesplit[1].Trim(),
                                WorkingStatus = "Working",
                                ClockIn = obj.ClockIn,
                                ClockOut = "",
                                Rate = -1,
                                Remark = remark,
                                BreakFlag = "",
                                StatusClocktime = "",
                                CreateDate = DateTime.Now,
                                CreateBy = EmpID,//User.Identity.Name,
                                UpdateDate = DateTime.Now,
                                UpdateBy = EmpID//User.Identity.Name,
                            });


                        }
                        else
                        {
                            var startt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID)).Select(x => x.StartTime).SingleOrDefault();
                            var Endt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID)).Select(x => x.EndTime).SingleOrDefault();
                            db.TbServicesTransaction.Add(new TbServicesTransaction()
                            {
 
                                TransactionDate = Convert.ToDateTime(TransactionDateVar),
                                EmployeeID = empid,
                                Shift = empdetails.ShiftID,
                                StartTime = startt,
                                EndTime = Endt,
                                Line = obj.LineID,//obj.LineName,
                                ClockIn = obj.ClockIn,
                                Rate = -1,
                                CreateDate = DateTime.Now,
                                CreateBy = EmpID,//User.Identity.Name,
                                UpdateDate = DateTime.Now,
                                UpdateBy = EmpID,//User.Identity.Name,
                            });



                        }
                       
                 //   }
                }
            }
            db.SaveChanges();
            return RedirectToAction("ServicesClockIn");


        }

        [HttpGet]
        public ActionResult ServicesClockInSaveCurrent(View_ServicesClocktime obj, string[] EmployeeIDchk, string TableData, string LineID, string SectionSelect, string TransactionDateFillter) //  , string action
        {

            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var TransactionDateVar = Convert.ToDateTime(TransactionDateFillter); // obj.TransactionDate;

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals("1")).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID) && x.ServicesStatus.Equals(1)).ToList(),
                view_ServicesClocktime = db.View_ServicesClocktime.Where(x => x.PlantID.Equals(PlantID)).OrderBy(x => x.ShiftID).ThenBy(x => x.EmployeeID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),

            };
            ViewBag.VBRoleServicesClockIn = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(18)).Select(x => x.RoleAction).FirstOrDefault();

            // Create Function
            //int datacnt = EmployeeIDchk.Count();
            List<TableDataRow> tableRows = JsonConvert.DeserializeObject<List<TableDataRow>>(TableData);




            string[] EmployeeIDchksplite = EmployeeIDchk[0].Split(",");
            int datacnt = EmployeeIDchksplite.Count();


            for (int i = 0; i < datacnt; ++i)
            {
                string empid = EmployeeIDchksplite[i];
                foreach (var itmservice in tableRows)
                {
                    var Empdb = new TbServicesTransaction();
                    // string empid = EmployeeIDchk[i];

                    var sectionsplit = itmservice.Section.Split(":").ToList();
                    var servicesplit = itmservice.Service.Split(":").ToList();
                    var remark = "";
                    if (SectionSelect == "All")
                    {
                        remark = "All";
                    }
                    else
                    {
                        remark = "";
                    }

                    ////////////////////////// select service normal case //////////////////////////////////

                    var empdbcheck = db.TbEmployeeTransaction
                    .Where(x => x.TransactionDate.Equals(TransactionDateVar) &&
                      (!string.IsNullOrEmpty(x.ClockIn) ||
                       !string.IsNullOrEmpty(x.ClockOut)) && x.EmployeeID.Equals(empid))
                    .ToList();
                    if (empdbcheck.Count() != 0)
                    {
                        foreach (var itm in empdbcheck)
                        {
                            var empidvar = itm.EmployeeID;
                            var clockinvar = Convert.ToDateTime(itm.ClockIn);


                            var clockoutvar = Convert.ToDateTime(itm.ClockOut);
                            var empdb = db.View_Employee.FirstOrDefault(x => x.EmployeeID.Equals(empidvar));

                            if (empdb != null)
                            {
                                var startTime = Convert.ToDateTime(empdb.StartTime);
                                var endTime = Convert.ToDateTime(empdb.EndTime);

                                TimeSpan timeSpan = endTime - startTime;
                                TimeSpan timeclockspan = clockoutvar - clockinvar;

                                var durationInMinutes = timeSpan.TotalMinutes;
                                var durationInMinutesclock = timeclockspan.TotalMinutes;
                                if (durationInMinutesclock > durationInMinutes)
                                {
                                    // Filter out the EmployeeID from view_EmployeeClocktime
                                    Employee.view_ServicesClocktime = Employee.view_ServicesClocktime
                                        .Where(x => !x.EmployeeID.Equals(empidvar))
                                        .ToList();
                                }
                            }

                        }

                    }
                    /////////////////////////////////////////////////////////////////////////////////////////
                    ///


                    var EmpTran = db.TbServicesTransaction.Where(x => x.EmployeeID.Equals(empid) && x.TransactionDate == TransactionDateVar && x.Line.Equals(obj.LineID) && x.SectionID.Equals(sectionsplit[0].Trim()) && x.ServicesID.Equals(servicesplit[0].Trim())).ToList();
                    // check TbServicesTransaction == Null ?

                    if (EmpTran.Count() != 0)
                    {

                        //Update Transaction


                        //Empdb = db.TbServicesTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.TransactionDate == TransactionDateVar && x.SectionID.Equals(sectionsplit[0].Trim()) && x.ServicesID.Equals(servicesplit[0].Trim())).SingleOrDefault();
                        //Empdb.ClockIn = obj.ClockIn.ToString();
                        //Empdb.Rate = -1;
                        //Empdb.UpdateBy = EmpID;
                        //Empdb.UpdateDate = DateTime.Now;
                        // db.SaveChanges();

                    }
                    else
                    {

                        var empdetails = db.TbEmployeeMaster.Where(x => x.EmployeeID == empid.Trim() && x.PlantID.Equals(PlantID)).SingleOrDefault();

                        // Insert new Line               
                        if (!string.IsNullOrEmpty(obj.ClockIn))
                        {
                            var startt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID)).Select(x => x.StartTime).SingleOrDefault();
                            var Endt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID)).Select(x => x.EndTime).SingleOrDefault();
                            var Prefixt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.Prefix).SingleOrDefault();
                            var serviceID = db.TbService.Where(x => x.ServicesName.Equals(servicesplit[1].Trim()) && x.ServicesStatus.Equals(1) && x.PlantID.Equals(PlantID) && LineID.Equals(obj.LineID) && x.SectionID.Equals(sectionsplit[0].Trim())).Select(x => x.ServicesID).SingleOrDefault();

                            if (serviceID == null)
                            {
                                serviceID = db.TbService.Where(x => x.ServicesID.Equals(servicesplit[0].Trim()) && x.ServicesStatus.Equals(1) && x.PlantID.Equals(PlantID) && LineID.Equals(obj.LineID)).Select(x => x.ServicesID).SingleOrDefault();

                            }

                            db.TbServicesTransaction.Add(new TbServicesTransaction()
                            {
                                TransactionDate = Convert.ToDateTime(TransactionDateVar),
                                EmployeeID = empid,
                                Plant = PlantID,
                                Shift = empdetails.ShiftID,
                                Prefix = Prefixt,
                                StartTime = startt,
                                EndTime = Endt,
                                Line = obj.LineID,//obj.LineName,
                                SectionID = sectionsplit[0].Trim(),
                                SectionName = sectionsplit[1].Trim(),
                                ServicesID = serviceID,
                                ServicesName = servicesplit[1].Trim(),
                                WorkingStatus = "Working",
                                ClockIn = obj.ClockIn,
                                ClockOut = "",
                                Rate = -1,
                                Remark = remark,
                                BreakFlag = "",
                                StatusClocktime = "",
                                CreateDate = DateTime.Now,
                                CreateBy = EmpID,//User.Identity.Name,
                                UpdateDate = DateTime.Now,
                                UpdateBy = EmpID//User.Identity.Name,
                            });


                        }
                        else
                        {
                            var startt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID)).Select(x => x.StartTime).SingleOrDefault();
                            var Endt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID)).Select(x => x.EndTime).SingleOrDefault();
                            db.TbServicesTransaction.Add(new TbServicesTransaction()
                            {

                                TransactionDate = Convert.ToDateTime(TransactionDateVar),
                                EmployeeID = empid,
                                Shift = empdetails.ShiftID,
                                StartTime = startt,
                                EndTime = Endt,
                                Line = obj.LineID,//obj.LineName,
                                ClockIn = obj.ClockIn,
                                Rate = -1,
                                CreateDate = DateTime.Now,
                                CreateBy = EmpID,//User.Identity.Name,
                                UpdateDate = DateTime.Now,
                                UpdateBy = EmpID,//User.Identity.Name,
                            });



                        }

                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("ServicesClockIn");


        }


        [HttpPost]
        public IActionResult ServicesClockInCheck(View_ServicesClocktime obj, string[] EmployeeIDchk, string TableData, string LineID, string SectionSelect, string TransactionDateFillter) //  , string action
        {

            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var TransactionDateVar = obj.TransactionDate;
            // var TransactionDateVar = DateTime.Today;

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals("1")).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID) && x.ServicesStatus.Equals(1)).ToList(),
                view_ServicesClocktime = db.View_ServicesClocktime.Where(x => x.PlantID.Equals(PlantID)).OrderBy(x => x.ShiftID).ThenBy(x => x.EmployeeID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),

            };
            ViewBag.VBRoleServicesClockIn = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(18)).Select(x => x.RoleAction).FirstOrDefault();

            // Create Function
            int datacnt = EmployeeIDchk.Count();
            // decimal rateservicecheck = 0.0;
            //Check Date and Time
            if (obj.TransactionDate == DateTime.MinValue || obj.ClockIn == null)
            {
                
                return Json(new { success = false, message = "Please fill time and date to clock-in!" });
             

            }
            List<TableDataRow> tableRows = JsonConvert.DeserializeObject<List<TableDataRow>>(TableData);
            var distinctServices = new HashSet<decimal>();
            int j = 0;
            //check service rate before Add
            foreach (var itmservice in tableRows)
            {
                var servicesplit = itmservice.Service.Split(":");
                var servicerate = db.TbService.Where(x => x.PlantID.Equals(PlantID) && x.LineID.Equals(LineID) && x.ServicesName.Equals(servicesplit[1])).Select(x => x.ServicesRate).SingleOrDefault();
                decimal rate = Convert.ToDecimal(servicerate);
                if (servicesplit.Length > 0)
                {
                    // Add the first part of the service split to the HashSet
                    distinctServices.Add(rate);
                }

                j++;
            }

            if (distinctServices.Count > 1)
            {
                return Json(new { success = false, message = "Rate Differnence !" });
             
            }


            for (int i = 0; i < datacnt; ++i)
            {

                foreach (var itmservice in tableRows)
                {
                    var Empdb = new TbServicesTransaction();
                    string empid = EmployeeIDchk[i];

                    var sectionsplit = itmservice.Section.Split(":").ToList();
                    var servicesplit = itmservice.Service.Split(":").ToList();
                    var remark = "";
                    if (SectionSelect == "All")
                    {
                        remark = "All";
                    }
                    else
                    {
                        remark = "";
                    }

                    ////////////////////////// select service normal case //////////////////////////////////

                    var empdbcheck = db.TbEmployeeTransaction
                    .Where(x => x.TransactionDate.Equals(TransactionDateVar) &&
                      (!string.IsNullOrEmpty(x.ClockIn) ||
                       !string.IsNullOrEmpty(x.ClockOut)) && x.EmployeeID.Equals(empid))
                    .ToList();
                    if (empdbcheck.Count() != 0)
                    {
                        foreach (var itm in empdbcheck)
                        {
                            var empidvar = itm.EmployeeID;
                            var clockinvar = Convert.ToDateTime(itm.ClockIn);
                            if (itm.ClockOut == "")
                            {
                                return Json(new { success = false, message = "Please Employee Clock out Employee ID :" + empidvar + "Date :" + empdbcheck.First().TransactionDate });
                         
                            }


                            var empcheckduplicate = db.View_EmployeeClocktime.Where(x => x.EmployeeID.Equals(empid) && x.ClockIn == obj.ClockIn.ToString() && x.ClockIn != x.ClockOut && x.TransactionDate.Equals(TransactionDateVar)).ToList();


                            // Fetch data as much as possible using SQL
                            var data = db.View_EmployeeClocktime
                                .Where(x => x.EmployeeID.Equals(empid)
                                            && x.TransactionDate.Equals(TransactionDateVar) && x.ClockIn != "" && x.ClockOut != "")
                                .ToList(); 

                            // Perform the DateTime comparison in memory
                            var empcheckduplicate2 = data.Where(x => DateTime.Parse(x.ClockIn) <= DateTime.Parse(obj.ClockIn)
                                            && DateTime.Parse(obj.ClockIn) <= DateTime.Parse(x.ClockOut)).ToList();

                            if (empcheckduplicate.Count != 0 && empcheckduplicate2.Count != 0)
                            {
                                return Json(new { success = false, message = "Clock in Duplicate , Employee :" + empid + " Date :" + empcheckduplicate.First().TransactionDate});
                            }



                        }

                    }

                }
            }
            return Json(new { success = true });

        }







        [HttpGet]
        public ActionResult ServicesClockInSaveBackup(View_ServicesClocktime obj, string[] EmployeeIDchk, string TableData, string LineID, string SectionSelect, string TransactionDateFillter) //  , string action
        {

            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var TransactionDateVar = obj.TransactionDate;
            // var TransactionDateVar = DateTime.Today;

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals("1")).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID) && x.ServicesStatus.Equals(1)).ToList(),
                view_ServicesClocktime = db.View_ServicesClocktime.Where(x => x.PlantID.Equals(PlantID)).OrderBy(x => x.ShiftID).ThenBy(x => x.EmployeeID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),

            };
            ViewBag.VBRoleServicesClockIn = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(18)).Select(x => x.RoleAction).FirstOrDefault();

            // Create Function
            int datacnt = EmployeeIDchk.Count();
            // decimal rateservicecheck = 0.0;
            //Check Date and Time
            if (obj.TransactionDate == DateTime.MinValue || obj.ClockIn == null)
            {
                TempData["AlertMessage"] = "Please fill time and date to clock-in!";
                return View("ServicesClockIn", Employee);

            }
            List<TableDataRow> tableRows = JsonConvert.DeserializeObject<List<TableDataRow>>(TableData);
            var distinctServices = new HashSet<decimal>();
            int j = 0;
            //check service rate before Add
            foreach (var itmservice in tableRows)
            {
                var servicesplit = itmservice.Service.Split(":");
                var servicerate = db.TbService.Where(x => x.PlantID.Equals(PlantID) && x.LineID.Equals(LineID) && x.ServicesName.Equals(servicesplit[1])).Select(x => x.ServicesRate).SingleOrDefault();
                decimal rate = Convert.ToDecimal(servicerate);
                if (servicesplit.Length > 0)
                {
                    // Add the first part of the service split to the HashSet
                    distinctServices.Add(rate);
                }

                j++;
            }

            if (distinctServices.Count > 1)
            {
                TempData["AlertMessage"] = "Rate Differnence !";
                return View("ServicesClockIn", Employee);
            }


            for (int i = 0; i < datacnt; ++i)
            {

                foreach (var itmservice in tableRows)
                {
                    var Empdb = new TbServicesTransaction();
                    string empid = EmployeeIDchk[i];

                    var sectionsplit = itmservice.Section.Split(":").ToList();
                    var servicesplit = itmservice.Service.Split(":").ToList();
                    var remark = "";
                    if (SectionSelect == "All")
                    {
                        remark = "All";
                    }
                    else
                    {
                        remark = "";
                    }

                    ////////////////////////// select service normal case //////////////////////////////////

                    var empdbcheck = db.TbEmployeeTransaction
                    .Where(x => x.TransactionDate.Equals(TransactionDateVar) &&
                      (!string.IsNullOrEmpty(x.ClockIn) ||
                       !string.IsNullOrEmpty(x.ClockOut)) && x.EmployeeID.Equals(empid))
                    .ToList();
                    if (empdbcheck.Count() != 0)
                    {
                        foreach (var itm in empdbcheck)
                        {
                            var empidvar = itm.EmployeeID;
                            var clockinvar = Convert.ToDateTime(itm.ClockIn);
                            if (itm.ClockOut == "")
                            {
                                TempData["AlertMessage"] = "Please Employee Clock out Employee ID :" + empidvar + "Date :" + empdbcheck.First().TransactionDate;
                                return RedirectToAction("ServicesClockIn");
                            }


                            var empcheckduplicate = db.View_EmployeeClocktime.Where(x => x.EmployeeID.Equals(empid) && x.ClockIn == obj.ClockIn.ToString() && x.ClockIn != x.ClockOut && x.TransactionDate.Equals(TransactionDateVar)).ToList();


                            // Fetch data as much as possible using SQL
                            var data = db.View_EmployeeClocktime
                                .Where(x => x.EmployeeID.Equals(empid)
                                            && x.TransactionDate.Equals(TransactionDateVar) && x.ClockIn != "" && x.ClockOut != "")
                                .ToList(); // This brings the data into memory

                            // Perform the DateTime comparison in memory
                            var empcheckduplicate2 = data.Where(x => DateTime.Parse(x.ClockIn) <= DateTime.Parse(obj.ClockIn)
                                            && DateTime.Parse(obj.ClockIn) <= DateTime.Parse(x.ClockOut)).ToList();

                            if (empcheckduplicate.Count != 0 && empcheckduplicate2.Count != 0)
                            {
                                TempData["AlertMessage"] = "Clock in Duplicate , Employee :" + empid + " Date :" + empcheckduplicate.First().TransactionDate;
                                return RedirectToAction("ServicesClockIn");
                            }


                            var clockoutvar = Convert.ToDateTime(itm.ClockOut);
                            var empdb = db.View_Employee.FirstOrDefault(x => x.EmployeeID.Equals(empidvar));

                            if (empdb != null)
                            {
                                var startTime = Convert.ToDateTime(empdb.StartTime);
                                var endTime = Convert.ToDateTime(empdb.EndTime);

                                // Calculate time span
                                TimeSpan timeSpan = endTime - startTime;
                                TimeSpan timeclockspan = clockoutvar - clockinvar;
                                // Now you have the time span, you can use it as needed


                                // var durationInHours = timeSpan.TotalHours;
                                var durationInMinutes = timeSpan.TotalMinutes;
                                // var durationInHours = timeSpan.TotalHours;
                                var durationInMinutesclock = timeclockspan.TotalMinutes;
                                if (durationInMinutesclock > durationInMinutes)
                                {
                                    // Filter out the EmployeeID from view_EmployeeClocktime
                                    Employee.view_ServicesClocktime = Employee.view_ServicesClocktime
                                        .Where(x => !x.EmployeeID.Equals(empidvar))
                                        .ToList();
                                }
                            }

                        }

                    }
                    /////////////////////////////////////////////////////////////////////////////////////////
                    ///


                    var EmpTran = db.TbServicesTransaction.Where(x => x.EmployeeID.Equals(empid) && x.TransactionDate == TransactionDateVar && x.Line.Equals(obj.LineID) && x.SectionID.Equals(sectionsplit[0].Trim()) && x.ServicesID.Equals(servicesplit[0].Trim())).ToList();
                    // check TbServicesTransaction == Null ?

                    if (EmpTran.Count() != 0)
                    {

                        //Update Transaction
                        Empdb = db.TbServicesTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.TransactionDate == TransactionDateVar && x.SectionID.Equals(sectionsplit[0].Trim()) && x.ServicesID.Equals(servicesplit[0].Trim())).SingleOrDefault();
                        Empdb.ClockIn = obj.ClockIn.ToString();
                        Empdb.Rate = -1;
                        Empdb.UpdateBy = EmpID;
                        Empdb.UpdateDate = DateTime.Now;
                        db.SaveChanges();

                    }
                    else
                    {

                        var empdetails = db.TbEmployeeMaster.Where(x => x.EmployeeID == empid.Trim() && x.PlantID.Equals(PlantID)).SingleOrDefault();

                        // Insert new Line               
                        if (!string.IsNullOrEmpty(obj.ClockIn))
                        {
                            var startt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID)).Select(x => x.StartTime).SingleOrDefault();
                            var Endt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID)).Select(x => x.EndTime).SingleOrDefault();
                            var Prefixt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.Prefix).SingleOrDefault();
                            var serviceID = db.TbService.Where(x => x.ServicesName.Equals(servicesplit[1].Trim()) && x.ServicesStatus.Equals(1) && x.PlantID.Equals(PlantID) && LineID.Equals(obj.LineID) && x.SectionID.Equals(sectionsplit[0].Trim())).Select(x => x.ServicesID).SingleOrDefault();

                            if (serviceID == null)
                            {
                                serviceID = db.TbService.Where(x => x.ServicesID.Equals(servicesplit[0].Trim()) && x.ServicesStatus.Equals(1) && x.PlantID.Equals(PlantID) && LineID.Equals(obj.LineID)).Select(x => x.ServicesID).SingleOrDefault();

                            }

                            db.TbServicesTransaction.Add(new TbServicesTransaction()
                            {
                                TransactionDate = Convert.ToDateTime(TransactionDateVar),
                                EmployeeID = empid,
                                Plant = PlantID,
                                Shift = empdetails.ShiftID,
                                Prefix = Prefixt,
                                StartTime = startt,
                                EndTime = Endt,
                                Line = obj.LineID,//obj.LineName,
                                SectionID = sectionsplit[0].Trim(),
                                SectionName = sectionsplit[1].Trim(),
                                ServicesID = serviceID,
                                ServicesName = servicesplit[1].Trim(),
                                WorkingStatus = "Working",
                                ClockIn = obj.ClockIn,
                                ClockOut = "",
                                Rate = -1,
                                Remark = remark,
                                BreakFlag = "",
                                StatusClocktime = "",
                                CreateDate = DateTime.Now,
                                CreateBy = EmpID,//User.Identity.Name,
                                UpdateDate = DateTime.Now,
                                UpdateBy = EmpID//User.Identity.Name,
                            });


                        }
                        else
                        {
                            var startt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID)).Select(x => x.StartTime).SingleOrDefault();
                            var Endt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID)).Select(x => x.EndTime).SingleOrDefault();
                            db.TbServicesTransaction.Add(new TbServicesTransaction()
                            {

                                TransactionDate = Convert.ToDateTime(TransactionDateVar),
                                EmployeeID = empid,
                                Shift = empdetails.ShiftID,
                                StartTime = startt,
                                EndTime = Endt,
                                Line = obj.LineID,//obj.LineName,
                                ClockIn = obj.ClockIn,
                                Rate = -1,
                                CreateDate = DateTime.Now,
                                CreateBy = EmpID,//User.Identity.Name,
                                UpdateDate = DateTime.Now,
                                UpdateBy = EmpID,//User.Identity.Name,
                            });



                        }

                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("ServicesClockIn");


        }


        //2.  Function service Clock in Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult SericesClockInEdit(int ID, string ServicesID, string SectionID, int TransactionNo)
        {


            //var todayDate = DateTime.Today.ToString("yyyy-MM-dd");  // Use the format "yyyy-MM-dd" to match the expected format
            // var EmplID = db.View_ServicesClocktime.Where(x => x.ID == ID && x.SectionID.Equals(SectionID) && x.TransactionDate.Equals(TranDate) && x.ServicesID.Equals(ServicesID)).Select(x => x.EmployeeID).SingleOrDefault();

            //  DateTime parsedTodayDate = DateTime.ParseExact(todayDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);




            //var EmpsCount = db.TbServicesTransaction
            //      .Where(x => x.EmployeeID.Equals(EmplID) && x.SectionID.Equals(SectionID) && x.ServicesID.Equals(ServicesID) && (x.TransactionDate.Date.Equals(parsedTodayDate.Date) && x.ClockOut == "" || x.TransactionDate.Date.Equals(parsedTodayDate.Date.AddDays(-1)) && x.ClockOut == ""))
            //      .ToList();


            //if (EmpsCount.Count > 1)
            //{
            //    return Json(new { success = false, message = "Please contact IT some data not clock out.Please check. : " });

            //}

            //var Emps = db.TbServicesTransaction
            //        .Where(x => x.EmployeeID.Equals(EmplID) && x.SectionID.Equals(SectionID) && x.ServicesID.Equals(ServicesID) && ( x.TransactionDate.Date.Equals(parsedTodayDate) && x.ClockOut == "" || x.TransactionDate.Date.Equals(parsedTodayDate.Date.AddDays(-1)) && x.ClockOut == ""))
            //        .SingleOrDefault();

            //  var ServiceIDdb = db.View_ServicesClocktime.Where(x => x.ID.Equals(ID) && x.SectionID.Equals(SectionID) && x.TransactionDate.Equals(TranDate) && x.ServicesID.Equals(ServicesID)).SingleOrDefault();

            var Emps = db.TbServicesTransaction.Where(x => x.TransactionNo.Equals(TransactionNo)).SingleOrDefault();




            return Json(Emps);
        }




        [HttpPost]
        public ActionResult ServicesClockInUpdate(View_ServicesClocktime obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            // 1. check TbEmployeeTransaction == Null ?
            var Empdb = new TbServicesTransaction();
            //var startDate = obj.TransactionDate.Date;
            //var endDate = startDate.AddDays(1);
            //var EmployeeID = obj.EmployeeID.Trim();
            var EmpTran = db.TbServicesTransaction.Where(x => x.TransactionNo.Equals(obj.TransactionNo) && x.Plant.Equals(PlantID) && x.EmployeeID.Equals(obj.EmployeeID) && x.TransactionDate.Equals(obj.TransactionDate.Date)).ToList();
            if (EmpTran.Count() != 0)
            {
                //Update TbServicesTransaction
                Empdb = db.TbServicesTransaction.Where(x => x.Plant.Equals(PlantID) && x.TransactionNo.Equals(obj.TransactionNo)).SingleOrDefault();
                if (obj.TransactionDate != DateTime.MinValue)
                {
                    Empdb.TransactionDate = Convert.ToDateTime(obj.TransactionDate);
                }

                Empdb.ClockIn = obj.ClockIn;
                Empdb.UpdateBy = EmpID;//User.Identity.Name,
                Empdb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }
            else
            {
                TempData["AlertMessage"] = "Please Clock In !";


            }

            return RedirectToAction("ServicesClockIn");

        }



        public ActionResult ServicesClockInClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals("1")).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID) && x.ServicesStatus.Equals(1)).ToList(),
                view_ServicesClocktime = db.View_ServicesClocktime.Where(x => x.PlantID.Equals(PlantID)).OrderBy(x => x.ID).ThenBy(x=>x.ShiftID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //  tbServicesTransaction = db.TbServicesTransaction.Where(x => x.TransactionDate == DateTime.Now && x.Plant.Equals(PlantID)),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //  view_Employee = db.View_Employee.ToList()

            };

            // ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
            Employee.view_ServicesClocktime = Employee.view_ServicesClocktime.Where(p => p.TransactionDate.Equals(DateTime.Today) || p.TransactionDate.Equals(DateTime.MinValue)).ToList();
            ViewBag.VBRoleServicesClockIn = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(18)).Select(x => x.RoleAction).FirstOrDefault();

            return View("ServicesClockIn", Employee);

        }


        [HttpGet]
        public IActionResult FilterSectionByLine(string LineID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            // Replace this with your logic to filter products based on the lineId

            if (PlantID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            // var emp = db.View_EmployeeClocktime.Where(x=>x.EmployeeID == selectedEmpID && x.PlantID.Equals(PlantID) && x.TransactionDate == DateTime.Today ).ToList();

            // Query with time-based filtering in addition to date and other conditions
            var sectioncheck = db.View_PLPS
                .Where(x => x.LineID == LineID
                            && x.PlantID.Equals(PlantID) && x.Status.Equals(1))
                 .GroupBy(x => new { x.PlantID, x.LineID, x.SectionID, x.SectionName })
                 .Select(group => new
                 {
                     SectionID = group.Key.SectionID,
                     SectionName = group.Key.SectionName

                 }).ToList();

            if (sectioncheck.Count() == 0)
            {

                //TempData["AlertMessage"] = "Data haven't clockin or Data already clock out. : " + selectedEmpID ;
                //return View("WorkingFunction", mymodel);

                return Json(new { success = false, message = "Section no data." });

            }


            return Json(sectioncheck);
        }




        [HttpGet]
        public IActionResult FilterServiceByLine(string LineID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            // Replace this with your logic to filter products based on the lineId

            if (PlantID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var sectioncheck = db.TbService
            .Where(x => x.LineID == LineID
               && x.PlantID.Equals(PlantID) && x.ServicesStatus.Equals(1))
                .GroupBy(x => new { x.PlantID, x.LineID, x.ServicesName })
                    .Select(group => new
                    {
                        serviceid = group.Max(g => g.ServicesID),  // Select the maximum ServiceID
                        servicename = group.Key.ServicesName
                    })
                    .ToList();


            if (sectioncheck.Count() == 0)
            {

                //TempData["AlertMessage"] = "Data haven't clockin or Data already clock out. : " + selectedEmpID ;
                //return View("WorkingFunction", mymodel);

                return Json(new { success = false, message = "Service no data." });

            }




            return Json(sectioncheck);
        }








        [HttpGet]
        public IActionResult FilterServiceBySection(string LineID , string SectionID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            // Replace this with your logic to filter products based on the lineId

            if (PlantID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            // var emp = db.View_EmployeeClocktime.Where(x=>x.EmployeeID == selectedEmpID && x.PlantID.Equals(PlantID) && x.TransactionDate == DateTime.Today ).ToList();

            // Query with time-based filtering in addition to date and other conditions
            var sectioncheck = db.TbService
                .Where(x => x.LineID == LineID && x.SectionID == SectionID
                            && x.PlantID.Equals(PlantID) && x.ServicesStatus.Equals(1))
                 .GroupBy(x => new { x.PlantID, x.LineID, x.SectionID,x.ServicesID, x.ServicesName })
                 .Select(group => new
                 {
                     serviceid = group.Key.ServicesID,
                     servicename = group.Key.ServicesName

                 }).ToList();

            if (sectioncheck.Count() == 0)
            {

                //TempData["AlertMessage"] = "Data haven't clockin or Data already clock out. : " + selectedEmpID ;
                //return View("WorkingFunction", mymodel);

                return Json(new { success = false, message = "Service no data! " });

            }




            return Json(sectioncheck);
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Services Clock out Function
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ServicesClockOut(View_ServicesClocktime obj, string[] EmployeeIDchk, string[] ID, string[] EmployeeIDlist, string TransactionDateFillter, string action)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            //var TransactionDateVar = obj.TransactionDate;
            var TransactionDateVar = DateTime.Today;

            if (string.IsNullOrEmpty(TransactionDateFillter))
            {
                TransactionDateFillter = DateTime.Today.ToString("yyyy-MM-dd");
            }

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_ServicesClocktime = db.View_ServicesClocktime.Where(x => x.PlantID.Equals(PlantID)).OrderBy(x => x.ShiftID).ThenBy(x=>x.EmployeeID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                // tbServicesTransaction = db.TbServicesTransaction.Where(x =>  x.Plant.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //  view_Employee = db.View_Employee.ToList()
            };
            ViewBag.VBRoleServicesClockOut = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(19)).Select(x => x.RoleAction).FirstOrDefault();
            //if (EmployeeIDchk.Count() == 0)  //if (action == "Search" || action == "ServicesClockOut")
            //{

                if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.SectionName) || !string.IsNullOrEmpty(TransactionDateFillter))
                {
                    if (!string.IsNullOrEmpty(obj.EmployeeID))
                    {
                        ViewBag.SelectedEmpID = obj.EmployeeID;
                        mymodel.view_ServicesClocktime = mymodel.view_ServicesClocktime.Where(p => p.EmployeeID == obj.EmployeeID).ToList();
                    }
                    if (!string.IsNullOrEmpty(obj.LineName))
                    {
                        ViewBag.SelectedLineName = obj.LineName;
                        mymodel.view_ServicesClocktime = mymodel.view_ServicesClocktime.Where(p => p.LineName == obj.LineName).ToList();
                    }
                    if (!string.IsNullOrEmpty(obj.SectionName))
                    {
                        ViewBag.SelectedSectionName = obj.SectionName;
                        mymodel.view_ServicesClocktime = mymodel.view_ServicesClocktime.Where(p => p.SectionID == obj.SectionName).ToList();
                    }
                    if (!string.IsNullOrEmpty(TransactionDateFillter) && Convert.ToDateTime(TransactionDateFillter) != DateTime.Today)
                    {

                        if (DateTime.TryParse(TransactionDateFillter, out DateTime dateFilter))
                        {
                            ViewBag.SelectedTransactionDate = dateFilter.ToString("yyyy-MM-dd");
                            mymodel.view_ServicesClocktime = mymodel.view_ServicesClocktime.Where(p => p.TransactionDate.Date == dateFilter.Date).ToList();
                        }
                    }
                    else if (Convert.ToDateTime(TransactionDateFillter) == DateTime.Today)
                    {
                        ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
                        mymodel.view_ServicesClocktime = mymodel.view_ServicesClocktime.Where(p => p.TransactionDate.Equals(DateTime.Today)).ToList();

                    }
                    else
                    {

                    }
                    return View(mymodel);
                }
                else
                {

                    //ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
                    mymodel.view_ServicesClocktime = mymodel.view_ServicesClocktime.Where(p => p.TransactionDate.Equals(DateTime.Today) || p.TransactionDate.Equals(DateTime.MinValue)).ToList();

                    return View(mymodel);
                }
           // }
            //else
            //{
            //    // Create Function
            //    int datacnt = EmployeeIDchk.Count();

            //    for (int i = 0; i < datacnt; ++i)
            //    {
            //        var Empdb = new TbServicesTransaction();
            //        string empid = EmployeeIDchk[i];
            //        if (empid != "on")
            //        {


            //            // var EmpClockView = db.TbEmployeeMaster.Where(x => x.ID.Equals(Convert.ToInt32(empid))).Select(x => x.EmployeeID).SingleOrDefault();
            //            var ViewEmpTran = db.View_ServicesClocktime.Where(x => x.TransactionNo.Equals(Convert.ToInt32(empid))).First();

            //            var EmpTran = db.TbServicesTransaction.Where(x => x.TransactionNo.Equals(Convert.ToInt32(empid))).SingleOrDefault();
            //            // check clockout beofre endtime?
            //            var EmpMaster = db.View_EmployeeMaster.Where(x => x.EmployeeID.Equals(EmpTran.EmployeeID) && x.PlantID.Equals(PlantID)).SingleOrDefault();

            //            var startTime = Convert.ToDateTime(EmpMaster.StartTime);
            //            var endTime = Convert.ToDateTime(EmpMaster.EndTime);
            //            var clockinvar = Convert.ToDateTime(EmpTran.ClockIn);
            //            var clockoutvar = Convert.ToDateTime(obj.ClockOut);


            //            // Calculate time span
            //            TimeSpan timeSpan = endTime - startTime;
            //            TimeSpan timeclockspan = clockoutvar - clockinvar;
            //            // Now you have the time span, you can use it as needed


            //            // var durationInHours = timeSpan.TotalHours;
            //            var durationInMinutes = timeSpan.TotalMinutes;
            //            // var durationInHours = timeSpan.TotalHours;
            //            var durationInMinutesclock = timeclockspan.TotalMinutes;
            //            var workingvar = "";
            //            //if (durationInMinutesclock < durationInMinutes)
            //            //{
            //            //    if (obj.WorkingStatus == null)
            //            //    {
            //            //        workingvar = "Rotate";
            //            //    }
            //            //    else
            //            //    { workingvar = obj.WorkingStatus; }

            //            //}
            //            //else
            //            //{
            //            //    workingvar = "Working";
            //            //}
            //            //  var EmpTran = db.TbServicesTransaction.Where(x => x.TransactionNo ==Convert.ToInt32(tranNo)).SingleOrDefault();
            //            // check TbServicesTransaction == Null ?

            //            if (EmpTran != null && !string.IsNullOrEmpty(EmpTran.ClockIn))
            //            {
            //                //Update Transaction
            //                EmpTran.ClockOut = obj.ClockOut.ToString();
            //                EmpTran.WorkingStatus = obj.WorkingStatus;
            //                EmpTran.UpdateBy = EmpID;
            //                EmpTran.UpdateDate = DateTime.Now;
            //                db.SaveChanges();

            //            }
            //        }
            //    }
            //    return RedirectToAction("ServicesClockOut");

            //}



        }


        public ActionResult ServicesClockOutSave(View_ServicesClocktime obj, string[] EmployeeIDchk, string[] ID, string[] EmployeeIDlist, string TransactionDateFillter, string action)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            //var TransactionDateVar = obj.TransactionDate;
            var TransactionDateVar = DateTime.Today;

            if (string.IsNullOrEmpty(TransactionDateFillter))
            {
                TransactionDateFillter = DateTime.Today.ToString("yyyy-MM-dd");
            }

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_ServicesClocktime = db.View_ServicesClocktime.Where(x => x.PlantID.Equals(PlantID)).OrderBy(x => x.ShiftID).ThenBy(x => x.EmployeeID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                // tbServicesTransaction = db.TbServicesTransaction.Where(x =>  x.Plant.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //  view_Employee = db.View_Employee.ToList()
            };
            ViewBag.VBRoleServicesClockOut = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(19)).Select(x => x.RoleAction).FirstOrDefault();
            if (EmployeeIDchk.Length == 0)
            {
                return RedirectToAction("ServicesClockOut");

            }
            else
            {
                // Create Function
                int datacnt = EmployeeIDchk.Count();



                for (int i = 0; i < datacnt; ++i)
                {
                    var Empdb = new TbServicesTransaction();
                    string empid = EmployeeIDchk[i];
                    if (empid != "on")
                    {


                        // var EmpClockView = db.TbEmployeeMaster.Where(x => x.ID.Equals(Convert.ToInt32(empid))).Select(x => x.EmployeeID).SingleOrDefault();
                        var ViewEmpTran = db.View_ServicesClocktime.Where(x => x.TransactionNo.Equals(Convert.ToInt32(empid))).First();

                        var EmpTran = db.TbServicesTransaction.Where(x => x.TransactionNo.Equals(Convert.ToInt32(empid))).SingleOrDefault();
                        // check clockout beofre endtime?
                        var EmpMaster = db.View_EmployeeMaster.Where(x => x.EmployeeID.Equals(EmpTran.EmployeeID) && x.PlantID.Equals(PlantID)).SingleOrDefault();



                        var startTime = Convert.ToDateTime(EmpMaster.StartTime);
                        var endTime = Convert.ToDateTime(EmpMaster.EndTime);
                        var clockinvar = Convert.ToDateTime(EmpTran.ClockIn);
                        var clockoutvar = Convert.ToDateTime(obj.ClockOut);


                        // Calculate time span
                        TimeSpan timeSpan = endTime - startTime;
                        TimeSpan timeclockspan = clockoutvar - clockinvar;

                        var durationInMinutes = timeSpan.TotalMinutes;
                        var durationInMinutesclock = timeclockspan.TotalMinutes;
                        var workingvar = "";


                        //if (durationInMinutesclock < durationInMinutes)
                        //{
                        //    if (obj.WorkingStatus == null)
                        //    {
                        //        workingvar = "Rotate";
                        //    }
                        //    else
                        //    { workingvar = obj.WorkingStatus; }

                        //}
                        //else
                        //{
                        //    workingvar = "Working";
                        //}
                        //  var EmpTran = db.TbServicesTransaction.Where(x => x.TransactionNo ==Convert.ToInt32(tranNo)).SingleOrDefault();
                        // check TbServicesTransaction == Null ?

                        if (EmpTran != null && !string.IsNullOrEmpty(EmpTran.ClockIn))
                        {

                            //get rate
                            var ratenow = db.TbService.Where(x=>x.PlantID.Equals(PlantID) 
                            && x.ServicesID.Equals(EmpTran.ServicesID)).Select(x=>x.ServicesRate).FirstOrDefault();

                            //Update Transaction
                            EmpTran.ClockOut = obj.ClockOut.ToString();
                            EmpTran.Rate = ratenow;
                            EmpTran.WorkingStatus = obj.WorkingStatus;
                            EmpTran.UpdateBy = EmpID;
                            EmpTran.UpdateDate = DateTime.Now;
                            db.SaveChanges();

                        }
                    }
                }
                return RedirectToAction("ServicesClockOut");
            }

        }


        //2.  Function service Clock in Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult ServicesClockOutEdit(int ID, string ServicesID, string SectionID, int TransactionNo)
        {

            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            //var todayDate = DateTime.Today.ToString("yyyy-MM-dd");  // Use the format "yyyy-MM-dd" to match the expected format
            //var EmplID = db.View_ServicesClocktime.Where(x => x.TransactionNo == ID ).Select(x => x.EmployeeID).SingleOrDefault();

            //DateTime parsedTodayDate = DateTime.ParseExact(todayDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //  var EmplID = db.View_ServicesClocktime.Where(x => x.ID == ID && x.SectionID.Equals(SectionID) && x.TransactionDate.Equals(TranDate) && x.ServicesID.Equals(ServicesID)).Select(x => x.EmployeeID).SingleOrDefault();

            //  var ServiceIDdb = db.View_ServicesClocktime.Where(x => x.ID.Equals(ID) && x.SectionID.Equals(SectionID) && x.TransactionDate.Equals(TranDate) && x.ServicesID.Equals(ServicesID)).SingleOrDefault();

            var Emps = db.TbServicesTransaction.Where(x => x.TransactionNo.Equals(TransactionNo)).SingleOrDefault();


            //var Emps = db.TbServicesTransaction
            //    .Where(x => x.TransactionNo.Equals(ID) && x.TransactionDate.Date.Equals(TranDate) && x.ServicesID.Equals(ServicesID) && x.SectionID.Equals(SectionID))
            //    .SingleOrDefault();

            return Json(Emps);
        }



        [HttpPost]
        public ActionResult ServicesClockOutUpdate(View_ServicesClocktime obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            // 1. check TbEmployeeTransaction == Null ?
            var Empdb = new TbServicesTransaction();

            var EmpTran = db.TbServicesTransaction.Where(x => x.TransactionNo.Equals(obj.TransactionNo) && x.EmployeeID.Equals(obj.EmployeeID) && x.TransactionDate == obj.TransactionDate).ToList();
            if (EmpTran.Count() != 0 && obj.ClockOut != null)
            {
                //Update TbServicesTransaction
                Empdb = db.TbServicesTransaction.Where(x => x.TransactionNo.Equals(obj.TransactionNo)).SingleOrDefault();
                if (obj.TransactionDate != DateTime.MinValue)
                {
                    Empdb.TransactionDate = Convert.ToDateTime(obj.TransactionDate);
                }

                Empdb.ClockOut = obj.ClockOut;
                Empdb.UpdateBy = EmpID;//User.Identity.Name,
                Empdb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }
            else
            {
                TempData["AlertMessage"] = "Please Clock time !";


            }

            return RedirectToAction("ServicesClockOut");

        }


        public ActionResult ServicesClockOutClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_ServicesClocktime = db.View_ServicesClocktime.Where(x => x.PlantID.Equals(PlantID)).OrderBy(x => x.ShiftID).ThenBy(x => x.EmployeeID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //  tbServicesTransaction = db.TbServicesTransaction.Where(x => x.TransactionDate == DateTime.Now && x.Plant.Equals(PlantID)),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                // view_Employee = db.View_Employee.ToList()

            };
            ViewBag.VBRoleServicesClockOut = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(19)).Select(x => x.RoleAction).FirstOrDefault();

            // ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
            Employee.view_ServicesClocktime = Employee.view_ServicesClocktime.Where(p => p.TransactionDate.Equals(DateTime.Today) || p.TransactionDate.Equals(DateTime.MinValue)).ToList();

            return View("ServicesClockOut", Employee);

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        /// <summary>
        /// EmployeeLeaveHoliday Function
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EmployeeLeaveHoliday(View_EmployeeLeaveHolidayClocktime obj, string[] EmployeeIDchk, string WorkingstatusCreate, string remarkcreate, DateTime TreansactionDateCreate, string StartTime, string EndTime)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeMaster = db.View_EmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeLeaveHoliday = db.TbEmployeeLeaveHoliday.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeLeaveHolidayClocktime = db.View_EmployeeLeaveHolidayClocktime.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.TransactionDate).ThenBy(x => x.EmployeeID).ToList(),
            };
            ViewBag.VBRoleEmployeeLeaveHoliday = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(21)).Select(x => x.RoleAction).FirstOrDefault();

            if (EmployeeIDchk.Length == 0)
            {
                if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName) || obj.TransactionDate.HasValue != false || !string.IsNullOrEmpty(obj.WorkingStatus))
                {

                    if (!string.IsNullOrEmpty(obj.EmployeeID))
                    {
                        mymodel.view_EmployeeLeaveHolidayClocktime = mymodel.view_EmployeeLeaveHolidayClocktime.Where(p => p.EmployeeID == obj.EmployeeID).ToList();
                        ViewBag.SelectedEmployeeID = obj.EmployeeID;
                    }
                    if (!string.IsNullOrEmpty(obj.LineName))
                    {
                        mymodel.view_EmployeeLeaveHolidayClocktime = mymodel.view_EmployeeLeaveHolidayClocktime.Where(p => p.LineName == obj.LineName).ToList();
                        ViewBag.SelectedLineName = obj.LineName;
                    }

                    if (obj.TransactionDate.HasValue != false)
                    {
                        ViewBag.SelectedTransactionDate = obj.TransactionDate.Value.ToString("yyyy-MM-dd");
                        mymodel.view_EmployeeLeaveHolidayClocktime = mymodel.view_EmployeeLeaveHolidayClocktime.Where(p => p.TransactionDate == obj.TransactionDate).ToList();
                        
                    }
                    if (!string.IsNullOrEmpty(obj.WorkingStatus))
                    {
                        mymodel.view_EmployeeLeaveHolidayClocktime = mymodel.view_EmployeeLeaveHolidayClocktime.Where(p => p.WorkingStatus == obj.WorkingStatus).ToList();
                        ViewBag.SelectedWorkingStatus = obj.WorkingStatus;
                    }


                    return View(mymodel);

                }
                else
                {

                    return View(mymodel);
                }

            }
            else
            {

                // Create Function
                int datacnt = EmployeeIDchk.Count();
                for (int i = 0; i < datacnt; ++i)
                {

                    // 1. check TbEmployeeTransaction == Null ?
                    var Empdb = new TbEmployeeLeaveHoliday();
                    //var Emp = db.TbEmployeeMasters.Where(x => x.EmployeeID.Equals(EmployeeID[i]));
                    string empid = EmployeeIDchk[i];
                    var varclockin = "";
                    var varclockout = "";
                    //Check clockin first
                    var EmpCheckclock = db.View_ClockTime.Where(x => x.EmployeeID.Equals(empid) && x.WorkingStatus == "Working" && x.ClockIn != ""  && x.ClockOut == "").Select(x=>x.TransactionDate).ToList();

                    if(EmpCheckclock.Count > 0)
                    {
                        TempData["AlertMessage"] = "Please Clock Out ! Date : " + EmpCheckclock.FirstOrDefault();
                        return RedirectToAction("EmployeeLeaveHoliday");
                    }
                    




                    //Check Insert or Update
                    var EmpTran = db.TbEmployeeLeaveHoliday.Where(x => x.EmployeeID.Equals(empid) && x.TransactionDate == TreansactionDateCreate && (x.WorkingStatus == "Leave" || x.WorkingStatus == "Holiday")).ToList();
                    if (StartTime == null && EndTime == null)
                    {
                        var varWorking = db.View_Employee.Where(x => x.EmployeeID.Equals(empid))
                        .Select(x => new
                        {
                            StartTime = x.StartTime,
                            EndTime = x.EndTime
                        }).ToList();

                        foreach (var item in varWorking)
                        {
                            varclockin = item.StartTime;
                            varclockout = item.EndTime;
                        }
                    }
                    else
                    {

                        varclockin = StartTime;
                        varclockout = EndTime;
                    }



                    // Fetch data as much as possible using SQL
                    var data = db.View_EmployeeClocktime
                        .Where(x => x.EmployeeID.Equals(empid)
                                    && x.TransactionDate.Equals(TreansactionDateCreate))
                        .ToList(); // This brings the data into memory

                    // Perform the DateTime comparison in memory
                    var empcheckduplicate2 = data.Where(x => DateTime.Parse(x.ClockIn) <= DateTime.Parse(varclockin)
                                    && DateTime.Parse(varclockin) <= DateTime.Parse(x.ClockOut)).ToList();

                    if(empcheckduplicate2.Count != 0)
                    {
                        TempData["AlertMessage"] = "Clock time duplicate! ";
                        return RedirectToAction("EmployeeLeaveHoliday");
                    }


                    if (EmpTran.Count() != 0)
                    {
                        //Update Transaction
                        Empdb = db.TbEmployeeLeaveHoliday.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.TransactionDate == TreansactionDateCreate).SingleOrDefault();
                        Empdb.ClockIn = varclockin;
                        Empdb.ClockOut = varclockout;
                        Empdb.WorkingStatus = WorkingstatusCreate;
                        Empdb.Remark = remarkcreate;
                        Empdb.UpdateBy = EmpID;//User.Identity.Name;
                        Empdb.UpdateDate = DateTime.Now;
                        db.SaveChanges();

                    }
                    else
                    {

                        var empdetails = db.TbEmployeeMaster.Where(x => x.EmployeeID == empid.Trim()).SingleOrDefault();


                        if (StartTime == null && EndTime == null)
                        {
                            // Case First No Clock out
                            db.TbEmployeeLeaveHoliday.Add(new TbEmployeeLeaveHoliday()
                            {
                                // TransactionNo = db.TbEmployeeLeaveHoliday.Count() + 1,
                                TransactionDate = TreansactionDateCreate,
                                EmployeeID = empid,
                                PlantID = PlantID,
                                ShiftID = empdetails.ShiftID,
                                LineID = empdetails.LineID,//obj.LineName,
                                SectionID = empdetails.SectionID,
                                ClockIn = varclockin,
                                ClockOut = varclockout,
                                WorkingStatus = WorkingstatusCreate,
                                Remark = remarkcreate,
                                CreateDate = DateTime.Now,
                                CreateBy = EmpID,//User.Identity.Name,
                                UpdateDate = DateTime.Now,
                                UpdateBy = EmpID,//User.Identity.Name,
                            });

                        }
                        else
                        {


                            db.TbEmployeeLeaveHoliday.Add(new TbEmployeeLeaveHoliday()
                            {
                                // TransactionNo = db.TbEmployeeLeaveHoliday.Count() + 1,
                                TransactionDate = TreansactionDateCreate,
                                EmployeeID = empid,
                                ShiftID = empdetails.ShiftID,
                                PlantID = PlantID,
                                LineID = empdetails.LineID,//obj.LineName,
                                SectionID = empdetails.SectionID,
                                WorkingStatus = WorkingstatusCreate,
                                ClockIn = StartTime,
                                ClockOut = EndTime,
                                Remark = remarkcreate,
                                CreateDate = DateTime.Now,
                                CreateBy = EmpID,//User.Identity.Name,
                                UpdateDate = DateTime.Now,
                                UpdateBy = EmpID//User.Identity.Name,
                            });



                        }
                        db.SaveChanges();

                    }
                }

                return RedirectToAction("EmployeeLeaveHoliday");
            }

        }





        public ActionResult EmployeeLeaveHolidayClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeMaster = db.View_EmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeLeaveHoliday = db.TbEmployeeLeaveHoliday.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeLeaveHolidayClocktime = db.View_EmployeeLeaveHolidayClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
            };
            ViewBag.VBRoleEmployeeLeaveHoliday = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(21)).Select(x => x.RoleAction).FirstOrDefault();
            return RedirectToAction("EmployeeLeaveHoliday");

        }

        //2.  Function service Clock in Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult EmployeeLeaveHolidayEdit(int ID)
        {

            var Emps = db.TbEmployeeLeaveHoliday
                 .Where(x => x.TransactionNo.Equals(ID))
                 .SingleOrDefault();
            return Json(Emps);
        }





        [HttpPost]
        public ActionResult EmployeeLeaveHolidayUpdate(TbEmployeeTransaction obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            // 1. check TbEmployeeTransaction == Null ?
            var Empdb = new TbEmployeeLeaveHoliday();

            var EmpTran = db.TbEmployeeLeaveHoliday.Where(x => x.EmployeeID.Equals(obj.EmployeeID) && x.TransactionNo == obj.TransactionNo).ToList();
            if (EmpTran.Count() != 0 )
            {
                //Update Transaction
                Empdb = db.TbEmployeeLeaveHoliday.Where(x => x.EmployeeID == obj.EmployeeID && x.TransactionNo == obj.TransactionNo).SingleOrDefault();
                Empdb.TransactionDate = obj.TransactionDate;
                Empdb.Remark = obj.Remark;
                Empdb.WorkingStatus = obj.WorkingStatus;
                Empdb.ClockIn = obj.ClockIn;
                Empdb.ClockOut = obj.ClockOut;
                Empdb.UpdateBy = EmpID;//User.Identity.Name,
                Empdb.UpdateDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("EmployeeLeaveHoliday");
            }
            else
            {
                TempData["AlertMessage"] = "Please Clock In !";


            }

            return RedirectToAction("EmployeeLeaveHoliday");

        }




        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////









        /// <summary>
        /// Employee Adjust Line
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="EmployeeIDchk"></param>
        /// <returns></returns>

        [HttpGet]
        public ActionResult EmployeeAdjustLine(View_EmployeeClocktime obj, string[] EmployeeIDchk, string StartTime, string EndTime, string ToLine, string ToSection, string FromLine, DateTime TransactionDateFillter, string SectionIDFillter, string ToSectionIDFillter)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.TransactionDate == DateTime.Now),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                // view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //view_EmployeeAdjustLine = db.View_EmployeeAdjustLine.Where(x => x.PlantID.Equals(PlantID))
                //.OrderByDescending(x => x.TransactionDate)
                // .ThenBy(x => x.SectionID)
                //.ThenBy(x => x.EmployeeID)
                //.ToList(),
                view_EmployeeAdjustLine = db.View_EmployeeAdjustLine.Where(x => x.PlantID.Equals(PlantID))
                .OrderBy(x => x.ClockIn != "" && x.ClockOut == "" ? 1 :
                  x.TransactionDate == new DateTime(1, 1, 1) ? 2 :
                  x.ClockIn != "" && x.ClockOut != "" ? 3 : 4)
                .ThenByDescending(x => x.TransactionDate)
    .ThenBy(x => x.SectionID).ThenBy(x => x.ShiftID)
    .ThenBy(x => x.EmployeeID).ToList(),
                view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),

            };
            ViewBag.VBRoleEmployeeAdjustLine = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(31)).Select(x => x.RoleAction).FirstOrDefault();


            if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(SectionIDFillter) || TransactionDateFillter != DateTime.MinValue)
            {

                if (!string.IsNullOrEmpty(obj.EmployeeID))
                {
                    ViewBag.SelectedEmpID = obj.EmployeeID;
                    Employee.view_EmployeeAdjustLine = Employee.view_EmployeeAdjustLine.Where(p => p.EmployeeID == obj.EmployeeID).ToList();
                }
                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    ViewBag.SelectedLineName = obj.LineName;
                    Employee.view_EmployeeAdjustLine = Employee.view_EmployeeAdjustLine.Where(p => p.LineName == obj.LineName).ToList();
                }
                if (!string.IsNullOrEmpty(SectionIDFillter))
                {
                    ViewBag.SelectedSectionID = SectionIDFillter;
                    Employee.view_EmployeeAdjustLine = Employee.view_EmployeeAdjustLine.Where(p => p.FromSectionID == SectionIDFillter).ToList();
                }
                if (!string.IsNullOrEmpty(ToSectionIDFillter))
                {
                    ViewBag.SelectedToSectionID = ToSectionIDFillter;
                    Employee.view_EmployeeAdjustLine = Employee.view_EmployeeAdjustLine.Where(p => p.SectionID == ToSectionIDFillter).ToList();
                }
                if (TransactionDateFillter != DateTime.Today && Convert.ToDateTime(TransactionDateFillter) == DateTime.MinValue)
                {

                    // ViewBag.SelectedTransactionDate = TransactionDateFillter;
                    //   Employee.view_EmployeeAdjustLine = Employee.view_EmployeeAdjustLine.Where(p => p.TransactionDate.Date.Equals(TransactionDateFillter.ToString("yyyy-MM-dd"))).ToList();

                }
                else if (Convert.ToDateTime(TransactionDateFillter) == DateTime.Today)
                {
                    ViewBag.SelectedTransactionDateFillter = DateTime.Today.ToString("yyyy-MM-dd");

                    Employee.view_EmployeeAdjustLine = Employee.view_EmployeeAdjustLine.Where(p => p.TransactionDate.Date.Equals(DateTime.Today)).ToList();

                }
                else
                {
                    var startDate = TransactionDateFillter.Date;
                    var endDate = TransactionDateFillter.AddDays(1);
                    ViewBag.SelectedTransactionDateFillter = TransactionDateFillter;
                    Employee.view_EmployeeAdjustLine = Employee.view_EmployeeAdjustLine.Where(p => p.TransactionDate >= startDate && p.TransactionDate < endDate).ToList();

                }


                return View(Employee);

            }
            else
            {

                Employee.view_EmployeeAdjustLine = Employee.view_EmployeeAdjustLine.Where(p => p.TransactionDate.Equals(DateTime.Today) || p.TransactionDate.Equals(DateTime.MinValue)).ToList();

                return View(Employee);
            }




        }



        [HttpGet]
        public ActionResult EmployeeAdjustLineSave(View_EmployeeClocktime obj, string[] EmployeeIDchk,  string StartTime, string EndTime, string ToLine, string ToSection, string FromLine, DateTime TransactionDate)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

        
            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.Plant.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                // view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_Employee = db.View_Employee,
                //view_EmployeeAdjustLine = db.View_EmployeeAdjustLine.Where(x => x.PlantID.Equals(PlantID))
                // .OrderByDescending(x => x.TransactionDate)
                //.ThenBy(x => x.EmployeeID).ToList(),
                view_EmployeeAdjustLine = db.View_EmployeeAdjustLine.Where(x => x.PlantID.Equals(PlantID))
                .OrderBy(x => x.ClockIn != "" && x.ClockOut == "" ? 1 :
                  x.TransactionDate == new DateTime(1, 1, 1) ? 2 :
                  x.ClockIn != "" && x.ClockOut != "" ? 3 : 4)
                .ThenByDescending(x => x.TransactionDate)
    .ThenBy(x => x.SectionID).ThenBy(x => x.SectionID)
    .ThenBy(x => x.EmployeeID).ToList(),

                view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),


            };
            ViewBag.VBRoleEmployeeAdjustLine = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(31)).Select(x => x.RoleAction).FirstOrDefault();

          
            // Create Function
            var thisday = TransactionDate; // DateTime.Now.Date;
            int datacnt = EmployeeIDchk.Count();
            for (int i = 0; i < datacnt; ++i)
            {
                var Empdb = new TbEmployeeTransaction();
               
                //string empidfillter = EmployeeIDchk[i];
                string[] empidfillter = EmployeeIDchk[i].Split(":");
                string empid = "";
                DateTime empdate ;

                if (empidfillter[1] == "0")
                {
                    // Insert new
                    var emp = Employee.view_EmployeeAdjustLine.Where(x => x.EmployeeID.Equals(empidfillter[0]) && x.TransactionNo.Equals(0)).Select(x => new { x.EmployeeID, x.TransactionDate }).SingleOrDefault();
                    empid = emp.EmployeeID;
                    empdate = emp.TransactionDate;
                }
                else
                {
                    //update
                    var emp = Employee.view_EmployeeAdjustLine.Where(x => x.TransactionNo.Equals(Convert.ToInt32(empidfillter[1]))).Select(x => new { x.EmployeeID, x.TransactionDate }).SingleOrDefault();
                    empid = emp.EmployeeID;
                    empdate = emp.TransactionDate;

                }

                //Check Toline Tosection
                var checklinesection = Employee.tbEmployeeMaster.Where(x => x.EmployeeID.Equals(empid) && x.LineID.Equals(ToLine) && x.SectionID.Equals(ToSection)).ToList();
                var checkclockinhold = db.View_ClockTime.Where(x => x.ClockIn != "" && x.ClockOut == "" && x.EmployeeID.Equals(empid)&& x.WorkingStatus.Equals("Working")).ToList();
              

                if (checklinesection.Count() > 0) //Check ว่า Section เดิมที่ผูกไว้ใน master หรือไม่
                {
                    TempData["AlertMessage"] = "Please clock in Employee Page!";
                    return RedirectToAction("EmployeeAdjustLine");
                }
                if (checkclockinhold.Count() > 1) // check ว่ามี adjust ค้างหรือไม่
                {
                    TempData["AlertMessage"] = "Please clock out. Employee : " + checkclockinhold.First().EmployeeID + " Date :" + checkclockinhold.First().TransactionDate;
                    return RedirectToAction("EmployeeAdjustLine");
                }

                List <TbEmployeeTransaction> EmpTran;
              

                //Check View_EmployeeAdjustLine เพื่อดูว่าเป้นการ clockoutใช่ไหม จาก EmployeeID , EndTime
                var Empadjustlist = Employee.view_EmployeeAdjustLine.Where(x => x.EmployeeID.Equals(empid) && x.ClockIn != "" && x.ClockOut == "" && x.WorkingStatus == "Working" && x.Remark == "Adjust" && x.TransactionNo.Equals(Convert.ToInt32(empidfillter[1]))).ToList();
                if (Empadjustlist.Count > 0)
                {
                    // Update 
                    //Check 
                    if(EndTime == null && StartTime == Empadjustlist.First().ClockIn )
                    {
                        TempData["AlertMessage"] = "Please Fill time clock out!";
                        return RedirectToAction("EmployeeAdjustLine");
                    }
                    if(StartTime != Empadjustlist.First().ClockIn && EndTime == null && StartTime != null)
                    {
                      
                            Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == empid && x.Plant.Equals(PlantID) && x.ClockOut == "" && x.Remark == "Adjust" && x.TransactionNo.Equals(Convert.ToInt32(empidfillter[1]))).SingleOrDefault();
                            Empdb.ClockIn = StartTime;
                            db.SaveChanges();
                        

                    }
                    if((StartTime != Empadjustlist.First().ClockIn && EndTime != null ) ||( StartTime == Empadjustlist.First().ClockIn  && EndTime != null))
                    {

                    var Empadjustlistrecord = Employee.view_EmployeeAdjustLine.Where(x => x.EmployeeID.Equals(empid) && x.ClockIn != "" && x.ClockOut == "" && x.WorkingStatus == "Working" && x.Remark == "Adjust").SingleOrDefault();


                        //var EmpProducttiondata = db.TbProductionTransaction.Where(x => x.TransactionDate.Equals(empdate) && x.QRCode == empid && x.PlantID.Equals(PlantID)).ToList();

                        //if (EmpProducttiondata.Count() == 0) //check พนักงานไม่มีการผลิต
                        //{
                        //    TempData["AlertMessage"] = "This Employee no data scan. " + empid;
                        //    return RedirectToAction("EmployeeAdjustLine", "Employee");
                        //}

                        //Check incentive
                        var incentiverateGrade = db.TbIncentiveMaster.Where(x => x.PlantID.Equals(PlantID) && x.SectionID.Equals(Empadjustlistrecord.SectionID) && x.LineID.Equals(Empadjustlistrecord.LineID))
                        .Select(x => new
                        {
                            x.Grade,
                            x.Rate,
                            x.ProductID,
                            x.Min,
                            x.Max
                        }).ToList();
                    // insert record

                    //check มีแล้วหรือยังถ้ามีแล้วไม่แอด 
                    foreach (var item in incentiverateGrade)
                    {
                      
                        var checkincentive = db.tbRateTransaction.Where(x => x.TransactionDate.Equals(Convert.ToDateTime(thisday))
                        && x.LineID.Equals(Empadjustlistrecord.LineID)
                        && x.SectionID.Equals(Empadjustlistrecord.SectionID)
                        && x.Grade.Equals(item.Grade)).ToList();


                        if (checkincentive.Count == 0)
                        {
                          
                            // Table : TbTransactionRate  Create
                            db.tbRateTransaction.Add(new TbRateTransaction()
                            {
                                TransactionDate = Convert.ToDateTime(thisday),
                                PlantID = PlantID,
                                LineID = Empadjustlistrecord.LineID,
                                SectionID = Empadjustlistrecord.SectionID,
                                ProductID = item.ProductID,
                                Type = "Employee",
                                Rate = item.Rate,
                                Grade = item.Grade,
                                Min = item.Min,
                                Max = item.Max,
                                CreateDate = DateTime.Now,
                                CreateBy = EmpID,
                                UpdateBy = EmpID,
                                UpdateDate = DateTime.Now
                            });
                            db.SaveChanges();

                        }


                    }


                    Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == empid && x.Plant.Equals(PlantID) && x.ClockOut == "" && x.Remark == "Adjust" && x.TransactionNo.Equals(Convert.ToInt32(empidfillter[1]))).SingleOrDefault();
                    Empdb.ClockOut = EndTime;
                    Empdb.WorkingStatus = "Rotate";
                    db.SaveChanges();
                    }
                }
                else
                {
                    //Insert
                    if (((StartTime == "" || StartTime == null) && (EndTime == "" || EndTime == null)) || (ToLine == "" || ToLine == null) || (ToSection == "" || ToSection == null) || TransactionDate == DateTime.MinValue || TransactionDate == null)
                    {
                        TempData["AlertMessage"] = "Please Fill data !";
                        return RedirectToAction("EmployeeAdjustLine");
                    }

                    var Empcheckclockout = Employee.tbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid)  && x.WorkingStatus == "Working" && x.ClockOut == "" && x.Remark != "Adjust").ToList();
                    if (StartTime == null)
                    {
                        EmpTran = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid) && x.Plant.Equals(PlantID) && x.ClockOut == "" && x.Remark == "Adjust").ToList();

                    }
                    else
                    {
                        var Empcheckclockoutadjust = Employee.tbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid)  && x.WorkingStatus == "Working" && x.ClockOut == "" && x.Remark == "Adjust").ToList();

                        if (Empcheckclockoutadjust.Count != 0)
                        {
                            TempData["AlertMessage"] = "Please Employee Clock out Employee Adjust :" + empid + " Date :" + Empcheckclockoutadjust.First().TransactionDate.Date;
                            return RedirectToAction("EmployeeAdjustLine");
                        }

                        EmpTran = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid) && x.TransactionDate == thisday && x.Plant.Equals(PlantID) && x.Line.Equals(ToLine) && x.Section.Equals(ToSection) && x.Remark == "Adjust" && x.TransactionNo.Equals(empidfillter[1])).ToList();

                    }

                    if (Empcheckclockout.Count != 0)
                    {
                        TempData["AlertMessage"] = "Please Employee Clock out Employee ID :" + empid + " Date : " + Empcheckclockout.First().TransactionDate.Date;
                        return RedirectToAction("EmployeeAdjustLine");
                    }
                    // If has data update
                    if (EmpTran.Count() != 0)
                    {

                        //Update Transaction
                        if (StartTime != null)
                        {

                            //Insert clockin
                            Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.TransactionDate == thisday && x.Plant.Equals(PlantID)).SingleOrDefault();

                            Empdb.ClockIn = StartTime;
                            Empdb.Line = ToLine;
                            Empdb.Section = ToSection;

                        }
                        if (EndTime != null)
                        {

                            //var EmpProducttiondata = db.TbProductionTransaction.Where(x => x.TransactionDate.Equals(empdate) && x.QRCode == empid && x.PlantID.Equals(PlantID)).ToList();

                            //if (EmpProducttiondata.Count() == 0) //check พนักงานไม่มีการผลิต
                            //{
                            //    TempData["AlertMessage"] = "This Employee no data scan. " + empid;
                            //    return RedirectToAction("EmployeeAdjustLine", "Employee");
                            //}

                            //Check incentive
                            var incentiverateGrade = db.TbIncentiveMaster.Where(x => x.PlantID.Equals(PlantID) && x.SectionID.Equals(ToSection) && x.LineID.Equals(ToLine))
                                .Select(x => new
                                {
                                    x.Grade,
                                    x.Rate,
                                    x.ProductID,
                                    x.Min,
                                    x.Max
                                }).ToList();
                            // insert record

                            //check มีแล้วหรือยังถ้ามีแล้วไม่แอด 
                            foreach (var item in incentiverateGrade)
                            {

                                var checkincentive = db.tbRateTransaction.Where(x => x.TransactionDate.Equals(Convert.ToDateTime(thisday))
                                && x.LineID.Equals(ToLine)
                                && x.SectionID.Equals(ToSection)
                                && x.Grade.Equals(item.Grade)).ToList();


                                if (checkincentive.Count == 0)
                                {
                                    // Table : TbTransactionRate  Create
                                    db.tbRateTransaction.Add(new TbRateTransaction()
                                    {
                                        TransactionDate = Convert.ToDateTime(thisday),
                                        PlantID = PlantID,
                                        LineID = ToLine,
                                        SectionID = ToSection,
                                        ProductID = item.ProductID,
                                        Type = "Employee",
                                        Rate = item.Rate,
                                        Grade = item.Grade,
                                        Min = item.Min,
                                        Max = item.Max,
                                        CreateDate = DateTime.Now,
                                        CreateBy = EmpID,
                                        UpdateBy = EmpID,
                                        UpdateDate = DateTime.Now
                                    });
                                    db.SaveChanges();

                                }


                            }


                            Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.Plant.Equals(PlantID) && x.ClockOut == "" && x.Remark == "Adjust").SingleOrDefault();
                            Empdb.ClockOut = EndTime;
                            Empdb.WorkingStatus = "Rotate";
                        }
                        if (StartTime != null && EndTime != null)
                        {

                            //var EmpProducttiondata = db.TbProductionTransaction.Where(x => x.TransactionDate.Equals(empdate) && x.QRCode == empid && x.PlantID.Equals(PlantID)).ToList();

                            //if (EmpProducttiondata.Count() == 0) //check พนักงานไม่มีการผลิต
                            //{
                            //    TempData["AlertMessage"] = "This Employee no data scan. " + empid;
                            //    return RedirectToAction("EmployeeAdjustLine", "Employee");
                            //}

                            //Check incentive
                            var incentiverateGrade = db.TbIncentiveMaster.Where(x => x.PlantID.Equals(PlantID) && x.SectionID.Equals(ToSection) && x.LineID.Equals(ToLine))
                                .Select(x => new
                                {
                                    x.Grade,
                                    x.Rate,
                                    x.ProductID,
                                    x.Min,
                                    x.Max
                                }).ToList();
                            // insert record

                            //check มีแล้วหรือยังถ้ามีแล้วไม่แอด 
                            foreach (var item in incentiverateGrade)
                            {

                                var checkincentive = db.tbRateTransaction.Where(x => x.TransactionDate.Equals(Convert.ToDateTime(thisday))
                                && x.LineID.Equals(ToLine)
                                && x.SectionID.Equals(ToSection)
                                && x.Grade.Equals(item.Grade)).ToList();


                                if (checkincentive.Count == 0)
                                {
                                    // Table : TbTransactionRate  Create
                                    db.tbRateTransaction.Add(new TbRateTransaction()
                                    {
                                        TransactionDate = Convert.ToDateTime(thisday),
                                        PlantID = PlantID,
                                        LineID = ToLine,
                                        SectionID = ToSection,
                                        ProductID = item.ProductID,
                                        Type = "Employee",
                                        Rate = item.Rate,
                                        Grade = item.Grade,
                                        Min = item.Min,
                                        Max = item.Max,
                                        CreateDate = DateTime.Now,
                                        CreateBy = EmpID,
                                        UpdateBy = EmpID,
                                        UpdateDate = DateTime.Now
                                    });
                                    db.SaveChanges();

                                }


                            }

                            Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.TransactionDate == thisday && x.Plant.Equals(PlantID)).SingleOrDefault();
                            Empdb.ClockIn = StartTime;
                            Empdb.ClockOut = EndTime;
                            Empdb.Line = ToLine;
                            Empdb.Section = ToSection;

                        }


                        Empdb.UpdateBy = EmpID;//User.Identity.Name;
                        Empdb.UpdateDate = DateTime.Now;
                        db.SaveChanges();

                    }
                    else
                    {
                        // Insert Transaction เพราะ Empclock adjust ไม่มี

                        var empdetails = db.TbEmployeeMaster.Where(x => x.EmployeeID == empid.Trim() && x.PlantID.Equals(PlantID)).SingleOrDefault();
                        // Create Transaction
                        var empshift = db.View_EmployeeMaster.Where(x => x.EmployeeID == empid.Trim() && x.PlantID.Equals(PlantID)).SingleOrDefault();
                        if (StartTime == "" || StartTime == null)
                        {
                            TempData["AlertMessage"] = "Please Employee Clock out Employee ID :" + empid;
                            return RedirectToAction("EmployeeAdjustLine");
                        }

                        int counttran = db.TbEmployeeTransaction.Where(x => x.EmployeeID == empid.Trim() && x.Plant.Equals(PlantID) && x.ClockOut == "").ToList().Count;
                        if (counttran == 0)
                        {

                            //Case with clock out
                            db.TbEmployeeTransaction.Add(new TbEmployeeTransaction()
                            {

                                TransactionDate = thisday, // Convert.ToDateTime(obj.TransactionDate),
                                EmployeeID = empid,
                                Shift = empdetails.ShiftID,
                                Plant = PlantID,
                                Line = ToLine,//obj.LineName,
                                Section = ToSection,
                                WorkingStatus = "Working",
                                Prefix = empshift.Prefix,
                                Remark = "Adjust",
                                BreakFlag = "",
                                StartTime = empshift.StartTime,
                                EndTime = empshift.EndTime,
                                ClockIn = StartTime,
                                ClockOut = "",
                                CreateDate = DateTime.Now,
                                CreateBy = EmpID,//User.Identity.Name,
                                UpdateDate = DateTime.Now,
                                UpdateBy = EmpID//User.Identity.Name,
                            });

                        }
                        }
                        db.SaveChanges();
                    

                }

            }
            Employee.view_EmployeeAdjustLine = Employee.view_EmployeeAdjustLine.Where(p => p.TransactionDate.Equals(DateTime.Today) || p.TransactionDate.Equals(DateTime.MinValue)).ToList();

            return RedirectToAction("EmployeeAdjustLine");

        }



        [HttpGet]
        public ActionResult EmployeeAdjustLineSavecurrent(View_EmployeeClocktime obj, string[] EmployeeIDchk, string StartTime, string EndTime, string ToLine, string ToSection, string FromLine, DateTime TransactionDate)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.Plant.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                // view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_Employee = db.View_Employee,
                //view_EmployeeAdjustLine = db.View_EmployeeAdjustLine.Where(x => x.PlantID.Equals(PlantID))
                // .OrderByDescending(x => x.TransactionDate)
                //.ThenBy(x => x.EmployeeID).ToList(),
                view_EmployeeAdjustLine = db.View_EmployeeAdjustLine.Where(x => x.PlantID.Equals(PlantID))
                .OrderBy(x => x.ClockIn != "" && x.ClockOut == "" ? 1 :
                  x.TransactionDate == new DateTime(1, 1, 1) ? 2 :
                  x.ClockIn != "" && x.ClockOut != "" ? 3 : 4)
                .ThenByDescending(x => x.TransactionDate)
    .ThenBy(x => x.SectionID).ThenBy(x => x.SectionID)
    .ThenBy(x => x.EmployeeID).ToList(),

                view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),


            };
            ViewBag.VBRoleEmployeeAdjustLine = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(31)).Select(x => x.RoleAction).FirstOrDefault();

            if (((StartTime == "" || StartTime == null) && (EndTime == "" || EndTime == null)) || (ToLine == "" || ToLine == null) || (ToSection == "" || ToSection == null) || TransactionDate == DateTime.MinValue || TransactionDate == null)
            {
                TempData["AlertMessage"] = "Please Fill data !";
                return RedirectToAction("EmployeeAdjustLine");
            }
            // Create Function
            var thisday = TransactionDate; // DateTime.Now.Date;
            int datacnt = EmployeeIDchk.Count();
            for (int i = 0; i < datacnt; ++i)
            {
                var Empdb = new TbEmployeeTransaction();
                string empid = EmployeeIDchk[i];
                // var EmpTran = new object();
                List<TbEmployeeTransaction> EmpTran;
                //Check View_EmployeeAdjustLine เพื่อดูว่าเป้นการ clockoutใช่ไหม จาก EmployeeID , EndTime
                //var Empadjustlist = Employee.view_EmployeeAdjustLine.Where(x => x.EmployeeID.Equals(empid) && x.ClockIn != "" && x.ClockOut == "" && x.WorkingStatus == "Working" && x.Remark == "Adjust").ToList();
                //if(Empadjustlist.Count > 0)
                //{
                //    // Update 
                //    //Check 
                //}
                //else
                //{
                //    //Insert



                //}

                var Empcheckclockout = Employee.tbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid) && x.TransactionDate == thisday && x.WorkingStatus == "Working" && x.ClockOut == "" && x.Remark != "Adjust").ToList();
                if (StartTime == null)
                {
                    EmpTran = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid) && x.Plant.Equals(PlantID) && x.ClockOut == "" && x.Remark == "Adjust").ToList();

                }
                else
                {
                    var Empcheckclockoutadjust = Employee.tbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid) && x.TransactionDate == thisday && x.WorkingStatus == "Working" && x.ClockOut == "" && x.Remark == "Adjust").ToList();

                    if (Empcheckclockoutadjust.Count != 0)
                    {
                        TempData["AlertMessage"] = "Please Employee Clock out Employee Adjust :" + empid;
                        return RedirectToAction("EmployeeAdjustLine");
                    }

                    EmpTran = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid) && x.TransactionDate == thisday && x.Plant.Equals(PlantID) && x.Line.Equals(ToLine) && x.Section.Equals(ToSection) && x.Remark == "Adjust").ToList();

                }

                if (Empcheckclockout.Count != 0)
                {
                    TempData["AlertMessage"] = "Please Employee Clock out Employee ID :" + empid;
                    return RedirectToAction("EmployeeAdjustLine");
                }
                // If has data update
                if (EmpTran.Count() != 0)
                {

                    //Update Transaction
                    if (StartTime != null)
                    {

                        //Insert clockin
                        Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.TransactionDate == thisday && x.Plant.Equals(PlantID)).SingleOrDefault();

                        Empdb.ClockIn = StartTime;
                        Empdb.Line = ToLine;
                        Empdb.Section = ToSection;
                    }
                    if (EndTime != null)
                    {

                        //Check incentive
                        var incentiverateGrade = db.TbIncentiveMaster.Where(x => x.PlantID.Equals(PlantID) && x.SectionID.Equals(ToSection) && x.LineID.Equals(ToLine))
                            .Select(x => new
                            {
                                x.Grade,
                                x.Rate,
                                x.ProductID,
                                x.Min,
                                x.Max
                            }).ToList();
                        // insert record

                        //check มีแล้วหรือยังถ้ามีแล้วไม่แอด 
                        foreach (var item in incentiverateGrade)
                        {

                            var checkincentive = db.tbRateTransaction.Where(x => x.TransactionDate.Equals(Convert.ToDateTime(thisday))
                            && x.LineID.Equals(ToLine)
                            && x.SectionID.Equals(ToSection)
                            && x.Grade.Equals(item.Grade)).ToList();


                            if (checkincentive.Count == 0)
                            {
                                // Table : TbTransactionRate  Create
                                db.tbRateTransaction.Add(new TbRateTransaction()
                                {
                                    TransactionDate = Convert.ToDateTime(thisday),
                                    PlantID = PlantID,
                                    LineID = ToLine,
                                    SectionID = ToSection,
                                    ProductID = item.ProductID,
                                    Type = "Employee",
                                    Rate = item.Rate,
                                    Grade = item.Grade,
                                    Min = item.Min,
                                    Max = item.Max,
                                    CreateDate = DateTime.Now,
                                    CreateBy = EmpID,
                                    UpdateBy = EmpID,
                                    UpdateDate = DateTime.Now
                                });
                                db.SaveChanges();

                            }


                        }


                        Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.Plant.Equals(PlantID) && x.ClockOut == "" && x.Remark == "Adjust").SingleOrDefault();
                        Empdb.ClockOut = EndTime;
                    }
                    if (StartTime != null && EndTime != null)
                    {


                        //Check incentive
                        var incentiverateGrade = db.TbIncentiveMaster.Where(x => x.PlantID.Equals(PlantID) && x.SectionID.Equals(ToSection) && x.LineID.Equals(ToLine))
                            .Select(x => new
                            {
                                x.Grade,
                                x.Rate,
                                x.ProductID,
                                x.Min,
                                x.Max
                            }).ToList();
                        // insert record

                        //check มีแล้วหรือยังถ้ามีแล้วไม่แอด 
                        foreach (var item in incentiverateGrade)
                        {

                            var checkincentive = db.tbRateTransaction.Where(x => x.TransactionDate.Equals(Convert.ToDateTime(thisday))
                            && x.LineID.Equals(ToLine)
                            && x.SectionID.Equals(ToSection)
                            && x.Grade.Equals(item.Grade)).ToList();


                            if (checkincentive.Count == 0)
                            {
                                // Table : TbTransactionRate  Create
                                db.tbRateTransaction.Add(new TbRateTransaction()
                                {
                                    TransactionDate = Convert.ToDateTime(thisday),
                                    PlantID = PlantID,
                                    LineID = ToLine,
                                    SectionID = ToSection,
                                    ProductID = item.ProductID,
                                    Type = "Employee",
                                    Rate = item.Rate,
                                    Grade = item.Grade,
                                    Min = item.Min,
                                    Max = item.Max,
                                    CreateDate = DateTime.Now,
                                    CreateBy = EmpID,
                                    UpdateBy = EmpID,
                                    UpdateDate = DateTime.Now
                                });
                                db.SaveChanges();

                            }


                        }

                        Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.TransactionDate == thisday && x.Plant.Equals(PlantID)).SingleOrDefault();
                        Empdb.ClockIn = StartTime;
                        Empdb.ClockOut = EndTime;
                        Empdb.Line = ToLine;
                        Empdb.Section = ToSection;

                    }


                    Empdb.UpdateBy = EmpID;//User.Identity.Name;
                    Empdb.UpdateDate = DateTime.Now;
                    db.SaveChanges();

                }
                else
                {

                    var empdetails = db.TbEmployeeMaster.Where(x => x.EmployeeID == empid.Trim() && x.PlantID.Equals(PlantID)).SingleOrDefault();
                    // Create Transaction
                    var empshift = db.View_EmployeeMaster.Where(x => x.EmployeeID == empid.Trim() && x.PlantID.Equals(PlantID)).SingleOrDefault();
                    if (StartTime == "" || StartTime == null)
                    {
                        TempData["AlertMessage"] = "Please Employee Clock out Employee ID :" + empid;
                        return RedirectToAction("EmployeeAdjustLine");
                    }


                    //Case with clock out
                    db.TbEmployeeTransaction.Add(new TbEmployeeTransaction()
                    {
                        // TransactionNo = db.TbEmployeeTransaction.Count() + 1,
                        TransactionDate = thisday, // Convert.ToDateTime(obj.TransactionDate),
                        EmployeeID = empid,
                        Shift = empdetails.ShiftID,
                        Plant = PlantID,
                        Line = ToLine,//obj.LineName,
                        Section = ToSection,
                        WorkingStatus = "Working",
                        Prefix = empshift.Prefix,
                        Remark = "Adjust",
                        BreakFlag = "",
                        StartTime = empshift.StartTime,
                        EndTime = empshift.EndTime,
                        ClockIn = StartTime,
                        ClockOut = "",
                        CreateDate = DateTime.Now,
                        CreateBy = EmpID,//User.Identity.Name,
                        UpdateDate = DateTime.Now,
                        UpdateBy = EmpID//User.Identity.Name,
                    }); ;

                    // }

                }
                db.SaveChanges();

            }
            Employee.view_EmployeeAdjustLine = Employee.view_EmployeeAdjustLine.Where(p => p.TransactionDate.Equals(DateTime.Today) || p.TransactionDate.Equals(DateTime.MinValue)).ToList();

            return RedirectToAction("EmployeeAdjustLine");

        }




        public IActionResult FilterSectionsByLine(string lineId)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));


            var mymodel = new ViewModelAll
            {
                view_Incentive = db.View_Incentive.ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.view_Incentive = mymodel.view_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList();

            }


            //var filteredSections = mymodel.view_PLPS.Where(x => x.LineID.Equals(lineId))
            //         .Select(x => new
            //         {
            //             SectionID = x.SectionID,
            //             SectionName = x.SectionName

            //         }).ToList();

            var filteredSections = mymodel.view_PLPS
    .Where(x => x.LineID.Equals(lineId))
    .Select(x => new
    {
        SectionID = x.SectionID,
        SectionName = x.SectionName
    })
    .GroupBy(x => new { x.SectionID, x.SectionName })
    .Select(g => g.First())
    .ToList();

            return Json(filteredSections);
        }



        // Function Services Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult EmployeeAdjustLineSectionEdit(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var empAdjust = db.View_EmployeeAdjustLine.Where(x => x.TransactionNo.Equals(id) && x.PlantID.Equals(PlantID)).SingleOrDefault();
            return Json(empAdjust);
        }


        [HttpPost]
        public ActionResult EmployeeAdjustLineSectionUpdate(View_EmployeeAdjustLine obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var EmplAdjust = db.TbEmployeeTransaction.Where(x => x.TransactionNo.Equals(Convert.ToInt32(obj.TransactionNo)) && x.Plant.Equals(PlantID)).SingleOrDefault();

            if (obj.LineID != null)
            {
                EmplAdjust.Line = obj.LineID;

            }
            if (obj.SectionID != null)
            {
                EmplAdjust.Section = obj.SectionID;
            }

            if (obj.StartTime != null)
            {
                EmplAdjust.ClockIn = obj.StartTime;
            }


            if (obj.EndTime != null)
            {
                EmplAdjust.ClockOut = obj.EndTime;
            }


            EmplAdjust.UpdateBy = EmpID;
            obj.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("EmployeeAdjustLine");

        }



        public ActionResult EmployeeAdjustLineClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.TransactionDate == DateTime.Now),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_Employee = db.View_Employee.ToList(),
                //view_EmployeeAdjustLine = db.View_EmployeeAdjustLine.Where(x => x.PlantID.Equals(PlantID)).ToList().OrderBy(x => x.EmployeeID),
                view_EmployeeAdjustLine = db.View_EmployeeAdjustLine.Where(x => x.PlantID.Equals(PlantID))
                .OrderBy(x => x.ClockIn != "" && x.ClockOut == "" ? 1 :
                  x.TransactionDate == new DateTime(1, 1, 1) ? 2 :
                  x.ClockIn != "" && x.ClockOut != "" ? 3 : 4)
                .ThenByDescending(x => x.TransactionDate)
    .ThenBy(x => x.SectionID).ThenBy(x => x.SectionID)
    .ThenBy(x => x.EmployeeID).ToList(),

            };
            ViewBag.VBRoleEmployeeAdjustLine = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(31)).Select(x => x.RoleAction).FirstOrDefault();
            mymodel.view_EmployeeAdjustLine = mymodel.view_EmployeeAdjustLine.Where(p => p.TransactionDate.Equals(DateTime.Today) || p.TransactionDate.Equals(DateTime.MinValue)).ToList();

            return RedirectToAction("EmployeeAdjustLine");

        }




        public ActionResult EmployeeBreakAdjust(View_EmployeeClocktime obj, string[] EmployeeIDchk)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeAdjustBreak = db.View_EmployeeAdjustBreak.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                // tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.TransactionDate == DateTime.Today),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList(),
            };
            ViewBag.VBRoleEmployeeBreakAdjust = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(31)).Select(x => x.RoleAction).FirstOrDefault();
            if (EmployeeIDchk.Length == 0)
            {

                if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName) || obj.TransactionDate != DateTime.MinValue || !string.IsNullOrEmpty(obj.SectionName))
                {

                    if (!string.IsNullOrEmpty(obj.EmployeeID))
                    {
                        ViewBag.SelectedEmpID = obj.EmployeeID;
                        Employee.view_EmployeeAdjustBreak = Employee.view_EmployeeAdjustBreak.Where(p => p.EmployeeID == obj.EmployeeID).ToList();
                    }
                    if (!string.IsNullOrEmpty(obj.SectionName))
                    {
                        ViewBag.SelectedSectionName = obj.SectionName;
                        Employee.view_EmployeeAdjustBreak = Employee.view_EmployeeAdjustBreak.Where(p => p.SectionID == obj.SectionName).ToList();
                    }
                    if (!string.IsNullOrEmpty(obj.LineName))
                    {
                        ViewBag.SelectedLineName = obj.LineName;
                        Employee.view_EmployeeAdjustBreak = Employee.view_EmployeeAdjustBreak.Where(p => p.LineName == obj.LineName).ToList();
                    }
                    if (obj.TransactionDate != DateTime.MinValue)
                    {
                        if (obj.TransactionDate != DateTime.Today)
                        {

                            ViewBag.SelectedTransactionDate = obj.TransactionDate.ToString("yyyy-MM-dd");
                            Employee.view_EmployeeAdjustBreak = Employee.view_EmployeeAdjustBreak.Where(p => p.TransactionDate == obj.TransactionDate).ToList();

                        }
                        else if (obj.TransactionDate == DateTime.Today)
                        {
                            ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
                            Employee.view_EmployeeAdjustBreak = Employee.view_EmployeeAdjustBreak.Where(p => p.TransactionDate.Equals(DateTime.Today)).ToList();

                        }
                    }

                    return View(Employee);

                }
                else
                {
                    Employee.view_EmployeeAdjustBreak = Employee.view_EmployeeAdjustBreak.Where(p => p.TransactionDate.Equals(DateTime.Today)).ToList();


                    return View(Employee);
                }

            }
            else
            {
                // Create Function
                int datacnt = EmployeeIDchk.Count();
                for (int i = 0; i < datacnt; ++i)
                {

                    //   var Empdb = new TbEmployeeTransaction();
                    string empid = EmployeeIDchk[i];
                    int IDtran = Convert.ToInt32(empid);
                    string EmpType = Employee.view_EmployeeAdjustBreak.Where(p => p.TransactionNo.Equals(IDtran) && p.TransactionDate.Equals(obj.TransactionDate)).Select(x => x.Type).SingleOrDefault();
                    if (EmpType == "E")
                    {
                        var EmpDbtran = db.TbEmployeeTransaction.Where(p => p.Plant.Equals(PlantID) && p.TransactionDate.Equals(obj.TransactionDate) && p.TransactionNo.Equals(IDtran) && p.ClockIn != null && p.ClockOut != null).SingleOrDefault();
                        if (EmpDbtran != null)
                        {
                            //Update Transaction
                            //Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.TransactionDate == obj.TransactionDate).SingleOrDefault();
                            EmpDbtran.BreakFlag = "Y";
                            EmpDbtran.UpdateDate = DateTime.Now;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                       // var ServDbtrantest = db.TbServicesTransaction.Where(p => p.Plant.Equals(PlantID) && p.TransactionNo.Equals(IDtran) && p.TransactionDate.Equals(obj.TransactionDate) && p.ClockIn != null && p.ClockOut != null).ToList();

                        var ServDbtran = db.TbServicesTransaction.Where(p => p.Plant.Equals(PlantID) && p.TransactionNo.Equals(IDtran) && p.TransactionDate.Equals(obj.TransactionDate) && p.ClockIn != null && p.ClockOut != null).SingleOrDefault();
                        if (ServDbtran != null)
                        {
                            //Update Transaction
                            //Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.TransactionDate == obj.TransactionDate).SingleOrDefault();
                            ServDbtran.BreakFlag = "Y";
                            ServDbtran.UpdateDate = DateTime.Now;
                            db.SaveChanges();
                        }

                    }
                   // var EmpDb = db.TbEmployeeTransaction.Where(p => p.TransactionNo.Equals(IDtran) && p.ClockIn != null && p.ClockOut != null).SingleOrDefault();


                }
                return RedirectToAction("EmployeeBreakAdjust");

            }

        }

        public ActionResult EmployeeBreakAdjustClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeAdjustBreak = db.View_EmployeeAdjustBreak.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.TransactionDate == DateTime.Now),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList(),

            };
            ViewBag.VBRoleEmployeeBreakAdjust = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(31)).Select(x => x.RoleAction).FirstOrDefault();

            Employee.view_EmployeeAdjustBreak = Employee.view_EmployeeAdjustBreak.Where(p => p.TransactionDate.Equals(DateTime.Today)).ToList();

            return RedirectToAction("EmployeeBreakAdjust");

        }


        [HttpGet]
        public JsonResult EmployeeBreakAdjustRevert(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {

                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeAdjustBreak = db.View_EmployeeAdjustBreak.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.TransactionDate == DateTime.Now),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList()
            };

            //var thisday = DateTime.Now.Date;
           
            var EmpDbfillter = mymodel.view_EmployeeAdjustBreak.Where(p => p.TransactionNo.Equals(id)).SingleOrDefault();

            DateTime datestart = EmpDbfillter.TransactionDate.Value.Date;
            DateTime dateend = EmpDbfillter.TransactionDate.Value.AddDays(1);
            //&& p.ClockIn.Equals(EmpDbfillter.ClockIn) && p.ClockOut.Equals(EmpDbfillter.ClockOut)
            if(EmpDbfillter.Type == "E")
            {
                var EmpDb = db.TbEmployeeTransaction.Where(p => p.TransactionNo.Equals(id)).SingleOrDefault();
                if (EmpDb != null)
                {
                    EmpDb.BreakFlag = "";
                    EmpDb.UpdateBy = EmpID;
                    EmpDb.UpdateDate = DateTime.Now;
                    db.SaveChanges();

                }
            }
            else
            {
                var SerDb = db.TbServicesTransaction.Where(p => p.TransactionNo.Equals(id)).SingleOrDefault();
                if (SerDb != null)
                {
                    SerDb.BreakFlag = "";
                    SerDb.UpdateBy = EmpID;
                    SerDb.UpdateDate = DateTime.Now;
                    db.SaveChanges();

                }
            }
           
            return Json(new { success = true });

        }











        //////////////////////////////////////////// End Employee Controller ///////////////////////////////////

    }



}

