using ImportMasterWolf.Models.Common;
using ImportMasterWolf.Models.Table.LAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportMasterWolf.Models.MyRequest
{

    public class MultiDocMast
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
        public List<workerImages> workerImages { get; set; }
        public List<ViewHistoryApproved> stepHistory { get; set; }

    }


}
