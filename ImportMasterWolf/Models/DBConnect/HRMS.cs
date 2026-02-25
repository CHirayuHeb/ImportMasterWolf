using Microsoft.EntityFrameworkCore;
using ImportMasterWolf.Models.Table.HRMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportMasterWolf.Models.DBConnect
{
    public class HRMS : DbContext
    {
        public HRMS(DbContextOptions<HRMS> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ViewAccDirIndirMast>(entity => {
                entity.HasKey(k => new { k.diDiviCode, k.diDeptCode,k.diSectCode,k.diDirIndir });
            });
        }
        public DbSet<ViewAccEMPLOYEE> AccEMPLOYEE { get; set; }
        public DbSet<ViewAccDeptMast> AccDEPTMAST { get; set; }
        public DbSet<ViewAccDIVIMAST> AccDIVIMAST { get; set; }
        public DbSet<ViewAccGRPMAST> AccGROMAST { get; set; }
        public DbSet<ViewAccSECMAST> AccSECMAST { get; set; }
        public DbSet<ViewAccPOSMAST> AccPOSMAST { get; set; }
        public DbSet<ViewAccUNITMAST> AccUNITMAST { get; set; }
        public DbSet<ViewAccDirIndirMast> AccDirIndirMast { get; set; }

    }
}
