using Microsoft.AspNetCore.Mvc;
using Plims.Models;
using Plims.ViewModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using Plims.Data;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.VisualStudio.Web.CodeGeneration;
using System.Globalization;
using Newtonsoft.Json;
using static System.Collections.Specialized.BitVector32;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Razor;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Plims.Controllers
{
    public class EmployeeController : Controller
    {
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


        [HttpGet]
        public ActionResult EmployeeClockIn(View_EmployeeClocktime obj, string[] EmployeeIDchk , string TransactionDate)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            var TransactionDateVar = DateTime.Today;

            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
              //  tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.TransactionDate == DateTime.Now && x.Plant.Equals(PlantID)),
                view_PermissionMaster = db.View_PermissionMaster.ToList()
                //view_Employee = db.View_Employee.Where(x => x.PlantID.Equals(PlantID)).ToList()

            };



            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
          

            ViewBag.VBRoleEmpClockIn = db.View_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();
            if (EmployeeIDchk.Length == 0)
            {
               
                if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.SectionName))
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

                    return View(mymodel);

                }
                else
                {
                 
                    return View(mymodel);
                }

            }
            else
            {
                if (obj.ClockIn == null || obj.ClockIn == "")
                {
                    return RedirectToAction("EmployeeClockIn", "Employee");
                }

                // Create Function

                int datacnt = EmployeeIDchk.Count();
                for (int i = 0; i < datacnt; ++i)
                {

                    string empid = EmployeeIDchk[i];
                    DateTime clockoutvar;
                    DateTime clockinvar;
                    var empdbcheck = db.TbServicesTransaction.Where(x => x.TransactionDate.Equals(TransactionDateVar) && x.EmployeeID.Equals(empid) && x.ClockOut == "").ToList();
                    if (empdbcheck.Count() != 0)
                    {
                        TempData["AlertMessage"] = "Please Services Clock out Employee ID :" + empid;
                        return RedirectToAction("EmployeeClockIn");
                    }


                    var EmpTrans = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid) && x.Plant.Equals(PlantID) && x.TransactionDate == TransactionDateVar && x.Remark == "" && x.WorkingStatus == "Working").ToList();
                    if (EmpTrans.Count() != 0)
                    {

                        var Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.Plant.Equals(PlantID) && x.TransactionDate == TransactionDateVar).SingleOrDefault();
                        Empdb.ClockIn = obj.ClockIn.ToString();
                        Empdb.UpdateBy = EmpID;//User.Identity.Name;
                        Empdb.UpdateDate = DateTime.Now;
                        db.SaveChanges();

                    }
                    else
                    {

                        var empdetails = db.TbEmployeeMaster.Where(x => x.EmployeeID == empid.Trim() && x.PlantID.Equals(PlantID)).SingleOrDefault();

                        if (!string.IsNullOrEmpty(obj.ClockOut))
                        {
                            var startt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.StartTime).SingleOrDefault();
                            var Endt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.EndTime).SingleOrDefault();
                            var Prefixt = db.TbShift.Where(x => x.ShiftID.Equals(empdetails.ShiftID) && x.PlantID.Equals(PlantID)).Select(x => x.Prefix).SingleOrDefault();

                            //Case with clock out
                            db.TbEmployeeTransaction.Add(new TbEmployeeTransaction()
                            {
                                TransactionDate = Convert.ToDateTime(TransactionDateVar),
                                EmployeeID = empid,
                                Shift = empdetails.ShiftID,
                                StartTime = startt,
                                EndTime = Endt,
                                Plant = PlantID,
                                Line = empdetails.LineID,//obj.LineName,
                                Section = empdetails.SectionID,
                                WorkingStatus = "Working",
                                Prefix = Prefixt,
                                BreakFlag = "",
                                Remark = "",
                                ClockIn = obj.ClockIn,
                                CreateDate = DateTime.Now,
                                CreateBy = EmpID,//User.Identity.Name,
                                UpdateDate = DateTime.Now,
                                UpdateBy = EmpID//User.Identity.Name,
                            });


                        }
                        else
                        {
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
                                ClockIn = obj.ClockIn,
                                ClockOut = "",
                                WorkingStatus = "Working",
                                BreakFlag = "",
                                Remark = "",
                                CreateDate = DateTime.Now,
                                CreateBy = EmpID,//User.Identity.Name,
                                UpdateDate = DateTime.Now,
                                UpdateBy = EmpID,//User.Identity.Name,
                            });



                        }
                        db.SaveChanges();

                    }
                }
                return RedirectToAction("EmployeeClockIn");
               // return View(mymodel);

            }

        }

        
  [HttpGet]
        public ActionResult EmployeeClockInsave( string[] EmployeeIDchk ,string ClockIn, DateTime TransactionDate)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            var TransactionDateVar = TransactionDate;// DateTime.Today;

            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.TransactionDate == DateTime.Now && x.Plant.Equals(PlantID)),
                view_PermissionMaster = db.View_PermissionMaster.ToList()
                //view_Employee = db.View_Employee.Where(x => x.PlantID.Equals(PlantID)).ToList()

            };



            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

           
            ViewBag.VBRoleEmpClockIn = db.View_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();
            if (EmployeeIDchk.Length == 0)
            {
                    return View(mymodel);
               
            }
            else
            {

                if (ClockIn == null || ClockIn == "")
                {
                    return RedirectToAction("EmployeeClockIn", "Employee");
                }

                // Create Function

                int datacnt = EmployeeIDchk.Count();
                for (int i = 0; i < datacnt; ++i)
                {

                    string empid = EmployeeIDchk[i];
                    DateTime clockoutvar;
                    DateTime clockinvar;
                    var empdbcheck = db.TbServicesTransaction.Where(x => x.TransactionDate.Equals(TransactionDateVar) && x.EmployeeID.Equals(empid) && x.ClockOut == "").ToList();
                    if (empdbcheck.Count() != 0)
                    {
                        TempData["AlertMessage"] = "Please Services Clock out Employee ID :" + empid;
                        return RedirectToAction("EmployeeClockIn");
                    }


                    var EmpTrans = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid) && x.Plant.Equals(PlantID) && x.TransactionDate == TransactionDateVar && x.Remark == "" && x.WorkingStatus == "Working").ToList();
                    if (EmpTrans.Count() != 0)
                    {

                        var Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.Plant.Equals(PlantID) && x.TransactionDate == TransactionDateVar).SingleOrDefault();
                        Empdb.ClockIn = ClockIn;
                        Empdb.UpdateBy = EmpID;//User.Identity.Name;
                        Empdb.UpdateDate = DateTime.Now;
                        db.SaveChanges();

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
                return RedirectToAction("EmployeeClockIn");
               // return View("EmployeeClockIn",mymodel);

            }

        }


        //Function Employee Clock in Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function

        [HttpGet]
        public JsonResult EmployeeClockInEdit(int ID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var todayDate = DateTime.Today.ToString("yyyy-MM-dd");  // Use the format "yyyy-MM-dd" to match the expected format
            var EmplID = db.View_EmployeeClocktime.Where(x => x.ID.Equals(ID)).Select(x => x.EmployeeID).SingleOrDefault();

            DateTime parsedTodayDate = DateTime.ParseExact(todayDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime parsedTodayDateBefore = DateTime.ParseExact(todayDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(-1);

            //var Emps = db.TbEmployeeTransaction
            //    .Where(x => x.EmployeeID.Equals(EmplID) && x.TransactionDate.Date.Equals(parsedTodayDate.Date) && (x.Remark == null || x.Remark == "") && x.WorkingStatus.Equals("Working"))
            //    .SingleOrDefault();

            var Emps = db.TbEmployeeTransaction
               .Where(x => x.EmployeeID.Equals(EmplID) && ( (x.TransactionDate.Date.Equals(parsedTodayDate.Date) && (x.Remark == null || x.Remark == "") && x.WorkingStatus.Equals("Working")) || (x.TransactionDate.Date.Equals(parsedTodayDateBefore.Date) && (x.Remark == null || x.Remark == "") && x.WorkingStatus.Equals("Working"))))
               .SingleOrDefault();


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

            var EmpTran = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(obj.EmployeeID) && x.TransactionDate == DateTime.Today.Date).ToList();
            if (EmpTran.Count() != 0)
            {
                //Update Transaction
                Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == obj.EmployeeID && x.TransactionDate == DateTime.Today.Date).SingleOrDefault();
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
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID) && x.Status.Equals(1)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
             //   tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
               // tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.TransactionDate == DateTime.Now && x.Plant.Equals(PlantID)),
                view_PermissionMaster = db.View_PermissionMaster.ToList()
                //view_Employee = db.View_Employee.Where(x => x.PlantID.Equals(PlantID)).ToList()
            };
            return View("EmployeeClockIn", Employee);

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
         public ActionResult EmployeeClockOut(View_EmployeeClocktime obj, string[] EmployeeIDchk , DateTime TransactionDate)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            //var TransactionDateVar = obj.TransactionDate;
            var TransactionDateVar = TransactionDate; //DateTime.Today;

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
                view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
               tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.Plant.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_Employee = db.View_Employee.ToList()

            };

         



            ViewBag.VBRoleEmpClockOut = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(16)).Select(x => x.RoleAction).FirstOrDefault();
            if (EmployeeIDchk.Length == 0)
            {

                if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.SectionName))
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

                    return View(Employee);

                }
                else
                {


                    return View(Employee);
                }

            }
            else
            {
                if (obj.ClockOut == null || obj.ClockOut == "")
                {
                    return RedirectToAction("EmployeeClockOut", "Employee");
                }

                // Create Function
                int datacnt = EmployeeIDchk.Count();
                for (int i = 0; i < datacnt; ++i)
                {

                    // 1. check TbEmployeeTransaction == Null ?
                    var Empdb = new TbEmployeeTransaction();
                    //var Emp = db.TbEmployeeMasters.Where(x => x.EmployeeID.Equals(EmployeeID[i]));
                    string empid = EmployeeIDchk[i];
                    var EmpClockView = db.TbEmployeeMaster.Where(x => x.ID.Equals(Convert.ToInt32(empid))).Select(x => x.EmployeeID).SingleOrDefault();
                    var EmpTrancheck = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(EmpClockView) && (x.TransactionDate == TransactionDateVar || (x.TransactionDate == TransactionDateVar.AddDays(-1) && x.ClockOut == "")) && (x.Remark == null || x.Remark == "") && x.WorkingStatus == "Working").ToList();
                    if (EmpTrancheck.Count() == 1)
                    {
                        var EmpTran = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(EmpClockView) && (x.TransactionDate == TransactionDateVar || (x.TransactionDate == TransactionDateVar.AddDays(-1) && x.ClockOut == "")) && (x.Remark == null || x.Remark == "") && x.WorkingStatus == "Working").SingleOrDefault();


                        if (EmpTran != null)
                        {
                            //Update Transaction
                            // Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(EmpClockView) && x.TransactionDate == TransactionDateVar && (x.Remark == null || x.Remark == "") && x.WorkingStatus.Equals("Working")).SingleOrDefault();
                            EmpTran.ClockOut = obj.ClockOut.ToString();
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
                        TempData["AlertMessage"] = "Please Contact IT : Check data in database !";
                        return RedirectToAction("EmployeeClockOut");

                    }
                }
                

            }return RedirectToAction("EmployeeClockOut");

        }


        //3.  Function Employee Clock in Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult EmployeeClockOutEdit(int ID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var todayDate = DateTime.Today.ToString("yyyy-MM-dd");  // Use the format "yyyy-MM-dd" to match the expected format
            var EmplID = db.View_EmployeeClocktime.Where(x => x.ID.Equals(ID)).Select(x => x.EmployeeID).SingleOrDefault();

            DateTime parsedTodayDate = DateTime.ParseExact(todayDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var Emps = db.TbEmployeeTransaction
                .Where(x => x.TransactionNo.Equals(ID) && x.TransactionDate.Date.Equals(parsedTodayDate.Date))
                .SingleOrDefault();


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

            var EmpTran = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(obj.EmployeeID) && x.TransactionDate == DateTime.Today.Date).ToList();
            if (EmpTran.Count() != 0)
            {
                //Update Transaction
                Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == obj.EmployeeID && x.TransactionDate == DateTime.Today.Date).SingleOrDefault();
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
                view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.TransactionDate == DateTime.Now && x.Plant.Equals(PlantID)),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_Employee = db.View_Employee.ToList()

            };
            return RedirectToAction("EmployeeClockOut");

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
        public ActionResult ServicesClockIn(View_ServicesClocktime obj, string[] EmployeeIDchk , string TableData, string LineID, string SectionSelect,DateTime TransactionDate)
        {

            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            //var TransactionDateVar = obj.TransactionDate;
            var TransactionDateVar = TransactionDate;// DateTime.Today;

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Employee = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x=>x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_ServicesClocktime = db.View_ServicesClocktime.Where(x=>x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbServicesTransaction = db.TbServicesTransaction.Where(x => x.TransactionDate == DateTime.Now && x.Plant.Equals(PlantID)),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_Employee = db.View_Employee.ToList()
            };
            ViewBag.VBRoleServicesClockIn = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(18)).Select(x => x.RoleAction).FirstOrDefault();

                if (EmployeeIDchk.Length == 0)
            {

                if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.SectionName))
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
                    return View(Employee);
                }
                else
                {

                        return View(Employee);
                }
            }
            else
            {
                // Create Function
                int datacnt = EmployeeIDchk.Count();
               // decimal rateservicecheck = 0.0;

                List<TableDataRow> tableRows = JsonConvert.DeserializeObject<List<TableDataRow>>(TableData);
                var distinctServices = new HashSet<decimal>();
                int j = 0;
                //check service rate before Add
                foreach (var itmservice in tableRows)
                {
                    var servicesplit = itmservice.Service.Split(":");
                    var servicerate = db.TbService.Where(x => x.PlantID.Equals(PlantID) && x.LineID.Equals(LineID) && x.ServicesID.Equals(servicesplit[0])).Select(x => x.ServicesRate).SingleOrDefault();
                    decimal rate = Convert.ToDecimal(servicerate);
                    if (servicesplit.Length > 0)
                    {
                        // Add the first part of the service split to the HashSet
                        distinctServices.Add(rate);
                    }

                    j++;
                }

                if(distinctServices.Count > 1 )
                {
                    TempData["AlertMessage"] = "Rate Differnence !";
                    return View(Employee);
                }


                    for (int i = 0; i < datacnt; ++i)
                {
                 
                    foreach (var itmservice in tableRows)
                    {
                        var Empdb = new TbServicesTransaction();
                        string empid = EmployeeIDchk[i];

                        var sectionsplit = itmservice.Section.Split(":");
                        var servicesplit = itmservice.Service.Split(":");
                        var remark = "";
                        if (SectionSelect == "ALL")
                        {
                            remark = "ALL";
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
                                    TempData["AlertMessage"] = "Please Employee Clock out Employee ID :" + empidvar;
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
                                db.TbServicesTransaction.Add(new TbServicesTransaction()
                            {
                                TransactionDate = Convert.ToDateTime(TransactionDateVar),
                                EmployeeID = empid,
                                Plant = PlantID,
                                Shift = empdetails.ShiftID,
                                StartTime = startt,
                                EndTime = Endt,
                                 Line = obj.LineID,//obj.LineName,
                                SectionID = sectionsplit[0].Trim(),
                                SectionName = sectionsplit[1].Trim(),
                                    ServicesID = servicesplit[0].Trim(),
                                ServicesName = servicesplit[1].Trim(),  
                                WorkingStatus = "Working",
                                ClockIn = obj.ClockIn,
                                ClockOut = "",
                                Remark = remark,
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
                              //  TransactionNo = db.TbServicesTransaction.Count() + 1,
                                TransactionDate = Convert.ToDateTime(TransactionDateVar),
                                EmployeeID = empid,
                                Shift = empdetails.ShiftID,
                                    StartTime = startt,
                                    EndTime = Endt,
                                    Line = obj.LineID,//obj.LineName,
                                ClockIn = obj.ClockIn,
                                    CreateDate = DateTime.Now,
                                CreateBy = EmpID,//User.Identity.Name,
                                UpdateDate = DateTime.Now,
                                UpdateBy = EmpID,//User.Identity.Name,
                            });



                        }
                        db.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("ServicesClockIn");

            }         

        }


        //2.  Function service Clock in Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult SericesClockInEdit(int ID,string ServicesID , string SectionID)
        {


            var todayDate = DateTime.Today.ToString("yyyy-MM-dd");  // Use the format "yyyy-MM-dd" to match the expected format
            var EmplID = db.View_ServicesClocktime.Where(x => x.ID == ID && x.SectionID.Equals(SectionID) && x.ServicesID.Equals(ServicesID)).Select(x => x.EmployeeID).SingleOrDefault();

            DateTime parsedTodayDate = DateTime.ParseExact(todayDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var Emps = db.TbServicesTransaction
                .Where(x => x.EmployeeID.Equals(EmplID) && x.SectionID.Equals(SectionID) && x.ServicesID.Equals(ServicesID) && x.TransactionDate.Date.Equals(parsedTodayDate.Date))
                .SingleOrDefault();


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

            var EmpTran = db.TbServicesTransaction.Where(x => x.EmployeeID.Equals(obj.EmployeeID) && x.TransactionDate == DateTime.Today.Date).ToList();
            if (EmpTran.Count() != 0)
            {
                //Update TbServicesTransaction
                Empdb = db.TbServicesTransaction.Where(x => x.TransactionNo.Equals(obj.TransactionNo)).SingleOrDefault();
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
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_ServicesClocktime = db.View_ServicesClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbServicesTransaction = db.TbServicesTransaction.Where(x => x.TransactionDate == DateTime.Now && x.Plant.Equals(PlantID)),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_Employee = db.View_Employee.ToList()

            };
            return RedirectToAction("ServicesClockIn");

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Services Clock out Function
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ServicesClockOut(View_ServicesClocktime obj, string[] EmployeeIDchk, string[] ID, string[]  EmployeeIDlist, DateTime TransactionDate)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            //var TransactionDateVar = obj.TransactionDate;
            var TransactionDateVar = TransactionDate; // DateTime.Today;

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
                view_ServicesClocktime = db.View_ServicesClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbServicesTransaction = db.TbServicesTransaction.Where(x => x.TransactionDate == DateTime.Now && x.Plant.Equals(PlantID)),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_Employee = db.View_Employee.ToList()
            };
            ViewBag.VBRoleServicesClockOut = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(19)).Select(x => x.RoleAction).FirstOrDefault();
            if (EmployeeIDchk.Length == 0)
            {

                if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.SectionName))
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
                        var Empdb = new TbServicesTransaction();
                        string empid = EmployeeIDchk[i];
                    if(empid != "on")
                    {
           
     
                   // var EmpClockView = db.TbEmployeeMaster.Where(x => x.ID.Equals(Convert.ToInt32(empid))).Select(x => x.EmployeeID).SingleOrDefault();
                    var ViewEmpTran = db.View_ServicesClocktime.Where(x => x.TransactionNo.Equals(Convert.ToInt32(empid)) && (x.TransactionDate == TransactionDateVar || (x.TransactionDate == TransactionDateVar.AddDays(-1) && x.ClockOut == ""))).First();

                    var EmpTran = db.TbServicesTransaction.Where(x => x.TransactionNo.Equals(Convert.ToInt32(empid)) && x.SectionID.Equals(ViewEmpTran.SectionID)  && (x.TransactionDate == TransactionDateVar || (x.TransactionDate == TransactionDateVar.AddDays(-1) && x.ClockOut == "")) ).SingleOrDefault();
                        // check clockout beofre endtime?
                        var EmpMaster = db.View_EmployeeMaster.Where(x => x.EmployeeID.Equals(EmpTran.EmployeeID) && x.PlantID.Equals(PlantID)).SingleOrDefault();

                        var startTime = Convert.ToDateTime(EmpMaster.StartTime);
                        var endTime = Convert.ToDateTime(EmpMaster.EndTime);
                        var clockinvar = Convert.ToDateTime(EmpTran.ClockIn);
                        var clockoutvar = Convert.ToDateTime(obj.ClockOut);


                        // Calculate time span
                        TimeSpan timeSpan = endTime - startTime;
                        TimeSpan timeclockspan = clockoutvar - clockinvar;
                        // Now you have the time span, you can use it as needed


                        // var durationInHours = timeSpan.TotalHours;
                        var durationInMinutes = timeSpan.TotalMinutes;
                        // var durationInHours = timeSpan.TotalHours;
                        var durationInMinutesclock = timeclockspan.TotalMinutes;
                        var workingvar = "";
                        if(durationInMinutesclock < durationInMinutes)
                        {
                            workingvar = obj.WorkingStatus;
                        }
                        else
                        {
                            workingvar = "Working";
                        }
                        //  var EmpTran = db.TbServicesTransaction.Where(x => x.TransactionNo ==Convert.ToInt32(tranNo)).SingleOrDefault();
                        // check TbServicesTransaction == Null ?

                        if (EmpTran != null && !string.IsNullOrEmpty(EmpTran.ClockIn))
                        {
                        //Update Transaction
                        EmpTran.ClockOut = obj.ClockOut.ToString();
                            EmpTran.WorkingStatus = workingvar;
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
        public JsonResult ServicesClockOutEdit(int ID, string ServicesID, string SectionID)
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

            var Emps = db.TbServicesTransaction
                .Where(x => x.TransactionNo.Equals(ID))
                .SingleOrDefault();




            return Json(Emps);
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
                view_ServicesClocktime = db.View_ServicesClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbServicesTransaction = db.TbServicesTransaction.Where(x => x.TransactionDate == DateTime.Now && x.Plant.Equals(PlantID)),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_Employee = db.View_Employee.ToList()

            };
            return RedirectToAction("ServicesClockOut");

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        /// <summary>
        /// EmployeeLeaveHoliday Function
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EmployeeLeaveHoliday(View_EmployeeLeaveHolidayClocktime obj, string[] EmployeeIDchk , string WorkingstatusCreate , string remarkcreate,DateTime TreansactionDateCreate,string StartTime, string EndTime)
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
                tbShift = db.TbShift.Where(x=>x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeLeaveHoliday = db.TbEmployeeLeaveHoliday.Where(x=>x.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_EmployeeLeaveHolidayClocktime =  db.View_EmployeeLeaveHolidayClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
            };
            ViewBag.VBRoleEmployeeLeaveHoliday = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(21)).Select(x => x.RoleAction).FirstOrDefault();

            if (EmployeeIDchk.Length == 0)
            {
                        if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName) || obj.TransactionDate.HasValue  != false || !string.IsNullOrEmpty(obj.WorkingStatus))
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
                                mymodel.view_EmployeeLeaveHolidayClocktime = mymodel.view_EmployeeLeaveHolidayClocktime.Where(p => p.TransactionDate == obj.TransactionDate).ToList();
                        ViewBag.SelectedTransactionDate = obj.TransactionDate;
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

                    var EmpTran = db.TbEmployeeLeaveHoliday.Where(x => x.EmployeeID.Equals(empid) && x.TransactionDate == TreansactionDateCreate && (x.WorkingStatus == "Leave" || x.WorkingStatus == "Holiday")).ToList();
                    if (obj.ClockIn == null && obj.ClockOut == null)
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
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
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

            var EmpTran = db.TbEmployeeLeaveHoliday.Where(x => x.EmployeeID.Equals(obj.EmployeeID) && x.TransactionDate == DateTime.Today.Date).ToList();
            if (EmpTran.Count() != 0)
            {
                //Update Transaction
                Empdb = db.TbEmployeeLeaveHoliday.Where(x => x.EmployeeID == obj.EmployeeID && x.TransactionDate == DateTime.Today.Date).SingleOrDefault();
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
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>











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

            var EmpTran = db.TbServicesTransaction.Where(x => x.EmployeeID.Equals(obj.EmployeeID) && x.TransactionDate == DateTime.Today.Date).ToList();
            if (EmpTran.Count() != 0)
            {
                //Update TbServicesTransaction
                Empdb = db.TbServicesTransaction.Where(x => x.TransactionNo.Equals(obj.TransactionNo)).SingleOrDefault();
                Empdb.ClockOut = obj.ClockOut;
                Empdb.UpdateBy = EmpID;//User.Identity.Name,
                Empdb.UpdateDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("ServicesClockOut");
            }
            else
            {
                TempData["AlertMessage"] = "Please Clock Out !";


            }

            return RedirectToAction("ServicesClockOut");

        }






        /// <summary>
        /// Employee Adjust Line
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="EmployeeIDchk"></param>
        /// <returns></returns>

        [HttpGet]
        public ActionResult EmployeeAdjustLine(View_EmployeeClocktime obj, string[] EmployeeIDchk,string StartTime , string EndTime , string ToLine, string ToSection, string FromLine,DateTime TransactionDate)
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
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
               // view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_Employee = db.View_Employee,
                view_EmployeeAdjustLine = db.View_EmployeeAdjustLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                 view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),


            };
            ViewBag.VBRoleEmployeeBreakAdjust = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(31)).Select(x => x.RoleAction).FirstOrDefault();
            if (EmployeeIDchk.Length == 0)
            {

                if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName) || obj.TransactionDate != null)
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
                    if (!string.IsNullOrEmpty(obj.SectionID))
                    {
                        ViewBag.SelectedSectionID = obj.SectionID;
                        Employee.view_EmployeeAdjustLine = Employee.view_EmployeeAdjustLine.Where(p => p.SectionID == obj.SectionID).ToList();
                    }
                    if (obj.TransactionDate != null)
                    {
                        ViewBag.SelectedTransactionDate = obj.TransactionDate;
                        Employee.view_EmployeeAdjustLine = Employee.view_EmployeeAdjustLine.Where(p => p.TransactionDate == obj.TransactionDate).ToList();
                    }

                    return View(Employee);

                }
                else
                {


                    return View(Employee);
                }

            }
            else
            {
                if(StartTime == "" || StartTime  == null || EndTime == "" || EndTime == null)
                {
                    TempData["AlertMessage"] = "Please Fill StartTime or EndTime !";
                    return RedirectToAction("EmployeeAdjustLine");
                }
                // Create Function
                var thisday = DateTime.Now.Date;
                int datacnt = EmployeeIDchk.Count();
                for (int i = 0; i < datacnt; ++i)
                {
                    var Empdb = new TbEmployeeTransaction();
                    string empid = EmployeeIDchk[i];





                    //check current line not clockin
                    var Empcheckclockout = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid) && x.TransactionDate == thisday && x.Plant.Equals(PlantID) &&  x.ClockOut == "").ToList();
                    var EmpTran = db.TbEmployeeTransaction.Where(x => x.EmployeeID.Equals(empid) && x.TransactionDate == thisday && x.Plant.Equals(PlantID)  && x.Line.Equals(ToLine) && x.Section.Equals(ToSection)  && x.Remark == "Adjust").ToList();



                    if (Empcheckclockout.Count != 0)
                    {
                        TempData["AlertMessage"] = "Please Employee Clock out Employee ID :" + empid;
                        return RedirectToAction("EmployeeAdjustLine");
                    }
                    // If has data update
                    if (EmpTran.Count() != 0 )
                    {
                        
                        //Update Transaction
                        Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.TransactionDate == thisday && x.Plant.Equals(PlantID)).SingleOrDefault();
                        Empdb.ClockIn = StartTime;
                        Empdb.ClockOut = EndTime;
                        Empdb.Line = ToLine;
                        Empdb.Section = ToSection;
                        Empdb.UpdateBy = EmpID;//User.Identity.Name;
                        Empdb.UpdateDate = DateTime.Now;
                        db.SaveChanges();
                     
                    }
                    else
                    {

                        var empdetails = db.TbEmployeeMaster.Where(x => x.EmployeeID == empid.Trim() && x.PlantID.Equals(PlantID)).SingleOrDefault();
                        // Create Transaction
                        var empshift = db.View_EmployeeMaster.Where(x => x.EmployeeID == empid.Trim() && x.PlantID.Equals(PlantID)).SingleOrDefault();


                        // Check ช่วงเวลาในการ insert ว่าสามารถทำได้ไหม

                        //var clockinvar = Convert.ToDateTime(StartTime);
                        //var clockoutvar = Convert.ToDateTime(EndTime);
                        //// Check Rangetime for clock-in again.
                        //var startTime = Convert.ToDateTime(empshift.StartTime);
                        //var endTime = Convert.ToDateTime(empshift.EndTime);
                        //TimeSpan timeSpan = endTime - startTime;
                        //TimeSpan timeclockspan = clockoutvar - clockinvar;
                        //var durationInMinutes = timeSpan.TotalMinutes;
                        //var durationInMinutesclock = timeclockspan.TotalMinutes;

                        //if (durationInMinutesclock < durationInMinutes)
                        //{
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
                                Remark = "Adjust",
                                BreakFlag = "",
                                StartTime = empshift.StartTime,
                                EndTime = empshift.EndTime,
                                ClockIn = StartTime,
                                ClockOut = EndTime,
                                CreateDate = DateTime.Now,
                                CreateBy = EmpID,//User.Identity.Name,
                                UpdateDate = DateTime.Now,
                                UpdateBy = EmpID//User.Identity.Name,
                            }); ;

                       // }

                    }
                        db.SaveChanges();

                    }
                }
                return RedirectToAction("EmployeeAdjustLine");

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
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_Employee = db.View_Employee.ToList(),
                view_EmployeeAdjustLine = db.View_EmployeeAdjustLine.Where(x => x.PlantID.Equals(PlantID)).ToList()

            };
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
                tbEmployeeMaster = db.TbEmployeeMaster.ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeAdjustBreak = db.View_EmployeeAdjustBreak.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.TransactionDate == DateTime.Now),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList(),
            };
            ViewBag.VBRoleEmployeeBreakAdjust = Employee.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(31)).Select(x => x.RoleAction).FirstOrDefault();
            if (EmployeeIDchk.Length == 0)
            {

                if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName))
                {

                    if (!string.IsNullOrEmpty(obj.EmployeeID))
                    {
                        ViewBag.SelectedEmpID = obj.EmployeeID;
                        Employee.view_EmployeeAdjustBreak = Employee.view_EmployeeAdjustBreak.Where(p => p.EmployeeID == obj.EmployeeID).ToList();
                    }
                    if (!string.IsNullOrEmpty(obj.LineName))
                    {
                        ViewBag.SelectedLineName = obj.LineName;
                        Employee.view_EmployeeAdjustBreak = Employee.view_EmployeeAdjustBreak.Where(p => p.LineName == obj.LineName).ToList();
                    }


                    return View(Employee);

                }
                else
                {


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
                    var EmpDb = db.TbEmployeeTransaction.Where(p => p.TransactionNo.Equals(IDtran) && p.ClockIn != null && p.ClockOut != null).SingleOrDefault();
                    if (EmpDb != null)
                    {
                        //Update Transaction
                        //Empdb = db.TbEmployeeTransaction.Where(x => x.EmployeeID == EmployeeIDchk[i] && x.TransactionDate == obj.TransactionDate).SingleOrDefault();
                        EmpDb.BreakFlag = "Y";
                        EmpDb.UpdateDate = DateTime.Now;
                        db.SaveChanges();
                    }
                    
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
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList(),

            };
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

                tbEmployeeMaster = db.TbEmployeeMaster.ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeAdjustBreak = db.View_EmployeeAdjustBreak.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeTransaction = db.TbEmployeeTransaction.Where(x => x.TransactionDate == DateTime.Now),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList()
            };

            //var thisday = DateTime.Now.Date;

            var EmpDb = db.TbEmployeeTransaction.Where(p => p.TransactionNo.Equals(id)).SingleOrDefault();
            if (EmpDb != null)
            {
                EmpDb.BreakFlag = "";
                EmpDb.UpdateBy = EmpID;
                EmpDb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }
            return Json(new { success = true });

        }



        //////////////////////////////////////////// End Employee Controller ///////////////////////////////////

    }



}

