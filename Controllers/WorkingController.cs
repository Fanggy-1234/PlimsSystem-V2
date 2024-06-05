using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Plims.Data;
using Plims.Models;
using Plims.ViewModel;
using System.Data;
using System.Data.Common;
using System.Reflection.Metadata.Ecma335;
using System.Web.WebPages;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Drawing;
using QRCoder;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO.Pipelines;

namespace Plims.Controllers
{
    public class WorkingController : Controller
    {
        private readonly AppDbContext db;
        public WorkingController(AppDbContext _db)
        {
            db = _db;
        }



        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult WorkingFunction(TbProductionTransaction obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var mymodel = new ViewModelAll
                {
                    view_PermissionMaster = db.View_PermissionMaster.ToList(),
                    tbPlants = db.TbPlant.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                    tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                    tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                    tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                    view_Employee = db.View_Employee.ToList(),
                    tbReason = db.TbReason.Where(x => x.PlantID.Equals(PlantID)).ToList()
                    //  tbProductionTransaction = db.TbProductionTransaction.ToList()
                };
                return View(mymodel);
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Working page had problem! please contact IT";
                return RedirectToAction("Login");
            }
           
        }


        public ActionResult WorkingFunctionCreate(string employeeId,string productId)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                view_Employee = db.View_Employee.ToList(),
                tbReason = db.TbReason.Where(x => x.PlantID.Equals(PlantID)).ToList()
                //  tbProductionTransaction = db.TbProductionTransaction.ToList()
            };

            // check QRcode in system
            if (db.TbEmployeeMaster.Where(x=>x.EmployeeID.Equals(employeeId))== null || db.TbEmployeeGroupQR.Where(x => x.GroupID.Equals(employeeId)) == null)
            {
                TempData["AlertMessage"] = "QR Code is not available in the system!";
                return Json(mymodel);
            }

            if (db.View_PermissionMaster.Where(x => x.UserEmpID.Equals(EmpID) && x.PageID.Equals(22)).Select(x => x.RoleAction).SingleOrDefault() == "Full")
            {
                var currentDateTime = DateTime.Now;
                var currentDate = currentDateTime.Date;
                var currentTime = currentDateTime.TimeOfDay;
                try
                {
                    //Select EmployeeTransaction
                    var objEmp = db.TbEmployeeTransaction
                        .Where(x => x.EmployeeID.Equals(employeeId) &&
                                    x.TransactionDate.Date == currentDate &&
                                    x.Plant.Equals(PlantID))
                        .OrderByDescending(x => x.TransactionNo)
                        .FirstOrDefault();

                    if(objEmp != null)
                    {
                        var objPLPS = db.View_PLPS
                        .Where(x => x.PlantID.Equals(PlantID) &&
                                    x.LineID.Equals(objEmp.Line.ToString()) &&
                                    x.ProductID.Equals(productId) &&
                                    x.SectionID.Equals(objEmp.Section.ToString()))
                        .FirstOrDefault();

                        if (objPLPS != null)
                        {
                            var LastTransactionTime = db.TbProductionTransaction
                           .Where(x => x.QRCode.Equals(employeeId) && x.TransactionDate.Date == currentDate)
                           .OrderByDescending(x => x.TransactionDate)
                           .Select(x => x.TransactionDate.TimeOfDay)
                           .FirstOrDefault();


                            // Convert TimeSpan to total seconds
                            double lastTransactionSeconds = LastTransactionTime.TotalSeconds;
                            double delayTimeSeconds = Convert.ToDouble(objPLPS.Delaytime);

                            if (lastTransactionSeconds + delayTimeSeconds < currentTime.TotalSeconds)
                            {
                                // Perform actions if LastTransactionTime is greater than or equal to objPLPS.Delaytime                  
                                db.TbProductionTransaction.Add(new TbProductionTransaction()
                                {
                                    // TransactionNo = db.TbProductionTransaction.Count() + 1,
                                    TransactionDate = DateTime.Now,
                                    PlantID = Convert.ToInt32(objEmp.Plant),
                                    LineID = objEmp.Line,
                                    SectionID = objEmp.Section,
                                    ProductID = productId,
                                    QRCode = employeeId,
                                    Qty = 1,
                                    QtyPerQR = Convert.ToInt32(objPLPS.QTYPerQRCode),//Get from PLPS
                                    DataType = "Count",
                                    Reason = "",
                                    Note = "",
                                    CreateDate = DateTime.Today,
                                    CreateBy = EmpID,
                                    UpdateDate = DateTime.Today,
                                    UpdateBy = EmpID
                                });
                                db.SaveChanges();
                                string sectionval = objEmp.Section.ToString() + " : " + objPLPS.SectionName.ToString();
                                return Json(sectionval);

                            }
                            else
                            {
                                TempData["AlertMessage"] = "please check time!";
                                var sectionvalalert = "check time";
                                return Json(sectionvalalert);
                            }
                        }
                    }

                }
                catch
                {
                            TempData["AlertMessage"] = "Please check master data!";
                            return Json(mymodel);
                }
            }

           

            return Json(mymodel);
        }


        [HttpPost]
        public ActionResult GetSectionAndUnit(string employeeID, string productID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var currentDateTime = DateTime.Now;
            var currentDate = currentDateTime.Date;


            // Query your database or data source to retrieve section and unit based on employeeID and productID
            // For demonstration purposes, let's assume you have a method to get section and unit
            var objEmp = db.TbEmployeeTransaction
                        .Where(x => x.EmployeeID.Equals(employeeID) &&
                                    x.TransactionDate.Date == currentDate &&
                                    x.Plant.Equals(PlantID))
                        .OrderByDescending(x => x.TransactionNo)
                        .FirstOrDefault();

            var objPLPS = db.View_PLPS
                       .Where(x => x.PlantID.Equals(PlantID) &&
                                   x.LineID.Equals(objEmp.Line.ToString()) &&
                                   x.ProductID.Equals(productID) &&
                                   x.SectionID.Equals(objEmp.Section.ToString()))
                       .FirstOrDefault();



            string section = objEmp.Section;
            string unit = objPLPS.Unit;

            // Return section and unit as JSON
            return Json(new { section = section, unit = unit });
        }



        [HttpPost]
        public ActionResult GetSectionAndUnitDefect(string employeeID, string productID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var currentDateTime = DateTime.Now;
            var currentDate = currentDateTime.Date;


            // Query your database or data source to retrieve section and unit based on employeeID and productID
            // For demonstration purposes, let's assume you have a method to get section and unit
            var objEmp = db.TbEmployeeTransaction
                        .Where(x => x.EmployeeID.Equals(employeeID) &&
                                    x.TransactionDate.Date == currentDate &&
                                    x.Plant.Equals(PlantID))
                        .OrderByDescending(x => x.TransactionNo)
                        .FirstOrDefault();

            var objPLPS = db.View_PLPS
                       .Where(x => x.PlantID.Equals(PlantID) &&
                                   x.LineID.Equals(objEmp.Line.ToString()) &&
                                   x.ProductID.Equals(productID) &&
                                   x.SectionID.Equals(objEmp.Section.ToString()))
                       .FirstOrDefault();



            string section = objEmp.Section;
            string unit = objPLPS.Unit;

            // Return section and unit as JSON
            return Json(new { section = section, unit = unit });
        }



        // Function ProductQTYPiece
        public ActionResult ProductQTYPiece(TbProductionTransaction obj, string employeeID, string productID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var currentDateTime = DateTime.Now;
            var currentDate = currentDateTime.Date;
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                view_Employee = db.View_Employee.ToList(),
                tbProductionTransaction = db.TbProductionTransaction.ToList(),
                tbReason = db.TbReason.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList()
            };

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }



            try
            {
                

                var objEmp = db.TbEmployeeTransaction
                   .Where(x => x.EmployeeID.Equals(employeeID) &&
                               x.TransactionDate.Date == currentDate &&
                               x.Plant.Equals(PlantID))
                   .OrderByDescending(x => x.TransactionNo)
                   .FirstOrDefault();

                var objPLPS = db.View_PLPS
                           .Where(x => x.PlantID.Equals(PlantID) &&
                                       x.LineID.Equals(objEmp.Line.ToString()) &&
                                       x.ProductID.Equals(productID) &&
                                       x.SectionID.Equals(objEmp.Section.ToString()))
                           .FirstOrDefault();




                db.TbProductionTransaction.Add(new TbProductionTransaction()
                {
                    // TransactionNo = db.TbProductionTransaction.Count() + 1,
                    TransactionDate = DateTime.Now,
                    PlantID = PlantID,
                    LineID = objEmp.Line,
                    SectionID = objEmp.Section,
                    ProductID = productID,
                    QRCode = employeeID,
                    Qty = obj.Qty,
                    QtyPerQR = Convert.ToInt32(objPLPS.QTYPerQRCode),//Get from PLPS
                    DataType = "FG",
                    Reason = "",
                    Note = "",
                    PackageRef = 0,
                    EmployeeRef = "",
                    GroupRef = "",
                    CreateDate = DateTime.Today,
                    CreateBy = EmpID,
                    UpdateDate = DateTime.Today,
                    UpdateBy = EmpID
                });
                db.SaveChanges();


                return View("WorkingFunction",mymodel);
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Working page had problem! please contact IT";
                return RedirectToAction("Login");
            }

        }



        
        [HttpPost]
        public ActionResult ProductQtySpeialMinus(TbProductionTransaction obj, string employeeID, string productID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var currentDateTime = DateTime.Now;
            var currentDate = currentDateTime.Date;
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                view_Employee = db.View_Employee.ToList(),
                tbProductionTransaction = db.TbProductionTransaction.ToList(),
                tbReason = db.TbReason.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList()
            };

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {

                var objEmp = db.View_ClockTime
                   .Where(x => x.EmployeeID.Equals(employeeID) &&
                               x.TransactionDate.Date == currentDate &&
                               x.PlantID.Equals(PlantID))
                   .FirstOrDefault();


                if(objEmp == null)
                {
                    TempData["AlertMessage"] = "Please check Clockin";
                    return View("WorkingFunctionWithPackage", mymodel);
                }
                var objPLPS = db.View_PLPS
                           .Where(x => x.PlantID.Equals(PlantID) &&
                                       x.LineID.Equals(objEmp.LineID.ToString()) &&
                                       x.ProductID.Equals(productID) &&
                                       x.SectionID.Equals(objEmp.SectionID.ToString()))
                           .FirstOrDefault();


                db.TbProductionTransaction.Add(new TbProductionTransaction()
                {
                    // TransactionNo = db.TbProductionTransaction.Count() + 1,
                    TransactionDate = DateTime.Now,
                    PlantID = PlantID,
                    LineID = objEmp.LineID,
                    SectionID = objEmp.SectionID,
                    ProductID = productID,
                    Prefix = objEmp.Prefix,
                    QRCode = employeeID,
                    Qty = obj.Qty,
                    QtyPerQR = Convert.ToInt32(objPLPS.QTYPerQRCode),//Get from PLPS
                    DataType = "Minus",
                    Reason = "",
                    Note = "",
                    PackageRef = 0,
                    EmployeeRef = "",
                    GroupRef = "",
                    CreateDate = DateTime.Today,
                    CreateBy = EmpID,
                    UpdateDate = DateTime.Today,
                    UpdateBy = EmpID
                });
                db.SaveChanges();


                return View("WorkingFunctionWithPackage", mymodel);
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Working page had problem! please contact IT";
                return RedirectToAction("Login");
            }

        }

        [HttpGet]
        public IActionResult FilterReasonByProduct(string selectedProductID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            // Replace this with your logic to filter products based on the lineId
            var groupedProducts = db.TbReason
                 .Where(x => x.ProductID.Equals(selectedProductID) && x.PlantID.Equals(PlantID))
                 .GroupBy(x => new { x.ReasonID, x.ReasonName })
                 .Select(group => new
                 {
                     ProductID = group.Key.ReasonID,
                     ProductName = group.Key.ReasonName
                 }).ToList();
            return Json(groupedProducts);
        }



        [HttpPost]
        public ActionResult ProductQtyDefect(TbProductionTransaction obj, string employeeID, string productID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var currentDateTime = DateTime.Now;
            var currentDate = currentDateTime.Date;
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                view_Employee = db.View_Employee.ToList(),
                tbProductionTransaction = db.TbProductionTransaction.ToList(),
                tbReason = db.TbReason.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList()
            };

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {

                var objEmp = db.TbEmployeeTransaction
                   .Where(x => x.EmployeeID.Equals(employeeID) &&
                               x.TransactionDate.Date == currentDate &&
                               x.Plant.Equals(PlantID))
                   .OrderByDescending(x => x.TransactionNo)
                   .FirstOrDefault();

                var objPLPS = db.View_PLPS
                           .Where(x => x.PlantID.Equals(PlantID) &&
                                       x.LineID.Equals(objEmp.Line.ToString()) &&
                                       x.ProductID.Equals(productID) &&
                                       x.SectionID.Equals(objEmp.Section.ToString()))
                           .FirstOrDefault();





                db.TbProductionTransaction.Add(new TbProductionTransaction()
                {
                    // TransactionNo = db.TbProductionTransaction.Count() + 1,
                    TransactionDate = DateTime.Now,
                    PlantID = PlantID,
                    LineID = objEmp.Line,
                    SectionID = objEmp.Section,
                    ProductID = productID,
                    QRCode = employeeID,
                    Qty = obj.Qty,
                    QtyPerQR = Convert.ToInt32(objPLPS.QTYPerQRCode),//Get from PLPS
                    DataType = "Defect",
                    Reason = obj.Reason,
                    Note = "",
                    PackageRef = 0,
                    EmployeeRef = "",
                    GroupRef = "",
                    CreateDate = DateTime.Today,
                    CreateBy = EmpID,
                    UpdateDate = DateTime.Today,
                    UpdateBy = EmpID
                });
                db.SaveChanges();


                return View("WorkingFunction", mymodel);
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Working page had problem! please contact IT";
                return RedirectToAction("Login");
            }

        }



        public ActionResult SetUpRefreshTime()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbSetup = db.TbSetup.Where(x=>x.PlantID.Equals(PlantID)).ToList()
            };


            return View(mymodel);
            //var objPLPS = db.TbSetup
            //         .Where(x => x.Name.Equals("RefreshTime") && x.PlantID.Equals(PlantID)).FirstOrDefault();

            //int refrestime = objPLPS.Valuesetup;

            //// Return section and unit as JSON
            //return Json(new { refrestime = refrestime });

        }

        public ActionResult SetUpRefreshTimeSubmit(TbSetup obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbSetup = db.TbSetup.Where(x => x.PlantID.Equals(PlantID)).ToList()
            };


            var SetupDb = db.TbSetup.Where(x => x.Name.Equals("RefreshTime") && x.PlantID.Equals(PlantID)).FirstOrDefault();
            if (SetupDb != null)
            {
                SetupDb.Valuesetup = obj.Valuesetup;
                SetupDb.UpdateBy = EmpID;
                SetupDb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return View("SetUpRefreshTime",mymodel);
            //// Return section and unit as JSON
            //return Json(new { refrestime = refrestime });

        }


        [HttpGet]
        public ActionResult RollBackDataProduction(View_RollBackData obj, string[] Productchk ,string ProductTo)
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
                view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                View_RollBackData = db.View_RollBackData.Where(x => x.PlantID.Equals(PlantID) && x.ProductionDate == DateTime.Today).ToList()
        };

            ViewBag.VBRoleRollBackDataProduction = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(29)).Select(x => x.RoleAction).FirstOrDefault();
            if (Productchk.Length == 0)
            {


                if (!string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.SectionName) || obj.ProductionDate != DateTime.MinValue)
                {

                    if (!string.IsNullOrEmpty(obj.EmployeeID))
                    {

                        mymodel.View_RollBackData = mymodel.View_RollBackData.Where(x => x.EmployeeID.Contains(obj.EmployeeID)).ToList();

                    }
                    if (!string.IsNullOrEmpty(obj.LineName))
                    {
                        ViewBag.SelectedLineName = obj.LineName;
                        mymodel.View_RollBackData = mymodel.View_RollBackData.Where(p => p.LineID == obj.LineName).ToList();
                    }

                    if (!string.IsNullOrEmpty(obj.SectionID))
                    {
                        ViewBag.SelectedSectionName = obj.SectionID;
                        mymodel.View_RollBackData = mymodel.View_RollBackData.Where(p => p.SectionID == obj.SectionName).ToList();
                    }

                    if (obj.ProductionDate != DateTime.MinValue)
                    {
                        ViewBag.SelectedProductionDate = obj.ProductionDate;
                        mymodel.View_RollBackData = mymodel.View_RollBackData.Where(p => p.ProductionDate == obj.ProductionDate).ToList();
                    }
                    return View(mymodel);
                }
              //  mymodel.View_RollBackData = db.View_RollBackData.Where(x => x.PlantID.Equals(PlantID) && x.ProductionDate == DateTime.Today).ToList();

                return View(mymodel);
            }
            else
            {
                int datacnt = Productchk.Count();
                for (int i = 0; i < datacnt; ++i)
                {

                    //select 
                    long runningno = Convert.ToInt64(Productchk[i]);
                    var selectview = db.View_RollBackData.Where(x => x.RunningNumber == runningno).SingleOrDefault();
                    var TransactiodbUpdate = db.TbProductionTransaction.Where(p => p.PlantID.Equals(PlantID) && p.TransactionDate.Date == DateTime.Today && p.LineID.Equals(selectview.LineID) && p.SectionID.Equals(selectview.SectionID) && p.ProductID.Equals(selectview.ProductID) && p.QRCode.Equals(selectview.QRCode)).ToList();

                    //Check PLPS

                    var PLPSdata = db.TbPLPS.Where(x => x.PlantID.Equals(PlantID) && x.LineID.Equals(selectview.LineID) && x.ProductID.Equals(ProductTo)).ToList();
                        if(PLPSdata.Count == 0)
                    {
                        TempData["AlertMessage"] = "Please check PLPS ";
                    }
                    foreach (var product in TransactiodbUpdate)
                    {
                        // Apply changes to the list
                        product.ProductID = ProductTo; // Apply a 10% discount
                    }

                    db.SaveChanges();

                }
            }
            TempData["SuccessMessage"] = "Data successfully updated.";
            return RedirectToAction("RollBackDataProduction");
            //// Return section and unit as JSON
            //return Json(new { refrestime = refrestime });

        }



        /// <summary>
        /// //////////////////
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ActionResult RollBackDataProductionClear(TbProductionTransaction obj)
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
                view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeClocktime = db.View_EmployeeClocktime.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                View_RollBackData = db.View_RollBackData.Where(x => x.PlantID.Equals(PlantID)).ToList(),
            };

            mymodel.View_RollBackData = db.View_RollBackData.Where(x => x.PlantID.Equals(PlantID) && x.ProductionDate == DateTime.Today).ToList();

            return View("RollBackDataProduction",mymodel);
            //// Return section and unit as JSON
            //return Json(new { refrestime = refrestime });

        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ActionResult ImportManualData(TbProductionTransaction obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                //tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                //tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                //tbShift = db.TbShift.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                //view_ProductionPlan = db.View_ProductionPlan.Where(x => x.PlantID == PlantID).ToList(),
                view_PLPS = db.View_PLPS.Where(p => p.PlantID.Equals(PlantID)).ToList(),
                tbProductionTransaction = db.TbProductionTransaction.Where(p => p.PlantID.Equals(PlantID)).ToList()


            };

            ViewBag.VBRoleManualImport = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(32)).Select(x => x.RoleAction).FirstOrDefault();


            return View(mymodel);
            //// Return section and unit as JSON
            //return Json(new { refrestime = refrestime });

        }


        

       [HttpGet]
        public ActionResult ImportManualExport(View_ProductionPlan obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                //tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                //tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                //tbShift = db.TbShift.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                //view_ProductionPlan = db.View_ProductionPlan.Where(x => x.PlantID == PlantID).ToList(),
                view_PLPS = db.View_PLPS.Where(p => p.PlantID.Equals(PlantID)).ToList(),
                tbProductionTransaction = db.TbProductionTransaction.Where(p => p.PlantID.Equals(PlantID)).ToList()


            };

            ViewBag.VBRoleManualImport = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(32)).Select(x => x.RoleAction).FirstOrDefault();
            if (!string.IsNullOrEmpty(obj.SectionName) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.ProductName))
            {//obj.PlanDate.HasValue != false ||

                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    mymodel.view_ProductionPlan = mymodel.view_ProductionPlan.Where(x => x.SectionName.Equals(obj.SectionName)).ToList();
                    ViewBag.SelectedSectionName = obj.SectionName;
                }

                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    mymodel.view_ProductionPlan = mymodel.view_ProductionPlan.Where(x => x.LineName.Equals(obj.LineName)).ToList();
                    ViewBag.SelectedLineName = obj.LineName;
                }
                if (!string.IsNullOrEmpty(obj.ProductName))
                {
                    mymodel.view_ProductionPlan = mymodel.view_ProductionPlan.Where(x => x.ProductName.Equals(obj.ProductName)).ToList();
                    ViewBag.SelectedProductName = obj.ProductName;
                }



                var collection = mymodel.tbProductionTransaction.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("ImportManualData");
                Sheet.Cells["A1"].Value = "TransactionDate";
                Sheet.Cells["B1"].Value = "PlantID";
                Sheet.Cells["C1"].Value = "LineID";
                Sheet.Cells["D1"].Value = "SectionID";
                Sheet.Cells["E1"].Value = "ProductID";
                Sheet.Cells["F1"].Value = "QRCode";
                Sheet.Cells["G1"].Value = "Qty";
                Sheet.Cells["H1"].Value = "QtyPerQR";
                Sheet.Cells["I1"].Value = "EmployeeRef";
                int row = 2;
                foreach (var item in collection)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = "yyyy-MM-dd";
                    // Sheet.Cells[string.Format("B{0}", row)].Value = item.PlanDate;
                    Sheet.Cells[string.Format("B{0}", row)].Value = PlantID;
                    Sheet.Cells[string.Format("C{0}", row)].Value = "000001";
                    Sheet.Cells[string.Format("D{0}", row)].Value = PlantID+"000001";
                    Sheet.Cells[string.Format("E{0}", row)].Value = "000001";
                    Sheet.Cells[string.Format("F{0}", row)].Value = "xxxxxxx";
                    Sheet.Cells[string.Format("G{0}", row)].Value = "1";
                    Sheet.Cells[string.Format("H{0}", row)].Value = "1";
                    Sheet.Cells[string.Format("I{0}", row)].Value = "xxxxxxx";
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=ImportManualData.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());



                return RedirectToAction("ImportManualData", mymodel);

            }
            else
            {

                var collection = mymodel.tbProductionTransaction.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("ImportManualData");
                Sheet.Cells["A1"].Value = "TransactionDate";
                Sheet.Cells["B1"].Value = "PlantID";
                Sheet.Cells["C1"].Value = "LineID";
                Sheet.Cells["D1"].Value = "SectionID";
                Sheet.Cells["E1"].Value = "ProductID";
                Sheet.Cells["F1"].Value = "QRCode";
                Sheet.Cells["G1"].Value = "Qty";
                Sheet.Cells["H1"].Value = "QtyPerQR";
                Sheet.Cells["I1"].Value = "EmployeeRef";
                int row = 2;
                foreach (var item in collection)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = "yyyy-MM-dd";
                    // Sheet.Cells[string.Format("B{0}", row)].Value = item.PlanDate;
                    Sheet.Cells[string.Format("B{0}", row)].Value = PlantID;
                    Sheet.Cells[string.Format("C{0}", row)].Value = "000001";
                    Sheet.Cells[string.Format("D{0}", row)].Value = PlantID + "000001";
                    Sheet.Cells[string.Format("E{0}", row)].Value = "000001";
                    Sheet.Cells[string.Format("F{0}", row)].Value = "xxxxxxx";
                    Sheet.Cells[string.Format("G{0}", row)].Value = "1";
                    Sheet.Cells[string.Format("H{0}", row)].Value = "1";
                    Sheet.Cells[string.Format("I{0}", row)].Value = "xxxxxxx";
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=ImportManualData.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());


                //ViewBag.InactiveStatus = true;
                return RedirectToAction("ImportManualData", mymodel);
            }

        }


        
        [HttpPost]
        public IActionResult ImportManualUpload(IFormFile FileUpload)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            if (FileUpload == null || FileUpload.Length <= 0)
            {
                var mymodel = new ViewModelAll
                {
                    view_PermissionMaster = db.View_PermissionMaster.ToList(),
                    tbProductionTransaction = db.TbProductionTransaction.Where(p => p.PlantID.Equals(PlantID)).ToList()
                };

                ViewBag.Error = "Please select a valid Excel file.";
                return View("ImportManualData", mymodel);
            }

            using (var stream = new MemoryStream())
            {
                FileUpload.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        if (worksheet.Cells[row, 2].Value != null)
                        {

                            string LineVar = worksheet.Cells[row, 3].Text;
                            string SectionVar = worksheet.Cells[row, 4].Text;
                            string ProductVar = worksheet.Cells[row, 5].Text;
                            string EmployeeVar = worksheet.Cells[row, 6].Text;
                            string EmployeeRefVar = worksheet.Cells[row, 9].Text;

                            var LineIDDb = db.TbLine.Where(x => x.LineID.Equals(LineVar)).Select(x => x.LineID).SingleOrDefault();
                            var ProductIDDb = db.TbProduct.Where(x => x.ProductID.Equals(ProductVar)).Select(x => x.ProductID).SingleOrDefault();
                            var SectionIDDb = db.TbSection.Where(x => x.SectionID.Equals(SectionVar)).Select(x => x.SectionID).SingleOrDefault();
                            var EmployeeIDDb = db.TbEmployeeMaster.Where(x => x.EmployeeID.Equals(EmployeeVar)).Select(x => x.EmployeeID).SingleOrDefault();
                            var EmployeeRefIDDb = "";

                            if (EmployeeRefVar != "")
                            {
                                 EmployeeRefIDDb = db.TbEmployeeMaster.Where(x => x.EmployeeID.Equals(EmployeeRefVar)).Select(x => x.EmployeeID).SingleOrDefault();

                            }
                            else
                            {
                                 EmployeeRefIDDb = "";

                            }

                      
                            if (LineIDDb == null || ProductIDDb == null || SectionIDDb == null || EmployeeIDDb == null || EmployeeRefIDDb == null)
                            {
                                int rowerror = row - 1;
                                TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake ";
                                return RedirectToAction("ImportManualData");

                            }


                            else
                            {


                                    //int CntDb = db.TbProductSTD.ToList().Count;
                                    //int CntDbnext = CntDb + 1;
                             
                                    // Insert new record
                                    var newData = new TbProductionTransaction
                                    {

                                     TransactionDate = Convert.ToDateTime(worksheet.Cells[row, 1].Text),
                                    PlantID = PlantID,
                                    LineID = LineIDDb,
                                    SectionID = SectionIDDb,
                                    ProductID = ProductIDDb,
                                    QRCode = EmployeeIDDb,
                                    Qty = Convert.ToInt32(worksheet.Cells[row, 7].Text),
                                    QtyPerQR = Convert.ToInt32(worksheet.Cells[row, 8].Text),
                                    DataType = "Count",
                                    Reason = "",
                                    Note = "",
                                    PackageRef = 1,
                                    GroupRef = "",
                                    EmployeeRef = worksheet.Cells[row, 9].Text,
                                        CreateDate = DateTime.Now,
                                        CreateBy = EmpID,
                                        UpdateDate = DateTime.Now,
                                    UpdateBy = EmpID,


                                    };


                                    db.TbProductionTransaction.Add(newData);


                               
                            }
                        }

                    }
                    db.SaveChanges();
                }

            }

            ViewBag.Success = "Data imported and updated successfully!";
            return RedirectToAction("ImportManualData");

        }



        //DateTime startDate, DateTime endDate,
        [HttpGet]
        public async Task<IActionResult> DailyReport(string EmployeeID , DateTime StartDate , DateTime EndDate , string LineID ,string SectionID,string Prefix)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            // Check if user is logged in
            if (string.IsNullOrEmpty(EmpID))
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID == PlantID).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID == PlantID).ToList(),
                view_DailyReportSummary = db.View_DailyReportSummary.Where(x => x.PlantID.Equals(PlantID)).ToList()
            };

          

            ViewBag.VBRoleDailyReport = mymodel.view_PermissionMaster
                                            .Where(x => x.UserEmpID == EmpID && x.PageID == 23)
                                            .Select(x => x.RoleAction)
                                            .FirstOrDefault();


           
            if (!string.IsNullOrEmpty(EmployeeID) || !string.IsNullOrEmpty(LineID) || !string.IsNullOrEmpty(SectionID) || !string.IsNullOrEmpty(Prefix) || StartDate != DateTime.MinValue || EndDate != DateTime.MinValue)
            {


                if (!string.IsNullOrEmpty(EmployeeID))
                {
                    mymodel.view_DailyReportSummary = mymodel.view_DailyReportSummary.Where(x => x.QRCode == EmployeeID).ToList();
                    ViewBag.SelectedEmpID = EmployeeID;
                }

                if (!string.IsNullOrEmpty(LineID))
                {
                    mymodel.view_DailyReportSummary = mymodel.view_DailyReportSummary.Where(x => x.LineID == LineID).ToList();
                    ViewBag.SelectedLineID = LineID;
                }

                if (!string.IsNullOrEmpty(SectionID))
                {
                    mymodel.view_DailyReportSummary = mymodel.view_DailyReportSummary.Where(x => x.SectionID == SectionID).ToList();
                    ViewBag.SelectedSectionID = SectionID;
                }
                if (!string.IsNullOrEmpty(Prefix))
                {
                    mymodel.view_DailyReportSummary = mymodel.view_DailyReportSummary.Where(x => x.Prefix == Prefix).ToList();
                    ViewBag.SelectedPrefix = Prefix;
                }

                if ( StartDate != DateTime.MinValue && EndDate != DateTime.MinValue)
                {
                     mymodel.view_DailyReportSummary = mymodel.view_DailyReportSummary
                        .Where(x => x.TransactionDate >= StartDate && x.TransactionDate <= EndDate)
                        .ToList();

                    ViewBag.SelectedStartDate = StartDate.ToString("yyyy-MM-dd"); 
                    ViewBag.SelectedEndDate = EndDate.ToString("yyyy-MM-dd"); ;

                }
                else if (StartDate != DateTime.MinValue)
                {
                    mymodel.view_DailyReportSummary = mymodel.view_DailyReportSummary
                         .Where(x => x.TransactionDate >= StartDate)
                         .ToList();

                    ViewBag.SelectedStartDate = StartDate.ToString("yyyy-MM-dd"); ;
                }
                else if (EndDate != DateTime.MinValue)
                {
                    mymodel.view_DailyReportSummary = mymodel.view_DailyReportSummary
                     .Where(x => x.TransactionDate <= EndDate)
                     .ToList();

                    ViewBag.SelectedEndDate = EndDate.ToString("yyyy-MM-dd"); ;
                }

                return View(mymodel);
            }
            else
            {

                //var varLine = from a in db.View_DailyReportSummary
                //              where a.PlantID.Equals(PlantID)
                //              group a by new { a.LineID, a.LineName } into g
                //              select new SelectListItem
                //              {
                //                  Value = $"{g.Key.LineID}",
                //                  Text = $"{g.Key.LineName}"
                //              };
                //ViewBag.varLine = new SelectList(varLine, "Value", "Text");


                //var varPoint = from a in db.View_DailyReportSummary
                //               where a.PlantID.Equals(PlantID)
                //               group a by new { a.SectionID, a.SectionName } into g
                //               select new SelectListItem
                //               {
                //                   Value = $"{g.Key.SectionID}",
                //                   Text = $"{g.Key.SectionName}"
                //               };
                //ViewBag.varPoint = new SelectList(varPoint, "Value", "Text");


                //var varEmp = from a in db.View_DailyReportSummary
                //               where a.PlantID.Equals(PlantID)
                //               group a by new { a.QRCode, a.EmployeeName  } into g
                //               select new SelectListItem
                //               {
                //                   Value = $"{g.Key.QRCode}",
                //                   Text = $"{g.Key.EmployeeName}"
                //               };
                //ViewBag.varEmp = new SelectList(varEmp, "Value", "Text");

                //var sumGrpEmp = db.View_DailyReportSummary
                //         .Where(mydata => mydata.TransactionDate == DateTime.Today && mydata.PlantID == PlantID)
                //         .ToList();


                //var mydata= new ViewModelReport
                //{
                //    view_PermissionMaster = db.View_PermissionMaster.ToList(),
                //    view_DailyReportSummary = sumGrpEmp,
                //    FilterLine = LineID,
                //    FilterEmp = EmployeeID,
                //    FilterPoint = SectionID,

                //};

                mymodel.view_DailyReportSummary = mymodel.view_DailyReportSummary.Where(x=> x.TransactionDate == DateTime.Today);
            
                return View(mymodel);

            }


           

        }





    //DateTime startDateString;
    //DateTime endDateString;




    public ActionResult DailyReportClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID == PlantID).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID == PlantID).ToList(),
                view_DailyReportSummary = db.View_DailyReportSummary.Where(x => x.PlantID.Equals(PlantID)).ToList()
            };

            ViewBag.VBRoleDailyReport = mymodel.view_PermissionMaster
                                          .Where(x => x.UserEmpID == EmpID && x.PageID == 23)
                                          .Select(x => x.RoleAction)
                                          .FirstOrDefault();

            mymodel.view_DailyReportSummary = db.View_DailyReportSummary.Where(x => x.TransactionDate.Equals(DateTime.Today)).ToList();
            return View("DailyReport", mymodel);

        }

        [HttpGet]
        public ActionResult DailyReportExport(string EmployeeID, DateTime StartDate, DateTime EndDate, string LineID, string SectionID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }


            try
            {


                var mymodel = new ViewModelAll
                {

                    tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID == PlantID).ToList(),
                    tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                    tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                    tbShift = db.TbShift.Where(x => x.PlantID == PlantID).ToList(),
                    view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID == PlantID).ToList(),
                    view_DailyReportSummary = db.View_DailyReportSummary.Where(x => x.PlantID.Equals(PlantID)).ToList()

                };

                // Check Admin
                if (PlantID != 0)
                {
                    mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                }


                ViewBag.VBRoleDailyReport = mymodel.view_PermissionMaster
                                          .Where(x => x.UserEmpID == EmpID && x.PageID == 23)
                                          .Select(x => x.RoleAction)
                                          .FirstOrDefault();



                if (!string.IsNullOrEmpty(EmployeeID) || !string.IsNullOrEmpty(LineID) || !string.IsNullOrEmpty(SectionID) || StartDate != DateTime.MinValue || EndDate != DateTime.MinValue)
                {
                    if (!string.IsNullOrEmpty(EmployeeID))
                    {
                        mymodel.view_DailyReportSummary = mymodel.view_DailyReportSummary.Where(x => x.QRCode == EmployeeID).ToList();
                        ViewBag.SelectedEmpID = EmployeeID;
                    }
                    if (!string.IsNullOrEmpty(LineID))
                    {
                        mymodel.view_DailyReportSummary = mymodel.view_DailyReportSummary.Where(x => x.LineID == LineID).ToList();
                        ViewBag.SelectedLineID = LineID;
                    }
                    if (!string.IsNullOrEmpty(SectionID))
                    {
                        mymodel.view_DailyReportSummary = mymodel.view_DailyReportSummary.Where(x => x.SectionID == SectionID).ToList();
                        ViewBag.SelectedSectionID = SectionID;
                    }
                    else if (StartDate != DateTime.MinValue)
                    {
                        mymodel.view_DailyReportSummary = mymodel.view_DailyReportSummary
                             .Where(x => x.TransactionDate >= StartDate)
                             .ToList();

                        ViewBag.SelectedStartDate = StartDate;
                    }
                    else if (EndDate != DateTime.MinValue)
                    {
                        mymodel.view_DailyReportSummary = mymodel.view_DailyReportSummary
                         .Where(x => x.TransactionDate <= EndDate)
                         .ToList();

                        ViewBag.SelectedEndDate = EndDate;
                    }



                    var collection = mymodel.view_DailyReportSummary.ToList();
                    ExcelPackage Ep = new ExcelPackage();
                    ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Dailyreport");
                    Sheet.Cells["A1"].Value = "Plant";
                    Sheet.Cells["B1"].Value = "Line";
                    Sheet.Cells["C1"].Value = "Date";

                    Sheet.Cells["D1"].Value = "Shift";
                    Sheet.Cells["E1"].Value = "Product";
                    Sheet.Cells["F1"].Value = "Employee ID";

                    Sheet.Cells["G1"].Value = "Employee Name";
                    Sheet.Cells["H1"].Value = "Section";
                    Sheet.Cells["I1"].Value = "Total Count";

                    Sheet.Cells["J1"].Value = "Total Peice Real Time Employee";
                    Sheet.Cells["K1"].Value = "Total Defect Real Time Employee";
                    Sheet.Cells["L1"].Value = "Total Defect Adjust";
                    Sheet.Cells["M1"].Value = "Actual FG ";
                    Sheet.Cells["N1"].Value = "TotalPiece";

                    Sheet.Cells["O1"].Value = "Work Hours";
                    Sheet.Cells["P1"].Value = " % Yield";
                    Sheet.Cells["Q1"].Value = " Piece Per Hr.";

                    Sheet.Cells["R1"].Value = " EFF.-M/STD";
                    Sheet.Cells["S1"].Value = "Grade Eff. Real Time";
                    Sheet.Cells["T1"].Value = "wage Real Time Per Employee";

                    for (char col = 'A'; col <= 'T'; col++)
                    {
                        Sheet.Cells[$"{col}1"].Style.Font.Bold = true;
                    }

                    int row = 2;
                    int sumTotalCount = 0;
                    int sumTotalPeice = 0;
                    int sumTotalDefect = 0;
                    decimal sumTotalDefectAll = 0;
                    decimal sumTotalActualFG = 0;

                    decimal sumTotalHr = 0;
                    int sumTotalYield = 0;
                    decimal sumTotalWage = 0;
                    foreach (var item in collection)
                    {

                        Sheet.Cells[string.Format("A{0}", row)].Value = item.PlantID;
                        Sheet.Cells[string.Format("B{0}", row)].Value = item.LineName;
                        Sheet.Cells[string.Format("C{0}", row)].Value = item.TransactionDate;

                        Sheet.Cells[string.Format("D{0}", row)].Value = item.ShiftName;
                        Sheet.Cells[string.Format("E{0}", row)].Value = item.ProductName;
                        Sheet.Cells[string.Format("F{0}", row)].Value = item.QRCode;

                        Sheet.Cells[string.Format("G{0}", row)].Value = item.EmployeeName;
                        Sheet.Cells[string.Format("H{0}", row)].Value = item.SectionName;
                        Sheet.Cells[string.Format("I{0}", row)].Value = item.CountQty;
                        sumTotalCount = sumTotalCount + item.CountQty;

                        Sheet.Cells[string.Format("J{0}", row)].Value = item.FGQty;
                        sumTotalPeice = sumTotalPeice + item.FGQty;

                        Sheet.Cells[string.Format("K{0}", row)].Value = item.DefectQty;
                        sumTotalDefect = sumTotalDefect + item.DefectQty;

                        Sheet.Cells[string.Format("L{0}", row)].Value = item.TotalDefect;  //Total defect adjust
                        sumTotalDefectAll = sumTotalDefectAll + item.TotalDefect;

                        Sheet.Cells[string.Format("M{0}", row)].Value = item.ActualFG;  //Actual FG
                        sumTotalActualFG = sumTotalActualFG + item.ActualFG;

                        Sheet.Cells[string.Format("N{0}", row)].Value = 0;  //Total Piece
                        sumTotalDefectAll = sumTotalDefectAll + 0;


                        Sheet.Cells[string.Format("O{0}", row)].Value = item.DiffHours;
                        sumTotalHr = sumTotalHr + item.DiffHours;

                        Sheet.Cells[string.Format("P{0}", row)].Value = item.YieldDefect;
                        Sheet.Cells[string.Format("Q{0}", row)].Value = item.PiecePerHr;

                        Sheet.Cells[string.Format("R{0}", row)].Value = item.EffManPerSTD;
                        Sheet.Cells[string.Format("S{0}", row)].Value = item.Grade;
                        Sheet.Cells[string.Format("T{0}", row)].Value = item.wage;
                        sumTotalWage = sumTotalWage + item.wage;

                        row++;
                    }

                    Sheet.Cells[string.Format("H{0}", row)].Value = "Total";
                    Sheet.Cells[string.Format("I{0}", row)].Value = sumTotalCount;
                    Sheet.Cells[string.Format("J{0}", row)].Value = sumTotalPeice;
                    Sheet.Cells[string.Format("K{0}", row)].Value = sumTotalDefect;
                    Sheet.Cells[string.Format("L{0}", row)].Value = sumTotalDefectAll;

                    Sheet.Cells[string.Format("O{0}", row)].Value = sumTotalHr;//DiffHours
                    Sheet.Cells[string.Format("P{0}", row)].Value = (sumTotalPeice- sumTotalDefect) / sumTotalPeice *100; //YieldDefect
                    Sheet.Cells[string.Format("Q{0}", row)].Value = sumTotalPeice/ sumTotalHr; // PiecePerHr
                    Sheet.Cells[string.Format("T{0}", row)].Value = sumTotalWage;//WAGE

                    for (char col = 'H'; col <= 'T'; col++)
                    {
                        Sheet.Cells[$"{col}{row}"].Style.Font.Bold = true;
                    }


                    Sheet.Cells["A:AZ"].AutoFitColumns();
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.Headers.Add("content-disposition", "attachment; filename=DailyReport.xlsx"); // Fix typo ':' should be ';'
                    Response.Body.WriteAsync(Ep.GetAsByteArray());


                    // mymodel.tbLine = db.TbLine.Where(p => p.LineName.Equals(obj.LineName) || p.LineID.Equals(obj.LineID)).OrderByDescending(x => x.Status);
                    return View("DailyReport", mymodel);
                }
                else
                {

                    var collection = mymodel.view_DailyReportSummary.ToList();
                    using (var Ep = new ExcelPackage())
                    {
                        var Sheet = Ep.Workbook.Worksheets.Add("DailyReport");
                        Sheet.Cells["A1"].Value = "Plant";
                        Sheet.Cells["B1"].Value = "Line";
                        Sheet.Cells["C1"].Value = "Date";

                        Sheet.Cells["D1"].Value = "Shift";
                        Sheet.Cells["E1"].Value = "Product";
                        Sheet.Cells["F1"].Value = "Employee ID";

                        Sheet.Cells["G1"].Value = "Employee Name";
                        Sheet.Cells["H1"].Value = "Section";
                        Sheet.Cells["I1"].Value = "Total Count";

                        Sheet.Cells["J1"].Value = "Total Peice Real Time Employee";
                        Sheet.Cells["K1"].Value = "Total Defect Real Time Employee";
                        Sheet.Cells["L1"].Value = "Total Defect Adjust";
                        Sheet.Cells["M1"].Value = "Actual FG ";
                        Sheet.Cells["N1"].Value = "TotalPiece";

                        Sheet.Cells["O1"].Value = "Work Hours";
                        Sheet.Cells["P1"].Value = " % Yield";
                        Sheet.Cells["Q1"].Value = " Piece Per Hr.";

                        Sheet.Cells["R1"].Value = " EFF.-M/STD";
                        Sheet.Cells["S1"].Value = "Grade Eff. Real Time";
                        Sheet.Cells["T1"].Value = "wage Real Time Per Employee";

                        for (char col = 'A'; col <= 'T'; col++)
                        {
                            Sheet.Cells[$"{col}1"].Style.Font.Bold = true;
                        }


                        int row = 2;

                        int sumTotalCount = 0;
                        int sumTotalPeice = 0;
                        int sumTotalDefect = 0;
                        int sumTotalDefectAll = 0;
                        decimal sumTotalHr = 0;
                        int sumTotalYield = 0;
                        decimal sumTotalWage = 0;
                        foreach (var item in collection)
                        {
                            Sheet.Cells[string.Format("A{0}", row)].Value = item.PlantID;
                            Sheet.Cells[string.Format("B{0}", row)].Value = item.LineName;
                            Sheet.Cells[string.Format("C{0}", row)].Value = item.TransactionDate;

                            Sheet.Cells[string.Format("D{0}", row)].Value = item.ShiftName;
                            Sheet.Cells[string.Format("E{0}", row)].Value = item.ProductName;
                            Sheet.Cells[string.Format("F{0}", row)].Value = item.QRCode;

                            Sheet.Cells[string.Format("G{0}", row)].Value = item.EmployeeName;
                            Sheet.Cells[string.Format("H{0}", row)].Value = item.SectionName;
                            Sheet.Cells[string.Format("I{0}", row)].Value = item.CountQty;
                            sumTotalCount = sumTotalCount + item.CountQty;

                            Sheet.Cells[string.Format("J{0}", row)].Value = item.FGQty;
                            sumTotalPeice = sumTotalPeice + item.FGQty;

                            Sheet.Cells[string.Format("K{0}", row)].Value = item.DefectQty;
                            sumTotalDefect = sumTotalDefect + item.DefectQty;

                            Sheet.Cells[string.Format("L{0}", row)].Value = 0;  //Total defect adjust
                            sumTotalDefectAll = sumTotalDefectAll + 0;

                            Sheet.Cells[string.Format("M{0}", row)].Value = 0;  //Actual FG
                            sumTotalDefectAll = sumTotalDefectAll + 0;

                            Sheet.Cells[string.Format("N{0}", row)].Value = 0;  //Total Piece
                            sumTotalDefectAll = sumTotalDefectAll + 0;


                            Sheet.Cells[string.Format("O{0}", row)].Value = item.DiffHours;
                            sumTotalHr = sumTotalHr + item.DiffHours;

                            Sheet.Cells[string.Format("P{0}", row)].Value = item.YieldDefect;
                            Sheet.Cells[string.Format("Q{0}", row)].Value = item.PiecePerHr;

                            Sheet.Cells[string.Format("R{0}", row)].Value = item.EffManPerSTD;
                            Sheet.Cells[string.Format("S{0}", row)].Value = item.Grade;
                            Sheet.Cells[string.Format("T{0}", row)].Value = item.wage;
                            sumTotalWage = sumTotalWage + item.wage;


                            row++;
                        }



                        Sheet.Cells[string.Format("H{0}", row)].Value = "Total";
                        Sheet.Cells[string.Format("I{0}", row)].Value = sumTotalCount;
                        Sheet.Cells[string.Format("J{0}", row)].Value = sumTotalPeice;
                        Sheet.Cells[string.Format("K{0}", row)].Value = sumTotalDefect;
                        Sheet.Cells[string.Format("L{0}", row)].Value = sumTotalDefectAll;

                        Sheet.Cells[string.Format("O{0}", row)].Value = sumTotalHr;//DiffHours
                        Sheet.Cells[string.Format("P{0}", row)].Value = (sumTotalPeice - sumTotalDefect) / sumTotalPeice * 100; //YieldDefect
                        Sheet.Cells[string.Format("Q{0}", row)].Value = sumTotalPeice / sumTotalHr; // PiecePerHr
                        Sheet.Cells[string.Format("T{0}", row)].Value = sumTotalWage;//WAGE

                        for (char col = 'H'; col <= 'T'; col++)
                        {
                            Sheet.Cells[$"{col}{row}"].Style.Font.Bold = true;
                        }

                        Sheet.Cells["A:AZ"].AutoFitColumns();
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.Headers.Add("content-disposition", "attachment; filename=DailyReport.xlsx"); // Fix typo ':' should be ';'
                        Response.Body.WriteAsync(Ep.GetAsByteArray());


                        // Send the Excel file as the response
                        //  return File(content, contentType, fileName);
                    }


                    ViewBag.InactiveStatus = true;
                    return RedirectToAction("DailyReport", mymodel);

                }
            }
            catch
            {
                TempData["AlertMessage"] = "System Some has Problem in Line, Plese contact IT!";
                return RedirectToAction("Login");

            }


        }








        [HttpGet]
        public ActionResult WorkingFunctionWithPackage(TbProductionTransaction obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var mymodel = new ViewModelAll
                {
                    view_PermissionMaster = db.View_PermissionMaster.ToList(),
                    tbPlants = db.TbPlant.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                    tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                    tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                    tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                    view_Employee = db.View_Employee.ToList(),
                    tbReason = db.TbReason.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                    //tbPackage = db.TbPackage.ToList()
                };
                return View(mymodel);
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Working page had problem! please contact IT";
                return RedirectToAction("Login");
            }

        }


        public ActionResult WorkingFunctionCreateWithRef(string employeeId, string productId, String EmployeeRef)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
            //    view_Employee = db.View_Employee.ToList(),
                tbReason = db.TbReason.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbPLPS = db.TbPLPS.Where(x => x.PlantID.Equals(PlantID) && x.ProductID.Equals(productId)).ToList()
                //  tbProductionTransaction = db.TbProductionTransaction.ToList()
            };

            // check QRcode in system
            if (db.TbEmployeeMaster.Where(x => x.EmployeeID.Equals(employeeId)) == null || db.TbEmployeeGroupQR.Where(x => x.GroupID.Equals(employeeId)) == null)
            {
                TempData["AlertMessage"] = "QR Code is not available in the system!";

                return Json(mymodel);
            }

            if (db.View_PermissionMaster.Where(x => x.UserEmpID.Equals(EmpID) && x.PageID.Equals(22)).Select(x => x.RoleAction).SingleOrDefault() == "Full")
            {
                var currentDateTime = DateTime.Now;
                var currentDate = currentDateTime.Date;
                var currentTime = currentDateTime.TimeOfDay;
                var currentDatebefore = currentDateTime.Date.AddDays(-1);
                string sectionval = "";
                try
                {
                    //check employee or Group
                    if(db.TbEmployeeMaster.Where(x => x.EmployeeID.Equals(employeeId)).Count() != 0) // case Employee
                    {

                        //Check EmployeeClockin
                     
                       var objEmpcount = db.View_ClockTime.Where(x => x.EmployeeID.Equals(employeeId) &&
                                                                              (x.TransactionDate.Date == currentDate || x.TransactionDate.Date == currentDatebefore) &&
                                                                              x.PlantID.Equals(PlantID) &&
                                                                               (((x.ClockOut == null || x.ClockOut == "") && x.Remark == "") ||
                                                                               ((x.ClockOut != null || x.ClockOut != "") && x.Remark == "Adjust")  )).ToList();

                        if (objEmpcount.Count > 1)
                        {
                            TempData["AlertMessage"] = "Please check clock out!";
                            return Json(mymodel);
                        }
                        else
                        {
                            var objEmp = db.View_ClockTime
                          .Where(x => x.EmployeeID.Equals(employeeId) &&
                                       (x.TransactionDate.Date == currentDate || x.TransactionDate.Date == currentDatebefore) &&
                                      x.PlantID.Equals(PlantID) &&
                                      (((x.ClockOut == null || x.ClockOut == "") && x.Remark == "") ||
                                         ((x.ClockOut != null || x.ClockOut != "") && x.Remark == "Adjust")))
                          .FirstOrDefault();

                            if (objEmp != null)
                            {
                                var objPLPS = db.View_PLPS
                                .Where(x => x.PlantID.Equals(PlantID) &&
                                            x.LineID.Equals(objEmp.LineID.ToString()) &&
                                            x.ProductID.Equals(productId) &&
                                            x.SectionID.Equals(objEmp.SectionID.ToString()))
                                .FirstOrDefault();

                                // check formular for insert or return to enter Employee
                                if (objPLPS == null)
                                {
                                    var section = "Please check PLPS";
                                    return Json(section);
                                }
                                if (objPLPS.FormularID == 10)
                                {
                                    var section = "EmployeeRefSectionEnabled";
                                    return Json(section);
                                }
                                else
                                {
                                    if (objPLPS != null)
                                    {
                                        var LastTransactionTime = db.TbProductionTransaction
                                       .Where(x => x.QRCode.Equals(employeeId) && x.TransactionDate.Date == currentDate)
                                       .OrderByDescending(x => x.TransactionDate)
                                       .Select(x => x.TransactionDate.TimeOfDay)
                                       .FirstOrDefault();


                                        // Convert TimeSpan to total seconds
                                        double lastTransactionSeconds = LastTransactionTime.TotalSeconds;
                                        double delayTimeSeconds = Convert.ToDouble(objPLPS.Delaytime);
                                        double difftime = ((lastTransactionSeconds + delayTimeSeconds) - currentTime.TotalSeconds);
                                        double roundedDifftime = Math.Round(difftime, 2);

                                        if (lastTransactionSeconds + delayTimeSeconds < currentTime.TotalSeconds)
                                        {
                                            // Perform actions if LastTransactionTime is greater than or equal to objPLPS.Delaytime                  
                                            db.TbProductionTransaction.Add(new TbProductionTransaction()
                                            {
                                                // TransactionNo = db.TbProductionTransaction.Count() + 1,
                                                TransactionDate = objEmp.TransactionDate,//DateTime.Now,
                                                PlantID = Convert.ToInt32(objEmp.PlantID),
                                                LineID = objEmp.LineID,
                                                SectionID = objEmp.SectionID,
                                                ProductID = productId,
                                                Prefix = objEmp.Prefix,
                                                QRCode = employeeId,
                                                Qty = 1,
                                                QtyPerQR = Convert.ToInt32(objPLPS.QTYPerQRCode),//Get from PLPS
                                                DataType = "Count",
                                                Reason = "",
                                                Note = "",
                                                PackageRef = 0,
                                                EmployeeRef = string.IsNullOrEmpty(EmployeeRef) ? "" : EmployeeRef,
                                                GroupRef = "",
                                                CreateDate = DateTime.Now,
                                                CreateBy = EmpID,
                                                UpdateDate = DateTime.Now,
                                                UpdateBy = EmpID
                                            });
                                            db.SaveChanges();
                                            sectionval = objEmp.SectionID.ToString() + " : " + objPLPS.SectionName.ToString();
                                            return Json(sectionval);

                                        }
                                        else
                                        {
                                            // TempData["AlertMessage"] = "please check time!";
                                            var sectionvalalert = "check time : " + roundedDifftime + " Sec.";
                                            return Json(sectionvalalert);
                                        }
                                    }
                                    else
                                    {
                                        var sectionvalalert = "Check Master PLPS";
                                        return Json(sectionvalalert);
                                    }
                                }


                            }
                            else
                            {
                                var sectionvalalert = "Check Clock in time";
                                return Json(sectionvalalert);
                            }
                        }
                        //End case employee
                    }
                    else  // case group
                    {

                        //select group
                        var objgroup = db.TbEmployeeGroupQR.Where(x => x.GroupID.Equals(employeeId)).ToList();
                        foreach (var item in objgroup)
                        {

                            //Select EmployeeTransaction
                            var objEmp = db.View_ClockTime
                            .Where(x => x.EmployeeID.Equals(item.EmployeeID) &&
                                        x.TransactionDate.Date == currentDate &&
                                        x.PlantID.Equals(PlantID))
                            .FirstOrDefault();



                   //         var objEmp = db.View_ClockTime
                   //.Where(x => x.EmployeeID.Equals(employeeId) &&
                   //            x.TransactionDate.Date == currentDate &&
                   //            x.Plant.Equals(PlantID) &&
                   //            (((x.ClockOut == null || x.ClockOut == "") && x.Remark == "") ||
                   //               ((x.ClockOut != null || x.ClockOut != "") && x.Remark == "Adjust")))
                   //.FirstOrDefault();



                            if (objEmp != null)
                            {
                                var objPLPS = db.View_PLPS
                                .Where(x => x.PlantID.Equals(PlantID) &&
                                            x.LineID.Equals(objEmp.LineID.ToString()) &&
                                            x.ProductID.Equals(productId) &&
                                            x.SectionID.Equals(objEmp.SectionID.ToString()))
                                .FirstOrDefault();

                                if (objPLPS != null)
                                {
                                    var LastTransactionTime = db.TbProductionTransaction
                                   .Where(x => x.QRCode.Equals(objEmp.EmployeeID) && x.TransactionDate.Date == currentDate)
                                   .OrderByDescending(x => x.TransactionDate)
                                   .Select(x => x.TransactionDate.TimeOfDay)
                                   .FirstOrDefault();

                                    // Convert TimeSpan to total seconds
                                    double lastTransactionSeconds = LastTransactionTime.TotalSeconds;
                                    double delayTimeSeconds = Convert.ToDouble(objPLPS.Delaytime);
                                    double difftime = ((lastTransactionSeconds + delayTimeSeconds) - currentTime.TotalSeconds);
                                    double roundedDifftime = Math.Round(difftime, 2);


                                    if (lastTransactionSeconds + delayTimeSeconds < currentTime.TotalSeconds)
                                    {
                                        // Perform actions if LastTransactionTime is greater than or equal to objPLPS.Delaytime                  
                                        db.TbProductionTransaction.Add(new TbProductionTransaction()
                                        {
                                            // TransactionNo = db.TbProductionTransaction.Count() + 1,
                                            TransactionDate = objEmp.TransactionDate,//DateTime.Now,
                                            PlantID = Convert.ToInt32(objEmp.PlantID),
                                            LineID = objEmp.LineID,
                                            SectionID = objEmp.SectionID,
                                            ProductID = productId,
                                            Prefix = objEmp.Prefix,
                                            QRCode = objEmp.EmployeeID,
                                            Qty = 1,
                                            QtyPerQR = Convert.ToInt32(objPLPS.QTYPerQRCode),//Get from PLPS
                                            DataType = "Count",
                                            Reason = "",
                                            Note = "",
                                            PackageRef =  0,
                                            EmployeeRef = string.IsNullOrEmpty(EmployeeRef) ? "" : EmployeeRef,
                                            GroupRef = employeeId,
                                            CreateDate = DateTime.Now,
                                            CreateBy = EmpID,
                                            UpdateDate = DateTime.Now,
                                            UpdateBy = EmpID
                                        });
                                        db.SaveChanges();
                                        sectionval = objEmp.SectionID.ToString();


                                    }
                                    else
                                    {
                                      //  TempData["AlertMessage"] = "please check time!";
                                        var sectionvalalert = "check time :" + roundedDifftime + " sec.";
                                        return Json(sectionvalalert);
                                    }
                                }
                            }
                         
                        }
                       
                        return Json(sectionval);

                        //End case group
                    }
                       



                }
                catch
                {
                    TempData["AlertMessage"] = "Please check master data!";
                    return Json(mymodel);
                }
            }



            return Json(mymodel);
        }

        public ActionResult InsertIntoTbProductionTransaction(string employeeId, string productId, int PackageRef, String EmployeeRef)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                view_Employee = db.View_Employee.ToList(),
                tbReason = db.TbReason.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbPLPS = db.TbPLPS.Where(x => x.PlantID.Equals(PlantID) && x.ProductID.Equals(productId)).ToList()
                //  tbProductionTransaction = db.TbProductionTransaction.ToList()
            };

            // check QRcode in system
            if (db.TbEmployeeMaster.Where(x => x.EmployeeID.Equals(employeeId)) == null || db.TbEmployeeGroupQR.Where(x => x.GroupID.Equals(employeeId)) == null)
            {
                TempData["AlertMessage"] = "QR Code is not available in the system!";
                return Json(mymodel);
            }

            if (db.View_PermissionMaster.Where(x => x.UserEmpID.Equals(EmpID) && x.PageID.Equals(22)).Select(x => x.RoleAction).SingleOrDefault() == "Full")
            {
                var currentDateTime = DateTime.Now;
                var currentDate = currentDateTime.Date;
                var currentTime = currentDateTime.TimeOfDay;
                var currentDatebefore = currentDateTime.Date.AddDays(-1);
                string sectionval = "";
                try
                {
                    //check employee or Group
                    if (db.TbEmployeeMaster.Where(x => x.EmployeeID.Equals(employeeId)).Count() != 0) // case Employee
                    {
                        //Check EmployeeClockin

                        var objEmpcount = db.View_ClockTime.Where(x => x.EmployeeID.Equals(employeeId) &&
                                                                               (x.TransactionDate.Date == currentDate || x.TransactionDate.Date == currentDatebefore) &&
                                                                               x.PlantID.Equals(PlantID) &&
                                                                                (((x.ClockOut == null || x.ClockOut == "") && x.Remark == "") ||
                                                                                ((x.ClockOut != null || x.ClockOut != "") && x.Remark == "Adjust"))).ToList();

                        if (objEmpcount.Count > 1)
                        {
                            TempData["AlertMessage"] = "Please check clock out!";
                            return Json(mymodel);
                        }
                        else
                        {
                            var objEmp = db.View_ClockTime
                          .Where(x => x.EmployeeID.Equals(employeeId) &&
                                       (x.TransactionDate.Date == currentDate || x.TransactionDate.Date == currentDatebefore) &&
                                      x.PlantID.Equals(PlantID) &&
                                      (((x.ClockOut == null || x.ClockOut == "") && x.Remark == "") ||
                                         ((x.ClockOut != null || x.ClockOut != "") && x.Remark == "Adjust")))
                          .FirstOrDefault();

                            if (objEmp != null)
                            {
                                var objPLPS = db.View_PLPS
                                 .Where(x => x.PlantID.Equals(PlantID) &&
                                             x.LineID.Equals(objEmp.LineID.ToString()) &&
                                             x.ProductID.Equals(productId) &&
                                             x.SectionID.Equals(objEmp.SectionID.ToString()))
                                 .FirstOrDefault();

                                var objproductSTD = db.View_ProductSTD
                              .Where(x => x.PlantID.Equals(PlantID) &&
                                          x.LineID.Equals(objEmp.LineID.ToString()) &&
                                          x.ProductID.Equals(productId) &&
                                          x.SectionID.Equals(objEmp.SectionID.ToString()))
                              .FirstOrDefault();

                                if (objPLPS != null && objproductSTD != null)
                                {
                                    var LastTransactionTime = db.TbProductionTransaction
                                   .Where(x => x.QRCode.Equals(employeeId) && x.TransactionDate.Date == currentDate)
                                   .OrderByDescending(x => x.TransactionDate)
                                   .Select(x => x.TransactionDate.TimeOfDay)
                                   .FirstOrDefault();


                                    // Convert TimeSpan to total seconds
                                    double lastTransactionSeconds = LastTransactionTime.TotalSeconds;
                                    double delayTimeSeconds = Convert.ToDouble(objPLPS.Delaytime);

                                    if (lastTransactionSeconds + delayTimeSeconds < currentTime.TotalSeconds)
                                    {
                                        // Perform actions if LastTransactionTime is greater than or equal to objPLPS.Delaytime                  
                                        db.TbProductionTransaction.Add(new TbProductionTransaction()
                                        {
                                            // TransactionNo = db.TbProductionTransaction.Count() + 1,
                                            TransactionDate = objEmp.TransactionDate, //DateTime.Now,
                                            PlantID = Convert.ToInt32(objEmp.PlantID),
                                            LineID = objEmp.LineID,
                                            SectionID = objEmp.SectionID,
                                            ProductID = productId,
                                            QRCode = employeeId,
                                            Qty = 1,
                                            QtyPerQR = Convert.ToInt32(objPLPS.QTYPerQRCode),//Get from PLPS
                                            DataType = "Count",
                                            Reason = "",
                                            Note = "",
                                            PackageRef = PackageRef,
                                            EmployeeRef = string.IsNullOrEmpty(EmployeeRef) ? "" : EmployeeRef,
                                            GroupRef = "",
                                            CreateDate = DateTime.Now,
                                            CreateBy = EmpID,
                                            UpdateDate = DateTime.Now,
                                            UpdateBy = EmpID
                                        });
                                        db.SaveChanges();
                                        sectionval = objEmp.SectionID.ToString() + " : " + objPLPS.SectionName.ToString();
                                        return Json(sectionval);

                                    }
                                    else
                                    {
                                        TempData["AlertMessage"] = "please check time!";
                                        var sectionvalalert = "check time";
                                        ViewBag.ErrorAlert = "please check time!";
                                        return Json(sectionvalalert);
                                    }
                                }
                                else
                                {
                                    var sectionvalalert = "Check Master PLPS And ProductSTD";
                                    return Json(sectionvalalert);
                                }



                            }
                            else
                            {
                                var sectionvalalert = "Check Clock in time";
                                return Json(sectionvalalert);
                            }
                            //End case employee
                        }
                    }
                    else  // case group
                    {

                        //select group
                        var objgroup = db.TbEmployeeGroupQR.Where(x => x.GroupID.Equals(employeeId)).ToList();
                        foreach (var item in objgroup)
                        {

                            //Check EmployeeClockin

                            var objEmpcount = db.View_ClockTime.Where(x => x.EmployeeID.Equals(employeeId) &&
                                                                                   (x.TransactionDate.Date == currentDate || x.TransactionDate.Date == currentDatebefore) &&
                                                                                   x.PlantID.Equals(PlantID) &&
                                                                                    (((x.ClockOut == null || x.ClockOut == "") && x.Remark == "") ||
                                                                                    ((x.ClockOut != null || x.ClockOut != "") && x.Remark == "Adjust"))).ToList();

                            if (objEmpcount.Count > 1)
                            {
                                TempData["AlertMessage"] = "Please check clock out!";
                                return Json(mymodel);
                            }
                            else
                            {
                                var objEmp = db.View_ClockTime
                              .Where(x => x.EmployeeID.Equals(employeeId) &&
                                           (x.TransactionDate.Date == currentDate || x.TransactionDate.Date == currentDatebefore) &&
                                          x.PlantID.Equals(PlantID) &&
                                          (((x.ClockOut == null || x.ClockOut == "") && x.Remark == "") ||
                                             ((x.ClockOut != null || x.ClockOut != "") && x.Remark == "Adjust")))
                              .FirstOrDefault();
                                if (objEmp != null)
                                {
                                    var objPLPS = db.View_PLPS
                                    .Where(x => x.PlantID.Equals(PlantID) &&
                                                x.LineID.Equals(objEmp.LineID.ToString()) &&
                                                x.ProductID.Equals(productId) &&
                                                x.SectionID.Equals(objEmp.SectionID.ToString()))
                                    .FirstOrDefault();

                                    if (objPLPS != null)
                                    {
                                        var LastTransactionTime = db.TbProductionTransaction
                                       .Where(x => x.QRCode.Equals(objEmp.EmployeeID) && x.TransactionDate.Date == currentDate)
                                       .OrderByDescending(x => x.TransactionDate)
                                       .Select(x => x.TransactionDate.TimeOfDay)
                                       .FirstOrDefault();


                                        // Convert TimeSpan to total seconds
                                        double lastTransactionSeconds = LastTransactionTime.TotalSeconds;
                                        double delayTimeSeconds = Convert.ToDouble(objPLPS.Delaytime);

                                        if (lastTransactionSeconds + delayTimeSeconds < currentTime.TotalSeconds)
                                        {
                                            // Perform actions if LastTransactionTime is greater than or equal to objPLPS.Delaytime                  
                                            db.TbProductionTransaction.Add(new TbProductionTransaction()
                                            {
                                                // TransactionNo = db.TbProductionTransaction.Count() + 1,
                                                TransactionDate = objEmp.TransactionDate,// DateTime.Now,
                                                PlantID = Convert.ToInt32(objEmp.PlantID),
                                                LineID = objEmp.LineID,
                                                SectionID = objEmp.SectionID,
                                                ProductID = productId,
                                                QRCode = objEmp.EmployeeID,
                                                Qty = 1,
                                                QtyPerQR = Convert.ToInt32(objPLPS.QTYPerQRCode),//Get from PLPS
                                                DataType = "Count",
                                                Reason = "",
                                                Note = "",
                                                PackageRef = PackageRef,
                                                EmployeeRef = string.IsNullOrEmpty(EmployeeRef) ? "" : EmployeeRef,
                                                GroupRef = employeeId,
                                                CreateDate = DateTime.Now,
                                                CreateBy = EmpID,
                                                UpdateDate = DateTime.Now,
                                                UpdateBy = EmpID
                                            });
                                            db.SaveChanges();
                                            sectionval = objEmp.SectionID.ToString();


                                        }
                                        else
                                        {
                                            TempData["AlertMessage"] = "please check time!";
                                            var sectionvalalert = "check time";
                                            return Json(sectionvalalert);
                                        }
                                    }
                                }

                            }
                        }

                        return Json(sectionval);

                        //End case group
                    }




                }
                catch
                {
                    TempData["AlertMessage"] = "Please check master data!";
                    return Json(mymodel);
                }
            }



            return Json(mymodel);
        }


        // Backup
        //public ActionResult WorkingFunctionCreateWithRef(string employeeId, string productId, int PackageRef, String EmployeeRef)
        //{
        //    int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
        //    string EmpID = HttpContext.Session.GetString("UserEmpID");

        //    if (EmpID == null)
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }
        //    var mymodel = new ViewModelAll
        //    {
        //        view_PermissionMaster = db.View_PermissionMaster.ToList(),
        //        tbPlants = db.TbPlant.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
        //        tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
        //        tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
        //        tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
        //        view_Employee = db.View_Employee.ToList(),
        //        tbReason = db.TbReason.Where(x => x.PlantID.Equals(PlantID)).ToList()
        //        //  tbProductionTransaction = db.TbProductionTransaction.ToList()
        //    };

        //    // check QRcode in system
        //    if (db.TbEmployeeMaster.Where(x => x.EmployeeID.Equals(employeeId)) == null || db.TbEmployeeGroupQR.Where(x => x.GroupID.Equals(employeeId)) == null)
        //    {
        //        TempData["AlertMessage"] = "QR Code is not available in the system!";
        //        return Json(mymodel);
        //    }

        //    if (db.View_PermissionMaster.Where(x => x.UserEmpID.Equals(EmpID) && x.PageID.Equals(22)).Select(x => x.RoleAction).SingleOrDefault() == "Full")
        //    {
        //        var currentDateTime = DateTime.Now;
        //        var currentDate = currentDateTime.Date;
        //        var currentTime = currentDateTime.TimeOfDay;
        //        string sectionval = "";
        //        try
        //        {
        //            //check employee or Group
        //            if (db.TbEmployeeMaster.Where(x => x.EmployeeID.Equals(employeeId)).Count() != 0) // case Employee
        //            {
        //                //Select EmployeeTransaction
        //                var objEmp = db.TbEmployeeTransaction
        //                .Where(x => x.EmployeeID.Equals(employeeId) &&
        //                            x.TransactionDate.Date == currentDate &&
        //                            x.Plant.Equals(PlantID))
        //                .OrderByDescending(x => x.TransactionNo)
        //                .FirstOrDefault();
        //                if (objEmp != null)
        //                {
        //                    var objPLPS = db.View_PLPS
        //                    .Where(x => x.PlantID.Equals(PlantID) &&
        //                                x.LineID.Equals(objEmp.Line.ToString()) &&
        //                                x.ProductID.Equals(productId) &&
        //                                x.SectionID.Equals(objEmp.Section.ToString()))
        //                    .FirstOrDefault();

        //                    if (objPLPS != null)
        //                    {
        //                        var LastTransactionTime = db.TbProductionTransaction
        //                       .Where(x => x.QRCode.Equals(employeeId) && x.TransactionDate.Date == currentDate)
        //                       .OrderByDescending(x => x.TransactionDate)
        //                       .Select(x => x.TransactionDate.TimeOfDay)
        //                       .FirstOrDefault();


        //                        // Convert TimeSpan to total seconds
        //                        double lastTransactionSeconds = LastTransactionTime.TotalSeconds;
        //                        double delayTimeSeconds = Convert.ToDouble(objPLPS.Delaytime);

        //                        if (lastTransactionSeconds + delayTimeSeconds < currentTime.TotalSeconds)
        //                        {
        //                            // Perform actions if LastTransactionTime is greater than or equal to objPLPS.Delaytime                  
        //                            db.TbProductionTransaction.Add(new TbProductionTransaction()
        //                            {
        //                                // TransactionNo = db.TbProductionTransaction.Count() + 1,
        //                                TransactionDate = DateTime.Now,
        //                                PlantID = Convert.ToInt32(objEmp.Plant),
        //                                LineID = objEmp.Line,
        //                                SectionID = objEmp.Section,
        //                                ProductID = productId,
        //                                QRCode = employeeId,
        //                                Qty = 1,
        //                                QtyPerQR = Convert.ToInt32(objPLPS.QTYPerQRCode),//Get from PLPS
        //                                DataType = "Count",
        //                                Reason = "",
        //                                Note = "",
        //                                PackageRef = PackageRef,
        //                                EmployeeRef = string.IsNullOrEmpty(EmployeeRef) ? "" : EmployeeRef,
        //                                GroupRef = "",
        //                                CreateDate = DateTime.Now,
        //                                CreateBy = EmpID,
        //                                UpdateDate = DateTime.Now,
        //                                UpdateBy = EmpID
        //                            });
        //                            db.SaveChanges();
        //                            sectionval = objEmp.Section.ToString() + " : " + objPLPS.SectionName.ToString();
        //                            return Json(sectionval);

        //                        }
        //                        else
        //                        {
        //                            TempData["AlertMessage"] = "please check time!";
        //                            var sectionvalalert = "check time";
        //                            return Json(sectionvalalert);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var sectionvalalert = "Check Master PLPS";
        //                        return Json(sectionvalalert);
        //                    }
        //                }
        //                else
        //                {
        //                    var sectionvalalert = "Check Clock in time";
        //                    return Json(sectionvalalert);
        //                }
        //                //End case employee
        //            }
        //            else  // case group
        //            {

        //                //select group
        //                var objgroup = db.TbEmployeeGroupQR.Where(x => x.GroupID.Equals(employeeId)).ToList();
        //                foreach (var item in objgroup)
        //                {

        //                    //Select EmployeeTransaction
        //                    var objEmp = db.TbEmployeeTransaction
        //                    .Where(x => x.EmployeeID.Equals(item.EmployeeID) &&
        //                                x.TransactionDate.Date == currentDate &&
        //                                x.Plant.Equals(PlantID))
        //                    .OrderByDescending(x => x.TransactionNo)
        //                    .FirstOrDefault();
        //                    if (objEmp != null)
        //                    {
        //                        var objPLPS = db.View_PLPS0
        //                        .Where(x => x.PlantID.Equals(PlantID) &&
        //                                    x.LineID.Equals(objEmp.Line.ToString()) &&
        //                                    x.ProductID.Equals(productId) &&
        //                                    x.SectionID.Equals(objEmp.Section.ToString()))
        //                        .FirstOrDefault();

        //                        if (objPLPS != null)
        //                        {
        //                            var LastTransactionTime = db.TbProductionTransaction
        //                           .Where(x => x.QRCode.Equals(objEmp.EmployeeID) && x.TransactionDate.Date == currentDate)
        //                           .OrderByDescending(x => x.TransactionDate)
        //                           .Select(x => x.TransactionDate.TimeOfDay)
        //                           .FirstOrDefault();


        //                            // Convert TimeSpan to total seconds
        //                            double lastTransactionSeconds = LastTransactionTime.TotalSeconds;
        //                            double delayTimeSeconds = Convert.ToDouble(objPLPS.Delaytime);

        //                            if (lastTransactionSeconds + delayTimeSeconds < currentTime.TotalSeconds)
        //                            {
        //                                // Perform actions if LastTransactionTime is greater than or equal to objPLPS.Delaytime                  
        //                                db.TbProductionTransaction.Add(new TbProductionTransaction()
        //                                {
        //                                    // TransactionNo = db.TbProductionTransaction.Count() + 1,
        //                                    TransactionDate = DateTime.Now,
        //                                    PlantID = Convert.ToInt32(objEmp.Plant),
        //                                    LineID = objEmp.Line,
        //                                    SectionID = objEmp.Section,
        //                                    ProductID = productId,
        //                                    QRCode = objEmp.EmployeeID,
        //                                    Qty = 1,
        //                                    QtyPerQR = Convert.ToInt32(objPLPS.QTYPerQRCode),//Get from PLPS
        //                                    DataType = "Count",
        //                                    Reason = "",
        //                                    Note = "",
        //                                    PackageRef = PackageRef,
        //                                    EmployeeRef = string.IsNullOrEmpty(EmployeeRef) ? "" : EmployeeRef,
        //                                    GroupRef = employeeId,
        //                                    CreateDate = DateTime.Now,
        //                                    CreateBy = EmpID,
        //                                    UpdateDate = DateTime.Now,
        //                                    UpdateBy = EmpID
        //                                });
        //                                db.SaveChanges();
        //                                sectionval = objEmp.Section.ToString();


        //                            }
        //                            else
        //                            {
        //                                TempData["AlertMessage"] = "please check time!";
        //                                var sectionvalalert = "check time";
        //                                return Json(sectionvalalert);
        //                            }
        //                        }
        //                    }

        //                }

        //                return Json(sectionval);

        //                //End case group
        //            }




        //        }
        //        catch
        //        {
        //            TempData["AlertMessage"] = "Please check master data!";
        //            return Json(mymodel);
        //        }
        //    }



        //    return Json(mymodel);
        //}



        [HttpPost]
        public ActionResult GetSectionAndUnitWithRef(string employeeID, string productID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var currentDateTime = DateTime.Now;
            var currentDate = currentDateTime.Date;


            // Query your database or data source to retrieve section and unit based on employeeID and productID
            // For demonstration purposes, let's assume you have a method to get section and unit
            var objEmp = db.TbEmployeeTransaction
                        .Where(x => x.EmployeeID.Equals(employeeID) &&
                                    x.TransactionDate.Date == currentDate &&
                                    x.Plant.Equals(PlantID))
                        .OrderByDescending(x => x.TransactionNo)
                        .FirstOrDefault();

            var objPLPS = db.View_PLPS
                       .Where(x => x.PlantID.Equals(PlantID) &&
                                   x.LineID.Equals(objEmp.Line.ToString()) &&
                                   x.ProductID.Equals(productID) &&
                                   x.SectionID.Equals(objEmp.Section.ToString()))
                       .FirstOrDefault();



            string section = objEmp.Section;
            string unit = objPLPS.Unit;

            // Return section and unit as JSON
            return Json(new { section = section, unit = unit });
        }



        [HttpPost]
        public ActionResult GetSectionAndUnitDefectWithRef(string employeeID, string productID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var currentDateTime = DateTime.Now;
            var currentDate = currentDateTime.Date;


            // Query your database or data source to retrieve section and unit based on employeeID and productID
            // For demonstration purposes, let's assume you have a method to get section and unit
            var objEmp = db.TbEmployeeTransaction
                        .Where(x => x.EmployeeID.Equals(employeeID) &&
                                    x.TransactionDate.Date == currentDate &&
                                    x.Plant.Equals(PlantID))
                        .OrderByDescending(x => x.TransactionNo)
                        .FirstOrDefault();

            var objPLPS = db.View_PLPS
                       .Where(x => x.PlantID.Equals(PlantID) &&
                                   x.LineID.Equals(objEmp.Line.ToString()) &&
                                   x.ProductID.Equals(productID) &&
                                   x.SectionID.Equals(objEmp.Section.ToString()))
                       .FirstOrDefault();



            string section = objEmp.Section;
            string unit = objPLPS.Unit;

            // Return section and unit as JSON
            return Json(new { section = section, unit = unit });
        }



        // Function ProductQTYPiece
        public ActionResult ProductQTYPieceWithRef(TbProductionTransaction obj, string employeeID, string productID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var currentDateTime = DateTime.Now;
            var currentDate = currentDateTime.Date;
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                view_Employee = db.View_Employee.ToList(),
                tbProductionTransaction = db.TbProductionTransaction.ToList(),
                tbReason = db.TbReason.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList()
            };

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }



            try
            {


                var objEmp = db.View_ClockTime
                   .Where(x => x.EmployeeID.Equals(employeeID) &&
                               x.TransactionDate.Date == currentDate &&
                               x.PlantID.Equals(PlantID))
                   .FirstOrDefault();

                var objPLPS = db.View_PLPS
                           .Where(x => x.PlantID.Equals(PlantID) &&
                                       x.LineID.Equals(objEmp.LineID.ToString()) &&
                                       x.ProductID.Equals(productID) &&
                                       x.SectionID.Equals(objEmp.SectionID.ToString()))
                           .FirstOrDefault();




                db.TbProductionTransaction.Add(new TbProductionTransaction()
                {
                    // TransactionNo = db.TbProductionTransaction.Count() + 1,
                    TransactionDate = DateTime.Now,
                    PlantID = PlantID,
                    LineID = objEmp.LineID,
                    SectionID = objEmp.SectionID,
                    ProductID = productID,
                    Prefix = objEmp.Prefix,
                    QRCode = employeeID,
                    Qty = obj.Qty,
                    QtyPerQR = Convert.ToInt32(objPLPS.QTYPerQRCode),//Get from PLPS
                    DataType = "FG",
                    Reason = "",
                    Note = "",
                    GroupRef = "",
                    EmployeeRef ="",
                    PackageRef=0,
                    CreateDate = DateTime.Today,
                    CreateBy = EmpID,
                    UpdateDate = DateTime.Today,
                    UpdateBy = EmpID
                });
                db.SaveChanges();


                return View("WorkingFunctionWithPackage", mymodel);
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Working page had problem! please contact IT";
                return RedirectToAction("Login");
            }

        }


        public ActionResult ProductQtyDefectWithRef(TbProductionTransaction obj, string employeeID, string productID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var currentDateTime = DateTime.Now;
            var currentDate = currentDateTime.Date;
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                tbProductionTransaction = db.TbProductionTransaction.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbReason = db.TbReason.Where(x => x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList()
            };

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {

                var objEmp = db.View_ClockTime
                   .Where(x => x.EmployeeID.Equals(employeeID) &&
                               x.TransactionDate.Date == currentDate &&
                               x.PlantID.Equals(PlantID))
                   .FirstOrDefault();

                var objPLPS = db.View_PLPS
                           .Where(x => x.PlantID.Equals(PlantID) &&
                                       x.LineID.Equals(objEmp.LineID.ToString()) &&
                                       x.ProductID.Equals(productID) &&
                                       x.SectionID.Equals(objEmp.SectionID.ToString()))
                           .FirstOrDefault();





                db.TbProductionTransaction.Add(new TbProductionTransaction()
                {
                    // TransactionNo = db.TbProductionTransaction.Count() + 1,
                    TransactionDate = DateTime.Now,
                    PlantID = PlantID,
                    LineID = objEmp.LineID,
                    SectionID = objEmp.SectionID,
                    ProductID = productID,
                    Prefix = objEmp.Prefix,
                    QRCode = employeeID,
                    Qty = obj.Qty,
                    QtyPerQR = Convert.ToInt32(objPLPS.QTYPerQRCode),//Get from PLPS
                    DataType = "Defect",
                    Reason = obj.Reason,
                    Note = "",
                    GroupRef = "",
                    EmployeeRef = "",
                    PackageRef = 0,
                    CreateDate = DateTime.Today,
                    CreateBy = EmpID,
                    UpdateDate = DateTime.Today,
                    UpdateBy = EmpID
                });
                db.SaveChanges();


                return View("WorkingFunctionWithPackage", mymodel);
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Working page had problem! please contact IT";
                return RedirectToAction("Login");
            }

        }



        //DateTime startDate, DateTime endDate,
        [HttpGet]
        public async Task<IActionResult> FinancialReport(string EmployeeID, DateTime StartDate, DateTime EndDate, string LineID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            // Check if user is logged in
            if (string.IsNullOrEmpty(EmpID))
            {
                return RedirectToAction("Login", "Home");
            }


                // Generate date range
                List < DateTime > dateRange = Enumerable.Range(0, 1 + EndDate.Subtract(StartDate).Days)
                                      .Select(offset => StartDate.AddDays(offset))
                                      .ToList();

            // Pass date range along with other data to the view
            ViewBag.DateRange = dateRange;


            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID == PlantID).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_FinancialReport = db.View_FinancialReport.Where(x => x.PlantID == PlantID).ToList()
                
            };

            ViewBag.VBRoleFinancial = mymodel.view_PermissionMaster
                                            .Where(x => x.UserEmpID == EmpID && x.PageID == 24)
                                            .Select(x => x.RoleAction)
                                            .FirstOrDefault();


            if (!string.IsNullOrEmpty(EmployeeID) || !string.IsNullOrEmpty(LineID) || StartDate != DateTime.MinValue || EndDate != DateTime.MinValue)
            {


                if (!string.IsNullOrEmpty(EmployeeID))
                {
                    mymodel.view_FinancialReport = mymodel.view_FinancialReport.Where(x => x.QRCode == EmployeeID).ToList();
                    ViewBag.SelectedEmpID = EmployeeID;
                }

                if (!string.IsNullOrEmpty(LineID))
                {
                    mymodel.view_FinancialReport = mymodel.view_FinancialReport.Where(x => x.LineID == LineID).ToList();
                    ViewBag.SelectedLineID = LineID;
                }


                if (StartDate != DateTime.MinValue && EndDate != DateTime.MinValue)
                {
                    mymodel.view_FinancialReport = mymodel.view_FinancialReport
                       .Where(x => x.TransactionDate >= StartDate && x.TransactionDate <= EndDate)
                       .ToList();


                    ViewBag.SelectedStartDate  = StartDate.ToString("yyyy-MM-dd");
                    ViewBag.SelectedEndDate = EndDate.ToString("yyyy-MM-dd"); 

                }
                //else if (StartDate != DateTime.MinValue)
                //{
                //    mymodel.view_FinancialReport = mymodel.view_FinancialReport
                //         .Where(x => x.TransactionDate >= StartDate)
                //         .ToList();

                //    ViewBag.SelectedStartDate = StartDate;
                //}
                //else if (EndDate != DateTime.MinValue)
                //{
                //    mymodel.view_FinancialReport = mymodel.view_FinancialReport
                //     .Where(x => x.TransactionDate <= EndDate)
                //     .ToList();

                //    ViewBag.SelectedEndDate = EndDate;
                //}

                var groupedData = mymodel.view_FinancialReport.GroupBy(x => new { x.TransactionDate.Date,x.LineID, x.QRCode,x.SectionID})
                   .Select(g => new GroupedFinancialData // Use the correct model type here
                   {
                       TransactionDate = g.Key.Date,
                       QRCode = g.Key.QRCode,
                       EmployeeName = g.Max(x => x.EmployeeName),
                       TotalIncentive = g.Sum(x => x.Incentive),
                       SectionName = g.Max(x=>x.SectionName)
                   })
                   .ToList<GroupedFinancialData>(); // Specify the type explicitly


                mymodel = new ViewModelAll
                {
                    tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID == PlantID).ToList(),
                    tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                    tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                    view_PermissionMaster = db.View_PermissionMaster.ToList(),
                    view_FinancialReport = db.View_FinancialReport.Where(x => x.PlantID == PlantID).ToList(),
                    groupedData = groupedData
                    .OrderBy(x => x.QRCode) // First order by QRCode
                    .ThenBy(x => x.SectionName) // First order by QRCode
                    .ThenBy(x => x.TransactionDate) // Then order by TransactionDate
                    .ToList()

                };

                
              //  return View(groupedData);
               return View(mymodel);

            }
            else
            {
                var groupedData = mymodel.view_FinancialReport.GroupBy(x => new { x.TransactionDate.Date, x.QRCode })
               .Select(g => new
               {
                   TransactionDate = g.Key.Date,
                   QRCode = g.Key.QRCode,
                   TotalIncentive = g.Sum(x => x.Incentive)
               })
               .ToList();
              
                mymodel.view_FinancialReport = db.View_FinancialReport.Where(x => x.TransactionDate.Equals(DateTime.Today) && x.PlantID.Equals(PlantID)).ToList();
                ViewBag.SelectedStartDate = DateTime.Today.ToString("yyyy-MM-dd");
                ViewBag.SelectedEndDate = DateTime.Today.ToString("yyyy-MM-dd");
                return View(mymodel);
            }
           
        }







        public ActionResult FinanceReportExport(string EmployeeID, DateTime StartDate, DateTime EndDate, string LineID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }


            try
            {

                var mymodel = new ViewModelAll
                {
                    tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID == PlantID).ToList(),
                    tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                    tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                    view_PermissionMaster = db.View_PermissionMaster.ToList(),
                    view_FinancialReport = db.View_FinancialReport.Where(x => x.PlantID == PlantID).ToList()

                };

                // Generate date range
                List<DateTime> dateRange = Enumerable.Range(0, 1 + EndDate.Subtract(StartDate).Days)
                                      .Select(offset => StartDate.AddDays(offset))
                                      .ToList();

               
             


                ViewBag.VBRoleFinancial = mymodel.view_PermissionMaster
                                            .Where(x => x.UserEmpID == EmpID && x.PageID == 24)
                                            .Select(x => x.RoleAction)
                                            .FirstOrDefault();




                if (!string.IsNullOrEmpty(EmployeeID) || !string.IsNullOrEmpty(LineID)  || StartDate != DateTime.MinValue || EndDate != DateTime.MinValue)
                {

                    if (!string.IsNullOrEmpty(EmployeeID))
                    {
                        mymodel.view_FinancialReport = mymodel.view_FinancialReport.Where(x => x.QRCode == EmployeeID).ToList();
                        ViewBag.SelectedEmpID = EmployeeID;
                    }

                    if (!string.IsNullOrEmpty(LineID))
                    {
                        mymodel.view_FinancialReport = mymodel.view_FinancialReport.Where(x => x.LineID == LineID).ToList();
                        ViewBag.SelectedLineID = LineID;
                    }


                    if (StartDate != DateTime.MinValue && EndDate != DateTime.MinValue)
                    {
                        mymodel.view_FinancialReport = mymodel.view_FinancialReport
                           .Where(x => x.TransactionDate >= StartDate && x.TransactionDate <= EndDate)
                           .ToList();


                        ViewBag.SelectedStartDate = StartDate.ToString("yyyy-MM-dd");
                        ViewBag.SelectedEndDate = EndDate.ToString("yyyy-MM-dd");

                    }

                    var groupedData = mymodel.view_FinancialReport.GroupBy(x => new { x.TransactionDate.Date, x.QRCode, x.SectionID })
                 .Select(g => new GroupedFinancialData // Use the correct model type here
                 {
                     TransactionDate = g.Key.Date,
                     QRCode = g.Key.QRCode,
                     EmployeeName = g.Max(x => x.EmployeeName),
                     TotalIncentive = g.Sum(x => x.Incentive),
                     SectionName = g.Max(x => x.SectionName)
                 })
                 .ToList<GroupedFinancialData>(); // Specify the type explicitly


                    mymodel = new ViewModelAll
                    {
                        tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID == PlantID).ToList(),
                        tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                        tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                        view_PermissionMaster = db.View_PermissionMaster.ToList(),
                        view_FinancialReport = db.View_FinancialReport.Where(x => x.PlantID == PlantID).ToList(),
                        groupedData = groupedData
                       .OrderBy(x => x.QRCode) // First order by QRCode
                       .ThenBy(x => x.SectionName) // First order by QRCode
                       .ThenBy(x => x.TransactionDate) // Then order by TransactionDate
                       .ToList()

                    };



                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Employee Data");

                        // Set header
                        worksheet.Cells[1, 1].Merge = true;
                        worksheet.Cells[1, 2].Merge = true;

                        worksheet.Cells[2, 1].Value = "Employee ID";
                        worksheet.Cells[2, 2].Value = "Employee Name";

                        worksheet.Cells[2,1].Style.Font.Bold = true;
                        worksheet.Cells[2, 2].Style.Font.Bold = true;


                        int dateStartColumn = 3;
                        decimal[] sumByDate = new decimal[dateRange.Count];

                        for (int i = 0; i < dateRange.Count; i++)
                        {
                            worksheet.Cells[2, dateStartColumn + i].Value = dateRange[i].ToString("dd/MM/yy");
                            worksheet.Cells[2, dateStartColumn + i].Style.Font.Bold = true;
                        }

                        // Merge the date header columns
                        worksheet.Cells[1, dateStartColumn, 1, dateStartColumn + dateRange.Count - 1].Merge = true;
                        worksheet.Cells[1, dateStartColumn].Value = "สิทธิเงินพิเศษ";
                        worksheet.Cells[1, dateStartColumn ].Style.Font.Bold = true;


                        worksheet.Cells[2, dateStartColumn + dateRange.Count].Value = "จำนวนเงิน";
                        worksheet.Cells[2, dateStartColumn + dateRange.Count + 1].Value = "จุดงาน";
                        worksheet.Cells[1, dateStartColumn + dateRange.Count].Merge = true;
                        worksheet.Cells[1, dateStartColumn + dateRange.Count + 1].Merge = true;
                        worksheet.Cells[2, dateStartColumn + dateRange.Count].Style.Font.Bold = true;
                        worksheet.Cells[2, dateStartColumn + dateRange.Count +1].Style.Font.Bold = true;

                        // Set data
                        if (groupedData != null && groupedData.Any())
                        {
                            var groupedByEmployeeAndSection = mymodel.groupedData.GroupBy(x => new { x.QRCode, x.SectionName });
                            int row = 3;

                            foreach (var group in groupedByEmployeeAndSection)
                            {
                                worksheet.Cells[row, 1].Value = group.Key.QRCode;
                                worksheet.Cells[row, 2].Value = group.Max(x => x.EmployeeName);

                                decimal totalIncentive = 0;
                                //decimal totalIncentivedate = 0;

                                for (int i = 0; i < dateRange.Count; i++)
                                {
                                    var date = dateRange[i];
                                    var matchingTransaction = group.FirstOrDefault(x => x.TransactionDate.Date == date.Date);
                                    decimal incentive = matchingTransaction != null ? matchingTransaction.TotalIncentive : 0;
                                    worksheet.Cells[row, dateStartColumn + i].Value = matchingTransaction != null ? matchingTransaction.TotalIncentive.ToString("0.00") : "";
                                    
                                    totalIncentive += matchingTransaction != null ? matchingTransaction.TotalIncentive : 0;
                                   // totalIncentivedate += incentive;
                                    sumByDate[i] += incentive;
                                }

                                worksheet.Cells[row, dateStartColumn + dateRange.Count].Value = totalIncentive.ToString("0.00");
                                worksheet.Cells[row, dateStartColumn + dateRange.Count + 1].Value = group.Key.SectionName;


                                row++;
                            }
                            worksheet.Cells[row, 2].Value = "Total";
                            worksheet.Cells[row, 2].Style.Font.Bold = true;

                            for (int i = 0; i < dateRange.Count; i++)
                            {
                                worksheet.Cells[row, dateStartColumn + i].Value = sumByDate[i].ToString("0.00");
                                worksheet.Cells[row, dateStartColumn + i].Style.Font.Bold = true;
                            }
                            worksheet.Cells[row, dateStartColumn + dateRange.Count].Formula = $"SUM({worksheet.Cells[3, dateStartColumn + dateRange.Count].Address}:{worksheet.Cells[row - 1, dateStartColumn + dateRange.Count].Address})";
                            worksheet.Cells[row, dateStartColumn + dateRange.Count].Style.Font.Bold = true;


                        }
                        else
                        {
                            worksheet.Cells[2, 1].Value = "No data available.";
                        }



                        // Auto fit columns
                        worksheet.Cells.AutoFitColumns();

                        var stream = new MemoryStream();
                        package.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FinanceReport.xlsx");
                    }


                    // mymodel.tbLine = db.TbLine.Where(p => p.LineName.Equals(obj.LineName) || p.LineID.Equals(obj.LineID)).OrderByDescending(x => x.Status);
                   // return View("DailyReport", mymodel);
                }
                else
                {

                    var groupedData = mymodel.view_FinancialReport.GroupBy(x => new { x.TransactionDate.Date, x.QRCode, x.SectionID })
                 .Select(g => new GroupedFinancialData // Use the correct model type here
                 {
                     TransactionDate = g.Key.Date,
                     QRCode = g.Key.QRCode,
                     EmployeeName = g.Max(x => x.EmployeeName),
                     TotalIncentive = g.Sum(x => x.Incentive),
                     SectionName = g.Max(x => x.SectionName)
                 })
                 .ToList<GroupedFinancialData>(); // Specify the type explicitly


                    mymodel = new ViewModelAll
                    {
                        tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID == PlantID).ToList(),
                        tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                        tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                        view_PermissionMaster = db.View_PermissionMaster.ToList(),
                        view_FinancialReport = db.View_FinancialReport.Where(x => x.PlantID == PlantID).ToList(),
                        groupedData = groupedData
                       .OrderBy(x => x.QRCode) // First order by QRCode
                       .ThenBy(x => x.SectionName) // First order by QRCode
                       .ThenBy(x => x.TransactionDate) // Then order by TransactionDate
                       .ToList()

                    };


                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Employee Data");

                        // Set header
                        worksheet.Cells[1, 1].Merge = true;
                        worksheet.Cells[1, 2].Merge = true;

                        worksheet.Cells[2, 1].Value = "Employee ID";
                        worksheet.Cells[2, 2].Value = "Employee Name";

                        worksheet.Cells[2, 1].Style.Font.Bold = true;
                        worksheet.Cells[2, 2].Style.Font.Bold = true;


                        int dateStartColumn = 3;
                        decimal[] sumByDate = new decimal[dateRange.Count];

                        for (int i = 0; i < dateRange.Count; i++)
                        {
                            worksheet.Cells[2, dateStartColumn + i].Value = dateRange[i].ToString("dd/MM/yy");
                            worksheet.Cells[2, dateStartColumn + i].Style.Font.Bold = true;
                        }

                        // Merge the date header columns
                        worksheet.Cells[1, dateStartColumn, 1, dateStartColumn + dateRange.Count - 1].Merge = true;
                        worksheet.Cells[1, dateStartColumn].Value = "สิทธิเงินพิเศษ";
                        worksheet.Cells[1, dateStartColumn].Style.Font.Bold = true;


                        worksheet.Cells[2, dateStartColumn + dateRange.Count].Value = "จำนวนเงิน";
                        worksheet.Cells[2, dateStartColumn + dateRange.Count + 1].Value = "จุดงาน";
                        worksheet.Cells[1, dateStartColumn + dateRange.Count].Merge = true;
                        worksheet.Cells[1, dateStartColumn + dateRange.Count + 1].Merge = true;
                        worksheet.Cells[2, dateStartColumn + dateRange.Count].Style.Font.Bold = true;
                        worksheet.Cells[2, dateStartColumn + dateRange.Count + 1].Style.Font.Bold = true;

                        // Set data
                        if (groupedData != null && groupedData.Any())
                        {
                            var groupedByEmployeeAndSection = mymodel.groupedData.GroupBy(x => new { x.QRCode, x.SectionName });
                            int row = 3;

                            foreach (var group in groupedByEmployeeAndSection)
                            {
                                worksheet.Cells[row, 1].Value = group.Key.QRCode;
                                worksheet.Cells[row, 2].Value = group.Max(x => x.EmployeeName);

                                decimal totalIncentive = 0;
                                //decimal totalIncentivedate = 0;

                                for (int i = 0; i < dateRange.Count; i++)
                                {
                                    var date = dateRange[i];
                                    var matchingTransaction = group.FirstOrDefault(x => x.TransactionDate.Date == date.Date);
                                    decimal incentive = matchingTransaction != null ? matchingTransaction.TotalIncentive : 0;
                                    worksheet.Cells[row, dateStartColumn + i].Value = matchingTransaction != null ? matchingTransaction.TotalIncentive.ToString("0.00") : "";

                                    totalIncentive += matchingTransaction != null ? matchingTransaction.TotalIncentive : 0;
                                    // totalIncentivedate += incentive;
                                    sumByDate[i] += incentive;
                                }

                                worksheet.Cells[row, dateStartColumn + dateRange.Count].Value = totalIncentive.ToString("0.00");
                                worksheet.Cells[row, dateStartColumn + dateRange.Count + 1].Value = group.Key.SectionName;


                                row++;
                            }
                            worksheet.Cells[row, 2].Value = "Total";
                            worksheet.Cells[row, 2].Style.Font.Bold = true;

                            for (int i = 0; i < dateRange.Count; i++)
                            {
                                worksheet.Cells[row, dateStartColumn + i].Value = sumByDate[i].ToString("0.00");
                                worksheet.Cells[row, dateStartColumn + i].Style.Font.Bold = true;
                            }
                            worksheet.Cells[row, dateStartColumn + dateRange.Count].Formula = $"SUM({worksheet.Cells[3, dateStartColumn + dateRange.Count].Address}:{worksheet.Cells[row - 1, dateStartColumn + dateRange.Count].Address})";
                            worksheet.Cells[row, dateStartColumn + dateRange.Count].Style.Font.Bold = true;


                        }
                        else
                        {
                            worksheet.Cells[2, 1].Value = "No data available.";
                        }


                        // Auto fit columns
                        worksheet.Cells.AutoFitColumns();

                        var stream = new MemoryStream();
                        package.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FinanceReport.xlsx");

                    }



                }
            }
            catch
            {
                TempData["AlertMessage"] = "System Some has Problem in Line, Plese contact IT!";
                return RedirectToAction("Login");

            }


        }


        public ActionResult FinancialReportClear(string EmployeeID, DateTime StartDate, DateTime EndDate, string LineID)
        { 
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID == PlantID).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_FinancialReport = db.View_FinancialReport.Where(x => x.PlantID == PlantID).ToList()
            };


            // Generate date range
            List<DateTime> dateRange = Enumerable.Range(0, 1 + EndDate.Subtract(StartDate).Days)
                                  .Select(offset => StartDate.AddDays(offset))
                                  .ToList();

            // Pass date range along with other data to the view
            ViewBag.DateRange = dateRange;


            mymodel.view_FinancialReport = db.View_FinancialReport.Where(x => x.TransactionDate.Equals(DateTime.Today) && x.PlantID.Equals(PlantID)).ToList();
            return View("FinancialReport", mymodel);
            

        }



        //DateTime startDate, DateTime endDate,
        [HttpGet]
        public async Task<IActionResult> EFFReport(string EmployeeID, DateTime StartDate, DateTime EndDate, string LineID,String SectionName)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            // Check if user is logged in
            if (string.IsNullOrEmpty(EmpID))
            {
                return RedirectToAction("Login", "Home");
            }


            // Generate date range
            List<DateTime> dateRange = Enumerable.Range(0, 1 + EndDate.Subtract(StartDate).Days)
                                  .Select(offset => StartDate.AddDays(offset))
                                  .ToList();

            // Pass date range along with other data to the view
            ViewBag.DateRange = dateRange;


            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID == PlantID).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_FinancialReport = db.View_FinancialReport.Where(x => x.PlantID == PlantID).ToList(),
                view_EFFReport = db.View_EFFReport.Where(x=>x.PlantID == PlantID).ToList()
            };

            ViewBag.VBRoleEfficiency = mymodel.view_PermissionMaster
                                            .Where(x => x.UserEmpID == EmpID && x.PageID == 25)
                                            .Select(x => x.RoleAction)
                                            .FirstOrDefault();


            if (!string.IsNullOrEmpty(EmployeeID) || !string.IsNullOrEmpty(LineID) || !string.IsNullOrEmpty(SectionName)  || StartDate != DateTime.MinValue || EndDate != DateTime.MinValue)
            {


                if (!string.IsNullOrEmpty(LineID))
                {
                    mymodel.view_EFFReport = mymodel.view_EFFReport.Where(x => x.LineID == LineID).ToList();
                    ViewBag.SelectedLineID = LineID;
                }

                if (!string.IsNullOrEmpty(SectionName))
                {
                    mymodel.view_EFFReport = mymodel.view_EFFReport.Where(x => x.SectionID == SectionName).ToList();
                    ViewBag.SelectedSectionName = SectionName;
                }

                if (StartDate != DateTime.MinValue && EndDate != DateTime.MinValue)
                {
                    mymodel.view_EFFReport = mymodel.view_EFFReport
                       .Where(x => x.TransactionDate >= StartDate && x.TransactionDate <= EndDate)
                       .ToList();


                    ViewBag.SelectedStartDate = StartDate.ToString("yyyy-MM-dd");
                    ViewBag.SelectedEndDate = EndDate.ToString("yyyy-MM-dd");

                }
                return View(mymodel);

            }
            else
            {

                mymodel.view_EFFReport = db.View_EFFReport.Where(x => x.TransactionDate.Equals(DateTime.Today) && x.PlantID.Equals(PlantID)).ToList();
                return View(mymodel);
            }

        }

        public ActionResult EFFReportClear(string EmployeeID, DateTime StartDate, DateTime EndDate, string LineID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            // Check if user is logged in
            if (string.IsNullOrEmpty(EmpID))
            {
                return RedirectToAction("Login", "Home");
            }



            var mymodel = new ViewModelAll
            {
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID == PlantID).ToList(),
                tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_FinancialReport = db.View_FinancialReport.Where(x => x.PlantID == PlantID).ToList(),
                view_EFFReport = db.View_EFFReport.Where(x => x.PlantID == PlantID).ToList()
            };

            ViewBag.VBRoleEfficiency = mymodel.view_PermissionMaster
                                            .Where(x => x.UserEmpID == EmpID && x.PageID == 25)
                                            .Select(x => x.RoleAction)
                                            .FirstOrDefault();


         
                mymodel.view_EFFReport = db.View_EFFReport.Where(x => x.TransactionDate.Equals(DateTime.Today) && x.PlantID.Equals(PlantID)).ToList();
                return View(mymodel);
            

        }

        public ActionResult EFFReportExport(string EmployeeID, DateTime StartDate, DateTime EndDate, string LineID, String SectionName)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }


            try
            {

                var mymodel = new ViewModelAll
                {
                    tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID == PlantID).ToList(),
                    tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                    tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                    view_PermissionMaster = db.View_PermissionMaster.ToList(),
                    view_FinancialReport = db.View_FinancialReport.Where(x => x.PlantID == PlantID).ToList(),
                    view_EFFReport = db.View_EFFReport.Where(x => x.PlantID == PlantID).ToList()

                };

               

                ViewBag.VBRoleEfficiency = mymodel.view_PermissionMaster
                                            .Where(x => x.UserEmpID == EmpID && x.PageID == 25)
                                            .Select(x => x.RoleAction)
                                            .FirstOrDefault();


                if (!string.IsNullOrEmpty(EmployeeID) || !string.IsNullOrEmpty(LineID) || !string.IsNullOrEmpty(SectionName) || StartDate != DateTime.MinValue || EndDate != DateTime.MinValue)
                {

                    if (!string.IsNullOrEmpty(LineID))
                    {
                        mymodel.view_EFFReport = mymodel.view_EFFReport.Where(x => x.LineID == LineID).ToList();
                        ViewBag.SelectedLineID = LineID;
                    }

                    if (!string.IsNullOrEmpty(SectionName))
                    {
                        mymodel.view_EFFReport = mymodel.view_EFFReport.Where(x => x.SectionID == SectionName).ToList();
                        ViewBag.SelectedSectionName = SectionName;
                    }

                    if (StartDate != DateTime.MinValue && EndDate != DateTime.MinValue)
                    {
                        mymodel.view_EFFReport = mymodel.view_EFFReport
                           .Where(x => x.TransactionDate >= StartDate && x.TransactionDate <= EndDate)
                           .ToList();


                        ViewBag.SelectedStartDate = StartDate.ToString("yyyy-MM-dd");
                        ViewBag.SelectedEndDate = EndDate.ToString("yyyy-MM-dd");

                    }


                    var collection = mymodel.view_EFFReport.ToList();
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("EFF Report");

                    worksheet.Cells[1, 1].Value = "Line";
                    worksheet.Cells[1, 2].Value = "Section";
                    worksheet.Cells[1, 3].Value = "ProductID";

                    worksheet.Cells[1, 4].Value = "ProductName";
                    worksheet.Cells[1, 5].Value = "Unit";
                    worksheet.Cells[1, 6].Value = "EFF-STD";

                        worksheet.Cells[1, 7].Value = "ชม. งาน STD";
                        worksheet.Cells[1, 8].Value = "ชม. งาน  ACT";
                        worksheet.Cells[1, 9].Value = "ชิ้นรับเข้า";

                        worksheet.Cells[1, 10].Value = "ชั่วโมงจริง";  // EFF1
                        worksheet.Cells[1, 11].Value = "บริการแยกได้";
                        worksheet.Cells[1, 12].Value = "บริการแยกไม่ได้";

                        worksheet.Cells[1, 13].Value = "ชม.จริง+บริการแยกได้"; //EFF2
                        worksheet.Cells[1, 14].Value = "ชม.จริง+บริการแยกได้+บริการแยกไม่ได้"; //EFF3
                        worksheet.Cells[1, 15].Value = "EFF ชม.1"; 

                        worksheet.Cells[1, 16].Value = "EFF ชม.2";
                        worksheet.Cells[1, 17].Value = "EFF ชม.3";
                        worksheet.Cells[1, 18].Value = "KPI อัตราส่วน";

                        worksheet.Cells[1, 19].Value = "ค่ากลาง ชม.3";
                        worksheet.Cells[1, 20].Value = "ค่าที่ได้";
                        worksheet.Cells[1, 21].Value = "KPI อัตราส่วน";

                        worksheet.Cells[1, 22].Value = "ค่ากลาง ชม.1";
                        worksheet.Cells[1, 23].Value = "ค่าที่ได้";
                       

                        for (int i = 1; i < 24; i++)
                        {
                            worksheet.Cells[1, i].Style.Font.Bold = true;
                        }

                            int row = 2;
                        decimal sumWorkinghourSTD = 0;
                        decimal sumWorkinghourACT = 0;
                        decimal sumFinishGood = 0;
                        decimal sumEFF1 = 0;
                        decimal  sumServicehour = 0;
                        decimal sumSupporthour = 0;
                        decimal sumEFF2 = 0;
                        decimal sumEFF3 = 0;
                        decimal sumEFFhr1 = 0;
                        decimal sumEFFhr2 = 0;
                        decimal sumEFFhr3 = 0;
                        decimal sumKPIh3 = 0;
                        decimal sumMEDh3 = 0;
                        decimal sumValEffh3 = 0;
                        decimal sumKPIh1 = 0;
                        decimal sumMEDh1 = 0;
                        decimal sumValEffh1 = 0;
                        foreach (var item in collection)
                    {

                            worksheet.Cells[row,1].Value = item.LineName;
                            worksheet.Cells[row, 2].Value = item.SectionName;
                            worksheet.Cells[row, 3].Value = item.ProductID;

                            worksheet.Cells[row, 4].Value = item.ProductName;
                            worksheet.Cells[row, 5].Value = item.Unit;
                            worksheet.Cells[row, 6].Value = item.ProductSTD;

                            worksheet.Cells[row, 7].Value = item.WorkinghourSTD;
                            sumWorkinghourSTD += item.WorkinghourSTD;

                            worksheet.Cells[row, 8].Value = item.WorkinghourACT;
                            sumWorkinghourACT += item.WorkinghourACT;

                            worksheet.Cells[row, 9].Value = item.FinishGood;
                            sumFinishGood += item.FinishGood;

                            worksheet.Cells[row, 10].Value = item.EFF1;
                            sumEFF1 += item.EFF1;

                            worksheet.Cells[row, 11].Value = item.Servicehour;
                            sumServicehour += item.Servicehour;

                            worksheet.Cells[row, 12].Value = item.Supporthour;
                            sumSupporthour += item.Supporthour;

                            worksheet.Cells[row, 13].Value = item.EFF2;
                            sumEFF2 += item.EFF2;

                            worksheet.Cells[row, 14].Value = item.EFF3;
                            sumEFF3 += item.EFF3;

                            worksheet.Cells[row, 15].Value = item.EFFhr1;
                            sumEFFhr1 += item.EFFhr1;

                            worksheet.Cells[row, 16].Value = item.EFFhr2;
                            sumEFFhr2 += item.EFFhr2;

                            worksheet.Cells[row, 17].Value = item.EFFhr3;
                            sumEFFhr3 += item.EFFhr3;

                            worksheet.Cells[row, 18].Value = item.KPIh3;
                            sumKPIh3 += item.KPIh3;

                            worksheet.Cells[row, 19].Value = item.MEDh3;
                            sumMEDh3 += item.MEDh3;

                            worksheet.Cells[row, 20].Value = item.ValEffh3;
                            sumValEffh3 += item.ValEffh3;

                            worksheet.Cells[row, 21].Value = item.KPIh1;
                            sumKPIh1 += item.KPIh1;

                            worksheet.Cells[row, 22].Value = item.MEDh1;
                            sumMEDh1 += item.MEDh1;

                            worksheet.Cells[row, 23].Value = item.ValEffh1;
                            sumValEffh1 += item.ValEffh1;
                            row++;
                    }

                        worksheet.Cells[row, 6].Value = "Total";
                        worksheet.Cells[row, 7].Value = sumWorkinghourSTD;
                        worksheet.Cells[row, 8].Value = sumWorkinghourACT;
                        worksheet.Cells[row, 9].Value = sumFinishGood;
                        worksheet.Cells[row, 10].Value = sumEFF1;
                        worksheet.Cells[row, 11].Value = sumServicehour;
                        worksheet.Cells[row, 12].Value = sumSupporthour;
                        worksheet.Cells[row, 13].Value = sumEFF2;
                        worksheet.Cells[row, 14].Value = sumEFF3;
                        worksheet.Cells[row, 15].Value = sumEFFhr1;
                        worksheet.Cells[row, 16].Value = sumEFFhr2;
                        worksheet.Cells[row, 17].Value = sumEFFhr3;
                        worksheet.Cells[row, 18].Value = sumKPIh3;
                        worksheet.Cells[row, 19].Value = sumMEDh3;
                        worksheet.Cells[row, 20].Value = sumValEffh3;
                        worksheet.Cells[row, 21].Value = sumKPIh1;
                        worksheet.Cells[row, 22].Value = sumMEDh1;
                        worksheet.Cells[row, 23].Value = sumValEffh1;

                        for (int i = 6; i < 24; i++)
                        {
                            worksheet.Cells[row, i].Style.Font.Bold = true;
                        }




                        // Auto fit columns
                        worksheet.Cells.AutoFitColumns();

                        var stream = new MemoryStream();
                        package.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EFFReport.xlsx");

                    }
                }

                else
                {

                    var collection = mymodel.view_EFFReport.ToList();
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("EFF Report");

                        worksheet.Cells[1, 1].Value = "Line";
                        worksheet.Cells[1, 2].Value = "Section";
                        worksheet.Cells[1, 3].Value = "ProductID";

                        worksheet.Cells[1, 4].Value = "ProductName";
                        worksheet.Cells[1, 5].Value = "Unit";
                        worksheet.Cells[1, 6].Value = "EFF-STD";

                        worksheet.Cells[1, 7].Value = "ชม. งาน STD";
                        worksheet.Cells[1, 8].Value = "ชม. งาน  ACT";
                        worksheet.Cells[1, 9].Value = "ชิ้นรับเข้า";

                        worksheet.Cells[1, 10].Value = "ชั่วโมงจริง";  // EFF1
                        worksheet.Cells[1, 11].Value = "บริการแยกได้";
                        worksheet.Cells[1, 12].Value = "บริการแยกไม่ได้";

                        worksheet.Cells[1, 13].Value = "ชม.จริง+บริการแยกได้"; //EFF2
                        worksheet.Cells[1, 14].Value = "ชม.จริง+บริการแยกได้+บริการแยกไม่ได้"; //EFF3
                        worksheet.Cells[1, 15].Value = "EFF ชม.1";

                        worksheet.Cells[1, 16].Value = "EFF ชม.2";
                        worksheet.Cells[1, 17].Value = "EFF ชม.3";
                        worksheet.Cells[1, 18].Value = "KPI อัตราส่วน";

                        worksheet.Cells[1, 19].Value = "ค่ากลาง ชม.3";
                        worksheet.Cells[1, 20].Value = "ค่าที่ได้";
                        worksheet.Cells[1, 21].Value = "KPI อัตราส่วน";

                        worksheet.Cells[1, 22].Value = "ค่ากลาง ชม.1";
                        worksheet.Cells[1, 23].Value = "ค่าที่ได้";

                        for (int i = 1; i < 24; i++)
                        {
                            worksheet.Cells[1, i].Style.Font.Bold = true;
                        }


                        int row = 2;
                        decimal sumWorkinghourSTD = 0;
                        decimal sumWorkinghourACT = 0;
                        decimal sumFinishGood = 0;
                        decimal sumEFF1 = 0;
                        decimal sumServicehour = 0;
                        decimal sumSupporthour = 0;
                        decimal sumEFF2 = 0;
                        decimal sumEFF3 = 0;
                        decimal sumEFFhr1 = 0;
                        decimal sumEFFhr2 = 0;
                        decimal sumEFFhr3 = 0;
                        decimal sumKPIh3 = 0;
                        decimal sumMEDh3 = 0;
                        decimal sumValEffh3 = 0;
                        decimal sumKPIh1 = 0;
                        decimal sumMEDh1 = 0;
                        decimal sumValEffh1 = 0;

                        foreach (var item in collection)
                        {

                            worksheet.Cells[row, 1].Value = item.LineName;
                            worksheet.Cells[row, 2].Value = item.SectionName;
                            worksheet.Cells[row, 3].Value = item.ProductID;

                            worksheet.Cells[row, 4].Value = item.ProductName;
                            worksheet.Cells[row, 5].Value = item.Unit;
                            worksheet.Cells[row, 6].Value = item.ProductSTD;

                            worksheet.Cells[row, 7].Value = item.WorkinghourSTD;
                            worksheet.Cells[row, 8].Value = item.WorkinghourACT;
                            worksheet.Cells[row, 9].Value = item.FinishGood;

                            worksheet.Cells[row, 10].Value = item.EFF1;
                            worksheet.Cells[row, 11].Value = item.Servicehour;
                            worksheet.Cells[row, 12].Value = item.Supporthour;

                            worksheet.Cells[row, 13].Value = item.EFF2;
                            worksheet.Cells[row, 14].Value = item.EFF3;
                            worksheet.Cells[row, 15].Value = item.EFFhr1;

                            worksheet.Cells[row, 16].Value = item.EFFhr2;
                            worksheet.Cells[row, 17].Value = item.EFFhr3;
                            worksheet.Cells[row, 18].Value = item.KPIh3;


                            worksheet.Cells[row, 19].Value = item.MEDh3;
                            worksheet.Cells[row, 20].Value = item.ValEffh3;
                            worksheet.Cells[row, 21].Value = item.KPIh1;

                            worksheet.Cells[row, 22].Value = item.MEDh1;
                            worksheet.Cells[row, 23].Value = item.ValEffh1;

                            row++;
                        }

                        worksheet.Cells[row, 6].Value = "Total";
                        worksheet.Cells[row, 7].Value = sumWorkinghourSTD;
                        worksheet.Cells[row, 8].Value = sumWorkinghourACT;
                        worksheet.Cells[row, 9].Value = sumFinishGood;
                        worksheet.Cells[row, 10].Value = sumEFF1;
                        worksheet.Cells[row, 11].Value = sumServicehour;
                        worksheet.Cells[row, 12].Value = sumSupporthour;
                        worksheet.Cells[row, 13].Value = sumEFF2;
                        worksheet.Cells[row, 14].Value = sumEFF3;
                        worksheet.Cells[row, 15].Value = sumEFFhr1;
                        worksheet.Cells[row, 16].Value = sumEFFhr2;
                        worksheet.Cells[row, 17].Value = sumEFFhr3;
                        worksheet.Cells[row, 18].Value = sumKPIh3;
                        worksheet.Cells[row, 19].Value = sumMEDh3;
                        worksheet.Cells[row, 20].Value = sumValEffh3;
                        worksheet.Cells[row, 21].Value = sumKPIh1;
                        worksheet.Cells[row, 22].Value = sumMEDh1;
                        worksheet.Cells[row, 23].Value = sumValEffh1;

                        for (int i = 6; i < 24; i++)
                        {
                            worksheet.Cells[row, i].Style.Font.Bold = true;
                        }



                        // Auto fit columns
                        worksheet.Cells.AutoFitColumns();

                        var stream = new MemoryStream();
                        package.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EFFReport.xlsx");

                    }



                }
                  

                }

            catch
            {
                TempData["AlertMessage"] = "System Some has Problem in Line, Plese contact IT!";
                return RedirectToAction("Login");

            }



        }




        [HttpGet]
        public ActionResult ProductionPlan(View_ProductionPlan obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_ProductionPlan = db.View_ProductionPlan.Where(x => x.PlantID == PlantID).ToList(),
                view_PLPS = db.View_PLPS.Where(p => p.PlantID.Equals(PlantID)).ToList()

            };
            
            ViewBag.VBRoleProductionPlan = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(32)).Select(x => x.RoleAction).FirstOrDefault();
            if ( !string.IsNullOrEmpty(obj.SectionName) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.ProductName) || obj.PlanDate != DateTime.MinValue)
            {//obj.PlanDate.HasValue != false ||

                if (obj.PlanDate != DateTime.MinValue)
                {
                    mymodel.view_ProductionPlan = mymodel.view_ProductionPlan.Where(x => x.PlanDate.Equals(obj.PlanDate)).ToList();
                    ViewBag.SelectedPlantDate = obj.PlanDate;
                }

                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    mymodel.view_ProductionPlan = mymodel.view_ProductionPlan.Where(x => x.SectionID.Equals(obj.SectionName)).ToList();
                    ViewBag.SelectedSectionName = obj.SectionName;
                }

                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    mymodel.view_ProductionPlan = mymodel.view_ProductionPlan.Where(x => x.LineID.Equals(obj.LineName)).ToList();
                    ViewBag.SelectedLineName = obj.LineName;
                }
                if (!string.IsNullOrEmpty(obj.ProductName))
                {
                    mymodel.view_ProductionPlan = mymodel.view_ProductionPlan.Where(x => x.ProductID.Equals(obj.ProductName)).ToList();
                    ViewBag.SelectedProductName = obj.ProductName;
                }


                // var ViewEmpPlantName = incentives.view_Incentive.Where(x => x.IncentiveName.Equals(obj.SectionName) || x.PlantName.Equals(obj.PlantName) || x.LineName.Equals(obj.LineName) || x.ProductName.Equals(obj.ProductName)).ToList();
              
                return View(mymodel);

            }
            else
            {
                ViewBag.SelectedPlantDate = DateTime.Today;
                mymodel.view_ProductionPlan = mymodel.view_ProductionPlan.Where(x => x.PlanDate == DateTime.Today).ToList();

              
                return View(mymodel);
            }


        }



        [HttpGet]
        public ActionResult ProductionPlanClear(View_ProductionPlan obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_ProductionPlan = db.View_ProductionPlan.Where(x => x.PlantID == PlantID).ToList(),
                view_PLPS = db.View_PLPS.Where(p => p.PlantID.Equals(PlantID)).ToList()

            };
 
            ViewBag.SelectedPlantDate = DateTime.Today;
            mymodel.view_ProductionPlan = mymodel.view_ProductionPlan.Where(x => x.PlanDate == DateTime.Today).ToList();

            return View("ProductionPlan",mymodel);
            


        }

        public JsonResult ProductionPlanEdit(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var ProductionPlan = db.View_ProductionPlan.Where(p => p.TransactionID.Equals(id) && p.PlantID.Equals(PlantID)).SingleOrDefault();
            return Json(ProductionPlan);
        }


        public ActionResult ProductionPlanUpdate(View_ProductionPlan obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbProductionPlan = db.TbProductionPlan.Where(x => x.PlantID == PlantID).ToList(),
                view_ProductionPlan = db.View_ProductionPlan.Where(x => x.PlantID == PlantID).ToList(),
                view_PLPS = db.View_PLPS.Where(p => p.PlantID.Equals(PlantID)).ToList()

            };
            ViewBag.SelectedPlantDate = DateTime.Now;
            ViewBag.VBRoleProductionPlan = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(32)).Select(x => x.RoleAction).FirstOrDefault();
            // Check Admin
            if (PlantID != 0)
            {
                // mymodel.view_Incentive = incentives.view_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_ProductionPlan = db.View_ProductionPlan.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }


            var Productplandb = mymodel.tbProductionPlan.Where(x => x.PlanDate == obj.PlanDate && x.LineID.Equals(obj.LineID) && x.SectionID.Equals(obj.SectionID) && x.ProductID.Equals(obj.ProductID) && x.Prefix.Equals(obj.Prefix)).SingleOrDefault();
            //if (obj.Weight != null)
            //{
            //    Productplandb.Weight = obj.Weight;

            //}
            if (obj.SizeMin != null)
            {
                Productplandb.SizeMin = obj.SizeMin;
            }
            if (obj.SizeMax != null)
            {
                Productplandb.SizeMax = obj.SizeMax;
            }
            //if (obj.QTY != null)
            //{
            //    Productplandb.QTY = obj.QTY;
            //}
            if (obj.QRcodeperday != null)
            {
                Productplandb.QRcodeperday = obj.QRcodeperday;
            }

            if (obj.TotalPiecePerDay != null)
            {
                Productplandb.TotalPiecePerDay = obj.TotalPiecePerDay;
            }


            Productplandb.UpdateBy = EmpID;// User.Identity.Name;
            obj.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("ProductionPlan");



        }


        public ActionResult ProductionPlanCreate(View_ProductionPlan obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            //Check Duplicate
            var plantdb = db.TbProductionPlan.Where(p => p.PlantID == obj.PlantID && p.LineID == obj.LineID && p.ProductID.Equals(obj.ProductName) && p.SectionID == obj.SectionID && p.Prefix.Equals(obj.Prefix)).ToList();
            //string maxIncentiveID = db.TbProductionPlan.Max(p => p.IncentiveID);

            // Increment the maximum IncentiveID for the new record
            //int nextIncentiveID = 1;
            //if (!string.IsNullOrEmpty(maxIncentiveID))
            //{
            //    nextIncentiveID = Convert.ToInt32(maxIncentiveID) + 1;
            //}
            if (plantdb.Count() == 0)
            {
                // string[] MonthYearArr = obj.MonthYear.Split('-');
                // Insert new Plant

                db.TbProductionPlan.Add(new TbProductionPlan()
                {
                    //IncentiveID = nextIncentiveID.ToString().PadLeft(5, '0'),
                    PlanDate = obj.PlanDate,
                    PlantID = PlantID,
                    Prefix = obj.Prefix,
                    LineID = obj.LineID,
                    SectionID = obj.SectionID,
                    ProductID = obj.ProductID,
                    SizeMin = obj.SizeMin,
                    SizeMax = obj.SizeMax,
                    Weight = obj.Weight,
                    QTY = obj.QTY,
                    CreateDate = DateTime.Today,
                    CreateBy = EmpID,//User.Identity.Name,
                    UpdateDate = DateTime.Today,
                    UpdateBy = EmpID,//User.Identity.Name,
                });
                db.SaveChanges();

            }
            else
            {
                TempData["AlertMessage"] = "Incentive Duplicate! please Inactive old data";
                ViewBag.Error = "Incentive Duplicate!";
                //  return PartialView("Plant",obj);
            }
            return RedirectToAction("ProductionPlan");


        }


        [HttpGet]
        public ActionResult ProductionPlanExport(View_ProductionPlan obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_ProductionPlan = db.View_ProductionPlan.Where(x => x.PlantID == PlantID).ToList(),
                    view_PLPS = db.View_PLPS.Where(p => p.PlantID.Equals(PlantID)).ToList()

            };

            ViewBag.VBRoleProductionPlan = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(32)).Select(x => x.RoleAction).FirstOrDefault();
            if (!string.IsNullOrEmpty(obj.SectionName) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.ProductName))
            {//obj.PlanDate.HasValue != false ||

                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    mymodel.view_ProductionPlan = mymodel.view_ProductionPlan.Where(x => x.SectionName.Equals(obj.SectionName)).ToList();
                    ViewBag.SelectedSectionName = obj.SectionName;
                }

                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    mymodel.view_ProductionPlan = mymodel.view_ProductionPlan.Where(x => x.LineName.Equals(obj.LineName)).ToList();
                    ViewBag.SelectedLineName = obj.LineName;
                }
                if (!string.IsNullOrEmpty(obj.ProductName))
                {
                    mymodel.view_ProductionPlan = mymodel.view_ProductionPlan.Where(x => x.ProductName.Equals(obj.ProductName)).ToList();
                    ViewBag.SelectedProductName = obj.ProductName;
                }



                var collection = mymodel.view_ProductionPlan.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("ProductionPlan");
                Sheet.Cells["A1"].Value = "TransactionID";
                Sheet.Cells["B1"].Value = "PlanDate";
                Sheet.Cells["C1"].Value = "ShiftID";
                Sheet.Cells["D1"].Value = "LineID";
                Sheet.Cells["E1"].Value = "ProductID";
                Sheet.Cells["F1"].Value = "SectionID";
                Sheet.Cells["G1"].Value = "Size-Min";
                Sheet.Cells["H1"].Value = "Size-Max";
                Sheet.Cells["I1"].Value = "QRcodeperday";
                Sheet.Cells["J1"].Value = "TotalPiecePerDay";
                Sheet.Cells["K1"].Value = "QTY";
                int row = 2;
                foreach (var item in collection)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.TransactionID;
                   // Sheet.Cells[string.Format("B{0}", row)].Value = item.PlanDate;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.PlanDate.ToString("yyyy-MM-dd");
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Prefix;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.LineID;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.ProductID;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.SectionID;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.SizeMin;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.SizeMax;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.QRcodeperday;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.TotalPiecePerDay;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.TotalPiecePerDay / item.QRcodeperday;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=ProductionPlan-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());

                return RedirectToAction("ProductionPlan", mymodel);

            }
            else
            {


                var collection = mymodel.view_ProductionPlan.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("ProductionPlan");
                Sheet.Cells["A1"].Value = "TransactionID";
                Sheet.Cells["B1"].Value = "PlanDate";
                Sheet.Cells["C1"].Value = "ShiftID";
                Sheet.Cells["D1"].Value = "LineID";
                Sheet.Cells["E1"].Value = "ProductID";
                Sheet.Cells["F1"].Value = "SectionID";
                Sheet.Cells["G1"].Value = "Size-Min";
                Sheet.Cells["H1"].Value = "Size-Max";
                Sheet.Cells["I1"].Value = "QRcodeperday";
                Sheet.Cells["J1"].Value = "TotalPiecePerDay";
                Sheet.Cells["K1"].Value = "QTY";
                int row = 2;
                foreach (var item in collection)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.TransactionID;
                   // Sheet.Cells[string.Format("B{0}", row)].Value = item.PlanDate;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.PlanDate.ToString("yyyy-MM-dd");
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Prefix;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.LineID;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.ProductID;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.SectionID;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.SizeMin;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.SizeMax;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.QRcodeperday;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.TotalPiecePerDay;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.TotalPiecePerDay/ item.QRcodeperday;
                    row++;
                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=ProductionPlan-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());


                //ViewBag.InactiveStatus = true;
                return RedirectToAction("ProductionPlan", mymodel);
            }

        }




        [HttpPost]
        public IActionResult ProductionPlanUpload(IFormFile FileUpload)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (FileUpload == null || FileUpload.Length <= 0)
            {
                //ViewBag.Error = "Please select a valid Excel file.";
                //return View("ProductionPlan");
                return Json(new { success = true, message = "Please select a valid Excel file!" });
            }
            int CntDb = db.TbProductionPlan.ToList().Count;
            int CntDbnext = CntDb;
            using (var stream = new MemoryStream())
            {
                FileUpload.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        if (worksheet.Cells[row, 2].Value != null && worksheet.Cells[row, 2].Value != "")
                        {

                            int id = 0 ;
                            if (worksheet.Cells[row, 1].Value != null)
                            {
                                id = Convert.ToInt32(worksheet.Cells[row, 1].Value.ToString().Trim());
                            }
                           // int shiftvar = Convert.ToInt32(worksheet.Cells[row, 3].Text.Trim());


                            var DataDb = db.TbProductionPlan.Where(x => x.TransactionID == id).SingleOrDefault();
                          //  var ShiftIDDb = db.TbShift.Where(x => x.ShiftID.Equals(shiftvar) && x.PlantID.Equals(PlantID)).Select(x => x.ShiftID).SingleOrDefault();
                            var LineIDDb = db.TbLine.Where(x => x.LineID.Equals(worksheet.Cells[row, 4].Text.Trim()) && x.PlantID.Equals(PlantID)).Select(x => x.LineID).SingleOrDefault();
                            var ProductIDDb = db.TbProduct.Where(x => x.ProductID.Equals(worksheet.Cells[row, 5].Text.Trim()) && x.PlantID.Equals(PlantID)).Select(x => x.ProductID).SingleOrDefault();
                            var SectionIDDb = db.TbSection.Where(x => x.SectionID.Equals(worksheet.Cells[row, 6].Text.Trim()) && x.PlantID.Equals(PlantID)).Select(x => x.SectionID).SingleOrDefault();

                            if ( LineIDDb == null || ProductIDDb == null || SectionIDDb == null)
                            {
                                int rowerror = row - 1;
                                TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake ";
                                return Json(new { success = false, message = "Data Row : " + rowerror + " =>  Mistake please check. " });
                                // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                              //  return RedirectToAction("ProductionPlan");

                            }



                            else
                            {
                                //checked PLPS
                                var PLPSData = db.TbPLPS.Where(x =>
                                x.PlantID.Equals(PlantID) &&
                                x.LineID.Equals(LineIDDb) &&
                                x.ProductID.Equals(ProductIDDb) &&
                                x.SectionID.Equals(SectionIDDb)
                                ).SingleOrDefault();

                                if (PLPSData == null)
                                {
                                    int rowerror = row - 1;
                                    TempData["AlertMessage"] = "Data Row : " + rowerror + " =>   Mistake please PLPS check. ";
                                  //  return Json(new { success = false, message = "Data Row : " + rowerror + " =>  " });
                                    // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                                  //  return RedirectToAction("ProductionPlan");
                                }
                                else
                                {


                                    if (DataDb != null)
                                    {

                                        // case code
                                        // Update existing record
                                        DataDb.PlanDate = DateTime.Parse(worksheet.Cells[row, 2].Text);
                                        DataDb.PlantID = PlantID;
                                        DataDb.Prefix = worksheet.Cells[row, 3].Text;
                                        DataDb.LineID = LineIDDb;
                                        DataDb.ProductID = ProductIDDb;
                                        DataDb.SectionID = SectionIDDb;
                                        DataDb.SizeMin = int.Parse(worksheet.Cells[row, 7].Text);
                                        DataDb.SizeMax = int.Parse(worksheet.Cells[row, 8].Text);
                                        DataDb.Weight = int.Parse(worksheet.Cells[row, 9].Text);
                                        DataDb.QRcodeperday = int.Parse(worksheet.Cells[row, 9].Text);
                                        DataDb.TotalPiecePerDay = int.Parse(worksheet.Cells[row, 10].Text);
                                         DataDb.QTY = int.Parse(worksheet.Cells[row, 10].Text) / int.Parse(worksheet.Cells[row, 9].Text);
                                        DataDb.UpdateDate = DateTime.Now;
                                        DataDb.UpdateBy = EmpID;

                                    }
                                    else
                                    {

                                      //  CntDbnext = CntDbnext + 1;
                              
                                        // Insert new record
                                        var newData = new TbProductionPlan
                                        {

                                            PlanDate = DateTime.Parse(worksheet.Cells[row, 2].Text),
                                            PlantID = PlantID,
                                            Prefix = worksheet.Cells[row,3].Text,
                                            LineID = LineIDDb,
                                            ProductID = ProductIDDb,
                                            SectionID = SectionIDDb,
                                            SizeMin = int.Parse(worksheet.Cells[row, 7].Text),
                                            SizeMax = int.Parse(worksheet.Cells[row, 8].Text),
                                            Weight ='0' ,//int.Parse(worksheet.Cells[row, 9].Text),
                                            QRcodeperday = int.Parse(worksheet.Cells[row, 9].Text),
                                            TotalPiecePerDay = int.Parse(worksheet.Cells[row, 10].Text),
                                            QTY = int.Parse(worksheet.Cells[row, 10].Text) / int.Parse(worksheet.Cells[row, 9].Text),
                                            CreateDate = DateTime.Now,
                                            CreateBy = EmpID,//User.Identity.Name;
                                            UpdateDate = DateTime.Now,
                                            UpdateBy = EmpID//User.Identity.Name;

                                        };


                                        db.TbProductionPlan.Add(newData);
                                    }

                                }
                            }
                        }

                    }
                    db.SaveChanges();
                }

            }

            ViewBag.Success = "Data imported and updated successfully!";
           // return RedirectToAction("ProductionPlan");
            return Json(new { success = true, message = "Data imported and updated successfully!" });

        }



        

       [HttpGet]
        public IActionResult FilterProductByEmployee(string selectedEmpID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            // Replace this with your logic to filter products based on the lineId

           // var emp = db.View_EmployeeClocktime.Where(x=>x.EmployeeID == selectedEmpID && x.PlantID.Equals(PlantID) && x.TransactionDate == DateTime.Today ).ToList();

            // Query with time-based filtering in addition to date and other conditions
            var empsection = db.View_EmployeeClocktime
                .Where(x => x.EmployeeID == selectedEmpID
                            && x.PlantID.Equals(PlantID)
                            && x.TransactionDate == DateTime.Today
                            && x.ClockIn != ""
                            && x.ClockOut == "")
                .SingleOrDefault();



            var groupedProducts = db.View_PLPS
                 .Where(x => x.SectionID.Equals(empsection.SectionID) && x.LineID.Equals(empsection.LineID))
                 .GroupBy(x => new { x.ProductID, x.ProductName })
                 .Select(group => new
                 {
                     ProductID = group.Key.ProductID,
                     ProductName = group.Key.ProductName
                 }).ToList();
            return Json(groupedProducts);
        }



        [HttpGet]
        public IActionResult FilterProductByEmployeeMinus(string selectedEmpID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            // Replace this with your logic to filter products based on the lineId

            // var emp = db.View_EmployeeClocktime.Where(x=>x.EmployeeID == selectedEmpID && x.PlantID.Equals(PlantID) && x.TransactionDate == DateTime.Today ).ToList();

            // Query with time-based filtering in addition to date and other conditions
            var empsection = db.View_EmployeeClocktime
                .Where(x => x.EmployeeID.Equals(selectedEmpID)
                            && x.PlantID.Equals(PlantID)
                            && x.TransactionDate == DateTime.Today
                            && x.ClockIn != ""
                            && x.ClockOut == "")
                .SingleOrDefault();



            var groupedProducts = db.View_PLPS
                 .Where(x => x.SectionID.Equals(empsection.SectionID) && x.LineID.Equals(empsection.LineID))
                 .GroupBy(x => new { x.ProductID, x.ProductName })
                 .Select(group => new
                 {
                     ProductID = group.Key.ProductID,
                     ProductName = group.Key.ProductName
                 }).ToList();
            return Json(groupedProducts);
        }



        [HttpGet]
        public IActionResult ProductionTransactionAdjust(View_ProductionTransactionAdjust obj)
        {


            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                 view_ProductionTransactionAdjust = db.View_ProductionTransactionAdjust.Where(x => x.PlantID == PlantID).ToList(),

            };

            ViewBag.VBRoleProducttionTransactionAjust = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(33)).Select(x => x.RoleAction).FirstOrDefault();
            if (!string.IsNullOrEmpty(obj.SectionName) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.Prefix) || obj.TransactionDate != DateTime.MinValue)
            {//obj.PlanDate.HasValue != false ||

                if (obj.TransactionDate != DateTime.MinValue)
                {
                    mymodel.view_ProductionTransactionAdjust = mymodel.view_ProductionTransactionAdjust.Where(x => x.TransactionDate.Equals(obj.TransactionDate)).ToList();
                    ViewBag.SelectedTransactionDate = obj.TransactionDate.ToString("yyyy-MM-dd");
                 
                }

                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    mymodel.view_ProductionTransactionAdjust = mymodel.view_ProductionTransactionAdjust.Where(x => x.SectionID.Equals(obj.SectionName)).ToList();
                    ViewBag.SelectedSectionName = obj.SectionName;
                }

                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    mymodel.view_ProductionTransactionAdjust = mymodel.view_ProductionTransactionAdjust.Where(x => x.LineID.Equals(obj.LineName)).ToList();
                    ViewBag.SelectedLineName = obj.LineName;
                }
                if (!string.IsNullOrEmpty(obj.Prefix))
                {
                    mymodel.view_ProductionTransactionAdjust = mymodel.view_ProductionTransactionAdjust.Where(x => x.Prefix.Equals(obj.Prefix)).ToList();
                    ViewBag.SelectedPrefix = obj.Prefix;
                }


                return View(mymodel);

            }
            else
            {
              mymodel.view_ProductionTransactionAdjust = mymodel.view_ProductionTransactionAdjust.Where(x => x.TransactionDate == DateTime.Today).ToList();
                ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
                return View(mymodel);
            }

        }


        public IActionResult ProductionTransactionAdjustClear(View_ProductionTransactionAdjust obj)
        {


            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_ProductionTransactionAdjust = db.View_ProductionTransactionAdjust.Where(x => x.PlantID == PlantID).ToList(),

            };

            ViewBag.VBRoleProducttionTransactionAjust = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(33)).Select(x => x.RoleAction).FirstOrDefault();
            mymodel.view_ProductionTransactionAdjust = mymodel.view_ProductionTransactionAdjust.Where(x => x.TransactionDate == DateTime.Today).ToList();
     
            return View("ProductionTransactionAdjust",mymodel);

        }




        public IActionResult ProductionTransactionAdjustFG(string FGPlanDate, String FGLine, String FGSection, String FGShift, int FGQTY, string[] TransactionID)
        {
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var mymodel = new ViewModelAll
            {
                tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_ProductionTransactionAdjust = db.View_ProductionTransactionAdjust.Where(x => x.PlantID == PlantID).ToList(),

            };

            //Check Duplicate
            int checkDuplicate = db.TbProductionTransactionAdjust.Where(x => x.TransactionDate.Date.Equals(Convert.ToDateTime(FGPlanDate)) && x.PlantID.Equals(PlantID) && x.LineID.Equals(FGLine) && x.SectionID.Equals(FGSection) && x.Prefix.Equals(FGShift) && x.Type.Equals("FG")).ToList().Count();
            if (checkDuplicate > 0)
            {

                //Update  TbProductionTransactionAdjust      
                var TranFGAdjust = db.TbProductionTransactionAdjust.Where(x => x.TransactionDate.Date.Equals(Convert.ToDateTime(FGPlanDate)) && x.PlantID.Equals(PlantID) && x.LineID.Equals(FGLine) && x.SectionID.Equals(FGSection) && x.Prefix.Equals(FGShift) && x.Type.Equals("FG")).SingleOrDefault();
                TranFGAdjust.QTY = FGQTY;
                db.SaveChanges();
                int ProductionTrand = db.TbProductionTransaction.Where(x => x.TransactionDate.Date.Equals(Convert.ToDateTime(FGPlanDate)) && x.PlantID.Equals(PlantID) && x.LineID.Equals(FGLine) && x.SectionID.Equals(FGSection) && x.Prefix.Equals(FGShift)).ToList().Count();
                if (ProductionTrand == 0)
                {

                    ViewBag.VBRoleProducttionTransactionAjust = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(33)).Select(x => x.RoleAction).FirstOrDefault();
                    mymodel.view_ProductionTransactionAdjust = mymodel.view_ProductionTransactionAdjust.Where(x => x.TransactionDate == DateTime.Today).ToList();
                    ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
                    return View("ProductionTransactionAdjust", mymodel);
                }
                // Calculate FG/Count for QTYPerQR
                int QRPerAdjust = FGQTY / ProductionTrand;
                string[] note;
                if (ProductionTrand != 0)
                {
                    var EmpIDtran = db.View_ProductionTransactionAdjust.Where(x => x.TransactionDate.Equals(Convert.ToDateTime(FGPlanDate)) && PlantID.Equals(PlantID) && x.LineID.Equals(FGLine) && x.SectionID.Equals(FGSection) && x.Prefix.Equals(FGShift)).Select(x => x.QRCode).ToList();

                    foreach (string item in EmpIDtran)
                    {
                        // Update Table : TbProductionTransaction column QTYPerQR
                        var ProdUpdate = db.TbProductionTransaction
                             .Where(x => x.TransactionDate.Date.Equals(Convert.ToDateTime(FGPlanDate)) &&
                                         x.PlantID.Equals(PlantID) &&
                                         x.LineID.Equals(FGLine) &&
                                         x.SectionID.Equals(FGSection) &&
                                         x.Prefix.Equals(FGShift) &&
                                         x.QRCode.Equals(item) &&
                                         x.DataType.Equals("Count"))
                             .ToList();

                        foreach (var transaction in ProdUpdate)
                        {

                            transaction.QtyPerQR = QRPerAdjust;

                            note = transaction.Note.Split(":");
                            if (note.Length > 0)
                            {
                                transaction.Note = "Replace : " + note[1] + "," + transaction.QtyPerQR;
                            }
                            else
                            {
                                transaction.Note = "Replace : " + transaction.QtyPerQR;
                            }
                            transaction.UpdateBy = EmpID; // User.Identity.Name;
                            transaction.UpdateDate = DateTime.Now;
                        }
                    }
                    db.SaveChanges();



                }
            }
            else
            {


                // Table : TbProductionTransactionAdjust  Create
                db.TbProductionTransactionAdjust.Add(new TbProductionTransactionAdjust()
                {
                    TransactionDate = Convert.ToDateTime(FGPlanDate),
                    PlantID = PlantID,
                    LineID = FGLine,
                    SectionID = FGSection,
                    Prefix = FGShift,
                    Type = "FG",
                    QTY = FGQTY,
                    CreateDate = DateTime.Now,
                    CreateBy = EmpID
                });
                //  db.SaveChanges();


                //// Count Employee base on plant, line ,section, productiondate ,prefix
                int ProductionTrandinsert = db.TbProductionTransaction.Where(x => x.TransactionDate.Date.Equals(Convert.ToDateTime(FGPlanDate)) && x.PlantID.Equals(PlantID) && x.LineID.Equals(FGLine) && x.SectionID.Equals(FGSection) && x.Prefix.Equals(FGShift)).ToList().Count();

                if (ProductionTrandinsert == 0)
                {

                    ViewBag.VBRoleProducttionTransactionAjust = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(33)).Select(x => x.RoleAction).FirstOrDefault();
                    mymodel.view_ProductionTransactionAdjust = mymodel.view_ProductionTransactionAdjust.Where(x => x.TransactionDate == DateTime.Today).ToList();
                    ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
                    return View("ProductionTransactionAdjust", mymodel);
                }
                // Calculate FG/Count for QTYPerQR
                int QRPerAdjustinsert = FGQTY / ProductionTrandinsert;
                if (ProductionTrandinsert != 0)
                {
                    var EmpIDtran = db.View_ProductionTransactionAdjust.Where(x => x.TransactionDate.Equals(Convert.ToDateTime(FGPlanDate)) && PlantID.Equals(PlantID) && x.LineID.Equals(FGLine) && x.SectionID.Equals(FGSection) && x.Prefix.Equals(FGShift)).Select(x => x.QRCode).ToList();

                    foreach (string item in EmpIDtran)
                    {
                        // Update Table : TbProductionTransaction column QTYPerQR
                        var ProdUpdate = db.TbProductionTransaction
                             .Where(x => x.TransactionDate.Date.Equals(Convert.ToDateTime(FGPlanDate)) &&
                                         x.PlantID.Equals(PlantID) &&
                                         x.LineID.Equals(FGLine) &&
                                         x.SectionID.Equals(FGSection) &&
                                         x.Prefix.Equals(FGShift) &&
                                         x.QRCode.Equals(item) &&
                                         x.DataType.Equals("Count"))
                             .ToList();

                        foreach (var transaction in ProdUpdate)
                        {

                            transaction.QtyPerQR = QRPerAdjustinsert;
                            transaction.Note = "Replace : " + transaction.QtyPerQR;
                            transaction.UpdateBy = EmpID; // User.Identity.Name;
                            transaction.UpdateDate = DateTime.Now;
                        }
                    }
                    db.SaveChanges();

                }


            }

            ViewBag.VBRoleProducttionTransactionAjust = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(33)).Select(x => x.RoleAction).FirstOrDefault();
            mymodel.view_ProductionTransactionAdjust = mymodel.view_ProductionTransactionAdjust.Where(x => x.TransactionDate == DateTime.Today).ToList();
            ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
            return View("ProductionTransactionAdjust", mymodel);

        }

        public IActionResult ProductionTransactionAdjustDefect(string DefectPlanDate, String DefectLine, String DefectSection, String DefectShift, int DefectQTY, string[] TransactionID)
        {
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var mymodel = new ViewModelAll
            {
                tbLine = db.TbLine.Where(x => x.PlantID == PlantID).ToList(),
                tbSection = db.TbSection.Where(x => x.PlantID == PlantID).ToList(),
                tbShift = db.TbShift.Where(x => x.PlantID == PlantID).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_ProductionTransactionAdjust = db.View_ProductionTransactionAdjust.Where(x => x.PlantID == PlantID).ToList(),

            };


            //Check Duplicate
            int checkDuplicate = db.TbProductionTransactionAdjust.Where(x => x.TransactionDate.Date.Equals(Convert.ToDateTime(DefectPlanDate)) && x.PlantID.Equals(PlantID) && x.LineID.Equals(DefectLine) && x.SectionID.Equals(DefectSection) && x.Prefix.Equals(DefectShift) && x.Type.Equals("Defect")).ToList().Count();
            if (checkDuplicate > 0)
            {

                //Update  TbProductionTransactionAdjust      
                var TranDefectAdjust = db.TbProductionTransactionAdjust.Where(x => x.TransactionDate.Date.Equals(Convert.ToDateTime(DefectPlanDate)) && x.PlantID.Equals(PlantID) && x.LineID.Equals(DefectLine) && x.SectionID.Equals(DefectSection) && x.Prefix.Equals(DefectShift) && x.Type.Equals("Defect")).SingleOrDefault();
                TranDefectAdjust.QTY = DefectQTY;
                db.SaveChanges();
            }
            else { 

            // Table : TbProductionTransactionAdjust  Create
            db.TbProductionTransactionAdjust.Add(new TbProductionTransactionAdjust()
            {
                TransactionDate = Convert.ToDateTime(DefectPlanDate),
                PlantID = PlantID,
                LineID = DefectLine,
                SectionID = DefectSection,
                Prefix = DefectShift,
                Type = "Defect",
                QTY = DefectQTY,
                CreateDate = DateTime.Now,
                CreateBy = EmpID
            });
              db.SaveChanges();

            }
            ViewBag.VBRoleProducttionTransactionAjust = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(33)).Select(x => x.RoleAction).FirstOrDefault();
            mymodel.view_ProductionTransactionAdjust = mymodel.view_ProductionTransactionAdjust.Where(x => x.TransactionDate == DateTime.Today).ToList();
            ViewBag.SelectedTransactionDate = DateTime.Today.ToString("yyyy-MM-dd");
            return View("ProductionTransactionAdjust", mymodel);


        }



        /////////////////////////////********************   End Controller *******************///////////////////////////////////////////////

    }
}
