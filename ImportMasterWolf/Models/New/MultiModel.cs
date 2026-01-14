using ImportMasterWolf.Models.Common;
using ImportMasterWolf.Models.Table.LAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportMasterWolf.Models.New
{
    public class multiModelOTForm
    {
        public ViewMastRequestOT mastRequestOT { get; set; }
        public List<OTTimeStart> timeStart { get; set; }
        public List<OTTimeEnd> timeEnd { get; set; }
        public List<OTModel> models { get; set; }
        public List<OTProdLine> prodLines { get; set; }
        public List<OTReason> reasons { get; set; }
    }

    public class multiModelCateWorker
    {
        public CategoryWorkerList CategoryWorkerList { get; set; }
        public List<ViewMastJob> Jobs { get; set; }
        public string image { get; set; }
    }

    public class multiOTEmailForm
    {
        public ViewMastRequestOT EFMastRequestOT { get; set; }
        public ViewHistoryApproved historyApproveds { get; set; }
        public string FullNameMailTo { get; set; }
        public string positionNext { get; set; }
    }

    //public class multiDisplay
    //{
    //    public List<ViewMastRequestOT> mastRequestOT { get; set; }
    //    public List<ViewDetailRequestOT> detailRequestOTs { get; set; }
    //    public List<ViewMastFlowApprove> mastFlowApprove { get; set; }
    //    public List<ViewHistoryApproved> historyApproveds { get; set; }

    //}

    //public class multiDisplay
    //{
    //    public List<MultiDocDetails> docList { get; set; }
    //    public List<ViewMastFlowApprove> mastFlow { get; set; }
    //    public List<ViewMastJob> mastJobs { get; set; }
    //    public string req { get; set; }
    //}

    //public class MultiDocDetails
    //{
    //    public ViewMastRequestOT requestOT { get; set; }
    //    public List<ViewDetailRequestOT> workerList { get; set; }
    //    public List<ViewHistoryApproved> stepHistory { get; set; }

    //}

    public class multiEditModel
    {
        public ViewMastRequestOT ViewMastRequestOT { get; set; }
        public multiModelOTForm multiModelOTForm { get; set; }
        public multiOTEmailForm multiOTEmailForm { get; set; }
        public List<multiModelCateWorker> multiModelCateWorker { get; set; }
    }
}
