using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ImportMasterWolf.Models.Common;
using ImportMasterWolf.Models.DBConnect;
using ImportMasterWolf.Models.Table.HRMS;
using ImportMasterWolf.Models.Table.IT;
using ImportMasterWolf.Models.Table.LAMP;
using ImportMasterWolf.Models.Table.WolfApproveCore_thaistanley;

namespace ImportMasterWolf.Controllers.Account
{
    public class LoginController : Controller
    {
        private LAMP _LAMP;
        private HRMS _HRMS;
        private IT _IT;
        private WolfApproveCore_thaistanley _WolfApproveCore_thaistanley;
        private CacheSettingController _Cache;
        public LoginController(LAMP lamp, HRMS hrms, IT it, WolfApproveCore_thaistanley WolfApproveCore_thaistanley, CacheSettingController cacheController)
        {
            _LAMP = lamp;
            _HRMS = hrms;
            _IT = it;
            _WolfApproveCore_thaistanley = WolfApproveCore_thaistanley;
            _Cache = cacheController;
        }
        public IActionResult Index(string req)
        {
            string remember = User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            Class @class = new Class();
            if (req != null)
                @class.param = req;

            //ViewBag.userb = User.Identity.Name;

            @class._ViewMSTATACCEmployee = new ViewMSTATACCEmployee();
            @class._ViewMSTATACCEmployee = _WolfApproveCore_thaistanley._ViewMSTATACCEmployee.Where(x => x.EMPCODE == "015142").FirstOrDefault();


            if (remember != null)
            {
                return RedirectToAction("RememberMe", "Login", @class);
            }
            return View(@class);
        }

        [HttpPost]
        public async Task<IActionResult> Autherize(Class @class)
        {
            ViewLoginPgm login = new ViewLoginPgm();
            string sUsername = @class._ViewLoginPgm.UserId.Trim();
            string sPassword = @class._ViewLoginPgm.Password.Trim();

            ViewAccEMPLOYEE accData = new ViewAccEMPLOYEE();
            ViewLoginPgm _ViewLoginPgm = _IT._ViewLoginPgm.FirstOrDefault(x => x.UserId == sUsername && x.Password == sPassword && x.Program == "ImportDataWolf");
            if (_ViewLoginPgm != null)
            {
                accData = _HRMS.AccEMPLOYEE.FirstOrDefault(x => x.EMP_CODE == _ViewLoginPgm.Empcode);
                string[] stat = await Task.Run(() => SetClaim(accData, _ViewLoginPgm));
                if (stat[0] == "Ok")
                {

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    @class._Error = new Error();
                    @class._Error.validation = stat[1];
                    return View("Index", @class);
                }
            }
            else
            {
                @class._Error = new Error();
                @class._Error.validation = "Username or Password invalid";
                return View("Index", @class);
            }

        }

        public async Task<IActionResult> RememberMe(Class @class)
        {
            try
            {
                ViewLoginPgm login = new ViewLoginPgm();
                string sUsername = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Username")?.Value;
                string sPassword = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Password")?.Value;
                ViewLoginPgm _ViewLoginPgm = _IT._ViewLoginPgm.FirstOrDefault(x => x.UserId == sUsername && x.Password == sPassword && x.Program == "ImportDataWolf");
                ViewAccEMPLOYEE accData = new ViewAccEMPLOYEE();
                accData = _HRMS.AccEMPLOYEE.FirstOrDefault(x => x.EMP_CODE == _ViewLoginPgm.Empcode);
                string[] stat = await Task.Run(() => SetClaim(accData, _ViewLoginPgm));
                if (stat[0] == "Ok")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return RedirectToAction("Logout");
            }
        }

        public IActionResult Logout()
        {
            this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData.Clear();
            return RedirectToAction("Index", "Login");
        }

