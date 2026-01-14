using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ImportMasterWolf.Models.Table.WolfApproveCore_thaistanley
{


    [Table("MSTATACCEmployee")]
    public class ViewMSTATACCEmployee
    {
        [Key]
        public int EMPID { get; set; }
        public string EMPCODE { get; set; }
        public string Name { get; set; }
        public string NameTH { get; set; }
        public string JOB_NAME { get; set; }
        public string PositionName { get; set; }
        public string DivisionName { get; set; }
        public string DepartmentName { get; set; }
        public string SECName { get; set; }
        public string GRPName { get; set; }
        public string UNTName { get; set; }
        public string DIRECT_INDIRECT_CODE { get; set; }
        public string INTERCOMNO { get; set; }
        public string NICKNAME { get; set; }
    }
}
