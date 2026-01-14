using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using ImportMasterWolf.Controllers.Approval;
using ImportMasterWolf.Models.Common;
using ImportMasterWolf.Models.New;
using ImportMasterWolf.Models.Table.HRMS;
using ImportMasterWolf.Models.Table.LAMP;

namespace ImportMasterWolf.Controllers
{
    public class FunctionsController : Controller
    {
        private CacheSettingController _Cache;
        public FunctionsController(CacheSettingController cacheController)
        {
            _Cache = cacheController;
        }

        public async Task<PartialViewResult> ViewOTDetail(string req)
        {
            if(req != null)
                req = "OT" + req.Split("OT")[1].Split("=")[0];
            string reqDepartment = User.Claims.FirstOrDefault(s => s.Type == "Department").Value?.ToString();
            multiEditModel editModel = new multiEditModel();

            //main model
            editModel.ViewMastRequestOT = new ViewMastRequestOT();
            editModel.multiModelOTForm = new multiModelOTForm();
            editModel.multiOTEmailForm = new multiOTEmailForm();
            editModel.multiModelCateWorker = new List<multiModelCateWorker>();

            //new mastrequestot model
            editModel.multiModelOTForm.mastRequestOT = new ViewMastRequestOT();
            editModel.multiOTEmailForm.EFMastRequestOT = new ViewMastRequestOT();

            //get MastRequestOT put in OTMyData, OTForm
            _Cache.clearCacheMastRequestOT();
            ViewMastRequestOT requestOT = Task.Run(() => _Cache.cacheMastRequestOT().Where(w => w.mrNoReq == req).FirstOrDefault()).Result;
            editModel.ViewMastRequestOT = requestOT;
            editModel.multiModelOTForm.mastRequestOT = requestOT;
            editModel.multiOTEmailForm.EFMastRequestOT = requestOT;

            //set format date dd/MM/yyyy to yyyy-MM-dd for input type=date
            string[] dmyOTDate = editModel.multiModelOTForm.mastRequestOT.mrOTDate.Split("/");
            editModel.multiModelOTForm.mastRequestOT.mrOTDate = dmyOTDate.Length > 1 ? dmyOTDate[2] + "-" + dmyOTDate[1] + "-" + dmyOTDate[0] : "";

            //OTForm
            //set prod line list
            editModel.multiModelOTForm.prodLines = new List<OTProdLine>();
            foreach (var row in _Cache.cacheMastProdLine().Where(w => w.plant.StartsWith(TransferDepartmentToPlant(reqDepartment))).ToList())
            {
                editModel.multiModelOTForm.prodLines.Add(
                    new OTProdLine()
                    {
                        Name = row.prodline
                    });
            }

            //set model list
            string qryModelOfProdLine = new string("");
            if (_Cache.cacheMastModel().Where(w => w.mmModelName == requestOT.mrModel).Select(s => s.mmProdline).FirstOrDefault() != null)
                qryModelOfProdLine = _Cache.cacheMastModel().Where(w => w.mmModelName == requestOT.mrModel).Select(s => s.mmProdline).FirstOrDefault();

            editModel.multiModelOTForm.models = new List<OTModel>();
            foreach (var row in _Cache.cacheMastModel().Where(w => w.mmProdline == qryModelOfProdLine).ToList())
            {
                editModel.multiModelOTForm.models.Add(
                    new OTModel()
                    {
                        Name = row.mmModelName
                    });
            }

            //set reason list
            editModel.multiModelOTForm.reasons = new List<OTReason>();
            foreach (var row in _Cache.cacheMastOTReason().ToList())
            {
                editModel.multiModelOTForm.reasons.Add(new OTReason()
                {
                    Code = row.mrCode,
                    Caption = row.mrReasonTH
                });
            }

            //OTAddWorker

            foreach (var items in _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == req))
            {
                multiModelCateWorker worker = new multiModelCateWorker();
                worker.CategoryWorkerList = new CategoryWorkerList();
                worker.Jobs = new List<ViewMastJob>();
                worker.CategoryWorkerList.EmpCode = items.drEmpCode;
                worker.CategoryWorkerList.PriName = items.drPriName;
                worker.CategoryWorkerList.Name = items.drName;
                worker.CategoryWorkerList.Surname = items.drLastName;
                worker.CategoryWorkerList.GRP_Code = items.drGrp;
                worker.CategoryWorkerList.Job = items.drJobCode;
                foreach (var job in _Cache.cacheMastJob().ToList())
                {
                    worker.Jobs.Add(
                    new ViewMastJob()
                    {
                        mjJobCode = job.mjJobCode,
                        mjJobName = job.mjJobName,
                    });
                }
                try
                {
                    WebClient request = new WebClient();
                    string imgPath = GlobalVariable.imgPath + "/" + items.drEmpCode + ".jpg";
                    request.Credentials = new NetworkCredential(GlobalVariable.UFTP, GlobalVariable.PFTP);
                    byte[] imgFile = request.DownloadData(imgPath);
                    string file64String = Convert.ToBase64String(imgFile);
                    string imgDataURL = string.Format("data:image/jpg;base64,{0}", file64String);
                    worker.image = imgDataURL;
                }
                catch (WebException wex)
                {
                    Console.WriteLine(wex.ToString());
                }
                editModel.multiModelCateWorker.Add(worker);
            }

