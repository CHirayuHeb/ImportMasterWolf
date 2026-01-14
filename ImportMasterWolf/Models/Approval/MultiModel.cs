using ImportMasterWolf.Models.Common;
using ImportMasterWolf.Models.New;
using ImportMasterWolf.Models.Table.HRMS;
using ImportMasterWolf.Models.Table.LAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportMasterWolf.Models.Approval
{

    public class MultiApproveSelected
    {
        public List<ApproveSelected> Document { get; set; }
        public string empcodeMailTo { get; set; }
        public List<string> cc { get; set; }
        public string remark { get; set; }
    }

    public class MutiApprove{
        public string reqNo { get; set; }
        public List<string> workerList { get; set; }
        public string empcode { get; set; }
        public List<string> cc { get; set; }
        public string remark { get; set; }
    }

    public class MultiNewLate
    {
        public List<MultiDocDetails> docList { get; set; }
        public List<ViewMastFlowApprove> mastFlow { get; set; }
        public List<ViewMastJob> mastJobs { get; set; }
        public string req { get; set; }
    }

    public class MultiDocDetails
    {
        public ViewMastRequestOT requestOT { get; set; }
        public List<ViewDetailRequestOT> workerList { get; set; }
        public List<ViewHistoryApproved> stepHistory { get; set; }
        public List<workerImages> workerImages { get; set; }

    }

    public class MultiSendEmail
    {
        public ViewMastRequestOT mastRequestOT { get; set; }
        public ViewHistoryApproved historyApproveds { get; set; }
        public ViewMastFlowApprove nextStep { get; set; }
        public string FullNameMailTo { get; set; }
    }

    public class ApproveSelected
    {
        public string reqNo { get; set; }
        public List<string> workerList { get; set; }
    }
}
