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
using ImportMasterWolf.Models.Table.WolfApproveCore_Center;


using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;

namespace ImportMasterWolf.Controllers.Account
{
    public class LoginController : Controller
    {
        private LAMP _LAMP;
        private HRMS _HRMS;
        private IT _IT;
        private WolfApproveCore_thaistanley _WolfApproveCore_thaistanley;
        private CacheSettingController _Cache;
        private WolfApproveCore_Center _WolfApproveCore_Center;

        public LoginController(LAMP lamp, HRMS hrms, IT it, WolfApproveCore_thaistanley WolfApproveCore_thaistanley, WolfApproveCore_Center WolfApproveCore_Center, CacheSettingController cacheController)
        {
            _LAMP = lamp;
            _HRMS = hrms;
            _IT = it;
            _WolfApproveCore_thaistanley = WolfApproveCore_thaistanley;
            _Cache = cacheController;
            _WolfApproveCore_Center = WolfApproveCore_Center;
        }
        public IActionResult Index(string req, string key)
        {
            string remember = User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            Class @class = new Class();
            if (req != null)
                @class.param = req;

            //var source = Request.Headers["X-Request-Source"];
            //var secret = Request.Headers["X-Secret-Key"];
            //if (source != "InternalBatch" && secret != "Wolf2026Secure" && key != null)
            //{
            //    //dep divi 
            //    if (key == "UpdateDepWolf") { RunDep(key); }
            //    //employee
            //    if (key == "AddDataWolf") { RunImport(key); }
            //    if (key == "UpdateDataWolf") { RunImport(key); }
            //}
            //dep divi 
            if (key == "UpdateDepWolf") { RunDep(key); }
            //employee
            if (key == "AddDataWolf") { RunImport(key); }
            if (key == "UpdateDataWolf") { RunImport(key); }


            if (remember != null)
            {
                return RedirectToAction("RememberMe", "Login", @class);
            }
            return View(@class);
        }


        private void RunDep(string vWolf)
        {
            try
            {
                Class @class = new Class();
                @class._ListViewMSTDivision = new List<ViewMSTDivision>();
                @class._ListViewMSTDivision = UpdateTBMSTDivision();
                ////MSTDepartment
                @class._ListViewMSTDepartment = new List<ViewMSTDepartment>();
                @class._ListViewMSTDepartment = UpdateTBMSTDepartment(@class._ListViewMSTDivision);
                ////MSTPosition
                @class._ListViewMSTPosition = new List<ViewMSTPosition>();
                @class._ListViewMSTPosition = UpdateTBMSTPosition();

                saveDataDivi(@class, "");
            }
            catch (Exception ex) { string msgr = ex.Message; }



        }
        public List<ViewMSTDivision> UpdateTBMSTDivision()
        {
            List<ViewMSTDivision> _listViewMSTDivision = new List<ViewMSTDivision>();
            try
            {
                string IssueBy = "WolfAdmin";// HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                //'DC','LE','AD','ST' เอา dep มาด้วย ยกเว้น TMP


                var dvCodes = new[] { "DC", "LE", "AD", "ST" };
                int vrow = 1;
                var mastList = _HRMS.AccDIVIMAST.OrderBy(x => x.DIVI_NO).AsEnumerable().Select(x => new ViewMSTDivision
                {
                    DivisionId = vrow++, // หรือใช้ ID จริงจาก table
                    NameTh = x.DIVI_NAME,
                    NameEn = x.DIVI_NAME,
                    ModifiedBy = IssueBy,
                    IsActive = true,
                    AccountId = 1,
                    DivisionCode = x.DIVI_NAME.Contains("-") ? x.DIVI_NAME.Substring(0, x.DIVI_NAME.IndexOf("-")) : x.DIVI_NAME.Contains("NONE") ? "N" : x.DIVI_NAME
                })
                                .ToList();



                var startId = mastList.Count + 1;

                var deptList = _HRMS.AccEMPLOYEE
         .Where(x => dvCodes.Contains(x.DIVI_CODE)
                  && string.IsNullOrEmpty(x.QUIT_CODE)
                  && x.DEPT_CODE != "TMP"
                  && x.DEPT_CODE != "N")
         .Join(
             _HRMS.AccDEPTMAST,
             emp => emp.DEPT_CODE,
             dept => dept.DEPT_CODE,
             (emp, dept) => dept.DEPT_NAME
         )
         .Distinct()
         .AsEnumerable()   // 🔴 จุดสำคัญ ตัดจาก IQueryable → IEnumerable
         .Select((name, i) => new ViewMSTDivision
         {
             DivisionId = startId + i,
             NameTh = name,
             NameEn = name,
             ModifiedBy = IssueBy,
             IsActive = true,
             AccountId = 1,
             DivisionCode = name.Contains("-")
                 ? name.Substring(0, name.IndexOf("-"))
                 : name.Contains("NONE") ? "N" : name
         })
         .ToList();

                var MastviewMSTDivisions = mastList
                    .Concat(deptList)
                    .ToList();

                //_listViewMSTDivision = MastviewMSTDivisions;
                _listViewMSTDivision = _WolfApproveCore_thaistanley._ViewMSTDivision.ToList();
                var existingMSTDivision = _listViewMSTDivision.ToDictionary(x => x.DivisionCode);
                int nextDivis = _listViewMSTDivision.Any() ? _listViewMSTDivision.Max(x => x.DivisionId) + 1 : 1;
                foreach (var newItem in MastviewMSTDivisions)
                {
                    if (existingMSTDivision.TryGetValue(newItem.DivisionCode, out var oldItem))
                    {

                    }
                    else
                    {
                        // Add
                        newItem.DivisionId = nextDivis++;
                        //newItem.Userid_Line = "New";
                        _listViewMSTDivision.Add(newItem);
                    }
                }



            }
            catch (Exception ex)
            {
                string msge = ex.Message;
            }

            return _listViewMSTDivision;
        }

       

