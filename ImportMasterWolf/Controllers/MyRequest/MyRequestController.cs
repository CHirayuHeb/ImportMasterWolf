using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ImportMasterWolf.Models.Common;
using ImportMasterWolf.Models.DBConnect;
using ImportMasterWolf.Models.MyRequest;
using ImportMasterWolf.Models.Table.HRMS;
using ImportMasterWolf.Models.Table.LAMP;

namespace ImportMasterWolf.Controllers.MyRequest
{
    public class MyRequestController : Controller
    {
        private LAMP _LAMP;
        private HRMS _HRMS;
        private IT _IT;
        private CacheSettingController _Cache;
        private FunctionsController _callFunc;
        public MyRequestController(LAMP lamp, HRMS hrms, IT it, CacheSettingController cacheController, FunctionsController callfunction)
        {
            _LAMP = lamp;
            _HRMS = hrms;
            _IT = it;
            _Cache = cacheController;
            _callFunc = callfunction;
        }

        public IActionResult Index()
        {
            string EmpCode = User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value?.ToString();
            WebClient request = new WebClient();
            string imgPath = GlobalVariable.imgPath;
            MultiDocMast docMast = new MultiDocMast();
            docMast.docList = new List<MultiDocDetails>();
            docMast.mastFlow = _Cache.cacheMastFlowApprove().ToList();
            docMast.mastJobs = _Cache.cacheMastJob().ToList();
            docMast.req = "";
            _Cache.clearCacheMastRequestOT();

            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT().Where(w => w.mrEmpReq == EmpCode && !w.mrStatus.StartsWith(GlobalVariable.StatusFinishedST) && w.mrStep != 0)
                                                                            .OrderByDescending(o => DateTime.Parse(o.mrDateReq.Substring(o.mrDateReq.Length - 4) + "/"
                                                                                                  + o.mrDateReq.Substring(3, 2) + "/"
                                                                                                  + o.mrDateReq.Substring(0, 2)))
                                                                            .ThenByDescending(o => o.mrNoReq.Substring(o.mrNoReq.Length - 2)).ToList())
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

            return View(docMast);
        }

        public IActionResult DisplayWaiting()
        {
            string EmpCode = User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value?.ToString();
            WebClient request = new WebClient();
            string imgPath = GlobalVariable.imgPath;
            MultiDocMast docMast = new MultiDocMast();
            docMast.docList = new List<MultiDocDetails>();
            docMast.mastFlow = _Cache.cacheMastFlowApprove().ToList();
            docMast.mastJobs = _Cache.cacheMastJob().ToList();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT().Where(w => w.mrEmpReq == EmpCode && !w.mrStatus.StartsWith(GlobalVariable.StatusFinishedST) && w.mrStep != 0)
                                                                            .OrderByDescending(o => DateTime.Parse(o.mrDateReq.Substring(o.mrDateReq.Length - 4) + "/"
                                                                                                  + o.mrDateReq.Substring(3, 2) + "/"
                                                                                                  + o.mrDateReq.Substring(0, 2)))
                                                                            .ThenByDescending(o => o.mrNoReq.Substring(o.mrNoReq.Length - 2)).ToList())
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
            return PartialView("_DisplayWaiting", docMast);
        }
        public IActionResult DisplayDone()
        {
            string EmpCode = User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value?.ToString();
            WebClient request = new WebClient();
            string imgPath = GlobalVariable.imgPath;
            MultiDocMast docMast = new MultiDocMast();
            docMast.docList = new List<MultiDocDetails>();
            docMast.mastFlow = _Cache.cacheMastFlowApprove().ToList();
            docMast.mastJobs = _Cache.cacheMastJob().ToList();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT().Where(w => w.mrEmpReq == EmpCode && w.mrStatus.StartsWith(GlobalVariable.StatusFinishedST))
                                                                            .OrderByDescending(o => DateTime.Parse(o.mrDateReq.Substring(o.mrDateReq.Length - 4) + "/"
                                                                                                  + o.mrDateReq.Substring(3, 2) + "/"
                                                                                                  + o.mrDateReq.Substring(0, 2)))
                                                                            .ThenByDescending(o => o.mrNoReq.Substring(o.mrNoReq.Length - 2)).ToList())
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

            return PartialView("_DisplayDone", docMast);
        }
        public IActionResult DisplayDisapproved()
        {
            string EmpCode = User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value?.ToString();
            WebClient request = new WebClient();
            string imgPath = GlobalVariable.imgPath;
            MultiDocMast docMast = new MultiDocMast();
            docMast.docList = new List<MultiDocDetails>();
            docMast.mastFlow = _Cache.cacheMastFlowApprove().ToList();
            docMast.mastJobs = _Cache.cacheMastJob().ToList();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT().Where(w => w.mrEmpReq == EmpCode && w.mrStatus.StartsWith(GlobalVariable.StatusRejected))
                                                                            .OrderByDescending(o => DateTime.Parse(o.mrDateReq.Substring(o.mrDateReq.Length - 4) + "/"
                                                                                                  + o.mrDateReq.Substring(3, 2) + "/"
                                                                                                  + o.mrDateReq.Substring(0, 2)))
                                                                            .ThenByDescending(o => o.mrNoReq.Substring(o.mrNoReq.Length - 2)).ToList())
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
            return PartialView("_DisplayDisapproved", docMast);
        }

        public IActionResult DisplayDraft()
        {
            string EmpCode = User.Claims.FirstOrDefault(s => s.Type == "EmpCode").Value?.ToString();
            WebClient request = new WebClient();
            string imgPath = GlobalVariable.imgPath;
            MultiDocMast docMast = new MultiDocMast();
            docMast.docList = new List<MultiDocDetails>();
            docMast.mastFlow = _Cache.cacheMastFlowApprove().ToList();
            docMast.mastJobs = _Cache.cacheMastJob().ToList();
            foreach (ViewMastRequestOT items in _Cache.cacheMastRequestOT().Where(w => w.mrEmpReq == EmpCode && w.mrStep == 0)
                                                                            .OrderByDescending(o => DateTime.Parse(o.mrDateReq.Substring(o.mrDateReq.Length - 4) + "/"
                                                                                                  + o.mrDateReq.Substring(3, 2) + "/"
                                                                                                  + o.mrDateReq.Substring(0, 2)))
                                                                            .ThenByDescending(o => o.mrNoReq.Substring(o.mrNoReq.Length - 2)).ToList())
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
            return PartialView("_DisplayDraft", docMast);
        }
    }
}