            //OTEmailForm
            string posApprover = _Cache.cacheMastFlowApprove().Where(w => w.mfStep == requestOT.mrStep && w.mfFlowNo == requestOT.mrFlow).Select(s => s.mfPermission).FirstOrDefault();
            string perAdmin = _Cache.cacheMastUserApprove().Where(w => w.muEmpCode == requestOT.mrEmpApp).Select(s => s.muPosition).FirstOrDefault();
            if (perAdmin != null){
                perAdmin = perAdmin.ToUpper();
                if (perAdmin == GlobalVariable.perAdmin.ToUpper())
                    posApprover = perAdmin;
            }
            //editModel.multiOTEmailForm.positionNext = TransLevelToPosition(TransPositionToLevel(posApprover) - 1);
            editModel.multiOTEmailForm.positionNext = posApprover;
            editModel.multiOTEmailForm.historyApproveds = new ViewHistoryApproved();
            editModel.multiOTEmailForm.historyApproveds = _Cache.cacheHistoryApproved().Where(w => w.htNoReq == req && w.htStep == requestOT.mrStep).FirstOrDefault();

            return await Task.Run(() => PartialView("~/Views/Shared/_ptvViewOTDetail.cshtml", editModel));
        }

        public bool SendEmail(string mailSubject ,string mailSender, string mailReceiver, string mailBody,ViewAccEMPLOYEE reqProfile, ViewAccEMPLOYEE profileApprover, string mailCC, ViewMastRequestOT reqData, string remark)
        {
            if (reqProfile is null){
                reqProfile = new ViewAccEMPLOYEE();
                reqProfile.PRI_THAI = "";
                reqProfile.EMP_TNAME = "ผู้ร้องขอ";
                reqProfile.LAST_TNAME = "";
            }
            if(profileApprover is null)
            {
                profileApprover = new ViewAccEMPLOYEE();
                profileApprover.EMP_TNAME = "";
                profileApprover.LAST_TNAME = "";
            }

            if (mailReceiver != null && mailReceiver != "")
            {
                var senderEmail = new MailAddress(mailSender, GlobalVariable.ProgramName);
                var receiverEmail = new MailAddress(mailReceiver, "คุณ" + profileApprover.EMP_TNAME + " " + profileApprover.LAST_TNAME);
                var subject = mailSubject;
                ContentType mimeType = new System.Net.Mime.ContentType("text/html");

                var body = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
                body += "<HTML><HEAD><META http-equiv=Content-Type content=\"text/html; charset=UTF-8\">";
                body += "</HEAD><BODY><DIV style='font-family: Browallia New, sans-serif; font-size: 30px;'>";
                body += "<p><b>เรียน</b> คุณ" + profileApprover.EMP_TNAME + " " + profileApprover.LAST_TNAME + "</p>";
                body += "<p><b>เรื่อง</b> สถานะการอนุมัติขอทำงานล่วงเวลา</p>";
                body += "<DIV>ขอแจ้งสถานะคำร้องของพนักงานที่มีการยื่นคำร้องการของทำงานล่วงเวลา โดยมีรายละเอียดดังนี้</ DIV>";
                body += "<DIV>ชื่อ นามสกุล: " + reqData.mrPriNameReq + reqData.mrNameReq + " " + reqData.mrLastNameReq + "</DIV>";
                body += "<DIV>วันที่ขอ: " + reqData.mrDateReq + "</DIV>";
                body += "<DIV>วันที่ขอเริ่มต้น(): " + reqData.mrOTDate + "</DIV>";
                if (reqData.mrOTTimeSt_Before != null && reqData.mrOTTimeSt_Before != "")
                {
                    body += "<DIV>ตั้งแต่เวลา(ก่อนเริ่มเวลางาน): " + reqData.mrOTTimeSt_Before + "</DIV>";
                    body += "<DIV>ถึงเวลา(ก่อนเริ่มเวลางาน): " + reqData.mrOTTimeEd_Before + "</DIV>";
                }
                if (reqData.mrOTTimeSt_During != null && reqData.mrOTTimeSt_During != "")
                {
                    body += "<DIV>ตั้งแต่เวลา(ระหว่างเวลางาน): " + reqData.mrOTTimeSt_During + "</DIV>";
                    body += "<DIV>ถึงเวลา(ระหว่างเวลางาน): " + reqData.mrOTTimeEd_During + "</DIV>";
                }
                if (reqData.mrOTTimeSt_After != null && reqData.mrOTTimeSt_After != "")
                {
                    body += "<DIV>ตั้งแต่เวลา(หลังเลิกเวลางาน): " + reqData.mrOTTimeSt_After + "</DIV>";
                    body += "<DIV>ถึงเวลา(หลังเลิกเริ่มงาน): " + reqData.mrOTTimeEd_After + "</DIV>";
                }

                body += "<DIV>ฝ่าย: " + reqData.mrDeptReq + "</DIV>";
                body += "<DIV>ไลน์การผลิต: " + reqData.mrProductionLine + "</ DIV>";
                body += "<DIV>โมเดล: " + reqData.mrModel + "</DIV>";
                if (body != GlobalVariable.PermissionHCM)
                    body += "<DIV>สถานะ: "; body += (mailBody == GlobalVariable.StatusRejected ? GlobalVariable.StatusRejected : GlobalVariable.StatusApproved + " โดย คุณ" + reqProfile.EMP_TNAME + " " + reqProfile.LAST_TNAME); body += "</DIV>";
                if (remark != null && remark != "")
                    body += "<DIV>หมายเหตุ(ผู้อนุมัติ/ผู้ไม่อนุมัติ): " + remark + "</DIV>";

                body += "<DIV>เอกสาร: <a href='http://thsweb/MVCPublish/ImportMasterWolf?req=" + reqData.mrNoReq + "'>" + reqData.mrNoReq + "</a></DIV>";
                body += "</DIV></BODY></HTML>";

                AlternateView alternate = AlternateView.CreateAlternateViewFromString(body, mimeType);

                SmtpClient smtp = new SmtpClient(GlobalVariable.ServerEmail);
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                using (MailMessage mess = new MailMessage(senderEmail, receiverEmail))
                {
                    mess.Subject = subject;
                    if (mailCC != null && mailCC != "")
                        foreach (string email in mailCC.Split(","))
                        {
                            if (!string.IsNullOrEmpty(email))
                                mess.CC.Add(email);
                        }
                    mess.AlternateViews.Add(alternate);
                    try
                    {
                        smtp.Send(mess);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught in CreateCopyMessage(): {0}", ex.ToString());
                    }
                }
                return true;
            }
            return true;
        }

