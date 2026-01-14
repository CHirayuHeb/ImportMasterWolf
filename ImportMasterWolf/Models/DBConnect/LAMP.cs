using Microsoft.EntityFrameworkCore;
using ImportMasterWolf.Models.Table.LAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportMasterWolf.Models.DBConnect
{
    public class LAMP : DbContext
    {
        public LAMP(DbContextOptions<LAMP> options) : base(options){ }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ViewMastJob>(entity => {
                entity.HasKey(k => new { k.mjGroupCode, k.mjJobCode });
            });

            modelBuilder.Entity<ViewDetailRequestOT>(entity => {
                entity.HasKey(k => new { k.drNoReq, k.drEmpCode });
            });

            modelBuilder.Entity<ViewMastFlowApprove>(entity => {
                entity.HasKey(k=> new { k.mfFlowNo, k.mfStep });
            });

            modelBuilder.Entity<ViewMastModel>(entity => {
                entity.HasKey(k => new { k.mmLamp, k.mmModelName, k.mmProdline });
            });

            modelBuilder.Entity<ViewMastFlowApprove>(entity => {
                entity.HasKey(k => new { k.mfFlowNo, k.mfStep });
            });
        }

        public DbSet<ViewLogin> Login { get; set; }
        public DbSet<ViewDetailRequestOT> DetailRequestOTs { get; set; }
        public DbSet<ViewHistoryApproved> HistoryApproveds { get; set; }
        public DbSet<ViewMastFlowApprove> MastFlowApproves { get; set; }
        public DbSet<ViewMastOTTime> MastOTTime { get; set; }
        public DbSet<ViewMastJob> MastJobs { get; set; }
        public DbSet<ViewMastModel> MastModels { get; set; }
        public DbSet<ViewMastOTReason> MastOTReasons { get; set; }
        public DbSet<ViewMastRequestOT> MastRequestOTs { get; set; }
        public DbSet<ViewMastUserApprove> MastUserApproves { get; set; }
    }
}
