using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ImportMasterWolf.Models.Table.HRMS
{
    [Table("AccEMPLOYEE")]
    public class ViewAccEMPLOYEE
    {
        [Key]
        public string EMP_CODE { get; set; }
        public string PRI_THAI { get; set; }
        public string EMP_ENAME { get; set; }
        public string EMP_TNAME { get; set; }
        public string LAST_ENAME { get; set; }
        public string LAST_TNAME { get; set; }
        public string NICKNAME { get; set; }
        public string DIVI_CODE { get; set; }
        public string SEC_CODE { get; set; }
        public string DEPT_CODE { get; set; }
        public string GRP_CODE { get; set; }
        public string UNT_CODE { get; set; }
        public string POS_CODE { get; set; }
        public string DirOrIndir { get; set; }
        public string QUIT_CODE { get; set; }

    }
    [Table("AccDEPTMAST")]
    public class ViewAccDeptMast
    {
        [Key]
        public string DEPT_CODE { get; set; }
        public string DEPT_NAME { get; set; }
    }

    [Table("AccDIVIMAST")]
    public class ViewAccDIVIMAST
    {
        [Key]
        public string DIVI_CODE { get; set; }
        public string DIVI_NAME { get; set; }
    }

    [Table("AccSECMAST")]
    public class ViewAccSECMAST
    {
        [Key]
        public string SEC_CODE { get; set; }
        public string SEC_NAME { get; set; }
    }

    [Table("AccGRPMAST")]
    public class ViewAccGRPMAST
    {
        [Key]
        public string GRP_CODE { get; set; }
        public string GRP_NAME { get; set; }
    }
  
}