        public string FindEmpCodeFromEmail(string email)
        {
            if (email == GlobalVariable.AdminEmail)
                return GlobalVariable.AdminEmpCode;
            string empcode = _Cache.cacheEmail().Where(w => w.emEmail_M365 != null && w.emEmail_M365.ToLower().Trim() == email.ToLower().Trim()).FirstOrDefault() is null ? "" : _Cache.cacheEmail().Where(w => w.emEmail_M365.ToLower().Trim() == email.ToLower().Trim()).FirstOrDefault().emEmpcode;
            return empcode;
        }

        public string FindEmailFromEmpCode(string empcode)
        {
            if (empcode == GlobalVariable.AdminEmpCode)
                return GlobalVariable.AdminEmail;
            string email = _Cache.cacheEmail().Where(w => w.emEmail_M365 != null && w.emEmpcode.Trim() == empcode.Trim()).FirstOrDefault() is null ? "" : _Cache.cacheEmail().Where(w => w.emEmpcode.Trim() == empcode.Trim()).FirstOrDefault().emEmail_M365.Trim();
            return email;
        }

        public int PreviosStepSelectWord(string position)
        {
            int condition = TransPositionToLevel(position);
            int level = 1;
            if (condition == 5)             //DM
                level++;
            return level;
        }

        public int TransPositionToLevel(string Position)
        {
            int level = new int();
            switch (Position.ToUpper())
            {
                case "UL":
                    level = 1;
                    break;
                case "GL": case "ZZ":
                    level = 2;
                    break;
                case "CS":
                    level = 3;
                    break;
                case "DDM":
                    level = 4;
                    break;
                case "DM":
                    level = 5;
                    break;
                case "ADMIN":
                    level = 6;
                    break;
                case "HCM":
                    level = 7;
                    break;
            }
            return level;
        }

        public string TransLevelToPosition(int Level)
        {
            string position = "";
            switch (Level)
            {
                case 1:
                    position = "UL";
                    break;
                case 2:
                    position = "GL";
                    break;
                case 3:
                    position = "CS";
                    break;
                case 4:
                    position = "DDM";
                    break;
                case 5:
                    position = "DM";
                    break;
                case 6:
                    position = "Admin";
                    break;
                case 7:
                    position = "HCM";
                    break;
            }
            return position;
        }

        public string TransPositionToFlowType(string PositionToUpper)
        {
            string flowType = "";
            switch (PositionToUpper)
            {
                case "UL":
                    flowType = "1";
                    break;
                case "GL":
                case "ZZ":
                    flowType = "2";
                    break;
            }
            return flowType;
        }

        public string TransferDivisionToCodeName(string FullNameDivision)
        {
            string codeName = _Cache.cacheAccDIVIMast().Where(w => w.DIVI_NAME.Trim() == FullNameDivision.Trim()).FirstOrDefault() is null ? null :_Cache.cacheAccDIVIMast().Where(w => w.DIVI_NAME.Trim() == FullNameDivision.Trim()).FirstOrDefault().DIVI_CODE;
            if ((FullNameDivision != null || FullNameDivision != "") && codeName == null)
                codeName = FullNameDivision;
            return codeName;
        }

