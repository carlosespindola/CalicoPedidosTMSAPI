using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrainSystem.Pedidos.DAL
{
    public interface IBrainSystemSaadisDBContextFactory
    {
        string CompanyDBName { get; set; }

        SaadisDBContext Create();
    }
    public class SaadisDBContextFactory : IBrainSystemSaadisDBContextFactory
    {
        private string connectionString;

        public string CompanyDBName { get; set; }

        public SaadisDBContextFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public SaadisDBContext Create()
        {
            SaadisDBContext context = null;

            var optionsBuilder = new DbContextOptionsBuilder<SaadisDBContext>(); 

            optionsBuilder.UseSqlServer(this.connectionString);

            var options = optionsBuilder.Options;

            context = new SaadisDBContext(options);

            return context;
        }
    }
}
