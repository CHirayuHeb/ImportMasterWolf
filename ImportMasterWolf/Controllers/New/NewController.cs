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
using ImportMasterWolf.Models.MyRequest;
using ImportMasterWolf.Models.New;
using ImportMasterWolf.Models.Table.HRMS;
using ImportMasterWolf.Models.Table.IT;
using ImportMasterWolf.Models.Table.LAMP;

namespace ImportMasterWolf.Controllers.New
{
    public class NewController : Controller
    {
        //username emppic ftpdb
        private LAMP _LAMP;
        private HRMS _HRMS;
        private IT _IT;
        private CacheSettingController _Cache;
        private FunctionsController _callFunc;
        public NewController(LAMP lamp, HRMS hrms, IT it, CacheSettingController cacheController, FunctionsController callfunction)
        {
            _LAMP = lamp;
            _HRMS = hrms;
            _IT = it;
            _Cache = cacheController;
            _callFunc = callfunction;
        }

        [Authorize(Policy = "perGeneral")]
        public IActionResult Index()
        {
            MultiDocMast modelDisplay = queryMyToday();
            return View(modelDisplay);
        }

        #region Process Action

        public JsonResult DraftDocument(ViewMastRequestOT otRequest, multiModelOTForm otDetail, multiOTEmailForm otHistory, string[] NewWorkerList, string[] MailCCs)
        {
            return DraftOTDocument(otRequest, otDetail, otHistory, NewWorkerList, MailCCs);
        }

        [HttpPost]
        public JsonResult CreateNew(ViewMastRequestOT otRequest, multiModelOTForm otDetail, multiOTEmailForm otHistory, string[] NewWorkerList, string[] MailCCs)
        {
            return CreateOTDocument(otRequest, otDetail, otHistory, NewWorkerList, MailCCs);
        }

        [HttpPost]
        public JsonResult ChangeWorker(string req, string[] NewWorkerList)
        {
            string DateReq = DateTime.Today.ToString("dd/MM/yyyy");
            _Cache.cacheAccEmployee();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //get old worker oT
                    List<ViewDetailRequestOT> oldWorkerList = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == req).ToList();
                    //set new worker OT
                    List<ViewDetailRequestOT> workerDetails = new List<ViewDetailRequestOT>();
                    foreach (string worker in NewWorkerList)
                    {
                        ViewDetailRequestOT workerDetail = JsonConvert.DeserializeObject<ViewDetailRequestOT>(worker);
                        ViewAccEMPLOYEE workerProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == workerDetail.drEmpCode).FirstOrDefault();
                        workerDetail.drNoReq = req;
                        workerDetail.drDateReq = DateReq;
                        workerDetail.drPriName = workerProfile.PRI_THAI;
                        workerDetail.drName = workerProfile.EMP_TNAME;
                        workerDetail.drLastName = workerProfile.LAST_TNAME;
                        workerDetail.drDivi = workerProfile.DIVI_CODE;
                        workerDetail.drDept = workerProfile.DEPT_CODE;
                        workerDetail.drSec = workerProfile.SEC_CODE;
                        workerDetail.drGrp = workerProfile.GRP_CODE;
                        workerDetail.drUnit = workerProfile.UNT_CODE;
                        workerDetail.drSubDirOrInDir = workerProfile.DirOrIndir;
                        workerDetails.Add(workerDetail);
                    }

                    if (workerDetails.Count == 0)
                        return Json(new { icon = "error", title = "ไม่สำเร็จ", message = "กรุณาเลือกพนักงานที่จะทำ OT" });

                    _LAMP.DetailRequestOTs.RemoveRange(oldWorkerList);
                    _LAMP.DetailRequestOTs.AddRangeAsync(workerDetails);
                    _LAMP.SaveChangesAsync();

                    _Cache.clearCacheDetailRequestOT();
                    scope.Complete();