        public string TransferCodeNameToDivision(string DiviCode)
        {
            string Name = _Cache.cacheAccDIVIMast().Where(w => w.DIVI_CODE.Trim() == DiviCode.Trim()).FirstOrDefault() is null ? null : _Cache.cacheAccDIVIMast().Where(w => w.DIVI_CODE.Trim() == DiviCode.Trim()).FirstOrDefault().DIVI_NAME;
            if ((DiviCode != null || DiviCode != "") && Name == null)
                Name = DiviCode;
            return Name;
        }

        public string TransferCodeNameToGroup(string GroupCode)
        {
            string Name = _Cache.cacheGRPMast().Where(w => w.GRP_CODE.Trim() == GroupCode.Trim()).FirstOrDefault() is null ? null : _Cache.cacheGRPMast().Where(w => w.GRP_CODE.Trim() == GroupCode.Trim()).FirstOrDefault().GRP_NAME;
            if ((GroupCode != null || GroupCode != "") && Name == null)
                Name = GroupCode;
            return Name;
        }

        public string TransferGroupToCodeName(string GroupName)
        {
            string grpCode = _Cache.cacheGRPMast().Where(w => w.GRP_NAME.Trim() == GroupName.Trim()).FirstOrDefault() is null ? null : _Cache.cacheGRPMast().Where(w => w.GRP_NAME.Trim() == GroupName.Trim()).FirstOrDefault().GRP_CODE;
            if ((GroupName != null || GroupName != "") && grpCode == null)
                grpCode = GroupName;
            return grpCode;
        }

        public string TransferDivisionToPlant(string Division)
        {
            int defaultPlant = 3000;
            string plant = "";
            string codeName = TransferDivisionToCodeName(Division);
            try {
                if (codeName.IndexOf("LE") >= 0)
                {
                    plant = int.Parse(codeName.Split("LE")[1]) + defaultPlant.ToString();
                }
                if (codeName == GlobalVariable.AdminDivision)
                {
                    plant = "300";
                }
            }
            catch{
                plant = "";
            }
            
            return plant;
        }

        public string TransferDepartmentToCodeName(string FullNameDepartment)
        {
            string codeName = _Cache.cacheDEPTMast().Where(w => w.DEPT_NAME.Trim() == FullNameDepartment.Trim()).FirstOrDefault() is null ? null : _Cache.cacheDEPTMast().Where(w => w.DEPT_NAME.Trim() == FullNameDepartment.Trim()).FirstOrDefault().DEPT_CODE;
            if ((FullNameDepartment != null || FullNameDepartment != "") && codeName == null)
                codeName = FullNameDepartment;

            return codeName;
        }

        public string TransferCodeNameToDepartment(string DeptCode)
        {
            string Name = _Cache.cacheDEPTMast().Where(w => w.DEPT_CODE.Trim() == DeptCode.Trim()).FirstOrDefault() is null ? null : _Cache.cacheDEPTMast().Where(w => w.DEPT_CODE.Trim() == DeptCode.Trim()).FirstOrDefault().DEPT_NAME;

            if ((DeptCode != null || DeptCode != "") && Name == null)
                Name = DeptCode;

            return Name;
        }

        public string TransNumberToMonth(int Number)
        {
            string[] Month = { "", "ม.ค.", "ก.พ.", "มี.ค.", "เม.ย.", "พ.ค.", "มิ.ย.", "ก.ค.", "ส.ค.", "ก.ย.", "ต.ม.", "พ.ย.", "ธ.ค." };
            return Month[Number];
        }

        public string TransferCodeNameToSection(string SectCode)
        {
            string Name = _Cache.cacheSECMast().Where(w => w.SEC_CODE.Trim() == SectCode.Trim()).FirstOrDefault() is null ? null : _Cache.cacheSECMast().Where(w => w.SEC_CODE.Trim() == SectCode.Trim()).FirstOrDefault().SEC_NAME;

            if ((SectCode != null || SectCode != "") && Name == null)
                Name = SectCode;

            return Name;
        }

        public string TransferSectionToCodName(string Section)
        {
            string Code = _Cache.cacheSECMast().Where(w => w.SEC_NAME.Trim() == Section.Trim()).FirstOrDefault() is null ? null : _Cache.cacheSECMast().Where(w => w.SEC_NAME.Trim() == Section.Trim()).FirstOrDefault().SEC_CODE;

            if ((Section != null || Section != "") && Code == null)
                Code = Section;

            return Code;
        }

        public string TransferDepartmentToPlant(string Department)
        {
            int defaultPlant = 3000;
            string plant = "";
            string codeName = TransferDepartmentToCodeName(Department);
            try
            {
                if (codeName.IndexOf("LE") >= 0)
                {
                    plant = (int.Parse(codeName.Split("LE")[1]) + defaultPlant).ToString();
                }
                if (codeName == GlobalVariable.AdminDivision)
                {
                    plant = "300";
                }
            }
            catch
            {
                plant = "";
            }

            return plant;
        }

