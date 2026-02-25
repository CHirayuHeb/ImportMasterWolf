using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ImportMasterWolf.Models.Table.WolfApproveCore_Center
{


    [Table("WOLFAccount")]
    public class ViewWOLFAccount
    {
        [Key]
        public int ID { get; set; }
        public string ContactCode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsVerify { get; set; }
        public string GuidVerify { get; set; }
        public string Note { get; set; }
        public string Remark { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }

    }

  
}
