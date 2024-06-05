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
using System.Text;
using System.Drawing.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace Plims.Controllers
{
    public class MasterController : Controller
    {
        private readonly AppDbContext db;
        public MasterController(AppDbContext _db)
        {
            db = _db;
        }


        public readonly string startpath = "E:\\Plims_files\\Excel Export\\";
        public readonly string QREmppath = "E:\\Plims_files\\QRCode\\Employee\\";
        public readonly string driveDPath = @"E:\Plims_files\QRCode\EmployeeGroup";

        /// <summary>
        /// View Plant
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Plant(TbPlant obj)

        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ModelState.Clear();

            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.ToList(),

            };

            if (PlantID != 0)
            {
                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }
            
            if (!string.IsNullOrEmpty(obj.PlantName) || obj.PlantID != 0 )
            {
               
                if (!string.IsNullOrEmpty(obj.PlantName))
                {
                    mymodel.tbPlants = db.TbPlant.Where(x => x.PlantName == obj.PlantName).ToList();
                    ViewBag.SelectedPlant = obj.PlantName;
                }
                if (obj.PlantID != 0)
                {
                    mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID == obj.PlantID).ToList();
                }
                // mymodel.tbPlants = db.TbPlant.Where(p => p.PlantName.Equals(obj.PlanlintName) || p.PlantID.Equals(obj.PlantID)).OrderByDescending(x => x.Status); ;

                ViewBag.VBRolePlant = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(5)).Select(x => x.RoleAction).FirstOrDefault();




                return View(mymodel);


            }
            else
            {
  
                ViewBag.VBRolePlant = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID  && x.PageID.Equals(5)).Select(x => x.RoleAction).FirstOrDefault();

                return View(mymodel);

            }

        }

        [HttpPost]
        public ActionResult PlantCreate(TbPlant obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (db.View_PermissionMaster.Where(x => x.UserEmpID.Equals(EmpID) && x.PageID.Equals(1)).Select(x => x.RoleAction).SingleOrDefault() == "Full")
            {
                //Check Duplicate
                var plantdb = db.TbPlant.Where(p => p.PlantName.Equals(obj.PlantName) & p.Status.Equals(1));
                //var userdb = db.TbUser.Where(x => x.ID.Equals("User.Identity.Name")).SingleOrDefault();
                int PlantNext = db.TbPlant.Select(x => x.PlantID).Max() + 1;

                if (plantdb.Count() == 0  && PlantID == 0)
                {
                    // Insert new Plant               
                    db.TbPlant.Add(new TbPlant()
                    {
                         PlantID = PlantNext,
                        PlantName = obj.PlantName,
                        Status = obj.Status,
                        CreateDate = DateTime.Now,
                        CreateBy = EmpID,//userdb.UserEmpID
                        UpdateDate = DateTime.Now,
                        UpdateBy = EmpID//userdb.UserEmpID
                    });

                    //IF Admin create user

                    for (int i = 1; i <= db.TbPageMaster.Count(); i++)
                    {
                        var pagenamevar = db.TbPageMaster.Where(x => x.PageNo.Equals(i)).Select(x => x.PageName).SingleOrDefault();

                        db.TbPage.Add(new TbPage()
                        {
                            PageID = i,
                            PageName = pagenamevar,
                            PageStatus = 1,
                            PlantID = PlantNext,
                            CreateDate = DateTime.Today,
                            CreateBy = EmpID,//User.Identity.Name,
                            UpdateDate = DateTime.Today,
                            UpdateBy = EmpID,//User.Identity.Name

                        });


                        db.TbPermission.Add(new TbPermission()
                        {

                            RoleID = 1,
                            PageID = i,
                            RoleAction = "Full",
                            PlantID = PlantNext,
                            CreateBy = EmpID,
                            CreateDate = DateTime.Today,
                            UpdateBy = EmpID,
                            UpdateDate = DateTime.Today
                        });

                    }
                  
                        db.TbUser.Add(new TbUser() 
                        { 
                               UserName = "Admin",
                               UserLastName = obj.PlantName, 
                               UserPassword = "Admin"+obj.PlantName,
                               UserPermission = 1,
                               UserEmpID = "Admin"+obj.PlantName,
                               Status = 1,
                                UserEmail = "",
                                PasswordLastUpdate = DateTime.Today,
                            PlantID  = PlantNext,
                            Lineconcern ="",
                            CreateDate = DateTime.Today,
                            CreateBy = EmpID,
                            UpdateDate = DateTime.Today,
                            UpdateBy = EmpID,


                        });

                        db.TbRole.Add(new TbRole()
                        {

                        RoleID = 1 ,
                        RoleName = "Admin",
                        RoleStatus = 1,
                            PlantID = PlantNext,
                            CreateBy = EmpID,
                            CreateDate = DateTime.Today,
                            UpdateBy = EmpID,
                            UpdateDate = DateTime.Today
                        });

                   
                    db.SaveChanges();

                }
                else
                {
                    TempData["AlertMessage"] = "Plant Duplicate!";
                    ViewBag.Error = "Plant Duplicate!";

                }
            }
            return RedirectToAction("Plant");
        }


        // 4. Function Plant Clear Fillter
        public ActionResult PlantClear()
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
                tbPlants = db.TbPlant.ToList(),

            };
            if(PlantID != 0)
            {
                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            return RedirectToAction("Plant");

        }

        [HttpGet]
        public JsonResult PlantEdit(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.ToList()
            };

            if (PlantID != 0)
            {
                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            if (db.View_PermissionMaster.Where(x => x.UserEmpID.Equals(EmpID) && x.PageID.Equals(1)).Select(x => x.RoleAction).SingleOrDefault() == "Full")
            {

                var plantdb = mymodel.tbPlants.Where(x => x.PlantID == id).SingleOrDefault();
                return Json(plantdb);
            }
            else
            {
                return Json(mymodel);
            }
           
         }


        [HttpPost]
        public ActionResult PlantUpdate(TbPlant obj)
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
                tbPlants = db.TbPlant.ToList(),

            };

            // check admin
            if (PlantID != 0)
            {
                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }

            var plantdb = mymodel.tbPlants.Where(x => x.PlantID == obj.PlantID).SingleOrDefault();
            if (obj.PlantName != null)
            { 

                plantdb.PlantName = obj.PlantName;
            }
            if (obj.Status == 1) 
            { 
                plantdb.Status = 1; 
            }
            else 
            { 
                plantdb.Status = 0; 
            }

            plantdb.UpdateBy = EmpID;
            plantdb.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Plant");

        }

        public JsonResult PlantActive(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.ToList(),

            };

            if (PlantID != 0)
            {
                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }



            var Plantdb = mymodel.tbPlants.Where(p => p.PlantID.Equals(id)).SingleOrDefault();
            if (Plantdb != null)
            {
                Plantdb.Status = 1;
                Plantdb.UpdateBy = EmpID;
                Plantdb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }
            return Json(new { success = true });
            //   return Json(db.TbPlant, JsonRequestBehavior.AllowGet);

        }



        // 7. Function Plant Inactive transaction
        public JsonResult PlantInactive(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.ToList(),

            };

            if (PlantID != 0)
            {
                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            var Plantdb = mymodel.tbPlants.Where(p => p.PlantID.Equals(id)).SingleOrDefault();
            if (Plantdb != null)
            {
                Plantdb.Status = 0;
                Plantdb.UpdateBy = EmpID;
                Plantdb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }
            return Json(new { success = true });

        }

        //End Master Plant
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Master Line
        /// </summary>
        /// <returns></returns>


        [HttpGet]
        public ActionResult Line(TbLine obj , string submit, bool? inactivestatus)
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
                tbLine = db.TbLine.OrderByDescending(x => x.Status).ToList(),
            };
                // Check Admin
                if (PlantID != 0)
                {
                    mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                }

                ViewBag.VBRoleLine = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(6)).Select(x => x.RoleAction).FirstOrDefault();

            if (submit == "clear")
            {
                ViewBag.SelectedLineID = "";
            }

            if (!string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.LineID) || inactivestatus != null)
            {
                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    mymodel.tbLine = db.TbLine.Where(x => x.LineName == obj.LineName).ToList();
                    ViewBag.SelectedLineName = obj.LineName;
                }
                if (!string.IsNullOrEmpty(obj.LineID))
                {
                    mymodel.tbLine = mymodel.tbLine.Where(x => x.LineID == obj.LineID).ToList();
                    ViewBag.SelectedLineID = obj.LineID;
                }
               
                    if (inactivestatus == true)
                    {
                        mymodel.tbLine = mymodel.tbLine.ToList();
                      ViewBag.InactiveStatus = true;
                }
                    else
                    {
                        mymodel.tbLine = mymodel.tbLine.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }
                  
               

                // mymodel.tbLine = db.TbLine.Where(p => p.LineName.Equals(obj.LineName) || p.LineID.Equals(obj.LineID)).OrderByDescending(x => x.Status);
                return View(mymodel);
            }
            else
            {
           
                ViewBag.InactiveStatus = true;
                return View(mymodel);

            }
            }
            catch
            {
                TempData["AlertMessage"] = "System Some has Problem in Line, Plese contact IT!";
                return RedirectToAction("Login");

            }

        }

        public ActionResult UserClear()
        {
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbLine = db.TbLine.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            ViewBag.InactiveStatus = true;
            return RedirectToAction("Line");

        }

        //3. Function Line Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult LineEdit(int id)
        {
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbLine = db.TbLine.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            // var Linedb = db.TbLine.Where(x => x.LineID.Equals(id.PadLeft(5, '0')) && x.PlantID.Equals(PlantID)).SingleOrDefault();
            var Linedb = mymodel.tbLine.Where(x => x.ID.Equals(id) ).SingleOrDefault();
            return Json(Linedb);


        }

        // 5. Function Line Update Transaction
        [HttpPost]
        public ActionResult LineUpdate(TbLine obj)
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
                tbLine = db.TbLine.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            var Linedb = mymodel.tbLine.Where(x => x.LineID == obj.LineID).SingleOrDefault();
                if (obj.LineName != null)
                {
                    Linedb.LineName = obj.LineName;

                }
                    if (obj.Status != null)
                    {
                        if(obj.Status == 1)
                        {
                            Linedb.Status = 1;
                        }
                        else
                        {
                            Linedb.Status = 0;
                        }
                    }
                Linedb.UpdateBy = EmpID;
                obj.UpdateDate = DateTime.Now;
                db.SaveChanges();
            
            
            return RedirectToAction("Line");
        }


        // 6. Function Line Create transaction
        [HttpPost]
        public ActionResult LineCreate(TbLine obj)
        {
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }


                var cntline = db.TbLine.Select(x => x.LineID).Max();
                int cntlinenext = Convert.ToInt16(cntline) +1;

            // Insert new Line
            db.TbLine.Add(new TbLine()
            {
               // ID = db.TbLine.Count() + 1,
                LineID = cntlinenext.ToString().PadLeft(5,'0'),
                LineName = obj.LineName,
                PlantID = PlantID,
                Status = obj.Status,
                CreateDate = DateTime.Now,
                CreateBy = EmpID,
                UpdateDate = DateTime.Now,
                UpdateBy = EmpID,
            }); 
                db.SaveChanges();

            return RedirectToAction("Line");
        }

        // 7. Function Line Active transaction
        public JsonResult LineActive(int id)
        {
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));

            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbLine = db.TbLine.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }

            var Linedb = mymodel.tbLine.Where(p => p.ID.Equals(id)).SingleOrDefault();
            if (Linedb != null)
            {
                Linedb.Status = 1;
                Linedb.UpdateBy = EmpID;
                Linedb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbLine);

        }




        // 7. Function Line Inactive transaction
        public JsonResult LineInactive(int id)
        {
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));

            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbLine = db.TbLine.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }

            var Linedb = mymodel.tbLine.Where(p => p.ID.Equals(id) ).SingleOrDefault();
            if (Linedb != null)
            {
                Linedb.Status = 0;
                Linedb.UpdateBy = EmpID;
                Linedb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbLine);

        }

        [HttpGet]
        public ActionResult LineExport(TbLine obj, string submit, bool? inactivestatus)
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
                    tbLine = db.TbLine.OrderByDescending(x => x.Status).ToList(),

                };

                // Check Admin
                if (PlantID != 0)
                {
                    mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                }


                ViewBag.VBRoleLine = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(6)).Select(x => x.RoleAction).FirstOrDefault();

                if (submit == "clear")
                {
                    ViewBag.SelectedLineID = "";
                }

                if (!string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.LineID) || inactivestatus != null)
                {
                    if (!string.IsNullOrEmpty(obj.LineName))
                    {
                        mymodel.tbLine = db.TbLine.Where(x => x.LineName == obj.LineName).ToList();
                        ViewBag.SelectedLineName = obj.LineName;
                    }
                    if (!string.IsNullOrEmpty(obj.LineID))
                    {
                        mymodel.tbLine = mymodel.tbLine.Where(x => x.LineID == obj.LineID).ToList();
                        ViewBag.SelectedLineID = obj.LineID;
                    }

                    if (inactivestatus == true)
                    {
                        mymodel.tbLine = mymodel.tbLine.ToList();
                        ViewBag.InactiveStatus = true;
                    }
                    else
                    {
                        mymodel.tbLine = mymodel.tbLine.Where(x => x.Status == 1).ToList();
                        ViewBag.InactiveStatus = false;
                    }

                    var collection = mymodel.tbLine.ToList();
                    ExcelPackage Ep = new ExcelPackage();
                    ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Line");
                    Sheet.Cells["A1"].Value = "LineID";
                    Sheet.Cells["B1"].Value = "LineName";
                    Sheet.Cells["C1"].Value = "Status";
                    int row = 2;
                    foreach (var item in collection)
                    {

                        Sheet.Cells[string.Format("A{0}", row)].Value = item.LineID;
                        Sheet.Cells[string.Format("B{0}", row)].Value = item.LineName;
                        Sheet.Cells[string.Format("C{0}", row)].Value = item.Status;
                        row++;
                    }
                    Sheet.Cells["A:AZ"].AutoFitColumns();
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.Headers.Add("content-disposition", "attachment; filename=Line-Master.xlsx"); // Fix typo ':' should be ';'
                    Response.Body.WriteAsync(Ep.GetAsByteArray());


                    // mymodel.tbLine = db.TbLine.Where(p => p.LineName.Equals(obj.LineName) || p.LineID.Equals(obj.LineID)).OrderByDescending(x => x.Status);
                    return View("Line",mymodel);
                }
                else
                {

                    var collection = mymodel.tbLine.ToList();
                    using (var Ep = new ExcelPackage())
                    {
                        var Sheet = Ep.Workbook.Worksheets.Add("Line");
                        Sheet.Cells["A1"].Value = "LineID";
                        Sheet.Cells["B1"].Value = "LineName";
                        Sheet.Cells["C1"].Value = "Status";
                        int row = 2;
                        foreach (var item in collection)
                        {
                            Sheet.Cells[string.Format("A{0}", row)].Value = item.LineID;
                            Sheet.Cells[string.Format("B{0}", row)].Value = item.LineName;
                            Sheet.Cells[string.Format("C{0}", row)].Value = item.Status;
                            row++;
                        }
                        Sheet.Cells["A:AZ"].AutoFitColumns();

                        Sheet.Cells["A:AZ"].AutoFitColumns();
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.Headers.Add("content-disposition", "attachment; filename=Line-Master.xlsx"); // Fix typo ':' should be ';'
                        Response.Body.WriteAsync(Ep.GetAsByteArray());


                        // Send the Excel file as the response
                        //  return File(content, contentType, fileName);
                    }


                    ViewBag.InactiveStatus = true;
                    return RedirectToAction("Line", mymodel);

                }
            }
            catch
            {
                TempData["AlertMessage"] = "System Some has Problem in Line, Plese contact IT!";
                return RedirectToAction("Login");

            }

        }






        [HttpPost]
        public IActionResult LineUpload(IFormFile FileUpload)
        {
            // Retrieve additional data from session or request context
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                // Check if file was uploaded
                if (FileUpload == null || FileUpload.Length <= 0)
                {
                    ViewBag.Error = "Please select a valid Excel file.";
                    return View("Line");
                }
                string fileName = FileUpload.FileName;
                long fileSize = FileUpload.Length;
                // Read Excel file data
                using (var stream = new MemoryStream())
                {
                    FileUpload.CopyTo(stream);
                    stream.Position = 0; // Reset stream position

                    try
                    {
                        // Access file properties and content
                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets[0];

                            // Process each row in the Excel file
                            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                            {
                                if (worksheet.Cells[row, 2].Value != null)
                                {

                                    if (Convert.ToInt32(worksheet.Cells[row, 3].Text) != 1 || Convert.ToInt32(worksheet.Cells[row, 3].Text) != 0)
                                    {
                                        int rowerror = row - 1;
                                        TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake please check Master ";
                                        // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                                        return RedirectToAction("Section");

                                    }

                                    var lineIds = worksheet.Cells[row, 1].Value?.ToString() ?? "0";
                                    var lineName = worksheet.Cells[row, 2].Text;
                                    var lineStatus = worksheet.Cells[row, 3].Text; // Assuming a default value

                                    // Call your method to update or insert line data
                                    UpdateOrInsertLine(lineIds, lineName, lineStatus, EmpID);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log and handle exceptions related to reading Excel package
                        Console.WriteLine($"Error reading Excel package: {ex}");
                        throw; // Rethrow the exception to propagate it further
                    }
                }

               // ViewBag.Success = "Data imported and updated successfully!";
               // return RedirectToAction("Line");
                return Json(new { success = true, message = "Data imported and updated successfully!" });
            }
            catch (Exception ex)
            {
                // Log and handle exceptions
                //ViewBag.Error = $"An error occurred: {ex.Message}";
                //return RedirectToAction("Line");
                return Json(new { success = false, message = "Data Upload mistake please check!" });


            }
        }

        private void UpdateOrInsertLine(string lineIds, string lineName, string lineStatus, string EmpId)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            //var lineDb = db.TbLine.Where(x=>x.LineID.Equals(lineIds)).ToList();
            var lineDb = db.TbLine.Where(x => x.LineID.Equals(lineIds) && x.PlantID.Equals(PlantID)).SingleOrDefault();
            if (lineDb != null)
            {
                // Update existing record
                lineDb.LineName = lineName;
                lineDb.Status = int.Parse(lineStatus);
                lineDb.UpdateDate = DateTime.Now;
                lineDb.UpdateBy = EmpId;
            }
            else
            {
                int CntlineDb = db.TbLine.ToList().Count;
                int CntlineDbnext = CntlineDb + 1;
                // Insert new record
                var newLine = new TbLine
                {

                    LineID = CntlineDbnext.ToString().PadLeft(5, '0'),
                    LineName = lineName,
                    PlantID = PlantID,
                    Status = 1,
                    CreateDate = DateTime.Now,
                    CreateBy = EmpId,
                    UpdateDate = DateTime.Now,
                    UpdateBy = EmpId
                };

                db.TbLine.Add(newLine);
            }

            db.SaveChanges();
        }

   



        // End Master Line
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        /// <summary>
        /// Master Product
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult Product(TbProduct obj, bool? inactivestatus)
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
                tbProduct = db.TbProduct.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            ViewBag.VBRoleProduct = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(7)).Select(x => x.RoleAction).FirstOrDefault();


            if (!string.IsNullOrEmpty(obj.ProductName) || !string.IsNullOrEmpty(obj.ProductID) || inactivestatus != null)
            {
                var TbProduct = from p in db.TbProduct
                                select p;
                if (!string.IsNullOrEmpty(obj.ProductName))
                {
                    mymodel.tbProduct = mymodel.tbProduct.Where(p => p.ProductName.Equals(obj.ProductName));
                    ViewBag.SelectedProductName = obj.ProductName;
                }
                if (!string.IsNullOrEmpty(obj.ProductID))
                {
                    mymodel.tbProduct = mymodel.tbProduct.Where(x => x.ProductID == obj.ProductID);
                    ViewBag.SelectedProductID = obj.ProductID;
                }

                if (inactivestatus == true)
                {
                    mymodel.tbProduct = mymodel.tbProduct.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    mymodel.tbProduct = mymodel.tbProduct.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }



                //var TbProduct = db.TbProduct.Where(p => p.ProductName.Equals(obj.ProductName) && p.ProductID.Equals(obj.ProductID)).ToList();
                return View(mymodel);
            }
            else
            {
                ViewBag.InactiveStatus = true;
                return View(mymodel);
            }

        }


        // 3.Function Product Clear fillter
        public ActionResult ProductClear()
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
                tbProduct = db.TbProduct.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }
            return RedirectToAction("Product");

        }

        //4. Function product Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult ProductEdit(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbProduct = db.TbProduct.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }

            var productdb = mymodel.tbProduct.Where(x => x.ID.Equals(id)).SingleOrDefault();
            return Json(productdb);

        }


        // 5. Function product Update Transaction
        [HttpPost]
        public ActionResult ProductUpdate(TbProduct obj)
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
                tbProduct = db.TbProduct.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            var productdb = mymodel.tbProduct.Where(x => x.ProductID == obj.ProductID ).SingleOrDefault();
            if (obj.ProductName != null)
            {
                productdb.ProductName = obj.ProductName;

            }
            if (obj.Status != null)
            {
                if (obj.Status == 1)
                {
                    productdb.Status = 1;
                }
                else
                {
                    productdb.Status = 0;
                }
            }
            productdb.UpdateBy = EmpID;// User.Identity.Name;
            productdb.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Product");

        }


        // 6. Function Product Create transaction
        [HttpPost]
        public ActionResult ProductCreate(TbProduct obj)
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
                tbProduct = db.TbProduct.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            var Productdb = mymodel.tbProduct.Where(p => p.ProductID.Equals(obj.ProductID) && p.ProductName.Equals(obj.ProductName));
            int cnt = db.TbProduct.Count() + 1;
          
                // Insert new Product               
                db.TbProduct.Add(new TbProduct()
                {
                    ProductID = obj.ProductID, //cnt.ToString().PadLeft(5,'0'),
                    ProductName = obj.ProductName,
                    PlantID = PlantID,
                    Status = obj.Status,
                    CreateDate = DateTime.Now,
                    CreateBy = EmpID,//User.Identity.Name,
                    UpdateDate = DateTime.Now,
                    UpdateBy = EmpID,//User.Identity.Name,
                });
                db.SaveChanges();

           
            return RedirectToAction("Product");
        }



        // 7. Function Product Inactive transaction
        public JsonResult ProductActive(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbProduct = db.TbProduct.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }

            var Producdb = mymodel.tbProduct.Where(p => p.ID.Equals(id)).SingleOrDefault();
            if (Producdb != null)
            {
                Producdb.Status = 1;
                Producdb.UpdateBy = EmpID;// User.Identity.Name;
                Producdb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbProduct);

        }




        // 7. Function Product Inactive transaction
        public JsonResult ProductInactive(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbProduct = db.TbProduct.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            var Producdb = mymodel.tbProduct.Where(p => p.ID.Equals(id)).SingleOrDefault();
            if (Producdb != null)
            {
                Producdb.Status = 0;
                Producdb.UpdateBy = EmpID;// User.Identity.Name;
                Producdb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbProduct);

        }



        //[HttpPost]
        //public IActionResult ProductProcessDataTable([FromBody] List<Dictionary<string, string>> tableData)
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
        //        tbProduct = db.TbProduct.Where(p=>p.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
        //    };

        //    try
        //    {
        //        // Create a DataTable
        //        DataTable dataTable = new DataTable("Product");

        //        // Add headers to the DataTable
        //        //var headers = tableData.First().Keys.ToList();
        //        var headers = tableData.First().Keys.Take(3).ToList();
        //        foreach (var header in headers)
        //        {
        //            dataTable.Columns.Add(header.Trim());

        //        }

        //        // Add data to the DataTable
        //        foreach (var rowData in tableData)
        //        {
        //            var dataRow = dataTable.NewRow();
        //            foreach (var header in headers)
        //            {
        //                dataRow[header.Trim()] = rowData[header].Trim();
        //            }
        //            dataTable.Rows.Add(dataRow);
        //        }

        //        using (var package = new ExcelPackage())
        //        {
        //            // Create a worksheet
        //            var worksheet = package.Workbook.Worksheets.Add("Product");

        //            // Add headers to the worksheet
        //            for (int col = 1; col <= dataTable.Columns.Count; col++)
        //            {
        //                var headername = dataTable.Columns[col - 1].ColumnName;
        //                worksheet.Cells[1, col].Value = headername;
        //            }

        //            // Add data to the worksheet
        //            for (int row = 0; row < dataTable.Rows.Count; row++)
        //            {
        //                for (int col = 1; col <= dataTable.Columns.Count; col++)
        //                {
        //                    try
        //                    {
        //                        var DataName = dataTable.Rows[row][col - 1].ToString();
        //                        worksheet.Cells[row + 2, col].Value = DataName;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Console.WriteLine($"Error accessing data at row {row}, col {col}: {ex.Message}");
        //                        // Log the error using your logging framework (e.g., Serilog, NLog)
        //                        return StatusCode(500, $"Error exporting data: {ex.Message}");
        //                    }
        //                }
        //            }

        //            var filePath = startpath + "ProductMaster.xls";
        //            System.IO.File.WriteAllBytes(filePath, package.GetAsByteArray());

        //            //// Save the Excel package to a stream
        //            // var stream = new MemoryStream(package.GetAsByteArray());
        //            //// Set the position to the beginning of the stream
        //            //stream.Position = 0;
        //            //// Return the Excel file as a FileStreamResult
        //            //File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LineReport.xlsx");
        //            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        //            return File(fileStream, "application/vnd.ms-excel", "ProductMaster.xls");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error generating Excel file: {ex.Message}");
        //        // Log the error using your logging framework (e.g., Serilog, NLog)
        //        return StatusCode(500, $"Error exporting data: {ex.Message}");
        //    }
        //}





        [HttpGet]
        public ActionResult ProductExport(TbProduct obj, bool? inactivestatus)
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
                tbProduct = db.TbProduct.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }

            ViewBag.VBRoleProduct = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(7)).Select(x => x.RoleAction).FirstOrDefault();


            if (!string.IsNullOrEmpty(obj.ProductName) || !string.IsNullOrEmpty(obj.ProductID) || inactivestatus != null)
            {
                var TbProduct = from p in db.TbProduct
                                select p;
                if (!string.IsNullOrEmpty(obj.ProductName))
                {
                    mymodel.tbProduct = mymodel.tbProduct.Where(p => p.ProductName.Equals(obj.ProductName));
                    ViewBag.SelectedProductName = obj.ProductName;
                }
                if (!string.IsNullOrEmpty(obj.ProductID))
                {
                    mymodel.tbProduct = mymodel.tbProduct.Where(x => x.ProductID == obj.ProductID);
                    ViewBag.SelectedProductID = obj.ProductID;
                }

                if (inactivestatus == true)
                {
                    mymodel.tbProduct = mymodel.tbProduct.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    mymodel.tbProduct = mymodel.tbProduct.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }


                var collection = mymodel.tbProduct.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Line");
                Sheet.Cells["A1"].Value = "ProductID";
                Sheet.Cells["B1"].Value = "ProductName";
                Sheet.Cells["C1"].Value = "Status";
                int row = 2;
                foreach (var item in collection)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.ProductID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.ProductName;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Status;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=Product-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());


                //var TbProduct = db.TbProduct.Where(p => p.ProductName.Equals(obj.ProductName) && p.ProductID.Equals(obj.ProductID)).ToList();
                return RedirectToAction("Product", mymodel);
            }
            else
            {

                var collection = mymodel.tbProduct.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Product");
                Sheet.Cells["A1"].Value = "ProductID";
                Sheet.Cells["B1"].Value = "ProductName";
                Sheet.Cells["C1"].Value = "Status";
                int row = 2;
                foreach (var item in collection)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.ProductID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.ProductName;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Status;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=Product-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());


                ViewBag.InactiveStatus = true;
                return RedirectToAction("Product",mymodel);
            }

        }




        [HttpPost]
        public IActionResult ProductUpload(IFormFile FileUpload)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            int CntDb = db.TbProduct.ToList().Count;
            int CntDbnext = CntDb;
            if (FileUpload == null || FileUpload.Length <= 0)
            {
                TempData["AlertMessage"] = "Please select a valid Excel file.";
                //ViewBag.Error = "Please select a valid Excel file.";
                return View("Product");
            }

            using (var stream = new MemoryStream())
            {

                FileUpload.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        if (worksheet.Cells[row, 2].Value != null )
                        {
                            if (worksheet.Cells[row,3].Text != "1" && worksheet.Cells[row, 3].Text != "0")
                            {
                                int rowerror = row - 1;
                                //TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake please check Master ";
                                return Json(new { success = true, message =  "Data Row : " + rowerror + " =>  Mistake  please check Master " });

                                // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                              //  return RedirectToAction("Product");

                            }

                            if (worksheet.Cells[row, 1].Text == "" )
                            {
                                int rowerror = row - 1;
                              //  TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake please check Product ID ";
                                // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                                return Json(new { success = true, message  = "Data Row : " + rowerror + " =>  Mistake please check Product ID " });
                               // return RedirectToAction("Product");

                            }


                            var id = "";
                            if (worksheet.Cells[row, 1].Value != null)
                            {
                               id = worksheet.Cells[row, 1].Value.ToString();
                            }
                            var productID = worksheet.Cells[row,1].Text;
                            var name = worksheet.Cells[row, 2].Text;
                            int Status = Convert.ToInt32(worksheet.Cells[row, 3].Text);

                            var DataDb = db.TbProduct.Where(x => x.ProductID.Equals(id) && x.PlantID.Equals(PlantID)).SingleOrDefault();
                           // var DataDb = db.TbProduct.Find(id).;

                            if (DataDb != null)
                            {
                                // Update existing record

                                DataDb.ProductName = name;
                                DataDb.PlantID = PlantID;
                                DataDb.Status = Status;//int.Parse(Status);
                                DataDb.UpdateDate = DateTime.Now;
                                DataDb.UpdateBy = EmpID; //User.Identity.Name;
                            }
                            else
                            {
                                //int CntDb = db.TbProduct.ToList().Count;
                              //   CntDbnext = CntDb + 1;
                                // Insert new record
                                var newData = new TbProduct
                                {
                                    ProductID = productID , //CntDbnext.ToString().PadLeft(5, '0'),
                                    ProductName = name,
                                    PlantID = PlantID,
                                    Status = Status,
                                    CreateDate = DateTime.Now,
                                    CreateBy = EmpID,//User.Identity.Name;
                                    UpdateDate = DateTime.Now,
                                    UpdateBy = EmpID//User.Identity.Name;

                                };
                                db.TbProduct.Add(newData);
                                db.SaveChanges();
                            }
                           
                        }
                    }

                    db.SaveChanges();
                }

            }

            return Json(new { success = true, message = "Data imported and updated successfully!" });

        }



        // End Master Product
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////



        /// <summary>
        /// Master Section
        /// </summary>
        /// <returns></returns>

        public ActionResult Section()
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
                tbSection = db.TbSection.OrderByDescending(x => x.Status).ToList(),
            };


            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            ViewBag.VBRoleSection = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(8)).Select(x => x.RoleAction).FirstOrDefault();

            return View(mymodel);

         

        }

        //2. Function Section show filler information
        [HttpGet]
        public ActionResult Section(TbSection obj, bool? inactivestatus)
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
                tbSection = db.TbSection.OrderByDescending(x => x.Status).ThenBy(x => x.SectionID).ToList(),
            };


            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            if (!string.IsNullOrEmpty(obj.SectionName) || !string.IsNullOrEmpty(obj.SectionID) || inactivestatus != null)
            {
                           
                if(!string.IsNullOrEmpty(obj.SectionName))
                {
                    mymodel.tbSection = mymodel.tbSection.Where(p => p.SectionName.Equals(obj.SectionName)).OrderByDescending(x => x.Status);

                    ViewBag.SelectedSectionName = obj.SectionName;
                }
                if(!string.IsNullOrEmpty(obj.SectionID))
                {
                    mymodel.tbSection = mymodel.tbSection.Where(p => p.SectionID.Equals(obj.SectionID)).OrderByDescending(x => x.Status);
                    ViewBag.SelectedSectionID = obj.SectionID;
                }
                if (inactivestatus == true)
                {
                    mymodel.tbSection = mymodel.tbSection.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    mymodel.tbSection = mymodel.tbSection.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }



                //sectiondb = sectiondb.Where(p => p.SectionName.Equals(obj.SectionName) || p.SectionID.Equals(obj.SectionID)).OrderByDescending(x => x.Status);
                ViewBag.VBRoleSection = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(8)).Select(x => x.RoleAction).FirstOrDefault();

                return View(mymodel);
            }
            else
            {

                ViewBag.VBRoleSection = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(8)).Select(x => x.RoleAction).FirstOrDefault();
                ViewBag.InactiveStatus = true;
                return View(mymodel);

            }


        }

        // 3.Function Section Clear fillter
        public ActionResult SectionClear()
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
                tbSection = db.TbSection.OrderByDescending(x => x.Status).ThenBy(x => x.SectionID).ToList(),
            };


            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            ViewBag.InactiveStatus = true;
            return RedirectToAction("Section");

        }



        //4. Function Section Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult SectionEdit(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbSection = db.TbSection.OrderByDescending(x => x.Status).ThenBy(x => x.SectionID).ToList(),
            };


            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }



            var Section = mymodel.tbSection.Where(x=>x.SectionID.Equals(id.PadLeft(5, '0'))).SingleOrDefault();
            return Json(Section);

        }


        // 5. Function Section Update Transaction
        [HttpPost]
        public ActionResult SectionUpdate(TbSection obj)
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
                tbSection = db.TbSection.OrderByDescending(x => x.Status).ThenBy(x => x.SectionID).ToList(),
            };


            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }

            var Sectiondb = mymodel.tbSection.Where(x => x.SectionID == obj.SectionID).SingleOrDefault();

            if (obj.SectionName != null)
            {
                Sectiondb.SectionName = obj.SectionName;

            }
            if (obj.Delaytime != null)
            {
             
                    Sectiondb.Delaytime = obj.Delaytime;
               
            }
            if (obj.Status != null)
            {

                Sectiondb.Status = obj.Status;

            }

            Sectiondb.UpdateBy = EmpID;// User.Identity.Name;
            Sectiondb.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Section");

        }


        // 6. Function Section Create transaction
        [HttpPost]
        public ActionResult SectionCreate(TbSection obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            //Check Duplicate
            var cntsection = db.TbSection.Select(x => x.SectionID).Max();
            string cntsectionString = cntsection.ToString();
            string sectiononly = cntsectionString.Substring(cntsectionString.Length - 5);
            int nextcntsection = Convert.ToInt32(sectiononly) + 1;

            var Sectiondb = db.TbSection.Where(p => p.SectionName.Equals(obj.SectionName) && p.PlantID.Equals(PlantID) && p.Status.Equals(1));
            if (Sectiondb.Count() == 0)
            {
                // Insert new Plant

                db.TbSection.Add(new TbSection()
                {

                    SectionID = PlantID.ToString() + nextcntsection.ToString().PadLeft(5,'0'),
                    SectionName = obj.SectionName,
                    Delaytime = obj.Delaytime,
                    PlantID = PlantID,
                    Status = obj.Status,
                    CreateDate = DateTime.Now,
                    CreateBy = EmpID,//User.Identity.Name,
                    UpdateDate = DateTime.Now,
                    UpdateBy = EmpID,//User.Identity.Name,
                });
                db.SaveChanges();
               
            }
            else
            {
                TempData["AlertMessage"] = "Section Duplicate!";
                ViewBag.Error = "Section Duplicate!";
            }
            return RedirectToAction("Section");
        }

        // 7. Function Section Active transaction
        public JsonResult SectionActive(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbSection = db.TbSection.OrderByDescending(x => x.Status).ThenBy(x => x.SectionID).ToList(),
            };


            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            var Sectiondb = mymodel.tbSection.Where(p => p.SectionID.Equals(id.PadLeft(5, '0'))).SingleOrDefault();
            if (Sectiondb != null)
            {
                Sectiondb.Status = 1;
                Sectiondb.UpdateBy = EmpID;// User.Identity.Name;
                Sectiondb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbSection);

        }

        // 7. Function Section Inactive transaction
        public JsonResult SectionInactive(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbSection = db.TbSection.OrderByDescending(x => x.Status).ThenBy(x => x.SectionID).ToList(),
            };


            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }



            var Sectiondb = mymodel.tbSection.Where(p => p.SectionID.Equals(id.PadLeft(5, '0'))).SingleOrDefault();
            if (Sectiondb != null)
            {
                Sectiondb.Status = 0;
                Sectiondb.UpdateBy = EmpID;// User.Identity.Name;
                Sectiondb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbSection);

        }



        //[HttpPost]
        //public IActionResult SectionProcessDataTable([FromBody] List<Dictionary<string, string>> tableData)
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
        //        tbSection = db.TbSection.Where(x=>x.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
        //    };

        //    try
        //    {
        //        // Create a DataTable
        //        DataTable dataTable = new DataTable("Section");

        //        // Add headers to the DataTable
        //        //var headers = tableData.First().Keys.ToList();
        //        var headers = tableData.First().Keys.Take(4).ToList();
        //        foreach (var header in headers)
        //        {
        //            dataTable.Columns.Add(header.Trim());

        //        }

        //        // Add data to the DataTable
        //        foreach (var rowData in tableData)
        //        {
        //            var dataRow = dataTable.NewRow();
        //            foreach (var header in headers)
        //            {
        //                dataRow[header.Trim()] = rowData[header].Trim();
        //            }
        //            dataTable.Rows.Add(dataRow);
        //        }

        //        using (var package = new ExcelPackage())
        //        {
        //            // Create a worksheet
        //            var worksheet = package.Workbook.Worksheets.Add("Section");

        //            // Add headers to the worksheet
        //            for (int col = 1; col <= dataTable.Columns.Count; col++)
        //            {
        //                var headername = dataTable.Columns[col - 1].ColumnName;
        //                worksheet.Cells[1, col].Value = headername;
        //            }

        //            // Add data to the worksheet
        //            for (int row = 0; row < dataTable.Rows.Count; row++)
        //            {
        //                for (int col = 1; col <= dataTable.Columns.Count; col++)
        //                {
        //                    try
        //                    {
        //                        var DataName = dataTable.Rows[row][col - 1].ToString();
        //                        worksheet.Cells[row + 2, col].Value = DataName;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Console.WriteLine($"Error accessing data at row {row}, col {col}: {ex.Message}");
        //                        // Log the error using your logging framework (e.g., Serilog, NLog)
        //                        return StatusCode(500, $"Error exporting data: {ex.Message}");
        //                    }
        //                }
        //            }

        //            var filePath = startpath + "SectionMaster.xls";
        //            System.IO.File.WriteAllBytes(filePath, package.GetAsByteArray());

        //            //// Save the Excel package to a stream
        //            // var stream = new MemoryStream(package.GetAsByteArray());
        //            //// Set the position to the beginning of the stream
        //            //stream.Position = 0;
        //            //// Return the Excel file as a FileStreamResult
        //            //File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LineReport.xlsx");
        //            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        //            return File(fileStream, "application/vnd.ms-excel", "SectionMaster.xls");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error generating Excel file: {ex.Message}");
        //        // Log the error using your logging framework (e.g., Serilog, NLog)
        //        return StatusCode(500, $"Error exporting data: {ex.Message}");
        //    }
        //}



        [HttpGet]
        public ActionResult SectionExport(TbSection obj, bool? inactivestatus)
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
                tbSection = db.TbSection.OrderByDescending(x => x.Status).ThenBy(x => x.SectionID).ToList(),
            };


            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }

            if (!string.IsNullOrEmpty(obj.SectionName) || !string.IsNullOrEmpty(obj.SectionID) || inactivestatus != null)
            {

                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    mymodel.tbSection = mymodel.tbSection.Where(p => p.SectionName.Equals(obj.SectionName)).OrderByDescending(x => x.Status);

                    ViewBag.SelectedSectionName = obj.SectionName;
                }
                if (!string.IsNullOrEmpty(obj.SectionID))
                {
                    mymodel.tbSection = mymodel.tbSection.Where(p => p.SectionID.Equals(obj.SectionID)).OrderByDescending(x => x.Status);
                    ViewBag.SelectedSectionID = obj.SectionID;
                }
                if (inactivestatus == true)
                {
                    mymodel.tbSection = mymodel.tbSection.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    mymodel.tbSection = mymodel.tbSection.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }

                var collection = mymodel.tbSection.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Section");
                Sheet.Cells["A1"].Value = "SectionID";
                Sheet.Cells["B1"].Value = "SectionName";
                Sheet.Cells["C1"].Value = "DelayTime";
                Sheet.Cells["D1"].Value = "Status";
                int row = 2;
                foreach (var item in collection)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.SectionID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.SectionName;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Delaytime;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Status;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=Section-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());


                //sectiondb = sectiondb.Where(p => p.SectionName.Equals(obj.SectionName) || p.SectionID.Equals(obj.SectionID)).OrderByDescending(x => x.Status);
                ViewBag.VBRoleSection = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(8)).Select(x => x.RoleAction).FirstOrDefault();

                return RedirectToAction("Section", mymodel);
            }
            else
            {

                var collection = mymodel.tbSection.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Section");
                Sheet.Cells["A1"].Value = "SectionID";
                Sheet.Cells["B1"].Value = "SectionName";
                Sheet.Cells["C1"].Value = "DelayTime";
                Sheet.Cells["D1"].Value = "Status";
                int row = 2;
                foreach (var item in collection)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.SectionID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.SectionName;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Delaytime;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Status;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=Section-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());

                ViewBag.VBRoleSection = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(8)).Select(x => x.RoleAction).FirstOrDefault();
                ViewBag.InactiveStatus = true;
                return RedirectToAction("Section",mymodel);

            }


        }


        [HttpPost]
        public IActionResult SectionUpload(IFormFile FileUpload)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            int CntDb = db.TbSection.ToList().Count;
            int CntDbnext = CntDb ;

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (FileUpload == null || FileUpload.Length <= 0)
            {
                ViewBag.Error = "Please select a valid Excel file.";
                return View("Section");
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
                            if (Convert.ToInt32(worksheet.Cells[row, 4].Text) != 1 && Convert.ToInt32(worksheet.Cells[row, 4].Text) != 0)
                            {
                                int rowerror = row - 1;
                                TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake please check Master ";
                                // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                                return RedirectToAction("Section");

                            }

                            var id = 0;
                            if (worksheet.Cells[row, 1].Value != null)
                            {
                                id = int.Parse(worksheet.Cells[row, 1].Value.ToString());
                            }
                            //var name = worksheet.Cells[row, 2].Text;
                        

                           // var DataDb = db.TbSection.Find(id);
                            var DataDb = db.TbSection.Where(x => x.SectionID.Equals(id.ToString().PadLeft(5, '0'))).SingleOrDefault();

                            if (DataDb != null)
                            {
                                int Status;
                                if (worksheet.Cells[row, 4].Text == "1")
                                {
                                    Status = 1;
                                }
                                else
                                {
                                    Status = 0;
                                }

                                // Update existing record
                                DataDb.SectionName = worksheet.Cells[row, 2].Text;
                                DataDb.Delaytime = worksheet.Cells[row, 3].Text;
                                DataDb.Status = Status;
                                DataDb.UpdateDate = DateTime.Now;
                                DataDb.UpdateBy = EmpID; //User.Identity.Name;
                            }
                            else
                            {

                               // int CntDb = db.TbSection.ToList().Count;
                                CntDbnext = CntDbnext + 1;
                                string sctID = Convert.ToString(PlantID) + CntDbnext.ToString().PadLeft(5, '0');
                                // Insert new record
                                var newData = new TbSection
                                {
                                    SectionID = sctID,
                                    SectionName = worksheet.Cells[row, 2].Text,
                                    Delaytime = worksheet.Cells[row, 3].Text,
                                    PlantID = PlantID,
                                    Status = 1,
                                    CreateDate = DateTime.Now,
                                    CreateBy = EmpID,//User.Identity.Name;
                                    UpdateDate = DateTime.Now,
                                    UpdateBy = EmpID//User.Identity.Name;

                                };
                                db.TbSection.Add(newData);
                            }
                            db.SaveChanges();
                        }
                    }

                   
                }

            }
            return Json(new { success = true, message = "Data imported and updated successfully!" });

        }



        // End Master Section
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////



        /// <summary>
        /// Master  Shift
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult Shift(TbShift obj, bool? inactivestatus)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.plantID = PlantID;
            var mymodel = new ViewModelAll()
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbShift = db.TbShift.OrderByDescending(x => x.Status).ToList(),
                
            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbShift = mymodel.tbShift.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }



            ViewBag.VBRoleShift = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(9)).Select(x => x.RoleAction).FirstOrDefault();

            if (!string.IsNullOrEmpty(obj.ShiftName) || obj.ShiftID != 0 || inactivestatus != null)
            {
               

                if (!string.IsNullOrEmpty(obj.ShiftName))
                {
                    mymodel.tbShift = mymodel.tbShift.Where(p => p.ShiftName.Equals(obj.ShiftName)).OrderByDescending(x => x.Status);
                    ViewBag.SelectedShiftName = obj.ShiftName;
                }
                if (obj.ShiftID != 0)
                {
                    mymodel.tbShift = mymodel.tbShift.Where(p => p.ShiftID.Equals(obj.ShiftID)).OrderByDescending(x => x.Status);
                    ViewBag.SelectedShiftID  = obj.ShiftID;
                }
                if (inactivestatus == true)
                {
                    mymodel.tbShift = mymodel.tbShift.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    mymodel.tbShift = mymodel.tbShift.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }


                // shiftdb = shiftdb.Where(p => p.ShiftName.Equals(obj.ShiftName) || p.ShiftID.Equals(obj.ShiftID));
                return View(mymodel);
            }
            else
            {
                ViewBag.InactiveStatus = true;
                return View(mymodel);

            }

        }

        public ActionResult ShiftClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll()
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbShift = db.TbShift.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbShift = mymodel.tbShift.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            ViewBag.InactiveStatus = true;
            return RedirectToAction("Shift");

        }

        //4. Function Shift Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult ShiftEdit(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var mymodel = new ViewModelAll()
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbShift = db.TbShift.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbShift = mymodel.tbShift.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }



            var Shift = mymodel.tbShift.Where(x=>x.ShiftID.Equals(id)).SingleOrDefault();
            return Json(Shift);

        }


        // 5. Function Shift Update Transaction
        [HttpPost]
        public ActionResult ShiftUpdate(TbShift obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }


            var mymodel = new ViewModelAll()
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbShift = db.TbShift.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbShift = mymodel.tbShift.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }



            var Shiftdb = mymodel.tbShift.Where(x => x.ShiftID == obj.ShiftID).SingleOrDefault();
            if (obj.Prefix != null)
            {
                Shiftdb.Prefix = obj.Prefix;
            }

            if (obj.ShiftName != null)
            {
                Shiftdb.ShiftName = obj.ShiftName;

            }
            if (obj.StartTime != null)
            {
                Shiftdb.StartTime = obj.StartTime;
            }
            if (obj.EndTime != null)
            {
                Shiftdb.EndTime = obj.EndTime;
            }
            if (obj.Status == 0)
            {
                Shiftdb.Status = 0;
            }
            else
            {
                Shiftdb.Status = 1;
            }
            Shiftdb.UpdateBy = EmpID;// User.Identity.Name;
            obj.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Shift");

        }


        // 6. Function Shift Create transaction
        [HttpPost]
        public ActionResult ShiftCreate(TbShift obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            //Check duplicate
            int cntshift = db.TbShift.Count() + 1;
            var Shiftdb = db.TbShift.Where(p => p.ShiftID.Equals(cntshift) && p.PlantID.Equals(PlantID) && p.Status.Equals(1));
            if (Shiftdb.Count() == 0)
            {
                // Insert new Shift               
                db.TbShift.Add(new TbShift()
                {
                    ShiftID = cntshift,
                    Prefix = obj.Prefix,
                    ShiftName = obj.ShiftName,
                    StartTime = obj.StartTime,
                    EndTime = obj.EndTime,
                    PlantID = PlantID,
                    Status =obj.Status,
                    CreateDate = DateTime.Today,
                    CreateBy = EmpID,//User.Identity.Name,
                    UpdateDate = DateTime.Today,
                    UpdateBy = EmpID,//User.Identity.Name,
                });
                db.SaveChanges();
            }
            else
            {
                TempData["AlertMessage"] = "Shift Duplicate!";
                ViewBag.Error = "Shift Duplicate!";
            }

            return RedirectToAction("Shift");
        }


        // 7. Function Shift Active transaction
        public JsonResult ShiftActive(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }


            var mymodel = new ViewModelAll()
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbShift = db.TbShift.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbShift = mymodel.tbShift.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            var Shiftdb = mymodel.tbShift.Where(p => p.ShiftID.Equals(id)).SingleOrDefault();
            if (Shiftdb != null)
            {
                Shiftdb.Status = 1;
                Shiftdb.UpdateBy = EmpID;//User.Identity.Name;
                Shiftdb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbShift);

        }


        // 7. Function Shift Inactive transaction
        public JsonResult ShiftInactive(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var mymodel = new ViewModelAll()
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbShift = db.TbShift.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                mymodel.tbShift = mymodel.tbShift.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }



            var Shiftdb = mymodel.tbShift.Where(p => p.ShiftID.Equals(id)).SingleOrDefault();
            if (Shiftdb != null)
            {
                Shiftdb.Status = 0;
                Shiftdb.UpdateBy = EmpID;//User.Identity.Name;
                Shiftdb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbShift);

        }



        // End Master Shift
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Master  Incentive
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult Incentive(View_PLPS obj, bool? inactivestatus)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var incentives = new ViewModelAll
            {
                view_Incentive = db.View_Incentive.OrderByDescending(x => x.Status).ToList(),
              //  tbIncentiveMaster = db.TbIncentiveMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
              //  tbPLPS = db.TbPLPS.Where(x=>x.PlantID.Equals(PlantID)).ToList(),
                view_PLPS = db.View_PLPS.OrderByDescending(x => x.Status).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                incentives.view_Incentive = incentives.view_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList();
                incentives.view_PLPS = incentives.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }



            ViewBag.VBRoleIncentive = incentives.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(10)).Select(x => x.RoleAction).FirstOrDefault();

            if (!string.IsNullOrEmpty(obj.SectionName)  || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.ProductName) || inactivestatus != null)
            {

                
                if(!string.IsNullOrEmpty(obj.SectionName))
                {
                    incentives.view_Incentive = incentives.view_Incentive.Where(x => x.SectionID.Equals(obj.SectionName)).ToList();
                    ViewBag.SelectedSectionName = obj.SectionName;
                }
              
                if (!string.IsNullOrEmpty(obj.LineName))
                { 
                    incentives.view_Incentive = incentives.view_Incentive.Where(x => x.LineName.Equals(obj.LineName)).ToList();
                    ViewBag.SelectedLineName = obj.LineName;
                }
                if (!string.IsNullOrEmpty(obj.ProductName))
                {
                    incentives.view_Incentive = incentives.view_Incentive.Where(x => x.ProductID.Equals(obj.ProductName)).ToList();
                    ViewBag.SelectedProductName = obj.ProductName;
                }
                if (inactivestatus == true)
                {
                    incentives.view_Incentive = incentives.view_Incentive.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    incentives.view_Incentive = incentives.view_Incentive.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }




                // var ViewEmpPlantName = incentives.view_Incentive.Where(x => x.IncentiveName.Equals(obj.SectionName) || x.PlantName.Equals(obj.PlantName) || x.LineName.Equals(obj.LineName) || x.ProductName.Equals(obj.ProductName)).ToList();
                incentives.view_Incentive = incentives.view_Incentive.OrderByDescending(x => x.Status); ;

                return View(incentives);

            }
            else
            {
               
                ViewBag.InactiveStatus = true;
                return View(incentives);
            }

        }

        public IActionResult FilterProductsByLine(string lineId)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            // Replace this with your logic to filter products based on the lineId

            var incentives = new ViewModelAll
            {
                view_Incentive = db.View_Incentive.ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                incentives.view_Incentive = incentives.view_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList();
                incentives.view_PLPS = incentives.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }


            var groupedProducts = incentives.view_PLPS
                 .Where(x => x.LineID.Equals(lineId))
                 .GroupBy(x => new { x.ProductID, x.ProductName })
                 .Select(group => new
                 {
                     ProductID = group.Key.ProductID,
                     ProductName = group.Key.ProductName
                 }).ToList();
            return Json(groupedProducts);
        }


        public IActionResult FilterSectionByLine(string lineId)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));

            var incentives = new ViewModelAll
            {
                view_Incentive = db.View_Incentive.ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                incentives.view_Incentive = incentives.view_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList();
                incentives.view_PLPS = incentives.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }


            var filteredProducts = incentives.view_PLPS.Where(x => x.LineID.Equals(lineId))
                .Select(x => new
                {
                    SectionID = x.SectionID,
                    SectionName = x.SectionName

                }).ToList();
            return Json(filteredProducts);
        }



        public IActionResult FilterSectionsByProduct(string productId , string lineId)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));

            var incentives = new ViewModelAll
            {
                view_Incentive = db.View_Incentive.ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                incentives.view_Incentive = incentives.view_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList();
                incentives.view_PLPS = incentives.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }

            var filteredSections = incentives.view_PLPS.Where(x => x.LineID.Equals(lineId) && x.ProductID.Equals(productId))
                     .Select(x => new
                     {
                         SectionID = x.SectionID,
                         SectionName = x.SectionName

                     }).ToList();

            return Json(filteredSections);
        }



        public IActionResult FilterSectionsByLine(string lineId)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));


            var incentives = new ViewModelAll
            {
                view_Incentive = db.View_Incentive.ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                incentives.view_Incentive = incentives.view_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList();
                incentives.view_PLPS = incentives.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }


            var filteredSections = incentives.view_PLPS.Where(x => x.LineID.Equals(lineId))
                     .Select(x => new
                     {
                         SectionID = x.SectionID,
                         SectionName = x.SectionName

                     }).ToList();

            return Json(filteredSections);
        }


        // 3.Function Incentive Clear fillter
        public ActionResult IncentiveClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var incentives = new ViewModelAll
            {
                view_Incentive = db.View_Incentive.OrderByDescending(x => x.Status).ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                incentives.view_Incentive = incentives.view_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList();
                incentives.view_PLPS = incentives.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }
            return RedirectToAction("Incentive");
           // return View(incentives);


        }


        //4. Function Incentive Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult IncentiveEdit(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var incentives = new ViewModelAll
            {
                view_Incentive = db.View_Incentive.ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
                incentives.view_Incentive = incentives.view_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList();
                incentives.view_PLPS = incentives.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }


            var Incentive = incentives.view_Incentive.Where(p=>p.IncentiveID.Equals(id.PadLeft(5,'0')) ).SingleOrDefault();
            return Json(Incentive);

        }



        // 5. Function Incentive Update Transaction
        [HttpPost]
        public ActionResult IncentiveUpdate(TbIncentiveMaster obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var incentives = new ViewModelAll
            {
                view_Incentive = db.View_Incentive.ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbIncentiveMaster = db.TbIncentiveMaster.ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {
               // incentives.view_Incentive = incentives.view_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList();
                incentives.tbIncentiveMaster = db.TbIncentiveMaster.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }


            var Incentivedb = incentives.tbIncentiveMaster.Where(x => x.IncentiveID == obj.IncentiveID ).SingleOrDefault();
            if (obj.IncentiveName != null)
            {
                Incentivedb.IncentiveName = obj.IncentiveName;

            }
            if (obj.LineID != null)
            {
                Incentivedb.LineID = obj.LineID;
            }
            if (obj.ProductID != null)
            {
                Incentivedb.ProductID = obj.ProductID;
            }
            if (obj.SectionID != null)
            {
                Incentivedb.SectionID = obj.SectionID;
            }
            if (obj.STD != null)
            {
                Incentivedb.STD = obj.STD;
            }
            if (obj.Min != null)
            {
                Incentivedb.Min = obj.Min;
            }
            if (obj.Max != null)
            {
                Incentivedb.Max = obj.Max;
            }
            if (obj.Rate != null)
            {
                Incentivedb.Rate = obj.Rate;
            }
            if (obj.Grade != null)
            {
                Incentivedb.Grade = obj.Grade;
            }
            if (obj.Status == 0)
            {
                Incentivedb.Status = 0;
            }
            else
            {
                Incentivedb.Status = 1;
            }

            Incentivedb.UpdateBy = EmpID;// User.Identity.Name;
            obj.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Incentive");

        }


        [HttpPost]
        public ActionResult IncentiveCreate(ViewModelAll obj, View_Incentive tbincentive)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            //Check Duplicate
            var plantdb = db.TbIncentiveMaster.Where(p => p.PlantID == tbincentive.PlantID && p.LineID == tbincentive.LineID && p.ProductID.Equals(tbincentive.ProductName) && p.SectionID == tbincentive.SectionID && p.Grade.Equals(tbincentive.Grade) && p.Status.Equals(1)).ToList();
            string maxIncentiveID = db.TbIncentiveMaster.Max(p => p.IncentiveID);

            // Increment the maximum IncentiveID for the new record
            int nextIncentiveID = 1;
            if (!string.IsNullOrEmpty(maxIncentiveID))
            {
                nextIncentiveID = Convert.ToInt32(maxIncentiveID) + 1;
            }
            if (plantdb.Count() == 0)
            {
                // string[] MonthYearArr = obj.MonthYear.Split('-');
                // Insert new Plant

                db.TbIncentiveMaster.Add(new TbIncentiveMaster()
                {
                    IncentiveID = nextIncentiveID.ToString().PadLeft(5, '0'),
                    IncentiveName = tbincentive.IncentiveName,
                    PlantID = PlantID ,//Convert.ToInt32(tbincentive.PlantName),
                    LineID = tbincentive.LineName,
                    ProductID =tbincentive.ProductName,
                    SectionID = tbincentive.SectionName,
                    STD = Convert.ToInt32(tbincentive.STD),
                    Min = Convert.ToDecimal(tbincentive.Min),
                    Max = Convert.ToDecimal(tbincentive.Max),
                    Rate = Math.Round(Convert.ToDecimal(tbincentive.Rate), 3),
                    Grade = Convert.ToString(tbincentive.Grade),
                    Status = Convert.ToInt32(tbincentive.Status),
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
            return RedirectToAction("Incentive");
        }

        // 7. Function Incentive IActive transaction
        public JsonResult IncentiveActive(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var incentives = new ViewModelAll
            {
                view_Incentive = db.View_Incentive.ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbIncentiveMaster = db.TbIncentiveMaster.ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {
                // incentives.view_Incentive = incentives.view_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList();
                incentives.tbIncentiveMaster = db.TbIncentiveMaster.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }

            var Incentivedb = incentives.tbIncentiveMaster.Where(p => p.IncentiveID.Equals(id.PadLeft(5, '0')) ).SingleOrDefault();
            if (Incentivedb != null)
            {

                Incentivedb.Status = 1;
                Incentivedb.UpdateBy = EmpID;// User.Identity.Name;
                Incentivedb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbIncentiveMaster);
        }



        // 7. Function Incentive Inactive transaction
        public JsonResult IncentiveInactive(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var incentives = new ViewModelAll
            {
                view_Incentive = db.View_Incentive.ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbIncentiveMaster = db.TbIncentiveMaster.ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {
                // incentives.view_Incentive = incentives.view_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList();
                incentives.tbIncentiveMaster = db.TbIncentiveMaster.Where(x => x.PlantID.Equals(PlantID)).ToList();



            }
            var Incentivedb = incentives.tbIncentiveMaster.Where(p => p.IncentiveID.Equals(id.PadLeft(5,'0')) ).SingleOrDefault();
            if (Incentivedb != null)
            {

                Incentivedb.Status = 0;
                Incentivedb.UpdateBy = EmpID;// User.Identity.Name;
                Incentivedb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbIncentiveMaster);
        }





        //[HttpPost]
        //public IActionResult IncentiveProcessDataTable([FromBody] List<Dictionary<string, string>> tableData)
        //{
        //    int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
        //    var incentives = new ViewModelAll
        //    {
        //        tbIncentiveMaster = db.TbIncentiveMaster.ToList(),
        //        tbPlants = db.TbPlant.Where(x => x.PlantID.Equals(PlantID)).ToList(),
        //        tbLine = db.TbLine.Where(x => x.PlantID.Equals(PlantID)).ToList(),
        //        tbProduct = db.TbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList(),
        //        tbSection = db.TbSection.Where(x => x.PlantID.Equals(PlantID)).ToList(),
        //        view_Incentive = db.View_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList(),
        //        view_PermissionMaster = db.View_PermissionMaster.ToList(),

        //    };

        //    try
        //    {
        //        // Create a DataTable
        //        DataTable dataTable = new DataTable("Incentive");

        //        // Add headers to the DataTable
        //        //var headers = tableData.First().Keys.ToList();
        //        var headers = tableData.First().Keys.Take(11).ToList();
        //        foreach (var header in headers)
        //        {
        //            dataTable.Columns.Add(header.Trim());

        //        }

        //        // Add data to the DataTable
        //        foreach (var rowData in tableData)
        //        {
        //            var dataRow = dataTable.NewRow();
        //            foreach (var header in headers)
        //            {
        //                dataRow[header.Trim()] = rowData[header].Trim();
        //            }
        //            dataTable.Rows.Add(dataRow);
        //        }

        //        using (var package = new ExcelPackage())
        //        {
        //            // Create a worksheet
        //            var worksheet = package.Workbook.Worksheets.Add("Incentive");

        //            // Add headers to the worksheet
        //            for (int col = 1; col <= dataTable.Columns.Count; col++)
        //            {
        //                var headername = dataTable.Columns[col - 1].ColumnName;
        //                worksheet.Cells[1, col].Value = headername;
        //            }

        //            // Add data to the worksheet
        //            for (int row = 0; row < dataTable.Rows.Count; row++)
        //            {
        //                for (int col = 1; col <= dataTable.Columns.Count; col++)
        //                {
        //                    try
        //                    {
        //                        var DataName = dataTable.Rows[row][col - 1].ToString();
        //                        worksheet.Cells[row + 2, col].Value = DataName;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Console.WriteLine($"Error accessing data at row {row}, col {col}: {ex.Message}");
        //                        // Log the error using your logging framework (e.g., Serilog, NLog)
        //                        return StatusCode(500, $"Error exporting data: {ex.Message}");
        //                    }
        //                }
        //            }


        //            var filePath = startpath + "Incentive.xls";
        //            System.IO.File.WriteAllBytes(filePath, package.GetAsByteArray());

        //            //// Save the Excel package to a stream
        //            // var stream = new MemoryStream(package.GetAsByteArray());
        //            //// Set the position to the beginning of the stream
        //            //stream.Position = 0;
        //            //// Return the Excel file as a FileStreamResult
        //            //File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LineReport.xlsx");
        //            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        //            return File(fileStream, "application/vnd.ms-excel", "Incentive.xls");

               
               
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error generating Excel file: {ex.Message}");
        //        // Log the error using your logging framework (e.g., Serilog, NLog)
        //        return StatusCode(500, $"Error exporting data: {ex.Message}");
        //    }
        //}


        [HttpGet]
        public ActionResult IncentiveExport(View_PLPS obj, bool? inactivestatus)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var incentives = new ViewModelAll
            {
                view_Incentive = db.View_Incentive.OrderByDescending(x => x.Status).ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbIncentiveMaster = db.TbIncentiveMaster.ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {
                 incentives.view_Incentive = incentives.view_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList();
                incentives.tbIncentiveMaster = db.TbIncentiveMaster.Where(x => x.PlantID.Equals(PlantID)).ToList();
                incentives.view_PLPS = incentives.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();


            }


            ViewBag.VBRoleIncentive = incentives.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(10)).Select(x => x.RoleAction).FirstOrDefault();

            if (!string.IsNullOrEmpty(obj.SectionName) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.ProductName) || inactivestatus != null)
            {


                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    incentives.view_Incentive = incentives.view_Incentive.Where(x => x.SectionName.Equals(obj.SectionName)).ToList();
                    ViewBag.SelectedSectionName = obj.SectionName;
                }

                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    incentives.view_Incentive = incentives.view_Incentive.Where(x => x.LineName.Equals(obj.LineName)).ToList();
                    ViewBag.SelectedLineName = obj.LineName;
                }
                if (!string.IsNullOrEmpty(obj.ProductName))
                {
                    incentives.view_Incentive = incentives.view_Incentive.Where(x => x.ProductName.Equals(obj.ProductName)).ToList();
                    ViewBag.SelectedProductName = obj.ProductName;
                }
                if (inactivestatus == true)
                {
                    incentives.view_Incentive = incentives.view_Incentive.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    incentives.view_Incentive = incentives.view_Incentive.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }


                var collection = incentives.view_Incentive.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("incentives");
                Sheet.Cells["A1"].Value = "IncentiveID";
                Sheet.Cells["B1"].Value = "IncentiveName";
                Sheet.Cells["C1"].Value = "LineID";
                Sheet.Cells["D1"].Value = "ProductID";
                Sheet.Cells["E1"].Value = "SectionID";
                Sheet.Cells["F1"].Value = "STD";
                Sheet.Cells["G1"].Value = "Min";
                Sheet.Cells["H1"].Value = "Max";
                Sheet.Cells["I1"].Value = "Rate";
                Sheet.Cells["J1"].Value = "Grade";
                Sheet.Cells["K1"].Value = "Status";
                int row = 2;
                foreach (var item in collection)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IncentiveID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.IncentiveName;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.LineID;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.ProductID;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.SectionID;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.STD;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Min;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Max;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.Rate;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.Grade;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.Status;
           
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=Incentive-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());




                // var ViewEmpPlantName = incentives.view_Incentive.Where(x => x.IncentiveName.Equals(obj.SectionName) || x.PlantName.Equals(obj.PlantName) || x.LineName.Equals(obj.LineName) || x.ProductName.Equals(obj.ProductName)).ToList();
                incentives.view_Incentive = incentives.view_Incentive.OrderByDescending(x => x.Status); ;

                return RedirectToAction("Incentive",incentives);

            }
            else
            {


                var collection = incentives.view_Incentive.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("incentives");
                Sheet.Cells["A1"].Value = "IncentiveID";
                Sheet.Cells["B1"].Value = "IncentiveName";
                Sheet.Cells["C1"].Value = "LineID";
                Sheet.Cells["D1"].Value = "ProductID";
                Sheet.Cells["E1"].Value = "SectionID";
                Sheet.Cells["F1"].Value = "STD";
                Sheet.Cells["G1"].Value = "Min";
                Sheet.Cells["H1"].Value = "Max";
                Sheet.Cells["I1"].Value = "Rate";
                Sheet.Cells["J1"].Value = "Grade";
                Sheet.Cells["K1"].Value = "Status";
                int row = 2;
                foreach (var item in collection)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IncentiveID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.IncentiveName;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.LineID;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.ProductID;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.SectionID;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.STD;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Min;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Max;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.Rate;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.Grade;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.Status;

                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=Incentive-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());


                ViewBag.InactiveStatus = true;
                return RedirectToAction("Incentive", incentives);
            }

        }




        [HttpPost]
        public IActionResult IncentiveUpload(IFormFile FileUpload)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (FileUpload == null || FileUpload.Length <= 0)
            {
                ViewBag.Error = "Please select a valid Excel file.";
                return View("Incentive");
            }
            int CntDb = db.TbIncentiveMaster.ToList().Count;
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

                            var id ="";
                            if (worksheet.Cells[row, 1].Value != null)
                            {
                                id =worksheet.Cells[row, 1].Value.ToString();
                            }
                            

                            var DataDb = db.TbIncentiveMaster.Where(x => x.IncentiveID.Equals(id.PadLeft(5, '0'))).SingleOrDefault();
                            var LineIDDb = db.TbLine.Where(x => x.LineID.Equals(worksheet.Cells[row, 3].Text.Trim()) && x.PlantID.Equals(PlantID)).Select(x => x.LineID).SingleOrDefault();
                            var ProductIDDb = db.TbProduct.Where(x => x.ProductID.Equals(worksheet.Cells[row, 4].Text.Trim()) && x.PlantID.Equals(PlantID)).Select(x => x.ProductID).SingleOrDefault();
                            var SectionIDDb = db.TbSection.Where(x => x.SectionID.Equals(worksheet.Cells[row, 5].Text.Trim()) && x.PlantID.Equals(PlantID)).Select(x => x.SectionID).SingleOrDefault();

                            if (LineIDDb == null || ProductIDDb == null || SectionIDDb == null && (Convert.ToInt32(worksheet.Cells[row, 11].Text) != 1 || Convert.ToInt32(worksheet.Cells[row, 11].Text) != 0))
                            {
                                int rowerror = row - 1;
                                var incentives = new ViewModelAll
                                {
                                    view_Incentive = db.View_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                                    tbIncentiveMaster = db.TbIncentiveMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                                    tbPLPS = db.TbPLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                                    view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                                    view_PermissionMaster = db.View_PermissionMaster.ToList(),

                                };
                                return Json(new { success = false, message = "Data Row : " + rowerror + " =>  Mistake please check. " });

                               // return View("Incentive", incentives);

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
                                   // TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake ";
                                   // ViewBag.MessageAlertincentive = "Data Row : " + rowerror + " =>  Mistake ";

                                    // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                                    var incentives = new ViewModelAll
                                    {
                                        view_Incentive = db.View_Incentive.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                                        tbIncentiveMaster = db.TbIncentiveMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                                        tbPLPS = db.TbPLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                                        view_PLPS = db.View_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                                        view_PermissionMaster = db.View_PermissionMaster.ToList(),

                                    };
                                    return Json(new { success = false, message = "Data Row : " + rowerror + " =>  Mistake please check. " });
                                    //return View("Incentive", incentives);
                                }
                                else
                                {


                                    if (DataDb != null)
                                    {

                                        DataDb.IncentiveName = worksheet.Cells[row, 2].Text;
                                        DataDb.PlantID = PlantID;
                                        DataDb.LineID = LineIDDb;
                                        DataDb.ProductID = ProductIDDb;
                                        DataDb.SectionID = SectionIDDb;
                                        DataDb.STD = int.Parse(worksheet.Cells[row, 6].Text);
                                        DataDb.Min = decimal.Parse(worksheet.Cells[row, 7].Text);
                                        DataDb.Max = decimal.Parse(worksheet.Cells[row, 8].Text);
                                        DataDb.Rate = decimal.Parse(worksheet.Cells[row, 9].Text);
                                        DataDb.Grade = worksheet.Cells[row, 10].Text;
                                        DataDb.Status = int.Parse(worksheet.Cells[row, 11].Text);
                                        DataDb.UpdateDate = DateTime.Now;
                                        DataDb.UpdateBy = EmpID;


                                    }
                                    else
                                    {
  
                                        CntDbnext = CntDbnext + 1;
                                      
                                        var newData = new TbIncentiveMaster
                                        {

                                            IncentiveID = Convert.ToString(CntDbnext).PadLeft(5, '0'),
                                            IncentiveName = worksheet.Cells[row, 2].Text,
                                            PlantID = PlantID,
                                            LineID = LineIDDb,
                                            ProductID = ProductIDDb,
                                            SectionID = SectionIDDb,
                                            STD = int.Parse(worksheet.Cells[row, 6].Text),
                                            Min = decimal.Parse(worksheet.Cells[row, 7].Text),
                                            Max = decimal.Parse(worksheet.Cells[row, 8].Text),
                                            Rate = decimal.Parse(worksheet.Cells[row, 9].Text),
                                            Grade = worksheet.Cells[row, 10].Text,
                                            Status = int.Parse(worksheet.Cells[row, 11].Text),
                                        CreateDate = DateTime.Now,
                                            CreateBy = EmpID,//User.Identity.Name;
                                            UpdateDate = DateTime.Now,
                                            UpdateBy = EmpID//User.Identity.Name;

                                        };






                                        db.TbIncentiveMaster.Add(newData);
                                    }
                                  
                                }
                            }
                        }
                        
                    }
                    db.SaveChanges();
                }

            }

           // ViewBag.Success = "Data imported and updated successfully!";
           // return RedirectToAction("Incentive");
            return Json(new { success = true, message = "Data imported and updated successfully!" });
        }



        //End Master Incentive
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////




        /// <summary>
        /// Master : Sevices
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult Services(View_Service obj, bool? inactivestatus)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Mymodel = new ViewModelAll
            {
                tbService = db.TbService.ToList(),
                tbSection = db.TbSection.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                view_Service = db.View_Service.ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_PLPS = db.View_PLPS.ToList()
            };


            // Check Admin
            if (PlantID != 0)
            {
                Mymodel.tbService = Mymodel.tbService.Where(x => x.PlantID.Equals(PlantID)).ToList();
                Mymodel.tbSection = Mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                Mymodel.tbPlants = Mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                Mymodel.tbLine = Mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                Mymodel.view_Service = Mymodel.view_Service.Where(x => x.PlantID.Equals(PlantID)).ToList();
                Mymodel.view_PLPS = Mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }



            ViewBag.VBRoleServices = Mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(11)).Select(x => x.RoleAction).FirstOrDefault();

            if (!string.IsNullOrEmpty(obj.ServicesName) || !string.IsNullOrEmpty(obj.PlantName) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.SectionName) || inactivestatus != null)
            {

                if(!string.IsNullOrEmpty(obj.ServicesName))
                {
                    Mymodel.view_Service = Mymodel.view_Service.Where(x => x.ServicesName.Equals(obj.ServicesName)).ToList();
                    ViewBag.SelectedServicesName = obj.ServicesName;

                }
                if (!string.IsNullOrEmpty(obj.PlantName))
                {
                    Mymodel.view_Service = Mymodel.view_Service.Where(x => x.PlantName.Equals(obj.PlantName)).ToList();
                    ViewBag.SelectedPlantName = obj.PlantName;
                }
                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    Mymodel.view_Service = Mymodel.view_Service.Where(x => x.LineName.Equals(obj.LineName)).ToList();
                    ViewBag.SelectedLineName = obj.LineName;
                }
                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    Mymodel.view_Service = Mymodel.view_Service.Where(x => x.SectionName.Equals(obj.SectionName)).ToList();
                    ViewBag.SelectedSectionName = obj.SectionName;
                }
                if (inactivestatus == true)
                {
                    Mymodel.view_Service = Mymodel.view_Service.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    Mymodel.view_Service = Mymodel.view_Service.Where(x => x.ServicesStatus == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }

               


                // Mymodel.view_Service = db.View_Service.Where(x => x.ServicesID.Equals(obj.ServicesID) || x.ServicesName.Equals(obj.ServicesName) || x.PlantName.Equals(obj.PlantName) || x.LineName.Equals(obj.LineName)).ToList();

                return View(Mymodel);


            }
            else
            {
                ViewBag.InactiveStatus = true;
                return View(Mymodel);


            }

        }

        public ActionResult ServicesClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Mymodel = new ViewModelAll
            {
                tbService = db.TbService.ToList(),
                tbSection = db.TbSection.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                view_Service = db.View_Service.ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_PLPS = db.View_PLPS.ToList()
            };


            // Check Admin
            if (PlantID != 0)
            {
                Mymodel.tbService = Mymodel.tbService.Where(x => x.PlantID.Equals(PlantID)).ToList();
                Mymodel.tbSection = Mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                Mymodel.tbPlants = Mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                Mymodel.tbLine = Mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                Mymodel.view_Service = Mymodel.view_Service.Where(x => x.PlantID.Equals(PlantID)).ToList();
                Mymodel.view_PLPS = Mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }
            ViewBag.InactiveStatus = true;
            return RedirectToAction("Services");


        }




        // Function Services Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult ServicesEdit(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var Mymodel = new ViewModelAll
            {
                //tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
              //  tbSection = db.TbSection.ToList(),
               // tbPlants = db.TbPlant.ToList(),
               // tbLine = db.TbLine.ToList(),
                view_Service = db.View_Service.ToList(),
              //  view_PermissionMaster = db.View_PermissionMaster.ToList(),
               // view_PLPS = db.View_PLPS.ToList()
            };


            // Check Admin
            if (PlantID != 0)
            {
               // Mymodel.tbSection = Mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
               // Mymodel.tbPlants = Mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
               // Mymodel.tbLine = Mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                Mymodel.view_Service = Mymodel.view_Service.Where(x => x.PlantID.Equals(PlantID)).ToList();
               // Mymodel.view_PLPS = Mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }


            var service = Mymodel.view_Service.Where(x=>x.ServicesID.Equals(id.PadLeft(5, '0'))).SingleOrDefault();
            return Json(service);
        }


        // Function Services Update Transaction
        [HttpPost]
        public ActionResult ServicesUpdate(TbService obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }


            var Mymodel = new ViewModelAll
            {
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //  tbSection = db.TbSection.ToList(),
                // tbPlants = db.TbPlant.ToList(),
                // tbLine = db.TbLine.ToList(),
                //view_Service = db.View_Service.ToList(),
                //  view_PermissionMaster = db.View_PermissionMaster.ToList(),
                // view_PLPS = db.View_PLPS.ToList()
            };


            // Check Admin
            if (PlantID != 0)
            {

                 Mymodel.tbService = Mymodel.tbService.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.tbSection = Mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.tbPlants = Mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.tbLine = Mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.view_Service = Mymodel.view_Service.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.view_PLPS = Mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }



            var Servicedb = Mymodel.tbService.Where(x => x.ServicesID == obj.ServicesID).SingleOrDefault();
            if (obj.ServicesName != null)
            {
                Servicedb.ServicesName = obj.ServicesName;

            }
            if (obj.LineID != null)
            {
                Servicedb.LineID = obj.LineID;
            }

            if (obj.SectionID != null)
            {
                Servicedb.SectionID = obj.SectionID;
            }


            if (obj.ServicesRate != null)
            {
                Servicedb.ServicesRate = obj.ServicesRate;
            }

            if (obj.ServicesStatus != null)
            {
                if (obj.ServicesStatus == 1)
                {
                    Servicedb.ServicesStatus = 1;
                }
                else
                {
                    Servicedb.ServicesStatus = 0;
                }
            }

            Servicedb.UpdateBy = EmpID;
            obj.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Services");

        }


        // Function Services Create transaction
        [HttpPost]
        public ActionResult ServicesCreate(View_Service obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            int cntdb = db.TbService.Count()+1;
            //Check Duplicate

            var Mymodel = new ViewModelAll
            {
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //  tbSection = db.TbSection.ToList(),
                // tbPlants = db.TbPlant.ToList(),
                // tbLine = db.TbLine.ToList(),
                //view_Service = db.View_Service.ToList(),
                //  view_PermissionMaster = db.View_PermissionMaster.ToList(),
                // view_PLPS = db.View_PLPS.ToList()
            };


            // Check Admin
            if (PlantID != 0)
            {

                Mymodel.tbService = Mymodel.tbService.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.tbSection = Mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.tbPlants = Mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.tbLine = Mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.view_Service = Mymodel.view_Service.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.view_PLPS = Mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }

            var servicedb = Mymodel.tbService.Where(p => p.ServicesName.Equals(obj.ServicesName)  && p.LineID.Equals(obj.LineName) && p.ServicesStatus.Equals(1));
            if (servicedb.Count() == 0)
            {
                // Insert new Service
                db.TbService.Add(new TbService()
                {
                   ServicesID = cntdb.ToString().PadLeft(5,'0'),
                    ServicesName = obj.ServicesName,
                    PlantID = PlantID,//Convert.ToInt32(obj.PlantName),
                    LineID = obj.LineName,
                    SectionID = obj.SectionName,
                    ServicesRate = obj.ServicesRate,
                    ServicesStatus = Convert.ToInt32(obj.ServicesStatus),
                    CreateDate = DateTime.Today,
                    CreateBy = EmpID,//User.Identity.Name,
                    UpdateDate = DateTime.Today,
                    UpdateBy = EmpID,//User.Identity.Name,
                });
                db.SaveChanges();

            }
            else
            {TempData["AlertMessage"] = "Services Duplicate!";
                ViewBag.Error = "Services Duplicate!";
            }
            return RedirectToAction("Services");
        }

        // Function Services Inactive transaction
        public JsonResult ServicesInactive(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }


            var Mymodel = new ViewModelAll
            {
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //  tbSection = db.TbSection.ToList(),
                // tbPlants = db.TbPlant.ToList(),
                // tbLine = db.TbLine.ToList(),
                //view_Service = db.View_Service.ToList(),
                //  view_PermissionMaster = db.View_PermissionMaster.ToList(),
                // view_PLPS = db.View_PLPS.ToList()
            };


            // Check Admin
            if (PlantID != 0)
            {

                Mymodel.tbService = Mymodel.tbService.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.tbSection = Mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.tbPlants = Mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.tbLine = Mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.view_Service = Mymodel.view_Service.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.view_PLPS = Mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }

            var servicedb = Mymodel.tbService.Where(p => p.ServicesID.Equals(id.PadLeft(5, '0')) ).SingleOrDefault();
            if (servicedb != null)
            {
                servicedb.ServicesStatus = 0;
                servicedb.UpdateBy = EmpID;//User.Identity.Name;
                servicedb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.View_Service);

        }


        // Function Services Inactive transaction
        public JsonResult ServicesActive(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var Mymodel = new ViewModelAll
            {
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //  tbSection = db.TbSection.ToList(),
                // tbPlants = db.TbPlant.ToList(),
                // tbLine = db.TbLine.ToList(),
                //view_Service = db.View_Service.ToList(),
                //  view_PermissionMaster = db.View_PermissionMaster.ToList(),
                // view_PLPS = db.View_PLPS.ToList()
            };


            // Check Admin
            if (PlantID != 0)
            {

                Mymodel.tbService = Mymodel.tbService.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.tbSection = Mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.tbPlants = Mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.tbLine = Mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.view_Service = Mymodel.view_Service.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // Mymodel.view_PLPS = Mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }

            var servicedb = Mymodel.tbService.Where(p => p.ServicesID.Equals(id.PadLeft(5, '0'))).SingleOrDefault();
            if (servicedb != null)
            {
                servicedb.ServicesStatus = 1;
                servicedb.UpdateBy = EmpID;//User.Identity.Name;
                servicedb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.View_Service);

        }


        //[HttpPost]
        //public IActionResult ServicesProcessDataTable([FromBody] List<Dictionary<string, string>> tableData)
        //{
        //    int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
        //    var ServicesRecord = new ViewModelAll
        //    {
        //        tbService = db.TbService.Where(x=>x.PlantID.Equals(PlantID)).ToList(),
        //        tbPlants = db.TbPlant.ToList(),
        //        tbLine = db.TbLine.ToList(),
        //        view_Service = db.View_Service.Where(p=>p.PlantID.Equals(PlantID)).OrderByDescending(x => x.ServicesStatus).ToList(),
        //        view_PermissionMaster = db.View_PermissionMaster.ToList(),

        //    };

        //    try
        //    {
        //        // Create a DataTable
        //        DataTable dataTable = new DataTable("Services");

        //        // Add headers to the DataTable
        //        //var headers = tableData.First().Keys.ToList();
        //        var headers = tableData.First().Keys.Take(6).ToList();
        //        foreach (var header in headers)
        //        {
        //            dataTable.Columns.Add(header.Trim());

        //        }

        //        // Add data to the DataTable
        //        foreach (var rowData in tableData)
        //        {
        //            var dataRow = dataTable.NewRow();
        //            foreach (var header in headers)
        //            {
        //                dataRow[header.Trim()] = rowData[header].Trim();
        //            }
        //            dataTable.Rows.Add(dataRow);
        //        }

        //        using (var package = new ExcelPackage())
        //        {
        //            // Create a worksheet
        //            var worksheet = package.Workbook.Worksheets.Add("Services");

        //            // Add headers to the worksheet
        //            for (int col = 1; col <= dataTable.Columns.Count; col++)
        //            {
        //                var headername = dataTable.Columns[col - 1].ColumnName;
        //                worksheet.Cells[1, col].Value = headername;
        //            }

        //            // Add data to the worksheet
        //            for (int row = 0; row < dataTable.Rows.Count; row++)
        //            {
        //                for (int col = 1; col <= dataTable.Columns.Count; col++)
        //                {
        //                    try
        //                    {
        //                        var DataName = dataTable.Rows[row][col - 1].ToString();
        //                        worksheet.Cells[row + 2, col].Value = DataName;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Console.WriteLine($"Error accessing data at row {row}, col {col}: {ex.Message}");
        //                        // Log the error using your logging framework (e.g., Serilog, NLog)
        //                        return StatusCode(500, $"Error exporting data: {ex.Message}");
        //                    }
        //                }
        //            }


        //            var filePath = startpath + "Services.xls";
        //            System.IO.File.WriteAllBytes(filePath, package.GetAsByteArray());

        //            //// Save the Excel package to a stream
        //            // var stream = new MemoryStream(package.GetAsByteArray());
        //            //// Set the position to the beginning of the stream
        //            //stream.Position = 0;
        //            //// Return the Excel file as a FileStreamResult
        //            //File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LineReport.xlsx");
        //            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        //            return File(fileStream, "application/vnd.ms-excel", "Services.xls");



        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error generating Excel file: {ex.Message}");
        //        // Log the error using your logging framework (e.g., Serilog, NLog)
        //        return StatusCode(500, $"Error exporting data: {ex.Message}");
        //    }
        //}


        //public IActionResult ServicesDownloadExcel()
        //{

        //    var data = db.TbService;
        //    // Create a new Excel package
        //    using (var package = new ExcelPackage())
        //    {
        //        // Add a worksheet to the package
        //        var worksheet = package.Workbook.Worksheets.Add("Service");

        //        // Add headers
        //        worksheet.Cells["A1"].Value = "ServicesID";
        //        worksheet.Cells["B1"].Value = "ServicesName";
        //        worksheet.Cells["C1"].Value = "PlantID";
        //        worksheet.Cells["D1"].Value = "LineID";
        //        worksheet.Cells["E1"].Value = "ServicesRate";
        //        worksheet.Cells["F1"].Value = "ServiceStatus";


        //        // Add data rows
        //        int row = 2;
        //        foreach (var item in data)
        //        {
        //            worksheet.Cells[$"A{row}"].Value = item.ServicesID;
        //            worksheet.Cells[$"B{row}"].Value = item.ServicesName;
        //            worksheet.Cells[$"C{row}"].Value = item.Ser_PlantID;
        //            worksheet.Cells[$"D{row}"].Value = item.Ser_LineID;
        //            worksheet.Cells[$"E{row}"].Value = item.ServicesRate;
        //            worksheet.Cells[$"F{row}"].Value = item.ServicesStatus;
        //            row++;
        //        }

        //        // Save the Excel package to a stream
        //        var stream = new MemoryStream(package.GetAsByteArray());

        //        // Set the position to the beginning of the stream
        //        stream.Position = 0;

        //        // Return the Excel file as a FileStreamResult
        //        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ServiceReport.xlsx");
        //    }
        //}



        [HttpGet]
        public ActionResult ServicesExport(View_Service obj, bool? inactivestatus)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var Mymodel = new ViewModelAll
            {
                tbService = db.TbService.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                view_Service = db.View_Service.ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_PLPS = db.View_PLPS.ToList()
            };

            // Check Admin
            if (PlantID != 0)
            {

                Mymodel.tbService = Mymodel.tbService.Where(x => x.PlantID.Equals(PlantID)).ToList();
                 Mymodel.tbSection = Mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                 Mymodel.tbPlants = Mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                 Mymodel.tbLine = Mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                 Mymodel.view_Service = Mymodel.view_Service.Where(x => x.PlantID.Equals(PlantID)).ToList();
                 Mymodel.view_PLPS = Mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }

            ViewBag.VBRoleServices = Mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(11)).Select(x => x.RoleAction).FirstOrDefault();

            if (!string.IsNullOrEmpty(obj.ServicesName) || !string.IsNullOrEmpty(obj.PlantName) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.SectionName) || inactivestatus != null)
            {

                if (!string.IsNullOrEmpty(obj.ServicesName))
                {
                    Mymodel.view_Service = Mymodel.view_Service.Where(x => x.ServicesName.Equals(obj.ServicesName)).ToList();
                    ViewBag.SelectedServicesName = obj.ServicesName;

                }
                if (!string.IsNullOrEmpty(obj.PlantName))
                {
                    Mymodel.view_Service = Mymodel.view_Service.Where(x => x.PlantName.Equals(obj.PlantName)).ToList();
                    ViewBag.SelectedPlantName = obj.PlantName;
                }
                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    Mymodel.view_Service = Mymodel.view_Service.Where(x => x.LineName.Equals(obj.LineName)).ToList();
                    ViewBag.SelectedLineName = obj.LineName;
                }
                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    Mymodel.view_Service = Mymodel.view_Service.Where(x => x.SectionName.Equals(obj.SectionName)).ToList();
                    ViewBag.SelectedSectionName = obj.SectionName;
                }
                if (inactivestatus == true)
                {
                    Mymodel.view_Service = Mymodel.view_Service.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    Mymodel.view_Service = Mymodel.view_Service.Where(x => x.ServicesStatus == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }


                var collection = Mymodel.view_Service.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Services");
                Sheet.Cells["A1"].Value = "ServicesID";
                Sheet.Cells["B1"].Value = "ServicesName";
                Sheet.Cells["C1"].Value = "LineID";
                Sheet.Cells["D1"].Value = "SectionID";
                Sheet.Cells["E1"].Value = "ServicesRate";
                Sheet.Cells["F1"].Value = "ServicesStatus";
                int row = 2;
                foreach (var item in collection)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.ServicesID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.ServicesName;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.LineID;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.SectionID;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.ServicesRate;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.ServicesStatus;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=Services-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());



                // Mymodel.view_Service = db.View_Service.Where(x => x.ServicesID.Equals(obj.ServicesID) || x.ServicesName.Equals(obj.ServicesName) || x.PlantName.Equals(obj.PlantName) || x.LineName.Equals(obj.LineName)).ToList();

                return RedirectToAction("Services",Mymodel);


            }
            else
            {


                var collection = Mymodel.view_Service.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Services");
                Sheet.Cells["A1"].Value = "ServicesID";
                Sheet.Cells["B1"].Value = "ServicesName";
                Sheet.Cells["C1"].Value = "LineID";
                Sheet.Cells["D1"].Value = "SectionID";
                Sheet.Cells["E1"].Value = "ServicesRate";
                Sheet.Cells["F1"].Value = "ServicesStatus";
                int row = 2;
                foreach (var item in collection)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.ServicesID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.ServicesName;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.LineID;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.SectionID;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.ServicesRate;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.ServicesStatus;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=Services-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());
                ViewBag.InactiveStatus = true;
                return RedirectToAction("Services", Mymodel);


            }

        }




        [HttpPost]
        public IActionResult ServicesUpload(IFormFile FileUpload)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            int CntDb = db.TbService.ToList().Count;
            int CntDbnext = CntDb ;

            if (FileUpload == null || FileUpload.Length <= 0)
            {
                TempData["AlertMessage"] = "Please select a valid Excel file.";
               // ViewBag.Error = "Please select a valid Excel file.";
                return View("Services");
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
                            if (Convert.ToInt32(worksheet.Cells[row, 6].Text) != 1 && Convert.ToInt32(worksheet.Cells[row,6].Text) != 0)
                            {
                                int rowerror = row - 1;
                                TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake please check Master ";
                                // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                                return RedirectToAction("Services");

                            }

                            string id ="" ;
                            if (worksheet.Cells[row, 1].Value != null)
                            {
                                id = worksheet.Cells[row, 1].Value.ToString();
                            }

                           // var DataDb = db.TbService.Find(id);
                            var DataDb = db.TbService.Where(x => x.ServicesID.Equals(id.PadLeft(5, '0'))).SingleOrDefault();
                            //checked Line
                           // var LineDb = db.TbLine.Where(x => x.LineName.Equals(worksheet.Cells[row, 3].Text.Trim())).Select(x => x.LineID).SingleOrDefault();
                           ////check section
                           // var SectionDb = db.TbSection.Where(x => x.SectionName.Equals(worksheet.Cells[row, 4].Text.Trim())).Select(x => x.SectionID).SingleOrDefault();

                           // if (LineDb == null || SectionDb == null)
                           // {
                           //     int rowerror = row - 1;
                           //     TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake ";
                           //     // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                           //     return RedirectToAction("Services");

                           // }


                            var LineIDDb = db.TbLine.Where(x => x.LineID.Equals(worksheet.Cells[row, 3].Text.Trim()) && x.PlantID.Equals(PlantID)).Select(x => x.LineID).SingleOrDefault();
                            //check section
                            var SectionIDDb = db.TbSection.Where(x => x.SectionID.Equals(worksheet.Cells[row, 4].Text.Trim()) && x.PlantID.Equals(PlantID)).Select(x => x.SectionID).SingleOrDefault();

                            if (LineIDDb == null || SectionIDDb == null)
                            {
                                int rowerror = row - 1;
                               // TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake ";
                                // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                                //return RedirectToAction("Services");
                                return Json(new { success = false, message = "Data Row : " + rowerror + " =>  Mistake Please check!"});

                                }


                            if (DataDb != null)
                            {

                                DataDb.ServicesName = worksheet.Cells[row, 2].Text;
                                DataDb.PlantID = PlantID;
                                DataDb.LineID = LineIDDb; //worksheet.Cells[row, 3].Text.PadLeft(5, '0');
                                DataDb.SectionID = SectionIDDb; // worksheet.Cells[row, 4].Text.PadLeft(5, '0');
                                DataDb.ServicesRate = decimal.Parse(worksheet.Cells[row, 5].Text);
                                DataDb.ServicesStatus = int.Parse(worksheet.Cells[row, 6].Text); 
                                DataDb.UpdateDate = DateTime.Now;
                                DataDb.UpdateBy = EmpID; //User.Identity.Name;
                            }
                            else
                            {

                               // int CntDb = db.TbService.ToList().Count;
                                 CntDbnext = CntDbnext + 1;
                                // Insert new record
                                var newData = new TbService
                                {
                                    // Insert existing record
                                    ServicesID = CntDbnext.ToString().PadLeft(5, '0'),
                                    ServicesName = worksheet.Cells[row, 2].Text,
                                    PlantID = PlantID,
                                    LineID = LineIDDb, //worksheet.Cells[row, 3].Text.PadLeft(5, '0'),
                                    SectionID = SectionIDDb ,// worksheet.Cells[row, 4].Text.PadLeft(5, '0'),
                                    ServicesRate = decimal.Parse(worksheet.Cells[row, 5].Text),
                                    ServicesStatus = int.Parse(worksheet.Cells[row, 6].Text),
                                    CreateDate = DateTime.Now,
                                    CreateBy = EmpID,//User.Identity.Name;
                                    UpdateDate = DateTime.Now,
                                    UpdateBy = EmpID//User.Identity.Name;

                                };
                                db.TbService.Add(newData);
                            }
                        }
                    }

                    db.SaveChanges();
                }

            }
            return Json(new { success = true, message = "Data imported and updated successfully!" });

           // ViewBag.Success = "Data imported and updated successfully!";
           // return RedirectToAction("Services");

        }


        // End Master Services
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Reason Management
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Reason(View_Reason obj, bool? inactivestatus)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var reason = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbReason = db.TbReason.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                tbProduct = db.TbProduct.ToList(),
                tbSection = db.TbSection.ToList(),
                view_Reason = db.View_Reason.OrderByDescending(x => x.Status).ToList(),
                view_PLPS = db.View_PLPS.ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {
                reason.view_PLPS = reason.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();
                reason.view_Reason = reason.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                reason.tbReason = reason.tbReason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                reason.tbProduct = reason.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
                reason.tbPlants = reason.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                reason.tbLine = reason.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                reason.tbSection = reason.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                reason.view_PLPS = reason.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }

            ViewBag.VBRoleReason = reason.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(12)).Select(x => x.RoleAction).FirstOrDefault();

            if (!string.IsNullOrEmpty(obj.PlantName) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.ProductName) || !string.IsNullOrEmpty(obj.SectionName) || inactivestatus != null)
            {
  

                if(!string.IsNullOrEmpty(obj.PlantName))
                {
                    reason.view_Reason = reason.view_Reason.Where(p => p.PlantName.Equals(obj.PlantName));
                    ViewBag.SelectedPlantName = obj.PlantName;

                }
                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    reason.view_Reason = reason.view_Reason.Where(p => p.LineName.Equals(obj.LineName));
                    ViewBag.SelectedLineName = obj.LineName;
                }
                if (!string.IsNullOrEmpty(obj.ProductName))
                {
                    reason.view_Reason = reason.view_Reason.Where(p => p.ProductName.Equals(obj.ProductName));

                    ViewBag.SelectedProductName = obj.ProductName;
                }
                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    reason.view_Reason = reason.view_Reason.Where(p => p.SectionName.Equals(obj.SectionName));
                    ViewBag.SelectedSectionName = obj.SectionName;
                }
                if (inactivestatus == true)
                {
                    reason.view_Reason = reason.view_Reason.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    reason.view_Reason = reason.view_Reason.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }



                // reason.view_Reason = reason.view_Reason.Where(p => p.PlantName.Equals(obj.PlantName) || p.LineName.Equals(obj.LineName) || p.ProductName.Equals(obj.ProductName) || p.SectionName.Equals(obj.SectionName));
                return View(reason);
            }
            else
            {

                ViewBag.InactiveStatus = true;
                return View(reason);

            }

        }


        // 3. Function Plant Create transaction
        [HttpPost]
        public ActionResult ReasonCreate(View_Reason obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            //Check Duplicate
            int reasoncnt = db.TbReason.Count() + 1;
            var plantdb = db.TbReason.Where(p => p.ReasonID.Equals(obj.ReasonID) && p.PlantID.Equals(PlantID));
            if (plantdb.Count() == 0)
            {
                // Insert new Plant               
                db.TbReason.Add(new TbReason()
                {
                   ReasonID = reasoncnt.ToString().PadLeft(5,'0'),
                    ReasonName = obj.ReasonName,
                    PlantID = PlantID,
                    LineID = obj.LineID,
                    SectionID =obj.SectionID,
                    ProductID = obj.ProductID,
                    Status = obj.Status,
                    CreateDate = DateTime.Now,
                    CreateBy = EmpID ,//userdb.UserEmpID
                    UpdateDate = DateTime.Now,
                    UpdateBy = EmpID //userdb.UserEmpID
                });
                db.SaveChanges();

            }
            else
            {TempData["AlertMessage"] = "Reason Duplicate!";
                ViewBag.Error = "Reason Duplicate!";
            }
            return RedirectToAction("Reason");
        }


        // 4. Function Plant Clear Fillter
        public ActionResult ReasonClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var reason = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbReason = db.TbReason.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                tbProduct = db.TbProduct.ToList(),
                tbSection = db.TbSection.ToList(),
                view_Reason = db.View_Reason.OrderByDescending(x => x.Status).ToList(),
                view_PLPS = db.View_PLPS.ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {
                reason.view_PLPS = reason.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();
                reason.view_Reason = reason.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                reason.tbReason = reason.tbReason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                reason.tbProduct = reason.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
                reason.tbPlants = reason.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                reason.tbLine = reason.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                reason.tbSection = reason.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                reason.view_PLPS = reason.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();

            }
            ViewBag.InactiveStatus = true;
            return RedirectToAction("Reason");

        }

        // 5.  Function Plant Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult ReasonEdit(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var reason = new ViewModelAll
            {
                view_Reason = db.View_Reason.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {
              
                reason.view_Reason = reason.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            var Reason = db.View_Reason.Where(p => p.ReasonID.Equals(id.PadLeft(5, '0'))).SingleOrDefault();
           
               
            return Json(Reason);
        }

        // 6. Function Plant Update transaction
        [HttpPost]
        public ActionResult ReasonUpdate(View_Reason obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var reason = new ViewModelAll
            {
                view_Reason = db.View_Reason.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {

                reason.view_Reason = reason.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }

            var Reasondb = db.TbReason.Where(x => x.ReasonID.Equals(obj.ReasonID)).SingleOrDefault();

            if (obj.ReasonName != null)
            {
                Reasondb.ReasonName = obj.ReasonName;
            }

            if (obj.LineID != null)
            {
                Reasondb.LineID = obj.LineID;
            }
            if (obj.ProductID != null)
            {
                Reasondb.ProductID = obj.ProductID;
            }
            if (obj.SectionID != null)
            {
                Reasondb.SectionID = obj.SectionID;
            }
            if (obj.Status == 0)
            {
                Reasondb.Status = 0;
            }
            else
            {
                Reasondb.Status = 1;
            }

            //    if (obj.Status == 1) { Reasondb.Status = 1; } else { Reasondb.Status = 0; };
            Reasondb.UpdateBy = EmpID;// User.Identity.Name;
            Reasondb.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Reason");

        }

        // 7. Function Plant Active transaction
        public JsonResult ReasonActive(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var reason = new ViewModelAll
            {
                view_Reason = db.View_Reason.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {

                reason.view_Reason = reason.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }

            var Reasondb = db.TbReason.Where(p => p.ReasonID.Equals(id.PadLeft(5,'0'))).SingleOrDefault();
            if (Reasondb != null)
            {
                Reasondb.Status = 1;
                Reasondb.UpdateBy = EmpID;// User.Identity.Name;
                Reasondb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbReason);

        }


        // 7. Function Plant Inactive transaction
        public JsonResult ReasonInactive(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var reason = new ViewModelAll
            {
                view_Reason = db.View_Reason.OrderByDescending(x => x.Status).ToList(),

            };

            // Check Admin
            if (PlantID != 0)
            {

                reason.view_Reason = reason.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }

            var Reasondb = db.TbReason.Where(p => p.ReasonID.Equals(id.PadLeft(5,'0')) ).SingleOrDefault();
            if (Reasondb != null)
            {
                Reasondb.Status = 0;
                Reasondb.UpdateBy = EmpID;// User.Identity.Name;
                Reasondb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbReason);

        }

        //////End Reason Management
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// ProductSTD
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        public ActionResult ProductSTD(View_ProductSTD obj, bool? inactivestatus)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
               // tbProductSTD = db.TbProductSTD.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                tbProduct = db.TbProduct.ToList(),
                tbSection = db.TbSection.ToList(),
                view_ProductSTD = db.View_ProductSTD.OrderByDescending(x => x.Status).ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

              //  mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            ViewBag.VBRoleProducSTD = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(13)).Select(x => x.RoleAction).FirstOrDefault();

            if (!string.IsNullOrEmpty(obj.PlantName) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.ProductName) || !string.IsNullOrEmpty(obj.SectionName) || inactivestatus != null)
            {
             

                if(!string.IsNullOrEmpty(obj.PlantName))
                {
                    mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.PlantName == obj.PlantName).ToList();
                    ViewBag.SelectedPlantName = obj.PlantName;


                }
                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.LineName == obj.LineName).ToList();
                    ViewBag.SelectedLineName = obj.LineName;
                }

                if (!string.IsNullOrEmpty(obj.ProductName))
                {
                    mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.ProductName == obj.ProductName).ToList();
                    ViewBag.SelectedProductName = obj.ProductName;
                }

                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.SectionName == obj.SectionName).ToList();
                    ViewBag.SelectedSectionName = obj.SectionName;
                }
                if (inactivestatus == true)
                {
                    mymodel.view_ProductSTD = mymodel.view_ProductSTD.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }
                //  mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.PlantName == obj.PlantName || x.LineName == obj.LineName || x.ProductName == obj.ProductName || x.SectionName == obj.SectionName).ToList();


                return View(mymodel);
            }
            else
            {
                ViewBag.InactiveStatus = true;
                return View(mymodel);

            }
        }


        // 3. Function  Create transaction
        [HttpPost]
        public ActionResult ProductSTDCreate(View_ProductSTD obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            //Check Duplicate

            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                 tbProductSTD = db.TbProductSTD.ToList(),
               // tbPlants = db.TbPlant.ToList(),
               // tbLine = db.TbLine.ToList(),
              //  tbProduct = db.TbProduct.ToList(),
               // tbSection = db.TbSection.ToList(),
                view_ProductSTD = db.View_ProductSTD.OrderByDescending(x => x.Status).ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

               // mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
               // mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
               // mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
              //  mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
               // mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbProductSTD = mymodel.tbProductSTD.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }



            var PLPSdb = mymodel.view_ProductSTD.Where(p => p.ProductSTDID.Equals(obj.ProductSTDID) );
            string MaxValProdSTD = db.TbProductSTD.Select(x=>x.ProductSTDID).Max();
            int NextcountProdSTD = Convert.ToInt16(MaxValProdSTD);

            var Productstddb = mymodel.tbProductSTD.Where(p => p.LineID.Equals(obj.LineName) && p.SectionID.Equals(obj.SectionName) && p.ProductID.Equals(obj.ProductName) && p.Status == 1).ToList();
            //var userdb = db.TbUser.Where(x => x.ID.Equals(User.Identity.Name)).SingleOrDefault();

            if (Productstddb.Count() == 0)
            {
                NextcountProdSTD = NextcountProdSTD + 1;
                // Insert new Plant               
                db.TbProductSTD.Add(new TbProductSTD()
                {
                    ProductSTDID = (NextcountProdSTD).ToString().PadLeft(5, '0'),
                    PlantID = PlantID,
                    LineID = obj.LineName,
                    SectionID = obj.SectionName,
                    ProductID = obj.ProductName,
                    Unit = obj.Unit,
                    STD = Convert.ToDecimal(obj.STD),
                    PercentSTD = Convert.ToDecimal(obj.PercentSTD),
                    PercentYield = Convert.ToDecimal(obj.PercentYield),
                    YieldIncentive = Convert.ToDecimal(obj.YieldIncentive),
                    EFFSTD = Convert.ToDecimal(obj.EFFSTD),
                    Status = Convert.ToInt32(obj.Status),
                    CreateDate = DateTime.Now,
                    CreateBy = EmpID,//userdb.UserEmpID,
                    UpdateDate = DateTime.Now,
                    UpdateBy = EmpID//userdb.UserEmpID
                }); ; ;
                db.SaveChanges();

            }
            else
            {
                TempData["AlertMessage"] = "Standard Duplicate!";
                ViewBag.Error = "Standard Duplicate!";
            }
            return RedirectToAction("ProductSTD");
        }


        // 4. Function Plant Clear Fillter
        public ActionResult ProductSTDClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                 tbProductSTD = db.TbProductSTD.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                tbProduct = db.TbProduct.ToList(),
                tbSection = db.TbSection.ToList(),
                view_ProductSTD = db.View_ProductSTD.OrderByDescending(x => x.Status).ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

              //  mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbProductSTD = mymodel.tbProductSTD.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }
            ViewBag.InactiveStatus = true;
            return RedirectToAction("ProductSTD");

        }

        // 5.  Function Plant Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult ProductSTDEdit(string id)
        {

            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var mymodel = new ViewModelAll
            {
                //view_PermissionMaster = db.View_PermissionMaster.ToList(),
                //tbProductSTD = db.TbProductSTD.ToList(),
                //tbPlants = db.TbPlant.ToList(),
                //tbLine = db.TbLine.ToList(),
                //tbProduct = db.TbProduct.ToList(),
                //tbSection = db.TbSection.ToList(),
                view_ProductSTD = db.View_ProductSTD.OrderByDescending(x => x.Status).ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

                //mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbProductSTD = mymodel.tbProductSTD.Where(x => x.PlantID.Equals(PlantID));
            }


            var Reason = mymodel.view_ProductSTD.Where(p => p.ProductSTDID.Equals(id.PadLeft(5, '0')) ).SingleOrDefault();
            return Json(Reason);

           
        }

        // 7. Function Plant Inactive transaction
        public JsonResult ProductSTDActive(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                //view_PermissionMaster = db.View_PermissionMaster.ToList(),
                //tbProductSTD = db.TbProductSTD.ToList(),
                //tbPlants = db.TbPlant.ToList(),
                //tbLine = db.TbLine.ToList(),
                //tbProduct = db.TbProduct.ToList(),
                //tbSection = db.TbSection.ToList(),
                view_ProductSTD = db.View_ProductSTD.OrderByDescending(x => x.Status).ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

                //mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbProductSTD = mymodel.tbProductSTD.Where(x => x.PlantID.Equals(PlantID));
            }

            var ProductSTDdb = mymodel.view_ProductSTD.Where(p => p.ProductSTDID.Equals(id.PadLeft(5,'0')) ).SingleOrDefault();
            if (ProductSTDdb != null)
            {
                ProductSTDdb.Status = 1;
                ProductSTDdb.UpdateBy = EmpID;//User.Identity.Name;
                ProductSTDdb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbPLPS);


        }
        // 7. Function Plant Inactive transaction
        public JsonResult ProductSTDInactive(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var mymodel = new ViewModelAll
            {
                //view_PermissionMaster = db.View_PermissionMaster.ToList(),
                //tbProductSTD = db.TbProductSTD.ToList(),
                //tbPlants = db.TbPlant.ToList(),
                //tbLine = db.TbLine.ToList(),
                //tbProduct = db.TbProduct.ToList(),
                //tbSection = db.TbSection.ToList(),
                view_ProductSTD = db.View_ProductSTD.OrderByDescending(x => x.Status).ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

                //mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbProductSTD = mymodel.tbProductSTD.Where(x => x.PlantID.Equals(PlantID));
            }

            var ProductSTDdb = mymodel.view_ProductSTD.Where(p => p.ProductSTDID.Equals(id.PadLeft(5, '0')) ).SingleOrDefault();
            if (ProductSTDdb != null)
            {
                ProductSTDdb.Status = 0;
                ProductSTDdb.UpdateBy = EmpID;//User.Identity.Name;
                ProductSTDdb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbPLPS);

        }


        // 6. Function Plant Update transaction
        [HttpPost]
        public ActionResult ProductSTDUpdate(View_ProductSTD obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var mymodel = new ViewModelAll
            {
                //view_PermissionMaster = db.View_PermissionMaster.ToList(),
                //tbProductSTD = db.TbProductSTD.ToList(),
                //tbPlants = db.TbPlant.ToList(),
                //tbLine = db.TbLine.ToList(),
                //tbProduct = db.TbProduct.ToList(),
                //tbSection = db.TbSection.ToList(),
                view_ProductSTD = db.View_ProductSTD.OrderByDescending(x => x.Status).ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

                //mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbProductSTD = mymodel.tbProductSTD.Where(x => x.PlantID.Equals(PlantID));
            }

            var PLPSdb = mymodel.view_ProductSTD.Where(x => x.ProductSTDID == obj.ProductSTDID ).SingleOrDefault();
            if( PLPSdb != null)
            {

                PLPSdb.PlantID = PlantID;
                PLPSdb.LineID =obj.LineID;
                PLPSdb.ProductID = obj.ProductID;
                PLPSdb.SectionID =obj.SectionID;
                PLPSdb.STD = obj.STD;
                PLPSdb.PercentSTD = obj.PercentSTD;
                PLPSdb.PercentYield = obj.PercentYield;
                PLPSdb.YieldIncentive = obj.YieldIncentive;
                PLPSdb.EFFSTD = obj.EFFSTD;
                PLPSdb.Unit = obj.Unit;
                 if (obj.Status == 1) 
                { 
                    PLPSdb.Status = 1; 
                } 
                else 
                { 
                    PLPSdb.Status = 0; 
                }
                PLPSdb.UpdateBy = EmpID;//User.Identity.Name;
                PLPSdb.UpdateDate = DateTime.Now;
                db.SaveChanges(); 
            }
            return RedirectToAction("ProductSTD");

        }



        public ActionResult ProductSTDExport(View_ProductSTD obj, bool? inactivestatus)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbProductSTD = db.TbProductSTD.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                tbProduct = db.TbProduct.ToList(),
                tbSection = db.TbSection.ToList(),
                view_ProductSTD = db.View_ProductSTD.OrderByDescending(x => x.Status).ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

               // mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbProductSTD = mymodel.tbProductSTD.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }

            ViewBag.VBRoleProducSTD = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(13)).Select(x => x.RoleAction).FirstOrDefault();

            if (!string.IsNullOrEmpty(obj.PlantName) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.ProductName) || !string.IsNullOrEmpty(obj.SectionName) || inactivestatus != null)
            {


                if (!string.IsNullOrEmpty(obj.PlantName))
                {
                    mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.PlantName == obj.PlantName).ToList();
                    ViewBag.SelectedPlantName = obj.PlantName;


                }
                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.LineName == obj.LineName).ToList();
                    ViewBag.SelectedLineName = obj.LineName;
                }

                if (!string.IsNullOrEmpty(obj.ProductName))
                {
                    mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.ProductName == obj.ProductName).ToList();
                    ViewBag.SelectedProductName = obj.ProductName;
                }

                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.SectionName == obj.SectionName).ToList();
                    ViewBag.SelectedSectionName = obj.SectionName;
                }
                if (inactivestatus == true)
                {
                    mymodel.view_ProductSTD = mymodel.view_ProductSTD.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }
                //  mymodel.view_ProductSTD = mymodel.view_ProductSTD.Where(x => x.PlantName == obj.PlantName || x.LineName == obj.LineName || x.ProductName == obj.ProductName || x.SectionName == obj.SectionName).ToList();

                var collection = mymodel.view_ProductSTD.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("ProductSTD");
                Sheet.Cells["A1"].Value = "ProductSTDID";
                Sheet.Cells["B1"].Value = "LineID";
                Sheet.Cells["C1"].Value = "ProductID";
                Sheet.Cells["D1"].Value = "SectionID";
                Sheet.Cells["E1"].Value = "Unit";
                Sheet.Cells["F1"].Value = "STD";
                Sheet.Cells["G1"].Value = "%STD";
                Sheet.Cells["H1"].Value = "%Yield";
                Sheet.Cells["I1"].Value = "YieldIncentive";
                Sheet.Cells["J1"].Value = "EFFSTD";
                Sheet.Cells["K1"].Value = "Status";
                int row = 2;
                foreach (var item in collection)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.ProductSTDID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.LineID;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.ProductID;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.SectionID;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Unit;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.STD;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.PercentSTD;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.PercentYield;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.YieldIncentive;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.EFFSTD;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.Status;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=ProductSTD-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());

                return RedirectToAction("ProductSTD",mymodel);
            }
            else
            {

                var collection = mymodel.view_ProductSTD.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("ProductSTD");
                Sheet.Cells["A1"].Value = "ProductSTDID";
                Sheet.Cells["B1"].Value = "LineID";
                Sheet.Cells["C1"].Value = "ProductID";
                Sheet.Cells["D1"].Value = "SectionID";
                Sheet.Cells["E1"].Value = "Unit";
                Sheet.Cells["F1"].Value = "STD";
                Sheet.Cells["G1"].Value = "%STD";
                Sheet.Cells["H1"].Value = "%Yield";
                Sheet.Cells["I1"].Value = "YieldIncentive";
                Sheet.Cells["J1"].Value = "EFFSTD";
                Sheet.Cells["K1"].Value = "Status";
                int row = 2;
                foreach (var item in collection)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.ProductSTDID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.LineID;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.ProductID;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.SectionID;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Unit;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.STD;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.PercentSTD;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.PercentYield;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.YieldIncentive;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.EFFSTD;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.Status;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=ProductSTD-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());


                ViewBag.InactiveStatus = true;
                return RedirectToAction("ProductSTD", mymodel);

            }
        }



        [HttpPost]
        public IActionResult ProductSTDUpload(IFormFile FileUpload)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            if (FileUpload == null || FileUpload.Length <= 0)
            {
                ViewBag.Error = "Please select a valid Excel file.";
                return View("ProductSTD");
            }

            var CntDbnextfirst = db.TbProductSTD.Select(x => x.ProductSTDID).ToList().Max();
            int CntDbnext = Convert.ToInt16(CntDbnextfirst);

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

                            var id = "";
                            if (worksheet.Cells[row, 1].Value != null)
                            {
                                id = worksheet.Cells[row, 1].Value.ToString();
                            }


                           string prodStdID = worksheet.Cells[row, 1].Text;

                            string Linevar =  worksheet.Cells[row, 2].Text;

                            string[] Productvar = worksheet.Cells[row, 3].Text.Split(":");

                            string Sectionvar = worksheet.Cells[row, 4].Text;

                           //var DataDb = db.TbProductSTD.Where(x => x.PlantID.Equals(Plantvar) && x.LineID.Equals(Linevar) && x.SectionID.Equals(Sectionvar) && x.ProductID.Equals(Productvar[0])).SingleOrDefault();
                            //check line

                           // var LineDb = db.TbLine.Where(x => x.LineName.Equals(Linevar)).Select(x => x.LineID).SingleOrDefault();
                            //string[] pdcode = worksheet.Cells[row, 2].Text.Split(":");
                            //var ProductDb = db.TbProduct.Where(x => x.ProductID.Equals(Productvar[0])).Select(x => x.ProductID).SingleOrDefault();
                            //var SectionDb = db.TbSection.Where(x => x.SectionName.Equals(Sectionvar)).Select(x => x.SectionID).SingleOrDefault();
                            //var DataDb = db.TbProductSTD.Where(x => x.PlantID.Equals(PlantID) && x.LineID.Equals(LineDb) && x.SectionID.Equals(SectionDb) && x.ProductID.Equals(ProductDb)).SingleOrDefault();

                            //if (LineDb == null || ProductDb == null || SectionDb == null)
                            //{
                            //    int rowerror = row - 1;
                            //    TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake ";
                            //    // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                            //    return RedirectToAction("ProductSTD");

                            //}

                            var LineIDDb = db.TbLine.Where(x => x.LineID.Equals(worksheet.Cells[row,2].Text) && x.PlantID.Equals(PlantID)).Select(x => x.LineID).SingleOrDefault();
                            var ProductIDDb = db.TbProduct.Where(x => x.ProductID.Equals(worksheet.Cells[row, 3].Text) && x.PlantID.Equals(PlantID)).Select(x => x.ProductID).SingleOrDefault();
                            var SectionIDDb = db.TbSection.Where(x => x.SectionID.Equals(worksheet.Cells[row, 4].Text) && x.PlantID.Equals(PlantID)).Select(x => x.SectionID).SingleOrDefault();
                            var DataDb = db.TbProductSTD.Where(x => x.ProductSTDID.Equals(prodStdID) && x.PlantID.Equals(PlantID) && x.LineID.Equals(LineIDDb) && x.SectionID.Equals(SectionIDDb) && x.ProductID.Equals(ProductIDDb)).SingleOrDefault();

                            if (LineIDDb == null || ProductIDDb == null || SectionIDDb == null && (Convert.ToInt32(worksheet.Cells[row, 9].Text) != 1 || Convert.ToInt32(worksheet.Cells[row, 9].Text) != 0))
                            {
                                int rowerror = row - 1;
                               // TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake ";
                                // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                               // return RedirectToAction("ProductSTD");
                                return Json(new { success = false, message = "Data Row : " + rowerror + " =>  Mistake please check. " });

                            }


                            else
                            {


                                if (DataDb != null)
                                {

                                    int Status;
                                    //if (worksheet.Cells[row, 8].Text == "Active")
                                    //{
                                    //    Status = 1;
                                    //}
                                    //else
                                    //{
                                    //    Status = 0;
                                    //}

                                    // case code
                                    // Update existing record
                                    //DataDb.ProductSTDID = worksheet.Cells[row, 2].Text;
                                    DataDb.PlantID = PlantID;
                                    DataDb.LineID = LineIDDb;
                                    DataDb.ProductID = ProductIDDb;
                                    DataDb.SectionID = SectionIDDb;
                                    DataDb.Unit = worksheet.Cells[row, 5].Text;
                                    DataDb.STD = decimal.Parse(worksheet.Cells[row, 6].Text);
                                    DataDb.PercentSTD = decimal.Parse(worksheet.Cells[row, 7].Text);
                                    DataDb.PercentYield = decimal.Parse(worksheet.Cells[row, 8].Text);
                                    DataDb.YieldIncentive = decimal.Parse(worksheet.Cells[row, 9].Text);
                                    DataDb.EFFSTD = decimal.Parse(worksheet.Cells[row,10].Text);
                                    DataDb.Status = int.Parse(worksheet.Cells[row, 11].Text);
                                    DataDb.UpdateDate = DateTime.Now;
                                    DataDb.UpdateBy = EmpID;


                                }
                                else
                                {

                                   // int CntDb = db.TbProductSTD.ToList().Count;
                                     CntDbnext = CntDbnext + 1;
                                  //  int Status;
                                    //if (worksheet.Cells[row, 8].Text == "Active")
                                    //{
                                    //    Status = 1;
                                    //}
                                    //else
                                    //{
                                    //    Status = 0;
                                    //}
                                    //Data Code
                                    // Insert new record
                                    var newData = new TbProductSTD
                                    {

                                        ProductSTDID = Convert.ToString(CntDbnext).PadLeft(5, '0'),
                                        PlantID = PlantID,
                                        LineID = LineIDDb,
                                        ProductID = ProductIDDb,
                                        SectionID = SectionIDDb,
                                        STD = decimal.Parse(worksheet.Cells[row, 6].Text),
                                        Unit = worksheet.Cells[row,5].Text,
                                        PercentSTD = decimal.Parse(worksheet.Cells[row,7].Text),
                                        PercentYield = decimal.Parse(worksheet.Cells[row,8].Text),
                                        YieldIncentive = decimal.Parse(worksheet.Cells[row, 9].Text),
                                        EFFSTD = decimal.Parse(worksheet.Cells[row, 10].Text),
                                        Status = int.Parse(worksheet.Cells[row, 11].Text),
                                        CreateDate = DateTime.Now,
                                        CreateBy = EmpID,//User.Identity.Name;
                                        UpdateDate = DateTime.Now,
                                        UpdateBy = EmpID//User.Identity.Name;

                                    };


                                    db.TbProductSTD.Add(newData);
                                    // }
                                

                                }
                            }
                        }

                    }
                    db.SaveChanges();
                }

            }

            //ViewBag.Success = "Data imported and updated successfully!";
           // return RedirectToAction("ProductSTD");
            return Json(new { success = true, message = "Data imported and updated successfully!" });

        }



        /////// End Product STD Management
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// PLPS
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        public ActionResult PLPS(View_PLPS obj, bool? inactivestatus)
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
                tbPLPS = db.TbPLPS.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                tbProduct = db.TbProduct.ToList(),
                tbSection = db.TbSection.ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                tbFormular = db.TbFormular.ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

                // mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbPLPS = mymodel.tbPLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }



            ViewBag.VBRolePLPS = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(14)).Select(x => x.RoleAction).FirstOrDefault();


            if (!string.IsNullOrEmpty(obj.PlantName) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.ProductName) || !string.IsNullOrEmpty(obj.SectionName) || inactivestatus != null)
            {


                if(!string.IsNullOrEmpty(obj.PlantName))
                {
                    mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantName == obj.PlantName ).ToList();
                    ViewBag.SelectedPlantName = obj.PlantName;
                }
                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.LineName == obj.LineName).ToList();
                    ViewBag.SelectedLineName = obj.LineName;
                }
                if (!string.IsNullOrEmpty(obj.ProductName))
                {
                    mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.ProductName == obj.ProductName).ToList();
                    ViewBag.SelectedProductName = obj.ProductName;
                }
                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.SectionName == obj.SectionName).ToList();
                    ViewBag.SelectedSectionName = obj.SectionName;
                }
                if (inactivestatus == true)
                {
                    mymodel.view_PLPS = mymodel.view_PLPS.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }


                // mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantName == obj.PlantName || x.LineName == obj.LineName || x.ProductName == obj.ProductName || x.SectionName == obj.SectionName).ToList();


                return View(mymodel);
            }
            else
            {
                ViewBag.InactiveStatus = true;
                return View(mymodel);
            }


        }


        [HttpPost]
        public ActionResult PLPSCreate(View_PLPS obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var mymodel = new ViewModelAll
            {
              //  view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPLPS = db.TbPLPS.ToList(),
              //  tbPlants = db.TbPlant.ToList(),
              //  tbLine = db.TbLine.ToList(),
              //  tbProduct = db.TbProduct.ToList(),
              //  tbSection = db.TbSection.ToList(),
               view_PLPS = db.View_PLPS.ToList(),
              //  tbFormular = db.TbFormular.ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

                // mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
             //   mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
              //  mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
              //  mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
              //  mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbPLPS = mymodel.tbPLPS.Where(x => x.PlantID.Equals(PlantID));
            }



            //Check Duplicate
            var PLPSdb = db.View_PLPS.Where(p => p.PlantID.Equals(PlantID) && p.LineID.Equals(obj.LineID) && p.SectionID.Equals(obj.SectionID) && p.Status == 1).ToList();

            string MaxValPLPS = db.TbPLPS.Select(x => x.PLPSID).Max();
            int NextcountPLPS = Convert.ToInt16(MaxValPLPS);


            //   var userdb = db.TbUser.Where(x => x.ID.Equals(User.Identity.Name)).SingleOrDefault();
            if (PLPSdb.Count() == 0)
            {
                NextcountPLPS = NextcountPLPS + 1;
                // Insert new Plant               
                db.TbPLPS.Add(new TbPLPS()
                {
                    PLPSID = (NextcountPLPS).ToString().PadLeft(5,'0'),
                    PlantID = PlantID,
                    LineID = obj.LineName,
                    SectionID = obj.SectionName,
                    ProductID = obj.ProductName,
                    Size= "0",
                    QTYPerQRCode = Convert.ToInt32(obj.QTYPerQRCode),
                    Unit = obj.Unit,
                    FormularID = obj.FormularID,
                    Status = Convert.ToInt32(obj.Status),
                    CreateDate = DateTime.Now,
                    CreateBy = EmpID,//userdb.UserEmpID,
                    UpdateDate = DateTime.Now,
                    UpdateBy = EmpID//userdb.UserEmpID
                });
                db.SaveChanges();

            }
            else
            {
                TempData["AlertMessage"] = "Standard Duplicate!";
                ViewBag.Error = "Standard Duplicate!";
            }
            return RedirectToAction("PLPS");
        }


        public ActionResult PLPSClear()
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
                tbPLPS = db.TbPLPS.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                tbProduct = db.TbProduct.ToList(),
                tbSection = db.TbSection.ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                tbFormular = db.TbFormular.ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

                // mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbPLPS = mymodel.tbPLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }

            ViewBag.InactiveStatus = true;
            return RedirectToAction("PLPS");

        }

        // 5.  Function Plant Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult PLPSEdit(string ID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var mymodel = new ViewModelAll
            {
                //view_PermissionMaster = db.View_PermissionMaster.ToList(),
              //  tbPLPS = db.TbPLPS.ToList(),
               // tbPlants = db.TbPlant.ToList(),
               // tbLine = db.TbLine.ToList(),
               // tbProduct = db.TbProduct.ToList(),
               // tbSection = db.TbSection.ToList(),
                view_PLPS = db.View_PLPS.ToList(),
               // tbFormular = db.TbFormular.ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

                // mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
               // mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
               // mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
               // mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
               // mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //   mymodel.tbPLPS = mymodel.tbPLPS.Where(x => x.PlantID.Equals(PlantID));
            }


            var PLPS = db.View_PLPS.Where(p => p.PLPSID.Equals(ID.PadLeft(5, '0'))).SingleOrDefault();
            return Json(PLPS);
        }

        // 6. Function Plant Update transaction
        [HttpPost]
        public ActionResult PLPSUpdate(View_PLPS obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var mymodel = new ViewModelAll
            {
                //view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPLPS = db.TbPLPS.ToList(),
                // tbPlants = db.TbPlant.ToList(),
                // tbLine = db.TbLine.ToList(),
                // tbProduct = db.TbProduct.ToList(),
                // tbSection = db.TbSection.ToList(),
               // view_PLPS = db.View_PLPS.ToList(),
                // tbFormular = db.TbFormular.ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

                // mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
               // mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID));
                mymodel.tbPLPS = mymodel.tbPLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            var PLPSdb = db.TbPLPS.Where(x => x.PLPSID == obj.PLPSID ).SingleOrDefault();

            if (obj.ProductID != null)
            {
                PLPSdb.ProductID = obj.ProductID;
            }
            if (obj.LineID != null)
            {
                PLPSdb.LineID = obj.LineID;
            }
            if (obj.ProductID != null)
            {
                PLPSdb.ProductID = obj.ProductID;
            }
            if (obj.SectionID != null)
            {
                PLPSdb.SectionID = obj.SectionID;
            }

            //if(obj.Size != null)
            //{
            //    PLPSdb.Size = obj.Size;
            //}
            if (obj.Unit != null)
            {
                PLPSdb.Unit = obj.Unit;
            }
            if (obj.FormularID != null)
            {
                PLPSdb.FormularID = obj.FormularID;
            }
            if (obj.QTYPerQRCode != null)
            {
                PLPSdb.QTYPerQRCode = Convert.ToInt32(obj.QTYPerQRCode);
            }

            if (obj.Status == 1) 
            { 
                PLPSdb.Status = 1; 
            } 
            else 
            { 
                PLPSdb.Status = 0; 
            }
            PLPSdb.UpdateBy = EmpID;//User.Identity.Name;
            PLPSdb.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("PLPS");

        }


        // 7. Function Plant Inactive transaction
        public JsonResult PLPSActive(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var mymodel = new ViewModelAll
            {
                //view_PermissionMaster = db.View_PermissionMaster.ToList(),
                //  tbPLPS = db.TbPLPS.ToList(),
                // tbPlants = db.TbPlant.ToList(),
                // tbLine = db.TbLine.ToList(),
                // tbProduct = db.TbProduct.ToList(),
                // tbSection = db.TbSection.ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                // tbFormular = db.TbFormular.ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

                // mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //   mymodel.tbPLPS = mymodel.tbPLPS.Where(x => x.PlantID.Equals(PlantID));
            }


            var PLPSdb = db.View_PLPS.Where(p => p.PLPSID.Equals(id.PadLeft(5,'0')) ).SingleOrDefault();
            if (PLPSdb != null)
            {
                PLPSdb.Status = 1;
                PLPSdb.UpdateBy = EmpID;//User.Identity.Name;
                PLPSdb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbPLPS);



        }
        // 7. Function Plant Inactive transaction
        public JsonResult PLPSInactive(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var mymodel = new ViewModelAll
            {
                //view_PermissionMaster = db.View_PermissionMaster.ToList(),
                //  tbPLPS = db.TbPLPS.ToList(),
                // tbPlants = db.TbPlant.ToList(),
                // tbLine = db.TbLine.ToList(),
                // tbProduct = db.TbProduct.ToList(),
                // tbSection = db.TbSection.ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                // tbFormular = db.TbFormular.ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

                // mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
                // mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //   mymodel.tbPLPS = mymodel.tbPLPS.Where(x => x.PlantID.Equals(PlantID));
            }

            var PLPSdb = db.View_PLPS.Where(p => p.PLPSID.Equals(id.PadLeft(5,'0'))).SingleOrDefault();
            if (PLPSdb != null)
            {
                PLPSdb.Status = 0;
                PLPSdb.UpdateBy = EmpID;//User.Identity.Name;
                PLPSdb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbPLPS);

        }


        public ActionResult PLPSExport(View_PLPS obj, bool? inactivestatus)
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
                tbPLPS = db.TbPLPS.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                tbProduct = db.TbProduct.ToList(),
                tbSection = db.TbSection.ToList(),
                view_PLPS = db.View_PLPS.ToList(),
                tbFormular = db.TbFormular.ToList()

            };

            // Check Admin
            if (PlantID != 0)
            {

                // mymodel.view_Reason = mymodel.view_Reason.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbProduct = mymodel.tbProduct.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbPLPS = mymodel.tbPLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            ViewBag.VBRolePLPS = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(14)).Select(x => x.RoleAction).FirstOrDefault();


            if (!string.IsNullOrEmpty(obj.PlantName) || !string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.ProductName) || !string.IsNullOrEmpty(obj.SectionName) || inactivestatus != null)
            {


                if (!string.IsNullOrEmpty(obj.PlantName))
                {
                    mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantName == obj.PlantName).ToList();
                    ViewBag.SelectedPlantName = obj.PlantName;
                }
                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.LineName == obj.LineName).ToList();
                    ViewBag.SelectedLineName = obj.LineName;
                }
                if (!string.IsNullOrEmpty(obj.ProductName))
                {
                    mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.ProductName == obj.ProductName).ToList();
                    ViewBag.SelectedProductName = obj.ProductName;
                }
                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.SectionName == obj.SectionName).ToList();
                    ViewBag.SelectedSectionName = obj.SectionName;
                }
                if (inactivestatus == true)
                {
                    mymodel.view_PLPS = mymodel.view_PLPS.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }


                var collection = mymodel.view_PLPS.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("PLPS");
                Sheet.Cells["A1"].Value = "PLPSID";
                Sheet.Cells["B1"].Value = "LineID";
                Sheet.Cells["C1"].Value = "ProductID";
                Sheet.Cells["D1"].Value = "SectionID";
                Sheet.Cells["E1"].Value = "QTYPerQRCode";
                Sheet.Cells["F1"].Value = "Unit";
                Sheet.Cells["G1"].Value = "FormularID";
                Sheet.Cells["H1"].Value = "Status";
                int row = 2;
                foreach (var item in collection)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.PLPSID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.LineID;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.ProductID;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.SectionID;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.QTYPerQRCode;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Unit;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.FormularID;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Status;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=PLPS-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());



                // mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantName == obj.PlantName || x.LineName == obj.LineName || x.ProductName == obj.ProductName || x.SectionName == obj.SectionName).ToList();


                return RedirectToAction("PLPS",mymodel);
            }
            else
            {

                var collection = mymodel.view_PLPS.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("PLPS");
                Sheet.Cells["A1"].Value = "PLPSID";
                Sheet.Cells["B1"].Value = "LineID";
                Sheet.Cells["C1"].Value = "ProductID";
                Sheet.Cells["D1"].Value = "SectionID";
                Sheet.Cells["E1"].Value = "QTYPerQRCode";
                Sheet.Cells["F1"].Value = "Unit";
                Sheet.Cells["G1"].Value = "FormularID";
                Sheet.Cells["H1"].Value = "Status";
                int row = 2;
                foreach (var item in collection)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.PLPSID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.LineID;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.ProductID;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.SectionID;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.QTYPerQRCode;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Unit;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.FormularID;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Status;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=PLPS-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());



                ViewBag.InactiveStatus = true;

                return RedirectToAction("PLPS", mymodel);
            }


        }




        [HttpPost]
        public IActionResult PLPSUpload(IFormFile FileUpload)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            if (FileUpload == null || FileUpload.Length <= 0)
            {
                ViewBag.Error = "Please select a valid Excel file.";
                return View("PLPS");
            }

            var CntDbnextfirst = db.TbPLPS.Select(x=>x.PLPSID).ToList().Max();
            int CntDbnext = Convert.ToInt16(CntDbnextfirst);
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

                            var id = "";
                            if (worksheet.Cells[row, 1].Value != null)
                            {
                                id = worksheet.Cells[row, 1].Value.ToString();
                            }


                            string PLPSvar = worksheet.Cells[row, 1].Text;

                            string Linevar = worksheet.Cells[row, 2].Text;
                            // string[] Productvar = worksheet.Cells[row, 2].Text.Split(":");
                            string Productvar = worksheet.Cells[row, 3].Text;
                            string Sectionvar = worksheet.Cells[row, 4].Text;
                            
                            //check line

                            //var LineDb = db.TbLine.Where(x => x.LineName.Equals(Linevar)).Select(x => x.LineID).SingleOrDefault();
                            ////Check Product
                            //string[] pdcode = worksheet.Cells[row, 2].Text.Split(":");
                            //var ProductDb = db.TbProduct.Where(x => x.ProductID.Equals(Productvar[0])).Select(x => x.ProductID).SingleOrDefault();
                            ////check section
                            //var SectionDb = db.TbSection.Where(x => x.SectionName.Equals(Sectionvar)).Select(x => x.SectionID).SingleOrDefault();
                            //var DataDb = db.TbPLPS.Where(x => x.PlantID.Equals(PlantID) && x.LineID.Equals(LineDb) && x.SectionID.Equals(SectionDb) && x.ProductID.Equals(ProductDb)).SingleOrDefault();

                            //if (LineDb == null || ProductDb == null || SectionDb == null)
                            //{
                            //    int rowerror = row - 1;
                            //    TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake ";
                            //    // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                            //    return RedirectToAction("PLPS");

                            //}


                            var LineDb = db.TbLine.Where(x => x.LineID.Equals(Linevar) && x.PlantID.Equals(PlantID)).Select(x => x.LineID).SingleOrDefault();
                            //Check Product
                            var ProductDb = db.TbProduct.Where(x => x.ProductID.Equals(Productvar) && x.PlantID.Equals(PlantID)).Select(x => x.ProductID).SingleOrDefault();
                            //check section
                            var SectionDb = db.TbSection.Where(x => x.SectionID.Equals(Sectionvar) && x.PlantID.Equals(PlantID)).Select(x => x.SectionID).SingleOrDefault();
                            var DataDb = db.TbPLPS.Where(x => x.PLPSID.Equals(PLPSvar) && x.PlantID.Equals(PlantID) && x.LineID.Equals(LineDb) && x.SectionID.Equals(SectionDb) && x.ProductID.Equals(ProductDb)).SingleOrDefault();

                            if (LineDb == null || ProductDb == null || SectionDb == null && (Convert.ToInt32(worksheet.Cells[row, 8].Text) != 1 || Convert.ToInt32(worksheet.Cells[row, 8].Text) != 0))
                            {
                                int rowerror = row - 1;
                                // TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake ";
                                // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                                // return RedirectToAction("PLPS");
                                return Json(new { success = false, message = "Data Row : " + rowerror + " =>  Mistake please check. " });
                            }
                            else
                            {


                                if (DataDb != null)
                                {

                                    //int Status;
                                    //if (worksheet.Cells[row, 7].Text == "Active")
                                    //{
                                    //    Status = 1;
                                    //}
                                    //else
                                    //{
                                    //    Status = 0;
                                    //}

                                    // case code
                                    // Update existing record
                                    //DataDb.ProductSTDID = worksheet.Cells[row, 2].Text;
                                    DataDb.LineID = LineDb;
                                    DataDb.ProductID = ProductDb;
                                    DataDb.SectionID = SectionDb;

                                    DataDb.Size = "0";// worksheet.Cells[row, 4].Text;
                                    DataDb.QTYPerQRCode = Convert.ToInt32(worksheet.Cells[row, 5].Text);
                                    DataDb.Unit = worksheet.Cells[row, 6].Text;
                                    DataDb.FormularID = Convert.ToInt32(worksheet.Cells[row,7].Text);
                                    DataDb.Status = Convert.ToInt32(worksheet.Cells[row, 8].Text);
                                    DataDb.UpdateDate = DateTime.Now;
                                    DataDb.UpdateBy = EmpID;


                                }
                                else
                                {


                                    CntDbnext = CntDbnext + 1;
                                    // int Status;
                                    //if (worksheet.Cells[row,7].Text == "Active")
                                    //{
                                    //    Status = 1;
                                    //}
                                    //else
                                    //{
                                    //    Status = 0;
                                    //}
                                    //Data Code
                                    // Insert new record
                                    var newData = new TbPLPS
                                    {

                                        PLPSID = Convert.ToString(CntDbnext).PadLeft(5, '0'),
                                        PlantID = PlantID,
                                        LineID = LineDb,
                                        ProductID = ProductDb,
                                        SectionID = SectionDb,
                                        Size = "0",//worksheet.Cells[row, 4].Text,
                                        QTYPerQRCode = Convert.ToInt32(worksheet.Cells[row, 5].Text),
                                        Unit = worksheet.Cells[row, 6].Text,
                                        FormularID = Convert.ToInt32(worksheet.Cells[row, 7].Text),
                                        Status = Convert.ToInt32(worksheet.Cells[row, 8].Text),
                                        CreateDate = DateTime.Now,
                                        CreateBy = EmpID,//User.Identity.Name;
                                        UpdateDate = DateTime.Now,
                                        UpdateBy = EmpID//User.Identity.Name;

                                    };


                                    db.TbPLPS.Add(newData);
                                    // }


                                }
                            }
                        }

                    }
                    db.SaveChanges();
                }

            }

            ViewBag.Success = "Data imported and updated successfully!";
            // return RedirectToAction("PLPS");
            return Json(new { success = true, message = "Data imported and updated successfully!" });

        }




            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            /// <summary>
            /// Employee Management
            /// </summary>
            /// <returns></returns>

            [HttpGet]
        public ActionResult EmployeeManagement(View_EmployeeMaster obj, bool? inactivestatus)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll()
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                tbSection = db.TbSection.ToList(),
                tbShift = db.TbShift.ToList(),
                tbEmployeeMaster = db.TbEmployeeMaster.ToList(),
                view_EmployeeMaster = db.View_EmployeeMaster.ToList(),
                view_PLPS = db.View_PLPS.ToList()

            };


            // Check Admin
            if (PlantID != 0)
            {

                
                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbShift = mymodel.tbShift.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();

                mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbEmployeeMaster = mymodel.tbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(); 
                mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }

            ViewBag.VBRoleEmployee = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(3)).Select(x => x.RoleAction).FirstOrDefault();
            if (!string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.SectionName) || !string.IsNullOrEmpty(obj.ShiftName) || inactivestatus != null)
            {
                ModelState.Clear();


                if (!string.IsNullOrEmpty(obj.EmployeeID))
                {
                    mymodel.view_EmployeeMaster = db.View_EmployeeMaster.Where(p => p.EmployeeID.Equals(obj.EmployeeID));
                    ViewBag.SelectedEmployeeID = obj.EmployeeID;
                }
                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(p => p.LineName.Equals(obj.LineName));
                    ViewBag.SelectedLineName = obj.LineName;
                }

                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(p => p.SectionName.Equals(obj.SectionName));
                    ViewBag.SelectedSectionName = obj.SectionName;
                }
                if (!string.IsNullOrEmpty(obj.ShiftName))
                {
                    mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(p => p.ShiftName.Equals(obj.ShiftName));
                    ViewBag.SelectedShiftName = obj.ShiftName;
                }

                if (inactivestatus == true)
                {
                    mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }


                //mymodel.view_EmployeeMaster = db.View_EmployeeMaster.Where(p => p.EmployeeName.Equals(obj.EmployeeName) || p.EmployeeID.Equals(obj.EmployeeID)).OrderByDescending(x => x.Status);
                return View(mymodel);
            }
            else
            {
                ViewBag.InactiveStatus = true;
                return View(mymodel);
            }

        }


        public ActionResult EmployeeManagementClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll()
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                tbSection = db.TbSection.ToList(),
                tbShift = db.TbShift.ToList(),
                tbEmployeeMaster = db.TbEmployeeMaster.ToList(),
                view_EmployeeMaster = db.View_EmployeeMaster.ToList(),
                view_PLPS = db.View_PLPS.ToList()

            };


            // Check Admin
            if (PlantID != 0)
            {


                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbShift = mymodel.tbShift.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();

                mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbEmployeeMaster = mymodel.tbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList();
            }


            ViewBag.VBRoleEmployee = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(3)).Select(x => x.RoleAction).FirstOrDefault();

            ViewBag.InactiveStatus = true;
            return RedirectToAction("EmployeeManagement");
        }




        [HttpPost]
        public IActionResult CreateQRCode(string employeeID) //(ViewModelAll qRCode)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));

            string qrCodeText = employeeID.Trim();
            string labelText = $"Employee: {employeeID.Trim()}";

            // Generate the QR code
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrBitmap = qrCode.GetGraphic(20);

            // Add label to the QR code
            using (Graphics g = Graphics.FromImage(qrBitmap))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAlias; // For smooth text
                Font font = new Font("Arial", 22, FontStyle.Bold);
                SolidBrush brush = new SolidBrush(System.Drawing.Color.Black);

                // Calculate where to draw the text
                float x = 10; // Left margin
                float y = qrBitmap.Height - 30; // Position at the bottom

                g.DrawString(labelText, font, brush, new System.Drawing.PointF(x, y)); // Draw the label
            }

            byte[] bitmapArray;
            using (MemoryStream stream = new MemoryStream())
            {
                qrBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                bitmapArray = stream.ToArray();
                ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(stream.ToArray());
            }

            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrcodes");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Construct the file path including the directory and file name
            string filePath = Path.Combine(directoryPath, $"{employeeID}_QRCode.png"); //qRCode.EmployeeID

            // Construct the URI for the image
            string QrUri = Url.Content("~/qrcodes/" + $"{employeeID}_QRCode.png"); //qRCode.EmployeeID
            // Save the image to the specified directory
            System.IO.File.WriteAllBytes(filePath, bitmapArray);

 
          //  string directoryPaths = Server.MapPath("~/qrcodes");

            // Assign the URI to the ViewBag
            ViewData["QrCodeUri"] = QrUri;
            ViewBag.EmployeeID = employeeID; //qRCode.EmployeeID;
            ViewBag.QRCodeGenerated = true;


            return Content(QrUri);
            
        }

        [HttpGet]
        public ActionResult EmployeeRegenerate1()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var Employeevar = db.TbEmployeeMaster.Where(x => x.Status.Equals(1) && PlantID.Equals(PlantID)).ToList();
            foreach (var item in Employeevar)
            {

                // Text data to be encoded in the QR code
                string qrCodeText = item.EmployeeID.Trim();


                QRCodeGenerator QrGenerator = new QRCodeGenerator();
                QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q); //qRCode.EmployeeID

                QRCode QrCode = new QRCode(QrCodeInfo);
                Bitmap QrBitmap = QrCode.GetGraphic(20);
                byte[] BitmapArray = QrBitmap.BitmapToByteArray();

                // Convert Bitmap to byte array
                using (MemoryStream stream = new MemoryStream())
                {
                    QrBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(stream.ToArray());
                }

                // Pass label value to the view
                ViewBag.LabelValue = qrCodeText;


                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrcodes");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string filePath = Path.Combine(directoryPath, $"{qrCodeText}_QRCode.png"); //qRCode.EmployeeID
                System.IO.File.WriteAllBytes(filePath, BitmapArray);
                string QrUri = Url.Content("~/qrcodes/" + $"{qrCodeText}_QRCode.png"); //qRCode.EmployeeID


            }
            var mymodel = new ViewModelAll()
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.ToList().Where(p => p.PlantID.Equals(PlantID)).ToList(),
                tbLine = db.TbLine.ToList().Where(p => p.PlantID.Equals(PlantID)).ToList(),
                tbSection = db.TbSection.ToList().Where(p => p.PlantID.Equals(PlantID)).ToList(),
                tbShift = db.TbShift.ToList().Where(p => p.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeMaster = db.TbEmployeeMaster.Where(p => p.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeMaster = db.View_EmployeeMaster.Where(p => p.PlantID.Equals(PlantID)).ToList(),
                view_PLPS = db.View_PLPS.Where(p => p.PlantID.Equals(PlantID)).ToList()

            };
            ViewBag.VBRoleEmployee = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(3)).Select(x => x.RoleAction).FirstOrDefault();

            return View("EmployeeManagement", mymodel);



        }




        [HttpGet]
        public ActionResult EmployeeRegenerate()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var Employeevar = db.TbEmployeeMaster
                .Where(x => x.Status.Equals(1) && PlantID.Equals(PlantID))
                .ToList();

            foreach (var item in Employeevar)
            {
                string qrCodeText = item.EmployeeID.Trim();
                string labelText = $"Employee: {item.EmployeeID.Trim()}";

                // Generate the QR code
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrBitmap = qrCode.GetGraphic(20);

                // Add label to the QR code
                using (Graphics g = Graphics.FromImage(qrBitmap))
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAlias; // For smooth text
                    Font font = new Font("Arial", 22, FontStyle.Bold);
                    SolidBrush brush = new SolidBrush(System.Drawing.Color.Black);

                    // Calculate where to draw the text
                    float x = 10; // Left margin
                    float y = qrBitmap.Height - 30; // Position at the bottom

                    g.DrawString(labelText, font, brush, new System.Drawing.PointF(x, y)); // Draw the label
                }

                byte[] bitmapArray;
                using (MemoryStream stream = new MemoryStream())
                {
                    qrBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    bitmapArray = stream.ToArray();
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(stream.ToArray());
                }

                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrcodes");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string filePath = Path.Combine(directoryPath, $"{qrCodeText}_QRCode.png");
                System.IO.File.WriteAllBytes(filePath, bitmapArray);
                string QrUri = Url.Content("~/qrcodes/" + $"{qrCodeText}_QRCode.png"); //qRCode.EmployeeID
            }

            var mymodel = new ViewModelAll()
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                tbSection = db.TbSection.ToList(),
                tbShift = db.TbShift.ToList(),
                tbEmployeeMaster = db.TbEmployeeMaster.ToList(),
                view_EmployeeMaster = db.View_EmployeeMaster.ToList(),
                view_PLPS = db.View_PLPS.ToList()

            };


            // Check Admin
            if (PlantID != 0)
            {


                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbShift = mymodel.tbShift.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();

                mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID));
                mymodel.tbEmployeeMaster = mymodel.tbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID));
                mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(x => x.PlantID.Equals(PlantID));
            }


            ViewBag.VBRoleEmployee = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(3)).Select(x => x.RoleAction).FirstOrDefault();

            return View("EmployeeManagement", mymodel);


        }

        // Function Create transaction : View EmployeeManagement
        [HttpPost] 
        public ActionResult EmployeeCreate(View_EmployeeMaster obj, string submit)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (submit == "Generate")
            {
                // Generate a Simple BarCode image and save as PNG
                var qrCode = QRCodeWriter.CreateQrCode(obj.EmployeeID, 500, QRCodeWriter.QrErrorCorrectionLevel.Medium);
                var qrCodePath = Path.Combine(QREmppath, obj.EmployeeID + ".png");
                qrCode.SaveAsPng(qrCodePath);

                // Save the file to the server
                SaveQRCodeToServer(qrCodePath);

                var data = QREmppath + obj.EmployeeID + ".png";

                return Json(data);
            }
            else
            {


                //Check Duplicate
                var plantdb = db.View_Employee.Where(p => p.EmployeeID.Equals(obj.EmployeeID));
                string[] EmpName = obj.EmployeeName.Split(" ");
                //  var userdb = db.TbUsers.Where(x => x.ID.Equals(1)).SingleOrDefault();
                if (plantdb.Count() == 0)
                {
                    // Insert new Plant               
                    db.TbEmployeeMaster.Add(new TbEmployeeMaster()
                    {
                        //ID = db.TbEmployeeMaster.Count() + 1,
                        EmployeeID = obj.EmployeeID,
                        EmployeeName = obj.EmployeeName,
                        EmployeeLastName = obj.EmployeeLastName,
                        PlantID = PlantID,// db.TbPlants.Where(s => s.PlantName.Equals(obj.PlantName)).Select(s => s.PlantID).First(),
                        LineID = obj.LineName,//db.TbLines.Where(s => s.LineName.Equals(obj.LineName)).Select(s => s.LineID).First(),
                        SectionID = obj.SectionID,//obj.PlantName,
                        ShiftID = Convert.ToInt32(obj.ShiftName),
                        Type = "1",
                        Status = obj.Status,
                        QRCodePerEmployee = 1,//obj.QRCodePerEmployee,
                        CreateDate = DateTime.Today,
                        CreateBy = EmpID,
                        UpdateDate = DateTime.Today,
                        UpdateBy = EmpID
                    }) ;
                    db.SaveChanges();

                }
                else
                {
                    TempData["AlertMessage"] = "Employee Duplicate!";
                    ViewBag.Error = "Employee Duplicate!";
                }

            }
            return RedirectToAction("EmployeeManagement");
        }


      
        private void SaveQRCodeToServer(string filePath)
        {
            // Implement the logic to save the file to the server
            // This might include copying the file to a specific directory, storing the path in a database, etc.
            // Example:
            var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", Path.GetFileName(filePath));
            System.IO.File.Copy(filePath, destinationPath, true);
        }


        [HttpGet]
        public JsonResult EmployeeEdit(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }

            var mymodel = new ViewModelAll()
            {
                //view_PermissionMaster = db.View_PermissionMaster.ToList(),
                //tbPlants = db.TbPlant.ToList(),
                //tbLine = db.TbLine.ToList(),
                //tbSection = db.TbSection.ToList(),
                //tbShift = db.TbShift.ToList(),
                //tbEmployeeMaster = db.TbEmployeeMaster.ToList(),
                view_EmployeeMaster = db.View_EmployeeMaster.ToList(),
               // view_PLPS = db.View_PLPS.ToList()

            };


            // Check Admin
            if (PlantID != 0)
            {


                //mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbShift = mymodel.tbShift.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();

                //mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID));
                //mymodel.tbEmployeeMaster = mymodel.tbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID));
                mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(x => x.PlantID.Equals(PlantID));
            }

            var Emp = db.View_EmployeeMaster.Where(p=>p.ID.Equals(id)).SingleOrDefault();
            return Json(Emp);
        }



        [HttpPost]
        public ActionResult EmployeeUpdate(TbEmployeeMaster obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var mymodel = new ViewModelAll()
            {
                //view_PermissionMaster = db.View_PermissionMaster.ToList(),
                //tbPlants = db.TbPlant.ToList(),
                //tbLine = db.TbLine.ToList(),
                //tbSection = db.TbSection.ToList(),
                //tbShift = db.TbShift.ToList(),
                tbEmployeeMaster = db.TbEmployeeMaster.ToList(),
                //view_EmployeeMaster = db.View_EmployeeMaster.ToList(),
                //view_PLPS = db.View_PLPS.ToList()

            };


            // Check Admin
            if (PlantID != 0)
            {


                //mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbShift = mymodel.tbShift.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();

                //mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID));
                mymodel.tbEmployeeMaster = mymodel.tbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID));
               // mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(x => x.PlantID.Equals(PlantID));
            }

            var Empdb = db.TbEmployeeMaster.Where(x => x.EmployeeID == obj.EmployeeID).SingleOrDefault();
            if (obj.EmployeeName != null)
            {
                Empdb.EmployeeName = obj.EmployeeName;

            }
            if (obj.EmployeeLastName != null)
            {
                Empdb.EmployeeLastName = obj.EmployeeLastName;

            }
            if (obj.PlantID != 0)
            {
                Empdb.PlantID = obj.PlantID;

            }
            if (obj.LineID != null)
            {
                Empdb.LineID = obj.LineID;
            }
            if (obj.SectionID != null)
            {
                Empdb.SectionID = obj.SectionID;
            }
            if (obj.ShiftID != 0)
            {
                Empdb.ShiftID = obj.ShiftID;
            }
            if (obj.Type != null)
            {
                Empdb.Type = obj.Type;
            }

            if (obj.Status != null)
            {
                if (obj.Status == 1)
                {
                    Empdb.Status = 1;
                }
                else
                {
                    Empdb.Status = 0;
                }
            }
         

            Empdb.UpdateBy = EmpID;// User.Identity.Name;
            obj.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("EmployeeManagement");

        }

        public JsonResult EmployeeInactive(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var mymodel = new ViewModelAll()
            {
                //view_PermissionMaster = db.View_PermissionMaster.ToList(),
                //tbPlants = db.TbPlant.ToList(),
                //tbLine = db.TbLine.ToList(),
                //tbSection = db.TbSection.ToList(),
                //tbShift = db.TbShift.ToList(),
                tbEmployeeMaster = db.TbEmployeeMaster.ToList(),
                //view_EmployeeMaster = db.View_EmployeeMaster.ToList(),
                //view_PLPS = db.View_PLPS.ToList()

            };


            // Check Admin
            if (PlantID != 0)
            {


                //mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbShift = mymodel.tbShift.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();

                //mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID));
                mymodel.tbEmployeeMaster = mymodel.tbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID));
               // mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(x => x.PlantID.Equals(PlantID));
            }

            var Employeedb = db.TbEmployeeMaster.Where(p => p.ID.Equals(id)).SingleOrDefault();
            if (Employeedb != null)
            {
                Employeedb.Status = 0;
                Employeedb.UpdateBy = EmpID;//User.Identity.Name;
                Employeedb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.View_EmployeeMaster);

        }


        public JsonResult EmployeeActive(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return Json("Login", "Home");
            }
            var mymodel = new ViewModelAll()
            {
                //view_PermissionMaster = db.View_PermissionMaster.ToList(),
                //tbPlants = db.TbPlant.ToList(),
                //tbLine = db.TbLine.ToList(),
                //tbSection = db.TbSection.ToList(),
                //tbShift = db.TbShift.ToList(),
                tbEmployeeMaster = db.TbEmployeeMaster.ToList(),
                //view_EmployeeMaster = db.View_EmployeeMaster.ToList(),
                //view_PLPS = db.View_PLPS.ToList()

            };


            // Check Admin
            if (PlantID != 0)
            {


                //mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbShift = mymodel.tbShift.Where(x => x.PlantID.Equals(PlantID)).ToList();
                //mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();

                //mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID));
                mymodel.tbEmployeeMaster = mymodel.tbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID));
                // mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(x => x.PlantID.Equals(PlantID));
            }
            var Employeedb = db.TbEmployeeMaster.Where(p => p.ID.Equals(id)).SingleOrDefault();
            if (Employeedb != null)
            {
                Employeedb.Status = 1;
                Employeedb.UpdateBy = EmpID;//User.Identity.Name;
                Employeedb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.View_EmployeeMaster);

        }




        public IActionResult EmployeeManagementDownloadExcel(ViewModelAll obj)
        {

            var data = db.View_EmployeeMaster;
            // Create a new Excel package
            using (var package = new ExcelPackage())
            {
                // Add a worksheet to the package
                var worksheet = package.Workbook.Worksheets.Add("Employee");

                // Add headers
                worksheet.Cells["A1"].Value = "ID";
                worksheet.Cells["B1"].Value = "EmployeeID";
                worksheet.Cells["C1"].Value = "EmployeeName";
                worksheet.Cells["D1"].Value = "EmployeeLastName";
                worksheet.Cells["E1"].Value = "PlantID";
                worksheet.Cells["F1"].Value = "LineID";
                worksheet.Cells["G1"].Value = "SectionID";
                worksheet.Cells["H1"].Value = "ShiftID";
                worksheet.Cells["I1"].Value = "QRCodePerEmployee";
                worksheet.Cells["J1"].Value = "Type";
                worksheet.Cells["K1"].Value = "Status";


                // Add data rows
                int row = 2;
                foreach (var item in data)
                {
                    //worksheet.Cells[$"A{row}"].Value = item.ID;
                    //worksheet.Cells[$"B{row}"].Value = item.EmployeeID;
                    //worksheet.Cells[$"C{row}"].Value = item.EmployeeName;
                    //worksheet.Cells[$"D{row}"].Value = item.EmployeeLastName;
                    //worksheet.Cells[$"E{row}"].Value = item.PlantID;
                    //worksheet.Cells[$"F{row}"].Value = item.LineID;
                    //worksheet.Cells[$"G{row}"].Value = item.SectionID;
                    //worksheet.Cells[$"H{row}"].Value = item.ShiftID;
                    //worksheet.Cells[$"I{row}"].Value = item.QRCodePerEmployee;
                    //worksheet.Cells[$"J{row}"].Value = item.Type;
                    //worksheet.Cells[$"K{row}"].Value = item.Status;

                    row++;
                }

                // Save the Excel package to a stream
                var stream = new MemoryStream(package.GetAsByteArray());

                // Set the position to the beginning of the stream
                stream.Position = 0;

                // Return the Excel file as a FileStreamResult
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeReport.xlsx");
            }
        }


        [HttpGet]
        public ActionResult EmployeeExport(View_EmployeeMaster obj, bool? inactivestatus)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");


            if (EmpID == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var mymodel = new ViewModelAll()
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                tbSection = db.TbSection.ToList(),
                tbShift = db.TbShift.ToList(),
                tbEmployeeMaster = db.TbEmployeeMaster.ToList(),
                view_EmployeeMaster = db.View_EmployeeMaster.ToList(),
                view_PLPS = db.View_PLPS.ToList()

            };


            // Check Admin
            if (PlantID != 0)
            {


                mymodel.tbPlants = mymodel.tbPlants.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbLine = mymodel.tbLine.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbShift = mymodel.tbShift.Where(x => x.PlantID.Equals(PlantID)).ToList();
                mymodel.tbSection = mymodel.tbSection.Where(x => x.PlantID.Equals(PlantID)).ToList();

                mymodel.view_PLPS = mymodel.view_PLPS.Where(x => x.PlantID.Equals(PlantID));
                mymodel.tbEmployeeMaster = mymodel.tbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID));
                mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(x => x.PlantID.Equals(PlantID));
            }

            ViewBag.VBRoleEmployee = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(3)).Select(x => x.RoleAction).FirstOrDefault();
            if (!string.IsNullOrEmpty(obj.LineName) || !string.IsNullOrEmpty(obj.EmployeeID) || !string.IsNullOrEmpty(obj.SectionName) || !string.IsNullOrEmpty(obj.ShiftName) || inactivestatus != null)
            {
                ModelState.Clear();


                if (!string.IsNullOrEmpty(obj.EmployeeID))
                {
                    mymodel.view_EmployeeMaster = db.View_EmployeeMaster.Where(p => p.EmployeeID.Equals(obj.EmployeeID));
                    ViewBag.SelectedEmployeeID = obj.EmployeeID;
                }
                if (!string.IsNullOrEmpty(obj.LineName))
                {
                    mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(p => p.LineName.Equals(obj.LineName));
                    ViewBag.SelectedLineName = obj.LineName;
                }

                if (!string.IsNullOrEmpty(obj.SectionName))
                {
                    mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(p => p.SectionName.Equals(obj.SectionName));
                    ViewBag.SelectedSectionName = obj.SectionName;
                }
                if (!string.IsNullOrEmpty(obj.ShiftName))
                {
                    mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(p => p.ShiftName.Equals(obj.ShiftName));
                    ViewBag.SelectedShiftName = obj.ShiftName;
                }

                if (inactivestatus == true)
                {
                    mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    mymodel.view_EmployeeMaster = mymodel.view_EmployeeMaster.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }

                var collection = mymodel.view_EmployeeMaster.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Line");
                Sheet.Cells["A1"].Value = "EmployeeID";
                Sheet.Cells["B1"].Value = "EmployeeName";
                Sheet.Cells["C1"].Value = "EmployeeLastName";
                Sheet.Cells["D1"].Value = "PlantID";
                Sheet.Cells["E1"].Value = "LineID";
                Sheet.Cells["F1"].Value = "SectionID";
                Sheet.Cells["G1"].Value = "ShiftID";
                Sheet.Cells["H1"].Value = "Status";


                int row = 2;
                foreach (var item in collection)
                {


                    Sheet.Cells[string.Format("A{0}", row)].Value = item.EmployeeID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.EmployeeName;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.EmployeeLastName;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.PlantID;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.LineID;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.SectionID;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.ShiftID;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Status;

                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=Employee-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());


                //mymodel.view_EmployeeMaster = db.View_EmployeeMaster.Where(p => p.EmployeeName.Equals(obj.EmployeeName) || p.EmployeeID.Equals(obj.EmployeeID)).OrderByDescending(x => x.Status);
                return RedirectToAction("EmployeeManagement",mymodel);
            }
            else
            {

                var collection = mymodel.view_EmployeeMaster.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Line");
                Sheet.Cells["A1"].Value = "EmployeeID";
                Sheet.Cells["B1"].Value = "EmployeeName";
                Sheet.Cells["C1"].Value = "EmployeeLastName";
                Sheet.Cells["D1"].Value = "PlantID";
                Sheet.Cells["E1"].Value = "LineID";
                Sheet.Cells["F1"].Value = "SectionID";
                Sheet.Cells["G1"].Value = "ShiftID";
                Sheet.Cells["H1"].Value = "Status";


                int row = 2;
                foreach (var item in collection)
                {


                    Sheet.Cells[string.Format("A{0}", row)].Value = item.EmployeeID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.EmployeeName;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.EmployeeLastName;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.PlantID;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.LineID;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.SectionID;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.ShiftID;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Status;

                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=Employee-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());


                ViewBag.InactiveStatus = true;
                return RedirectToAction("EmployeeManagement", mymodel);
            }

        }



        [HttpPost]
        public IActionResult EmployeeManagementUpload(IFormFile FileUpload)
        {
             int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            if (FileUpload == null || FileUpload.Length <= 0)
            {
                ViewBag.AlertMessage = "Please select a valid Excel file.";
                return View("EmployeeManagement");
            }
            
             
            using (var stream = new MemoryStream())
            {
                FileUpload.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        if (worksheet.Cells[row, 2].Value != null )
                        {

                            var id = "";
                            if (worksheet.Cells[row, 1].Value != null)
                            {
                                id = worksheet.Cells[row, 1].Value.ToString();
                            }

                            var DataDb = db.TbEmployeeMaster.Where(x=>x.EmployeeID.Equals(id)).SingleOrDefault();
                            //check line

                            var LineDb = db.TbLine.Where(x => x.LineID.Equals(worksheet.Cells[row, 5].Text.Trim()) && x.PlantID.Equals(PlantID)).Select(x => x.LineID).SingleOrDefault();
                            //Check Product
                          
                            var SectionDb = db.TbSection.Where(x => x.SectionID.Equals(worksheet.Cells[row, 6].Text.Trim()) && x.PlantID.Equals(PlantID)).Select(x => x.SectionID).SingleOrDefault();

                            var ShiftDb = db.TbShift.Where(x => x.ShiftID.Equals(worksheet.Cells[row, 7].Text.Trim()) && x.PlantID.Equals(PlantID)).Select(x => x.ShiftID).SingleOrDefault();
                            var statuevar = worksheet.Cells[row, 8].Text;
                            bool isEmpty = string.IsNullOrWhiteSpace(statuevar);
                            bool isInt = int.TryParse(statuevar, out int result);
                            bool isNotOneOrZero = isInt && result != 1 && result != 0;

                            if (LineDb == null || SectionDb == null || ShiftDb == null  || ((isEmpty == true || isNotOneOrZero == true)))
                            {
                                int rowerror = row - 1;
                                TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake please check line or section Master ";
                                // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                                return RedirectToAction("EmployeeManagement");

                            }


                            if (DataDb != null)
                            {
                                //string[] name = worksheet.Cells[row, 2].Text.Split(" ");
                                DataDb.EmployeeID = worksheet.Cells[row, 1].Text;
                                DataDb.EmployeeName = worksheet.Cells[row, 2].Text;
                                DataDb.EmployeeLastName = worksheet.Cells[row, 3].Text;
                                DataDb.PlantID = PlantID;
                                DataDb.LineID = LineDb;
                                DataDb.SectionID = SectionDb;
                                DataDb.ShiftID = Convert.ToInt32(worksheet.Cells[row, 7].Text.Trim());
                                DataDb.QRCodePerEmployee = 1;// int.Parse(worksheet.Cells[row, 7].Text);
                                DataDb.Type = "1";// worksheet.Cells[row, 10].Text;
                                DataDb.Status = Convert.ToInt32(worksheet.Cells[row, 8].Text); 
                                DataDb.UpdateDate = DateTime.Now;
                                DataDb.UpdateBy = EmpID; //User.Identity.Name;
                            }
                            else
                            {
                                // Insert new record
                                // check PLPS 
                                //checked PLPS
                                var PLPSData = db.TbPLPS.Where(x =>
                                x.PlantID.Equals(PlantID) &&
                                x.LineID.Equals(LineDb) &&
                                x.SectionID.Equals(SectionDb)
                                ).ToList();

                                if (PLPSData.Count == 0)
                                {
                                    int rowerror = row - 1;
                                    TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake please add PLPS";
                                    // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                                    return RedirectToAction("EmployeeManagement");
                                }
                                else
                                {
                                   // string[] name = worksheet.Cells[row, 2].Text.Split(" ");
                                    var newData = new TbEmployeeMaster
                                    {
                                        EmployeeID = worksheet.Cells[row, 1].Text,
                                        EmployeeName = worksheet.Cells[row, 2].Text,
                                        EmployeeLastName = worksheet.Cells[row, 3].Text,
                                        PlantID = PlantID,
                                        LineID = LineDb,
                                        SectionID = SectionDb,
                                        ShiftID = Convert.ToInt32(worksheet.Cells[row, 7].Text.Trim()),
                                        QRCodePerEmployee = 1 , // int.Parse(worksheet.Cells[row, 8].Text),
                                        Type = "1",// worksheet.Cells[row, 10].Text,
                                            Status = Convert.ToInt32(worksheet.Cells[row, 8].Text),
                                            CreateDate = DateTime.Now,
                                            CreateBy = EmpID,//User.Identity.Name;
                                            UpdateDate = DateTime.Now,
                                            UpdateBy = EmpID//User.Identity.Name;

                                        };
                                        db.TbEmployeeMaster.Add(newData);


                                    // Text data to be encoded in the QR code
                                    string qrCodeText = worksheet.Cells[row, 1].Text;

                                    // Label value
                                    string labelValue = worksheet.Cells[row, 1].Text;

                                    // Concatenate label value with QR code text
                                    string qrCodeTextWithLabel = $"{qrCodeText} - {labelValue}";


                                    QRCodeGenerator QrGenerator = new QRCodeGenerator();
                                    QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q); //qRCode.EmployeeID

                                    QRCode QrCode = new QRCode(QrCodeInfo);
                                    Bitmap QrBitmap = QrCode.GetGraphic(20);
                                    byte[] BitmapArray = QrBitmap.BitmapToByteArray();
                                        QrBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                        ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(stream.ToArray());
                                  

                                    // Pass label value to the view
                                    ViewBag.LabelValue = labelValue;


                                    string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrcodes");

                                    if (!Directory.Exists(directoryPath))
                                    {
                                        Directory.CreateDirectory(directoryPath);
                                    }

                                    string filePath = Path.Combine(directoryPath, $"{qrCodeText}_QRCode.png"); //qRCode.EmployeeID
                                    System.IO.File.WriteAllBytes(filePath, BitmapArray);
                                    string QrUri = Url.Content("~/qrcodes/" + $"{qrCodeText}_QRCode.png"); //qRCode.EmployeeID



                                }
                            }
                        }
                    }

                    db.SaveChanges();
                }

            }
         
           
            ViewBag.Success = "Data imported and updated successfully!";
            return RedirectToAction("EmployeeManagement");

        }







    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Employee Group QRCode
    /// </summary>
    /// <returns></returns>
    public ActionResult EmployeeGroupQRCode(TbEmployeeGroupQR obj, bool? inactivestatus)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbEmployeeGroupQR = db.TbEmployeeGroupQR.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeGroup = db.View_EmployeeGroup.Where(x=>x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeGroupList = db.View_EmployeeGroupList.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //temp_Group = db.Temp_Group.ToList(),
                // view_Temp_Group = db.View_Temp_Group.ToList()

            };



            ViewBag.VBRoleEmpGroup = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(15)).Select(x => x.RoleAction).FirstOrDefault();


            if (!string.IsNullOrEmpty(obj.EmployeeID) || inactivestatus != null)
            {
                if (!string.IsNullOrEmpty(obj.EmployeeID))
                {

                    mymodel.view_EmployeeGroupList = mymodel.view_EmployeeGroupList.Where(x => x.EmployeeIDs.Contains(obj.EmployeeID)).ToList();
                    ViewBag.SelectedEmployeeID = obj.EmployeeID;
               
                    return View(mymodel);
                }
                if (inactivestatus == true)
                {
                    mymodel.view_EmployeeGroupList = mymodel.view_EmployeeGroupList.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    mymodel.view_EmployeeGroupList = mymodel.view_EmployeeGroupList.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }

             

                return View(mymodel);


            }
            else
            {
                ViewBag.InactiveStatus = true;
                return View(mymodel);


            }
        }



        public ActionResult EmployeeGroupQRClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbEmployeeGroupQR = db.TbEmployeeGroupQR.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeGroup = db.View_EmployeeGroup.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeGroupList = db.View_EmployeeGroupList.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //temp_Group = db.Temp_Group.ToList(),
                // view_Temp_Group = db.View_Temp_Group.ToList()

            };
            ViewBag.InactiveStatus = true;

            return RedirectToAction("EmployeeGroupQRCode");

        }


        // Function Create transaction : View EmployeeManagement
        [HttpGet] 
        public ActionResult EmployeeGroupeCreate(View_EmployeeGroup obj,string submit)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            if (submit == "Generate")
            {

                string qrCodeText = obj.GroupID.Trim();


                QRCodeGenerator QrGenerator = new QRCodeGenerator();
                QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q); //qRCode.EmployeeID

                QRCode QrCode = new QRCode(QrCodeInfo);
                Bitmap QrBitmap = QrCode.GetGraphic(20);
                byte[] BitmapArray = QrBitmap.BitmapToByteArray();

                // Convert Bitmap to byte array
                using (MemoryStream stream = new MemoryStream())
                {
                    QrBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(stream.ToArray());
                }

                // Pass label value to the view
                ViewBag.LabelValue = qrCodeText;


                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrcodes");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string filePath = Path.Combine(directoryPath, $"{qrCodeText}_QRCode.png"); //qRCode.EmployeeID
                System.IO.File.WriteAllBytes(filePath, BitmapArray);
                string QrUri = Url.Content("~/qrcodes/" + $"{qrCodeText}_QRCode.png"); //qRCode.EmployeeID

                var mymodel = new ViewModelAll
                {
                    view_PermissionMaster = db.View_PermissionMaster.ToList(),
                    tbEmployeeGroupQR = db.TbEmployeeGroupQR.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                    tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                    view_EmployeeGroup = db.View_EmployeeGroup.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                    view_EmployeeGroupList = db.View_EmployeeGroupList.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                    //temp_Group = db.Temp_Group.ToList(),
                    // view_Temp_Group = db.View_Temp_Group.ToList()

                };

                return View("EmployeeGroupQRCode",mymodel);
                //Directory.CreateDirectory(driveDPath);
                //// Relative path of the image file on Drive D


                //string relativePath = $"{obj.GroupID.PadLeft(5, '0')}.png";

                //try
                //{
                //    // Create QR code and save as PNG
                //    QRCodeWriter.CreateQrCode(obj.GroupID.PadLeft(5, '0'), 500, QRCodeWriter.QrErrorCorrectionLevel.Medium)
                //        .SaveAsPng(Path.Combine(driveDPath, relativePath));

                //    // Construct the full path
                //    var data = Path.Combine(driveDPath, relativePath);

                //    // Return the full file path as JSON response
                //    return Json(data);
                //}
                //catch (Exception ex)
                //{
                //    // Handle exceptions (e.g., file I/O errors)
                //    return Json($"Error: {ex.Message}");
                //}

            }
            else
            {
         
                //Check Duplicate checkline checkplant 
                var EmpGroupdb = db.View_EmployeeGroup.Where(p => p.EmployeeID.Equals(obj.EmployeeID) && p.Status != 0);
              
                if (EmpGroupdb.Count() == 0)
                {
                // Insert new Plant               
                        db.TbEmployeeGroupQR.Add(new TbEmployeeGroupQR()
                        {
                   
                            GroupID = obj.GroupID.PadLeft(5, '0'),
                            EmployeeID = obj.EmployeeID,
                            PlantID = PlantID,
                            Status = obj.Status,
                            CreateDate = DateTime.Today,
                            CreateBy = EmpID,
                            UpdateDate = DateTime.Today,
                            UpdateBy = EmpID
                        }) ;
                            db.SaveChanges();

                }
                else
                {
                    TempData["AlertMessage"] = "EmployeeGroupQRCode duplicate!";
                    ViewBag.Error = "EmployeeGroupQRCode Duplicate!";
                }

            }
            return RedirectToAction("EmployeeGroupQRCode");
        }
        

        [HttpGet]
        public JsonResult EmployeeGroupEdit(string id)
        {
            db.Database.ExecuteSqlRaw("DELETE FROM Temp_Group");
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            // var Emp = db.View_EmployeeGroup.Where(x=>x.GroupID.EndsWith(id)).ToList();
          
         
            var Empgrupdb = db.TbEmployeeGroupQR.Where(p => p.GroupID.Equals(id.PadLeft(5,'0')) && p.Status.Equals(1)).ToList();
                    foreach( var items in Empgrupdb)
                    {
                            db.Temp_Group.Add(new Temp_Group()
                            {
                                ID = items.GroupID.ToString(),
                                EmployeeID = items.EmployeeID.ToString(),

                            });
                             db.SaveChanges();
                    }

            var employees = db.View_Temp_Group
         .Select(x => new
         {
             GroupID = x.ID,
             EmployeeID = x.EmployeeID,
             EmployeeName = x.EmployeeName + x.EmployeeLastName
             // Add other properties as needed
         })
  .ToArray();

            //   ViewBag.groupid = 00001;
            return Json(employees);
          
        }



        [HttpPost]
        public JsonResult EmployeeGroupAddRowEdit([FromBody] ViewModelAll request)
        {
            
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
           // Create, Edit->พนักงานคนเดียวกันสามารถอยู่ได้หลาย group
            //  var EmpGroupDb = db.TbEmployeeGroupQR.Where(x => x.GroupID.Equals(request.GroupID) && x.EmployeeID.Equals(request.EmployeeID) && x.Status.Equals(1)).ToList();
            var EmpGroupDb = db.TbEmployeeGroupQR.Where(x => x.GroupID.Equals(request.GroupID)  && x.Status.Equals(1)).ToList();

            if (EmpGroupDb.Count != null)
            {
                db.Temp_Group.Add(new Temp_Group()
                {
                    ID = request.GroupID.ToString(),
                    EmployeeID = request.EmployeeID.ToString()

                });
                db.SaveChanges();

            }
            else
            {
                TempData["AlertMessage"] = "EmployeeGroupQRCode Duplicate!";
            }

            var employees = db.View_Temp_Group
                 .Select(x => new
                 {
                     GroupID = x.ID,
                     EmployeeID = x.EmployeeID,
                     EmployeeName = x.EmployeeName + x.EmployeeLastName
                 })
          .ToArray();
            return Json("EmployeeGroupQRCode", employees);
           // return Json(employees);
        }


        [HttpPost]
        public JsonResult EmployeeGroupDeleteRowEdit([FromBody] ViewModelAll request)
        {
            try
            {
                int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));

                // Find the employee group record to delete
                var empGroupToDelete = db.Temp_Group
                    .FirstOrDefault(x => x.EmployeeID.Equals(request.EmployeeID));

                if (empGroupToDelete != null)
                {
                    // Delete the record
                    db.Temp_Group.Remove(empGroupToDelete);
                    db.SaveChanges();

                    var employees = db.View_Temp_Group
                        .Select(x => new
                        {
                            GroupID = x.ID,
                            EmployeeID = x.EmployeeID,
                            EmployeeName = x.EmployeeName + " " + x.EmployeeLastName
                        })
                        .ToArray();

                    return Json(employees);
                }
                else
                {
                    TempData["AlertMessage"] = "EmployeeGroupQRCode not found!";
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                TempData["AlertMessage"] = "An error occurred while deleting the record.";
            }

            return Json(null);
        }


        [HttpPost]
        public IActionResult CreateGroupQRCode(ViewModelAll qRCode)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(qRCode.GroupID, QRCodeGenerator.ECCLevel.Q);
            QRCode QrCode = new QRCode(QrCodeInfo);
            Bitmap QrBitmap = QrCode.GetGraphic(60);
            byte[] BitmapArray = QrBitmap.BitmapToByteArray();


            //string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));

            // Specify the directory where you want to save the QR codes
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrcodesgroup");

            // Create the directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Construct the file path including the directory and file name
            string filePath = Path.Combine(directoryPath, $"{qRCode.GroupID}_QRCode.png");

            // Save the image to the specified directory
            System.IO.File.WriteAllBytes(filePath, BitmapArray);

            // Construct the URI for the image
            // string QrUri = string.Format("/qrcodes/{0}_QRCode.png", qRCode.EmployeeID);
            // Construct the URI for the image
            string QrUri = Url.Content("~/qrcodesgroup/" + $"{qRCode.GroupID}_QRCode.png");

            // Assign the URI to the ViewBag
            // ViewBag.QrCodeUri = QrUri;
            ViewData["QrCodeUri"] = QrUri;
            //ViewBag.QrCodeUri = QrUri;
            ViewBag.GroupID = qRCode.GroupID;
            ViewBag.QRCodeGenerated = true;


            var mymodel = new ViewModelAll()
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                tbSection = db.TbSection.ToList(),
                tbShift = db.TbShift.ToList(),
                tbEmployeeMaster = db.TbEmployeeMaster.Where(p => p.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeMaster = db.View_EmployeeMaster.Where(p => p.PlantID.Equals(PlantID)).OrderByDescending(x => x.Status).ToList(),
                view_PLPS = db.View_PLPS.Where(p => p.PlantID.Equals(PlantID)).ToList()
            };

            return View("EmployeeGroupQRCode", mymodel);
        }




        // Delete/Add on Edit module click submit this function working
        [HttpPost]
        public JsonResult EmployeeGroupQRCodeUpdate([FromBody] ViewModelAll viewModel)
        {

            // Extract GroupID and tableData from jsonData
            string groupID = viewModel.GroupID;
            List<Dictionary<string, string>> tableData = viewModel.TableData;

            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            // Create a DataTable
            DataTable dataTable = new DataTable("SubEmployeeGroup");
            // Add headers to the DataTable
            var headers = tableData.First().Keys.Take(2).ToList();

            foreach (var header in headers)
            {
                dataTable.Columns.Add(header.Trim());

            }

            // Add data to the DataTable
            foreach (var rowData in tableData)
            {
                var dataRow = dataTable.NewRow();
                foreach (var header in headers)
                {
                    dataRow[header.Trim()] = rowData[header].Trim();
                }
                dataTable.Rows.Add(dataRow);
            }


            //var empGroupToDelete = db.TbEmployeeGroupQR
            // .Where(x => x.GroupID == groupID && x.Status.Equals(1))
            // .ToList();

            ////Inactive old data
            //foreach (var items in empGroupToDelete)
            //{
            //    items.Status = 0;
            //    items.UpdateDate = DateTime.Today;
            //    items.UpdateBy = EmpID;
            //    db.SaveChanges();
            //}


            ///case 1 ลบที่มีอยู่
            ///case 2 เพิ่ม
            ///case 3 ลบ + เพิ่ม
            
            var empgroupdata = db.TbEmployeeGroupQR.Where(x => x.GroupID.Equals(groupID)).ToList();
            foreach (var items in empgroupdata)
            {
                items.Status = 0;
                items.UpdateDate = DateTime.Today;
                items.UpdateBy = EmpID;
                db.SaveChanges();
            }
            //    int groupcnt = db.View_EmployeeGroupList.Count() + 1;
            for (int row = 0; row < dataTable.Rows.Count; row++)
                    {
                       // var DataGroupID = dataTable.Rows[row][0].ToString();
                        var DataEmpID = dataTable.Rows[row][0].ToString();
                        //check duplicate 
                        var empdata = db.TbEmployeeGroupQR.Where(x => x.GroupID.Equals(groupID) && x.EmployeeID.Equals(DataEmpID)).SingleOrDefault();
                                if(empdata == null)
                                {
                                var Empmasterdb = db.TbEmployeeMaster.Where(x => x.EmployeeID.Equals(DataEmpID)).SingleOrDefault();
                                    db.TbEmployeeGroupQR.Add(new TbEmployeeGroupQR()
                                    {
                                        //  SectionID = db.TbSection.Count() + 1,
                                        GroupID = groupID,
                                        EmployeeID = DataEmpID,
                                        PlantID = PlantID,
                                        Status = 1,
                                        LineID = Empmasterdb.LineID,
                                        SectionID = Empmasterdb.SectionID,
                                        CreateDate = DateTime.Today,
                                        CreateBy = EmpID,//User.Identity.Name,
                                        UpdateDate = DateTime.Today,
                                        UpdateBy = EmpID,//User.Identity.Name,
                                    });
                                  
                                }
                                else
                                {
                                    empdata.Status = 1;
                                    empdata.UpdateDate = DateTime.Today;
                                    empdata.UpdateBy = EmpID;
                                        
                                 }
                            db.SaveChanges();
                    }

           
                var mymodel = new ViewModelAll
                        {
                            view_PermissionMaster = db.View_PermissionMaster.ToList(),
                            tbEmployeeGroupQR = db.TbEmployeeGroupQR.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                            tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                            view_EmployeeGroup = db.View_EmployeeGroup.Where(x=>x.PlantID.Equals(PlantID)).ToList(),
                            view_EmployeeGroupList = db.View_EmployeeGroupList.Where(x=>x.PlantID.Equals(PlantID)).ToList(),   

                            };
       
          return Json("EmployeeGroupQRCode", mymodel);

           //  return Json("Finished");

        }

        public JsonResult EmployeeGroupInactive(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var Employeedb = db.TbEmployeeGroupQR.Where(p => p.GroupID.EndsWith(id) && p.PlantID.Equals(PlantID)).ToList();
            if (Employeedb != null)
            {
                foreach(var item in Employeedb)
                    {
                    item.Status = 0;
                    item.UpdateBy = EmpID;//User.Identity.Name;
                    item.UpdateDate = DateTime.Now;
                        db.SaveChanges();

                    }
                }
            return Json(db.View_EmployeeMaster);

        }


        public JsonResult EmployeeGroupActive(string id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var Employeedb = db.TbEmployeeGroupQR.Where(p => p.GroupID.EndsWith(id) && p.PlantID.Equals(PlantID)).ToList();
            if (Employeedb != null)
            {
                foreach (var item in Employeedb)
                {
                    item.Status = 1;
                    item.UpdateBy = EmpID;//User.Identity.Name;
                    item.UpdateDate = DateTime.Now;
                    db.SaveChanges();

                }
            }
            return Json(db.View_EmployeeMaster);

        }



        public IActionResult GetEmployeeName(string employeeID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            var employeeName = db.TbEmployeeMaster
                .Where(e => e.EmployeeID == employeeID && e.PlantID.Equals(PlantID))
                .Select(e => e.EmployeeName)
                .FirstOrDefault();

            return Json(employeeName);
        }

        public IActionResult GetEmployeeGroupData(string groupID)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            var employeeName = db.View_EmployeeGroup
                .Where(e => e.GroupID == groupID && e.PlantID.Equals(PlantID))
                 .Select(e => new View_EmployeeGroup
                 {
                     EmployeeID = e.EmployeeID,
                     EmployeeName = e.EmployeeName,
                     EmployeeLastName = e.EmployeeLastName,
                     // ... other properties ...
                 })
                .FirstOrDefault();
            return Json(employeeName);
        }


       [HttpPost]
        public IActionResult EmployeeGroupCreate([FromBody] List<Dictionary<string, string>> tableData)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var mymodel = new ViewModelAll
            {
                tbService = db.TbService.ToList(),
                tbPlants = db.TbPlant.ToList(),
                tbLine = db.TbLine.ToList(),
                view_Service = db.View_Service.OrderByDescending(x => x.ServicesStatus).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                temp_Group = db.Temp_Group.ToList()

            };

                // Create a DataTable
                DataTable dataTable = new DataTable("SubEmployeeGroup");

                // Add headers to the DataTable
                var headers = tableData.First().Keys.Take(2).ToList();
                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header.Trim());

                }

                // Add data to the DataTable
                foreach (var rowData in tableData)
                {
                    var dataRow = dataTable.NewRow();
                    foreach (var header in headers)
                    {
                        dataRow[header.Trim()] = rowData[header].Trim();
                    }
                    dataTable.Rows.Add(dataRow);
                }

            int groupcnt = db.View_EmployeeGroupList.Count() + 1;
            var EmpFirst = dataTable.Rows[0][0].ToString();
            var lineidvarFisrt = db.View_EmployeeMaster.Where(p => p.EmployeeID.Equals(EmpFirst) && p.PlantID.Equals(PlantID) && p.Status != 0).Select(x => x.LineID).SingleOrDefault();
            var SectionidvarFisrt = db.View_EmployeeMaster.Where(p => p.EmployeeID.Equals(EmpFirst) && p.PlantID.Equals(PlantID) && p.Status != 0).Select(x => x.SectionID).SingleOrDefault();
    

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                var DataEmpID = dataTable.Rows[row][0].ToString();

                var Empgrupdb = db.View_EmployeeGroup.Where(p => p.EmployeeID.Equals(DataEmpID) && p.PlantID.Equals(PlantID) && p.Status != 0).ToList();

                var lineidvar = db.View_EmployeeMaster.Where(p => p.EmployeeID.Equals(DataEmpID) && p.PlantID.Equals(PlantID) && p.Status != 0).Select(x => x.LineID).SingleOrDefault();
                var Sectionidvar = db.View_EmployeeMaster.Where(p => p.EmployeeID.Equals(DataEmpID) && p.PlantID.Equals(PlantID) && p.Status != 0).Select(x => x.SectionID).SingleOrDefault();
                var checkmoveline = db.TbEmployeeTransaction.Where(p => p.EmployeeID.Equals(DataEmpID) && p.Line.Equals(lineidvarFisrt) && p.Plant.Equals(PlantID) && p.TransactionDate.Equals(DateTime.Today.Date) && p.Remark.Equals("Adjust")).ToList();
                //var checkmoveline = db.TbEmployeeTransaction.Where(p => p.EmployeeID.Equals(DataEmpID) && p.Line.Equals(lineidvarFisrt) && p.Section.Equals(SectionidvarFisrt) && p.Plant.Equals(PlantID) && p.TransactionDate.Equals(DateTime.Today.Date) && p.Remark.Equals("Adjust")).ToList();

                var linetouse = "";
                var sectiontouse ="";
                if (Empgrupdb.Count() == 0 && (lineidvar == lineidvarFisrt || checkmoveline.Count() != 0 ))// && (SectionidvarFisrt == Sectionidvar || checkmoveline.Count() != 0))
                {
                    if(lineidvar == lineidvarFisrt) //  if(lineidvar == lineidvarFisrt && SectionidvarFisrt == Sectionidvar )
                    {
                        linetouse = lineidvar;
                        sectiontouse = Sectionidvar;
                    }
                    else
                    {
                        linetouse = checkmoveline.Select(x => x.Line).First();
                        sectiontouse = checkmoveline.Select(x => x.Section).First();
                    }


                    var newData = new TbEmployeeGroupQR
                    {
                        GroupID = groupcnt.ToString().PadLeft(5, '0'),
                        EmployeeID = DataEmpID,
                        PlantID = PlantID,//int.Parse(worksheet.Cells[row, 5].Text),
                        LineID = linetouse,
                        SectionID = sectiontouse,
                        Status = 1 ,
                        CreateDate = DateTime.Now,
                        CreateBy = EmpID,//User.Identity.Name;
                        UpdateDate = DateTime.Now,
                        UpdateBy = EmpID//User.Identity.Name;

                    };
                    db.TbEmployeeGroupQR.Add(newData);
                    //db.SaveChanges();
                }
                else
                {
                    TempData["AlertMessage"] = "EmployeeGroupQRCode Not Match Line please check!";
                    ViewBag.Error = "EmployeeGroupQRCode Duplicate!";
                         return Json("finish");
                }
                
            }
           db.SaveChanges();
            return Json("finish");
        }





        [HttpGet]
        public ActionResult GenerateQRGroup(string GroupID)
        {

            // Drive D root directory path
            string driveDPath = @"D:\QRCode\EmployeeGroup";

            // Ensure the directory exists, create if it doesn't
            Directory.CreateDirectory(driveDPath);

            // Relative path of the image file on Drive D
            string relativePath = $"{GroupID.PadLeft(5, '0')}.png";

            try
            {
                // Create QR code and save as PNG
                QRCodeWriter.CreateQrCode(GroupID.PadLeft(5, '0'), 500, QRCodeWriter.QrErrorCorrectionLevel.Medium)
                    .SaveAsPng(Path.Combine(driveDPath, relativePath));

                // Construct the full path
                var data = Path.Combine(driveDPath, relativePath);

                // Return the full file path as JSON response
                return Json(data);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file I/O errors)
                return Json($"Error: {ex.Message}");
            }
        }





        [HttpPost]
        public IActionResult EmployeeGroupProcessDataTable([FromBody] List<Dictionary<string, string>> tableData)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            var incentives = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbEmployeeGroupQR = db.TbEmployeeGroupQR.ToList(),
                tbEmployeeMaster = db.TbEmployeeMaster.ToList(),
                view_EmployeeGroup = db.View_EmployeeGroup.ToList(),
                view_EmployeeGroupList = db.View_EmployeeGroupList.ToList(),
                temp_Group = db.Temp_Group.ToList()
                

            };

            try
            {
                // Create a DataTable
                DataTable dataTable = new DataTable("EmployeeGroup");

                // Add headers to the DataTable
                //var headers = tableData.First().Keys.ToList();
                var headers = tableData.First().Keys.Take(3).ToList();
                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header.Trim());

                }

                // Add data to the DataTable
                foreach (var rowData in tableData)
                {
                    var dataRow = dataTable.NewRow();
                    foreach (var header in headers)
                    {
                        dataRow[header.Trim()] = rowData[header].Trim();
                    }
                    dataTable.Rows.Add(dataRow);
                }

                using (var package = new ExcelPackage())
                {
                    // Create a worksheet
                    var worksheet = package.Workbook.Worksheets.Add("EmployeeGroup");

                    // Add headers to the worksheet
                    for (int col = 1; col <= dataTable.Columns.Count; col++)
                    {
                        var headername = dataTable.Columns[col - 1].ColumnName;
                        worksheet.Cells[1, col].Value = headername;
                    }

                    // Add data to the worksheet
                    for (int row = 0; row < dataTable.Rows.Count; row++)
                    {
                        for (int col = 1; col <= dataTable.Columns.Count; col++)
                        {
                            try
                            {
                                var DataName = dataTable.Rows[row][col - 1].ToString();
                                worksheet.Cells[row + 2, col].Value = DataName;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error accessing data at row {row}, col {col}: {ex.Message}");
                                // Log the error using your logging framework (e.g., Serilog, NLog)
                                return StatusCode(500, $"Error exporting data: {ex.Message}");
                            }
                        }
                    }


                    var filePath = startpath + "EmployeeGroup.xls";
                    System.IO.File.WriteAllBytes(filePath, package.GetAsByteArray());

                    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    return File(fileStream, "application/vnd.ms-excel", "EmployeeGroup.xls");



                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating Excel file: {ex.Message}");
                // Log the error using your logging framework (e.g., Serilog, NLog)
                return StatusCode(500, $"Error exporting data: {ex.Message}");
            }
        }


        public ActionResult EmployeeGroupQRCodeExport(TbEmployeeGroupQR obj, bool? inactivestatus)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var mymodel = new ViewModelAll
            {
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                tbEmployeeGroupQR = db.TbEmployeeGroupQR.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbEmployeeMaster = db.TbEmployeeMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeGroup = db.View_EmployeeGroup.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_EmployeeGroupList = db.View_EmployeeGroupList.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                //temp_Group = db.Temp_Group.ToList(),
                // view_Temp_Group = db.View_Temp_Group.ToList()

            };

            ViewBag.VBRoleEmpGroup = mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(15)).Select(x => x.RoleAction).FirstOrDefault();


            if (!string.IsNullOrEmpty(obj.EmployeeID) || inactivestatus != null)
            {
                if (!string.IsNullOrEmpty(obj.EmployeeID))
                {

                    mymodel.view_EmployeeGroupList = mymodel.view_EmployeeGroupList.Where(x => x.EmployeeIDs.Contains(obj.EmployeeID)).ToList();
                    mymodel.view_EmployeeGroup = mymodel.view_EmployeeGroup.Where(x => x.EmployeeID.Contains(obj.EmployeeID)).ToList();

                    //return View(mymodel);
                }
                if (inactivestatus == true)
                {
                    mymodel.view_EmployeeGroupList = mymodel.view_EmployeeGroupList.ToList();
                    mymodel.view_EmployeeGroup = mymodel.view_EmployeeGroup.ToList();
                    ViewBag.InactiveStatus = true;
                }
                else
                {
                    mymodel.view_EmployeeGroupList = mymodel.view_EmployeeGroupList.Where(x => x.Status == 1).ToList();
                    mymodel.view_EmployeeGroup = mymodel.view_EmployeeGroup.Where(x => x.Status == 1).ToList();
                    ViewBag.InactiveStatus = false;
                }
                var grouplist = db.TbEmployeeGroupQR
                .Where(x => x.EmployeeID.Equals(obj.EmployeeID) && x.PlantID.Equals(PlantID))
                .GroupBy(x => x.GroupID)
                .Select(group => group.Key)
                .ToList();

               // var collection = db.view_EmployeeGroup.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("EmployeeGroup");
                Sheet.Cells["A1"].Value = "GroupID";
                Sheet.Cells["B1"].Value = "EmployeeID";
                Sheet.Cells["C1"].Value = "Status";
                int row = 2;

                var groupfirst = db.TbEmployeeGroupQR
                .Where(x => x.EmployeeID.Equals(obj.EmployeeID) && x.PlantID.Equals(PlantID))
                .GroupBy(x => x.GroupID)
                .Select(group => group.Key)
                .ToList().First();
                var employeelist = "";
                foreach (var item in grouplist)
                {
                    employeelist = "";
                    var selectemployee = db.View_EmployeeGroup.Where(x=>x.GroupID.Equals(item)).ToList();
                        foreach(var y in selectemployee)
                        {
                        employeelist = y.EmployeeID + "," + employeelist;
                            Sheet.Cells[string.Format("A{0}", row)].Value = item;
                            Sheet.Cells[string.Format("B{0}", row)].Value = employeelist;
                            Sheet.Cells[string.Format("C{0}", row)].Value = y.Status;
                            //groupfirst = item;
                        }
                        row++;
                       
                    }
 
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=EmployeeGroup-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());


                return View("EmployeeGroupQRCode",mymodel);

            }
            else
            {

                var grouplist = db.TbEmployeeGroupQR
                .Where(x=>x.PlantID.Equals(PlantID))
               .GroupBy(x => x.GroupID)
               .Select(group => group.Key)
               .ToList();

                // var collection = db.view_EmployeeGroup.ToList();
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("EmployeeGroup");
                Sheet.Cells["A1"].Value = "GroupID";
                Sheet.Cells["B1"].Value = "EmployeeID";
                Sheet.Cells["C1"].Value = "Status";
                int row = 2;

                var groupfirst = db.TbEmployeeGroupQR
                     .Where(x => x.PlantID.Equals(PlantID))
                .GroupBy(x => x.GroupID)
                .Select(group => group.Key)
                .ToList().First();
                var employeelist = "";
                foreach (var item in grouplist)
                {
                    employeelist = "";
                    var selectemployee = db.View_EmployeeGroup.Where(x => x.GroupID.Equals(item)).ToList();
                    foreach (var y in selectemployee)
                    {
                        employeelist = y.EmployeeID + "," + employeelist;
                        Sheet.Cells[string.Format("A{0}", row)].Value = item;
                        Sheet.Cells[string.Format("B{0}", row)].Value = employeelist;
                        Sheet.Cells[string.Format("C{0}", row)].Value = y.Status;
                        //groupfirst = item;
                    }
                    row++;

                }

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment; filename=EmployeeGroup-Master.xlsx"); // Fix typo ':' should be ';'
                Response.Body.WriteAsync(Ep.GetAsByteArray());


                ViewBag.InactiveStatus = true;
                return View("EmployeeGroupQRCode",mymodel);


            }
        }


        [HttpPost]
        public IActionResult EmployeeManagementGroupUpload(IFormFile FileUpload)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            int CntDb = Convert.ToInt32(db.TbEmployeeGroupQR.OrderBy(x => x.GroupID).Max(x => x.GroupID).ToString());
            int CntDbnext = CntDb;
          

            if (FileUpload == null || FileUpload.Length <= 0)
            {
                ViewBag.AlertMessage = "Please select a valid Excel file.";
                return View("EmployeeManagementGroup");
            }

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

                            if (Convert.ToInt32(worksheet.Cells[row, 3].Text) != 1 || Convert.ToInt32(worksheet.Cells[row, 3].Text) != 0)
                            {
                                int rowerror = row - 1;
                                TempData["AlertMessage"] = "Data Row : " + rowerror + " =>  Mistake please check Master ";
                                // ViewBag.Success = "Data Row : " + row + "=>  Mistake ";
                                return RedirectToAction("EmployeeManagementGroup");

                            }

                            var id = "";
                            var employeeList = new string[0];
                            employeeList = worksheet.Cells[row, 2].Text.Split(",");

                            if (worksheet.Cells[row, 1].Value != null)
                            {
                                id = worksheet.Cells[row, 1].Value.ToString().PadLeft(5, '0');

                            }
                            else
                            {
                                CntDbnext = CntDbnext + 1;
                            }

                            //Check Employee List                        
                           for (int j = 0; j < employeeList.Count(); j++)
                            {
                                if(employeeList[j] != "")
                                {

                                    string emp = employeeList[j].Trim();
                                  
                                var DataDb = db.TbEmployeeGroupQR.Where(x => x.GroupID.Equals(id) && x.EmployeeID.Equals(emp) && x.PlantID.Equals(PlantID)).SingleOrDefault();
                                var linedb = db.TbEmployeeMaster.Where(x => x.EmployeeID.Equals(emp) && x.PlantID.Equals(PlantID)).Select(x => x.LineID).SingleOrDefault();
                                var Sectiondb = db.TbEmployeeMaster.Where(x => x.EmployeeID.Equals(emp) && x.PlantID.Equals(PlantID)).Select(x => x.SectionID).SingleOrDefault();
                               
                                if (DataDb != null)
                                {

                                    //int Status;
                                    //if (worksheet.Cells[row, 4].Text == "Active")
                                    //{
                                    //    Status = 1;
                                    //}
                                    //else
                                    //{
                                    //    Status = 0;
                                    //}
                                    DataDb.EmployeeID = employeeList[j].Trim();// worksheet.Cells[row, 2].Value.ToString();// worksheet.Cells[row, 2].Text;
                                    DataDb.PlantID = PlantID;
                                    DataDb.LineID = linedb;
                                    DataDb.SectionID = Sectiondb;
                                    DataDb.Status = Convert.ToInt32(worksheet.Cells[row, 3].Value);
                                    DataDb.UpdateDate = DateTime.Now;
                                    DataDb.UpdateBy = EmpID; //User.Identity.Name;
                                }
                                else
                                {
                                    int Status;
                                    // 
                                    //if (worksheet.Cells[row, 4].Text == "Active")
                                    //{
                                    //    Status = 1;
                                    //}
                                    //else
                                    //{
                                    //    Status = 0;
                                    //}
                                    // Insert new record
                                    var newData = new TbEmployeeGroupQR
                                    {
                                        GroupID = Convert.ToString(CntDbnext).PadLeft(5, '0'),
                                        EmployeeID = employeeList[j].Trim(), // worksheet.Cells[row, 2].Value.ToString(),
                                        PlantID = PlantID,//int.Parse(worksheet.Cells[row, 5].Text),
                                        LineID = linedb,
                                        SectionID = Sectiondb,
                                        Status = Convert.ToInt32(worksheet.Cells[row, 3].Value),
                                        CreateDate = DateTime.Now,
                                        CreateBy = EmpID,//User.Identity.Name;
                                        UpdateDate = DateTime.Now,
                                        UpdateBy = EmpID//User.Identity.Name;

                                    };
                                    db.TbEmployeeGroupQR.Add(newData);
                                }
                                }
                            } // for

                        }


                    }

                    db.SaveChanges();
                }

            }

            ViewBag.Success = "Data imported and updated successfully!";
            return RedirectToAction("EmployeeGroupQRCode");

        }



        [HttpPost]
        public IActionResult EmployeeManagementGroupUpload2(IFormFile FileUpload)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            int CntDb = Convert.ToInt32(db.TbEmployeeGroupQR.OrderBy(x=>x.GroupID).Max(x=>x.GroupID).ToString());
            int CntDbnext = CntDb ;


            if (FileUpload == null || FileUpload.Length <= 0)
            {
                ViewBag.AlertMessage = "Please select a valid Excel file.";
                return View("EmployeeManagement");
            }

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

                            var id = "";
                            var employeeList = new string[0];
                            employeeList = worksheet.Cells[row, 2].Text.Split(",");
                            
                            if (worksheet.Cells[row, 1].Value != null)
                            {
                                id = worksheet.Cells[row, 1].Value.ToString().PadLeft(5, '0');

                            }
                            else
                            {
                                CntDbnext = CntDbnext + 1;
                            }

                            //Check Employee List                        
                            for(int j = 0; j < employeeList.Count(); j++)
                                {
                                    var DataDb = db.TbEmployeeGroupQR.Where(x => x.GroupID.Equals(id) && x.EmployeeID.Equals(employeeList[j].Trim())).SingleOrDefault();
                                    var linedb = db.TbEmployeeMaster.Where(x => x.EmployeeID.Equals(employeeList[j].Trim())).Select(x => x.LineID).SingleOrDefault();
                                    var Sectiondb = db.TbEmployeeMaster.Where(x => x.EmployeeID.Equals(employeeList[j].Trim())).Select(x => x.SectionID).SingleOrDefault();

                                    if (DataDb != null)
                                        {

                                                int Status;
                                                if (worksheet.Cells[row, 4].Text == "Active")
                                                {
                                                    Status = 1;
                                                }
                                                else
                                                {
                                                    Status = 0;
                                                }
                                                    DataDb.EmployeeID = employeeList[j].Trim();// worksheet.Cells[row, 2].Text;
                                                    DataDb.PlantID = PlantID;
                                                    DataDb.LineID = linedb;
                                                    DataDb.SectionID = Sectiondb;
                                                    DataDb.Status = Status;
                                                DataDb.UpdateDate = DateTime.Now;
                                                DataDb.UpdateBy = EmpID; //User.Identity.Name;
                                        }
                                        else
                                        {
                                                int Status;
                                               // 
                                                 if (worksheet.Cells[row, 4].Text == "Active")
                                                {
                                                    Status = 1;
                                                }
                                                else
                                                {
                                                    Status = 0;
                                                }
                                                // Insert new record
                                                var newData = new TbEmployeeGroupQR
                                             {
                                                    GroupID = Convert.ToString(CntDbnext).PadLeft(5, '0'),
                                                    EmployeeID = employeeList[j].Trim(),
                                                PlantID = PlantID,//int.Parse(worksheet.Cells[row, 5].Text),
                                                LineID = linedb,
                                                SectionID = Sectiondb,
                                                Status = Status,
                                                CreateDate = DateTime.Now,
                                                CreateBy = EmpID,//User.Identity.Name;
                                                UpdateDate = DateTime.Now,
                                                UpdateBy = EmpID//User.Identity.Name;

                                            };
                                            db.TbEmployeeGroupQR.Add(newData);
                                            }
                                } // for

                        }


                    }

                    db.SaveChanges();
                }
              
            }


        

            ViewBag.Success = "Data imported and updated successfully!";
            return RedirectToAction("EmployeeGroupQRCode");






        }



        [HttpPost]
        public IActionResult CreateQRCodeGroup(string employeeID) //(ViewModelAll qRCode)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));

            // string qrCodeText = employeeID.Trim();
            int qrCodeTextFirst = Convert.ToInt16(db.TbEmployeeGroupQR.Select(x => x.GroupID).Max() )+ 1;
            string qrCodeText = Convert.ToString(qrCodeTextFirst).PadLeft(5, '0');
            string labelText = $"Employee: {qrCodeText}";

            // Generate the QR code
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrBitmap = qrCode.GetGraphic(20);

            // Add label to the QR code
            using (Graphics g = Graphics.FromImage(qrBitmap))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAlias; // For smooth text
                Font font = new Font("Arial", 22, FontStyle.Bold);
                SolidBrush brush = new SolidBrush(System.Drawing.Color.Black);

                // Calculate where to draw the text
                float x = 10; // Left margin
                float y = qrBitmap.Height - 30; // Position at the bottom

                g.DrawString(labelText, font, brush, new System.Drawing.PointF(x, y)); // Draw the label
            }

            byte[] bitmapArray;
            using (MemoryStream stream = new MemoryStream())
            {
                qrBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                bitmapArray = stream.ToArray();
                ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(stream.ToArray());
            }

            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrcodes");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }


            // Construct the file path including the directory and file name
            string filePath = Path.Combine(directoryPath, $"{qrCodeText}_QRCode.png"); //qRCode.EmployeeID

            // Construct the URI for the image
            string QrUri = Url.Content("~/qrcodes/" + $"{qrCodeText}_QRCode.png"); //qRCode.EmployeeID
            // Save the image to the specified directory
            System.IO.File.WriteAllBytes(filePath, bitmapArray);


            //  string directoryPaths = Server.MapPath("~/qrcodes");

            // Assign the URI to the ViewBag
            ViewData["QrCodeUri"] = QrUri;
            ViewBag.EmployeeID = qrCodeText; //qRCode.EmployeeID;
            ViewBag.QRCodeGenerated = true;


            return Content(QrUri = QrUri, employeeID = qrCodeText);
           
        }




        // End of controller



    }


    //Extension method to convert Bitmap to Byte Array
    public static class BitmapExtension
    {
        public static byte[] BitmapToByteArray(this Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }

        public class ExportDataModel
        {
            public string Html { get; set; }
        }

}
