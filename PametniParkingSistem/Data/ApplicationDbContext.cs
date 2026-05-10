using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Rezervacija> Rezervacije { get; set; }
        public DbSet<ParkingMjesto> ParkingMjesta { get; set; }
        public DbSet<ParkingZona> ParkingZone { get; set; }
        public DbSet<Placanje> Placanja { get; set; }
        public DbSet<Recenzija> Recenzije { get; set; }
        public DbSet<EmailObavijest> EmailObavijesti { get; set; }
        public DbSet<Cjenovnik> Cjenovnici { get; set; }
        public DbSet<KriterijPretrage> KriterijiPretrage { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Korisnik>().ToTable("Korisnik");
            modelBuilder.Entity<Rezervacija>().ToTable("Rezervacija");
            modelBuilder.Entity<ParkingMjesto>().ToTable("ParkingMjesto");
            modelBuilder.Entity<ParkingZona>().ToTable("ParkingZona");
            modelBuilder.Entity<Placanje>().ToTable("Placanje");
            modelBuilder.Entity<Recenzija>().ToTable("Recenzija");
            modelBuilder.Entity<EmailObavijest>().ToTable("EmailObavijest");
            modelBuilder.Entity<Cjenovnik>().ToTable("Cjenovnik");
            modelBuilder.Entity<KriterijPretrage>().ToTable("KriterijPretrage");
        }
    }
}