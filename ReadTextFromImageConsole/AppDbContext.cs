using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ReadTextFromImageConsole
{
    public class AppDbContext : DbContext
    {
        private IConfiguration configuration;


        public virtual DbSet<Models.Camudatafield> Camudatafield { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var conString = Program.config.GetConnectionString("SterlingOnlineBankingDbContext");
                //optionsBuilder.UseSqlServer(@"Server=10.0.41.101;Initial Catalog=SterlingOnlineBanking;Integrated Security=False;User ID=sa;Password=tylent;MultipleActiveResultSets=True;");
                optionsBuilder.UseSqlServer(conString);
            }
        }
    }
}
