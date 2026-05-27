using ImportMasterWolf.Models.Table.HRMS;
using ImportMasterWolf.Models.Table.IT;
using ImportMasterWolf.Models.Table.LAMP;
using ImportMasterWolf.Models.Table.WolfApproveCore_Center;
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

        public string _vType { get; set; }
        public string _vSblock { get; set; }

        //IT
        public ViewrpEmail _ViewrpEmail { get; set; }
        public List<ViewrpEmail> _ListViewrpEmail { get; set; }



        //HRMS
        public ViewAccEMPLOYEE _ViewAccEMPLOYEE { get; set; }
        public List<ViewAccEMPLOYEE> _ListViewAccEMPLOYEE { get; set; }

        public ViewAccDeptMast _ViewAccDeptMast { get; set; }
        public List<ViewAccDeptMast> _ListViewAccDeptMast { get; set; }
        public ViewAccDIVIMAST _ViewAccDIVIMAST { get; set; }
        public List<ViewAccDIVIMAST> _ListViewAccDIVIMAST { get; set; }
        public ViewAccGRPMAST _ViewAccGRPMASTT { get; set; }
        public List<ViewAccGRPMAST> _ListViewAccGRPMAST { get; set; }
        public ViewAccSECMAST _ViewAccSECMAST { get; set; }
        public List<ViewAccSECMAST> _ListViewAccSECMAST { get; set; }
        public ViewAccPOSMAST _ViewAccPOSMAST { get; set; }
        public List<ViewAccPOSMAST> _ListViewAccPOSMAST { get; set; }
        public ViewAccUNITMAST _ViewAccUNITMAST { get; set; }
        public List<ViewAccUNITMAST> _ListViewAccUNITMAST { get; set; }
        public ViewAccDirIndirMast _ViewAccDirIndirMast { get; set; }
        public List<ViewAccDirIndirMast> _ListViewAccDirIndirMast { get; set; }







        //IT
        public ViewLoginPgm _ViewLoginPgm { get; set; }


        //WolfApproveCore_thaistanley
        public ViewMSTATACCEmployee _ViewMSTATACCEmployee { get; set; }
        public List<ViewMSTATACCEmployee> _ListViewMSTATACCEmployee { get; set; }
        public ViewMSTATEmployee _ViewMSTATEmployee { get; set; }
        public List<ViewMSTATEmployee> _ListViewMSTATEmployee { get; set; }
        public ViewMSTEmployee _ViewMSTEmployee { get; set; }
        public List<ViewMSTEmployee> _ListViewMSTEmployee { get; set; }
        public EmployeeSuperiorDetail _ViewEmployeeSuperiorDetail { get; set; }
        public List<EmployeeSuperiorDetail> _ListEmployeeSuperiorDetail { get; set; }
        public ViewMSTDepartment _ViewMSTDepartment { get; set; }
        public List<ViewMSTDepartment> _ListViewMSTDepartment { get; set; }
        public ViewMSTDivision _ViewMSTDivision { get; set; }
        public List<ViewMSTDivision> _ListViewMSTDivision { get; set; }
        public ViewMSTPosition _ViewMSTPosition { get; set; }
        public List<ViewMSTPosition> _ListViewMSTPosition { get; set; }


        //clone table
        public ViewCloneMSTATACCEmployee _ViewCloneMSTATACCEmployee { get; set; }
        public List<ViewCloneMSTATACCEmployee> _ListViewCloneMSTATACCEmployee { get; set; }

        public ViewCloneMSTATEmployee _ViewCloneMSTATEmployee { get; set; }
        public List<ViewCloneMSTATEmployee> _ListViewCloneMSTATEmployee { get; set; }

        public ViewCloneMSTEmployee _ViewCloneMSTEmployee { get; set; }
        public List<ViewCloneMSTEmployee> _ListViewCloneMSTEmployee { get; set; }



        //wolf center
        public ViewWOLFAccount _ViewWOLFAccount { get; set; }
        public List<ViewWOLFAccount> _ListViewWOLFAccount { get; set; }


        //open block slow
        public ViewMSTMasterData _ViewMSTMasterData{get;set;}
        public List<ViewMSTMasterData> _ListViewMSTMasterData { get; set; }

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
