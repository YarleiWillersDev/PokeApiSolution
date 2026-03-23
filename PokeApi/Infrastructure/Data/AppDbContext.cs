using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using PokeApiTeste.Model;

namespace PokeApi.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<PokemonColor> PokemonColors { get; set; } = null!;
        public DbSet<PokemonSpecies> PokemonSpecies { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<PokemonColor>()
                        .HasIndex(color => color.Name)
                        .IsUnique();
            
            modelBuilder.Entity<PokemonColor>()
                        .Property(color => color.Name)
                        .IsRequired()
                        .HasMaxLength(50);
            
            modelBuilder.Entity<PokemonSpecies>()
                        .HasIndex(specie => specie.Name)
                        .IsUnique();

            modelBuilder.Entity<PokemonSpecies>()
                        .Property(specie => specie.Name)
                        .IsRequired()
                        .HasMaxLength(100);

            base.OnModelCreating(modelBuilder);
        }

    }
}