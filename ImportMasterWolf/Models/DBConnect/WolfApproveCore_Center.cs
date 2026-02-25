using Microsoft.EntityFrameworkCore;
using ImportMasterWolf.Models.Table.PrdInvBF_Prd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportMasterWolf.Models.Table.WolfApproveCore_thaistanley;
using ImportMasterWolf.Models.Table.WolfApproveCore_Center;

namespace ImportMasterWolf.Models.DBConnect
{
    public class WolfApproveCore_Center : DbContext
    {
        public WolfApproveCore_Center(DbContextOptions<WolfApproveCore_Center> options) : base(options)
        { }

        public DbSet<ViewWOLFAccount> _ViewWOLFAccount { get; set; }
     
        


    }
}
