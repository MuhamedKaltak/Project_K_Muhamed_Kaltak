using Microsoft.EntityFrameworkCore;
using Project_K.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.Services
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }

        public Context()
        {

        }

        public Context(DbContextOptions<Context> options)
        {

            SQLitePCL.Batteries_V2.Init();

            Database.MigrateAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\Local;Initial Catalog=Project_K_DB;Integrated Security=True;Pooling=False");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
