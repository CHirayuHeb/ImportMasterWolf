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
        public DbSet<ViewMSTATEmployee> _ViewMSTATEmployee { get; set; }
        public DbSet<ViewMSTEmployee> _ViewMSTEmployee { get; set; }
        public DbSet<ViewMSTDepartment> _ViewMSTDepartment { get; set; }
        public DbSet<ViewMSTDivision> _ViewMSTDivision { get; set; }
        public DbSet<ViewMSTPosition> _ViewMSTPosition { get; set; }
        


    }
}