        public ViewAccEMPLOYEE EmployeeByPositionList(string pst, string grp, string sctn, string dept)  //get employee list higher 1 level position param(GL position) -> get (CS list)
        {
            ViewAccEMPLOYEE filterAccEmp = new ViewAccEMPLOYEE();
            List<ViewAccEMPLOYEE> Employees = new List<ViewAccEMPLOYEE>();
            switch (pst.ToUpper())
            {
                case "UL":
                    Employees = _Cache.cacheAccEmployee().Where(w => w.DEPT_CODE == TransferDepartmentToCodeName(dept)).ToList();
                    filterAccEmp = Employees
                                   .Where(w => !string.IsNullOrEmpty(w.DEPT_CODE) && !string.IsNullOrEmpty(w.PRI_THAI) && !string.IsNullOrEmpty(w.EMP_TNAME) && !string.IsNullOrEmpty(w.LAST_TNAME)
                                               && TransPositionToLevel(w.POS_CODE) == TransPositionToLevel(GlobalVariable.spUL) + 1 && w.GRP_CODE == TransferGroupToCodeName(grp))
                                   .FirstOrDefault();
                    if (filterAccEmp is null)
                        goto case "GL";
                    break;
                case "GL":
                    Employees = _Cache.cacheAccEmployee().Where(w => w.DEPT_CODE == TransferDepartmentToCodeName(dept)).ToList();
                    filterAccEmp = Employees
                                   .Where(w => !string.IsNullOrEmpty(w.DEPT_CODE) && !string.IsNullOrEmpty(w.PRI_THAI) && !string.IsNullOrEmpty(w.EMP_TNAME) && !string.IsNullOrEmpty(w.LAST_TNAME)
                                               && TransPositionToLevel(w.POS_CODE) == TransPositionToLevel(GlobalVariable.spGL) + 1 && w.SEC_CODE == TransferSectionToCodName(sctn))
                                   .FirstOrDefault();
                    if (filterAccEmp is null)
                        goto case "DDM";
                    break;
                case "CS":
                    Employees = _Cache.cacheAccEmployee().Where(w => w.DEPT_CODE == TransferDepartmentToCodeName(dept)).ToList();
                    filterAccEmp = Employees
                                   .Where(w => !string.IsNullOrEmpty(w.DEPT_CODE) && !string.IsNullOrEmpty(w.PRI_THAI) && !string.IsNullOrEmpty(w.EMP_TNAME) && !string.IsNullOrEmpty(w.LAST_TNAME)
                                               && TransPositionToLevel(w.POS_CODE) == TransPositionToLevel(GlobalVariable.spCS) + 2)
                                   .FirstOrDefault();
                    break;
                case "DDM":
                    Employees = _Cache.cacheAccEmployee().Where(w => w.DEPT_CODE == TransferDepartmentToCodeName(dept)).ToList();
                    string empcodeDDM = _Cache.cacheMastUserApprove().Where(w => w.muEmpCode == TransferSectionToCodName(sctn) && w.muCheck == GlobalVariable.statusOnline && w.muPosition == GlobalVariable.spDDM).Select(s=>s.muApprove).FirstOrDefault();
                    if (empcodeDDM != "" && empcodeDDM != null){
                        filterAccEmp = Employees
                                       .Where(w => !string.IsNullOrEmpty(w.DEPT_CODE) && !string.IsNullOrEmpty(w.PRI_THAI) && !string.IsNullOrEmpty(w.EMP_TNAME) && !string.IsNullOrEmpty(w.LAST_TNAME)
                                                  && w.EMP_CODE == empcodeDDM).FirstOrDefault();
                    }
                    else
                    {
                        filterAccEmp = Employees
                                  .Where(w => !string.IsNullOrEmpty(w.DEPT_CODE) && !string.IsNullOrEmpty(w.PRI_THAI) && !string.IsNullOrEmpty(w.EMP_TNAME) && !string.IsNullOrEmpty(w.LAST_TNAME)
                                              && TransPositionToLevel(w.POS_CODE) == TransPositionToLevel(GlobalVariable.spDDM))
                                  .FirstOrDefault();
                    }
                   
                    break;
            }

            return filterAccEmp;
        }

        [HttpPost]
        public JsonResult ModelsOfProdLine([FromBody]OTProdLine prodline)
        {
            List<string> models = new List<string>();
            models = _Cache.cacheMastModel().Where(w => w.mmProdline == prodline.Name).Select(s => s.mmModelName).OrderBy(o => o).ToList();
            return Json(models);
        }

        public OkResult ToListXlsm([FromBody]List<req> req)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                string TempPath = Path.GetTempPath();
                string fileName = "Tempfile.xlsx";
                ExcelWorksheet addWorkSheet = package.Workbook.Worksheets.Add("Sheet1");
                int headTable = 1;
                int saveOnRow = 2;

