using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrainSystem.Pedidos.DAL
{
    public interface IBrainSystemDBContextFactory
    {
        string CompanyDBName { get; set; }

        BrainSystemDBContext Create();
    }
    public class BrainSystemDBContextFactory : IBrainSystemDBContextFactory
    {
        private string connectionStringTemplate;

        public string CompanyDBName { get; set; }

        public BrainSystemDBContextFactory(string connectionStringTemplate)
        {
            this.connectionStringTemplate = connectionStringTemplate;
        }

        public BrainSystemDBContext Create()
        {
            BrainSystemDBContext context = null;

            //if (string.IsNullOrWhiteSpace(this.CompanyDBName))
            //{
            //    this.CompanyDBName = "AdminDB";
            //}

            var optionsBuilder = new DbContextOptionsBuilder<BrainSystemDBContext>(); 

            optionsBuilder.UseSqlServer(this.connectionStringTemplate);

            var options = optionsBuilder.Options;

            context = new BrainSystemDBContext(options);

            return context;
        }
    }
}
