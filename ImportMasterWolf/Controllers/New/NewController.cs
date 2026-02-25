using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ImportMasterWolf.Models.Common;
using ImportMasterWolf.Models.DBConnect;
using ImportMasterWolf.Models.Table.HRMS;
using ImportMasterWolf.Models.Table.IT;
using ImportMasterWolf.Models.Table.WolfApproveCore_thaistanley;
using System.Data.SqlClient;
using System.Data;


using Microsoft.EntityFrameworkCore.Storage;
using ImportMasterWolf.Models.Table.WolfApproveCore_Center;


using System.Security.Cryptography;
using System.Text;

namespace ImportMasterWolf.Controllers.New
{
    public class NewController : Controller
    {
        //username emppic ftpdb
        private LAMP _LAMP;
        private HRMS _HRMS;
        private IT _IT;
        private WolfApproveCore_thaistanley _WolfApproveCore_thaistanley;
        private WolfApproveCore_Center _WolfApproveCore_Center;

        private WolfApproveCore_thaistanley_PRD _WolfApproveCore_thaistanley_PRD;

        private CacheSettingController _Cache;
        private FunctionsController _callFunc;
        public NewController(LAMP lamp, HRMS hrms, IT it, WolfApproveCore_thaistanley WolfApproveCore_thaistanley,
            WolfApproveCore_thaistanley_PRD WolfApproveCore_thaistanley_PRD,
            WolfApproveCore_Center WolfApproveCore_Center, CacheSettingController cacheController, FunctionsController callfunction)
        {
            _LAMP = lamp;
            _HRMS = hrms;
            _IT = it;
            _WolfApproveCore_thaistanley = WolfApproveCore_thaistanley;
            _WolfApproveCore_Center = WolfApproveCore_Center;
            _WolfApproveCore_thaistanley_PRD = WolfApproveCore_thaistanley_PRD;

            _Cache = cacheController;
            _callFunc = callfunction;
        }

        [Authorize(Policy = "Checked")]
        public IActionResult Index(Class @class, string UpdateType)
        {

            @class._vType = UpdateType is null ? "" : UpdateType;
            if (@class._vType == "Accemployee")
            {
                @class._ListViewMSTATACCEmployee = new List<ViewMSTATACCEmployee>();
                @class._ListViewMSTATACCEmployee = UpdateTBMSTATACCEmployee();

                //@class._ListViewCloneMSTATACCEmployee = new List<ViewCloneMSTATACCEmployee>();
                //@class._ListViewCloneMSTATACCEmployee = UpdateCloneTBMSTATACCEmployee();

                @class._ListViewMSTATEmployee = new List<ViewMSTATEmployee>();
                @class._ListViewMSTATEmployee = UpdateTBMSTATEmployee();

                @class._ListViewMSTEmployee = new List<ViewMSTEmployee>();
                @class._ListViewMSTEmployee = UpdateTBMSTEmployee();

                //@class._ListViewCloneMSTEmployee = new List<ViewCloneMSTEmployee>();
                //@class._ListViewCloneMSTEmployee = UpdateAppendTBMSTEmployee();

                //table login wolf account
                @class._ListViewWOLFAccount = new List<ViewWOLFAccount>();
                @class._ListViewWOLFAccount = UpdateWOLFAccount();
            }
            else if (@class._vType == "Div")
            {
                //MSTDivision
                @class._ListViewMSTDivision = new List<ViewMSTDivision>();
                @class._ListViewMSTDivision = UpdateTBMSTDivision();
                ////MSTDepartment
                @class._ListViewMSTDepartment = new List<ViewMSTDepartment>();
                @class._ListViewMSTDepartment = UpdateTBMSTDepartment(@class._ListViewMSTDivision);
                ////MSTPosition
                @class._ListViewMSTPosition = new List<ViewMSTPosition>();
                @class._ListViewMSTPosition = UpdateTBMSTPosition();

            }

            return View("Index", @class);
        }

