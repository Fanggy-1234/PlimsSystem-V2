﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plims.Data;
using Plims.Models;
using Plims.ViewModel;
using static QRCoder.PayloadGenerator.SwissQrCode;


namespace Plims.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext db;
        public HomeController(AppDbContext _db)
        {
            db = _db;
        }



    


        // Get Action
        public IActionResult Login()
        {

            if (HttpContext.Session.GetString("UserEmpID") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("UserInformation");
               // return RedirectToAction("Login");
            }
        }

        //Post Action
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(TbUser u)
        {
            var mymodel = new ViewModelAll
            {
                tbUser = db.TbUser.ToList(),
                view_User = db.View_User.ToList(),
                tbPermission = db.TbPermission.ToList(),
                tbPage = db.TbPage.ToList(),
                tbRole = db.TbRole.ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList()
            };

            if (HttpContext.Session.GetString("UserEmpID") == null)
            {              
                var obj = mymodel.view_User.Where(a => a.UserEmpID.Equals(u.UserEmpID) && a.UserPassword.Equals(u.UserPassword) && a.Status.Equals(1)).SingleOrDefault();
                if (obj != null)
                {
                    HttpContext.Session.SetString("UserEmpID", obj.UserEmpID.ToString());
                    HttpContext.Session.SetString("UserName", obj.UserName.ToString() + ' '+ obj.UserLastName.ToString());
                    HttpContext.Session.SetString("PlantID", obj.PlantID.ToString());
                    return RedirectToAction("UserInformation");
                }
                else
                {
                    var statusdata = mymodel.view_User.Where(a => a.UserEmpID.Equals(u.UserEmpID) && a.UserPassword.Equals(u.UserPassword)).SingleOrDefault();
                    if(statusdata != null)
                    {
                        ViewBag.Message = "Inactive account, please contact your admin!!";
                      
                    }
                    else
                    {
                        ViewBag.Message = "User or Password mistake!";
                    }
                    
                    return View();
                }
                    
            }
            else
            {
               // TempData["AlertMessage"] = "ลงทะเบียนไม่สำเร็จ! กรุณาตรวจสอบข้อมูล!";
                ViewBag.Message = "Connection loss please close and open program again!";
                return View();

            }
     

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserRegister(TbUser uservalue)
        {
            try
            {
                if (ModelState.IsValid && !checkusername(uservalue.UserEmpID))
                {

                    uservalue.Status = 1;
                    uservalue.CreateDate = DateTime.Now;
                    db.TbUser.Add(uservalue);
                    db.SaveChanges();
                    ViewBag.Message = "ลงทะเบียนสำเร็จ";
                }
                else
                {
                    ViewBag.Message = "ลงทะเบียนไม่สำเร็จ! กรุณาตรวจสอบข้อมูล";
                }

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        private bool checkusername(string uservalue)
        {
            if (db.TbUser.Any(x => x.UserName.ToString() == uservalue))
            {
                return false;
            }
            else
            {
                return true;

            }
        }


        public ActionResult Logout()
        {

            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserEmpID");

            return RedirectToAction("Login");
        }

        /// <summary>
        /// User Information
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UserInformation(View_PermissionMaster users)
        {
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            var mymodel = new ViewModelAll
            {
                tbUser = db.TbUser.Where(p => p.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x=>x.PlantID.Equals(PlantID)).ToList(),
                view_User =  db.View_User.ToList(),
                tbRole = db.TbRole.Where(p => p.PlantID.Equals(PlantID)).ToList(),

            };
            string IDuser = HttpContext.Session.GetString("UserEmpID").ToString();
             mymodel.view_PermissionMaster = mymodel.view_PermissionMaster.Where(x => x.UserEmpID.Equals(IDuser)).ToList();
           
            return View(mymodel);
        }

        /*///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////*///
        /*///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////*///
        /// <summary>
        /// User Management
        /// </summary>
        /// <returns></returns>
      
        [HttpGet]
        public ActionResult UserManagement(View_User obj, bool? inactivestatus)
        {
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));

            var Mymodel = new ViewModelAll
            {
                tbRole = db.TbRole.Where(p => p.PlantID.Equals(PlantID)).ToList(),
                tbUser = db.TbUser.Where(p => p.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_User = db.View_User.Where(p => p.PlantID.Equals(PlantID)).ToList(),
                tbPlants = db.TbPlant.Where(p => p.PlantID.Equals(PlantID)).ToList(),


            };

            ViewBag.VBRoleUserManagement = Mymodel.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(2)).Select(x => x.RoleAction).FirstOrDefault();

            if (!string.IsNullOrEmpty(obj.UserEmpID) || !string.IsNullOrEmpty(obj.UserName) || !string.IsNullOrEmpty(obj.UserLastName) || !string.IsNullOrEmpty(obj.RoleName) || inactivestatus != null)
            {
              

                // Where data filter
                if (!string.IsNullOrEmpty(obj.UserEmpID))
                {
                    Mymodel.view_User = db.View_User.Where(x => x.UserEmpID == obj.UserEmpID).OrderByDescending(x => x.Status).ToList();
                    ViewBag.SelectedUserEmpID = obj.UserEmpID;
                    ViewBag.InactiveStatus = true;
                }
                if(!string.IsNullOrEmpty(obj.UserName))
                {
                    Mymodel.view_User = Mymodel.view_User.Where(x => x.UserName == obj.UserName).OrderByDescending(x => x.Status).ToList();
                    ViewBag.SelectedUserName = obj.UserName;
                    ViewBag.InactiveStatus = true;
                }
                if (!string.IsNullOrEmpty(obj.UserLastName))
                {
                    Mymodel.view_User = Mymodel.view_User.Where(x => x.UserLastName == obj.UserLastName).OrderByDescending(x => x.Status).ToList();
                    ViewBag.SelectedUserLastName = obj.UserLastName;
                    ViewBag.InactiveStatus = true;
                }
                if (!string.IsNullOrEmpty(obj.RoleName))
                {
                    Mymodel.view_User = Mymodel.view_User.Where(x => x.RoleName == obj.RoleName).OrderByDescending(x => x.Status).ToList();
                    ViewBag.SelectedRoleName = obj.RoleName;
                }
                if (inactivestatus == false)
                {
                    Mymodel.view_User = Mymodel.view_User.Where(x => x.Status == 1).OrderByDescending(x => x.Status).ToList();
                    ViewBag.InactiveStatus = false;
                }

                return View(Mymodel);
            }

            else
            {

                ViewBag.InactiveStatus = true;
                return View(Mymodel);
            }

        }


        // Function  Edit Transaction : ฟังก์ชั่นนี้ใช่ร่วมกับ Update function
        [HttpGet]
        public JsonResult UserEdit(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));

            var users = db.View_User.Where(p=>p.ID.Equals(id)).SingleOrDefault();
            return Json(users);
        }
          public ActionResult UserClear()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));

            var Mymodel = new ViewModelAll
            {
                tbRole = db.TbRole.Where(p => p.PlantID.Equals(PlantID)).ToList(),
                tbUser = db.TbUser.Where(p => p.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.ToList(),
                view_User = db.View_User.Where(p=>p.PlantID.Equals(PlantID)).ToList(),

            };
            ViewBag.InactiveStatus = true;
            return RedirectToAction("UserManagement", "Home");
        }


        // Function  Inactive transaction
        public JsonResult UserInactive(int id)
        {
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var Empdb = db.TbUser.Where(p => p.ID.Equals(id)).SingleOrDefault();
            if (Empdb != null)
            {
                Empdb.Status = 0;
                Empdb.UpdateBy = EmpID;//User.Identity.Name;
                Empdb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbUser);

        }

        // Function  Active transaction
        public JsonResult UserActive(int id)
        {
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var Empdb = db.TbUser.Where(p => p.ID.Equals(id)).SingleOrDefault();
            if (Empdb != null)
            {
                Empdb.Status = 1;
                Empdb.UpdateBy = EmpID;//User.Identity.Name;
                Empdb.UpdateDate = DateTime.Now;
                db.SaveChanges();

            }

            return Json(db.TbUser);

        }


        [HttpPost]
        public ActionResult UserUpdate(TbUser obj )
        {
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            var Userdb = db.TbUser.Where(x => x.UserEmpID == obj.UserEmpID && x.PlantID.Equals(PlantID)).SingleOrDefault();
            if (obj.UserName != null)
            {
                Userdb.UserName = obj.UserName;

            }
            if (obj.UserLastName != null)
            {
                Userdb.UserLastName = obj.UserLastName;
            }
            if (obj.UserPassword != null)
            {
                //Check Password
                if (!isValidpassword(obj.UserPassword))
                {

                    TempData["AlertMessage"] = "- รหัสผ่านไม่ควรมีช่องว่าง <br/>" +
                                    "- รหัสผ่านควรมีอย่างน้อยหนึ่งหลัก (0-9) <br/>" +
                                    "- ความยาวของรหัสผ่านควรอยู่ระหว่าง 8 ถึง 15 ตัวอักษร <br/>" +
                                    "- รหัสผ่านควรมีอักษรตัวพิมพ์เล็กอย่างน้อยหนึ่งตัว (az) <br/>" +
                                    "- รหัสผ่านควรมีอักษรตัวพิมพ์ใหญ่ (AZ) อย่างน้อยหนึ่งตัว <br/>" +
                                    "- รหัสผ่านควรมีอักขระพิเศษอย่างน้อยหนึ่งตัว ( @, #, %, &, !, $, ฯลฯ...)";
                    return RedirectToAction("UserManagement");
                }

                Userdb.UserPassword = obj.UserPassword;
            }
            if (obj.UserPermission != 0)
            {
                Userdb.UserPermission = obj.UserPermission;
            }

            if (obj.UserEmail != null)
            {
                Userdb.UserEmail = obj.UserEmail;
            }
                if (obj.Status == 0)
                {
                    Userdb.Status = 0;
                }
                else
                {
                    Userdb.Status = 1;
                }



            Userdb.Lineconcern = "00001";
            Userdb.UpdateBy = EmpID;
            obj.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("UserManagement");
        }



        [HttpPost]
        public ActionResult PasswordEdit(TbUser obj)
        {
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            var Userdb = db.TbUser.Where(x => x.UserEmpID == EmpID && x.PlantID.Equals(PlantID)).SingleOrDefault();
          
            if (obj.UserPassword != null)
            {
                //Check Password
                if (!isValidpassword(obj.UserPassword))
                {

                    TempData["AlertMessage"] = "- รหัสผ่านไม่ควรมีช่องว่าง <br/>" +
                                    "- รหัสผ่านควรมีอย่างน้อยหนึ่งหลัก (0-9) <br/>" +
                                    "- ความยาวของรหัสมาก 8 ตัวอักษร <br/>" +
                                    "- รหัสผ่านควรมีอักษรตัวพิมพ์เล็กอย่างน้อยหนึ่งตัว (az) <br/>" +
                                    "- รหัสผ่านควรมีอักษรตัวพิมพ์ใหญ่ (AZ) อย่างน้อยหนึ่งตัว <br/>" +
                                    "- รหัสผ่านควรมีอักขระพิเศษอย่างน้อยหนึ่งตัว ( @, #, %, &, !, $, ฯลฯ...)";
                    return RedirectToAction("UserInformation");
                }
                string passwrd = obj.UserPassword.ToString();

                Userdb.UserPassword = passwrd;
            }

            Userdb.UpdateBy = EmpID;
            obj.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("UserInformation");
        }



        //  Function Create transaction
        [HttpPost]
        public ActionResult UserCreate(View_User obj)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            //Check Duplicate
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var userdb = db.TbUser.Where(x => x.UserEmpID.Equals(obj.UserEmpID) && x.PlantID.Equals(PlantID));
            string plantname = db.TbPlant.Where(x => x.PlantID.Equals(PlantID)).Select(x => x.PlantName).SingleOrDefault();
            int cnt = db.TbUser.Where(x => x.PlantID.Equals(PlantID)).Count();
            //Check Password
            if(!isValidpassword(obj.UserPassword))
            {

                TempData["AlertMessage"] = "- รหัสผ่านไม่ควรมีช่องว่าง <br/>" +
                                "- รหัสผ่านควรมีอย่างน้อยหนึ่งหลัก (0-9) <br/>" +
                                "- ความยาวของรหัสผ่านควรอยู่ระหว่าง 8 ถึง 15 ตัวอักษร <br/>" +
                                "- รหัสผ่านควรมีอักษรตัวพิมพ์เล็กอย่างน้อยหนึ่งตัว (az) <br/>" +
                                "- รหัสผ่านควรมีอักษรตัวพิมพ์ใหญ่ (AZ) อย่างน้อยหนึ่งตัว <br/>" +
                                "- รหัสผ่านควรมีอักขระพิเศษอย่างน้อยหนึ่งตัว ( @, #, %, &, !, $, ฯลฯ...)";
                return RedirectToAction("UserManagement");
            }
           // var roledb = db.TbRole.Where(x => x.RoleName == obj.RoleName.Trim()).SingleOrDefault();
            if (userdb.Count() == 0)
            {
                // Insert new Plant               
                db.TbUser.Add(new TbUser()
                {
                    //ID = db.TbUser.Count() + 1,
                    UserName = obj.UserName,
                    UserLastName = obj.UserLastName,
                    UserPassword = obj.UserPassword,
                    UserEmpID = plantname+ cnt.ToString().PadLeft(5,'0'), // obj.UserEmpID,
                    UserEmail = obj.UserEmail,
                    UserPermission = obj.RoleID,
                    PlantID = PlantID,
                    Lineconcern = "00001,00002,00003",
                    PasswordLastUpdate = DateTime.Today,
                    Status = Convert.ToInt32(obj.Status),
                    CreateDate = DateTime.Today,
                    CreateBy = EmpID,//User.Identity.Name,
                    UpdateDate = DateTime.Today,
                    UpdateBy = EmpID,//User.Identity.Name
                });
                db.SaveChanges();

            }
            else
            {
                TempData["AlertMessage"] = "User is Duplicate!";
               // ViewBag.Error = "User is Duplicate!";
            }
            return RedirectToAction("UserManagement");
        }


        // A utility function to check
        // whether a password is valid or not
        public static bool isValidpassword(String password)
        {

            // for checking if password length
            // is between 8 and 15
            if (!((password.Length >= 8)
                && (password.Length <= 15)))
            {
                return false;
            }

            // to check space
            if (password.Contains(" "))
            {
                return false;
            }
            if (true)
            {
                int count = 0;

                // check digits from 0 to 9
                for (int i = 0; i <= 9; i++)
                {

                    // to convert int to string
                    String str1 = i.ToString();

                    if (password.Contains(str1))
                    {
                        count = 1;
                    }
                }
                if (count == 0)
                {
                    return false;
                }
            }

            // for special characters
            if (!(password.Contains("@") || password.Contains("#")
                || password.Contains("!") || password.Contains("~")
                || password.Contains("$") || password.Contains("%")
                || password.Contains("^") || password.Contains("&")
                || password.Contains("*") || password.Contains("(")
                || password.Contains(")") || password.Contains("-")
                || password.Contains("+") || password.Contains("/")
                || password.Contains(":") || password.Contains(".")
                || password.Contains(", ") || password.Contains("<")
                || password.Contains(">") || password.Contains("?")
                || password.Contains("|")))
            {
                return false;
            }

            if (true)
            {
                int count = 0;

                // checking capital letters
                for (int i = 65; i <= 90; i++)
                {

                    // type casting
                    char c = (char)i;

                    String str1 = c.ToString();
                    if (password.Contains(str1))
                    {
                        count = 1;
                    }
                }
                if (count == 0)
                {
                    return false;
                }
            }

            if (true)
            {
                int count = 0;

                // checking small letters
                for (int i = 97; i <= 122; i++)
                {

                    // type casting
                    char c = (char)i;
                    String str1 = c.ToString();

                    if (password.Contains(str1))
                    {
                        count = 1;
                    }
                }
                if (count == 0)
                {
                    return false;
                }
            }

            // if all conditions fails
            return true;
        }



        // User Add New
        public ActionResult UserManagementsave(TbUser user)
        {
            bool status = false;

            if (ModelState.IsValid)
            {
               
                    db.TbUser.Add(user);
                    db.SaveChanges();
                    status = true;

            }
            return RedirectToAction("UserManagementsave");
            //return new JsonResult { Data = new { status = status } };
        }


        /*///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////*///
        /*///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////*///

        /// <summary>
        /// Role Management
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult Role()
        {

            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var role = new ViewModelAll
            {
                tbPage = db.TbPage.ToList(),
                tbRole = db.TbRole.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbPermission = db.TbPermission.Where(x=>x.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PagePermission = db.View_PagePermission.Where(x => x.PlantID.Equals(PlantID)).ToList(),

            };


            return View(role);

        }
        public ActionResult RoleManagement()
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");

            var role = new ViewModelAll
            {
                tbPage = db.TbPage.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbRole = db.TbRole.Where(x=>x.PlantID.Equals(PlantID)).ToList(),
                tbPermission = db.TbPermission.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PagePermission = db.View_PagePermission.Where(x => x.PlantID.Equals(PlantID)).ToList(),

            };
            ViewBag.VBRoleRole = role.view_PermissionMaster.Where(x => x.UserEmpID == EmpID && x.PageID.Equals(4)).Select(x => x.RoleAction).FirstOrDefault();


            return View(role);

        }

        [HttpPost]
        public ActionResult RoleNameCreate(string RoleNames)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            //Check Duplicate
            var Roledb = db.View_PagePermission.Where(x => x.RoleName.Equals(RoleNames));
            //var roledb = db.TbRole.Where(x => x.RoleName == obj.RoleName.Trim()).SingleOrDefault();
            int roleParm = db.TbRole.Count() + 1;
            if (Roledb.Count() == 0)
            {
                db.TbRole.Add(new TbRole()
                {
                    RoleID = roleParm,
                    RoleName = RoleNames,
                    PlantID = PlantID,
                    RoleStatus = 1,
                    CreateDate = DateTime.Today,
                    CreateBy = EmpID,//User.Identity.Name,
                    UpdateDate = DateTime.Today,
                    UpdateBy = EmpID,//User.Identity.Name
                });


                    for (int i = 1; i <= db.TbPageMaster.Count(); i++)
                {
                        db.TbPermission.Add(new TbPermission()
                        {
                            RoleID = roleParm,
                            PageID = i,
                            RoleAction = "No",
                            PlantID = PlantID,
                            CreateDate = DateTime.Today,
                            CreateBy = EmpID,//User.Identity.Name,
                            UpdateDate = DateTime.Today,
                            UpdateBy = EmpID,//User.Identity.Name

                        });
                    }

                db.SaveChanges();




            }
            else
            {
                TempData["AlertMessage"] = "Role is Duplicate!";
               // ViewBag.Error = "Role is Duplicate!";
            }
            return Json(new { success = true, message = "Operation successful" });
}



        [HttpPost]
        public ActionResult RoleCreateSave([FromBody] ViewModelAll request)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            foreach (var perm in request.Permissions)
           {

                if (perm.PermissionValue != "No")
                {
                    var pageID = db.TbPage.Where(x => x.PageName.Equals(perm.PageName) && x.PlantID.Equals(PlantID)).SingleOrDefault();
                    int page = pageID.PageID;
                    int role = Convert.ToInt32(request.RoleID);
                    var permdb = db.TbPermission.Where(x => x.RoleID.Equals(role) && x.PageID.Equals(page) && x.PlantID.Equals(PlantID)).SingleOrDefault();
                    if (permdb != null)
                    {
                        permdb.RoleAction = perm.PermissionValue;
                        permdb.PlantID = PlantID;
                        permdb.UpdateBy = EmpID; //User.Identity.Name;
                        permdb.UpdateDate = DateTime.Now;
                    }
               } 
                    
              // Console.WriteLine($"PageName: {permission.PageName}, PermissionValue: {permission.PermissionValue}");
            }
            db.SaveChanges();
            return RedirectToAction("RoleManagement");
        }


        [HttpGet]
        public JsonResult RoleEdit(int id)
        {
            int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var roledb = db.View_PagePermission
                   .Where(x => x.RoleID.Equals(id) && x.PlantID.Equals(PlantID))
                   .Select(x => new
                   {
                       RoleID = x.RoleID,
                       RoleName = x.RoleName,
                       RoleAction = x.RoleAction,
                       PageName = x.PageName
                       // Add other properties as needed
                   })
                   .ToArray();

            //   ViewBag.groupid = 00001;
            return Json(roledb);

        }

        [HttpPost]
        public ActionResult RoleEditSave(ViewModelAll obj, string[] PageNames, string[] RoleActions , int RoleID , string RoleName)
        {
            int cntrole = 0;
             int PlantID = Convert.ToInt32(HttpContext.Session.GetString("PlantID"));
            string EmpID = HttpContext.Session.GetString("UserEmpID");
            var mymodel = new ViewModelAll
            {
                tbPage = db.TbPage.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbRole = db.TbRole.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                tbPermission = db.TbPermission.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PermissionMaster = db.View_PermissionMaster.Where(x => x.PlantID.Equals(PlantID)).ToList(),
                view_PagePermission = db.View_PagePermission.Where(x => x.PlantID.Equals(PlantID)).ToList(),
            };

            foreach (var role in RoleActions)
            {
                cntrole++;
                var Roleperdb = db.TbPermission.Where(x => x.RoleID == RoleID && x.PageID == cntrole && x.PlantID.Equals(PlantID)).SingleOrDefault();
                if(Roleperdb != null)
                {    
                Roleperdb.RoleAction = RoleActions[cntrole -1];
                Roleperdb.UpdateBy = EmpID;// User.Identity.Name;
                Roleperdb.UpdateDate = DateTime.Now;

                }
                else
                {
                   

                    return RedirectToAction("RoleManagement");
                }

                
            }

            db.SaveChanges();

            return RedirectToAction("RoleManagement");

        }



    }
}
