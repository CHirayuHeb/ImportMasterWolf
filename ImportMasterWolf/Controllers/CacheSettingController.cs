using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ImportMasterWolf.Models.Cache;
using ImportMasterWolf.Models.DBConnect;
using ImportMasterWolf.Models.Table.HRMS;
using ImportMasterWolf.Models.Table.IT;
using ImportMasterWolf.Models.Table.LAMP;
using ImportMasterWolf.Models.Table.PrdInvBF_Prd;

namespace ImportMasterWolf.Controllers
{
    public class CacheSettingController : Controller
    {
        public IMemoryCache _memoryCache;
        private HRMS _HRMS;
        private LAMP _LAMP;
        private IT _IT;
        private PrdInvBF_Prd _PrdInvBF_Prd;
        public CacheSettingController(LAMP lamp, HRMS hrms, IT it, PrdInvBF_Prd prdinvbF_prd,IMemoryCache memoryCache)
        {
            _LAMP = lamp;
            _HRMS = hrms;
            _IT = it;
            _PrdInvBF_Prd = prdinvbF_prd;
            _memoryCache = memoryCache;
        }

        #region Cachememo

        public List<ViewAccEMPLOYEE> cacheAccEmployee()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cAccEmployee, out List<ViewAccEMPLOYEE> EmployeeList))
            {
                EmployeeList = _HRMS.AccEMPLOYEE.Where(w=>w.QUIT_CODE == null || w.QUIT_CODE == "").ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),

                };
                _memoryCache.Set(CacheKeys.cAccEmployee, EmployeeList, cacheEntryOptions);
            }
            return EmployeeList;
        }

        public List<ViewrpEmail> cacheEmail()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cEmails, out List<ViewrpEmail> EMailList))
            {
                EMailList = _IT.rpEmails.Where(w=>w.emEmail_M365 != null).ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),

                };
                _memoryCache.Set(CacheKeys.cEmails, EMailList, cacheEntryOptions);
            }
            return EMailList;
        }

        public List<ViewAccDeptMast> cacheDEPTMast()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cAccDEPTMast, out List<ViewAccDeptMast> AccDEPTMast))
            {
                AccDEPTMast = _HRMS.AccDEPTMAST.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),

                };
                _memoryCache.Set(CacheKeys.cAccDEPTMast, AccDEPTMast, cacheEntryOptions);
            }
            return AccDEPTMast;
        }

        public List<ViewAccGRPMAST> cacheGRPMast()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cAccGRPMast, out List<ViewAccGRPMAST> AccGRPMast))
            {
                AccGRPMast = _HRMS.AccGROMAST.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),

                };
                _memoryCache.Set(CacheKeys.cAccGRPMast, AccGRPMast, cacheEntryOptions);
            }
            return AccGRPMast;
        }

        public List<ViewAccSECMAST> cacheSECMast()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cAccSECMast, out List<ViewAccSECMAST> AccSECMast))
            {
                AccSECMast = _HRMS.AccSECMAST.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),

                };
                _memoryCache.Set(CacheKeys.cAccSECMast, AccSECMast, cacheEntryOptions);
            }
            return AccSECMast;
        }

        public List<ViewAccDIVIMAST> cacheAccDIVIMast()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cAccDIVIMast, out List<ViewAccDIVIMAST> AccDIVIMast))
            {
                AccDIVIMast = _HRMS.AccDIVIMAST.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddDays(1),
                    SlidingExpiration = TimeSpan.FromMinutes(3),
                };
                _memoryCache.Set(CacheKeys.cAccDIVIMast, AccDIVIMast, cacheEntryOptions);
            }
            return AccDIVIMast;
        }

        public List<ViewDetailRequestOT> cacheDetailRequestOT()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cDetailRequestOT, out List<ViewDetailRequestOT> DetailRequestOTList))
            {
                DetailRequestOTList = _LAMP.DetailRequestOTs.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),

                };
                _memoryCache.Set(CacheKeys.cDetailRequestOT, DetailRequestOTList, cacheEntryOptions);
            }
            return DetailRequestOTList;
        }

        public List<ViewHistoryApproved> cacheHistoryApproved()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cHistoryApproved, out List<ViewHistoryApproved> HistoryApprovedList))
            {
                HistoryApprovedList = _LAMP.HistoryApproveds.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),

                };
                _memoryCache.Set(CacheKeys.cHistoryApproved, HistoryApprovedList, cacheEntryOptions);
            }
            return HistoryApprovedList;
        }

        public List<ViewMastFlowApprove> cacheMastFlowApprove()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cMastFlowApprove, out List<ViewMastFlowApprove> MastFlowApproveList))
            {
                MastFlowApproveList = _LAMP.MastFlowApproves.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),

                };
                _memoryCache.Set(CacheKeys.cMastFlowApprove, MastFlowApproveList, cacheEntryOptions);
            }
            return MastFlowApproveList;
        }

        public List<ViewMastOTTime> cacheMastOTTime()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cMastOTTime, out List<ViewMastOTTime> MastOTTimeList))
            {
                MastOTTimeList = _LAMP.MastOTTime.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),

                };
                _memoryCache.Set(CacheKeys.cMastOTTime, MastOTTimeList, cacheEntryOptions);
            }
            return MastOTTimeList;
        }

        public List<ViewMastJob> cacheMastJob()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cMastJob, out List<ViewMastJob> MastJobList))
            {
                MastJobList = _LAMP.MastJobs.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),

                };
                _memoryCache.Set(CacheKeys.cMastJob, MastJobList, cacheEntryOptions);
            }
            return MastJobList;
        }

        public List<ViewMastModel> cacheMastModel()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cMastModel, out List<ViewMastModel> MastModelList))
            {
                MastModelList = _LAMP.MastModels.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),

                };
                _memoryCache.Set(CacheKeys.cMastModel, MastModelList, cacheEntryOptions);
            }
            return MastModelList;
        }

        public List<ViewProdLine> cacheMastProdLine()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cMastProdLine, out List<ViewProdLine> MastProdLineList))
            {
                MastProdLineList = _PrdInvBF_Prd.ProdLines.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),

                };
                _memoryCache.Set(CacheKeys.cMastProdLine, MastProdLineList, cacheEntryOptions);
            }
            return MastProdLineList;
        }

        public List<ViewMastOTReason> cacheMastOTReason()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cMastOTReason, out List<ViewMastOTReason> OTReasonList))
            {
                OTReasonList = _LAMP.MastOTReasons.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),

                };
                _memoryCache.Set(CacheKeys.cMastOTReason, OTReasonList, cacheEntryOptions);
            }
            return OTReasonList;
        }

        public List<ViewMastRequestOT> cacheMastRequestOT()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cMastRequestOT, out List<ViewMastRequestOT> RequestOTList))
            {
                RequestOTList = _LAMP.MastRequestOTs.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),

                };
                _memoryCache.Set(CacheKeys.cMastRequestOT, RequestOTList, cacheEntryOptions);
            }
            return RequestOTList;
        }

        public List<ViewMastUserApprove> cacheMastUserApprove()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.cMastUserApprove, out List<ViewMastUserApprove> MastUserApproveList))
            {
                MastUserApproveList = _LAMP.MastUserApproves.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),

                };
                _memoryCache.Set(CacheKeys.cMastUserApprove, MastUserApproveList, cacheEntryOptions);
            }
            return MastUserApproveList;
        }

        #endregion

        #region ClearCache

        public OkResult clearCacheMastRequestOT()
        {
            if (_memoryCache.TryGetValue(CacheKeys.cMastRequestOT, out var _))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMilliseconds(1),
                };
                _memoryCache.Set(CacheKeys.cMastRequestOT, "", cacheEntryOptions);
            }
            System.Threading.Thread.Sleep(1000);
            return Ok();
        }

        public OkResult clearCacheMastJob()
        {

            if (_memoryCache.TryGetValue(CacheKeys.cMastJob, out var _))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMilliseconds(1),
                };
                _memoryCache.Set(CacheKeys.cMastJob, "", cacheEntryOptions);
            }
            System.Threading.Thread.Sleep(1000);
            return Ok();
        }

        public OkResult clearCacheDetailRequestOT()
        {
            if (_memoryCache.TryGetValue(CacheKeys.cDetailRequestOT, out var _))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMilliseconds(1),
                };
                _memoryCache.Set(CacheKeys.cDetailRequestOT, "", cacheEntryOptions);
            }
            System.Threading.Thread.Sleep(1000);
            return Ok();
        }

        public OkResult clearCacheHistoryApproved()
        {
            if (_memoryCache.TryGetValue(CacheKeys.cHistoryApproved, out var _))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMilliseconds(1),
                };
                _memoryCache.Set(CacheKeys.cHistoryApproved, "", cacheEntryOptions);
            }
            System.Threading.Thread.Sleep(1000);
            return Ok();
        }

        public OkResult clearCacheAccEmployee()
        {
            if (_memoryCache.TryGetValue(CacheKeys.cAccEmployee, out var _))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMilliseconds(1),
                };
                _memoryCache.Set(CacheKeys.cAccEmployee, "", cacheEntryOptions);
            }
            System.Threading.Thread.Sleep(1000);
            return Ok();
        }

        #endregion
    }
}