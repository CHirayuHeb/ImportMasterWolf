using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ImportMasterWolf.Models.Approval;
using ImportMasterWolf.Models.Common;
using ImportMasterWolf.Models.DBConnect;
using ImportMasterWolf.Models.New;
using ImportMasterWolf.Models.Table.HRMS;
using ImportMasterWolf.Models.Table.LAMP;

namespace ImportMasterWolf.Controllers.Approval
{
    public class WebClientWithTimeout : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest wr = base.GetWebRequest(address);
            wr.Timeout = 5000; // timeout in milliseconds (ms)
            return wr;
        }
    }
    public class ApprovalController : Controller
    {
        private LAMP _LAMP;
        private HRMS _HRMS;
        private IT _IT;
        private CacheSettingController _Cache;
        private FunctionsController _callFunc;
        public ApprovalController(LAMP lamp, HRMS hrms, IT it, CacheSettingController cacheController, FunctionsController callfunction)
        {
            _LAMP = lamp;
            _HRMS = hrms;
            _IT = it;
            _Cache = cacheController;
            _callFunc = callfunction;
        }

        [Authorize(Policy = "perGeneral")]
        public IActionResult Index(string req)
        {
            string imgPath = GlobalVariable.imgPath;
            string EmpCode = User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value?.ToString();
            MultiNewLate docNewLate = new MultiNewLate();
            docNewLate.docList = new List<MultiDocDetails>();
            docNewLate.req = req != null && req != "" ? req : "";
            docNewLate.mastFlow = _Cache.cacheMastFlowApprove().ToList();
            docNewLate.mastJobs = _Cache.cacheMastJob().ToList();

            //error case Permission
            ViewBag.PermissionThisPage =  _callFunc.TransferDepartmentToCodeName(User.Claims.FirstOrDefault(s => s.Type == "Department").Value);
            ViewMastUserApprove authSpecial = _Cache.cacheMastUserApprove().Where(w => w.muEmpCode == User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value).FirstOrDefault();
            if (authSpecial != null)
                ViewBag.PermissionThisPage = authSpecial.muDeptCode;
            
            //_Cache.clearCacheMastRequestOT();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT().Where(w => w.mrEmpApp == EmpCode && !w.mrStatus.StartsWith(GlobalVariable.StatusFinishedST) && w.mrStep != 0)
                                                                            .OrderByDescending(o => DateTime.Parse(o.mrDateReq.Substring(o.mrDateReq.Length - 4) + "/"
                                                                                                  + o.mrDateReq.Substring(3, 2) + "/"
                                                                                                  + o.mrDateReq.Substring(0, 2)))
                                                                            .ThenByDescending(o => o.mrNoReq.Substring(o.mrNoReq.Length - 2)).ToList())
            {
                items.mrDiviReq = _callFunc.TransferCodeNameToDivision(items.mrDiviReq);
                items.mrDeptReq = _callFunc.TransferCodeNameToDepartment(items.mrDeptReq);

                MultiDocDetails docDetails = new MultiDocDetails();
                docDetails.workerImages = new List<workerImages>();
                List<ViewDetailRequestOT> workerLists = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == items.mrNoReq).ToList();

                docDetails.requestOT = items;
                docDetails.workerList = workerLists;
                if (workerLists != null)
                {
                    List<workerImages> empPics = new List<workerImages>();
                    foreach (string empPic in workerLists.Select(s => s.drEmpCode).Distinct())
                    {
                        //imgPath = GlobalVariable.imgPath + "/" + empPic + ".jpg";
                        //using (WebClient request = new WebClientWithTimeout())
                        //{
                            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            //request.Credentials = new NetworkCredential(GlobalVariable.UFTP, GlobalVariable.PFTP);
                            try
                            {
                                //byte[] imgFile = request.DownloadData(imgPath);
                                //string file64String = Convert.ToBase64String(imgFile);
                                //string imgDataURL = string.Format("data:image/jpg;base64,{0}", file64String);
                                workerImages workerImage = new workerImages()
                                {
                                    empcode = empPic,
                                    //image = imgDataURL,
                                };
                                empPics.Add(workerImage);
                            }
                            catch (WebException wex)
                            { Console.Write(wex.ToString()); }
                        //}
                    }
                    foreach (string workerEmpcode in workerLists.Select(s => s.drEmpCode))
                    {
                            try
                            {
                                docDetails.workerImages.Add(empPics.Where(w=>w.empcode == workerEmpcode).FirstOrDefault());
                            }
                            catch (WebException wex)
                            { Console.Write(wex.ToString()); }
                    }
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
                docNewLate.docList.Add(docDetails);
            }
            return View(docNewLate);
        }

        [HttpPost]
        public async Task<JsonResult> ApproveSelected([FromBody]MultiApproveSelected list)
        {

            //error case #1
            if (list is null)
                return await Task.Run(() => Json(new { icon = "error", title = "ข้อมูล", message = "โปรดตรวจสอบข้อมูล แล้วลองอีกครั้ง" }));
            if (list.empcodeMailTo is null || list.empcodeMailTo.Trim() == "")
                return await Task.Run(() => Json(new { icon = "error", title = "ผู้อนุมัติ", message = "โปรดตรวจสอบรหัสพนักงานผู้อนุมัติ" }));
            string DateReq = DateTime.Today.ToString("dd/MM/yyyy");
            string TimeReq = DateTime.Now.ToString("HH:mm");
            int countingDisplay = 0;
            List<ViewMastRequestOT> DocSuccessForSendMail = new List<ViewMastRequestOT>();
            List<ViewMastRequestOT> DocRejectedForSendMail = new List<ViewMastRequestOT>();
            ViewAccEMPLOYEE reqProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value).FirstOrDefault();
            ViewAccEMPLOYEE profileApprover = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == list.empcodeMailTo.Trim()).FirstOrDefault();
            ViewAccEMPLOYEE approvingBy = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value).FirstOrDefault();
            string approvingName = approvingBy.EMP_TNAME + " " + approvingBy.LAST_TNAME;

            string strMail = "";
            if (list.cc != null)
                foreach (string email in list.cc)
                {
                    if (email.Trim() != "")
                        strMail += email.Trim() + ",";
                }

            bool hasExistingTransaction = Transaction.Current != null;
            using (var scope = hasExistingTransaction
                    ? new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled)
                    : new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    _Cache.cacheAccEmployee();
                    _Cache.cacheMastRequestOT();
                    _Cache.cacheHistoryApproved();
                    _Cache.cacheMastFlowApprove();
                    _Cache.cacheEmail();
                    foreach (ApproveSelected DocData in list.Document)
                    {
                        countingDisplay++;
                        if (DocData.workerList.Count > 0)
                        {
                            ViewMastRequestOT requestOT = await Task.Run(() => _LAMP.MastRequestOTs.Where(w => w.mrNoReq == DocData.reqNo).FirstOrDefault());
                            List<ViewDetailRequestOT> workerList = await Task.Run(() => _LAMP.DetailRequestOTs.Where(w => (w.drNoReq == DocData.reqNo)).ToList());
                            ViewHistoryApproved recentHistoryStep = await Task.Run(() => _LAMP.HistoryApproveds.Where(w => w.htNoReq == requestOT.mrNoReq && w.htStep == requestOT.mrStep).FirstOrDefault());
                            ViewMastFlowApprove nextStauts = await Task.Run(() => _Cache.cacheMastFlowApprove().Where(w => (w.mfFlowNo == requestOT.mrFlow) && int.Parse(w.mfStep.Value.ToString()) == (requestOT.mrStep + 2)).FirstOrDefault());

                            //error case #2
                            ViewMastFlowApprove authApprove = _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == requestOT.mrFlow && w.mfStep == requestOT.mrStep + 2).FirstOrDefault();
                            ViewMastUserApprove authSpecial = _Cache.cacheMastUserApprove().Where(w => w.muEmpCode == profileApprover.EMP_CODE).FirstOrDefault();

                            if(!_callFunc.AuthorizeApprover(profileApprover.POS_CODE, authApprove, authSpecial))
                                return Json(new { icon = "error", title = "ผู้อนุมัติ", message = "ผู้อนุมัติมีสิทธิหรือตำแหน่งไม่ตรงกับขั้นตอนต่อไป", count = countingDisplay });
                            //if (authApprove != null)
                            //    if (_callFunc.TransPositionToLevel(profileApprover.POS_CODE.ToUpper()) != _callFunc.TransPositionToLevel(authApprove.mfPermission.ToUpper()))
                            //    {
                            //        if (authSpecial != null)
                            //        {
                            //            if (authSpecial.muPosition.ToUpper() != authApprove.mfPermission.ToUpper())
                            //                return Json(new { icon = "error", title = "ผู้อนุมัติ", message = "ผู้อนุมัติมีสิทธิหรือตำแหน่งไม่ตรงกับขั้นตอนต่อไป", count = countingDisplay });
                            //        }
                            //        else
                            //        {
                            //            return Json(new { icon = "error", title = "ผู้อนุมัติ", message = "ผู้อนุมัติมีสิทธิหรือตำแหน่งไม่ตรงกับขั้นตอนต่อไป", count = countingDisplay });
                            //        }
                            //    }

                            //update value
                            //MastRequestOT
                            int nextStep = requestOT.mrStep.Value + 1;
                            bool notRejected = await Task.Run(() => _Cache.cacheHistoryApproved().Where(w => (w.htNoReq == requestOT.mrNoReq) && (w.htStep == nextStep)).FirstOrDefault()) is null;
                            requestOT.mrDiviReq = _callFunc.TransferDivisionToCodeName(requestOT.mrDiviReq);
                            requestOT.mrDeptReq = _callFunc.TransferDepartmentToCodeName(requestOT.mrDeptReq);
                            requestOT.mrStep = nextStep;
                            requestOT.mrStatus = nextStauts.mfSubject;
                            requestOT.mrEmpApp = profileApprover.EMP_CODE;
                            requestOT.mrNameApp = profileApprover.EMP_TNAME + " " + profileApprover.LAST_TNAME;

                            //DetaiRequestOT
                            //set all rejected
                            foreach (var row in workerList)
                            {
                                row.drStatus = GlobalVariable.StatusRejected + " โดย คุณ" + approvingName;
                            }

                            //set approved go to next step status
                            foreach (string empcode in DocData.workerList)
                            {
                                workerList.Where(w => w.drEmpCode.Trim() == empcode.Trim()).FirstOrDefault().drStatus = nextStauts.mfSubject;
                            }

                            //HistoryStep
                            recentHistoryStep.htStatus = GlobalVariable.StatusApproved + " โดย คุณ" + approvingName;
                            recentHistoryStep.htDate = DateReq;
                            recentHistoryStep.htTime = TimeReq;

                            //add new history
                            ViewHistoryApproved addHistory = new ViewHistoryApproved();
                            if (notRejected is false)
                                addHistory = await Task.Run(() => _LAMP.HistoryApproveds.Where(w => (w.htNoReq == requestOT.mrNoReq) && (w.htStep == nextStep)).FirstOrDefault());

                            addHistory.htNoReq = requestOT.mrNoReq;
                            addHistory.htDateReq = requestOT.mrDateReq;
                            addHistory.htStep = nextStep;
                            addHistory.htStatus = nextStauts.mfSubject;
                            addHistory.htFrom = User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value;
                            addHistory.htTo = list.empcodeMailTo.Trim();
                            addHistory.htCC = strMail.Length > 0 ? strMail.Substring(0, strMail.Length - 1) : "";
                            addHistory.htDate = DateReq;
                            addHistory.htTime = TimeReq;
                            addHistory.htRemark = list.remark is null ? "" : list.remark;


                            _LAMP.MastRequestOTs.Update(requestOT);
                            _LAMP.DetailRequestOTs.UpdateRange(workerList);
                            _LAMP.HistoryApproveds.Update(recentHistoryStep);
                            if (notRejected is true)
                                _LAMP.HistoryApproveds.Add(addHistory);
                            if (notRejected is false)
                                _LAMP.HistoryApproveds.Update(addHistory);

                            //setting send mail 
                            DocSuccessForSendMail.Add(requestOT);
                        }
                        else
                        {
                            bool returnStatus = await _Reject(DocData, list.remark);
                            if (returnStatus)
                            {
                                ViewMastRequestOT rejectRequestOT = await Task.Run(() => _LAMP.MastRequestOTs.Where(w => w.mrNoReq == DocData.reqNo).FirstOrDefault());
                                DocRejectedForSendMail.Add(rejectRequestOT);
                            }


                            if (returnStatus is false)
                            {
                                scope.Dispose();
                                return Json(new { icon = "error", title = "ไม่สำเร็จ", message = "wrong values at reject func. ", count = countingDisplay });
                            }
                        }
                    }
                    //in using scope out foreach
                    _LAMP.SaveChanges();
                    scope.Complete();

                    //send mail approved
                    foreach (ViewMastRequestOT requestOT in DocSuccessForSendMail)
                    {
                        string mailSender = GlobalVariable.ProgramEmail;
                        string mailReceiver = _Cache.cacheEmail().Where(w => w.emEmail_M365 != null && w.emEmpcode.Trim() == list.empcodeMailTo.Trim()).FirstOrDefault() is null
                                           ? ""
                                           : _Cache.cacheEmail().Where(w => w.emEmpcode.Trim() == list.empcodeMailTo.Trim()).FirstOrDefault().emEmail_M365;
                        string subject = "Request for Over time";
                        string body = "<h2>Test mail</h2>";
                        string mailCC = strMail;
                        _callFunc.SendEmail(subject, mailSender, mailReceiver, body, reqProfile, profileApprover, mailCC, requestOT, list.remark);
                    }

                    //send mail rejected
                    foreach (ViewMastRequestOT rejectedOT in DocRejectedForSendMail)
                    {
                        int previosStep = rejectedOT.mrStep.Value - 1;
                        ViewHistoryApproved recentHistoryStep = await Task.Run(() => _Cache.cacheHistoryApproved().Where(w => w.htNoReq == rejectedOT.mrNoReq && w.htStep == rejectedOT.mrStep).FirstOrDefault());
                        ViewAccEMPLOYEE profileRejecter = previosStep == 0
                                                        ? _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == rejectedOT.mrEmpReq.Trim()).FirstOrDefault()
                                                        : _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == recentHistoryStep.htFrom.Trim()).FirstOrDefault();
                        string mailSender = GlobalVariable.ProgramEmail;
                        string mailReceiver = _Cache.cacheEmail().Where(w => w.emEmail_M365 != null && w.emEmpcode.Trim() == profileRejecter.EMP_CODE.Trim()).FirstOrDefault() is null
                                           ? ""
                                           : _Cache.cacheEmail().Where(w => w.emEmpcode.Trim() == profileRejecter.EMP_CODE.Trim()).FirstOrDefault().emEmail_M365;
                        List<string> qryCC = await Task.Run(() => _Cache.cacheHistoryApproved().Where(w => w.htNoReq == rejectedOT.mrNoReq && w.htStep == rejectedOT.mrStep).Select(s => s.htFrom).ToList());
                        string rejectedMail = "";
                        if (qryCC != null)
                            foreach (string email in qryCC)
                            {
                                string ccemail = _callFunc.FindEmailFromEmpCode(email.Trim());
                                if (ccemail != "")
                                    rejectedMail += ccemail.Trim() + ",";
                            }
                        string mailCC = rejectedMail;
                        string subject = "Request for Over time : " + rejectedOT.mrNoReq + " Rejected";
                        string body = GlobalVariable.StatusRejected;
                        _callFunc.SendEmail(subject, mailSender, mailReceiver, body, reqProfile, profileRejecter, mailCC, rejectedOT, list.remark);
                    }

                    _Cache.clearCacheMastRequestOT();
                    _Cache.clearCacheDetailRequestOT();
                    _Cache.clearCacheHistoryApproved();
                }
                catch (Exception ex)
                {
                    { return Json(new { icon = "error", title = "ไม่สำเร็จ", message = ex.Message, count = countingDisplay }); }
                }
                finally { scope.Dispose(); }
            }
            return Json(new { icon = "success", title = "สำเร็จ", message = "อนุมัติสำเร็จทั้งหมด", count = countingDisplay });
        }

        [HttpPost]
        public async Task<JsonResult> Approved([FromBody]MutiApprove list)
        {
            bool hasExistingTransaction = Transaction.Current != null;
            string DateReq = DateTime.Today.ToString("dd/MM/yyyy");
            string TimeReq = DateTime.Now.ToString("HH:mm");
            ViewAccEMPLOYEE reqProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value).FirstOrDefault();
            ViewAccEMPLOYEE profileApprover = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == list.empcode.Trim()).FirstOrDefault();
            ViewMastRequestOT requestOT = await Task.Run(() => _LAMP.MastRequestOTs.Where(w => w.mrNoReq == list.reqNo).FirstOrDefault());
            List<ViewDetailRequestOT> workerList = await Task.Run(() => _LAMP.DetailRequestOTs.Where(w => (w.drNoReq == list.reqNo)).ToList());
            ViewHistoryApproved recentHistoryStep = await Task.Run(() => _LAMP.HistoryApproveds.Where(w => w.htNoReq == list.reqNo && w.htStep == requestOT.mrStep).FirstOrDefault());
            ViewMastFlowApprove nextStauts = await Task.Run(() => _Cache.cacheMastFlowApprove().Where(w => (w.mfFlowNo == requestOT.mrFlow) && int.Parse(w.mfStep.Value.ToString()) == (requestOT.mrStep + 2)).FirstOrDefault());
            ViewAccEMPLOYEE approvingBy = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == User.Claims.FirstOrDefault(s=>s.Type == "EmpCode").Value).FirstOrDefault();
            string approvingName = approvingBy.EMP_TNAME + " " + approvingBy.LAST_TNAME;
            string strMail = "";
            if (list.cc != null)
                foreach (string email in list.cc)
                {
                    if (email.Trim() != "")
                        strMail += email.Trim() + ",";
                }
            //update value
            //MastRequestOT
            int nextStep = requestOT.mrStep.Value + 1;
            bool notRejected = await Task.Run(() => _Cache.cacheHistoryApproved().Where(w => (w.htNoReq == list.reqNo) && (w.htStep == nextStep)).FirstOrDefault()) is null;
            requestOT.mrStep = nextStep;
            requestOT.mrStatus = nextStauts.mfSubject;
            requestOT.mrEmpApp = profileApprover.EMP_CODE;
            requestOT.mrNameApp = profileApprover.EMP_TNAME + " " + profileApprover.LAST_TNAME;

            //DetaiRequestOT
            //set all rejected
            foreach (var row in workerList)
            {
                row.drStatus = GlobalVariable.StatusRejected + " โดย คุณ" + approvingName;
            }
            //set approved go to next step status
            foreach (string empcode in list.workerList)
            {
                workerList.Where(w => w.drEmpCode.Trim() == empcode.Trim()).FirstOrDefault().drStatus = nextStauts.mfSubject;
            }

            //HistoryStep
            recentHistoryStep.htStatus = GlobalVariable.StatusApproved + " โดย คุณ" + approvingName;
            recentHistoryStep.htDate = DateReq;
            recentHistoryStep.htTime = TimeReq;
            //add new history

            ViewHistoryApproved addHistory = new ViewHistoryApproved();
            if (notRejected is false)
                addHistory = await Task.Run(() => _Cache.cacheHistoryApproved().Where(w => (w.htNoReq == list.reqNo) && (w.htStep == nextStep)).FirstOrDefault());

            addHistory.htNoReq = requestOT.mrNoReq;
            addHistory.htDateReq = requestOT.mrDateReq;
            addHistory.htStep = nextStep;
            addHistory.htStatus = nextStauts.mfSubject;
            addHistory.htFrom = User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value;
            addHistory.htTo = list.empcode.Trim();
            addHistory.htCC = strMail.Length > 0 ? strMail.Substring(0, strMail.Length - 1) : "";
            addHistory.htDate = DateReq;
            addHistory.htTime = TimeReq;
            addHistory.htRemark = list.remark is null ? "" : list.remark;

            //error case #2
            ViewMastFlowApprove authApprove = _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == requestOT.mrFlow && w.mfStep == requestOT.mrStep + 1).FirstOrDefault();
            ViewMastUserApprove authSpecial = _Cache.cacheMastUserApprove().Where(w => w.muEmpCode == profileApprover.EMP_CODE).FirstOrDefault();
            if (!_callFunc.AuthorizeApprover(profileApprover.POS_CODE, authApprove, authSpecial))
                return Json(new { icon = "error", title = "ผู้อนุมัติ", message = "ผู้อนุมัติมีสิทธิหรือตำแหน่งไม่ตรงกับขั้นตอนต่อไป" });
            //if (authApprove != null)
            //    if (_callFunc.TransPositionToLevel(profileApprover.POS_CODE.ToUpper()) < _callFunc.TransPositionToLevel(authApprove.mfPermission.ToUpper()))
            //        if (authSpecial != null)
            //        {
            //            if (authSpecial.muPosition.ToUpper() != authApprove.mfPermission.ToUpper())
            //                return Json(new { icon = "error", title = "ผู้อนุมัติ", message = "ผู้อนุมัติมีสิทธิหรือตำแหน่งไม่ตรงกับขั้นตอนต่อไป" });
            //        }
            //        else
            //        {
            //            return Json(new { icon = "error", title = "ผู้อนุมัติ", message = "ผู้อนุมัติมีสิทธิหรือตำแหน่งไม่ตรงกับขั้นตอนต่อไป" });
            //        }

            using (var scope = hasExistingTransaction
                    ? new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled)
                    : new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    //connect case error platform scope
                    _Cache.cacheAccEmployee();
                    _Cache.cacheMastRequestOT();
                    _Cache.cacheHistoryApproved();
                    _Cache.cacheMastFlowApprove();
                    _Cache.cacheEmail();

                    _LAMP.MastRequestOTs.Update(requestOT);
                    _LAMP.DetailRequestOTs.UpdateRange(workerList);
                    _LAMP.HistoryApproveds.Update(recentHistoryStep);
                    if (notRejected is true)
                        _LAMP.HistoryApproveds.Add(addHistory);
                    if (notRejected is false)
                        _LAMP.HistoryApproveds.Update(addHistory);

                    _LAMP.SaveChanges();
                    //setting send mail 
                    string mailSender = GlobalVariable.ProgramEmail;
                    string mailReceiver = _Cache.cacheEmail().Where(w => w.emEmail_M365 != null && w.emEmpcode.Trim() == list.empcode.Trim()).FirstOrDefault() is null
                                       ? ""
                                       : _Cache.cacheEmail().Where(w => w.emEmpcode.Trim() == list.empcode.Trim()).FirstOrDefault().emEmail_M365;
                    string mailCC = strMail;
                    string subject = "Request for Over time";
                    string body = "<h2>Test mail</h2>";
                    _callFunc.SendEmail(subject, mailSender, mailReceiver, body, reqProfile, profileApprover, mailCC, requestOT, list.remark);
                    scope.Complete();
                    _Cache.clearCacheMastRequestOT();
                    _Cache.clearCacheDetailRequestOT();
                    _Cache.clearCacheHistoryApproved();

                    return Json(new { icon = "success", title = "สำเร็จ", message = "อนุมัติสำเร็จ -> รอ" + nextStauts.mfSubject + "(คุณ" + requestOT.mrNameApp + ")" });
                }
                catch (Exception ex)
                {
                    { return Json(new { icon = "error", title = "ไม่สำเร็จ", message = ex.Message }); }
                }
                finally { scope.Dispose(); }
            }
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
                        string empcodeApprover = _Cache.cacheEmail().Where(w => w.emEmail_M365 != null && w.emEmail_M365.ToLower().Trim() == otHistory.historyApproveds.htTo.ToLower().Trim()).FirstOrDefault().emEmpcode;
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

                        foreach (string worker in NewWorkerList)
                        {
                            ViewDetailRequestOT workerDetail = JsonConvert.DeserializeObject<ViewDetailRequestOT>(worker);
                            ViewDetailRequestOT oldWorkerDetail = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == req && w.drEmpCode == workerDetail.drEmpCode).FirstOrDefault();
                            ViewAccEMPLOYEE workerProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == workerDetail.drEmpCode).FirstOrDefault();
                            oldWorkerDetail.drNoReq = req;
                            oldWorkerDetail.drEmpCode = workerProfile.EMP_CODE;
                            oldWorkerDetail.drJobCode = workerDetail.drJobCode;
                            oldWorkerDetail.drDateReq = DateReq;
                            oldWorkerDetail.drPriName = workerProfile.PRI_THAI;
                            oldWorkerDetail.drName = workerProfile.EMP_TNAME;
                            oldWorkerDetail.drLastName = workerProfile.LAST_TNAME;
                            oldWorkerDetail.drDivi = workerProfile.DIVI_CODE;
                            oldWorkerDetail.drDept = workerProfile.DEPT_CODE;
                            oldWorkerDetail.drSec = workerProfile.SEC_CODE;
                            oldWorkerDetail.drGrp = workerProfile.GRP_CODE;
                            oldWorkerDetail.drUnit = workerProfile.UNT_CODE;
                            oldWorkerDetail.drSubDirOrInDir = workerProfile.DirOrIndir;
                            await Task.Run(() => _LAMP.DetailRequestOTs.Update(oldWorkerDetail));
                        }

                        //set update history
                        //change mail form to db
                        string strMail = "";
                        if (MailCCs != null)
                            foreach (string item in MailCCs)
                            {
                                strMail += item.Trim() + ",";
                            }

                        ViewHistoryApproved historyRequest = await Task.Run(() => _Cache.cacheHistoryApproved().Where(w => w.htNoReq == req && w.htStep == 1 && w.htStatus != GlobalVariable.StatusApproved).FirstOrDefault());
                        historyRequest.htDateReq = DateReq;
                        historyRequest.htTo = profileApprover.EMP_CODE;
                        historyRequest.htCC = strMail.Length > 0 ? strMail.Substring(0, strMail.Length - 1) : "";
                        historyRequest.htDate = DateReq;
                        historyRequest.htTime = TimeReq;
                        historyRequest.htRemark = otHistory.historyApproveds.htRemark;

                        //error case
                        if (OTDate is null)
                            return Json(new { icon = "error", title = "ไม่สำเร็จ", message = "กรุณาเลือกวันที่จะทำ OT" });

                        await Task.Run(() => _LAMP.MastRequestOTs.Update(mastRequestOT));
                        await Task.Run(() => _LAMP.HistoryApproveds.Update(historyRequest));
                        await _LAMP.SaveChangesAsync();


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

        [HttpPost]
        public async Task<JsonResult> HRApproved([FromBody]req req)
        {
            //connect
            _Cache.cacheMastRequestOT();
            _Cache.cacheDetailRequestOT();
            _Cache.cacheHistoryApproved();
            _Cache.cacheEmail();

            bool hasExistingTransaction = Transaction.Current != null;
            string DateReq = DateTime.Today.ToString("dd/MM/yyyy");
            string TimeReq = DateTime.Now.ToString("HH:mm");
            ViewMastRequestOT requestOT = await Task.Run(() => _LAMP.MastRequestOTs.Where(w => w.mrNoReq == req.no).FirstOrDefault());
            ViewMastFlowApprove lastStepInFlow = _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == requestOT.mrFlow).OrderByDescending(o => o.mfStep).FirstOrDefault();
            ViewAccEMPLOYEE HRProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value).FirstOrDefault();
            ViewAccEMPLOYEE reqProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == requestOT.mrEmpReq).FirstOrDefault();
            List<ViewDetailRequestOT> workerListApproved = await Task.Run(() => _LAMP.DetailRequestOTs.Where(w => w.drNoReq == req.no && w.drStatus.StartsWith(lastStepInFlow.mfSubject)).ToList());
            ViewHistoryApproved recentHistoryStep = await Task.Run(() => _LAMP.HistoryApproveds.Where(w => w.htNoReq == req.no && w.htStep == requestOT.mrStep).FirstOrDefault());
            List<string> qryCC = await Task.Run(() => _LAMP.HistoryApproveds.Where(w => w.htNoReq == req.no && w.htStep < requestOT.mrStep).Select(s => s.htTo).ToList());

            string strMail = "";
            if (qryCC != null)
                foreach (string email in qryCC)
                {
                    string ccemail = _callFunc.FindEmailFromEmpCode(email.Trim());
                    if (ccemail != "")
                        strMail += ccemail.Trim() + ",";
                }
            //update value
            //MastRequestOT
            int nextStep = requestOT.mrStep.Value + 1;
            requestOT.mrStep = nextStep;
            requestOT.mrStatus = GlobalVariable.StatusFinishedST + " โดย " + GlobalVariable.StatusFinishedED;
            requestOT.mrEmpApp = HRProfile.EMP_CODE;
            requestOT.mrNameApp = HRProfile.EMP_TNAME + " " + HRProfile.LAST_TNAME;

            //DetaiRequestOT
            //set approved
            foreach (var row in workerListApproved)
            {
                row.drStatus = GlobalVariable.StatusFinishedST + " โดย " + GlobalVariable.StatusFinishedED;
            }

            //History Update
            recentHistoryStep.htStatus = GlobalVariable.StatusFinishedST + " โดย " + GlobalVariable.StatusFinishedED;

            using (var scope = hasExistingTransaction
                    ? new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled)
                    : new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    _LAMP.MastRequestOTs.Update(requestOT);
                    _LAMP.DetailRequestOTs.UpdateRange(workerListApproved);
                    _LAMP.HistoryApproveds.Update(recentHistoryStep);

                    _LAMP.SaveChanges();

                    //setting send mail 
                    string mailSender = GlobalVariable.ProgramEmail;
                    string mailReceiver = _Cache.cacheEmail().Where(w => w.emEmail_M365 != null && w.emEmpcode.Trim() == reqProfile.EMP_CODE.Trim()).FirstOrDefault() is null
                                       ? ""
                                       : _Cache.cacheEmail().Where(w => w.emEmpcode.Trim() == reqProfile.EMP_CODE.Trim()).FirstOrDefault().emEmail_M365;
                    string mailCC = strMail;
                    string subject = "HR Read : Request for Over time";
                    string body = GlobalVariable.PermissionHCM;
                    _callFunc.SendEmail(subject, mailSender, mailReceiver, body, HRProfile, reqProfile, mailCC, requestOT, "");
                    scope.Complete();
                    _Cache.clearCacheMastRequestOT();
                    _Cache.clearCacheDetailRequestOT();
                    _Cache.clearCacheHistoryApproved();

                    return Json(new { icon = "success", title = "สำเร็จ", message = "บันทึกสำเร็จ -> สามารถดาวน์โหลดไฟล์รายการนี้ได้ที่เมนู หน้าแรก > เอกสาร" });
                }
                catch (Exception ex)
                {
                    { return Json(new { icon = "error", title = "ไม่สำเร็จ", message = ex.Message }); }
                }
                finally { scope.Dispose(); }
            }
        }

        [HttpPost]
        public async Task<JsonResult> HRApprovedSelected([FromBody]List<req> reqs)
        {
            //Error data type truan ไม่ตรง
            bool hasExistingTransaction = Transaction.Current != null;
            List<ViewMastRequestOT> DocSuccessForSendMail = new List<ViewMastRequestOT>();
            int countingDisplay = 0;
            using (var scope = hasExistingTransaction
                        ? new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled)
                        : new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //connect
                _Cache.cacheMastRequestOT();
                _Cache.cacheDetailRequestOT();
                _Cache.cacheHistoryApproved();
                _Cache.cacheEmail();
                try
                {
                    foreach (req req in reqs)
                    {
                        string DateReq = DateTime.Today.ToString("dd/MM/yyyy");
                        string TimeReq = DateTime.Now.ToString("HH:mm");
                        ViewMastRequestOT requestOT = await Task.Run(() => _Cache.cacheMastRequestOT().Where(w => w.mrNoReq == req.no).FirstOrDefault());
                        ViewMastFlowApprove lastStepInFlow = _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == requestOT.mrFlow).OrderByDescending(o => o.mfStep).FirstOrDefault();
                        ViewAccEMPLOYEE HRProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value).FirstOrDefault();
                        ViewAccEMPLOYEE reqProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == requestOT.mrEmpReq).FirstOrDefault();
                        List<ViewDetailRequestOT> workerListApproved = await Task.Run(() => _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == req.no && w.drStatus.StartsWith(lastStepInFlow.mfSubject)).ToList());
                        ViewHistoryApproved recentHistoryStep = await Task.Run(() => _Cache.cacheHistoryApproved().Where(w => w.htNoReq == req.no && w.htStep == requestOT.mrStep).FirstOrDefault());
                        List<string> qryCC = await Task.Run(() => _Cache.cacheHistoryApproved().Where(w => w.htNoReq == req.no && w.htStep < requestOT.mrStep).Select(s => s.htTo).ToList());

                        //update value
                        //MastRequestOT
                        int nextStep = requestOT.mrStep.Value + 1;
                        requestOT.mrDiviReq = _callFunc.TransferDivisionToCodeName(requestOT.mrDiviReq);
                        requestOT.mrDeptReq = _callFunc.TransferDepartmentToCodeName(requestOT.mrDeptReq);
                        requestOT.mrStep = nextStep;
                        requestOT.mrStatus = GlobalVariable.StatusFinishedST + " โดย " + GlobalVariable.StatusFinishedED;
                        requestOT.mrEmpApp = HRProfile.EMP_CODE;
                        requestOT.mrNameApp = HRProfile.EMP_TNAME + " " + HRProfile.LAST_TNAME;

                        //DetaiRequestOT
                        //set approved
                        foreach (var row in workerListApproved)
                        {
                            row.drStatus = GlobalVariable.StatusFinishedST + " โดย " + GlobalVariable.StatusFinishedED;
                        }

                        //History Update
                        recentHistoryStep.htStatus = GlobalVariable.StatusFinishedST + " โดย " + GlobalVariable.StatusFinishedED;

                        _LAMP.MastRequestOTs.Update(requestOT);
                        _LAMP.DetailRequestOTs.UpdateRange(workerListApproved);
                        _LAMP.HistoryApproveds.Update(recentHistoryStep);

                        DocSuccessForSendMail.Add(requestOT);
                    }
                    _LAMP.SaveChanges();
                    scope.Complete();

                    //send mail
                    foreach (ViewMastRequestOT successOT in DocSuccessForSendMail)
                    {
                        string DateReq = DateTime.Today.ToString("dd/MM/yyyy");
                        string TimeReq = DateTime.Now.ToString("HH:mm");
                        ViewMastRequestOT requestOT = await Task.Run(() => _Cache.cacheMastRequestOT().Where(w => w.mrNoReq == successOT.mrNoReq).FirstOrDefault());
                        ViewMastFlowApprove lastStepInFlow = _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == requestOT.mrFlow).OrderByDescending(o => o.mfStep).FirstOrDefault();
                        ViewAccEMPLOYEE HRProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value).FirstOrDefault();
                        ViewAccEMPLOYEE reqProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == requestOT.mrEmpReq).FirstOrDefault();
                        List<ViewDetailRequestOT> workerListApproved = await Task.Run(() => _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == successOT.mrNoReq && w.drStatus.StartsWith(lastStepInFlow.mfSubject)).ToList());
                        ViewHistoryApproved recentHistoryStep = await Task.Run(() => _Cache.cacheHistoryApproved().Where(w => w.htNoReq == successOT.mrNoReq && w.htStep == requestOT.mrStep).FirstOrDefault());
                        List<string> qryCC = await Task.Run(() => _Cache.cacheHistoryApproved().Where(w => w.htNoReq == successOT.mrNoReq && w.htStep < requestOT.mrStep).Select(s => s.htTo).ToList());

                        string strMail = "";
                        if (qryCC != null)
                            foreach (string email in qryCC)
                            {
                                string ccemail = _callFunc.FindEmailFromEmpCode(email.Trim());
                                if (ccemail != "")
                                    strMail += ccemail.Trim() + ",";
                            }

                        //setting send mail 
                        string mailSender = GlobalVariable.ProgramEmail;
                        string mailReceiver = _Cache.cacheEmail().Where(w => w.emEmail_M365 != null && w.emEmpcode.Trim() == reqProfile.EMP_CODE.Trim()).FirstOrDefault() is null
                                           ? ""
                                           : _Cache.cacheEmail().Where(w => w.emEmpcode.Trim() == reqProfile.EMP_CODE.Trim()).FirstOrDefault().emEmail_M365;
                        string mailCC = strMail;
                        string subject = "HR Read : Request for Over time";
                        string body = GlobalVariable.PermissionHCM;
                        _callFunc.SendEmail(subject, mailSender, mailReceiver, body, HRProfile, reqProfile, mailCC, requestOT, "");
                    }

                    _Cache.clearCacheMastRequestOT();
                    _Cache.clearCacheDetailRequestOT();
                    _Cache.clearCacheHistoryApproved();

                    return await Task.Run(() => Json(new { icon = "success", title = "สำเร็จ", message = "บันทึกสำเร็จ -> สามารถดาวน์โหลดไฟล์รายการนี้ได้ที่เมนู หน้าแรก > เอกสาร", count = countingDisplay }));
                }
                catch (Exception ex)
                {
                    { return await Task.Run(() => Json(new { icon = "error", title = "ไม่สำเร็จ", message = ex.Message, count = countingDisplay })); }
                }
                finally { scope.Dispose(); }
            }
        }

        [HttpPost]
        public async Task<JsonResult> Reject([FromBody]MutiApprove list)
        {
            bool hasExistingTransaction = Transaction.Current != null;
            string DateReq = DateTime.Today.ToString("dd/MM/yyyy");
            string TimeReq = DateTime.Now.ToString("HH:mm");
            ViewMastRequestOT requestOT = await Task.Run(() => _LAMP.MastRequestOTs.Where(w => w.mrNoReq == list.reqNo).FirstOrDefault());
            int previosStep = requestOT.mrStep.Value - 1;
            ViewHistoryApproved recentHistoryStep = await Task.Run(() => _LAMP.HistoryApproveds.Where(w => w.htNoReq == list.reqNo && w.htStep == requestOT.mrStep).FirstOrDefault());
            ViewAccEMPLOYEE reqProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value).FirstOrDefault();
            ViewAccEMPLOYEE profileApprover = previosStep == 0
                                            ? _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == requestOT.mrEmpReq.Trim()).FirstOrDefault()
                                            : _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == recentHistoryStep.htFrom.Trim()).FirstOrDefault();
            List<ViewDetailRequestOT> workerList = await Task.Run(() => _LAMP.DetailRequestOTs.Where(w => (w.drNoReq == list.reqNo) && !(w.drStatus.StartsWith(GlobalVariable.StatusRejected) && w.drStatus != null)).ToList());
            ViewMastFlowApprove previosStauts = await Task.Run(() => _Cache.cacheMastFlowApprove().Where(w => (w.mfFlowNo == requestOT.mrFlow) && int.Parse(w.mfStep.Value.ToString()) == (requestOT.mrStep)).FirstOrDefault());
            List<string> qryCC = await Task.Run(() => _LAMP.HistoryApproveds.Where(w => w.htNoReq == list.reqNo && w.htStep == requestOT.mrStep).Select(s => s.htFrom).ToList());
            string strMail = "";
            if (qryCC != null)
                foreach (string email in qryCC)
                {
                    string ccemail = _callFunc.FindEmailFromEmpCode(email.Trim());
                    if (ccemail != "")
                        strMail += ccemail.Trim() + ",";
                }
            //update value
            //MastRequestOT

            requestOT.mrStep = previosStep;
            requestOT.mrStatus = GlobalVariable.StatusRejected + ",รอ" + previosStauts.mfSubject + "อีกครั้ง";
            requestOT.mrEmpApp = profileApprover.EMP_CODE;
            requestOT.mrNameApp = profileApprover.EMP_TNAME + " " + profileApprover.LAST_TNAME;

            //DetaiRequestOT
            //set all rejected
            foreach (var row in workerList)
            {
                row.drStatus = GlobalVariable.StatusRejected + " โดย คุณ" + reqProfile.EMP_TNAME + " " + reqProfile.LAST_TNAME;
            }

            //HistoryStep
            recentHistoryStep.htStatus = GlobalVariable.StatusRejected + " โดย คุณ" + reqProfile.EMP_TNAME + " " + reqProfile.LAST_TNAME;
            recentHistoryStep.htRemark = list.remark;
            using (var scope = hasExistingTransaction
                    ? new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled)
                    : new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    _LAMP.MastRequestOTs.Update(requestOT);
                    _LAMP.DetailRequestOTs.UpdateRange(workerList);
                    _LAMP.HistoryApproveds.Update(recentHistoryStep);
                    _LAMP.SaveChanges();

                    //setting send mail 
                    string mailSender = GlobalVariable.ProgramEmail;
                    string mailReceiver = _Cache.cacheEmail().Where(w => w.emEmail_M365 != null && w.emEmpcode.Trim() == profileApprover.EMP_CODE.Trim()).FirstOrDefault() is null
                                       ? ""
                                       : _Cache.cacheEmail().Where(w => w.emEmpcode.Trim() == profileApprover.EMP_CODE.Trim()).FirstOrDefault().emEmail_M365;
                    string mailCC = strMail;
                    string subject = "Request for Over time : " + requestOT.mrNoReq + " Rejected";
                    string body = GlobalVariable.StatusRejected;
                    _callFunc.SendEmail(subject, mailSender, mailReceiver, body, reqProfile, profileApprover, mailCC, requestOT, list.remark);
                    scope.Complete();
                    _Cache.clearCacheMastRequestOT();
                    _Cache.clearCacheDetailRequestOT();
                    _Cache.clearCacheHistoryApproved();

                    return Json(new { icon = "success", title = "สำเร็จ", message = "ไม่อนุมัติ -> รอ" + previosStauts.mfSubject + " อีกครั้ง(คุณ" + requestOT.mrNameApp + ")" });
                }
                catch (Exception ex)
                {
                    { return Json(new { icon = "error", title = "ไม่สำเร็จ", message = ex.Message }); }
                }
                finally { scope.Dispose(); }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ptvHeadEmailForm()
        {
            _Cache.clearCacheMastRequestOT();
            _Cache.cacheAccEmployee();
            string emp = User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value?.ToString();
            string grp = User.Claims.FirstOrDefault(s => s.Type == "Group").Value?.ToString();
            string sec = User.Claims.FirstOrDefault(s => s.Type == "Section").Value?.ToString();
            string dpt = User.Claims.FirstOrDefault(s => s.Type == "Department").Value?.ToString();
            string pst = User.Claims.FirstOrDefault(s => s.Type == "Position").Value?.ToString();
            string perm = User.Claims.FirstOrDefault(s => s.Type == "Permission").Value?.ToString();
            MultiSendEmail emailForm = new MultiSendEmail();
            emailForm.mastRequestOT = new ViewMastRequestOT();
            emailForm.historyApproveds = new ViewHistoryApproved();
            emailForm.nextStep = new ViewMastFlowApprove();
            ViewAccEMPLOYEE profileEmpNext = new ViewAccEMPLOYEE();

            emailForm.mastRequestOT.mrPositionReq = pst;
            bool perAdmin = perm.ToUpper() == GlobalVariable.perAdmin.ToUpper();
            bool perDM = pst.ToUpper() == GlobalVariable.spDM;

            if (perDM == false && perAdmin == false)
                return RedirectToAction("Index","ErrorCase");
            //return RedirectToAction("~/Views/Approval/SendEmail/ptvHeadEmailForm.cshtml");


            int recentPst = perAdmin ? _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == "1" && w.mfPermission.ToUpper() == perm.ToUpper()).FirstOrDefault().mfStep.Value 
                            : perDM ? _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == "1" && w.mfPermission == pst).FirstOrDefault().mfStep.Value: 0;
            int stepNext = recentPst + 1;
            int stepLast = Task.Run(() => _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == "1").Select(s => s.mfStep).OrderByDescending(o => o).FirstOrDefault().Value).Result;
            int stepActual = stepNext > stepLast ? stepLast : stepNext;

            emailForm.nextStep = Task.Run(() => _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == "1" && w.mfStep == (stepActual)).FirstOrDefault()).Result;

            ///========================
            //if (_Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == "1" && w.mfStep == reqOT.mrStep).FirstOrDefault() != null)
            //    nextPosition = _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == reqOT.mrFlow && w.mfStep == (reqOT.mrStep + 1)).FirstOrDefault().mfPermission;

            profileEmpNext = _callFunc.EmployeeByPositionList(pst, grp, sec, dpt);

            List<ViewMastUserApprove> userPerAdmin = _Cache.cacheMastUserApprove().Where(w => w.muPosition == GlobalVariable.StepTitleAdmin && w.muDeptCode == _callFunc.TransferDepartmentToCodeName(dpt) && w.muCheck == GlobalVariable.statusOnline).ToList();
            List<ViewMastUserApprove> userPerHCM = _Cache.cacheMastUserApprove().Where(w => w.muPosition == GlobalVariable.StepTitleHCM && w.muCheck == GlobalVariable.statusOnline).ToList();
            if (emailForm.nextStep.mfSubject.Contains(GlobalVariable.StepTitleAdmin) && !(userPerAdmin.FirstOrDefault() is null))
                profileEmpNext = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == userPerAdmin.FirstOrDefault().muEmpCode).FirstOrDefault();
            if (emailForm.nextStep.mfSubject.Contains(GlobalVariable.StepTitleHCM) && !(userPerHCM.FirstOrDefault() is null))
                profileEmpNext = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == userPerHCM.FirstOrDefault().muEmpCode).FirstOrDefault();

            emailForm.historyApproveds.htTo = profileEmpNext is null ? "" : profileEmpNext.EMP_CODE;
            emailForm.FullNameMailTo = profileEmpNext is null ? "" : profileEmpNext.EMP_TNAME + " " + profileEmpNext.LAST_TNAME;
            emailForm.historyApproveds.htCC = "";

            return await Task.Run(() => PartialView("~/Views/Approval/SendEmail/ptvHeadEmailForm.cshtml", emailForm));
        }

        [HttpPost]
        public async Task<IActionResult> ptvExportHelper()
        {
            string emp = User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value?.ToString();
            string grp = User.Claims.FirstOrDefault(s => s.Type == "Group").Value?.ToString();
            string sec = User.Claims.FirstOrDefault(s => s.Type == "Section").Value?.ToString();
            string dpt = User.Claims.FirstOrDefault(s => s.Type == "Department").Value?.ToString();
            string pst = User.Claims.FirstOrDefault(s => s.Type == "Position").Value?.ToString();
            string perm = User.Claims.FirstOrDefault(s => s.Type == "Permission").Value?.ToString();

            return await Task.Run(() => PartialView("~/Views/Approval/HRRead/ptvExportHelper.cshtml"));
        }


        [HttpPost]
        public async Task<PartialViewResult> ptvEmailForm([FromBody]req req)
        {
            _Cache.clearCacheMastRequestOT();
            _Cache.cacheAccEmployee();
            string nextPosition = "";
            MultiSendEmail emailForm = new MultiSendEmail();
            emailForm.historyApproveds = new ViewHistoryApproved();
            emailForm.nextStep = new ViewMastFlowApprove();
            ViewAccEMPLOYEE profileEmpNext = new ViewAccEMPLOYEE();

            ViewMastRequestOT reqOT = Task.Run(() => _Cache.cacheMastRequestOT().Where(w => w.mrNoReq == req.no).FirstOrDefault()).Result;
            emailForm.mastRequestOT = reqOT;

            ViewMastFlowApprove approverPosition = _Cache.cacheMastFlowApprove().Where(w => w.mfStep == (reqOT.mrStep + 1) && w.mfFlowNo == reqOT.mrFlow).FirstOrDefault();

            emailForm.mastRequestOT.mrPositionReq = approverPosition is null ? User.Claims.FirstOrDefault(s => s.Type == "Position").Value?.ToString() : approverPosition.mfPermission;

            int stepNext = emailForm.mastRequestOT.mrStep.Value + 2;
            int stepLast = Task.Run(() => _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == emailForm.mastRequestOT.mrFlow).Select(s => s.mfStep).OrderByDescending(o => o).FirstOrDefault().Value).Result;
            int stepActual = stepNext > stepLast ? stepLast : stepNext;

            emailForm.nextStep = Task.Run(() => _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == emailForm.mastRequestOT.mrFlow && w.mfStep == (stepActual)).FirstOrDefault()).Result;

            ///========================
            if (_Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == reqOT.mrFlow && w.mfStep == reqOT.mrStep).FirstOrDefault() != null)
                nextPosition = _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == reqOT.mrFlow && w.mfStep == (reqOT.mrStep + 1)).FirstOrDefault().mfPermission;

            profileEmpNext = _callFunc.EmployeeByPositionList(nextPosition, reqOT.mrGrpReq, reqOT.mrSecReq, reqOT.mrDeptReq);

            List<ViewMastUserApprove> userPerAdmin = _Cache.cacheMastUserApprove().Where(w => w.muPosition == GlobalVariable.StepTitleAdmin && w.muDeptCode == emailForm.mastRequestOT.mrDeptReq && w.muCheck == GlobalVariable.statusOnline).ToList();
            List<ViewMastUserApprove> userPerHCM = _Cache.cacheMastUserApprove().Where(w => w.muPosition == GlobalVariable.StepTitleHCM && w.muCheck == GlobalVariable.statusOnline).ToList();
            if (emailForm.nextStep.mfSubject.Contains(GlobalVariable.StepTitleAdmin) && !(userPerAdmin.FirstOrDefault() is null))
                profileEmpNext = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == userPerAdmin.FirstOrDefault().muEmpCode).FirstOrDefault();
            if (emailForm.nextStep.mfSubject.Contains(GlobalVariable.StepTitleHCM) && !(userPerHCM.FirstOrDefault() is null))
                profileEmpNext = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == userPerHCM.FirstOrDefault().muEmpCode).FirstOrDefault();

            emailForm.historyApproveds.htTo = profileEmpNext is null ? "" : profileEmpNext.EMP_CODE;
            emailForm.FullNameMailTo = profileEmpNext is null ? "" : profileEmpNext.EMP_TNAME + " " + profileEmpNext.LAST_TNAME;

            emailForm.historyApproveds.htCC = "";

            return await Task.Run(() => PartialView("~/Views/Approval/SendEmail/_ptvEmailForm.cshtml", emailForm));
        }

        [HttpPost]
        public async Task<PartialViewResult> ptvRejectForm([FromBody]req req)
        {

            MultiSendEmail emailForm = new MultiSendEmail();
            emailForm.historyApproveds = new ViewHistoryApproved();
            emailForm.nextStep = new ViewMastFlowApprove();

            emailForm.mastRequestOT = Task.Run(() => _Cache.cacheMastRequestOT().Where(w => w.mrNoReq == req.no).FirstOrDefault()).Result;
            //emailForm.mastRequestOT.mrPositionReq = User.Claims.FirstOrDefault(s => s.Type == "Position").Value?.ToString();

            return await Task.Run(() => PartialView("~/Views/Approval/SendEmail/_ptvRejectForm.cshtml", emailForm));
        }


        public async Task<PartialViewResult> DisplayNewlate()
        {
            WebClient request = new WebClient();
            string imgPath = GlobalVariable.imgPath;
            string EmpCode = User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value?.ToString();
            MultiNewLate docNewLate = new MultiNewLate();
            docNewLate.docList = new List<MultiDocDetails>();
            docNewLate.mastFlow = _Cache.cacheMastFlowApprove().ToList();
            docNewLate.mastJobs = _Cache.cacheMastJob().ToList();
            _Cache.clearCacheMastRequestOT();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT().Where(w => w.mrEmpApp == EmpCode && !w.mrStatus.StartsWith(GlobalVariable.StatusFinishedST))
                                                                            .OrderByDescending(o => DateTime.Parse(o.mrDateReq.Substring(o.mrDateReq.Length - 4) + "/"
                                                                                                  + o.mrDateReq.Substring(3, 2) + "/"
                                                                                                  + o.mrDateReq.Substring(0, 2)))
                                                                            .ThenByDescending(o => o.mrNoReq.Substring(o.mrNoReq.Length - 2)).ToList())
            {
                items.mrDiviReq = _callFunc.TransferCodeNameToDivision(items.mrDiviReq);
                items.mrDeptReq = _callFunc.TransferCodeNameToDepartment(items.mrDeptReq);

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
                            byte[] imgFile = request.DownloadData(imgPath);
                            string file64String = Convert.ToBase64String(imgFile);
                            string imgDataURL = string.Format("data:image/jpg;base64,{0}", file64String);
                            workerImages workerImage = new workerImages()
                            {
                                empcode = workerEmpcode,
                                image = imgDataURL,
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
                docNewLate.docList.Add(docDetails);
            }

            return await Task.Run(() => PartialView("~/Views/Approval/_DisplayNewlate.cshtml", docNewLate));
        }




        #region function

        public async Task<bool> _Reject(ApproveSelected docRejecting, string remark) {
            string DateReq = DateTime.Today.ToString("dd/MM/yyyy");
            string TimeReq = DateTime.Now.ToString("HH:mm");
            ViewMastRequestOT requestOT = await Task.Run(() => _LAMP.MastRequestOTs.Where(w => w.mrNoReq == docRejecting.reqNo).FirstOrDefault());
            int previosStep = requestOT.mrStep.Value - 1;
            ViewHistoryApproved recentHistoryStep = await Task.Run(() => _LAMP.HistoryApproveds.Where(w => w.htNoReq == docRejecting.reqNo && w.htStep == requestOT.mrStep).FirstOrDefault());
            ViewAccEMPLOYEE reqProfile = _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value).FirstOrDefault();
            ViewAccEMPLOYEE profileApprover = previosStep == 0
                                            ? _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == requestOT.mrEmpReq.Trim()).FirstOrDefault()
                                            : _Cache.cacheAccEmployee().Where(w => w.EMP_CODE == recentHistoryStep.htFrom.Trim()).FirstOrDefault();
            List<ViewDetailRequestOT> workerList = await Task.Run(() => _LAMP.DetailRequestOTs.Where(w => (w.drNoReq == docRejecting.reqNo) && !(w.drStatus.StartsWith(GlobalVariable.StatusRejected) && w.drStatus != null)).ToList());
            ViewMastFlowApprove previosStauts = await Task.Run(() => _Cache.cacheMastFlowApprove().Where(w => (w.mfFlowNo == requestOT.mrFlow) && int.Parse(w.mfStep.Value.ToString()) == (requestOT.mrStep)).FirstOrDefault());
            List<string> qryCC = await Task.Run(() => _LAMP.HistoryApproveds.Where(w => w.htNoReq == docRejecting.reqNo && w.htStep == requestOT.mrStep).Select(s => s.htFrom).ToList());
            string strMail = "";
            if (qryCC != null)
                foreach (string email in qryCC)
                {
                    string ccemail = _callFunc.FindEmailFromEmpCode(email.Trim());
                    if (ccemail != "")
                        strMail += ccemail.Trim() + ",";
                }
            //update value
            //MastRequestOT

            requestOT.mrStep = previosStep;
            requestOT.mrStatus = GlobalVariable.StatusRejected + ",รอ" + previosStauts.mfSubject + "อีกครั้ง";
            requestOT.mrEmpApp = profileApprover.EMP_CODE;
            requestOT.mrNameApp = profileApprover.EMP_TNAME + " " + profileApprover.LAST_TNAME;

            //DetaiRequestOT
            //set all rejected
            foreach (var row in workerList)
            {
                row.drStatus = GlobalVariable.StatusRejected + " โดย คุณ" + reqProfile.EMP_TNAME + " " + reqProfile.LAST_TNAME;
            }

            //HistoryStep
            recentHistoryStep.htStatus = GlobalVariable.StatusRejected + " โดย คุณ" + reqProfile.EMP_TNAME + " " + reqProfile.LAST_TNAME;
            recentHistoryStep.htRemark = remark;
                try
                {
                    _LAMP.MastRequestOTs.Update(requestOT);
                    _LAMP.DetailRequestOTs.UpdateRange(workerList);
                    _LAMP.HistoryApproveds.Update(recentHistoryStep);

                    return true;
                }
                catch(Exception ex)
                {
                    return false;
                }
        }

        public JsonResult suggestMails(string q, string pst, string req)
        {
            string department = "";
            ViewMastRequestOT otModel = new ViewMastRequestOT();
            if (req != null)
                otModel = _Cache.cacheMastRequestOT().Where(w => w.mrNoReq == req).FirstOrDefault();

            if (otModel != null)
            {
                department = otModel.mrDeptReq;
                pst = _Cache.cacheMastFlowApprove().Where(w => w.mfFlowNo == otModel.mrFlow && w.mfStep == otModel.mrStep + 1).Select(s => s.mfPermission).FirstOrDefault();
            }

            List<autocompleteEmail> emails = new List<autocompleteEmail>();
            string fullname = "";
            List<ViewAccEMPLOYEE> filterAccEmp = _callFunc.TransPositionToLevel(pst) == 0
                                                ? _Cache.cacheAccEmployee()
                                                  .Where(w => !string.IsNullOrEmpty(w.DEPT_CODE) && !string.IsNullOrEmpty(w.PRI_THAI) && !string.IsNullOrEmpty(w.EMP_TNAME) && !string.IsNullOrEmpty(w.LAST_TNAME) && w.DEPT_CODE == _callFunc.TransferDepartmentToCodeName(department))
                                                  .ToList()
                                                : _Cache.cacheAccEmployee()
                                                  .Where(w => !string.IsNullOrEmpty(w.DEPT_CODE) && !string.IsNullOrEmpty(w.PRI_THAI) && !string.IsNullOrEmpty(w.EMP_TNAME) && !string.IsNullOrEmpty(w.LAST_TNAME)
                                                                && (_callFunc.TransPositionToLevel(w.POS_CODE) >= _callFunc.TransPositionToLevel(pst)) && w.DEPT_CODE == _callFunc.TransferDepartmentToCodeName(department))
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
                fullname = "คุณ" + filterName.EMP_TNAME + " " + filterName.LAST_TNAME;

            return Json(new { emails, fullname });
        }

        #endregion
    }
}