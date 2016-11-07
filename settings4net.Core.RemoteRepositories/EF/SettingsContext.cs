using settings4net.Core.RemoteRepositories.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace settings4net.Core.RemoteRepositories.EF
{
    class SettingsContext : DbContext
    {

        public DbSet<SettingEF> Settings { get; set; }

        public SettingsContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }

    }
}
