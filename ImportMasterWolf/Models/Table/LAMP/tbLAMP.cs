using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ImportMasterWolf.Models.Table.LAMP
{
    [Table("Login")]
    public class ViewLogin
    {
        [Key]
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Program { get; set; }
        public string Empcode { get; set; }
        public string Permission { get; set; }
    }

    [Table("otDetailRequestOT")]
    public class ViewDetailRequestOT {
        public string drNoReq { get; set; }
        public string drDateReq { get; set; }
        public string drEmpCode { get; set; }
        public string drPriName { get; set; }
        public string drName { get; set; }
        public string drLastName { get; set; }
        public string drDivi { get; set; }
        public string drDept { get; set; }
        public string drSec { get; set; }
        public string drGrp { get; set; }
        public string drUnit { get; set; }
        public string drSubDirOrInDir { get; set; }
        public string drJobCode { get; set; }
        public string drStatus { get; set; }
    }

    [Table("otHistoryApproved")]
    public class ViewHistoryApproved {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? htNo { get; set; }
        public string htNoReq { get; set; }
        public string htDateReq { get; set; }
        public int? htStep { get; set; }
        public string htStatus { get; set; }
        public string htFrom { get; set; }
        public string htTo { get; set; }
        public string htCC { get; set; }
        public string htDate { get; set; }
        public string htTime { get; set; }
        public string htRemark { get; set; }
    }

    [Table("otMastFlowApprove")]
    public class ViewMastFlowApprove {
        [Key]
        public string mfFlowNo { get; set; }
        public int? mfStep { get; set; }
        public string mfSubject { get; set; }
        public string mfPermission { get; set; }
        public string mfCC { get; set; }
        public string mfIssueBy { get; set; }
        public string mfUpdateBy { get; set; }

    }

    [Table("otMastJob")]
    public class ViewMastJob {
        public string mjGroupCode { get; set; }
        public string mjJobCode { get; set; }
        public string mjJobName { get; set; }
        public string mjDetail { get; set; }
        public string mjIssueBy { get; set; }
        public string mjUpdateBy { get; set; }

    }

    [Table("otMastModel")]
    public class ViewMastModel {
        [Key]
        public string mmLamp { get; set; }
        public string mmProdline { get; set; }
        public string mmModelName { get; set; }
        public string mmIssueBy { get; set; }
        public string mmUpdateBy { get; set; }
    }

    [Table("otMastOTReason")]
    public class ViewMastOTReason {
        [Key]
        public string mrCode { get; set; }
        public string mrReasonEN { get; set; }
        public string mrReasonTH { get; set; }
        public string mrIssueDate { get; set; }
        public string mrUpdate { get; set; }
    }

    [Table("otMastRequestOT")]
    public class ViewMastRequestOT {
        [Key]
        public string mrNoReq { get; set; }
        public string mrDateReq { get; set; }
        public string mrEmpReq { get; set; }
        public string mrPriNameReq { get; set; }
        public string mrNameReq { get; set; }
        public string mrLastNameReq { get; set; }
        public string mrDiviReq { get; set; }
        public string mrPositionReq { get; set; }
        public string mrDeptReq { get; set; }
        public string mrSecReq { get; set; }
        public string mrGrpReq { get; set; }
        public string mrUnitReq { get; set; }
        public string mrOTType { get; set; }
        public string mrOTDate { get; set; }
        public string mrOTTimeSt_Before { get; set; }
        public string mrOTTimeEd_Before { get; set; }
        public string mrOTTimeSt_During { get; set; }
        public string mrOTTimeEd_During { get; set; }
        public string mrOTTimeSt_After { get; set; }
        public string mrOTTimeEd_After { get; set; }
        public string mrModel { get; set; }
        public string mrProductionLine { get; set; }
        public string mrReason { get; set; }
        public string mrRemark { get; set; }
        public string mrFlow { get; set; }
        public int? mrStep { get; set; }
        public string mrStatus { get; set; }
        public string mrEmpApp { get; set; }
        public string mrNameApp { get; set; }

    }

    [Table("otMastUserApprove")]
    public class ViewMastUserApprove {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int muNo { get; set; }
        public string muEmpCode { get; set; }
        public string muDeptCode { get; set; }
        public string muFlowNo { get; set; }
        public string muOperator { get; set; }
        public string muCheck { get; set; }
        public string muApprove { get; set; }
        public string muPosition { get; set; }
        public string muIssueBy { get; set; }
        public string muUpdateBy { get; set; }
    }

    [Table("otMastOTTime")]
    public class ViewMastOTTime {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int? mtNo { get; set; }
        public string mtTime { get; set; }
        public string mtType { get; set; }
    }


        public class Error
    {
        public string validation { get; set; }
    }
}
