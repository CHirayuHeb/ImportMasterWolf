using Microsoft.AspNetCore.Mvc;
using ImportMasterWolf.Models.Common;
using ImportMasterWolf.Models.Table.LAMP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImportMasterWolf.Models.New
{
    public class autocompleteEmpCode
    {
        public string EmpCode { get; set; }
        public string FullNameAndDept { get; set; }

    }

    public class autocompleteEmail
    {
        public string Mail { get; set; }
        public string EmpCode { get; set; }
        public string FullNameAndDept { get; set; }
    }
}