        public List<ViewMSTATACCEmployee> UpdateTBMSTATACCEmployee1()
        {
            Class @class = new Class();
            @class._ViewMSTATACCEmployee = new ViewMSTATACCEmployee();
            @class._ListViewMSTATACCEmployee = new List<ViewMSTATACCEmployee>();



            @class._ViewAccEMPLOYEE = new ViewAccEMPLOYEE();
            @class._ListViewAccEMPLOYEE = new List<ViewAccEMPLOYEE>();
            @class._ListViewAccEMPLOYEE = _HRMS.AccEMPLOYEE.Where(x => x.QUIT_CODE == null).OrderBy(x => int.Parse(x.EMP_CODE)).ToList();

            @class._ListViewAccPOSMAST = new List<ViewAccPOSMAST>();
            @class._ListViewAccPOSMAST = _HRMS.AccPOSMAST.ToList();

            @class._ListViewAccDIVIMAST = new List<ViewAccDIVIMAST>();
            @class._ListViewAccDIVIMAST = _HRMS.AccDIVIMAST.ToList();

            @class._ListViewAccDeptMast = new List<ViewAccDeptMast>();
            @class._ListViewAccDeptMast = _HRMS.AccDEPTMAST.ToList();

            @class._ListViewAccSECMAST = new List<ViewAccSECMAST>();
            @class._ListViewAccSECMAST = _HRMS.AccSECMAST.ToList();

            @class._ListViewAccGRPMAST = new List<ViewAccGRPMAST>();
            @class._ListViewAccGRPMAST = _HRMS.AccGROMAST.ToList();

            @class._ListViewAccUNITMAST = new List<ViewAccUNITMAST>();
            @class._ListViewAccUNITMAST = _HRMS.AccUNITMAST.ToList();

            @class._ListViewAccDirIndirMast = new List<ViewAccDirIndirMast>();
            @class._ListViewAccDirIndirMast = _HRMS.AccDirIndirMast.ToList();


            @class._ListViewMSTATACCEmployee = new List<ViewMSTATACCEmployee>();
            for (int i = 0; i < @class._ListViewAccEMPLOYEE.Count(); i++)
            {
                //string dir_code = @class._ListViewAccDirIndirMast.Where(x => x.diDiviCode == @class._ListViewAccEMPLOYEE[i].DIVI_CODE.ToString() && x.diDeptCode == @class._ListViewAccEMPLOYEE[i].DEPT_CODE.ToString() && x.diSectCode == @class._ListViewAccEMPLOYEE[i].SEC_CODE.ToString() && x.diDirIndir == @class._ListViewAccEMPLOYEE[i].DirOrIndir.ToString()).Select(x => x.diDirIndirCode).FirstOrDefault();
                @class._ListViewMSTATACCEmployee.Add(new ViewMSTATACCEmployee
                {
                    EMPID = i + 1,
                    EMPCODE = @class._ListViewAccEMPLOYEE[i].EMP_CODE.ToString(),
                    Name = @class._ListViewAccEMPLOYEE[i].PRI_ENG.ToString() + @class._ListViewAccEMPLOYEE[i].EMP_ENAME.ToString() + "  " + @class._ListViewAccEMPLOYEE[i].LAST_ENAME.ToString(),
                    NameTH = @class._ListViewAccEMPLOYEE[i].PRI_THAI.ToString() + @class._ListViewAccEMPLOYEE[i].EMP_TNAME.ToString() + "  " + @class._ListViewAccEMPLOYEE[i].LAST_TNAME.ToString(),
                    JOB_NAME = @class._ListViewAccEMPLOYEE[i].JOB_CODE.ToString(),
                    PositionName = @class._ListViewAccPOSMAST.Where(x => x.POS_CODE == @class._ListViewAccEMPLOYEE[i].POS_CODE.ToString()).Select(x => x.POS_NAME).FirstOrDefault(),
                    DivisionName = @class._ListViewAccDIVIMAST.Where(x => x.DIVI_CODE == @class._ListViewAccEMPLOYEE[i].DIVI_CODE.ToString()).Select(x => x.DIVI_NAME).FirstOrDefault(),
                    DepartmentName = @class._ListViewAccDeptMast.Where(x => x.DEPT_CODE == @class._ListViewAccEMPLOYEE[i].DEPT_CODE.ToString()).Select(x => x.DEPT_NAME).FirstOrDefault(),
                    SECName = @class._ListViewAccSECMAST.Where(x => x.SEC_CODE == @class._ListViewAccEMPLOYEE[i].SEC_CODE.ToString()).Select(x => x.SEC_NAME).FirstOrDefault(),
                    GRPName = @class._ListViewAccGRPMAST.Where(x => x.GRP_CODE == @class._ListViewAccEMPLOYEE[i].GRP_CODE.ToString()).Select(x => x.GRP_NAME).FirstOrDefault(),
                    UNTName = @class._ListViewAccUNITMAST.Where(x => x.UNT_CODE == @class._ListViewAccEMPLOYEE[i].UNT_CODE.ToString()).Select(x => x.UNT_NAME).FirstOrDefault(),
                    DIRECT_INDIRECT_CODE = @class._ListViewAccDirIndirMast.Where(x => x.diDiviCode == @class._ListViewAccEMPLOYEE[i].DIVI_CODE.ToString() && x.diDeptCode == @class._ListViewAccEMPLOYEE[i].DEPT_CODE.ToString() && x.diSectCode == @class._ListViewAccEMPLOYEE[i].SEC_CODE.ToString() && x.diDirIndir == @class._ListViewAccEMPLOYEE[i].DirOrIndir.ToString()).Select(x => x.diDirIndirCode).FirstOrDefault(),
                    INTERCOMNO = @class._ListViewAccEMPLOYEE[i].INTERCOMNO.ToString() is null || @class._ListViewAccEMPLOYEE[i].INTERCOMNO.ToString() == "" ? "-" : @class._ListViewAccEMPLOYEE[i].INTERCOMNO.ToString(),
                    NICKNAME = @class._ListViewAccEMPLOYEE[i].NICKNAME.ToString()

                });
            }



            //@class._ViewMSTATACCEmployee = _WolfApproveCore_thaistanley._ViewMSTATACCEmployee.Where(x => x.EMPCODE == "015142").FirstOrDefault();

            return @class._ListViewMSTATACCEmployee;

        }
        public List<ViewMSTATACCEmployee> UpdateTBMSTATACCEmployee2()
        {
            var resultList = new List<ViewMSTATACCEmployee>();

            // 1. ดึงข้อมูลพนักงานหลัก (คัดกรองตั้งแต่ระดับ Database จะเร็วกว่า)
            var empList = _HRMS.AccEMPLOYEE
                .Where(x => x.QUIT_CODE == null)
                .ToList();

            // 2. แปลง Master Data เป็น Dictionary เพื่อการค้นหาที่รวดเร็ว (Key คือ Code)
            var posDict = _HRMS.AccPOSMAST.ToDictionary(x => x.POS_CODE.ToString(), x => x.POS_NAME);
            var divDict = _HRMS.AccDIVIMAST.ToDictionary(x => x.DIVI_CODE.ToString(), x => x.DIVI_NAME);
            var deptDict = _HRMS.AccDEPTMAST.ToDictionary(x => x.DEPT_CODE.ToString(), x => x.DEPT_NAME);
            var secDict = _HRMS.AccSECMAST.ToDictionary(x => x.SEC_CODE.ToString(), x => x.SEC_NAME);
            var grpDict = _HRMS.AccGROMAST.ToDictionary(x => x.GRP_CODE.ToString(), x => x.GRP_NAME);
            var unitDict = _HRMS.AccUNITMAST.ToDictionary(x => x.UNT_CODE.ToString(), x => x.UNT_NAME);

            // กรณีตารางที่มีหลายเงื่อนไข (Composite Key) ให้รวม Key เป็น String
            var dirIndirLookup = _HRMS.AccDirIndirMast.ToDictionary(
                x => $"{x.diDiviCode}|{x.diDeptCode}|{x.diSectCode}|{x.diDirIndir}",
                x => x.diDirIndirCode
            );

            // 3. วนลูปสร้างข้อมูลพนักงานรอบเดียว
            for (int i = 0; i < empList.Count; i++)
            {
                var emp = empList[i];

                // เตรียม Key สำหรับค้นหา DirIndir
                string dirKey = $"{emp.DIVI_CODE}|{emp.DEPT_CODE}|{emp.SEC_CODE}|{emp.DirOrIndir}";

                resultList.Add(new ViewMSTATACCEmployee
                {
                    EMPID = i + 1,
                    EMPCODE = emp.EMP_CODE?.ToString(),
                    Name = $"{emp.PRI_ENG}{emp.EMP_ENAME}  {emp.LAST_ENAME}",
                    NameTH = $"{emp.PRI_THAI}{emp.EMP_TNAME}  {emp.LAST_TNAME}",
                    JOB_NAME = emp.JOB_CODE?.ToString(),

                    // ใช้ TryGetValue หรือ Indexer ในการดึงค่าจาก Dictionary (เร็วกว่า .Where มาก)
                    PositionName = posDict.ContainsKey(emp.POS_CODE?.ToString() ?? "") ? posDict[emp.POS_CODE.ToString()] : null,
                    DivisionName = divDict.ContainsKey(emp.DIVI_CODE?.ToString() ?? "") ? divDict[emp.DIVI_CODE.ToString()] : null,
                    DepartmentName = deptDict.ContainsKey(emp.DEPT_CODE?.ToString() ?? "") ? deptDict[emp.DEPT_CODE.ToString()] : null,
                    SECName = secDict.ContainsKey(emp.SEC_CODE?.ToString() ?? "") ? secDict[emp.SEC_CODE.ToString()] : null,
                    GRPName = grpDict.ContainsKey(emp.GRP_CODE?.ToString() ?? "") ? grpDict[emp.GRP_CODE.ToString()] : null,
                    UNTName = unitDict.ContainsKey(emp.UNT_CODE?.ToString() ?? "") ? unitDict[emp.UNT_CODE.ToString()] : null,

                    DIRECT_INDIRECT_CODE = dirIndirLookup.ContainsKey(dirKey) ? dirIndirLookup[dirKey] : null,

                    INTERCOMNO = string.IsNullOrWhiteSpace(emp.INTERCOMNO?.ToString()) ? "-" : emp.INTERCOMNO.ToString(),
                    NICKNAME = emp.NICKNAME?.ToString()
                });
            }

            return resultList;
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
                var posCode = emp.POS_CODE;
                var divCode = emp.DIVI_CODE;
                var deptCode = emp.DEPT_CODE;
                var secCode = emp.SEC_CODE;
                var grpCode = emp.GRP_CODE;
                var unitCode = emp.UNT_CODE;

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


        public List<ViewMSTATEmployee> UpdateTBMSTATEmployee1()
        {
            Class @class = new Class();
            List<ViewMSTATEmployee> _ListViewMSTATEmployeeCHK = new List<ViewMSTATEmployee>();
            List<ViewMSTATEmployee> _ListViewMSTATEmployeeChkOLD = new List<ViewMSTATEmployee>();
            @class._ListViewMSTATEmployee = new List<ViewMSTATEmployee>();
            @class._ListViewrpEmail = new List<ViewrpEmail>();
            try
            {
                @class._ListViewrpEmail = _IT.rpEmails.ToList();

                @class._ListViewAccEMPLOYEE = _HRMS.AccEMPLOYEE.Where(x => x.QUIT_CODE == null).OrderBy(x => int.Parse(x.EMP_CODE)).ToList();
                @class._ListViewAccEMPLOYEE = @class._ListViewAccEMPLOYEE.Where(emp => @class._ListViewrpEmail.Any(mail => mail.emEmpcode?.Trim() == emp.EMP_CODE?.Trim())).ToList();


                _ListViewMSTATEmployeeChkOLD = _WolfApproveCore_thaistanley._ViewMSTATEmployee.ToList();



                for (int i = 0; i < @class._ListViewAccEMPLOYEE.Count(); i++)
                {

                    if (@class._ListViewrpEmail.Where(x => x.emEmpcode == @class._ListViewAccEMPLOYEE[i].EMP_CODE).Select(x => x.emEmail_M365).FirstOrDefault() != null)
                    {
                        _ListViewMSTATEmployeeCHK.Add(new ViewMSTATEmployee
                        {
                            EMPID = i,
                            EMPCODE = @class._ListViewAccEMPLOYEE[i].EMP_CODE.ToString(),
                            NICKNAME = @class._ListViewAccEMPLOYEE[i].NICKNAME.ToString(),
                            INTERCOMNO = @class._ListViewAccEMPLOYEE[i].INTERCOMNO.ToString() is null || @class._ListViewAccEMPLOYEE[i].INTERCOMNO.ToString() == "" ? "-" : @class._ListViewAccEMPLOYEE[i].INTERCOMNO.ToString(),
                            JOBCODE = @class._ListViewAccEMPLOYEE[i].JOB_CODE.ToString(),
                            SECNAME = @class._ListViewAccEMPLOYEE[i].SEC_CODE.ToString(),
                            GRPNAME = @class._ListViewAccEMPLOYEE[i].GRP_CODE.ToString(),
                            UNTNAME = @class._ListViewAccEMPLOYEE[i].UNT_CODE.ToString(),
                        });
                    }
                    else
                    {
                        _ListViewMSTATEmployeeCHK.Add(new ViewMSTATEmployee
                        {
                            EMPID = _ListViewMSTATEmployeeChkOLD.Max(x => x.EMPID) + 1,
                            EMPCODE = @class._ListViewAccEMPLOYEE[i].EMP_CODE.ToString(),
                            NICKNAME = @class._ListViewAccEMPLOYEE[i].NICKNAME.ToString(),
                            INTERCOMNO = @class._ListViewAccEMPLOYEE[i].INTERCOMNO.ToString() is null || @class._ListViewAccEMPLOYEE[i].INTERCOMNO.ToString() == "" ? "-" : @class._ListViewAccEMPLOYEE[i].INTERCOMNO.ToString(),
                            JOBCODE = @class._ListViewAccEMPLOYEE[i].JOB_CODE.ToString(),
                            SECNAME = @class._ListViewAccEMPLOYEE[i].SEC_CODE.ToString(),
                            GRPNAME = @class._ListViewAccEMPLOYEE[i].GRP_CODE.ToString(),
                            UNTNAME = @class._ListViewAccEMPLOYEE[i].UNT_CODE.ToString(),
                        });
                    }
                }

                for (int i = 0; i < _ListViewMSTATEmployeeCHK.Count(); i++)
                {
                    if (_ListViewMSTATEmployeeChkOLD.Where(x => x.EMPCODE.ToString() == _ListViewMSTATEmployeeCHK[i].EMPCODE.ToString()).FirstOrDefault() != null)
                    {


                        var oldItem = _ListViewMSTATEmployeeChkOLD.Where(x => x.EMPCODE.ToString() == _ListViewMSTATEmployeeCHK[i].EMPCODE.ToString()).ToList();
                        _ListViewMSTATEmployeeChkOLD.Where(x => x.EMPCODE.ToString() == _ListViewMSTATEmployeeCHK[i].EMPCODE.ToString()).ToList()
                         .ForEach(x =>
                         {
                             x.NICKNAME = _ListViewMSTATEmployeeCHK[i].NICKNAME;
                             x.INTERCOMNO = _ListViewMSTATEmployeeCHK[i].INTERCOMNO;
                             x.JOBCODE = _ListViewMSTATEmployeeCHK[i].JOBCODE.ToString();
                             x.SECNAME = _ListViewMSTATEmployeeCHK[i].SECNAME.ToString();
                             x.GRPNAME = _ListViewMSTATEmployeeCHK[i].GRPNAME.ToString();
                             x.UNTNAME = _ListViewMSTATEmployeeCHK[i].UNTNAME.ToString();
                         });
                    }
                    else
                    {
                        _ListViewMSTATEmployeeChkOLD.Add(new ViewMSTATEmployee
                        {
                            EMPID = _ListViewMSTATEmployeeChkOLD.Select(x => x.EMPID).DefaultIfEmpty(0).Max() + 1,
                            EMPCODE = @class._ListViewAccEMPLOYEE[i].EMP_CODE.ToString(),
                            NICKNAME = @class._ListViewAccEMPLOYEE[i].NICKNAME.ToString(),
                            INTERCOMNO = @class._ListViewAccEMPLOYEE[i].INTERCOMNO.ToString() is null || @class._ListViewAccEMPLOYEE[i].INTERCOMNO.ToString() == "" ? "-" : @class._ListViewAccEMPLOYEE[i].INTERCOMNO.ToString(),
                            JOBCODE = @class._ListViewAccEMPLOYEE[i].JOB_CODE.ToString(),
                            SECNAME = @class._ListViewAccEMPLOYEE[i].SEC_CODE.ToString(),
                            GRPNAME = @class._ListViewAccEMPLOYEE[i].GRP_CODE.ToString(),
                            UNTNAME = @class._ListViewAccEMPLOYEE[i].UNT_CODE.ToString(),
                        });
                    }
                }



            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }


            return _ListViewMSTATEmployeeChkOLD;

        }
        public List<ViewMSTATEmployee> UpdateTBMSTATEmployee2()
        {
            try
            {
                // 1. ดึงข้อมูลมาเก็บไว้ในหน่วยความจำทีเดียว
                var emails = _IT.rpEmails.ToList();
                var hrmsEmployees = _HRMS.AccEMPLOYEE
                    .Where(x => x.QUIT_CODE == null)
                    .ToList();

                // ดึงข้อมูลเก่า และแปลงเป็น Dictionary เพื่อให้ค้นหาได้ไว (Key คือ EMPCODE)
                var oldEmployeeMap = _WolfApproveCore_thaistanley._ViewMSTATEmployee
                    .ToDictionary(x => x.EMPCODE.ToString(), x => x);

                // ทำ Lookup สำหรับ Email เพื่อลดเวลาค้นหา
                var emailLookup = emails
                    .Where(e => !string.IsNullOrEmpty(e.emEmpcode))
                    .ToLookup(e => e.emEmpcode.Trim(), e => e.emEmail_M365);

                // เตรียมตัวแปรสำหรับ New ID (หาค่า Max ครั้งเดียวข้างนอก)
                int currentMaxId = oldEmployeeMap.Values.Any() ? oldEmployeeMap.Values.Max(x => x.EMPID) : 0;

                foreach (var emp in hrmsEmployees)
                {
                    string empCode = emp.EMP_CODE?.Trim();
                    if (string.IsNullOrEmpty(empCode)) continue;

                    // ตรวจสอบว่ามีอีเมลหรือไม่
                    bool hasEmail = emailLookup.Contains(empCode);
                    if (!hasEmail) continue; // ถ้าไม่มีเมลตามเงื่อนไขเดิม อาจจะข้ามไป

                    string intercom = string.IsNullOrWhiteSpace(emp.INTERCOMNO?.ToString()) ? "-" : emp.INTERCOMNO.ToString();

                    // 2. ตรวจสอบว่ามีพนักงานคนนี้ในข้อมูลเก่าหรือยัง
                    if (oldEmployeeMap.TryGetValue(empCode, out var existingEmp))
                    {
                        // ถ้ามีแล้ว -> Update ข้อมูล (ไม่ต้องใช้ List.ForEach ให้ซับซ้อน)
                        existingEmp.NICKNAME = emp.NICKNAME?.ToString();
                        existingEmp.INTERCOMNO = intercom;
                        existingEmp.JOBCODE = emp.JOB_CODE?.ToString();
                        existingEmp.SECNAME = emp.SEC_CODE?.ToString();
                        existingEmp.GRPNAME = emp.GRP_CODE?.ToString();
                        existingEmp.UNTNAME = emp.UNT_CODE?.ToString();
                    }
                    else
                    {
                        // ถ้ายังไม่มี -> เพิ่มเข้าไปใหม่
                        currentMaxId++;
                        var newEmp = new ViewMSTATEmployee
                        {
                            EMPID = currentMaxId,
                            EMPCODE = empCode,
                            NICKNAME = emp.NICKNAME?.ToString(),
                            INTERCOMNO = intercom,
                            JOBCODE = emp.JOB_CODE?.ToString(),
                            SECNAME = emp.SEC_CODE?.ToString(),
                            GRPNAME = emp.GRP_CODE?.ToString(),
                            UNTNAME = emp.UNT_CODE?.ToString()
                        };
                        oldEmployeeMap.Add(empCode, newEmp);
                    }
                }

                return oldEmployeeMap.Values.ToList();
            }
            catch (Exception ex)
            {
                // แนะนำให้ใช้ Logging แทนการเก็บลง string เปล่าๆ
                throw;
            }

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

        public List<ViewMSTEmployee> UpdateTBMSTEmployee1()
        {
            string IssueBy = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            Class @class = new Class();

            try
            {
                @class._ListViewMSTDepartment = new List<ViewMSTDepartment>();
                @class._ListViewMSTDepartment = _WolfApproveCore_thaistanley._ViewMSTDepartment.ToList();


                @class._ListViewMSTDivision = new List<ViewMSTDivision>();
                @class._ListViewMSTDivision = _WolfApproveCore_thaistanley._ViewMSTDivision.ToList();

                @class._ListViewMSTPosition = new List<ViewMSTPosition>();
                @class._ListViewMSTPosition = _WolfApproveCore_thaistanley._ViewMSTPosition.ToList();

                @class._ListViewAccPOSMAST = new List<ViewAccPOSMAST>();
                @class._ListViewAccPOSMAST = _HRMS.AccPOSMAST.ToList();

                // MSTPosition
                //MSTDepartment
                //MSTDivision
                List<ViewrpEmail> emaillist = _IT.rpEmails.ToList();
                List<ViewAccEMPLOYEE> empList = _HRMS.AccEMPLOYEE.Where(x => x.QUIT_CODE == null).OrderBy(x => int.Parse(x.EMP_CODE)).ToList();
                empList = empList.Where(emp => emaillist.Any(mail => mail.emEmpcode?.Trim() == emp.EMP_CODE?.Trim())).ToList();


                ////N UL TL GL TM CS  DM   ไม่เอา AV SAV CM PJ DDM

                //var dmHcm = int.Parse(_HRMS.AccPOSMAST.First(x => x.POS_CODE == "DM").POS_HCM_CODE);
                List<ViewAccPOSMAST> posList = _HRMS.AccPOSMAST.ToList();
                var dmHcm = int.Parse(_HRMS.AccPOSMAST.First(x => x.POS_CODE == "DM").POS_HCM_CODE);
                var posList1 = _HRMS.AccPOSMAST
                    .Where(x => int.Parse(x.POS_HCM_CODE) >= dmHcm &&
                                !new[] { "DDM", "AV", "SAV", "CM", "PJ" }.Contains(x.POS_CODE))
                    .ToList();

                @class._ListViewMSTEmployee = new List<ViewMSTEmployee>();
                @class._ListViewMSTEmployee = _WolfApproveCore_thaistanley._ViewMSTEmployee.ToList();


                //check report to
                var result1 = (from e in empList
                               join p in posList on e.POS_CODE equals p.POS_CODE
                               where e.QUIT_CODE == null && e.DIVI_CODE == "ST"// && e.DIVI_CODE == "IS"
                               select new EmployeeSuperiorDetail
                               {
                                   EMP_CODE = e.EMP_CODE,
                                   EmployeeCode = _IT.rpEmails.Where(y => y.emEmpcode == e.EMP_CODE).Select(z => z.emEmail_M365).FirstOrDefault(),
                                   NameTh = e.PRI_THAI + e.EMP_TNAME + "  " + e.LAST_TNAME,
                                   NameEn = e.PRI_ENG + e.EMP_ENAME + "  " + e.LAST_ENAME,
                                   DIVI_CODE = @class._ListViewMSTDivision.Where(x => x.DivisionCode == e.DIVI_CODE).Select(x => x.DivisionId).FirstOrDefault().ToString(),
                                   DEPT_CODE = @class._ListViewMSTDepartment.Where(x => x.DepartmentCode == e.DEPT_CODE).Select(x => x.DepartmentId).FirstOrDefault().ToString(),
                                   SEC_CODE = e.SEC_CODE,
                                   GRP_CODE = e.GRP_CODE,
                                   POSITION = @class._ListViewMSTPosition.Where(x => x.NameTh == @class._ListViewAccPOSMAST.Where(y => y.POS_CODE == e.POS_CODE).Select(z => z.POS_NAME).FirstOrDefault()).Select(x => x.PositionId).FirstOrDefault().ToString(),
                                   CURRENT_POS = p.POS_NAME,
                                   CURRENT_HCM = p.POS_HCM_CODE,
                                   // จำลอง OUTER APPLY ด้วย Sub-query
                                   SUPERIOR_EMP_CODE =
                                                    (from s_e in empList
                                                     join s_p in posList1 on s_e.POS_CODE equals s_p.POS_CODE
                                                     where s_e.QUIT_CODE == null
                                                        && int.Parse(s_p.POS_HCM_CODE) < int.Parse(p.POS_HCM_CODE) // ตำแหน่งสูงกว่า
                                                        && s_e.DIVI_CODE == e.DIVI_CODE // Division เดียวกัน

                                                     let priority =
                                                                    (s_e.UNT_CODE == e.UNT_CODE && e.UNT_CODE != "N") ? 1 :
                                                                    (s_e.GRP_CODE == e.GRP_CODE && e.GRP_CODE != "N") ? 2 :
                                                                    (s_e.SEC_CODE == e.SEC_CODE && e.SEC_CODE != "N") ? 3 :
                                                                    (s_e.DEPT_CODE == e.DEPT_CODE && e.DEPT_CODE != "N") ? 4 : 4
                                                     orderby priority ascending, int.Parse(s_p.POS_HCM_CODE) ascending
                                                     select s_e.EMP_CODE).FirstOrDefault()
                               })
                  .OrderBy(x => int.Parse(x.CURRENT_HCM))
                  .Select(x => new ViewMSTEmployee
                  {
                      EmployeeId = int.Parse(x.EMP_CODE),// _ListviewMSTATEmployees.Where(u => u.EMPCODE == x.EMP_CODE).Select(c => c.EMPID).FirstOrDefault(),
                      EmployeeCode = x.EMP_CODE,
                      Username = x.EmployeeCode,
                      NameTh = x.NameTh,
                      NameEn = x.NameEn,
                      Email = x.EmployeeCode,
                      IsActive = true,//true,
                      PositionId = int.Parse(x.POSITION),//x.e.DEPT_CODE,
                      DepartmentId = int.Parse(x.DEPT_CODE),//x.e.DIVI_CODE,
                      ReportToEmpCode = x.SUPERIOR_EMP_CODE,//is null ? "" : _ListviewMSTATEmployees.Where(u => u.EMPCODE == x.SUPERIOR_EMP_CODE).Select(c => c.EMPID).FirstOrDefault().ToString(),
                      SignPicPath = "",//x.h1.EMP_CODE,
                      Lang = "EN",//x.h1.POS_CODE,
                      AccountId = 1,
                      CreatedDate = DateTime.Now,
                      CreatedBy = IssueBy,
                      ModifiedDate = DateTime.Now,
                      ModifiedBy = IssueBy,
                      ADTitle = "",
                      DivisionId = int.Parse(x.DIVI_CODE),
                      EmpLevel = "",
                      EMPL_RCD = "",
                      EmployeeLevel = 1,
                      EffectiveDate = DateTime.Now,
                      Userid_Line = "",

                  }).OrderBy(e => e.EmployeeId).ToList();

                // var vList1 = result1.Where(x => x.EmployeeCode == "015375").FirstOrDefault();
                //for loop add
                var ListMSTEmployee = @class._ListViewMSTEmployee.ToDictionary(x => x.EmployeeCode);
                foreach (var vAccem in result1)
                {
                    if (ListMSTEmployee.TryGetValue(vAccem.EmployeeCode, out var aItem))
                    {
                        aItem.Username = vAccem.Username;
                        aItem.NameTh = vAccem.NameTh;
                        aItem.NameEn = vAccem.NameEn;
                        aItem.Email = vAccem.EmployeeCode;
                        aItem.PositionId = vAccem.PositionId;//x.e.DEPT_CODE,
                        aItem.DepartmentId = vAccem.DepartmentId;//x.e.DIVI_CODE,
                        aItem.ReportToEmpCode = vAccem.ReportToEmpCode;// vAccem.ReportToEmpCode; //is null ? "" : _ListviewMSTATEmployees.Where(u => u.EMPCODE == x.SUPERIOR_EMP_CODE).Select(c => c.EMPID).FirstOrDefault().ToString(),
                    }
                    else
                    {
                        @class._ListViewMSTEmployee.Add(new ViewMSTEmployee
                        {
                            //vAccem .Max(x => x.EMPID) + 1
                            EmployeeId = @class._ListViewMSTEmployee.Max(x => x.EmployeeId) + 1,// _ListviewMSTATEmployees.Where(u => u.EMPCODE == x.EMP_CODE).Select(c => c.EMPID).FirstOrDefault(),
                            EmployeeCode = vAccem.EmployeeCode,
                            Username = vAccem.Username,
                            NameTh = vAccem.NameTh,
                            NameEn = vAccem.NameEn,
                            Email = vAccem.Email,
                            IsActive = true,//true,
                            PositionId = vAccem.PositionId,//x.e.DEPT_CODE,
                            DepartmentId = vAccem.DepartmentId,//x.e.DIVI_CODE,
                            ReportToEmpCode = vAccem.ReportToEmpCode,//is null ? "" : _ListviewMSTATEmployees.Where(u => u.EMPCODE == x.SUPERIOR_EMP_CODE).Select(c => c.EMPID).FirstOrDefault().ToString(),
                            SignPicPath = "",//x.h1.EMP_CODE,
                            Lang = "EN",//x.h1.POS_CODE,
                            AccountId = 1,
                            CreatedDate = DateTime.Now,
                            CreatedBy = IssueBy,
                            ModifiedDate = DateTime.Now,
                            ModifiedBy = IssueBy,
                            ADTitle = "",
                            DivisionId = vAccem.DivisionId,
                            EmpLevel = "",
                            EMPL_RCD = "",
                            EmployeeLevel = 1,
                            EffectiveDate = DateTime.Now,
                            Userid_Line = "",
                        });          // add
                    }
                }
                //update report to
                // var vList = @class._ListViewMSTEmployee.Where(x => x.EmployeeCode == "015375").FirstOrDefault();


                var dict = @class._ListViewMSTEmployee.ToDictionary(x => x.EmployeeCode, x => x.EmployeeId);

                foreach (var e in @class._ListViewMSTEmployee)
                    if (!string.IsNullOrEmpty(e.ReportToEmpCode) &&
                        dict.TryGetValue(e.ReportToEmpCode, out var id))
                        e.ReportToEmpCode = id.ToString();



            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }


            return @class._ListViewMSTEmployee;

        }
        public List<ViewMSTEmployee> UpdateTBMSTEmployee2()
        {
            string IssueBy = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            Class @class = new Class();

            try
            {
                // 1. ดึง Master Data และจัดการปัญหา Key ซ้ำด้วย GroupBy().ToDictionary()
                var divMap = _WolfApproveCore_thaistanley._ViewMSTDivision
                    .GroupBy(x => x.DivisionCode)
                    .ToDictionary(g => g.Key, g => g.First().DivisionId);

                var deptMap = _WolfApproveCore_thaistanley._ViewMSTDepartment
                    .GroupBy(x => x.DepartmentCode)
                    .ToDictionary(g => g.Key, g => g.First().DepartmentId);

                var posNameIdMap = _WolfApproveCore_thaistanley._ViewMSTPosition
                    .GroupBy(x => x.NameTh)
                    .ToDictionary(g => g.Key, g => g.First().PositionId);

                // 2. ดึงข้อมูลจาก HRMS และ IT
                var accPosDict = _HRMS.AccPOSMAST
                    .GroupBy(x => x.POS_CODE)
                    .ToDictionary(g => g.Key, g => g.First());

                var emailLookup = _IT.rpEmails
                    .Where(x => x.emEmpcode != null)
                    .ToLookup(x => x.emEmpcode.Trim(), x => x.emEmail_M365);

                var allEmps = _HRMS.AccEMPLOYEE.Where(x => x.QUIT_CODE == null).ToList();

                // 3. เตรียมข้อมูลสำหรับหา Superior (หัวหน้า)
                var dmHcmStr = accPosDict.ContainsKey("DM") ? accPosDict["DM"].POS_HCM_CODE : "0";
                int dmHcm = int.TryParse(dmHcmStr, out int val) ? val : 0;

                // ดึงเฉพาะคนที่มีสิทธิ์เป็นหัวหน้า (Potential Superiors)
                var potentialSuperiors = allEmps
                    .Where(e => accPosDict.ContainsKey(e.POS_CODE) &&
                                int.TryParse(accPosDict[e.POS_CODE].POS_HCM_CODE, out int h) && h >= dmHcm)
                    .ToList();

                var superiorLookup = potentialSuperiors.ToLookup(s => s.DIVI_CODE);

                // 4. ประมวลผลหา Superior และสร้างข้อมูลพนักงาน (ขั้นตอนนี้จะเร็วขึ้นมาก)
                var processedData = allEmps
                    .Where(e => emailLookup.Contains(e.EMP_CODE?.Trim())) // กรองเฉพาะคนที่มีเมลตามเงื่อนไขเดิม
                    .Select(e =>
                    {
                        var posInfo = accPosDict.ContainsKey(e.POS_CODE) ? accPosDict[e.POS_CODE] : null;
                        int currentHcm = (posInfo != null && int.TryParse(posInfo.POS_HCM_CODE, out int h)) ? h : 999;

                        // หาหัวหน้าโดยใช้ Lookup ตาม Division เพื่อลดรอบการ Search
                        var superior = superiorLookup[e.DIVI_CODE]
                                    .Select(s =>
                                    {
                                        int sHcm = int.TryParse(accPosDict[s.POS_CODE].POS_HCM_CODE, out int sh) ? sh : 0;
                                        return new
                                        {
                                            s.EMP_CODE,
                                            s.GRP_CODE,
                                            s.SEC_CODE,
                                            s.DEPT_CODE,
                                            Hcm = sHcm
                                        };
                                    })
                                    .Where(s => s.Hcm < currentHcm) // ตำแหน่งต้องสูงกว่า (HCM น้อยกว่า)
                                    .Select(s => new
                                    {
                                        s.EMP_CODE,
                                        Priority = (s.GRP_CODE == e.GRP_CODE && e.GRP_CODE != "N") ? 1 :
                                                   (s.SEC_CODE == e.SEC_CODE && e.SEC_CODE != "N") ? 2 :
                                                   (s.DEPT_CODE == e.DEPT_CODE && e.DEPT_CODE != "N") ? 3 : 4,
                                        s.Hcm
                                    })
                                    .OrderBy(s => s.Priority).ThenByDescending(s => s.Hcm)
                                    .FirstOrDefault();

                        return new
                        {
                            Emp = e,
                            Email = emailLookup[e.EMP_CODE.Trim()].FirstOrDefault(),
                            SuperiorCode = superior?.EMP_CODE,
                            PosId = (posInfo != null && posNameIdMap.ContainsKey(posInfo.POS_NAME)) ? posNameIdMap[posInfo.POS_NAME] : 0,
                            DivId = divMap.ContainsKey(e.DIVI_CODE) ? divMap[e.DIVI_CODE] : 0,
                            DepId = deptMap.ContainsKey(e.DEPT_CODE) ? deptMap[e.DEPT_CODE] : 0
                        };
                    }).ToList();

                // 5. จัดการ Update หรือ Insert เข้า List หลัก
                var existingEmpDict = _WolfApproveCore_thaistanley._ViewMSTEmployee
                    .GroupBy(x => x.EmployeeCode)
                    .ToDictionary(g => g.Key, g => g.First());

                int nextId = existingEmpDict.Values.Any() ? existingEmpDict.Values.Max(x => x.EmployeeId) + 1 : 1;

                foreach (var item in processedData)
                {
                    if (existingEmpDict.TryGetValue(item.Emp.EMP_CODE, out var oldEmp))
                    {
                        // Update ข้อมูลที่มีอยู่แล้ว
                        oldEmp.Username = item.Email;
                        oldEmp.NameTh = $"{item.Emp.PRI_THAI}{item.Emp.EMP_TNAME}  {item.Emp.LAST_TNAME}";
                        oldEmp.NameEn = $"{item.Emp.PRI_ENG}{item.Emp.EMP_ENAME}  {item.Emp.LAST_ENAME}";
                        oldEmp.Email = item.Email;
                        oldEmp.PositionId = item.PosId;
                        oldEmp.DepartmentId = item.DepId;
                        oldEmp.DivisionId = item.DivId;
                        oldEmp.ReportToEmpCode = item.SuperiorCode; // เก็บเป็น Code ไว้ก่อน
                        oldEmp.ModifiedDate = DateTime.Now;
                        oldEmp.ModifiedBy = IssueBy;
                    }
                    else
                    {
                        // เพิ่มพนักงานใหม่
                        var newEmp = new ViewMSTEmployee
                        {
                            EmployeeId = nextId++,
                            EmployeeCode = item.Emp.EMP_CODE,
                            Username = item.Email,
                            NameTh = $"{item.Emp.PRI_THAI}{item.Emp.EMP_TNAME}  {item.Emp.LAST_TNAME}",
                            NameEn = $"{item.Emp.PRI_ENG}{item.Emp.EMP_ENAME}  {item.Emp.LAST_ENAME}",
                            Email = item.Email,
                            IsActive = true,
                            PositionId = item.PosId,
                            DepartmentId = item.DepId,
                            DivisionId = item.DivId,
                            ReportToEmpCode = item.SuperiorCode,
                            AccountId = 1,
                            Lang = "EN",
                            CreatedDate = DateTime.Now,
                            CreatedBy = IssueBy,
                            ModifiedDate = DateTime.Now,
                            ModifiedBy = IssueBy,
                            EmployeeLevel = 1,
                            EffectiveDate = DateTime.Now
                        };
                        existingEmpDict.Add(newEmp.EmployeeCode, newEmp);
                    }
                }

                // 6. ขั้นตอนสุดท้าย: เปลี่ยน ReportTo จาก EMP_CODE เป็น EmployeeId (ใช้วิธี Mapping)
                var codeToIdMap = existingEmpDict.ToDictionary(x => x.Key, x => x.Value.EmployeeId);
                foreach (var emp in existingEmpDict.Values)
                {
                    if (!string.IsNullOrEmpty(emp.ReportToEmpCode) && codeToIdMap.TryGetValue(emp.ReportToEmpCode, out var bossId))
                    {
                        emp.ReportToEmpCode = bossId.ToString();
                    }
                }

                @class._ListViewMSTEmployee = existingEmpDict.Values.OrderBy(x => x.EmployeeId).ToList();
            }
            catch (Exception ex)
            {
                // Error Logging
                string msg = ex.Message;
            }

            return @class._ListViewMSTEmployee;
        }
        public List<ViewMSTEmployee> UpdateTBMSTEmployee()
        {
            string IssueBy = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
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
                    .Where(x => x.QUIT_CODE == null && x.EMP_CODE == "001794")
                    .ToList() // ดึงมาจัดการใน Memory ต่อ
                    .Where(emp => emailDict.ContainsKey(emp.EMP_CODE?.Trim()))
                    .OrderBy(x => int.Parse(x.EMP_CODE))
                    .ToList();


                var empListAll = _HRMS.AccEMPLOYEE
                    .Where(x => x.QUIT_CODE == null)
                    .ToList() // ดึงมาจัดการใน Memory ต่อ
                    .Where(emp => emailDict.ContainsKey(emp.EMP_CODE?.Trim()))
                    .OrderBy(x => int.Parse(x.EMP_CODE))
                    .ToList();

                // 3. ประมวลผล Logic หลัก (ใช้ Dictionary แทนการ Query ซ้อน Query)
                var result1 = empList.Select(e =>
                {
                    posNameDict.TryGetValue(e.POS_CODE, out var posName);
                    mstPosDict.TryGetValue(posName ?? "", out var posId);
                    emailDict.TryGetValue(e.EMP_CODE.Trim(), out var email);

                    // หา Superior (ยังคงต้องวนลูปชุดเล็ก แต่จำกัดวงด้วย Division)
                    var currentPos = posListAll.FirstOrDefault(p => p.POS_CODE == e.POS_CODE);
                    int currentHcm = int.Parse(currentPos?.POS_HCM_CODE ?? "999");

                    var superiorCode = new[] { "DDM", "AV", "SAV", "PJ", "AV" }.Contains(e.POS_CODE) ? "" :
                                        (from s_e in empListAll
                                         join s_p in superiorPosList on s_e.POS_CODE equals s_p.POS_CODE
                                         where s_e.DIVI_CODE == e.DIVI_CODE &&
                                               int.Parse(s_p.POS_HCM_CODE) < currentHcm
                                         let priority = (s_e.UNT_CODE == e.UNT_CODE && e.UNT_CODE != "N") ? 1 :
                                                        (s_e.GRP_CODE == e.GRP_CODE && e.GRP_CODE != "N") ? 2 :
                                                        (s_e.SEC_CODE == e.SEC_CODE && e.SEC_CODE != "N") ? 3 :
                                                        (s_e.DEPT_CODE == e.DEPT_CODE && e.DEPT_CODE != "N") ? 4 : 5
                                         //where //s_e.DIVI_CODE == e.DIVI_CODE &&
                                         //    int.Parse(s_p.POS_HCM_CODE) < currentHcm
                                         //let priority = (s_e.UNT_CODE == e.UNT_CODE && e.UNT_CODE != "N") ? 1 :
                                         //               (s_e.GRP_CODE == e.GRP_CODE && e.GRP_CODE != "N") ? 2 :
                                         //               (s_e.SEC_CODE == e.SEC_CODE && e.SEC_CODE != "N") ? 3 :
                                         //               (s_e.DEPT_CODE == e.DEPT_CODE && e.DEPT_CODE != "N") ? 4 :
                                         //               (s_e.DIVI_CODE == e.DIVI_CODE && e.DIVI_CODE != "N") ? 5 : 6
                                         orderby priority, (e.DEPT_CODE == "CAE" ? int.Parse(s_p.POS_HCM_CODE): - int.Parse(s_p.POS_HCM_CODE))
                                         select s_e.EMP_CODE).FirstOrDefault();

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
                        ReportToEmpCode = superiorCode,
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
                        oldItem.ReportToEmpCode = newItem.ReportToEmpCode;
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
        public List<ViewCloneMSTEmployee> UpdateAppendTBMSTEmployee()
        {
            string IssueBy = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
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
                                !new[] { "DDM", "AV", "SAV", "PJ" }.Contains(x.POS_CODE))
                    .ToList();

                var empList = _HRMS.AccEMPLOYEE
                    .Where(x => x.QUIT_CODE == null)
                    .ToList() // ดึงมาจัดการใน Memory ต่อ
                    .Where(emp => emailDict.ContainsKey(emp.EMP_CODE?.Trim()))
                    .OrderBy(x => int.Parse(x.EMP_CODE))
                    .ToList();

                // 3. ประมวลผล Logic หลัก (ใช้ Dictionary แทนการ Query ซ้อน Query)
                var result1 = empList.Select(e =>
                {
                    posNameDict.TryGetValue(e.POS_CODE, out var posName);
                    mstPosDict.TryGetValue(posName ?? "", out var posId);
                    emailDict.TryGetValue(e.EMP_CODE.Trim(), out var email);

                    // หา Superior (ยังคงต้องวนลูปชุดเล็ก แต่จำกัดวงด้วย Division)
                    var currentPos = posListAll.FirstOrDefault(p => p.POS_CODE == e.POS_CODE);
                    int currentHcm = int.Parse(currentPos?.POS_HCM_CODE ?? "999");

                    var superiorCode = (from s_e in empList
                                        join s_p in superiorPosList on s_e.POS_CODE equals s_p.POS_CODE
                                        where s_e.DIVI_CODE == e.DIVI_CODE &&
                                              int.Parse(s_p.POS_HCM_CODE) < currentHcm
                                        let priority = (s_e.UNT_CODE == e.UNT_CODE && e.UNT_CODE != "N") ? 1 :
                                                       (s_e.GRP_CODE == e.GRP_CODE && e.GRP_CODE != "N") ? 2 :
                                                       (s_e.SEC_CODE == e.SEC_CODE && e.SEC_CODE != "N") ? 3 :
                                                       (s_e.DEPT_CODE == e.DEPT_CODE && e.DEPT_CODE != "N") ? 4 : 5
                                        orderby priority, int.Parse(s_p.POS_HCM_CODE) descending
                                        select s_e.EMP_CODE).FirstOrDefault();

                    return new ViewCloneMSTEmployee
                    {
                        EmployeeCode = e.EMP_CODE,
                        Username = email,
                        NameTh = $"{e.PRI_THAI}{e.EMP_TNAME}  {e.LAST_TNAME}",
                        NameEn = $"{e.PRI_ENG}{e.EMP_ENAME}  {e.LAST_ENAME}",
                        Email = email,
                        PositionId = posId,
                        DepartmentId = deptDict.ContainsKey(e.DEPT_CODE) ? deptDict[e.DEPT_CODE] : 0,
                        DivisionId = divDict.ContainsKey(e.DIVI_CODE) ? divDict[e.DIVI_CODE] : 0,
                        ReportToEmpCode = superiorCode,
                        IsActive = true,
                        AccountId = 1,
                        CreatedDate = DateTime.Now,
                        CreatedBy = IssueBy,
                        ModifiedDate = DateTime.Now,
                        ModifiedBy = IssueBy,
                        DataStatus = ""
                    };
                }).ToList();

                // 4. Update หรือ Add ลงใน List หลัก
                //var existingCloneEmpList = new List<ViewCloneMSTEmployee>();



                //var existingEmpList = _WolfApproveCore_thaistanley._ViewMSTEmployee.ToList();

                var existingEmpList = _WolfApproveCore_thaistanley._ViewMSTEmployee
                                             .Select(x => new ViewCloneMSTEmployee
                                             {
                                                 EmployeeId = x.EmployeeId,
                                                 EmployeeCode = x.EmployeeCode,
                                                 Username = x.Username,
                                                 NameTh = x.NameTh,
                                                 NameEn = x.NameEn,
                                                 Email = x.Email,
                                                 PositionId = x.PositionId,
                                                 DepartmentId = x.DepartmentId,
                                                 DivisionId = x.DivisionId,
                                                 ReportToEmpCode = x.ReportToEmpCode,
                                                 IsActive = true,
                                                 AccountId = 1,
                                                 CreatedDate = DateTime.Now,
                                                 CreatedBy = IssueBy,
                                                 ModifiedDate = DateTime.Now,
                                                 ModifiedBy = IssueBy,
                                                 DataStatus = ""
                                             })
                                             .ToList();




                var existingEmpDict = existingEmpList.ToDictionary(x => x.EmployeeCode);
                int nextEmpId = existingEmpList.Any() ? existingEmpList.Max(x => x.EmployeeId) + 1 : 1;

                foreach (var newItem in result1)
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
                        oldItem.ReportToEmpCode = newItem.ReportToEmpCode;
                        oldItem.DataStatus = "Old";
                    }
                    else
                    {
                        // Add
                        newItem.EmployeeId = nextEmpId++;
                        newItem.DataStatus = "New";
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

                @class._ListViewCloneMSTEmployee = existingEmpList;
            }
            catch (Exception ex)
            {
                string mesr = ex.Message;
                // แนะนำให้ใช้ Logger แทนการเก็บลง string ครับ
            }

            return @class._ListViewCloneMSTEmployee;
        }
        public List<ViewWOLFAccount> UpdateWOLFAccount()
        {
            string IssueBy = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
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
                    Password = "Uat@tse1",
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


        public List<ViewMSTDivision> UpdateTBMSTDivision()
        {
            List<ViewMSTDivision> _listViewMSTDivision = new List<ViewMSTDivision>();
            try
            {
                string IssueBy = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
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
            try
            {
                string IssueBy = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                var ViewMSTDivision = _ListViewMSTDivision.ToDictionary(x => x.DivisionCode);
                var ListviewMSTDepartments = _HRMS.AccDEPTMAST.OrderBy(x => x.DeptNo).ToList();


                var divisionDict = _ListViewMSTDivision
    .ToDictionary(x => x.DivisionCode);
                var deptList = _HRMS.AccDEPTMAST
       .OrderBy(x => x.DeptNo)
       .ToList(); // 👈 ตัด EF ตรงนี้

                int vrow = 1;
                _listViewMSTDepartment = deptList
                    .Where(x => divisionDict.ContainsKey(x.DEPT_CODE))
                    .Select(item => new ViewMSTDepartment
                    {
                        DepartmentId = vrow++,
                        ParentId = 0,
                        DivisionId = divisionDict[item.DEPT_CODE].DivisionId,
                        DepartmentCode = item.DEPT_CODE,
                        NameTh = item.DEPT_NAME,
                        NameEn = item.DEPT_NAME,
                        CreatedBy = IssueBy,
                        ModifiedBy = IssueBy,
                        IsActive = true,
                        AccountId = 1,
                        CompanyCode = "01111" //01111
                    })
                    .ToList();



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

            return _listViewMSTDepartment;
        }
        public List<ViewMSTPosition> UpdateTBMSTPosition()
        {
            List<ViewMSTPosition> _listViewMSTPosition = new List<ViewMSTPosition>();
            try
            {
                string IssueBy = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                int vrow = 1;
                int vProw = 1;
                var _listViewAccPOSMAST = _HRMS.AccPOSMAST.OrderByDescending(x => int.Parse(x.POS_HCM_CODE)).AsEnumerable()
                    .Select(x => new ViewMSTPosition
                    {
                        PositionId = vrow++,
                        NameTh = x.POS_NAME,
                        NameEn = x.POS_NAME,
                        PositionLevelId = vProw++,
                        IsActive = true,
                        //CreatedDate
                        CreatedBy = IssueBy,
                        //ModifiedDate
                        //ModifiedBy 
                        AccountId = 1,
                        CompanyCode = "1",

                    })
                    .ToList();

                //_listViewMSTPosition = _WolfApproveCore_thaistanley._ViewMSTPosition.ToList();
                _listViewMSTPosition = _listViewAccPOSMAST;
            }
            catch (Exception ex)
            {
                string msgr = ex.Message;
            }

            return _listViewMSTPosition;
        }

        [HttpPost]
        public JsonResult CheckData(Class @class, string _ListViewMSTATACCEmployee, string _ListViewMSTATEmployee, string _ListViewMSTEmployee, string _ListViewWOLFAccount,
            string _ListViewMSTDivision,
            string _ListViewMSTDepartment,
            string _ListViewMSTPosition,
            string vType, string tsave)
        {

            string config = "S";
            string msg = "Success !!";
            string[] chkSave;

            try
            {
                if (vType == "Accemployee") //update employee
                {
                    if (_ListViewMSTATACCEmployee != null)
                    {
                        @class._ListViewMSTATACCEmployee = JsonConvert.DeserializeObject<List<ViewMSTATACCEmployee>>(_ListViewMSTATACCEmployee);
                    }
                    if (_ListViewMSTATEmployee != null)
                    {
                        @class._ListViewMSTATEmployee = JsonConvert.DeserializeObject<List<ViewMSTATEmployee>>(_ListViewMSTATEmployee);
                    }
                    if (_ListViewMSTEmployee != null)
                    {
                        @class._ListViewMSTEmployee = JsonConvert.DeserializeObject<List<ViewMSTEmployee>>(_ListViewMSTEmployee);
                    }
                    if (_ListViewWOLFAccount != null)
                    {
                        @class._ListViewWOLFAccount = JsonConvert.DeserializeObject<List<ViewWOLFAccount>>(_ListViewWOLFAccount);
                    }
                    //Accemployee
                    chkSave = saveDataAccem(@class, tsave);
                    msg = chkSave[1];
                    if (chkSave[0] == "E")
                    {
                        config = chkSave[0];
                        //msg = chkSave[1];
                        return Json(new { c1 = config, c2 = msg });
                    }

                }
                else if (vType == "Div")
                {
                    if (_ListViewMSTDivision != null)
                    {
                        @class._ListViewMSTDivision = JsonConvert.DeserializeObject<List<ViewMSTDivision>>(_ListViewMSTDivision);
                        @class._ListViewMSTDivision.OrderBy(x => x.DivisionId);
                    }
                    if (_ListViewMSTDepartment != null)
                    {
                        @class._ListViewMSTDepartment = JsonConvert.DeserializeObject<List<ViewMSTDepartment>>(_ListViewMSTDepartment);
                        @class._ListViewMSTDepartment.OrderBy(x => x.DivisionId);
                    }
                    if (_ListViewMSTPosition != null)
                    {
                        @class._ListViewMSTPosition = JsonConvert.DeserializeObject<List<ViewMSTPosition>>(_ListViewMSTPosition);
                        @class._ListViewMSTPosition.OrderBy(x => x.PositionLevelId);
                    }
                    chkSave = saveDataDivi(@class, tsave);
                    msg = chkSave[1];
                    if (chkSave[0] == "E")
                    {
                        config = chkSave[0];
                        //msg = chkSave[1];
                        return Json(new { c1 = config, c2 = msg });
                    }
                }
            }
            catch (Exception ex)
            {
                config = "E";
                msg = ex.Message;

            }
            return Json(new { c1 = config, c2 = msg });

        }
        public string[] saveDataAccem(Class @class, string tsave)
        {
            string config = "S";
            string msg = "Save Data Success !!!";
            string IssueBy = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

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
                                item.CreatedBy,
                                DateTime.Now,//item.ModifiedDate,
                                item.ModifiedBy,
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
                        _VWOLFAccount.IsActive = true;
                        _WolfApproveCore_Center._ViewWOLFAccount.AddAsync(_VWOLFAccount);
                    }


                    //table account wolf center
                    //ViewWOLFAccount _ViewWOLFAccount = new ViewWOLFAccount();
                    ////_ViewWOLFAccount.ID 
                    //_ViewWOLFAccount.ContactCode = "thaistanley";
                    //_ViewWOLFAccount.Username = "THS015388@stanley-electric.com";
                    //_ViewWOLFAccount.Password = MD5Hash("Uat@tse1");
                    //_ViewWOLFAccount.IsVerify = true;
                    //_ViewWOLFAccount.GuidVerify = "047e0d17-ef94-42a9-9a8b-f525a2e81fdd"; //มาจากไหนไม่รู้
                    //_ViewWOLFAccount.Note = "";
                    //_ViewWOLFAccount.Remark = "";
                    //_ViewWOLFAccount.Description = "";
                    //_ViewWOLFAccount.CreatedDate = DateTime.Now;
                    //_ViewWOLFAccount.CreatedBy = IssueBy;
                    //_ViewWOLFAccount.ModifiedDate = DateTime.Now;
                    //_ViewWOLFAccount.IsActive = true;

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
        public string[] saveDataAccem1(Class @class, string tsave)
        {
            string config = "S";
            string msg = "Save Data Success !!!";
            string IssueBy = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

            //WolfApproveCore
            using (var dbContextTransaction = _WolfApproveCore_thaistanley.Database.BeginTransaction())
            {
                try
                {
                    //SqlBulk insert

                    //รีเซ็ต Identity

                    _WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("Delete from MSTATACCEmployee");
                    _WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('MSTATACCEmployee', RESEED, 0)");
                    for (int i = 0; i < @class._ListViewMSTATACCEmployee.Count(); i++)
                    {
                        var _ViewMSTATACCEmployee = new ViewMSTATACCEmployee();
                        //_ViewMSTATACCEmployee.EMPID = @class._ListViewMSTATACCEmployee[i].EMPID;
                        _ViewMSTATACCEmployee.EMPCODE = @class._ListViewMSTATACCEmployee[i].EMPCODE;
                        _ViewMSTATACCEmployee.Name = @class._ListViewMSTATACCEmployee[i].Name;
                        _ViewMSTATACCEmployee.NameTH = @class._ListViewMSTATACCEmployee[i].NameTH;
                        _ViewMSTATACCEmployee.JOB_NAME = @class._ListViewMSTATACCEmployee[i].JOB_NAME;
                        _ViewMSTATACCEmployee.PositionName = @class._ListViewMSTATACCEmployee[i].PositionName;
                        _ViewMSTATACCEmployee.DivisionName = @class._ListViewMSTATACCEmployee[i].DivisionName;
                        _ViewMSTATACCEmployee.DepartmentName = @class._ListViewMSTATACCEmployee[i].DepartmentName;
                        _ViewMSTATACCEmployee.SECName = @class._ListViewMSTATACCEmployee[i].SECName;
                        _ViewMSTATACCEmployee.GRPName = @class._ListViewMSTATACCEmployee[i].GRPName;
                        _ViewMSTATACCEmployee.UNTName = @class._ListViewMSTATACCEmployee[i].UNTName;
                        _ViewMSTATACCEmployee.DIRECT_INDIRECT_CODE = @class._ListViewMSTATACCEmployee[i].DIRECT_INDIRECT_CODE;
                        _ViewMSTATACCEmployee.INTERCOMNO = @class._ListViewMSTATACCEmployee[i].INTERCOMNO;
                        _ViewMSTATACCEmployee.NICKNAME = @class._ListViewMSTATACCEmployee[i].NICKNAME;
                        _WolfApproveCore_thaistanley._ViewMSTATACCEmployee.Add(_ViewMSTATACCEmployee);

                    }

                    _WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("Delete from MSTATEmployee");
                    _WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('MSTATEmployee', RESEED, 0)");
                    for (int i = 0; i < @class._ListViewMSTATEmployee.Count(); i++)
                    {
                        var _ViewMSTATEmployee = new ViewMSTATEmployee();
                        //EMPID
                        _ViewMSTATEmployee.EMPCODE = @class._ListViewMSTATEmployee[i].EMPCODE;
                        _ViewMSTATEmployee.NICKNAME = @class._ListViewMSTATEmployee[i].NICKNAME;
                        _ViewMSTATEmployee.INTERCOMNO = @class._ListViewMSTATEmployee[i].INTERCOMNO;
                        _ViewMSTATEmployee.JOBCODE = @class._ListViewMSTATEmployee[i].JOBCODE;
                        _ViewMSTATEmployee.SECNAME = @class._ListViewMSTATEmployee[i].SECNAME;
                        _ViewMSTATEmployee.GRPNAME = @class._ListViewMSTATEmployee[i].GRPNAME;
                        _ViewMSTATEmployee.UNTNAME = @class._ListViewMSTATEmployee[i].UNTNAME;
                        _WolfApproveCore_thaistanley._ViewMSTATEmployee.Add(_ViewMSTATEmployee);

                    }


                    if (tsave.Equals("Append"))
                    {
                        @class._ListViewMSTEmployee = @class._ListViewMSTEmployee.Where(x => x.Userid_Line == "New").ToList();

                    }
                    else
                    {
                        _WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("Delete from MSTEmployee");
                        _WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('MSTEmployee', RESEED, 0)");

                    }
                    for (int i = 0; i < @class._ListViewMSTEmployee.Count(); i++)
                    {
                        var _ViewMSTEmployee = new ViewMSTEmployee();
                        //EmployeeId
                        _ViewMSTEmployee.EmployeeCode = @class._ListViewMSTEmployee[i].EmployeeCode;
                        _ViewMSTEmployee.Username = @class._ListViewMSTEmployee[i].Username;
                        _ViewMSTEmployee.NameTh = @class._ListViewMSTEmployee[i].NameTh;
                        _ViewMSTEmployee.NameEn = @class._ListViewMSTEmployee[i].NameEn;
                        _ViewMSTEmployee.Email = @class._ListViewMSTEmployee[i].Email;
                        _ViewMSTEmployee.IsActive = @class._ListViewMSTEmployee[i].IsActive;
                        _ViewMSTEmployee.PositionId = @class._ListViewMSTEmployee[i].PositionId;
                        _ViewMSTEmployee.DepartmentId = @class._ListViewMSTEmployee[i].DepartmentId;
                        _ViewMSTEmployee.ReportToEmpCode = @class._ListViewMSTEmployee[i].ReportToEmpCode;
                        _ViewMSTEmployee.SignPicPath = @class._ListViewMSTEmployee[i].SignPicPath;
                        _ViewMSTEmployee.Lang = @class._ListViewMSTEmployee[i].Lang;
                        _ViewMSTEmployee.AccountId = @class._ListViewMSTEmployee[i].AccountId;
                        _ViewMSTEmployee.CreatedDate = DateTime.Now;
                        _ViewMSTEmployee.CreatedBy = IssueBy;
                        _ViewMSTEmployee.ModifiedDate = DateTime.Now;
                        _ViewMSTEmployee.ModifiedBy = "";
                        _ViewMSTEmployee.ADTitle = @class._ListViewMSTEmployee[i].ADTitle;
                        _ViewMSTEmployee.DivisionId = @class._ListViewMSTEmployee[i].DivisionId;
                        _ViewMSTEmployee.EmpLevel = @class._ListViewMSTEmployee[i].EmpLevel;
                        _ViewMSTEmployee.EMPL_RCD = @class._ListViewMSTEmployee[i].EMPL_RCD;
                        _ViewMSTEmployee.EmployeeLevel = @class._ListViewMSTEmployee[i].EmployeeLevel;
                        _ViewMSTEmployee.EffectiveDate = DateTime.Now;
                        _ViewMSTEmployee.Userid_Line = @class._ListViewMSTEmployee[i].Userid_Line;
                        _WolfApproveCore_thaistanley._ViewMSTEmployee.Add(_ViewMSTEmployee);

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
                        _VWOLFAccount.IsActive = true;
                        _WolfApproveCore_Center._ViewWOLFAccount.AddAsync(_VWOLFAccount);
                    }

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




        public string[] saveDataDivi(Class @class, string tsave)
        {
            string config = "S";
            string msg = "Save Data Success !!!";
            string IssueBy = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            using (var dbContextTransaction = _WolfApproveCore_thaistanley.Database.BeginTransaction())
            {
                try
                {
                    var connection = (SqlConnection)_WolfApproveCore_thaistanley.Database.GetDbConnection();
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    // _WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("TRUNCATE TABLE MSTDivision");
                    //@class._ListViewMSTDivision
                    //for (int i = 0; i < @class._ListViewMSTDivision.Count(); i++)
                    //{
                    //    var viewMSTDivision = new ViewMSTDivision();
                    //    //DivisionId
                    //    viewMSTDivision.NameTh = @class._ListViewMSTDivision[i].NameTh;
                    //    viewMSTDivision.NameEn = @class._ListViewMSTDivision[i].NameEn;
                    //    viewMSTDivision.CreatedDate = DateTime.Now; // @class._ListViewMSTDivision[i].CreatedDate;
                    //    viewMSTDivision.CreatedBy = IssueBy;//@class._ListViewMSTDivision[i].CreatedBy;
                    //    viewMSTDivision.ModifiedDate = DateTime.Now;// @class._ListViewMSTDivision[i].ModifiedDate;
                    //    viewMSTDivision.ModifiedBy = "";//IssueBy;// @class._ListViewMSTDivision[i].ModifiedBy;
                    //    viewMSTDivision.IsActive = @class._ListViewMSTDivision[i].IsActive;
                    //    viewMSTDivision.AccountId = @class._ListViewMSTDivision[i].AccountId;
                    //    viewMSTDivision.DivisionCode = @class._ListViewMSTDivision[i].DivisionCode;
                    //    _WolfApproveCore_thaistanley._ViewMSTDivision.Add(viewMSTDivision);

                    //}
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
                                item.CreatedBy,
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
                    // _WolfApproveCore_thaistanley.SaveChanges();
                    //@class._ListViewMSTPosition
                    //_WolfApproveCore_thaistanley.Database.ExecuteSqlCommand("TRUNCATE TABLE MSTPosition");
                    //for (int i = 0; i < @class._ListViewMSTPosition.Count(); i++)
                    //{
                    //    var viewMSTPosition = new ViewMSTPosition();
                    //    // PositionId
                    //    viewMSTPosition.NameTh = @class._ListViewMSTPosition[i].NameTh;
                    //    viewMSTPosition.NameEn = @class._ListViewMSTPosition[i].NameEn;
                    //    viewMSTPosition.PositionLevelId = @class._ListViewMSTPosition[i].PositionLevelId;
                    //    viewMSTPosition.IsActive = @class._ListViewMSTPosition[i].IsActive;
                    //    viewMSTPosition.CreatedDate = DateTime.Now; ;//@class._ListViewMSTPosition[i].NameTh;
                    //    viewMSTPosition.CreatedBy = IssueBy;//@class._ListViewMSTPosition[i].NameTh;
                    //    viewMSTPosition.ModifiedDate = DateTime.Now; ;//@class._ListViewMSTPosition[i].NameTh;
                    //    viewMSTPosition.ModifiedBy = @class._ListViewMSTPosition[i].ModifiedBy;
                    //    viewMSTPosition.AccountId = @class._ListViewMSTPosition[i].AccountId;
                    //    viewMSTPosition.CompanyCode = @class._ListViewMSTPosition[i].CompanyCode;
                    //    _WolfApproveCore_thaistanley._ViewMSTPosition.Add(viewMSTPosition);
                    //}





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

        [HttpPost]
        [RequestSizeLimit(104857600)] // 100 MB ต่อ batch
        public async Task<IActionResult> Upload()
        {
            Class @class = new Class();

            //var form = await Request.ReadFormAsync();
            //var files = form.Files; // ถ้ามีไฟล์

            var form = await Request.ReadFormAsync();
            var files = form.Files;

            // อ่าน tbodyId
            var tbodyId = form["tbodyId"].ToString();

            @class._ListViewMSTATACCEmployee = new List<ViewMSTATACCEmployee>();
            @class._ListViewMSTATEmployee = new List<ViewMSTATEmployee>();


            var rowDict = new Dictionary<int, ViewMSTATACCEmployee>();


            @class._ListViewMSTEmployee = new List<ViewMSTEmployee>();

            foreach (var key in form.Keys)
            {

                if (key == "tbodyId") continue; // ข้าม field ชื่อ tbodyId

                var match = System.Text.RegularExpressions.Regex.Match(key, @"row\[(\d+)\]\[(\w+)\]");
                if (match.Success)
                {
                    int rowIndex = int.Parse(match.Groups[1].Value);
                    string fieldName = match.Groups[2].Value;
                    string value = form[key];

                    if (!rowDict.ContainsKey(rowIndex))
                        rowDict[rowIndex] = new ViewMSTATACCEmployee();

                    var row = rowDict[rowIndex];

                    switch (fieldName)
                    {
                        case "EMPID": row.EMPID = int.Parse(value); break;
                        case "EMPCODE": row.EMPCODE = value; break;
                        case "NICKNAME": row.NICKNAME = value; break;
                        case "INTERCOMNO": row.INTERCOMNO = value; break;
                            //case "JOBCODE": row.JOBCODE = value; break;
                            //case "SECNAME": row.SECNAME = value; break;
                            //case "GRPNAME": row.GRPNAME = value; break;
                            //case "UNTNAME": row.UNTNAME = value; break;
                    }
                }
            }

            @class._ListViewMSTATACCEmployee.AddRange(rowDict.Values);



            return Json(new { success = true });
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

    }
}