                    return Json(new { icon = "success", title = "สำเร็จ", message = "" });
                }
                catch
                {
                    return Json(new { icon = "error", title = "ไม่สำเร็จ", message = "" });
                }
                finally
                {
                    scope.Dispose();
                }

            }
        }

        [HttpPost]
        public async Task<JsonResult> ChangeUpdate(string req, ViewMastRequestOT otRequest, multiModelOTForm otDetail, multiOTEmailForm otHistory, string[] NewWorkerList, string[] MailCCs)
        {
            return await UpdateOTDocument(req, otRequest, otDetail, otHistory, NewWorkerList, MailCCs);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteReq(string req)
        {
            req = "OT" + req.Split("OT")[1].Split("=")[0];
            _Cache.cacheMastRequestOT();
            _Cache.cacheDetailRequestOT();
            _Cache.cacheHistoryApproved();

            ViewMastRequestOT requestOT = await Task.Run(() => _Cache.cacheMastRequestOT().Where(w => w.mrNoReq == req.Trim()).FirstOrDefault());
            List<ViewDetailRequestOT> CategoryWorkerList = await Task.Run(() => _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == req.Trim()).ToList());
            List<ViewHistoryApproved> ViewHistoryApproved = await Task.Run(() => _Cache.cacheHistoryApproved().Where(w => w.htNoReq == req.Trim()).ToList());
            bool hasExistingTransaction = Transaction.Current != null;

            ViewMastRequestOT mastRequestOT = await Task.Run(() => _Cache.cacheMastRequestOT().Where(w => w.mrNoReq == req && w.mrStep < 1).FirstOrDefault());
            if (mastRequestOT is null)
                return Json(new { icon = "info", title = "ไม่สำเร็จ", message = "คำร้องนี้ได้ผ่านการพิจารณาจากหัวหน้าไปแล้ว หรือได้ถูกลบไปแล้ว" });

            using (var scope = hasExistingTransaction
        ? new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled)
        : new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (requestOT is null && CategoryWorkerList.Count == 0 && ViewHistoryApproved.Count == 0)
                        return Json(new { icon = "warning", title = "เตือน", message = "ข้อมูลนี้ถูกลบไปแล้ว" });
                    if (requestOT != null)
                        await Task.Run(() => _LAMP.MastRequestOTs.Remove(requestOT));
                    if (CategoryWorkerList.Count > 0)
                        await Task.Run(() => _LAMP.DetailRequestOTs.RemoveRange(CategoryWorkerList));
                    if (ViewHistoryApproved.Count > 0)
                        await Task.Run(() => _LAMP.HistoryApproveds.RemoveRange(ViewHistoryApproved));

                    await _LAMP.SaveChangesAsync();

                    _Cache.clearCacheMastRequestOT();
                    _Cache.clearCacheDetailRequestOT();
                    _Cache.clearCacheHistoryApproved();
                    scope.Complete();

                    return Json(new { icon = "success", title = "สำเร็จ", message = "ยกเลิกคำร้องขอเรียบร้อยแล้ว" });
                }
                catch (Exception ex)
                {
                    return Json(new { icon = "error", title = "ไม่สำเร็จ", message = ex.Message });
                }
                finally
                {
                    scope.Dispose();
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteWorker(string req, string empcode)
        {
            req = "OT" + req.Split("OT")[1].Split("=")[0];
            _Cache.cacheDetailRequestOT();

            ViewDetailRequestOT workerTarget = await Task.Run(() => _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == req.Trim() && w.drEmpCode == empcode.Trim()).FirstOrDefault());

            bool hasExistingTransaction = Transaction.Current != null;

            using (var scope = hasExistingTransaction
        ? new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled)
        : new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (workerTarget != null)
                        await Task.Run(() => _LAMP.DetailRequestOTs.Remove(workerTarget));

                    await _LAMP.SaveChangesAsync();

                    _Cache.clearCacheDetailRequestOT();
                    scope.Complete();

                    return Json(new { icon = "success", title = "สำเร็จ", message = "" });
                }
                catch (Exception ex)
                {
                    return Json(new { icon = "error", title = "ไม่สำเร็จ", message = ex.Message });
                }
                finally
                {
                    scope.Dispose();
                }
            }
        }
        #endregion

        #region Partial
        public IActionResult OTType()
        {
            //return PartialView("_OTType");
            return PartialView("~/Views/New/_OTType.cshtml");
        }
        public IActionResult OTMyData()
        {
            List<ViewMastRequestOT> RequestOTList = _Cache.cacheMastRequestOT();
            string ToDayReq = DateTime.Today.ToString("dd/MM/yyyy");
            string EmpCodeReq = User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value?.ToString();
            int DocNoReq = 0;
            if (RequestOTList.Where(w => w.mrEmpReq == EmpCodeReq && w.mrDateReq == ToDayReq).ToList().Count > 0)
            {
                string getRefNo = RequestOTList.Where(w => w.mrEmpReq == EmpCodeReq && w.mrDateReq == ToDayReq)
                                        .OrderByDescending(o => o.mrNoReq)
                                        .FirstOrDefault().mrNoReq;
                DocNoReq = int.Parse(getRefNo.Substring(getRefNo.Length - 2));
            }
            ViewMastRequestOT requestOT = new ViewMastRequestOT();
            requestOT.mrDateReq = ToDayReq;
            requestOT.mrNoReq = (++DocNoReq).ToString();
            requestOT.mrPriNameReq = User.Claims.FirstOrDefault(s => s.Type == "PriName")?.Value;
            requestOT.mrEmpReq = User.Claims.FirstOrDefault(s => s.Type == "EmpCode")?.Value;
            requestOT.mrNameReq = User.Claims.FirstOrDefault(s => s.Type == "Name")?.Value;
            requestOT.mrLastNameReq = User.Claims.FirstOrDefault(s => s.Type == "SurName")?.Value;
            requestOT.mrDiviReq = User.Claims.FirstOrDefault(s => s.Type == "Division")?.Value;
            requestOT.mrDeptReq = User.Claims.FirstOrDefault(s => s.Type == "Department")?.Value;
            requestOT.mrSecReq = User.Claims.FirstOrDefault(s => s.Type == "Section")?.Value;
            requestOT.mrGrpReq = User.Claims.FirstOrDefault(s => s.Type == "Group")?.Value;
            requestOT.mrUnitReq = User.Claims.FirstOrDefault(s => s.Type == "Unit")?.Value;
            requestOT.mrPositionReq = User.Claims.FirstOrDefault(s => s.Type == "Position")?.Value;
            return PartialView("_OTMyData", requestOT);
        }
        public IActionResult OTForm()
        {
            string reqDepartment = User.Claims.FirstOrDefault(s => s.Type == "Department").Value?.ToString();
            multiModelOTForm modelOTForm = new multiModelOTForm();
            modelOTForm.mastRequestOT = new ViewMastRequestOT();


            //modelOTForm.timeStart = new List<OTTimeStart>();
            //foreach (var row in _Cache.cacheMastOTTime().Where(w => w.mtType == GlobalVariable.OtTimeSt).OrderBy(o => o.mtTime).ToList())
            //{
            //    modelOTForm.timeStart.Add(
            //        new OTTimeStart()
            //        {
            //            Time = row.mtTime
            //        });
            //};
            //modelOTForm.timeEnd = new List<OTTimeEnd>();
            //foreach (var row in _Cache.cacheMastOTTime().Where(w => w.mtType == GlobalVariable.OtTimeEd).OrderBy(o => o.mtTime).ToList())
            //{
            //    modelOTForm.timeEnd.Add(
            //        new OTTimeEnd()
            //        {
            //            Time = row.mtTime
            //        });
            //};
            modelOTForm.prodLines = new List<OTProdLine>();
            foreach (var row in _Cache.cacheMastProdLine().Where(w => w.plant.StartsWith(_callFunc.TransferDepartmentToPlant(reqDepartment))).ToList())
            {
                modelOTForm.prodLines.Add(
                    new OTProdLine()
                    {
                        Name = row.prodline
                    });
            }

            modelOTForm.models = new List<OTModel>();
            string qryModelOfProdLine = new string("");
            if (modelOTForm.prodLines.FirstOrDefault() != null)
                qryModelOfProdLine = modelOTForm.prodLines.FirstOrDefault().Name;
            foreach (var row in _Cache.cacheMastModel().Where(w => w.mmProdline == qryModelOfProdLine).ToList())
            {
                modelOTForm.models.Add(
                    new OTModel()
                    {
                        Name = row.mmModelName
                    });
            }

            modelOTForm.reasons = new List<OTReason>();
            foreach (var row in _Cache.cacheMastOTReason().ToList())
            {
                modelOTForm.reasons.Add(new OTReason()
                {
                    Code = row.mrCode,
                    Caption = row.mrReasonTH
                });
            }

            modelOTForm.mastRequestOT.mrPositionReq = User.Claims.FirstOrDefault(s => s.Type == "Position").Value?.ToString();

            return PartialView("_OTForm", modelOTForm);
        }
        public IActionResult OTAddWorker()
        {
            return PartialView("_OTAddWorker");
        }
        public IActionResult OTEmailForm()
        {
            string grpName = User.Claims.FirstOrDefault(s => s.Type == "Group").Value?.ToString();
            string secName = User.Claims.FirstOrDefault(s => s.Type == "Section").Value?.ToString();
            string deptName = User.Claims.FirstOrDefault(s => s.Type == "Department").Value?.ToString();
            string pstCode = User.Claims.FirstOrDefault(s => s.Type == "Position").Value?.ToString();
            string reqDepartment = User.Claims.FirstOrDefault(s => s.Type == "Department").Value?.ToString();
            multiOTEmailForm modelOTEmailForm = new multiOTEmailForm();
            modelOTEmailForm.EFMastRequestOT = new ViewMastRequestOT();
            modelOTEmailForm.historyApproveds = new ViewHistoryApproved();
            modelOTEmailForm.EFMastRequestOT.mrPositionReq = User.Claims.FirstOrDefault(s => s.Type == "Position").Value?.ToString();


            ViewAccEMPLOYEE filterAccEmp = _callFunc.EmployeeByPositionList(pstCode, grpName, secName, deptName);
            if (filterAccEmp != null)
            {
                ViewAccEMPLOYEE profileMailTo = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == filterAccEmp.EMP_CODE).FirstOrDefault();
                modelOTEmailForm.historyApproveds.htTo = filterAccEmp.EMP_CODE;
                modelOTEmailForm.FullNameMailTo = profileMailTo is null ? "" : "คุณ" + profileMailTo.EMP_TNAME + " " + profileMailTo.LAST_TNAME;
            }


            return PartialView("_OTEmailForm", modelOTEmailForm);
        }

        [HttpPost]
        public async Task<IActionResult> DraftForm(string req)
        {
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

            //change format date dd/MM/yyyy to yyyy-MM-dd for input type=date
            string[] dmyOTDate = editModel.multiModelOTForm.mastRequestOT.mrOTDate.Split("/");
            editModel.multiModelOTForm.mastRequestOT.mrOTDate = dmyOTDate.Length > 1 ? dmyOTDate[2] + "-" + dmyOTDate[1] + "-" + dmyOTDate[0] : "";

            //OTForm
            //set time start and end
            //editModel.multiModelOTForm.timeStart = new List<OTTimeStart>();
            //foreach (var row in _Cache.cacheMastOTTime().Where(w => w.mtType == GlobalVariable.OtTimeSt).OrderBy(o => o.mtTime).ToList())
            //{
            //    editModel.multiModelOTForm.timeStart.Add(
            //        new OTTimeStart()
            //        {
            //            Time = row.mtTime
            //        });
            //}

            //editModel.multiModelOTForm.timeEnd = new List<OTTimeEnd>();
            //foreach (var row in _Cache.cacheMastOTTime().Where(w => w.mtType == GlobalVariable.OtTimeEd).OrderBy(o => o.mtTime).ToList())
            //{
            //    editModel.multiModelOTForm.timeEnd.Add(
            //        new OTTimeEnd()
            //        {
            //            Time = row.mtTime
            //        });
            //};

            //set prod line list
            editModel.multiModelOTForm.prodLines = new List<OTProdLine>();
            foreach (var row in _Cache.cacheMastProdLine().Where(w => w.plant.StartsWith(_callFunc.TransferDepartmentToPlant(reqDepartment))).ToList())
            {
                editModel.multiModelOTForm.prodLines.Add(
                    new OTProdLine()
                    {
                        Name = row.prodline
                    });
            }

            //set model list
            editModel.multiModelOTForm.models = new List<OTModel>();
            string qryModelOfProdLine = new string("");
            if (_Cache.cacheMastModel().Where(w => w.mmModelName == requestOT.mrModel).Select(s => s.mmProdline).FirstOrDefault() != null)
                qryModelOfProdLine = _Cache.cacheMastModel().Where(w => w.mmModelName == requestOT.mrModel).Select(s => s.mmProdline).FirstOrDefault();
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
            string grpName = User.Claims.FirstOrDefault(s => s.Type == "Group").Value?.ToString();
            string secName = User.Claims.FirstOrDefault(s => s.Type == "Section").Value?.ToString();
            string deptName = User.Claims.FirstOrDefault(s => s.Type == "Department").Value?.ToString();
            string pstCode = User.Claims.FirstOrDefault(s => s.Type == "Position").Value?.ToString();

            editModel.multiOTEmailForm.EFMastRequestOT.mrPositionReq = pstCode;
            editModel.multiOTEmailForm.positionNext = pstCode;
            editModel.multiOTEmailForm.historyApproveds = new ViewHistoryApproved();
            int? errorStep = requestOT.mrStep == 0 ? 1 : requestOT.mrStep;
            
            //email next level position
            editModel.multiOTEmailForm.historyApproveds = new ViewHistoryApproved();
            ViewAccEMPLOYEE filterAccEmp = _callFunc.EmployeeByPositionList(pstCode, grpName, secName, deptName);
            if (filterAccEmp != null)
            {
                ViewAccEMPLOYEE profileMailTo = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == filterAccEmp.EMP_CODE).FirstOrDefault();
                editModel.multiOTEmailForm.historyApproveds.htTo = filterAccEmp.EMP_CODE;
                editModel.multiOTEmailForm.FullNameMailTo = profileMailTo is null ? "" : "คุณ" + profileMailTo.EMP_TNAME + " " + profileMailTo.LAST_TNAME;
            }
            return await Task.Run(() => PartialView("Draft//_DraftForm", editModel));
        }

        [Authorize(Policy ="perAdmin")]
        [HttpPost]
        public async Task<IActionResult> EditForm(string req)
        {
            if (User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.Role).Value.ToUpper().Trim() != GlobalVariable.perAdmin.ToUpper())
                return RedirectToAction("ErrorCase");
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

            //change format date dd/MM/yyyy to yyyy-MM-dd for input type=date
            string[] dmyOTDate = editModel.multiModelOTForm.mastRequestOT.mrOTDate.Split("/");
            editModel.multiModelOTForm.mastRequestOT.mrOTDate = dmyOTDate.Length > 1 ? dmyOTDate[2] + "-" + dmyOTDate[1] + "-" + dmyOTDate[0] : "";

            //OTForm
            //set time start and end
            //editModel.multiModelOTForm.timeStart = new List<OTTimeStart>();
            //foreach (var row in _Cache.cacheMastOTTime().Where(w => w.mtType == GlobalVariable.OtTimeSt).OrderBy(o => o.mtTime).ToList())
            //{
            //    editModel.multiModelOTForm.timeStart.Add(
            //        new OTTimeStart()
            //        {
            //            Time = row.mtTime
            //        });
            //}

            //editModel.multiModelOTForm.timeEnd = new List<OTTimeEnd>();
            //foreach (var row in _Cache.cacheMastOTTime().Where(w => w.mtType == GlobalVariable.OtTimeEd).OrderBy(o => o.mtTime).ToList())
            //{
            //    editModel.multiModelOTForm.timeEnd.Add(
            //        new OTTimeEnd()
            //        {
            //            Time = row.mtTime
            //        });
            //};

            //set prod line list
            editModel.multiModelOTForm.prodLines = new List<OTProdLine>();
            foreach (var row in _Cache.cacheMastProdLine().Where(w => w.plant.StartsWith(_callFunc.TransferDepartmentToPlant(reqDepartment))).ToList())
            {
                editModel.multiModelOTForm.prodLines.Add(
                    new OTProdLine()
                    {
                        Name = row.prodline
                    });
            }

            //set model list
            editModel.multiModelOTForm.models = new List<OTModel>();
            string qryModelOfProdLine = new string("");
            if (_Cache.cacheMastModel().Where(w => w.mmModelName == requestOT.mrModel).Select(s => s.mmProdline).FirstOrDefault() != null)
                qryModelOfProdLine = _Cache.cacheMastModel().Where(w => w.mmModelName == requestOT.mrModel).Select(s => s.mmProdline).FirstOrDefault();
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
            string posApprover = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == requestOT.mrEmpApp).Select(s => s.POS_CODE).FirstOrDefault();
            string perAdmin = _Cache.cacheMastUserApprove().Where(w => w.muEmpCode == requestOT.mrEmpApp).Select(s=>s.muPosition).FirstOrDefault();
            if (perAdmin != null){
                perAdmin = perAdmin.ToUpper();
                if (perAdmin == GlobalVariable.perAdmin.ToUpper())
                    posApprover = perAdmin;
            }
                
            editModel.multiOTEmailForm.positionNext = _callFunc.TransLevelToPosition(_callFunc.TransPositionToLevel(posApprover) - 1);
            editModel.multiOTEmailForm.historyApproveds = new ViewHistoryApproved();
            int? errorStep = requestOT.mrStep == 0 ? 1 : requestOT.mrStep;
            editModel.multiOTEmailForm.historyApproveds = _Cache.cacheHistoryApproved().Where(w => w.htNoReq == req && w.htStep == errorStep).FirstOrDefault();

            return await Task.Run(() => PartialView("Edit//_EditForm", editModel));
        }

        public IActionResult PushWorker(string p, string req)
        {
            _Cache.clearCacheMastJob();
            string DateReq = DateTime.Today.ToString("dd/MM/yyyy");
            string TimeReq = DateTime.Now.ToString("HH:mm");
            multiModelCateWorker cateWorker = new multiModelCateWorker();
            cateWorker.CategoryWorkerList = new CategoryWorkerList();
            cateWorker.Jobs = new List<ViewMastJob>();
            foreach (var items in _Cache.cacheMastJob().ToList())
            {
                cateWorker.Jobs.Add(
                    new ViewMastJob()
                    {
                        mjJobCode = items.mjJobCode,
                        mjJobName = items.mjJobName,
                        mjGroupCode = items.mjGroupCode,
                    });
            }

            ViewAccEMPLOYEE workerProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == p).FirstOrDefault();
            if (workerProfile != null)
            {
                cateWorker.CategoryWorkerList.PriName = workerProfile.PRI_THAI;
                cateWorker.CategoryWorkerList.EmpCode = workerProfile.EMP_CODE;
                cateWorker.CategoryWorkerList.Name = workerProfile.EMP_TNAME;
                cateWorker.CategoryWorkerList.Surname = workerProfile.LAST_TNAME;
                cateWorker.CategoryWorkerList.GRP_Code = workerProfile.GRP_CODE;

                try
                {
                    WebClient request = new WebClient();
                    string imgPath = GlobalVariable.imgPath + "/" + workerProfile.EMP_CODE + ".jpg";
                    request.Credentials = new NetworkCredential(GlobalVariable.UFTP, GlobalVariable.PFTP);
                    byte[] imgFile = request.DownloadData(imgPath);
                    string file64String = Convert.ToBase64String(imgFile);
                    string imgDataURL = string.Format("data:image/jpg;base64,{0}", file64String);
                    cateWorker.image = imgDataURL;


                    //add employee to db 
                    ViewDetailRequestOT workerDetail = new ViewDetailRequestOT();
                    string JobCode = _Cache.cacheMastJob().Where(w => w.mjGroupCode == workerProfile.GRP_CODE).FirstOrDefault() is null 
                        ? _Cache.cacheMastJob().Select(s=>s.mjJobCode).Distinct().OrderBy(o=>o).FirstOrDefault()
                        : _Cache.cacheMastJob().Where(w => w.mjGroupCode == workerProfile.GRP_CODE).FirstOrDefault().mjJobCode;
                    workerDetail.drNoReq = req;
                    workerDetail.drEmpCode = workerProfile.EMP_CODE;
                    workerDetail.drJobCode = JobCode;
                    workerDetail.drDateReq = DateReq;
                    workerDetail.drPriName = workerProfile.PRI_THAI;
                    workerDetail.drName = workerProfile.EMP_TNAME;
                    workerDetail.drLastName = workerProfile.LAST_TNAME;
                    workerDetail.drDivi = workerProfile.DIVI_CODE;
                    workerDetail.drDept = workerProfile.DEPT_CODE;
                    workerDetail.drSec = workerProfile.SEC_CODE;
                    workerDetail.drGrp = workerProfile.GRP_CODE;
                    workerDetail.drUnit = workerProfile.UNT_CODE;
                    workerDetail.drSubDirOrInDir = workerProfile.DirOrIndir;
                    _LAMP.DetailRequestOTs.Add(workerDetail);
                    _LAMP.SaveChanges();

                    _Cache.clearCacheDetailRequestOT();
                }
                catch (WebException wex)
                {
                    Console.WriteLine(wex.ToString());
                }

            }
            return PartialView("\\OTAddWorkerResult\\_CateWorker", cateWorker);
        }

        public IActionResult WorkerList(string req)
        {
            Models.Approval.MultiDocDetails multiDoc = new Models.Approval.MultiDocDetails();
            multiDoc.requestOT = new ViewMastRequestOT();
            multiDoc.workerImages = new List<workerImages>();
            multiDoc.requestOT.mrNoReq = req;
            multiDoc.workerList = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == req).ToList();

            foreach (var workerProfile in _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == req).ToList()) {
                workerImages modelImage = new workerImages();
                //WebClient request = new WebClient();
                //string imgPath = GlobalVariable.imgPath + "/" + workerProfile.drEmpCode + ".jpg";
                //request.Credentials = new NetworkCredential(GlobalVariable.UFTP, GlobalVariable.PFTP);
                //byte[] imgFile = request.DownloadData(imgPath);
                //string file64String = Convert.ToBase64String(imgFile);
                //string imgDataURL = string.Format("data:image/jpg;base64,{0}", file64String);
                modelImage.empcode = workerProfile.drEmpCode;
                //modelImage.image = imgDataURL;
                multiDoc.workerImages.Add(modelImage);
            };
            

            return PartialView("~/Views/Approval/Category/_CateWorker.cshtml", multiDoc);
        }

        public IActionResult DisplayMyToday()
        {
            MultiDocMast modelDisplay = queryMyToday();
            return PartialView("_DisplayMyToday", modelDisplay);
        }
        public IActionResult DisplaMyYesterday()
        {
            MultiDocMast modelDisplay = queryMyYesterday();
            return PartialView("_DisplayMyYesterday", modelDisplay);
        }
        public IActionResult DisplayAllToday()
        {
            MultiDocMast modelDisplay = queryAllToday();
            return PartialView("_DisplayAllToday", modelDisplay);
        }
        public IActionResult DisplayAllYesterday()
        {
            MultiDocMast modelDisplay = queryAllYesterday();
            return PartialView("_DisplayAllYesterday", modelDisplay);
        }
        #endregion

        #region suggest & caption
        public JsonResult suggest(string q)
        {
            List<autocompleteEmpCode> WorkerList = new List<autocompleteEmpCode>();
            foreach (var row in _Cache.cacheAccEmployee().Where(w => w.EMP_CODE.StartsWith(q) && w.EMP_CODE != q).Take(12))
            {
                WorkerList.Add(new autocompleteEmpCode()
                {
                    EmpCode = row.EMP_CODE,
                    FullNameAndDept = row.PRI_THAI + " " +
                                      row.EMP_TNAME + " " +
                                      row.LAST_TNAME + " (" + row.DEPT_CODE + ")",

                });

            }

            return Json(WorkerList);
        }

        public JsonResult suggestMails(string q, string pst)
        {
            string department = User.Claims.FirstOrDefault(s => s.Type == "Department").Value;
            List<autocompleteEmail> emails = new List<autocompleteEmail>();
            string fullname = "";
            List<ViewAccEMPLOYEE> filterAccEmp = _callFunc.TransPositionToLevel(pst) == 0
                                                ? _Cache.cacheAccEmployee()
                                                  .Where(w => !string.IsNullOrEmpty(w.DEPT_CODE) && !string.IsNullOrEmpty(w.PRI_THAI) && !string.IsNullOrEmpty(w.EMP_TNAME) && !string.IsNullOrEmpty(w.LAST_TNAME) && w.DEPT_CODE == _callFunc.TransferDepartmentToCodeName(department))
                                                  .ToList()
                                                : _Cache.cacheAccEmployee()
                                                  .Where(w => !string.IsNullOrEmpty(w.DEPT_CODE) && !string.IsNullOrEmpty(w.PRI_THAI) && !string.IsNullOrEmpty(w.EMP_TNAME) && !string.IsNullOrEmpty(w.LAST_TNAME)
                                                                && _callFunc.TransPositionToLevel(w.POS_CODE) >= _callFunc.TransPositionToLevel(pst) + 1 && w.DEPT_CODE == _callFunc.TransferDepartmentToCodeName(department))
                                                  .ToList();

            if (pst == GlobalVariable.StepTitleAdmin.ToUpper())
                filterAccEmp = _Cache.cacheAccEmployee().Where(w => !string.IsNullOrEmpty(w.DEPT_CODE) && !string.IsNullOrEmpty(w.PRI_THAI) && !string.IsNullOrEmpty(w.EMP_TNAME) && !string.IsNullOrEmpty(w.LAST_TNAME))
                                                        .Join(_Cache.cacheMastUserApprove().Where(w => w.muPosition == GlobalVariable.StepTitleAdmin), s => s.EMP_CODE, u => u.muEmpCode, (s, u) => s).ToList();

            if (pst == GlobalVariable.StepTitleHCM.ToUpper())
                filterAccEmp = _Cache.cacheAccEmployee().Where(w => !string.IsNullOrEmpty(w.DEPT_CODE) && !string.IsNullOrEmpty(w.PRI_THAI) && !string.IsNullOrEmpty(w.EMP_TNAME) && !string.IsNullOrEmpty(w.LAST_TNAME))
                                                        .Join(_Cache.cacheMastUserApprove().Where(w => w.muPosition == GlobalVariable.StepTitleHCM), s => s.EMP_CODE, u => u.muEmpCode, (s, u) => s).ToList();

            foreach (var row in filterAccEmp.Where(w => (w.EMP_CODE.StartsWith(q.ToLower()) || w.EMP_ENAME.Trim().ToLower().StartsWith(q.ToLower()) || w.LAST_ENAME.Trim().ToLower().StartsWith(q.ToLower())) && w.EMP_CODE.Trim() != q.ToLower()).Take(5).ToList())
            {
                emails.Add(new autocompleteEmail()
                {
                    EmpCode = row.EMP_CODE,
                    Mail = _Cache.cacheEmail().Where(w => w.emEmail_M365 != null && w.emEmpcode == row.EMP_CODE.Trim()).FirstOrDefault() is null ? row.EMP_CODE : _Cache.cacheEmail().Where(w => w.emEmpcode == row.EMP_CODE.Trim()).FirstOrDefault().emEmail_M365,
                    FullNameAndDept = " คุณ " +
                                      row.EMP_TNAME + " " +
                                      row.LAST_TNAME + " (" +
                                      row.DEPT_CODE + ")",
                });
            }

            ViewAccEMPLOYEE filterName = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == q).FirstOrDefault();
            if (filterName != null)
                fullname = filterName.EMP_TNAME + " " + filterName.LAST_TNAME;
            //foreach (var item in _Cache.cacheEmail()
            //                   .Where(w => (w.emEmpcode.StartsWith(q) || (w.emEmail.Trim().StartsWith(q) && w.emEmail.Trim() != q))
            //                                && !(filterAccEmp.Where(s => s.EMP_CODE.Trim() == w.emEmpcode.Trim()).FirstOrDefault() is null))
            //                   .Take(12).ToList())
            //{
            //    emails.Add(new autocompleteEmail()
            //    {
            //        EmpCode = item.emEmpcode,
            //        Mail = item.emEmail,
            //        FullNameAndDept = filterAccEmp.Where(w => w.EMP_CODE.Trim() == item.emEmpcode).FirstOrDefault().PRI_THAI + " " +
            //                          filterAccEmp.Where(w => w.EMP_CODE.Trim() == item.emEmpcode).FirstOrDefault().EMP_TNAME + " " +
            //                          filterAccEmp.Where(w => w.EMP_CODE.Trim() == item.emEmpcode).FirstOrDefault().LAST_TNAME + " (" +
            //                          filterAccEmp.Where(w => w.EMP_CODE.Trim() == item.emEmpcode).FirstOrDefault().DEPT_CODE + ")",
            //    });

            //}


            return Json(new { emails, fullname });
        }

        public JsonResult suggestCCMails(string q)
        {
            List<autocompleteEmail> emails = new List<autocompleteEmail>();
            List<ViewAccEMPLOYEE> filterAccEmp = _Cache.cacheAccEmployee()
                                                  .Where(w => !string.IsNullOrEmpty(w.DEPT_CODE) && !string.IsNullOrEmpty(w.PRI_THAI) && !string.IsNullOrEmpty(w.EMP_TNAME) && !string.IsNullOrEmpty(w.LAST_TNAME) && (w.EMP_ENAME.ToLower().StartsWith(q) || w.EMP_CODE.StartsWith(q)) )
                                                  .ToList();

            //foreach (var row in filterAccEmp.Where(w=>w.EMP_CODE.StartsWith(q) || w.EMP_ENAME.StartsWith(q) || w.LAST_ENAME.StartsWith(q) && (w.EMP_CODE != q )).Take(10).ToList()) {
            //    emails.Add(new autocompleteEmail() {
            //        EmpCode = row.EMP_CODE,
            //        Mail = _Cache.cacheEmail().Where(w=>w.emEmpcode == row.EMP_CODE.Trim()).FirstOrDefault() is null ? row.EMP_CODE : _Cache.cacheEmail().Where(w => w.emEmpcode == row.EMP_CODE.Trim()).FirstOrDefault().emEmail,
            //        FullNameAndDept = row.PRI_THAI + " " +
            //                          row.EMP_TNAME + " " +
            //                          row.LAST_TNAME + " (" +
            //                          row.DEPT_CODE + ")",
            //    });
            //}

            foreach (var row in _Cache.cacheEmail()
                                .Where(w => w.emEmail_M365 != null && w.emEmail_M365.ToLower() != q.ToLower()
                                && !(filterAccEmp.Where(s => s.EMP_CODE.Trim() == w.emEmpcode.Trim()).FirstOrDefault() is null)).Take(12).ToList())
            {
                emails.Add(new autocompleteEmail()
                {
                    EmpCode = row.emEmpcode,
                    Mail = row.emEmail_M365,
                    FullNameAndDept = filterAccEmp.Where(w => w.EMP_CODE.Trim() == row.emEmpcode).FirstOrDefault().PRI_THAI + " " +
                                      filterAccEmp.Where(w => w.EMP_CODE.Trim() == row.emEmpcode).FirstOrDefault().EMP_TNAME + " " +
                                      filterAccEmp.Where(w => w.EMP_CODE.Trim() == row.emEmpcode).FirstOrDefault().LAST_TNAME + " (" +
                                      filterAccEmp.Where(w => w.EMP_CODE.Trim() == row.emEmpcode).FirstOrDefault().DEPT_CODE + ")",
                });

            }

            return Json(emails);
        }

        public JsonResult captionReason(string q)
        {
            string caption = _Cache.cacheMastOTReason().Where(w => w.mrCode == q).FirstOrDefault().mrReasonTH;
            return Json(caption);
        }

        #endregion

        #region Funtions

        public OkResult UpdateWorkerJob(string empcode, string req, string jobselected) {

            if(req != null)
                req = "OT" + req.Split("OT")[1].Split("=")[0];
            if(empcode != null)
                empcode = empcode.Trim();
            if (jobselected != null)
                jobselected = jobselected.Trim();

            _Cache.cacheDetailRequestOT();
            ViewDetailRequestOT requestOT = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == req.Trim() && w.drEmpCode == empcode).FirstOrDefault();
            requestOT.drJobCode = jobselected;
            _LAMP.SaveChanges();
            _Cache.cacheDetailRequestOT();
            return Ok();
        }

        public JsonResult DraftOTDocument(ViewMastRequestOT otRequest, multiModelOTForm otDetail, multiOTEmailForm otHistory, string[] NewWorkerList, string[] MailCCs)
        {
            _Cache.clearCacheAccEmployee();
            bool perAdmin = User.Claims.FirstOrDefault(s => s.Type == "Permission").Value?.ToString().ToUpper() == GlobalVariable.AdminPermission.ToUpper()
                            ? true : false;

            string flowType = "";
            if (otRequest.mrNoReq != null)
                flowType = otRequest.mrNoReq.Length > 3 ? _Cache.cacheMastRequestOT().Where(w=>w.mrNoReq == otRequest.mrNoReq).Select(s=>s.mrFlow).FirstOrDefault() :_callFunc.TransPositionToFlowType(User.Claims.FirstOrDefault(s => s.Type == "Position").Value?.ToString().ToUpper());

            string strMail = "";
            if (MailCCs != null)
                foreach (string item in MailCCs)
                {
                    if(item != null)
                        if (item.Trim() != "")
                            strMail += item.Trim() + ",";
                }
            try
            {
                if (otRequest.mrNoReq != null)
                {
                    //set date format
                    string RefNo = otRequest.mrNoReq.Length > 3 ? otRequest.mrNoReq : "OT" + DateTime.Today.ToString("yyMMdd") + otRequest.mrEmpReq + otRequest.mrNoReq.ToString().PadLeft(2, '0');
                    string DateReq = DateTime.Today.ToString("dd/MM/yyyy");
                    string TimeReq = DateTime.Now.ToString("HH:mm");
                    string OTDate = otDetail.mastRequestOT is null ? "" : DateTime.Parse(otDetail.mastRequestOT.mrOTDate).ToString("dd/MM/yyyy");

                    //check draft
                    ViewMastRequestOT draftOTRequest = _Cache.cacheMastRequestOT().Where(w => w.mrNoReq == RefNo).FirstOrDefault();
                    ViewMastRequestOT draftCheck = draftOTRequest;
                    //qry profile approver
                    //find profile from mail
                    string empcodeApprover = otHistory.historyApproveds is null ? "" : otHistory.historyApproveds.htTo is null ? "" : otHistory.historyApproveds.htTo.Trim();
                    ViewAccEMPLOYEE profileApprover = empcodeApprover == "" ? null : _Cache.cacheAccEmployee().Where(w => w.EMP_CODE.Trim() == empcodeApprover).FirstOrDefault();

                    //qry flow from flowType
                    ViewMastFlowApprove mastFlowApprove = _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == flowType && w.mfStep == 1).FirstOrDefault();
                    if (mastFlowApprove is null)
                        return Json(new { icon = "warning", title = "Permission", message = "ตำแหน่งของคุณสูงหรือต่ำกว่าที่ระบบต้องการ" });
                    //doc OT
                    //req profile
                    if (draftOTRequest is null)
                        draftOTRequest = otRequest;
                    ViewAccEMPLOYEE reqProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == draftOTRequest.mrEmpReq).FirstOrDefault();
                    draftOTRequest.mrDiviReq = perAdmin is false ? reqProfile.DIVI_CODE : GlobalVariable.AdminDivision;
                    draftOTRequest.mrDeptReq = perAdmin is false ? reqProfile.DEPT_CODE : GlobalVariable.AdminDepartment;
                    draftOTRequest.mrSecReq = perAdmin is false ? reqProfile.SEC_CODE : GlobalVariable.AdminSection;
                    draftOTRequest.mrGrpReq = perAdmin is false ? reqProfile.GRP_CODE : GlobalVariable.AdminGroup;
                    draftOTRequest.mrUnitReq = perAdmin is false ? reqProfile.UNT_CODE : GlobalVariable.AdminUnit;
                    draftOTRequest.mrPositionReq = perAdmin is false ? reqProfile.POS_CODE : GlobalVariable.AdminPosition;

                    draftOTRequest.mrNoReq = RefNo;
                    draftOTRequest.mrDateReq = draftCheck is null ? DateReq : draftCheck.mrStep > 0 ? draftOTRequest.mrDateReq : DateReq;
                    draftOTRequest.mrOTDate = OTDate;
                    //draftOTRequest.mrOTTimeSt = otDetail.mastRequestOT is null ? "" : otDetail.mastRequestOT.mrOTTimeSt;
                    //draftOTRequest.mrOTTimeEd = otDetail.mastRequestOT is null ? "" : otDetail.mastRequestOT.mrOTTimeEd;
                    draftOTRequest.mrOTTimeSt_Before = otDetail.mastRequestOT is null ? "" : otDetail.mastRequestOT.mrOTTimeSt_Before;
                    draftOTRequest.mrOTTimeEd_Before = otDetail.mastRequestOT is null ? "" : otDetail.mastRequestOT.mrOTTimeEd_Before;
                    draftOTRequest.mrOTTimeSt_During = otDetail.mastRequestOT is null ? "" : otDetail.mastRequestOT.mrOTTimeSt_During;
                    draftOTRequest.mrOTTimeEd_During = otDetail.mastRequestOT is null ? "" : otDetail.mastRequestOT.mrOTTimeEd_During;
                    draftOTRequest.mrOTTimeSt_After = otDetail.mastRequestOT is null ? "" : otDetail.mastRequestOT.mrOTTimeSt_After;
                    draftOTRequest.mrOTTimeEd_After = otDetail.mastRequestOT is null ? "" : otDetail.mastRequestOT.mrOTTimeEd_After;
                    draftOTRequest.mrModel = otDetail.mastRequestOT is null ? "" : otDetail.mastRequestOT.mrModel;
                    draftOTRequest.mrReason = otDetail.mastRequestOT is null ? "" : otDetail.mastRequestOT.mrReason;
                    draftOTRequest.mrProductionLine = otDetail.mastRequestOT is null ? "" : otDetail.mastRequestOT.mrProductionLine is null ? "" : otDetail.mastRequestOT.mrProductionLine;
                    draftOTRequest.mrRemark = otDetail.mastRequestOT is null ? "" : otDetail.mastRequestOT.mrRemark;
                    draftOTRequest.mrFlow = mastFlowApprove.mfFlowNo;
                    draftOTRequest.mrStep = draftCheck is null ? 0 : draftCheck.mrStep > 0 ? draftCheck.mrStep : 0;
                    draftOTRequest.mrStatus = draftCheck is null ? GlobalVariable.StatusDraft : draftCheck.mrStep > 0 ? draftCheck.mrStatus : GlobalVariable.StatusDraft;
                    //draftOTRequest.mrEmpApp = profileApprover is null ? "" : profileApprover.EMP_CODE.Trim();
                    //draftOTRequest.mrNameApp = profileApprover is null ? "" : profileApprover.EMP_TNAME.Trim() + " " + profileApprover.LAST_TNAME.Trim();

                    using (TransactionScope scope = new TransactionScope())
                    {
                        if (draftCheck is null)
                        {
                            _LAMP.MastRequestOTs.Add(draftOTRequest);
                        }
                        else
                        {
                            _LAMP.MastRequestOTs.Update(draftOTRequest);
                        }

                        _LAMP.SaveChanges();

                        _Cache.clearCacheMastRequestOT();
                        _Cache.clearCacheDetailRequestOT();

                        scope.Complete();
                    }
                    return Json(new { icon = "success", title = "สำเร็จ", message = "สร้างเป็นฉบับร่างแล้ว, เลขอ้างอิง " + RefNo, req = RefNo });
                }
                return Json(new { icon = "success", title = "สำเร็จ", message = "", req=""});


            }
            catch (Exception ex)
            { return Json(new { icon = "error", title = "ไม่สำเร็จ", message = ex.Message, req = ""}); }
        }

        public JsonResult CreateOTDocument(ViewMastRequestOT otRequest, multiModelOTForm otDetail, multiOTEmailForm otHistory, string[] NewWorkerList, string[] MailCCs)
        {
            _Cache.clearCacheAccEmployee();
            bool perAdmin = User.Claims.FirstOrDefault(s => s.Type == "Permission").Value?.ToString().ToUpper() == GlobalVariable.AdminPermission.ToUpper()
                            ? true : false;
            string flowType = _callFunc.TransPositionToFlowType(User.Claims.FirstOrDefault(s => s.Type == "Position").Value?.ToString().ToUpper());
            string strMail = "";
            if (MailCCs != null)
                foreach (string item in MailCCs)
                {
                    if (item != null)
                        if (item.Trim() != "")
                            strMail += item.Trim() + ",";
                }
            try
            {
                //set date format
                ViewMastRequestOT DraftOTRequest = _Cache.cacheMastRequestOT().Where(w=>w.mrNoReq == otRequest.mrNoReq).FirstOrDefault();
                string DateReq = DateTime.Today.ToString("dd/MM/yyyy");
                string TimeReq = DateTime.Now.ToString("HH:mm");
                string OTDate = DateTime.Parse(otDetail.mastRequestOT.mrOTDate).ToString("dd/MM/yyyy");

                //qry profile approver
                //find profile from mail
                if (otHistory.historyApproveds.htTo is null || otHistory.historyApproveds.htTo == "")
                    return Json(new { icon = "error", title = "ไม่สำเร็จ", message = "โปรดกรอกอีเมลผู้พิจารณา" });

                string empcodeApprover = otHistory.historyApproveds.htTo.Trim();
                string emailApprover = _Cache.cacheEmail().Where(w => w.emEmail_M365 != null && w.emEmpcode.Trim() == otHistory.historyApproveds.htTo.Trim()).FirstOrDefault() is null
                                       ? ""
                                       : _Cache.cacheEmail().Where(w => w.emEmpcode.Trim() == otHistory.historyApproveds.htTo.Trim()).FirstOrDefault().emEmail_M365;
                ViewAccEMPLOYEE profileApprover = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE.Trim() == empcodeApprover).FirstOrDefault();

                //qry flow from flowType
                ViewMastFlowApprove mastFlowApprove = _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == flowType && w.mfStep == 2).FirstOrDefault();
                if (mastFlowApprove is null)
                    return Json(new { icon = "warning", title = "Permission", message = "ตำแหน่งของคุณสูงหรือต่ำกว่าที่ระบบต้องการ" });
                //doc OT
                //req profile
                ViewAccEMPLOYEE reqProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == otRequest.mrEmpReq).FirstOrDefault();
                DraftOTRequest.mrDiviReq = perAdmin is false ? reqProfile.DIVI_CODE : GlobalVariable.AdminDivision;
                DraftOTRequest.mrDeptReq = perAdmin is false ? reqProfile.DEPT_CODE : GlobalVariable.AdminDepartment;
                DraftOTRequest.mrSecReq = perAdmin is false ? reqProfile.SEC_CODE : GlobalVariable.AdminSection;
                DraftOTRequest.mrGrpReq = perAdmin is false ? reqProfile.GRP_CODE : GlobalVariable.AdminGroup;
                DraftOTRequest.mrUnitReq = perAdmin is false ? reqProfile.UNT_CODE : GlobalVariable.AdminUnit;
                DraftOTRequest.mrDateReq = DateReq;
                DraftOTRequest.mrOTDate = OTDate;
                DraftOTRequest.mrFlow = mastFlowApprove.mfFlowNo;
                DraftOTRequest.mrStep = 1;
                DraftOTRequest.mrStatus = mastFlowApprove.mfSubject;
                DraftOTRequest.mrEmpApp = profileApprover.EMP_CODE.Trim();
                DraftOTRequest.mrNameApp = profileApprover.EMP_TNAME.Trim() + " " + profileApprover.LAST_TNAME.Trim();


                //worker OT
                List<ViewDetailRequestOT> workerDetails = _Cache.cacheDetailRequestOT().Where(w=>w.drNoReq == DraftOTRequest.mrNoReq).ToList();

                //update history
                ViewHistoryApproved historyApproved = new ViewHistoryApproved();
                historyApproved.htNoReq = DraftOTRequest.mrNoReq;
                historyApproved.htDateReq = DateReq;
                historyApproved.htStep = 1;
                historyApproved.htStatus = mastFlowApprove.mfSubject;
                historyApproved.htFrom = perAdmin is false ? reqProfile.EMP_CODE : GlobalVariable.AdminEmpCode;
                historyApproved.htTo = perAdmin is false ? profileApprover.EMP_CODE : GlobalVariable.AdminEmpCode;
                historyApproved.htCC = strMail.Length > 0 ? strMail.Substring(0, strMail.Length - 1) : "";
                historyApproved.htDate = DateReq;
                historyApproved.htTime = TimeReq;
                historyApproved.htRemark = otHistory.historyApproveds.htRemark;
                
                
                //error case #2
                ViewMastFlowApprove authApprove = _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == DraftOTRequest.mrFlow && w.mfStep == DraftOTRequest.mrStep + 1).FirstOrDefault();
                ViewMastUserApprove authSpecial = _Cache.cacheMastUserApprove().Where(w => w.muEmpCode == profileApprover.EMP_CODE).FirstOrDefault();
                if (!_callFunc.AuthorizeApprover(profileApprover.POS_CODE, authApprove, authSpecial))
                    return Json(new { icon = "error", title = "ผู้อนุมัติ", message = "ผู้อนุมัติมีสิทธิหรือตำแหน่งไม่ตรงกับขั้นตอนต่อไป" });
                //if (authApprove != null)
                //    if (_callFunc.TransPositionToLevel(profileApprover.POS_CODE.ToUpper()) < _callFunc.TransPositionToLevel(authApprove.mfPermission.ToUpper()))
                //        if (authSpecial != null) { 
                //            if (authSpecial.muPosition.ToUpper() != authApprove.mfPermission.ToUpper())
                //                return Json(new { icon = "error", title = "ผู้อนุมัติ", message = "ผู้อนุมัติมีสิทธิหรือตำแหน่งไม่ตรงกับขั้นตอนต่อไป" });
                //        }else{
                //            return Json(new { icon = "error", title = "ผู้อนุมัติ", message = "ผู้อนุมัติมีสิทธิหรือตำแหน่งไม่ตรงกับขั้นตอนต่อไป" });
                //        }

                //error case #1
                if (OTDate is null)
                    return Json(new { icon = "error", title = "ไม่สำเร็จ", message = "กรุณาเลือกวันที่จะทำ OT" });
                if (workerDetails.Count == 0)
                    return Json(new { icon = "error", title = "ไม่สำเร็จ", message = "กรุณาเลือกพนักงานที่จะทำ OT" });
                if(profileApprover.DEPT_CODE != reqProfile.DEPT_CODE)
                    return Json(new { icon = "error", title = "ไม่สำเร็จ", message = "ผู้ร้องขอและผู้อนุมัติมีฝ่ายที่ไม่ตรงกัน" });

                using (TransactionScope scope = new TransactionScope())
                {
                    _LAMP.MastRequestOTs.Update(DraftOTRequest);
                    _LAMP.HistoryApproveds.Add(historyApproved);
                    _LAMP.SaveChanges();

                    _Cache.clearCacheMastRequestOT();
                    _Cache.clearCacheHistoryApproved();

                    //setting send mail 
                    string mailSender = GlobalVariable.ProgramEmail;
                    string mailReceiver = emailApprover;
                    string mailCC = strMail;
                    string subject = "Request for Over time";
                    string body = "<h2>Test mail</h2>";

                    _callFunc.SendEmail(subject, mailSender, mailReceiver, body, reqProfile, profileApprover, mailCC, DraftOTRequest, otHistory.historyApproveds.htRemark);

                    scope.Complete();
                }
                return Json(new { icon = "success", title = "สำเร็จ", message = "ส่งคำร้องขอทำงานล่วงเวลาเรียบร้อยแล้ว, เลขอ้างอิง " + DraftOTRequest.mrNoReq });
            }
            catch (Exception ex)
            { return Json(new { icon = "error", title = "ไม่สำเร็จ", message = ex.Message }); }
        }

        public async Task<JsonResult> UpdateOTDocument(string req, ViewMastRequestOT otRequest, multiModelOTForm otDetail, multiOTEmailForm otHistory, string[] NewWorkerList, string[] MailCCs)
        {
            bool perAdmin = User.Claims.FirstOrDefault(s => s.Type == "Permission").Value?.ToString().ToUpper() == GlobalVariable.AdminPermission.ToUpper()
                        ? true : false;
            string DateReq = DateTime.Today.ToString("dd/MM/yyyy");
            string TimeReq = DateTime.Now.ToString("HH:mm");
            string OTDate = DateTime.Parse(otDetail.mastRequestOT.mrOTDate).ToString("dd/MM/yyyy");

            //get request in db
            ViewMastRequestOT mastRequestOT = await Task.Run(() => _Cache.cacheMastRequestOT().Where(w => w.mrNoReq == req && w.mrStep == 1).FirstOrDefault());
            if (mastRequestOT is null)
            {
                return Json(new { icon = "info", title = "ไม่สำเร็จ", message = "คำร้องนี้ได้ผ่านการพิจารณาจากหัวหน้าไปแล้ว" });
            }
            else
            {
                bool hasExistingTransaction = Transaction.Current != null;
                _Cache.cacheEmail();
                _Cache.cacheAccEmployee();
                _Cache.cacheHistoryApproved();
                _Cache.cacheDetailRequestOT();
                using (var scope = hasExistingTransaction
        ? new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled)
        : new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        //qry profile approver
                        //find profile from mail
                        string empcodeApprover = otHistory.historyApproveds.htTo.Trim();
                        string emailApprover = _Cache.cacheEmail().Where(w => w.emEmail_M365 != null && w.emEmpcode.Trim() == otHistory.historyApproveds.htTo.Trim()).FirstOrDefault() is null
                                               ? ""
                                               : _Cache.cacheEmail().Where(w => w.emEmpcode.Trim() == otHistory.historyApproveds.htTo.Trim()).FirstOrDefault().emEmail_M365;
                        ViewAccEMPLOYEE profileApprover = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE.Trim() == empcodeApprover).FirstOrDefault();

                        //set update mastRequestOT
                        ViewAccEMPLOYEE reqProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == otRequest.mrEmpReq).FirstOrDefault();
                        mastRequestOT.mrDiviReq = perAdmin is false ? reqProfile.DIVI_CODE : GlobalVariable.AdminDivision;
                        mastRequestOT.mrDeptReq = perAdmin is false ? reqProfile.DEPT_CODE : GlobalVariable.AdminDepartment;
                        mastRequestOT.mrSecReq = perAdmin is false ? reqProfile.SEC_CODE : GlobalVariable.AdminSection;
                        mastRequestOT.mrGrpReq = perAdmin is false ? reqProfile.GRP_CODE : GlobalVariable.AdminGroup;
                        mastRequestOT.mrUnitReq = perAdmin is false ? reqProfile.UNT_CODE : GlobalVariable.AdminUnit;

                        mastRequestOT.mrOTDate = OTDate;
                        //mastRequestOT.mrOTTimeSt = otDetail.mastRequestOT.mrOTTimeSt;
                        //mastRequestOT.mrOTTimeEd = otDetail.mastRequestOT.mrOTTimeEd;

                        mastRequestOT.mrOTTimeSt_Before = otDetail.mastRequestOT.mrOTTimeSt_Before;
                        mastRequestOT.mrOTTimeEd_Before = otDetail.mastRequestOT.mrOTTimeEd_Before;
                        mastRequestOT.mrOTTimeSt_During = otDetail.mastRequestOT.mrOTTimeSt_During;
                        mastRequestOT.mrOTTimeEd_During = otDetail.mastRequestOT.mrOTTimeEd_During;
                        mastRequestOT.mrOTTimeSt_After = otDetail.mastRequestOT.mrOTTimeSt_After;
                        mastRequestOT.mrOTTimeEd_After = otDetail.mastRequestOT.mrOTTimeEd_After;

                        mastRequestOT.mrModel = otDetail.mastRequestOT.mrModel;
                        mastRequestOT.mrReason = otDetail.mastRequestOT.mrReason;
                        mastRequestOT.mrProductionLine = otDetail.mastRequestOT.mrProductionLine is null ? "" : otDetail.mastRequestOT.mrProductionLine;
                        mastRequestOT.mrRemark = otDetail.mastRequestOT.mrRemark;
                        mastRequestOT.mrEmpApp = profileApprover.EMP_CODE.Trim();
                        mastRequestOT.mrNameApp = profileApprover.EMP_TNAME.Trim() + " " + profileApprover.LAST_TNAME.Trim();

                        //set new worker OT
                        //worker OT
                        List<ViewDetailRequestOT> workerDetails = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == req).ToList();

                        //set update history
                        //change mail form to db
                        string strMail = "";
                        if (MailCCs != null)
                            foreach (string item in MailCCs)
                            {
                                if (item != null)
                                    if (item.Trim() != "")
                                        strMail += item.Trim() + ",";
                            }

                        ViewHistoryApproved historyRequest = await Task.Run(() => _Cache.cacheHistoryApproved().Where(w => w.htNoReq == req && w.htStep == 1 && w.htStatus != GlobalVariable.StatusApproved).FirstOrDefault());
                        historyRequest.htDateReq = DateReq;
                        historyRequest.htTo = perAdmin is false ? profileApprover.EMP_CODE : GlobalVariable.AdminEmpCode; ;
                        historyRequest.htCC = strMail.Length > 0 ? strMail.Substring(0, strMail.Length - 1) : "";
                        historyRequest.htDate = DateReq;
                        historyRequest.htTime = TimeReq;
                        historyRequest.htRemark = otHistory.historyApproveds.htRemark;

                        //error case
                        if (OTDate is null)
                            return Json(new { icon = "error", title = "ไม่สำเร็จ", message = "กรุณาเลือกวันที่จะทำ OT" });
                        if(workerDetails.Count == 0)
                            return Json(new { icon = "error", title = "ไม่สำเร็จ", message = "กรุณาเลือกพนักงานที่จะทำ OT" });

                        await Task.Run(() => _LAMP.MastRequestOTs.Update(mastRequestOT));
                        await Task.Run(() => _LAMP.HistoryApproveds.Update(historyRequest));
                        await _LAMP.SaveChangesAsync();

                        //setting send mail 
                        string mailSender = GlobalVariable.ProgramEmail;
                        string mailReceiver = emailApprover;
                        string mailCC = strMail;
                        string subject = "Request for Over time";
                        string body = "<h2>Test mail</h2>";

                        _callFunc.SendEmail(subject, mailSender, mailReceiver, body, reqProfile, profileApprover, mailCC, mastRequestOT, otHistory.historyApproveds.htRemark);

                        _Cache.clearCacheMastRequestOT();
                        _Cache.clearCacheDetailRequestOT();
                        _Cache.clearCacheHistoryApproved();

                        scope.Complete();
                        return Json(new { icon = "success", title = "สำเร็จ", message = "เปลี่ยนแปลงข้อมูลเรียบร้อยแล้ว" });

                    }
                    catch (Exception ex)
                    {
                        return Json(new { icon = "error", title = "ไม่สำเร็จ", message = ex.Message });
                    }
                    finally
                    {
                        scope.Dispose();
                    }
                }
            }
        }

        public MultiDocMast queryMyToday()
        {
            WebClient request = new WebClient();
            string imgPath = GlobalVariable.imgPath;
            string EmpCode = User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value?.ToString();
            string ToDayReq = DateTime.Today.ToString("dd/MM/yyyy");

            MultiDocMast docMast = new MultiDocMast();
            docMast.docList = new List<MultiDocDetails>();
            docMast.mastFlow = _Cache.cacheMastFlowApprove().ToList();
            docMast.mastJobs = _Cache.cacheMastJob().ToList();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT()
                                        .Where(w => w.mrEmpReq == EmpCode && w.mrDateReq == ToDayReq)
                                        .OrderByDescending(o => o.mrNoReq).ToList())
            {
                MultiDocDetails docDetails = new MultiDocDetails();
                docDetails.workerImages = new List<workerImages>();
                List<ViewDetailRequestOT> workerLists = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == items.mrNoReq).ToList();
                docDetails.requestOT = items;
                docDetails.workerList = workerLists;
                if (workerLists != null)
                    foreach (string workerEmpcode in workerLists.Select(s => s.drEmpCode))
                    {
                        imgPath = GlobalVariable.imgPath + "/" + workerEmpcode + ".jpg";
                        request.Credentials = new NetworkCredential(GlobalVariable.UFTP, GlobalVariable.PFTP);

                        try
                        {
                            //byte[] imgFile = request.DownloadData(imgPath);
                            //string file64String = Convert.ToBase64String(imgFile);
                            //string imgDataURL = string.Format("data:image/jpg;base64,{0}", file64String);
                            workerImages workerImage = new workerImages()
                            {
                                empcode = workerEmpcode,
                                //image = imgDataURL,
                            };
                            docDetails.workerImages.Add(workerImage);
                        }
                        catch (WebException wex)
                        { Console.Write(wex.ToString()); }
                    };
                docDetails.stepHistory = _Cache.cacheHistoryApproved().Where(w => w.htNoReq == items.mrNoReq).ToList();
                foreach (var oldFrom in docDetails.stepHistory)
                {
                    //lenght empcode 
                    if (oldFrom.htFrom.Length <= 6)
                    {
                        ViewAccEMPLOYEE profile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == oldFrom.htFrom).FirstOrDefault();
                        if (profile is null)
                            profile = new ViewAccEMPLOYEE();
                        oldFrom.htFrom = "คุณ" + profile.EMP_TNAME + " " + profile.LAST_TNAME;
                    }
                }

                docMast.docList.Add(docDetails);
            }

            return docMast;
        }

        public MultiDocMast queryMyYesterday()
        {
            WebClient request = new WebClient();
            string imgPath = GlobalVariable.imgPath;
            string EmpCode = User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value?.ToString();
            string YesterdayReq = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");

            MultiDocMast docMast = new MultiDocMast();
            docMast.docList = new List<MultiDocDetails>();
            docMast.mastFlow = _Cache.cacheMastFlowApprove().ToList();
            docMast.mastJobs = _Cache.cacheMastJob().ToList();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT()
                                        .Where(w => w.mrEmpReq == EmpCode && w.mrDateReq == YesterdayReq)
                                        .OrderByDescending(o => o.mrNoReq).ToList())
            {
                MultiDocDetails docDetails = new MultiDocDetails();
                docDetails.workerImages = new List<workerImages>();
                List<ViewDetailRequestOT> workerLists = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == items.mrNoReq).ToList();
                docDetails.requestOT = items;
                docDetails.workerList = workerLists;
                if (workerLists != null)
                    foreach (string workerEmpcode in workerLists.Select(s => s.drEmpCode))
                    {
                        imgPath = GlobalVariable.imgPath + "/" + workerEmpcode + ".jpg";
                        request.Credentials = new NetworkCredential(GlobalVariable.UFTP, GlobalVariable.PFTP);

                        try
                        {
                            //byte[] imgFile = request.DownloadData(imgPath);
                            //string file64String = Convert.ToBase64String(imgFile);
                            //string imgDataURL = string.Format("data:image/jpg;base64,{0}", file64String);
                            workerImages workerImage = new workerImages()
                            {
                                empcode = workerEmpcode,
                                //image = imgDataURL,
                            };
                            docDetails.workerImages.Add(workerImage);
                        }
                        catch (WebException wex)
                        { Console.Write(wex.ToString()); }
                    };
                docDetails.stepHistory = _Cache.cacheHistoryApproved().Where(w => w.htNoReq == items.mrNoReq).ToList();
                foreach (var oldFrom in docDetails.stepHistory)
                {
                    //lenght empcode 
                    if (oldFrom.htFrom.Length <= 6)
                    {
                        ViewAccEMPLOYEE profile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == oldFrom.htFrom).FirstOrDefault();
                        oldFrom.htFrom = "คุณ" + profile.EMP_TNAME + " " + profile.LAST_TNAME;
                    }
                }
                docMast.docList.Add(docDetails);
            }

            return docMast;
        }

        public MultiDocMast queryAllToday()
        {
            WebClient request = new WebClient();
            string imgPath = GlobalVariable.imgPath;
            string ToDayReq = DateTime.Today.ToString("dd/MM/yyyy");

            MultiDocMast docMast = new MultiDocMast();
            docMast.docList = new List<MultiDocDetails>();
            docMast.mastFlow = _Cache.cacheMastFlowApprove().ToList();
            docMast.mastJobs = _Cache.cacheMastJob().ToList();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT()
                                        .Where(w => w.mrDateReq == ToDayReq)
                                        .OrderByDescending(o => o.mrNoReq).ToList())
            {
                MultiDocDetails docDetails = new MultiDocDetails();
                docDetails.workerImages = new List<workerImages>();
                List<ViewDetailRequestOT> workerLists = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == items.mrNoReq).ToList();
                docDetails.requestOT = items;
                docDetails.workerList = workerLists;
                if (workerLists != null)
                    foreach (string workerEmpcode in workerLists.Select(s => s.drEmpCode))
                    {
                        imgPath = GlobalVariable.imgPath + "/" + workerEmpcode + ".jpg";
                        request.Credentials = new NetworkCredential(GlobalVariable.UFTP, GlobalVariable.PFTP);

                        try
                        {
                            //byte[] imgFile = request.DownloadData(imgPath);
                            //string file64String = Convert.ToBase64String(imgFile);
                            //string imgDataURL = string.Format("data:image/jpg;base64,{0}", file64String);
                            workerImages workerImage = new workerImages()
                            {
                                empcode = workerEmpcode,
                                //image = imgDataURL,
                            };
                            docDetails.workerImages.Add(workerImage);
                        }
                        catch (WebException wex)
                        { Console.Write(wex.ToString()); }
                    };
                docDetails.stepHistory = _Cache.cacheHistoryApproved().Where(w => w.htNoReq == items.mrNoReq).ToList();
                foreach (var oldFrom in docDetails.stepHistory)
                {
                    //lenght empcode 
                    if (oldFrom.htFrom.Length <= 6)
                    {
                        ViewAccEMPLOYEE profile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == oldFrom.htFrom).FirstOrDefault();
                        oldFrom.htFrom = "คุณ" + profile.EMP_TNAME + " " + profile.LAST_TNAME;
                    }
                }
                docMast.docList.Add(docDetails);
            }

            return docMast;
        }

        public MultiDocMast queryAllYesterday()
        {
            WebClient request = new WebClient();
            string imgPath = GlobalVariable.imgPath;
            string YesterdayReq = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
            MultiDocMast docMast = new MultiDocMast();
            docMast.docList = new List<MultiDocDetails>();
            docMast.mastFlow = _Cache.cacheMastFlowApprove().ToList();
            docMast.mastJobs = _Cache.cacheMastJob().ToList();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT()
                                        .Where(w => w.mrDateReq == YesterdayReq)
                                        .OrderByDescending(o => o.mrNoReq).ToList())
            {
                MultiDocDetails docDetails = new MultiDocDetails();
                docDetails.workerImages = new List<workerImages>();
                List<ViewDetailRequestOT> workerLists = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == items.mrNoReq).ToList();
                docDetails.requestOT = items;
                docDetails.workerList = workerLists;
                if (workerLists != null)
                    foreach (string workerEmpcode in workerLists.Select(s => s.drEmpCode))
                    {
                        imgPath = GlobalVariable.imgPath + "/" + workerEmpcode + ".jpg";
                        request.Credentials = new NetworkCredential(GlobalVariable.UFTP, GlobalVariable.PFTP);

                        try
                        {
                            //byte[] imgFile = request.DownloadData(imgPath);
                            //string file64String = Convert.ToBase64String(imgFile);
                            //string imgDataURL = string.Format("data:image/jpg;base64,{0}", file64String);
                            workerImages workerImage = new workerImages()
                            {
                                empcode = workerEmpcode,
                                //image = imgDataURL,
                            };
                            docDetails.workerImages.Add(workerImage);
                        }
                        catch (WebException wex)
                        { Console.Write(wex.ToString()); }
                    };
                docDetails.stepHistory = _Cache.cacheHistoryApproved().Where(w => w.htNoReq == items.mrNoReq).ToList();
                foreach (var oldFrom in docDetails.stepHistory)
                {
                    //lenght empcode 
                    if (oldFrom.htFrom.Length <= 6)
                    {
                        ViewAccEMPLOYEE profile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == oldFrom.htFrom).FirstOrDefault();
                        oldFrom.htFrom = "คุณ" + profile.EMP_TNAME + " " + profile.LAST_TNAME;
                    }
                }
                docMast.docList.Add(docDetails);
            }

            return docMast;
        }

        #endregion
    }
}