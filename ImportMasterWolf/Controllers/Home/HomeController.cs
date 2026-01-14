using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ImportMasterWolf.Models.Approval;
using ImportMasterWolf.Models.Canvas;
using ImportMasterWolf.Models.Common;
using ImportMasterWolf.Models.DBConnect;
using ImportMasterWolf.Models.Table.LAMP;

namespace ImportMasterWolf.Controllers.Home
{
    public class HomeController : Controller
    {
        private LAMP _LAMP;
        private HRMS _HRMS;
        private IT _IT;
        private CacheSettingController _Cache;
        private FunctionsController _callFunc;
        public HomeController(LAMP lamp, HRMS hrms, IT it, CacheSettingController cacheController, FunctionsController callfunction)
        {
            _LAMP = lamp;
            _HRMS = hrms;
            _IT = it;
            _Cache = cacheController;
            _callFunc = callfunction;
        }

        [Authorize(Policy = "Checked")]
        public IActionResult Index(Class @class)
        {
            return View("Index", @class);
        }

        [HttpPost]
        public async Task<PartialViewResult> DisplayHour()
        {
            string DateST = DateTime.Today.ToString("yyyyMMdd");
            string DateED = DateTime.Today.ToString("yyyyMMdd");
            MultiNewLate docNewLate = new MultiNewLate();
            docNewLate.docList = new List<MultiDocDetails>();
            docNewLate.mastFlow = _Cache.cacheMastFlowApprove().ToList();
            docNewLate.mastJobs = _Cache.cacheMastJob().ToList();
            _Cache.clearCacheMastRequestOT();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT().Where(w => w.mrOTDate != null && w.mrOTDate.Length > 1 && int.Parse(w.mrOTDate.Substring(w.mrOTDate.Length - 4)
                                                                                                  + w.mrOTDate.Substring(3, 2)
                                                                                                  + w.mrOTDate.Substring(0, 2)) >= int.Parse(DateST)
                                                                                                  && int.Parse(w.mrOTDate.Substring(w.mrOTDate.Length - 4)
                                                                                                  + w.mrOTDate.Substring(3, 2)
                                                                                                  + w.mrOTDate.Substring(0, 2)) <= int.Parse(DateED)
                                                                                                  && (w.mrStep
                                                                                                      >= _Cache.cacheMastFlowApprove().Where(wf => wf.mfFlowNo == w.mrFlow).OrderByDescending(o => o.mfStep).Select(s => s.mfStep).FirstOrDefault().Value - 1))
                                                                            .OrderByDescending(o => DateTime.Parse(o.mrOTDate.Substring(o.mrOTDate.Length - 4) + "/"
                                                                                                  + o.mrOTDate.Substring(3, 2) + "/"
                                                                                                  + o.mrOTDate.Substring(0, 2)))
                                                                            .ThenByDescending(o => o.mrNoReq.Substring(o.mrNoReq.Length - 2)).ToList())
            {
                items.mrDiviReq = _callFunc.TransferCodeNameToDivision(items.mrDiviReq);
                items.mrDeptReq = _callFunc.TransferCodeNameToDepartment(items.mrDeptReq);
                MultiDocDetails docDetails = new MultiDocDetails();
                docDetails.requestOT = items;
                docDetails.workerList = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == items.mrNoReq).ToList();
                docDetails.stepHistory = _Cache.cacheHistoryApproved().Where(w => w.htNoReq == items.mrNoReq).ToList();