        public async Task<string[]> SetClaim(ViewAccEMPLOYEE accdata, ViewLoginPgm _ViewLoginPgm)
        {
            try
            {
                ViewAccEMPLOYEE acc = new ViewAccEMPLOYEE();
                string Email = "";


                _Cache.clearCacheAccEmployee();
                acc = await Task.Run(() => _Cache.cacheAccEmployee().FirstOrDefault(s => s.EMP_CODE == accdata.EMP_CODE));
                ViewrpEmail Emails = _Cache.cacheEmail().Where(w => w.emEmail_M365 != null && w.emEmpcode.Trim() == acc.EMP_CODE.Trim()).FirstOrDefault();
                if (Emails is null)
                {
                    Email = GlobalVariable.AdminEmail;
                }
                else
                {
                    Email = _Cache.cacheEmail().Where(w => w.emEmpcode.Trim() == acc.EMP_CODE.Trim()).FirstOrDefault().emEmail_M365.Trim();
                }


                acc.DIVI_CODE = await Task.Run(() => _Cache.cacheAccDIVIMast().Where(w => w.DIVI_CODE == acc.DIVI_CODE).FirstOrDefault().DIVI_NAME);
                acc.DEPT_CODE = await Task.Run(() => _Cache.cacheDEPTMast().Where(w => w.DEPT_CODE == acc.DEPT_CODE).FirstOrDefault().DEPT_NAME);
                acc.SEC_CODE = await Task.Run(() => _Cache.cacheSECMast().Where(w => w.SEC_CODE == acc.SEC_CODE).FirstOrDefault().SEC_NAME);
                acc.GRP_CODE = await Task.Run(() => _Cache.cacheGRPMast().Where(w => w.GRP_CODE == acc.GRP_CODE).FirstOrDefault().GRP_NAME);

                acc.LAST_TNAME = acc.LAST_TNAME is null ? "" : acc.LAST_TNAME;
                acc.NICKNAME = acc.EMP_ENAME.Substring(0, 1) + (acc.LAST_ENAME == "" ? "" : acc.LAST_ENAME.Substring(0, 1))?.ToString();



                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Country, "ImportMasterWolf"));
                claims.Add(new Claim(ClaimTypes.Name, acc.EMP_CODE.ToString()));
                claims.Add(new Claim(ClaimTypes.Actor, acc.EMP_TNAME + " " + acc.LAST_TNAME));
                claims.Add(new Claim("UserId", acc.EMP_CODE.ToString()));

                claims.Add(new Claim("Username", _ViewLoginPgm.UserId?.ToString()));
                claims.Add(new Claim("Password", _ViewLoginPgm.Password?.ToString()));

                claims.Add(new Claim("EmpCode", acc.EMP_CODE?.ToString()));
                claims.Add(new Claim("Permission", _ViewLoginPgm.Permission?.ToString()));
                // claims.Add(new Claim(ClaimTypes.Role, login.Permission?.ToString()));
                claims.Add(new Claim("Division", acc.DIVI_CODE.ToUpper()));
                claims.Add(new Claim("Department", acc.DEPT_CODE.ToUpper()));
                claims.Add(new Claim("DeptCode", acc.DEPT_CODE.ToUpper()));
                claims.Add(new Claim("Section", acc.SEC_CODE.ToUpper()));
                claims.Add(new Claim("Group", acc.GRP_CODE.ToUpper()));
                claims.Add(new Claim("Unit", acc.UNT_CODE.ToUpper()));
                claims.Add(new Claim("Position", acc.POS_CODE.ToUpper()));
                claims.Add(new Claim("ProgramName", GlobalVariable.ProgramName));
                claims.Add(new Claim("PriName", acc.PRI_THAI));
                claims.Add(new Claim("Name", acc.EMP_TNAME));
                claims.Add(new Claim("SurName", acc.LAST_TNAME));
                claims.Add(new Claim("NICKNAME", acc.NICKNAME.ToUpper()));
                claims.Add(new Claim("Email", Email));

                //ClaimsIdentity identity = new ClaimsIdentity(claims,
                //    CookieAuthenticationDefaults.AuthenticationScheme);
                //ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                //await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                //    , principal, new AuthenticationProperties()
                //    {
                //        IsPersistent = true,
                //        AllowRefresh = true, // ✅ อนุญาตให้ session ถูกยืดอายุ
                //        ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
                //    }); //true is remember login

                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,   // ✅ คงอยู่แม้ปิด browser (ขึ้นอยู่กับ ExpireTimeSpan)
                        AllowRefresh = true    // ✅ อนุญาตให้ cookie ถูกรีเฟรชตาม SlidingExpiration
                                               // ❌ ไม่ต้องใส่ ExpiresUtc ที่นี่ ถ้าใช้ SlidingExpiration แล้ว
                    });


                string[] stat = { "Ok" };
                return stat;
            }
            catch (Exception ex)
            {
                string[] stat = { "NG", ex.Message };
                return stat;
            }
        }

        public IActionResult SignOut()
        {
            this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _Cache.clearCacheAccEmployee();
            TempData.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}