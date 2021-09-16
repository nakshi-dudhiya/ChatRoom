using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR_Ex.Models
{
    /// <summary>
    /// AppDbContext uses the database 
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): 
            base(options)
        {

        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public AppDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<ChatMessage> chatMessages { get; set; }
    }
}