                //head table
                addWorkSheet.Cells[headTable, 1].Value = "วันที่ขอ";
                addWorkSheet.Cells[headTable, 2].Value = "รหัสพนักงาน";
                addWorkSheet.Cells[headTable, 3].Value = "วันที่ทำ OT.";
                addWorkSheet.Cells[headTable, 4].Value = "OT.ก่อนเริ่มงานเริ่มต้น";
                addWorkSheet.Cells[headTable, 5].Value = "OT.ก่อนเริ่มงานสิ้นสุด";
                addWorkSheet.Cells[headTable, 6].Value = "OT.ระหว่างเวลางานเริ่มต้น";
                addWorkSheet.Cells[headTable, 7].Value = "OT.ระหว่างเวลางานสิ้นสุด";
                addWorkSheet.Cells[headTable, 8].Value = "OT.หลังเลิกงานเริ่มต้น";
                addWorkSheet.Cells[headTable, 9].Value = "OT.หลังเลิกงานสิ้นสุด";
                addWorkSheet.Cells[headTable, 10].Value = "รหัสสาเหตุการขอ";
                addWorkSheet.Cells[headTable, 11].Value = "รหัสหน่วยงานที่ไปปฏิบัติงาน";
                addWorkSheet.Cells[headTable, 12].Value = "รหัสผู้อนุมัติ";
                addWorkSheet.Cells[headTable, 13].Value = "วันที่อนุมัติ";
                for (int headCol = 1; headCol <= 13; headCol++)
                {
                    addWorkSheet.Cells[headTable, headCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    addWorkSheet.Cells[headTable, headCol].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    addWorkSheet.Cells[headTable, headCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    addWorkSheet.Cells[headTable, headCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    addWorkSheet.Cells[headTable, headCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    addWorkSheet.Cells[headTable, headCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    addWorkSheet.Cells[headTable, headCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    addWorkSheet.Cells[headTable, headCol].Style.Border.Bottom.Color.SetColor(Color.DarkGray);
                    addWorkSheet.Cells[headTable, headCol].Style.Border.Top.Color.SetColor(Color.DarkGray);
                    addWorkSheet.Cells[headTable, headCol].Style.Border.Left.Color.SetColor(Color.DarkGray);
                    addWorkSheet.Cells[headTable, headCol].Style.Border.Right.Color.SetColor(Color.DarkGray);
                    addWorkSheet.Cells[headTable, headCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    addWorkSheet.Cells[headTable, headCol].AutoFitColumns();
                }

                foreach (req reqNo in req) {
                    ViewMastRequestOT otRequest = _Cache.cacheMastRequestOT().Where(w => w.mrNoReq == reqNo.no).FirstOrDefault();
                    List<ViewDetailRequestOT> workers = _Cache.cacheDetailRequestOT().Where(w => w.drStatus != null && w.drNoReq == reqNo.no && !w.drStatus.StartsWith(GlobalVariable.StatusRejected)).ToList();
                    List<ViewHistoryApproved> historyOtRequests = _Cache.cacheHistoryApproved().Where(w => w.htNoReq == reqNo.no).ToList();
                    ViewHistoryApproved approvedBy = new ViewHistoryApproved();

                    //DM Approved
                    if (otRequest.mrFlow == "2" && otRequest.mrStep >= 3)
                        historyOtRequests = historyOtRequests.Where(w => w.htStep == 3).OrderByDescending(o => o.htDate).ThenByDescending(o => o.htTime).ToList();
                    if (otRequest.mrFlow == "1" && otRequest.mrStep >= 4)
                        historyOtRequests = historyOtRequests.Where(w => w.htStep == 4).OrderByDescending(o => o.htDate).ThenByDescending(o => o.htTime).ToList();

                    //protect case rejected
                    if (!historyOtRequests.FirstOrDefault().htStatus.StartsWith(GlobalVariable.StatusRejected))
                    {
                        approvedBy = historyOtRequests.FirstOrDefault();

                        //string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        foreach (var item in workers)
                        {
                            addWorkSheet.Cells[saveOnRow, 1].Value = otRequest.mrDateReq;
                            addWorkSheet.Cells[saveOnRow, 2].Value = item.drEmpCode;
                            addWorkSheet.Cells[saveOnRow, 3].Value = otRequest.mrOTDate;
                            addWorkSheet.Cells[saveOnRow, 4].Value = otRequest.mrOTTimeSt_Before is null ? "" :otRequest.mrOTTimeSt_Before.Replace(":", ""); ;
                            addWorkSheet.Cells[saveOnRow, 5].Value = otRequest.mrOTTimeEd_Before is null ? "" : otRequest.mrOTTimeEd_Before.Replace(":", ""); ;
                            addWorkSheet.Cells[saveOnRow, 6].Value = otRequest.mrOTTimeSt_During is null ? "" : otRequest.mrOTTimeSt_During.Replace(":", "");
                            addWorkSheet.Cells[saveOnRow, 7].Value = otRequest.mrOTTimeEd_During is null ? "" : otRequest.mrOTTimeEd_During.Replace(":", "");
                            addWorkSheet.Cells[saveOnRow, 8].Value = otRequest.mrOTTimeSt_After is null ? "" : otRequest.mrOTTimeSt_After.Replace(":", ""); ;
                            addWorkSheet.Cells[saveOnRow, 9].Value = otRequest.mrOTTimeEd_After is null ? "" : otRequest.mrOTTimeEd_After.Replace(":", ""); ;
                            addWorkSheet.Cells[saveOnRow, 10].Value = item.drJobCode + otRequest.mrReason;
                            addWorkSheet.Cells[saveOnRow, 11].Value = "";
                            addWorkSheet.Cells[saveOnRow, 12].Value = approvedBy.htTo;
                            addWorkSheet.Cells[saveOnRow, 13].Value = approvedBy.htDate;

                            for (int dataCol = 1; dataCol <= 13; dataCol++)
                            {
                                addWorkSheet.Cells[saveOnRow, dataCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            }
                            saveOnRow++;
                        }
                    }
                }
                FileInfo fileTemp = new FileInfo(TempPath + fileName);
                package.SaveAs(fileTemp);
                byte[] fileByte = System.IO.File.ReadAllBytes(TempPath + fileName);
                return Ok();
            }
        }

        [Authorize]
        public FileResult XlsxFromByte() {
            string TempPath = Path.GetTempPath();
            string fileName = "Tempfile.xlsx";
            byte[] fileByte = System.IO.File.ReadAllBytes(TempPath + fileName);
            
            return File(fileByte, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public bool AuthorizeApprover(string position, ViewMastFlowApprove authApprove, ViewMastUserApprove authSpecial)
        {
            //ป้องกันตำแหน่ง DM และ Andmin ไม่ให้ไป Flow ที่ step ต่ำกว่าของตัวเอง
            switch (position.ToUpper())
            {
                case string A when A == GlobalVariable.spDM:
                    if (TransPositionToLevel(authApprove.mfPermission) == TransPositionToLevel(GlobalVariable.spDM))        //ตำแหน่งของรหัสพนักงานที่กรอกเข้ามา = DM 
                        return true;
                    break;
                default:
                    if (authSpecial != null)
                        if (authSpecial.muPosition.ToUpper() == authApprove.mfPermission.ToUpper())
                            return true;

                    if ((TransPositionToLevel(position) >= TransPositionToLevel(authApprove.mfPermission)) && (TransPositionToLevel(position) <= TransPositionToLevel(GlobalVariable.spDDM)))
                        return true;
                    break;
            }
            return false;
        }

        public string LoadEmpPic(string empcode)
        {
            string imgDataURL = "";
            string imgPath = GlobalVariable.imgPath + "/" + empcode + ".jpg";
            using (WebClient request = new WebClientWithTimeout())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                request.Credentials = new NetworkCredential(GlobalVariable.UFTP, GlobalVariable.PFTP);
                try
                {
                    byte[] imgFile = request.DownloadData(imgPath);
                    string file64String = Convert.ToBase64String(imgFile);
                    imgDataURL = string.Format("data:image/jpg;base64,{0}", file64String);
                }
                catch (WebException wex)
                { Console.Write(wex.ToString()); }
                //}
                return imgDataURL;
            }
        }

        public FileResult ToXlsm(string req)
        {
            req = "OT" + req.Split("OT")[1].Split("=")[0];

            ViewMastRequestOT otRequest = _Cache.cacheMastRequestOT().Where(w => w.mrNoReq == req).FirstOrDefault();
            List<ViewDetailRequestOT> workers = _Cache.cacheDetailRequestOT().Where(w => w.drStatus != null && w.drNoReq == req && !w.drStatus.StartsWith(GlobalVariable.StatusRejected)).ToList();
            List<ViewHistoryApproved> historyOtRequests = _Cache.cacheHistoryApproved().Where(w => w.htNoReq == req).ToList();
            ViewHistoryApproved approvedBy = new ViewHistoryApproved();

            //DM Approved
            if (otRequest.mrFlow == "2" && otRequest.mrStep >= 3)
                historyOtRequests = historyOtRequests.Where(w => w.htStep == 3).OrderByDescending(o=>o.htDate).ThenByDescending(o=>o.htTime).ToList();
            if (otRequest.mrFlow == "1" && otRequest.mrStep >= 4)
                historyOtRequests = historyOtRequests.Where(w => w.htStep == 4).OrderByDescending(o => o.htDate).ThenByDescending(o => o.htTime).ToList();

            //protect case rejected
            if (!historyOtRequests.FirstOrDefault().htStatus.StartsWith(GlobalVariable.StatusRejected))
            {
                approvedBy = historyOtRequests.FirstOrDefault();
                string TempPath = Path.GetTempFileName();
                string fileName = "Tempfile.xlsx";
                int headTable = 1;
                int saveOnRow = 2;
                //string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet addWorkSheet = package.Workbook.Worksheets.Add("Sheet1");

                    //head table
                    addWorkSheet.Cells[headTable, 1].Value = "วันที่ขอ";
                    addWorkSheet.Cells[headTable, 2].Value = "รหัสพนักงาน";
                    addWorkSheet.Cells[headTable, 3].Value = "วันที่ทำ OT.";
                    addWorkSheet.Cells[headTable, 4].Value = "OT.ก่อนเริ่มงานเริ่มต้น";
                    addWorkSheet.Cells[headTable, 5].Value = "OT.ก่อนเริ่มงานสิ้นสุด";
                    addWorkSheet.Cells[headTable, 6].Value = "OT.ระหว่างเวลางานเริ่มต้น";
                    addWorkSheet.Cells[headTable, 7].Value = "OT.ระหว่างเวลางานสิ้นสุด";
                    addWorkSheet.Cells[headTable, 8].Value = "OT.หลังเลิกงานเริ่มต้น";
                    addWorkSheet.Cells[headTable, 9].Value = "OT.หลังเลิกงานสิ้นสุด";
                    addWorkSheet.Cells[headTable, 10].Value = "รหัสสาเหตุการขอ";
                    addWorkSheet.Cells[headTable, 11].Value = "รหัสหน่วยงานที่ไปปฏิบัติงาน";
                    addWorkSheet.Cells[headTable, 12].Value = "รหัสผู้อนุมัติ";
                    addWorkSheet.Cells[headTable, 13].Value = "วันที่อนุมัติ";
                    for (int headCol = 1; headCol <= 13; headCol++){
                        addWorkSheet.Cells[headTable, headCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        addWorkSheet.Cells[headTable, headCol].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                        addWorkSheet.Cells[headTable, headCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        addWorkSheet.Cells[headTable, headCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        addWorkSheet.Cells[headTable, headCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        addWorkSheet.Cells[headTable, headCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        addWorkSheet.Cells[headTable, headCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        addWorkSheet.Cells[headTable, headCol].Style.Border.Bottom.Color.SetColor(Color.DarkGray);
                        addWorkSheet.Cells[headTable, headCol].Style.Border.Top.Color.SetColor(Color.DarkGray);
                        addWorkSheet.Cells[headTable, headCol].Style.Border.Left.Color.SetColor(Color.DarkGray);
                        addWorkSheet.Cells[headTable, headCol].Style.Border.Right.Color.SetColor(Color.DarkGray);
                        addWorkSheet.Cells[headTable, headCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        addWorkSheet.Cells[headTable, headCol].AutoFitColumns();
                    }

                    foreach (var item in workers)
                    {
                        addWorkSheet.Cells[saveOnRow, 1].Value = otRequest.mrDateReq;
                        addWorkSheet.Cells[saveOnRow, 2].Value = item.drEmpCode;
                        addWorkSheet.Cells[saveOnRow, 3].Value = otRequest.mrOTDate;
                        addWorkSheet.Cells[saveOnRow, 4].Value = otRequest.mrOTTimeSt_Before is null ? "" : otRequest.mrOTTimeSt_Before.Replace(":", ""); ;
                        addWorkSheet.Cells[saveOnRow, 5].Value = otRequest.mrOTTimeEd_Before is null ? "" : otRequest.mrOTTimeEd_Before.Replace(":", ""); ;
                        addWorkSheet.Cells[saveOnRow, 6].Value = otRequest.mrOTTimeSt_During is null ? "" : otRequest.mrOTTimeSt_During.Replace(":", "");
                        addWorkSheet.Cells[saveOnRow, 7].Value = otRequest.mrOTTimeEd_During is null ? "" : otRequest.mrOTTimeEd_During.Replace(":", "");
                        addWorkSheet.Cells[saveOnRow, 8].Value = otRequest.mrOTTimeSt_After is null ? "" : otRequest.mrOTTimeSt_After.Replace(":", ""); ;
                        addWorkSheet.Cells[saveOnRow, 9].Value = otRequest.mrOTTimeEd_After is null ? "" : otRequest.mrOTTimeEd_After.Replace(":", ""); ;
                        addWorkSheet.Cells[saveOnRow, 10].Value = otRequest.mrReason;
                        addWorkSheet.Cells[saveOnRow, 11].Value = "";
                        addWorkSheet.Cells[saveOnRow, 12].Value = approvedBy.htTo;
                        addWorkSheet.Cells[saveOnRow, 13].Value = approvedBy.htDate;

                        for(int dataCol = 1; dataCol <= 13; dataCol++)
                        {
                            addWorkSheet.Cells[saveOnRow, dataCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                        saveOnRow++;
                    }

                    FileInfo fileTemp = new FileInfo(TempPath + fileName);
                    package.SaveAs(fileTemp);
                    byte[] fileByte = System.IO.File.ReadAllBytes(TempPath + fileName);


                    return File(fileByte, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            else
            {
                return File("", "", "notfund");
            }
        }
    }
}