        public List<ViewMSTDepartment> UpdateTBMSTDepartment(List<ViewMSTDivision> _ListViewMSTDivision)
        {
            List<ViewMSTDepartment> _listViewMSTDepartment = new List<ViewMSTDepartment>();
            List<ViewMSTDepartment> _ListMSTDepartment = new List<ViewMSTDepartment>();
            try
            {
                //string IssueBy = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                string IssueBy = "WolfAdmin";
                var ViewMSTDivision = _ListViewMSTDivision.ToDictionary(x => x.DivisionCode);
                var ListviewMSTDepartments = _HRMS.AccDEPTMAST.OrderBy(x => x.DeptNo).ToList();
                var ListMSTDivision = _ListViewMSTDivision.ToList();

                var divisionDict = _ListViewMSTDivision.ToDictionary(x => x.DivisionCode);
                var deptList = _HRMS.AccDEPTMAST.OrderBy(x => x.DeptNo).ToList(); // 👈 ตัด EF ตรงนี้


                //list dep ปัจจุบันใน WOLF

                _ListMSTDepartment = _WolfApproveCore_thaistanley._ViewMSTDepartment.ToList();
                var _DepartmentDict = _ListMSTDepartment.ToDictionary(x => x.DepartmentCode);
                var _DEPTMASTList = _HRMS.AccDEPTMAST.OrderBy(x => x.DeptNo).ToList();


                foreach (var dept in _DEPTMASTList)
                {
                    // ถ้าใน Dictionary 'ไม่มี' รหัสแผนกนี้อยู่ (ใช้ .ContainsKey)
                    if (!_DepartmentDict.ContainsKey(dept.DEPT_CODE))
                    {
                        var listDivisionId = _DEPTMASTList
                                         .Select(item =>
                                         {
                                             var key = item.DEPT_CODE == "TMP" ? "LE" : item.DEPT_CODE;

                                             return new { item, key };
                                         })
                                         .Where(x => divisionDict.TryGetValue(x.key, out _))
                                         .Select(x => divisionDict[x.key]).FirstOrDefault();


                        _ListMSTDepartment.Add(new ViewMSTDepartment
                        {
                            DepartmentId = _ListMSTDepartment.Max(x => x.DepartmentId) + 1,
                            ParentId = 0,
                            DivisionId = listDivisionId.DivisionId,
                            DepartmentCode = dept.DEPT_CODE,
                            NameTh = dept.DEPT_NAME,
                            NameEn = dept.DEPT_NAME,
                            //CreatedDate = IssueBy,
                            CreatedBy = IssueBy,
                            // ModifiedDate
                            ModifiedBy = IssueBy,
                            IsActive = true,
                            AccountId = 1,
                            CompanyCode = "01111"

                        });

                    }
                }



                //int vrow = 1;
                ////_listViewMSTDepartment = deptList
                ////      .Where(x => divisionDict.ContainsKey(x.DEPT_CODE)) TMP
                ////    // .Where(x => divisionDict.ContainsKey(x.DEPT_CODE.Contains("TMP") ? "LE" : x.DEPT_CODE))
                ////    .Select(item => new ViewMSTDepartment
                ////    {
                ////        DepartmentId = vrow++,
                ////        ParentId = 0,
                ////        DivisionId = divisionDict[item.DEPT_CODE].DivisionId,
                ////        DepartmentCode = item.DEPT_CODE,
                ////        NameTh = item.DEPT_NAME,
                ////        NameEn = item.DEPT_NAME,
                ////        CreatedBy = IssueBy,
                ////        ModifiedBy = IssueBy,
                ////        IsActive = true,
                ////        AccountId = 1,
                ////        CompanyCode = "01111" //01111
                ////    })
                ////    .ToList();

                //_listViewMSTDepartment = deptList
                //                        .Select(item =>
                //                        {
                //                            var key = item.DEPT_CODE == "TMP" ? "LE" : item.DEPT_CODE;

                //                            return new { item, key };
                //                        })
                //                        .Where(x => divisionDict.TryGetValue(x.key, out _))
                //                        .Select(x =>
                //                        {
                //                            var division = divisionDict[x.key];

                //                            return new ViewMSTDepartment
                //                            {
                //                                DepartmentId = vrow++,
                //                                ParentId = 0,
                //                                DivisionId = division.DivisionId,
                //                                DepartmentCode = x.item.DEPT_CODE,
                //                                NameTh = x.item.DEPT_NAME,
                //                                NameEn = x.item.DEPT_NAME,
                //                                CreatedBy = IssueBy,
                //                                ModifiedBy = IssueBy,
                //                                IsActive = true,
                //                                AccountId = 1,
                //                                CompanyCode = "01111"
                //                            };
                //                        })
                //                        .ToList();



                //foreach (var item in ListviewMSTDepartments)
                //{
                //    var vid = _ListViewMSTDivision.Where(x => x.DivisionCode == item.DEPT_CODE).ToList().FirstOrDefault();
                //    _listViewMSTDepartment.Add(new ViewMSTDepartment
                //    {
                //        DepartmentId = 1,
                //        ParentId = 0,
                //        DivisionId = vid.DivisionId,
                //        DepartmentCode = item.DEPT_CODE,
                //        NameTh = item.DEPT_NAME,
                //        NameEn = item.DEPT_NAME,
                //        // CreatedDate
                //        CreatedBy = IssueBy,
                //        //ModifiedDate
                //        ModifiedBy = IssueBy,
                //        IsActive = true,
                //        AccountId = 1,
                //        //LeaderId
                //        CompanyCode = "01111"

                //    });
                //}

                //var viewMSTDepartments = _HRMS.AccDEPTMAST.ToDictionary(x => x.DEPT_CODE, x => x.DEPT_NAME);



                //_listViewMSTDepartment = _WolfApproveCore_thaistanley._ViewMSTDepartment.ToList();

            }
            catch (Exception ex)
            {
                string msgr = ex.Message;
            }

            return _ListMSTDepartment;//   _listViewMSTDepartment;
        }
        public List<ViewMSTPosition> UpdateTBMSTPosition()
        {
            List<ViewMSTPosition> _listViewMSTPosition = new List<ViewMSTPosition>();
            try
            {
               // string IssueBy = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                string IssueBy = "WolfAdmin";
                int vrow = 1;
                int vProw = 1;


                _listViewMSTPosition = _WolfApproveCore_thaistanley._ViewMSTPosition.ToList();


                //var _listViewAccPOSMAST = _HRMS.AccPOSMAST.OrderByDescending(x => int.Parse(x.POS_HCM_CODE)).AsEnumerable()
                //    .Select(x => new ViewMSTPosition
                //    {
                //        PositionId = vrow++,
                //        NameTh = x.POS_NAME,
                //        NameEn = x.POS_NAME,
                //        PositionLevelId = vProw++,
                //        IsActive = true,
                //        //CreatedDate
                //        CreatedBy = IssueBy,
                //        //ModifiedDate
                //        //ModifiedBy 
                //        AccountId = 1,
                //        CompanyCode = "1",

                //    })
                //    .ToList();

                //_listViewMSTPosition = _WolfApproveCore_thaistanley._ViewMSTPosition.ToList();
                //_listViewMSTPosition = _listViewAccPOSMAST;
            }
            catch (Exception ex)
            {
                string msgr = ex.Message;
            }

            return _listViewMSTPosition;
        }


