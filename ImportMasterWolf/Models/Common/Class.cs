using ImportMasterWolf.Models.Table.HRMS;
using ImportMasterWolf.Models.Table.IT;
using ImportMasterWolf.Models.Table.LAMP;
using ImportMasterWolf.Models.Table.WolfApproveCore_thaistanley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportMasterWolf.Models.Common
{
    public class Class
    {
        public ViewLogin _ViewLogin { get; set; }
        public Error _Error { get; set; }
        public string param { get; set; }

        //IT
        public ViewLoginPgm _ViewLoginPgm { get; set; }


        //WolfApproveCore_thaistanley
        public ViewMSTATACCEmployee _ViewMSTATACCEmployee { get; set; }
        public List<ViewMSTATACCEmployee> _ListViewMSTATACCEmployee { get; set; }


    }

    public class OTTimeStart
    {
        public string Time { get; set; }
    }
    public class OTTimeEnd
    {
        public string Time { get; set; }
    }
    public class OTModel
    {
        public string Name { get; set; }
    }
    public class OTProdLine
    {
        public string Name { get; set; }
    }
    public class OTReason
    {
        public string Code { get; set; }
        public string Caption { get; set; }
    }
    public class CCMail
    {
        public string email { get; set; }
    }

    public class req
    {
        public string no { get; set; }
    }
    public class searchbydate
    {
        public string start { get; set; }
        public string end { get; set; }
    }

    public class CategoryWorkerList
    {
        public Guid Guid { get; set; }
        public byte EmpPic { get; set; }
        public string PriName { get; set; }
        public string EmpCode { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Job { get; set; }
        public string GRP_Code { get; set; }
    }

    public class workerImages
    {
        public string empcode { get; set; }
        public string image { get; set; }
    }
}
