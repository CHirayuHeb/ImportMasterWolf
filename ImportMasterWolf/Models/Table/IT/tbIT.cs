using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ImportMasterWolf.Models.Table.IT
{
    [Table("rpEmail")]
    public class ViewrpEmail
    {
        [Key]
        public string emEmpcode { get; set; }
        public string emEmail { get; set; }
        public string emDeptCode { get; set; }
        public string emEmail_M365 { get; set; }
    }
    [Table("Login")]
    public class ViewLoginPgm
    {
        [Key]
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Program { get; set; }
        public string Empcode { get; set; }
        public string Permission { get; set; }

    }
}
