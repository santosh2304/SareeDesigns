using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SareeDesigns.Models;

namespace SareeDesigns.Data
{
    public class SareeDesignsContext : DbContext
    {
        public SareeDesignsContext (DbContextOptions<SareeDesignsContext> options)
            : base(options)
        {
        }

        public DbSet<SareeDesigns.Models.Saree> Saree { get; set; } = default!;
    }
}