        public string[] saveDataDivi(Class @class, string tsave)
        {
            string config = "S";
            string msg = "Save Data Success !!!";
            string IssueBy = "WolfAdmin";// HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            using (var dbContextTransaction = _WolfApproveCore_thaistanley.Database.BeginTransaction())
            {
                try
                {
                    var connection = (SqlConnection)_WolfApproveCore_thaistanley.Database.GetDbConnection();
                    if (connection.State != ConnectionState.Open)
                        connection.Open();


                    _WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("TRUNCATE TABLE MSTDivision");
                    using (var bulkCopy1 = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, (SqlTransaction)dbContextTransaction.GetDbTransaction()))
                    {
                        bulkCopy1.DestinationTableName = "MSTDivision";
                        bulkCopy1.BatchSize = 5000;
                        bulkCopy1.BulkCopyTimeout = 300;
                        DataTable dt = new DataTable();
                        // dt.Columns.Add("DivisionId", typeof(int));
                        dt.Columns.Add("NameTh", typeof(string));
                        dt.Columns.Add("NameEn", typeof(string));
                        dt.Columns.Add("CreatedDate", typeof(DateTime));
                        dt.Columns.Add("CreatedBy", typeof(string));
                        dt.Columns.Add("ModifiedDate", typeof(DateTime));
                        dt.Columns.Add("ModifiedBy", typeof(string));
                        dt.Columns.Add("IsActive", typeof(bool));
                        dt.Columns.Add("AccountId", typeof(int));
                        dt.Columns.Add("DivisionCode", typeof(string));

                        foreach (var item in @class._ListViewMSTDivision)
                        {
                            dt.Rows.Add(
                                // item.EmployeeId,
                                item.NameTh,
                                item.NameEn,
                                DateTime.Now,
                                "WolfAdmin",
                                DateTime.Now,//item.CreatedDate,
                                "",//item.CreatedBy,
                                item.IsActive,//item.ModifiedDate,
                                item.AccountId,
                                item.DivisionCode


                            //""//item.Userid_Line
                            );
                        }

                        // Mapping columns
                        // bulkCopy2.ColumnMappings.Add("EmployeeId", "EmployeeId");
                        bulkCopy1.ColumnMappings.Add("NameTh", "NameTh");
                        bulkCopy1.ColumnMappings.Add("NameEn", "NameEn");
                        bulkCopy1.ColumnMappings.Add("CreatedDate", "CreatedDate");
                        bulkCopy1.ColumnMappings.Add("CreatedBy", "CreatedBy");
                        bulkCopy1.ColumnMappings.Add("ModifiedDate", "ModifiedDate");
                        bulkCopy1.ColumnMappings.Add("ModifiedBy", "ModifiedBy");
                        bulkCopy1.ColumnMappings.Add("IsActive", "IsActive");
                        bulkCopy1.ColumnMappings.Add("AccountId", "AccountId");
                        bulkCopy1.ColumnMappings.Add("DivisionCode", "DivisionCode");

                        // Bulk insert
                        bulkCopy1.WriteToServer(dt);
                    }





                    //_WolfApproveCore_thaistanley.SaveChanges();
                    //@class._ListViewMSTDepartment
                    _WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("TRUNCATE TABLE MSTDepartment");
                    for (int i = 0; i < @class._ListViewMSTDepartment.Count(); i++)
                    {
                        var viewMSTDepartment = new ViewMSTDepartment();
                        //DepartmentId
                        viewMSTDepartment.ParentId = @class._ListViewMSTDepartment[i].ParentId;
                        viewMSTDepartment.DivisionId = @class._ListViewMSTDepartment[i].DivisionId;
                        viewMSTDepartment.DepartmentCode = @class._ListViewMSTDepartment[i].DepartmentCode;
                        viewMSTDepartment.NameTh = @class._ListViewMSTDepartment[i].NameTh;
                        viewMSTDepartment.NameEn = @class._ListViewMSTDepartment[i].NameEn;
                        viewMSTDepartment.CreatedDate = DateTime.Now; //@class._ListViewMSTDepartment[i].ParentId;
                        viewMSTDepartment.CreatedBy = IssueBy;// @class._ListViewMSTDepartment[i].ParentId;
                        viewMSTDepartment.ModifiedDate = DateTime.Now; //@class._ListViewMSTDepartment[i].ParentId;
                        viewMSTDepartment.ModifiedBy = "";//@class._ListViewMSTDepartment[i].ParentId;
                        viewMSTDepartment.IsActive = @class._ListViewMSTDepartment[i].IsActive;
                        viewMSTDepartment.AccountId = @class._ListViewMSTDepartment[i].AccountId;
                        viewMSTDepartment.LeaderId = @class._ListViewMSTDepartment[i].LeaderId;
                        viewMSTDepartment.CompanyCode = @class._ListViewMSTDepartment[i].CompanyCode;
                        _WolfApproveCore_thaistanley._ViewMSTDepartment.Add(viewMSTDepartment);
                    }


                    _WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("TRUNCATE TABLE MSTPosition");
                    using (var bulkCopy2 = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, (SqlTransaction)dbContextTransaction.GetDbTransaction()))
                    {
                        bulkCopy2.DestinationTableName = "MSTPosition";
                        bulkCopy2.BatchSize = 5000;
                        bulkCopy2.BulkCopyTimeout = 300;
                        DataTable dt = new DataTable();
                        // dt.Columns.Add("PositionId", typeof(int));
                        // dt.Columns.Add("PositionId", typeof(string));
                        dt.Columns.Add("NameTh", typeof(string));
                        dt.Columns.Add("NameEn", typeof(string));
                        dt.Columns.Add("PositionLevelId", typeof(int));
                        dt.Columns.Add("IsActive", typeof(bool));
                        dt.Columns.Add("CreatedDate", typeof(DateTime));
                        dt.Columns.Add("CreatedBy", typeof(string));
                        dt.Columns.Add("ModifiedDate", typeof(DateTime));
                        dt.Columns.Add("ModifiedBy", typeof(string));
                        dt.Columns.Add("AccountId", typeof(int));
                        dt.Columns.Add("CompanyCode", typeof(string));


                        foreach (var item in @class._ListViewMSTPosition)
                        {
                            dt.Rows.Add(
                                // item.EmployeeId,
                                item.NameTh,
                                item.NameEn,
                                item.PositionLevelId,
                                item.IsActive,
                                DateTime.Now,//item.CreatedDate,
                                IssueBy,//item.CreatedBy,
                                DateTime.Now,//item.ModifiedDate,
                                item.ModifiedBy,
                                item.AccountId,
                                item.CompanyCode

                            //""//item.Userid_Line
                            );
                        }

                        // Mapping columns
                        // bulkCopy2.ColumnMappings.Add("EmployeeId", "EmployeeId");
                        bulkCopy2.ColumnMappings.Add("NameTh", "NameTh");
                        bulkCopy2.ColumnMappings.Add("NameEn", "NameEn");
                        bulkCopy2.ColumnMappings.Add("PositionLevelId", "PositionLevelId");
                        bulkCopy2.ColumnMappings.Add("IsActive", "IsActive");
                        bulkCopy2.ColumnMappings.Add("CreatedDate", "CreatedDate");
                        bulkCopy2.ColumnMappings.Add("CreatedBy", "CreatedBy");
                        bulkCopy2.ColumnMappings.Add("ModifiedDate", "ModifiedDate");
                        bulkCopy2.ColumnMappings.Add("ModifiedBy", "ModifiedBy");
                        bulkCopy2.ColumnMappings.Add("AccountId", "AccountId");
                        bulkCopy2.ColumnMappings.Add("CompanyCode", "CompanyCode");
                        // Bulk insert
                        bulkCopy2.WriteToServer(dt);
                    }





                    _WolfApproveCore_thaistanley.SaveChanges();
                    dbContextTransaction.Commit();

                }
                catch (Exception ex)
                {
                    try { dbContextTransaction.Rollback(); } catch { }
                    config = "E";
                    msg = "Error Save: " + ex.InnerException.Message;
                    //v_msg = "Error Save: " + (ex.InnerException?.Message ?? ex.Message);
                }



            }
            string[] returnResult = { config, msg };
            return returnResult;
        }



        private void RunImport(string vWolf)
        {
            try
            {
                Class @class = new Class();
                // โค้ดที่อยู่หลังปุ่ม Import
                @class._ListViewMSTATACCEmployee = new List<ViewMSTATACCEmployee>();
                @class._ListViewMSTATACCEmployee = UpdateTBMSTATACCEmployee();

                @class._ListViewMSTATEmployee = new List<ViewMSTATEmployee>();
                @class._ListViewMSTATEmployee = UpdateTBMSTATEmployee();

                @class._ListViewMSTEmployee = new List<ViewMSTEmployee>();
                @class._ListViewMSTEmployee = UpdateTBMSTEmployee();
                //table login wolf account
                @class._ListViewWOLFAccount = new List<ViewWOLFAccount>();
                @class._ListViewWOLFAccount = UpdateWOLFAccount();

                vWolf = vWolf == "AddDataWolf" ? "Append" : "update";

                saveDataAccem(@class, vWolf);
            }
            catch (Exception ex) { string msgr = ex.Message; }

        }
        public List<ViewMSTATACCEmployee> UpdateTBMSTATACCEmployee()
        {
            // ===== 1. เตรียม Master Dictionary =====
            var posDict = _HRMS.AccPOSMAST.ToDictionary(x => x.POS_CODE, x => x.POS_NAME);
            var divDict = _HRMS.AccDIVIMAST.ToDictionary(x => x.DIVI_CODE, x => x.DIVI_NAME);
            var deptDict = _HRMS.AccDEPTMAST.ToDictionary(x => x.DEPT_CODE, x => x.DEPT_NAME);
            var secDict = _HRMS.AccSECMAST.ToDictionary(x => x.SEC_CODE, x => x.SEC_NAME);
            var grpDict = _HRMS.AccGROMAST.ToDictionary(x => x.GRP_CODE, x => x.GRP_NAME);
            var unitDict = _HRMS.AccUNITMAST.ToDictionary(x => x.UNT_CODE, x => x.UNT_NAME);

            // Composite Key → ValueTuple (เร็วกว่า string)
            var dirIndirDict = _HRMS.AccDirIndirMast.ToDictionary(
                x => (x.diDiviCode, x.diDeptCode, x.diSectCode, x.diDirIndir),
                x => x.diDirIndirCode
            );

            // ===== 2. ดึงพนักงาน (ไม่ ToList) =====
            var empQuery = _HRMS.AccEMPLOYEE.Where(x => x.QUIT_CODE == null && x.JOB_CODE != "F");

            var result = new List<ViewMSTATACCEmployee>(1024); // กัน List โตช้า
            int empId = 1;

            foreach (var emp in empQuery)
            {
                var posCode = emp.POS_CODE == null || emp.POS_CODE == "" ? "N" : emp.POS_CODE;
                var divCode = emp.DIVI_CODE == null || emp.DIVI_CODE == "" ? "N" : emp.DIVI_CODE;
                var deptCode = emp.DEPT_CODE == null || emp.DEPT_CODE == "" ? "N" : emp.DEPT_CODE;
                var secCode = emp.SEC_CODE == null || emp.SEC_CODE == "" ? "N" : emp.SEC_CODE;
                var grpCode = emp.GRP_CODE == null || emp.GRP_CODE == "" ? "N" : emp.GRP_CODE;
                var unitCode = emp.UNT_CODE == null || emp.UNT_CODE == "" ? "N" : emp.UNT_CODE;

                dirIndirDict.TryGetValue(
                    (divCode, deptCode, secCode, emp.DirOrIndir),
                    out var dirIndirCode
                );

                posDict.TryGetValue(posCode, out var posName);
                divDict.TryGetValue(divCode, out var divName);
                deptDict.TryGetValue(deptCode, out var deptName);
                secDict.TryGetValue(secCode, out var secName);
                grpDict.TryGetValue(grpCode, out var grpName);
                unitDict.TryGetValue(unitCode, out var unitName);

                result.Add(new ViewMSTATACCEmployee
                {
                    EMPID = empId++,
                    EMPCODE = emp.EMP_CODE,
                    Name = emp.PRI_ENG + emp.EMP_ENAME + "  " + emp.LAST_ENAME,
                    NameTH = emp.PRI_THAI + emp.EMP_TNAME + "  " + emp.LAST_TNAME,
                    JOB_NAME = emp.JOB_CODE,

                    PositionName = posName,
                    DivisionName = divName,
                    DepartmentName = deptName,
                    SECName = secName,
                    GRPName = grpName,
                    UNTName = unitName,

                    DIRECT_INDIRECT_CODE = dirIndirCode,
                    INTERCOMNO = emp.INTERCOMNO == null ? "-" : emp.INTERCOMNO.ToString(),
                    NICKNAME = emp.NICKNAME
                });
            }
            var finalIdDict = _WolfApproveCore_thaistanley._ViewMSTATACCEmployee.ToDictionary(x => x.EMPCODE);
            foreach (var e in result)
            {
                if (string.IsNullOrEmpty(e.EMPCODE) || !finalIdDict.TryGetValue(e.EMPCODE, out var bossId))
                {
                    e.EMPID = 99999;
                }
            }



            return result;
        }
        public List<ViewMSTATEmployee> UpdateTBMSTATEmployee()
        {
            try
            {
                // ===== 1. เตรียม Email Dictionary (Key = EmpCode) =====
                var emailDict = _IT.rpEmails
                    .Where(e => !string.IsNullOrWhiteSpace(e.emEmpcode))
                    .GroupBy(e => e.emEmpcode.Trim())
                    .ToDictionary(g => g.Key, g => g.First().emEmail_M365);

                // ===== 2. เตรียม Employee เก่า =====
                //var oldEmployeeMap = _WolfApproveCore_thaistanley._ViewMSTATEmployee
                //    .ToDictionary(x => x.EMPCODE, x => x);
                var oldEmployeeMap = _WolfApproveCore_thaistanley._ViewMSTATEmployee
                    .ToDictionary(x => x.EMPCODE, x => x);


                int currentMaxId = oldEmployeeMap.Count == 0
                    ? 0
                    : oldEmployeeMap.Values.Max(x => x.EMPID);

                // ===== 3. Loop HRMS โดยตรง (ไม่ ToList) =====
                foreach (var emp in _HRMS.AccEMPLOYEE.Where(x => x.QUIT_CODE == null))
                {
                    var empCode = emp.EMP_CODE?.Trim();
                    if (string.IsNullOrEmpty(empCode)) continue;

                    // ไม่มี Email → ข้ามทันที
                    if (!emailDict.ContainsKey(empCode)) continue;

                    var intercom = emp.INTERCOMNO?.ToString() ?? "-";
                    var nickName = emp.NICKNAME?.ToString();
                    var job = emp.JOB_CODE?.ToString();
                    var sec = emp.SEC_CODE?.ToString();
                    var grp = emp.GRP_CODE?.ToString();
                    var unt = emp.UNT_CODE?.ToString();

                    // ===== Update / Insert =====
                    if (oldEmployeeMap.TryGetValue(empCode, out var existingEmp))
                    {
                        existingEmp.NICKNAME = nickName;
                        existingEmp.INTERCOMNO = intercom;
                        existingEmp.JOBCODE = job;
                        existingEmp.SECNAME = sec;
                        existingEmp.GRPNAME = grp;
                        existingEmp.UNTNAME = unt;
                    }
                    else
                    {
                        currentMaxId++;
                        oldEmployeeMap[empCode] = new ViewMSTATEmployee
                        {
                            EMPID = 99999,// currentMaxId, stsus new
                            EMPCODE = empCode,
                            NICKNAME = nickName,
                            INTERCOMNO = intercom,
                            JOBCODE = job,
                            SECNAME = sec,
                            GRPNAME = grp,
                            UNTNAME = unt
                        };
                    }
                }

                return oldEmployeeMap.Values.ToList();
            }
            catch
            {
                throw;
            }
        }
        public List<ViewMSTEmployee> UpdateTBMSTEmployee()
        {
            string IssueBy = "WolfAdmin";
            Class @class = new Class();

            try
            {
                // 1. ดึงข้อมูลพื้นฐานมาทำ Dictionary ไว้ก่อน (Lookup Tables)
                var divDict = _WolfApproveCore_thaistanley._ViewMSTDivision.ToDictionary(x => x.DivisionCode, x => x.DivisionId);
                //var deptDict = _WolfApproveCore_thaistanley._ViewMSTDepartment.ToDictionary(x => x.DepartmentCode, x => x.DepartmentId);

                var deptDict = _WolfApproveCore_thaistanley._ViewMSTDepartment.GroupBy(x => x.DepartmentCode).ToDictionary(g => g.Key, g => g.First().DepartmentId);


                var posNameDict = _HRMS.AccPOSMAST.ToDictionary(x => x.POS_CODE, x => x.POS_NAME);

                // สำหรับ Position Mapping (ดึง NameTh มาเทียบกับ POS_NAME)
                var mstPosDict = _WolfApproveCore_thaistanley._ViewMSTPosition
                    .GroupBy(x => x.NameTh)
                    .ToDictionary(g => g.Key, g => g.First().PositionId);

                var emailDict = _IT.rpEmails
                    .Where(x => !string.IsNullOrEmpty(x.emEmpcode))
                    .ToDictionary(x => x.emEmpcode.Trim(), x => x.emEmail_M365);

                // 2. เตรียมข้อมูลพนักงานและตำแหน่ง
                var posListAll = _HRMS.AccPOSMAST.ToList();
                var dmHcm = int.Parse(posListAll.First(x => x.POS_CODE == "DM").POS_HCM_CODE);

                // กรองหัวหน้างานไว้รอ (Superior Candidates)
                var superiorPosList = posListAll
                    .Where(x => int.Parse(x.POS_HCM_CODE) >= dmHcm &&
                                !new[] { "DDM", "AV", "SAV", "PJ", "UL" }.Contains(x.POS_CODE))
                    .ToList();

                var empList = _HRMS.AccEMPLOYEE
                    .Where(x => x.QUIT_CODE == null)
                    .ToList() // ดึงมาจัดการใน Memory ต่อ
                    .Where(emp => emailDict.ContainsKey(emp.EMP_CODE?.Trim()))// && emp.EMP_CODE == "000391")
                    .OrderBy(x => int.Parse(x.EMP_CODE))
                    .ToList();


                var empListAll = _HRMS.AccEMPLOYEE
                    .Where(x => x.QUIT_CODE == null)
                    .ToList() // ดึงมาจัดการใน Memory ต่อ
                    .Where(emp => emailDict.ContainsKey(emp.EMP_CODE?.Trim()))
                    .OrderBy(x => int.Parse(x.EMP_CODE))
                    .ToList();

                // 3. ประมวลผล Logic หลัก (ใช้ Dictionary แทนการ Query ซ้อน Query)
                //var v_EMP_HEADCODE = "";
                var result1 = empList.Select(e =>
                {
                    posNameDict.TryGetValue(e.POS_CODE, out var posName);
                    mstPosDict.TryGetValue(posName ?? "", out var posId);
                    emailDict.TryGetValue(e.EMP_CODE.Trim(), out var email);


                    //field emp code Superior 
                    //var v_EMP_HEADCODE = empList.Where(x => x.EMP_CODE == e.EMP_CODE.Trim()).Select(x => x.EMP_HEADCODE).FirstOrDefault();

                    // หา Superior (ยังคงต้องวนลูปชุดเล็ก แต่จำกัดวงด้วย Division)
                    var currentPos = posListAll.FirstOrDefault(p => p.POS_CODE == e.POS_CODE);
                    int currentHcm = int.Parse(currentPos?.POS_HCM_CODE ?? "999");
                    //var superiorCode = new[] { "DDM", "AV", "SAV", "PJ" }.Contains(e.POS_CODE) ? "" :
                    //                    (from s_e in empListAll
                    //                     join s_p in superiorPosList on s_e.POS_CODE equals s_p.POS_CODE
                    //                     where s_e.DIVI_CODE == e.DIVI_CODE &&
                    //                           int.Parse(s_p.POS_HCM_CODE) < currentHcm
                    //                     let priority = (s_e.UNT_CODE == e.UNT_CODE && e.UNT_CODE != "N") ? 1 :
                    //                                    (s_e.GRP_CODE == e.GRP_CODE && e.GRP_CODE != "N") ? 2 :
                    //                                    (s_e.SEC_CODE == e.SEC_CODE && e.SEC_CODE != "N") ? 3 :
                    //                                    (s_e.DEPT_CODE == e.DEPT_CODE && e.DEPT_CODE != "N") ? 4 :
                    //                                    (s_e.DIVI_CODE == e.DIVI_CODE && e.DIVI_CODE != "N") ? 5 : 6
                    //                     //where //s_e.DIVI_CODE == e.DIVI_CODE &&
                    //                     //    int.Parse(s_p.POS_HCM_CODE) < currentHcm
                    //                     //let priority = (s_e.UNT_CODE == e.UNT_CODE && e.UNT_CODE != "N") ? 1 :
                    //                     //               (s_e.GRP_CODE == e.GRP_CODE && e.GRP_CODE != "N") ? 2 :
                    //                     //               (s_e.SEC_CODE == e.SEC_CODE && e.SEC_CODE != "N") ? 3 :
                    //                     //               (s_e.DEPT_CODE == e.DEPT_CODE && e.DEPT_CODE != "N") ? 4 :
                    //                     //               (s_e.DIVI_CODE == e.DIVI_CODE && e.DIVI_CODE != "N") ? 5 : 6
                    //                     //orderby priority, (e.DEPT_CODE == "CAE" ? int.Parse(s_p.POS_HCM_CODE) : -int.Parse(s_p.POS_HCM_CODE))
                    //                     orderby priority
                    //                     select s_e.EMP_CODE).FirstOrDefault();



                    //             var superiorCode = new[] { "DDM", "AV", "SAV", "PJ" }.Contains(e.POS_CODE)
                    //? null
                    //: (from s_e in empListAll
                    //   join s_p in superiorPosList on s_e.POS_CODE equals s_p.POS_CODE
                    //   where s_e.DIVI_CODE == e.DIVI_CODE
                    //         && int.Parse(s_p.POS_HCM_CODE) < currentHcm
                    //   let priority =
                    //        (s_e.UNT_CODE == e.UNT_CODE && e.UNT_CODE != "N") ? 1 :
                    //        (s_e.GRP_CODE == e.GRP_CODE && e.GRP_CODE != "N") ? 2 :
                    //        (s_e.SEC_CODE == e.SEC_CODE && e.SEC_CODE != "N") ? 3 :
                    //        (s_e.DEPT_CODE == e.DEPT_CODE && e.DEPT_CODE != "N") ? 4 :
                    //        (s_e.DIVI_CODE == e.DIVI_CODE && e.DIVI_CODE != "N") ? 5 : 99 // ❗ กันหลุด
                    //   where priority <= 5   // เอาเฉพาะ 1-5 เท่านั้น
                    //   orderby priority,     // 🔥 เริ่มจาก 1 แน่นอน
                    //              (e.DEPT_CODE == "CAE"
                    //                  ? int.Parse(s_p.POS_HCM_CODE)
                    //                  : -int.Parse(s_p.POS_HCM_CODE))
                    //   select s_e.EMP_CODE
                    //  ).FirstOrDefault();

                    return new ViewMSTEmployee
                    {
                        EmployeeCode = e.EMP_CODE,
                        Username = email,
                        NameTh = $"{e.PRI_THAI}{e.EMP_TNAME}  {e.LAST_TNAME}",
                        NameEn = $"{e.PRI_ENG}{e.EMP_ENAME}  {e.LAST_ENAME}",
                        Email = email,
                        PositionId = posId,
                        DepartmentId = deptDict.ContainsKey(e.DEPT_CODE) ? deptDict[e.DEPT_CODE] : 0,
                        DivisionId = divDict.ContainsKey(e.DIVI_CODE) ? divDict[e.DIVI_CODE] : 0,
                        ReportToEmpCode = e.EMP_HEADCODE != null ? e.EMP_HEADCODE : "",  // superiorCode,//  v_EMP_HEADCODE != null ? v_EMP_HEADCODE:  superiorCode,   e.EMP_HEADCODE,//
                        IsActive = true,
                        Lang = "EN",
                        AccountId = 1,
                        CreatedDate = DateTime.Now,
                        CreatedBy = IssueBy,
                        ModifiedDate = DateTime.Now,
                        ModifiedBy = IssueBy
                    };
                }).ToList();

                // 4. Update หรือ Add ลงใน List หลัก
                var existingEmpList = _WolfApproveCore_thaistanley._ViewMSTEmployee.OrderBy(x => x.EmployeeId).ToList();
                var existingEmpDict = existingEmpList.ToDictionary(x => x.EmployeeCode);
                int nextEmpId = existingEmpList.Any() ? existingEmpList.Max(x => x.EmployeeId) + 1 : 1;


                var ordered = result1.OrderBy(x => x.EmployeeCode).ToList();

                foreach (var newItem in ordered)
                {
                    if (existingEmpDict.TryGetValue(newItem.EmployeeCode, out var oldItem))
                    {
                        // Update
                        oldItem.Username = newItem.Username;
                        oldItem.NameTh = newItem.NameTh;
                        oldItem.NameEn = newItem.NameEn;
                        oldItem.Email = newItem.Email;
                        oldItem.PositionId = newItem.PositionId;
                        oldItem.DepartmentId = newItem.DepartmentId;
                        oldItem.DivisionId = newItem.DivisionId;
                        // oldItem.ReportToEmpCode = newItem.ReportToEmpCode;
                        oldItem.Lang = oldItem.Lang is null || oldItem.Lang == "" ? "EN" : oldItem.Lang;
                        oldItem.Userid_Line = "";
                    }
                    else
                    {
                        // Add
                        newItem.EmployeeId = nextEmpId++;
                        newItem.Userid_Line = "New";
                        existingEmpList.Add(newItem);
                    }
                }

                // 5. เปลี่ยน ReportTo จาก Code เป็น ID (ใช้ Dictionary เพื่อความเร็ว)
                var finalIdDict = existingEmpList.ToDictionary(x => x.EmployeeCode, x => x.EmployeeId);
                foreach (var e in existingEmpList)
                {
                    if (!string.IsNullOrEmpty(e.ReportToEmpCode) && finalIdDict.TryGetValue(e.ReportToEmpCode, out var bossId))
                    {
                        e.ReportToEmpCode = bossId.ToString();
                    }
                }

                @class._ListViewMSTEmployee = existingEmpList;
            }
            catch (Exception ex)
            {
                string mesr = ex.Message;
                // แนะนำให้ใช้ Logger แทนการเก็บลง string ครับ
            }

            return @class._ListViewMSTEmployee;
        }
        public List<ViewWOLFAccount> UpdateWOLFAccount()
        {
            string IssueBy = "AdminWolf";// HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            List<ViewWOLFAccount> _ListViewWOLFAccount = new List<ViewWOLFAccount>();
            var emailDict = _IT.rpEmails
                  .Where(x => !string.IsNullOrEmpty(x.emEmpcode))
                  .ToDictionary(x => x.emEmpcode.Trim(), x => x.emEmail_M365);
            var empList1 = _HRMS.AccEMPLOYEE
                 .Where(x => x.QUIT_CODE == null)
                 .ToList(); // ดึงมาจัดการใน Memory ต่อ

            var empWolfAcc = _WolfApproveCore_Center._ViewWOLFAccount
              .Where(x => x.ContactCode == "thaistanley")
              .ToList(); // ดึงมาจัดการใน Memory ต่อ


            var empList = _HRMS.AccEMPLOYEE
                   .Where(x => x.QUIT_CODE == null)
                   .ToList() // ดึงมาจัดการใน Memory ต่อ
                   .Where(emp => emailDict.ContainsKey(emp.EMP_CODE?.Trim()))
                   .OrderBy(x => int.Parse(x.EMP_CODE))
                   .ToList();


            var wolfUsernameSet = empWolfAcc.Select(x => x.Username?.Trim())
                                  .Where(x => !string.IsNullOrEmpty(x))
                                  .ToHashSet(StringComparer.OrdinalIgnoreCase);
            //var wolfUsernameSet_f = wolfUsernameSet.Contains(x=>x);
            var notInWolfAccount = empList
                                   .Where(emp => emailDict.ContainsKey(emp.EMP_CODE))
                                   .Select(emp => new
                                   {
                                       emp.EMP_CODE,
                                       Email = emailDict[emp.EMP_CODE]
                                   })
                                   .Where(x => !wolfUsernameSet.Contains(x.Email))
                                   .ToList();
            for (int i = 0; i < notInWolfAccount.Count(); i++)
            {
                _ListViewWOLFAccount.Add(new ViewWOLFAccount
                {
                    ID = i + 1,
                    ContactCode = "thaistanley",
                    Username = notInWolfAccount[i].Email,
                    Password = "Wolf@tse1",
                    IsVerify = true,
                    GuidVerify = "", //มาจากไหนไม่รู้
                    Note = "",
                    Remark = "",
                    Description = "",
                    CreatedDate = DateTime.Now,
                    CreatedBy = IssueBy,
                    ModifiedDate = DateTime.Now,
                    IsActive = true,
                });
            }


            return _ListViewWOLFAccount;
        }




        public string[] saveDataAccem(Class @class, string tsave)
        {
            string config = "S";
            string msg = "Save Data Success !!!";
            string IssueBy = "AdminWolf";// HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

            //WolfApproveCore
            using (var dbContextTransaction = _WolfApproveCore_thaistanley.Database.BeginTransaction())
            {
                try
                {
                    //SqlBulk insert

                    //รีเซ็ต Identity

                    //99999
                    if (tsave.Equals("Append"))
                    {
                        @class._ListViewMSTATACCEmployee = @class._ListViewMSTATACCEmployee.Where(x => x.EMPID == 99999).ToList();
                    }
                    else
                    {
                        _WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("TRUNCATE TABLE MSTATACCEmployee");
                    }

                    //_WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("TRUNCATE TABLE MSTATACCEmployee");
                    //delete data
                    //_WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("DELETE FROM MSTATACCEmployee" );

                    var connection = (SqlConnection)_WolfApproveCore_thaistanley.Database.GetDbConnection();
                    if (connection.State != ConnectionState.Open)
                        connection.Open();
                    using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, (SqlTransaction)dbContextTransaction.GetDbTransaction()))
                    {
                        bulkCopy.DestinationTableName = "MSTATACCEmployee";
                        bulkCopy.BatchSize = 5000;
                        bulkCopy.BulkCopyTimeout = 300;

                        DataTable dt = new DataTable();
                        //dt.Columns.Add("EMPID", typeof(int)); //auto number
                        dt.Columns.Add("EMPCODE", typeof(string));
                        dt.Columns.Add("Name", typeof(string));
                        dt.Columns.Add("NameTH", typeof(string));
                        dt.Columns.Add("JOB_NAME", typeof(string));
                        dt.Columns.Add("PositionName", typeof(string));
                        dt.Columns.Add("DivisionName", typeof(string));
                        dt.Columns.Add("DepartmentName", typeof(string));
                        dt.Columns.Add("SECName", typeof(string));
                        dt.Columns.Add("GRPName", typeof(string));
                        dt.Columns.Add("UNTName", typeof(string));
                        dt.Columns.Add("DIRECT_INDIRECT_CODE", typeof(string));//  typeof(DateTime));
                        dt.Columns.Add("INTERCOMNO", typeof(string));//  typeof(DateTime));
                        dt.Columns.Add("NICKNAME", typeof(string));//  typeof(DateTime));
                        foreach (var item in @class._ListViewMSTATACCEmployee)
                        {
                            dt.Rows.Add(
                                // item.EMPID, //auto number
                                item.EMPCODE,
                                item.Name,
                                item.NameTH,
                                item.JOB_NAME,
                                item.PositionName,
                                item.DivisionName,
                                item.DepartmentName,
                                item.SECName,
                                item.GRPName,
                                item.UNTName,
                                item.DIRECT_INDIRECT_CODE,
                                item.INTERCOMNO,
                                item.NICKNAME

                            );
                        }

                        // Mapping columns
                        //bulkCopy.ColumnMappings.Add("EMPID", "EMPID"); //auto number
                        bulkCopy.ColumnMappings.Add("EMPCODE", "EMPCODE");
                        bulkCopy.ColumnMappings.Add("Name", "Name");
                        bulkCopy.ColumnMappings.Add("NameTH", "NameTH");
                        bulkCopy.ColumnMappings.Add("JOB_NAME", "JOB_NAME");
                        bulkCopy.ColumnMappings.Add("PositionName", "PositionName");
                        bulkCopy.ColumnMappings.Add("DivisionName", "DivisionName");
                        bulkCopy.ColumnMappings.Add("DepartmentName", "DepartmentName");
                        bulkCopy.ColumnMappings.Add("SECName", "SECName");
                        bulkCopy.ColumnMappings.Add("GRPName", "GRPName");
                        bulkCopy.ColumnMappings.Add("UNTName", "UNTName");
                        bulkCopy.ColumnMappings.Add("DIRECT_INDIRECT_CODE", "DIRECT_INDIRECT_CODE");
                        bulkCopy.ColumnMappings.Add("INTERCOMNO", "INTERCOMNO");
                        bulkCopy.ColumnMappings.Add("NICKNAME", "NICKNAME");

                        // Bulk insert
                        bulkCopy.WriteToServer(dt);

                    }



                    if (tsave.Equals("Append"))
                    {
                        @class._ListViewMSTATEmployee = @class._ListViewMSTATEmployee.Where(x => x.EMPID == 99999).ToList();

                    }
                    else
                    {
                        _WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("TRUNCATE TABLE MSTATEmployee");
                        // _WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("TRUNCATE TABLE MSTATACCEmployee");
                    }
                    //  _WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("TRUNCATE TABLE MSTATEmployee");
                    using (var bulkCopy1 = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, (SqlTransaction)dbContextTransaction.GetDbTransaction()))
                    {
                        bulkCopy1.DestinationTableName = "MSTATEmployee";
                        bulkCopy1.BatchSize = 5000;
                        bulkCopy1.BulkCopyTimeout = 300;

                        DataTable dt = new DataTable();
                        // dt.Columns.Add("EMPID", typeof(int));
                        dt.Columns.Add("EMPCODE", typeof(string));
                        dt.Columns.Add("NICKNAME", typeof(string));
                        dt.Columns.Add("INTERCOMNO", typeof(string));
                        dt.Columns.Add("JOBCODE", typeof(string));
                        dt.Columns.Add("SECNAME", typeof(string));
                        dt.Columns.Add("GRPNAME", typeof(string));
                        dt.Columns.Add("UNTNAME", typeof(string));

                        foreach (var item in @class._ListViewMSTATEmployee)
                        {
                            dt.Rows.Add(
                                // item.EMPID,
                                item.EMPCODE,
                                item.NICKNAME,
                                item.INTERCOMNO,
                                item.JOBCODE,
                                item.SECNAME,
                                item.GRPNAME,
                                item.UNTNAME
                            );
                        }

                        // Mapping columns
                        //bulkCopy1.ColumnMappings.Add("EMPID", "EMPID");
                        bulkCopy1.ColumnMappings.Add("EMPCODE", "EMPCODE");
                        bulkCopy1.ColumnMappings.Add("NICKNAME", "NICKNAME");
                        bulkCopy1.ColumnMappings.Add("INTERCOMNO", "INTERCOMNO");
                        bulkCopy1.ColumnMappings.Add("JOBCODE", "JOBCODE");
                        bulkCopy1.ColumnMappings.Add("SECNAME", "SECNAME");
                        bulkCopy1.ColumnMappings.Add("GRPNAME", "GRPNAME");
                        bulkCopy1.ColumnMappings.Add("UNTNAME", "UNTNAME");
                        // Bulk insert
                        bulkCopy1.WriteToServer(dt);
                    }

                    if (tsave.Equals("Append"))
                    {
                        @class._ListViewMSTEmployee = @class._ListViewMSTEmployee.Where(x => x.Userid_Line == "New").ToList();

                    }
                    else
                    {
                        _WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("TRUNCATE TABLE MSTEmployee");

                    }
                    using (var bulkCopy2 = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, (SqlTransaction)dbContextTransaction.GetDbTransaction()))
                    {
                        bulkCopy2.DestinationTableName = "MSTEmployee";
                        bulkCopy2.BatchSize = 5000;
                        bulkCopy2.BulkCopyTimeout = 300;
                        DataTable dt = new DataTable();
                        // dt.Columns.Add("EmployeeId", typeof(int));
                        dt.Columns.Add("EmployeeCode", typeof(string));
                        dt.Columns.Add("Username", typeof(string));
                        dt.Columns.Add("NameTh", typeof(string));
                        dt.Columns.Add("NameEn", typeof(string));
                        dt.Columns.Add("Email", typeof(string));
                        dt.Columns.Add("IsActive", typeof(bool));
                        dt.Columns.Add("PositionId", typeof(int));
                        dt.Columns.Add("DepartmentId", typeof(int));
                        dt.Columns.Add("ReportToEmpCode", typeof(string));
                        dt.Columns.Add("SignPicPath", typeof(string));
                        dt.Columns.Add("Lang", typeof(string));
                        dt.Columns.Add("AccountId", typeof(int));
                        dt.Columns.Add("CreatedDate", typeof(DateTime));
                        dt.Columns.Add("CreatedBy", typeof(string));
                        dt.Columns.Add("ModifiedDate", typeof(DateTime));
                        dt.Columns.Add("ModifiedBy", typeof(string));
                        dt.Columns.Add("ADTitle", typeof(string));
                        dt.Columns.Add("DivisionId", typeof(int));
                        dt.Columns.Add("EmpLevel", typeof(string));
                        dt.Columns.Add("EMPL_RCD", typeof(string));
                        dt.Columns.Add("EmployeeLevel", typeof(int));
                        dt.Columns.Add("EffectiveDate", typeof(DateTime));
                        dt.Columns.Add("Userid_Line", typeof(string));

                        foreach (var item in @class._ListViewMSTEmployee)
                        {
                            dt.Rows.Add(
                                // item.EmployeeId,
                                item.EmployeeCode,
                                item.Username,
                                item.NameTh,
                                item.NameEn,
                                item.Email,
                                item.IsActive,
                                item.PositionId,
                                item.DepartmentId,
                                item.ReportToEmpCode,
                                item.SignPicPath,
                                item.Lang,
                                item.AccountId,
                                DateTime.Now,//item.CreatedDate,// DateTime.Now
                                IssueBy,//item.CreatedBy,
                                DateTime.Now,//item.ModifiedDate,
                                IssueBy,//item.ModifiedBy,
                                item.ADTitle,
                                item.DivisionId,
                                item.EmpLevel,
                                item.EMPL_RCD,
                                item.EmployeeLevel,
                                 DateTime.Now,// item.EffectiveDate,
                                ""//item.Userid_Line
                            );
                        }

                        // Mapping columns
                        // bulkCopy2.ColumnMappings.Add("EmployeeId", "EmployeeId");
                        bulkCopy2.ColumnMappings.Add("EmployeeCode", "EmployeeCode");
                        bulkCopy2.ColumnMappings.Add("Username", "Username");
                        bulkCopy2.ColumnMappings.Add("NameTh", "NameTh");
                        bulkCopy2.ColumnMappings.Add("NameEn", "NameEn");
                        bulkCopy2.ColumnMappings.Add("Email", "Email");
                        bulkCopy2.ColumnMappings.Add("IsActive", "IsActive");
                        bulkCopy2.ColumnMappings.Add("PositionId", "PositionId");
                        bulkCopy2.ColumnMappings.Add("DepartmentId", "DepartmentId");
                        bulkCopy2.ColumnMappings.Add("ReportToEmpCode", "ReportToEmpCode");
                        bulkCopy2.ColumnMappings.Add("SignPicPath", "SignPicPath");
                        bulkCopy2.ColumnMappings.Add("Lang", "Lang");
                        bulkCopy2.ColumnMappings.Add("AccountId", "AccountId");
                        bulkCopy2.ColumnMappings.Add("CreatedDate", "CreatedDate");
                        bulkCopy2.ColumnMappings.Add("CreatedBy", "CreatedBy");
                        bulkCopy2.ColumnMappings.Add("ModifiedDate", "ModifiedDate");
                        bulkCopy2.ColumnMappings.Add("ModifiedBy", "ModifiedBy");
                        bulkCopy2.ColumnMappings.Add("ADTitle", "ADTitle");
                        bulkCopy2.ColumnMappings.Add("DivisionId", "DivisionId");
                        bulkCopy2.ColumnMappings.Add("EmpLevel", "EmpLevel");
                        bulkCopy2.ColumnMappings.Add("EMPL_RCD", "EMPL_RCD");
                        bulkCopy2.ColumnMappings.Add("EmployeeLevel", "EmployeeLevel");
                        bulkCopy2.ColumnMappings.Add("EffectiveDate", "EffectiveDate");
                        bulkCopy2.ColumnMappings.Add("Userid_Line", "Userid_Line");
                        // Bulk insert
                        bulkCopy2.WriteToServer(dt);
                    }



                    _WolfApproveCore_thaistanley.SaveChanges();
                    dbContextTransaction.Commit();

                }
                catch (Exception ex)
                {
                    try { dbContextTransaction.Rollback(); } catch { }
                    config = "E";
                    msg = "Error Save: " + ex.InnerException.Message;
                    //v_msg = "Error Save: " + (ex.InnerException?.Message ?? ex.Message);
                }

            }

            //wold account
            using (var dbContextTransaction1 = _WolfApproveCore_Center.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < @class._ListViewWOLFAccount.Count(); i++)
                    {
                        var _VWOLFAccount = new ViewWOLFAccount();
                        _VWOLFAccount.ContactCode = @class._ListViewWOLFAccount[i].ContactCode;
                        _VWOLFAccount.Username = @class._ListViewWOLFAccount[i].Username;
                        _VWOLFAccount.Password = MD5Hash(@class._ListViewWOLFAccount[i].Password);
                        _VWOLFAccount.IsVerify = true;
                        _VWOLFAccount.GuidVerify = "047e0d17-ef94-42a9-9a8b-f525a2e81fdd"; //มาจากไหนไม่รู้
                        _VWOLFAccount.Note = "";
                        _VWOLFAccount.Remark = "";
                        _VWOLFAccount.Description = "";
                        _VWOLFAccount.CreatedDate = DateTime.Now;
                        _VWOLFAccount.CreatedBy = IssueBy;
                        _VWOLFAccount.ModifiedDate = DateTime.Now;
                        _VWOLFAccount.ModifiedBy = IssueBy;
                        _VWOLFAccount.IsActive = true;
                        _WolfApproveCore_Center._ViewWOLFAccount.AddAsync(_VWOLFAccount);
                    }




                    //_WolfApproveCore_Center._ViewWOLFAccount.Add(_ViewWOLFAccount);
                    _WolfApproveCore_Center.SaveChanges();
                    dbContextTransaction1.Commit();
                }
                catch (Exception ex)
                {
                    try { dbContextTransaction1.Rollback(); } catch { }
                    config = "E";
                    msg = "Error Save: " + ex.InnerException.Message;
                    //v_msg = "Error Save: " + (ex.InnerException?.Message ?? ex.Message);

                }
            }



            string[] returnResult = { config, msg };
            return returnResult;
        }








        private string MD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
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
                @class._Error.validation = "Invalid username or password. Please try again.";
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