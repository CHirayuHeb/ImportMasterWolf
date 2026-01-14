using Microsoft.EntityFrameworkCore;
using ImportMasterWolf.Models.Table.IT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportMasterWolf.Models.DBConnect
{
    public class IT : DbContext
    {
        public IT(DbContextOptions<IT> options) : base(options)
        { }

        public DbSet<ViewrpEmail> rpEmails { get; set; }
        public DbSet<ViewLoginPgm> _ViewLoginPgm { get; set; }

    }
}
