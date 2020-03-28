using System;
using Domain;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.App.EF
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public DbSet<FamilyTree> FamilyTrees { get; set; } = default!;
        public DbSet<Person> Persons { get; set; } = default!;
        public DbSet<Relationship> Relationships { get; set; } = default!;
        public DbSet<RelationshipType> RelationshipTypes { get; set; } = default!;
        public DbSet<Gender> Genders { get; set; } = default!;
        
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
    
    
}