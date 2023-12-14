using DatabaseAccess.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }

        private bool Initialized = false;

        public Context()
        {
            if (!Initialized)
            {
                Initialized = true;

                if (OperatingSystem.IsIOS()) //Behövs för att initiera SQLite i iOS
                    SQLitePCL.Batteries_V2.Init();

                Database.MigrateAsync();

            }
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath;

            if (OperatingSystem.IsWindows())
            {
                dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Project_K_DB.db3"); //Gets stored on your "MyDocuments" folder
            }
            else
            {
                dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Project_K_DB.db3");
            }

            optionsBuilder
                .UseSqlite($"Filename={dbPath}");
        }
    }
}