                docNewLate.docList.Add(docDetails);
            }
            return await Task.Run(() => PartialView("_DisplayHour", docNewLate));
        }

        [HttpPost]
        public async Task<PartialViewResult> DisplayDocument()
        {
            string DateST = DateTime.Today.ToString("yyyyMMdd");
            string DateED = DateTime.Today.ToString("yyyyMMdd");
            MultiNewLate docNewLate = new MultiNewLate();
            docNewLate.docList = new List<MultiDocDetails>();
            docNewLate.mastFlow = await Task.Run(() => _Cache.cacheMastFlowApprove().ToList());
            docNewLate.mastJobs = await Task.Run(() => _Cache.cacheMastJob().ToList());
            _Cache.clearCacheMastRequestOT();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT().Where(w => w.mrStatus.StartsWith(GlobalVariable.StatusFinishedST) && int.Parse(w.mrDateReq.Substring(w.mrDateReq.Length - 4)
                                                                                                  + w.mrDateReq.Substring(3, 2)
                                                                                                  + w.mrDateReq.Substring(0, 2)) >= int.Parse(DateST)
                                                                                                  && int.Parse(w.mrDateReq.Substring(w.mrDateReq.Length - 4)
                                                                                                  + w.mrDateReq.Substring(3, 2)
                                                                                                  + w.mrDateReq.Substring(0, 2)) <= int.Parse(DateED))
                                                                            .OrderByDescending(o => DateTime.Parse(o.mrDateReq.Substring(o.mrDateReq.Length - 4) + "/"
                                                                                                  + o.mrDateReq.Substring(3, 2) + "/"
                                                                                                  + o.mrDateReq.Substring(0, 2)))
                                                                            .ThenByDescending(o => o.mrNoReq.Substring(o.mrNoReq.Length - 2)).ToList())
            {
                items.mrDiviReq = _callFunc.TransferCodeNameToDivision(items.mrDiviReq);
                items.mrDeptReq = _callFunc.TransferCodeNameToDepartment(items.mrDeptReq);
                MultiDocDetails docDetails = new MultiDocDetails();
                docDetails.requestOT = items;
                docDetails.workerList = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == items.mrNoReq).ToList();
                docDetails.stepHistory = _Cache.cacheHistoryApproved().Where(w => w.htNoReq == items.mrNoReq).ToList();

                docNewLate.docList.Add(docDetails);
            }
            return await Task.Run(() => PartialView("_DisplayDocument", docNewLate));
        }

        [HttpPost]
        public async Task<PartialViewResult> DisplayFollow()
        {
            string DateST = DateTime.Today.ToString("yyyyMMdd");
            string DateED = DateTime.Today.ToString("yyyyMMdd");
            MultiNewLate docNewLate = new MultiNewLate();
            docNewLate.docList = new List<MultiDocDetails>();
            docNewLate.mastFlow = await Task.Run(() => _Cache.cacheMastFlowApprove().ToList());
            docNewLate.mastJobs = await Task.Run(() => _Cache.cacheMastJob().ToList());
            _Cache.clearCacheMastRequestOT();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT()
                                                .Where(w => !w.mrStatus.StartsWith(GlobalVariable.StatusDraft) && int.Parse(w.mrDateReq.Substring(w.mrDateReq.Length - 4)
                                                                                                  + w.mrDateReq.Substring(3, 2)
                                                                                                  + w.mrDateReq.Substring(0, 2)) >= int.Parse(DateST)
                                                                                                  && int.Parse(w.mrDateReq.Substring(w.mrDateReq.Length - 4)
                                                                                                  + w.mrDateReq.Substring(3, 2)
                                                                                                  + w.mrDateReq.Substring(0, 2)) <= int.Parse(DateED))
                                                .OrderByDescending(o => DateTime.Parse(o.mrDateReq.Substring(o.mrDateReq.Length - 4) + "/"
                                                                                                  + o.mrDateReq.Substring(3, 2) + "/"
                                                                                                  + o.mrDateReq.Substring(0, 2)))
                                                .ThenByDescending(o => o.mrNoReq.Substring(o.mrNoReq.Length - 2)).ToList())
            {
                items.mrDiviReq = _callFunc.TransferCodeNameToDivision(items.mrDiviReq);
                items.mrDeptReq = _callFunc.TransferCodeNameToDepartment(items.mrDeptReq);
                MultiDocDetails docDetails = new MultiDocDetails();
                docDetails.requestOT = items;
                docDetails.workerList = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == items.mrNoReq).ToList();
                docDetails.stepHistory = _Cache.cacheHistoryApproved().Where(w => w.htNoReq == items.mrNoReq).ToList();

                docNewLate.docList.Add(docDetails);
            }
            return await Task.Run(() => PartialView("_DisplayFollow", docNewLate));
        }

        [HttpPost]
        public async Task<PartialViewResult> DisplayGraph()
        {
            
            return await Task.Run(() => PartialView("_DisplayGraph"));
        }

        [HttpPost]
        public async Task<JsonResult> qryDataToCanvas()
        {
            string ThisYear = DateTime.Today.Year.ToString();
            List<List<DataPoint>> qtyLamps = new List<List<DataPoint>>();
            List<DataPoint> dataPoints1 = new List<DataPoint>();

            MultiNewLate docNewLate = new MultiNewLate();
            docNewLate.docList = new List<MultiDocDetails>();
            docNewLate.mastFlow = await Task.Run(() => _Cache.cacheMastFlowApprove().ToList());
            docNewLate.mastJobs = await Task.Run(() => _Cache.cacheMastJob().ToList());
            _Cache.clearCacheMastRequestOT();
            for (int runningmonth = 1; runningmonth <= 12; runningmonth++)
            {
                int sumWorker = 0;
                int calcWorker = 0;
                int sumOTMinute = 0;
                int sumHourInt = 0;
                string sumHour = "";
                List<ViewMastRequestOT> otRequests = _Cache.cacheMastRequestOT().Where(w => w.mrDateReq.EndsWith(runningmonth.ToString().PadLeft(2, '0') + "/" + ThisYear)
                                                                                            && (w.mrStep >= _Cache.cacheMastFlowApprove().Where(wf => wf.mfFlowNo == w.mrFlow).OrderByDescending(o => o.mfStep).Select(s => s.mfStep).FirstOrDefault().Value - 1)).ToList();
                if (otRequests != null)
                    if (otRequests.Count > 0)
                        foreach (ViewMastRequestOT otRequest in otRequests)
                        {
                            int minuteST = 0, minuteED = 0;
                            if ((otRequest.mrOTTimeSt_Before != null && otRequest.mrOTTimeSt_Before != "") && (otRequest.mrOTTimeEd_Before != null && otRequest.mrOTTimeEd_Before != ""))
                            {
                                minuteST += (int.Parse(otRequest.mrOTTimeSt_Before.Split(":")[0]) * 60) + int.Parse(otRequest.mrOTTimeSt_Before.Split(":")[1]);
                                minuteED += (int.Parse(otRequest.mrOTTimeEd_Before.Split(":")[0]) * 60) + int.Parse(otRequest.mrOTTimeEd_Before.Split(":")[1]);
                            }
                            if ((otRequest.mrOTTimeSt_During != null && otRequest.mrOTTimeSt_During != "") && (otRequest.mrOTTimeEd_During != null && otRequest.mrOTTimeEd_During != ""))
                            {
                                minuteST += (int.Parse(otRequest.mrOTTimeSt_During.Split(":")[0]) * 60) + int.Parse(otRequest.mrOTTimeSt_During.Split(":")[1]);
                                minuteED += (int.Parse(otRequest.mrOTTimeEd_During.Split(":")[0]) * 60) + int.Parse(otRequest.mrOTTimeEd_During.Split(":")[1]);
                            }
                            if ((otRequest.mrOTTimeSt_After != null && otRequest.mrOTTimeSt_After != "") && (otRequest.mrOTTimeEd_After != null && otRequest.mrOTTimeEd_After != ""))
                            {
                                minuteST += (int.Parse(otRequest.mrOTTimeSt_After.Split(":")[0]) * 60) + int.Parse(otRequest.mrOTTimeSt_After.Split(":")[1]);
                                minuteED += (int.Parse(otRequest.mrOTTimeEd_After.Split(":")[0]) * 60) + int.Parse(otRequest.mrOTTimeEd_After.Split(":")[1]);
                            }

                            calcWorker = _Cache.cacheDetailRequestOT().Where(w => w.drStatus != null && !w.drStatus.StartsWith(GlobalVariable.StatusRejected) && w.drNoReq == otRequest.mrNoReq).Count();
                            sumWorker = sumWorker + calcWorker;
                            if (minuteED < minuteST)
                            {
                                sumOTMinute += (((24 * 60) - minuteST) + minuteED) * calcWorker;
                            }
                            else
                            {
                                sumOTMinute += ((minuteED - minuteST) * calcWorker);
                            }
                            string minuteString = sumOTMinute % 60 == 0 ? "" : (sumOTMinute % 60).ToString() + " นาที";
                            sumHour = (sumOTMinute / 60).ToString() + " ชั่วโมง " + minuteString;
                            sumHourInt = sumOTMinute / 60;
                        }

                dataPoints1.Add(new DataPoint(_callFunc.TransNumberToMonth(runningmonth), sumHourInt));
            }
            return await Task.Run(() => Json(JsonConvert.SerializeObject(dataPoints1))); 
        }

        [HttpPost]
        public async Task<PartialViewResult> SearchHour([FromBody]searchbydate searchbydate)
        {
            string DateST = DateTime.Parse(searchbydate.start).ToString("yyyyMMdd");
            string DateED = DateTime.Parse(searchbydate.end).ToString("yyyyMMdd");
            MultiNewLate docNewLate = new MultiNewLate();
            docNewLate.docList = new List<MultiDocDetails>();
            docNewLate.mastFlow = await Task.Run(() => _Cache.cacheMastFlowApprove().ToList());
            docNewLate.mastJobs = await Task.Run(() => _Cache.cacheMastJob().ToList());
            _Cache.clearCacheMastRequestOT();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT()
                                                .Where(w => w.mrOTDate != null && w.mrOTDate.Length > 1 && int.Parse(w.mrOTDate.Substring(w.mrOTDate.Length - 4)
                                                                                                  + w.mrOTDate.Substring(3, 2)
                                                                                                  + w.mrOTDate.Substring(0, 2)) >= int.Parse(DateST)
                                                                                                  && int.Parse(w.mrOTDate.Substring(w.mrOTDate.Length - 4)
                                                                                                  + w.mrOTDate.Substring(3, 2)
                                                                                                  + w.mrOTDate.Substring(0, 2)) <= int.Parse(DateED)
                                                                                                  && (w.mrStep >= _Cache.cacheMastFlowApprove().Where(wf => wf.mfFlowNo == w.mrFlow).OrderByDescending(o => o.mfStep).Select(s => s.mfStep).FirstOrDefault().Value - 1))
                                                .OrderByDescending(o => DateTime.Parse(o.mrOTDate.Substring(o.mrOTDate.Length - 4) + "/"
                                                                                                  + o.mrOTDate.Substring(3, 2) + "/"
                                                                                                  + o.mrOTDate.Substring(0, 2)))
                                                .ThenByDescending(o => o.mrNoReq.Substring(o.mrNoReq.Length - 2)).ToList())
            {
                items.mrDiviReq = _callFunc.TransferCodeNameToDivision(items.mrDiviReq);
                items.mrDeptReq = _callFunc.TransferCodeNameToDepartment(items.mrDeptReq);
                MultiDocDetails docDetails = new MultiDocDetails();
                docDetails.requestOT = items;
                docDetails.workerList = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == items.mrNoReq).ToList();
                docDetails.stepHistory = _Cache.cacheHistoryApproved().Where(w => w.htNoReq == items.mrNoReq).ToList();

                docNewLate.docList.Add(docDetails);
            }
            return await Task.Run(() => PartialView("Result/_Hour", docNewLate));
        }

        [HttpPost]
        public async Task<PartialViewResult> SearchFollow([FromBody]searchbydate searchbydate)
        {
            string DateST = DateTime.Parse(searchbydate.start).ToString("yyyyMMdd");
            string DateED = DateTime.Parse(searchbydate.end).ToString("yyyyMMdd");
            MultiNewLate docNewLate = new MultiNewLate();
            docNewLate.docList = new List<MultiDocDetails>();
            docNewLate.mastFlow = await Task.Run(() => _Cache.cacheMastFlowApprove().ToList());
            docNewLate.mastJobs = await Task.Run(() => _Cache.cacheMastJob().ToList());
            //_Cache.clearCacheMastRequestOT();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT()
                                                .Where(w => w.mrDateReq != null && int.Parse(w.mrDateReq.Substring(w.mrDateReq.Length - 4)
                                                                                                  + w.mrDateReq.Substring(3, 2)
                                                                                                  + w.mrDateReq.Substring(0, 2)) >= int.Parse(DateST) 
                                                                                                  && int.Parse(w.mrDateReq.Substring(w.mrDateReq.Length - 4)
                                                                                                  + w.mrDateReq.Substring(3, 2)
                                                                                                  + w.mrDateReq.Substring(0, 2)) <= int.Parse(DateED)
                                                                                                  && !w.mrStatus.StartsWith(GlobalVariable.StatusDraft))
                                                .OrderByDescending(o => DateTime.Parse(o.mrDateReq.Substring(o.mrDateReq.Length - 4) + "/"
                                                                                                  + o.mrDateReq.Substring(3, 2) + "/"
                                                                                                  + o.mrDateReq.Substring(0, 2)))
                                                .ThenByDescending(o => o.mrNoReq.Substring(o.mrNoReq.Length - 2)).ToList())
            {
                items.mrDiviReq = _callFunc.TransferCodeNameToDivision(items.mrDiviReq);
                items.mrDeptReq = _callFunc.TransferCodeNameToDepartment(items.mrDeptReq);
                MultiDocDetails docDetails = new MultiDocDetails();
                docDetails.requestOT = items;
                docDetails.workerList = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == items.mrNoReq).ToList();
                docDetails.stepHistory = _Cache.cacheHistoryApproved().Where(w => w.htNoReq == items.mrNoReq).ToList();

                docNewLate.docList.Add(docDetails);
            }
            return await Task.Run(() => PartialView("Result/_Follow", docNewLate));
        }

        [HttpPost]
        public async Task<PartialViewResult> SearchDocument([FromBody]searchbydate searchbydate)
        {
            string DateST = DateTime.Parse(searchbydate.start).ToString("yyyyMMdd");
            string DateED = DateTime.Parse(searchbydate.end).ToString("yyyyMMdd");
            MultiNewLate docNewLate = new MultiNewLate();
            docNewLate.docList = new List<MultiDocDetails>();
            docNewLate.mastFlow = await Task.Run(() => _Cache.cacheMastFlowApprove().ToList());
            docNewLate.mastJobs = await Task.Run(() => _Cache.cacheMastJob().ToList());
            _Cache.clearCacheMastRequestOT();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT()
                                                .Where(w => w.mrOTDate != null && w.mrStatus.StartsWith(GlobalVariable.StatusFinishedST) && w.mrOTDate.Length > 1 && int.Parse(w.mrOTDate.Substring(w.mrOTDate.Length - 4)
                                                                                                  + w.mrOTDate.Substring(3, 2)
                                                                                                  + w.mrOTDate.Substring(0, 2)) >= int.Parse(DateST)
                                                                                                  && int.Parse(w.mrOTDate.Substring(w.mrOTDate.Length - 4)
                                                                                                  + w.mrOTDate.Substring(3, 2)
                                                                                                  + w.mrOTDate.Substring(0, 2)) <= int.Parse(DateED)
                                                                                                  && (w.mrStep
                                                                                                      >= _Cache.cacheMastFlowApprove().Where(wf => wf.mfFlowNo == w.mrFlow).OrderByDescending(o => o.mfStep).Select(s => s.mfStep).FirstOrDefault().Value - 1))
                                                .OrderByDescending(o => DateTime.Parse(o.mrOTDate.Substring(o.mrOTDate.Length - 4) + "/"
                                                                                                  + o.mrOTDate.Substring(3, 2) + "/"
                                                                                                  + o.mrOTDate.Substring(0, 2)))
                                                .ThenByDescending(o => o.mrNoReq.Substring(o.mrNoReq.Length - 2)).ToList())
            {
                items.mrDiviReq = _callFunc.TransferCodeNameToDivision(items.mrDiviReq);
                items.mrDeptReq = _callFunc.TransferCodeNameToDepartment(items.mrDeptReq);
                MultiDocDetails docDetails = new MultiDocDetails();
                docDetails.requestOT = items;
                docDetails.workerList = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == items.mrNoReq).ToList();
                docDetails.stepHistory = _Cache.cacheHistoryApproved().Where(w => w.htNoReq == items.mrNoReq).ToList();

                docNewLate.docList.Add(docDetails);
            }
            return await Task.Run(() => PartialView("Result/_Document", docNewLate));
        }

        [HttpPost]
        public async Task<PartialViewResult> SearchGraph([FromBody]searchbydate searchbydate)
        {
            //searchbydate get only month-yyyy
            string DateST = DateTime.Parse(searchbydate.start).ToString("yyyyMM");
            string DateED = DateTime.Parse(searchbydate.end).ToString("yyyyMM");
            MultiNewLate docNewLate = new MultiNewLate();
            docNewLate.docList = new List<MultiDocDetails>();
            docNewLate.mastFlow = await Task.Run(() => _Cache.cacheMastFlowApprove().ToList());
            docNewLate.mastJobs = await Task.Run(() => _Cache.cacheMastJob().ToList());
            _Cache.clearCacheMastRequestOT();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT()
                                                .Where(w => w.mrOTDate != null && w.mrOTDate.Length > 1 && int.Parse(w.mrOTDate.Substring(w.mrOTDate.Length - 4)
                                                                                                  + w.mrOTDate.Substring(3, 2)
                                                                                                  + w.mrOTDate.Substring(0, 2)) >= int.Parse(DateST)
                                                                                                  && int.Parse(w.mrOTDate.Substring(w.mrOTDate.Length - 4)
                                                                                                  + w.mrOTDate.Substring(3, 2)
                                                                                                  + w.mrOTDate.Substring(0, 2)) <= int.Parse(DateED)
                                                                                                  && (w.mrStep >= _Cache.cacheMastFlowApprove().Where(wf => wf.mfFlowNo == w.mrFlow).OrderByDescending(o => o.mfStep).Select(s => s.mfStep).FirstOrDefault().Value - 1))
                                                .OrderByDescending(o => DateTime.Parse(o.mrOTDate.Substring(o.mrOTDate.Length - 4) + "/"
                                                                                                  + o.mrOTDate.Substring(3, 2) + "/"
                                                                                                  + o.mrOTDate.Substring(0, 2)))
                                                .ThenByDescending(o => o.mrNoReq.Substring(o.mrNoReq.Length - 2)).ToList())
            {
                items.mrDiviReq = _callFunc.TransferCodeNameToDivision(items.mrDiviReq);
                items.mrDeptReq = _callFunc.TransferCodeNameToDepartment(items.mrDeptReq);
                MultiDocDetails docDetails = new MultiDocDetails();
                docDetails.requestOT = items;
                docDetails.workerList = _Cache.cacheDetailRequestOT().Where(w => w.drNoReq == items.mrNoReq).ToList();
                docDetails.stepHistory = _Cache.cacheHistoryApproved().Where(w => w.htNoReq == items.mrNoReq).ToList();

                docNewLate.docList.Add(docDetails);
            }
            return await Task.Run(() => PartialView("Result/_Hour", docNewLate));
        }
    }
}