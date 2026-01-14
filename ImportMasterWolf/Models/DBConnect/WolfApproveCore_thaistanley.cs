using Microsoft.EntityFrameworkCore;
using ImportMasterWolf.Models.Table.PrdInvBF_Prd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportMasterWolf.Models.Table.WolfApproveCore_thaistanley;

namespace ImportMasterWolf.Models.DBConnect
{
    public class WolfApproveCore_thaistanley : DbContext
    {
        public WolfApproveCore_thaistanley(DbContextOptions<WolfApproveCore_thaistanley> options) : base(options)
        { }

        public DbSet<ViewMSTATACCEmployee> _ViewMSTATACCEmployee { get; set; }
    }
}
