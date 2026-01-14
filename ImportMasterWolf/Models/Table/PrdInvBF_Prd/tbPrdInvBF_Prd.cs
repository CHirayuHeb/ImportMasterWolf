using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ImportMasterWolf.Models.Table.PrdInvBF_Prd
{
    [Table("bf_prodline")]
    public class ViewProdLine
    {
        public string prodline { get; set; }
        public string plant { get; set; }
        public string StatusUse { get; set; }
    }
}
