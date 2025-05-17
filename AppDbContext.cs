using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Petition.Models
{
    public class AppDbContext : IdentityDbContext<Member>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<PetitionTable> Petitions { get; set; }
        public DbSet<Petition_signature> PetitionSignatures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Petition_signature>()
                .HasKey(ps => new { ps.PetitionId, ps.MemberId });

            modelBuilder.Entity<Petition_signature>()
                .HasOne(ps => ps.petition)
                .WithMany(p => p.petition_Signatures)
                .HasForeignKey(ps => ps.PetitionId);

            modelBuilder.Entity<Petition_signature>()
                .HasOne(ps => ps.member)
                .WithMany(m => m.petition_Signatures)
                .HasForeignKey(ps => ps.MemberId);
        }
    }
}
