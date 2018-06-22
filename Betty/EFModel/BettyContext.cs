using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Betty.EFModel
{
    public partial class BettyContext : DbContext
    {
        public BettyContext()
        {
        }

        public BettyContext(DbContextOptions<BettyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<GameOdds> GameOdds { get; set; }
        public virtual DbSet<Register> Register { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameOdds>(entity =>
            {
                entity.Property(e => e.CountryCode1)
                    .IsRequired()
                    .HasMaxLength(2);

                entity.Property(e => e.CountryCode2)
                    .IsRequired()
                    .HasMaxLength(2);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.MatchType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Odds1).HasColumnType("numeric(8, 3)");

                entity.Property(e => e.Odds2).HasColumnType("numeric(8, 3)");

                entity.Property(e => e.Player1)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Player2)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.Property(e => e.Win1).HasColumnType("numeric(8, 3)");

                entity.Property(e => e.Win2).HasColumnType("numeric(8, 3)");
            });

            modelBuilder.Entity<Register>(entity =>
            {
                entity.Property(e => e.RefOdds1).HasColumnType("numeric(8, 3)");

                entity.Property(e => e.RefOdds2).HasColumnType("numeric(8, 3)");

                entity.Property(e => e.RefWin1).HasColumnType("numeric(8, 3)");

                entity.Property(e => e.RefWin2).HasColumnType("numeric(8, 3)");

                entity.Property(e => e.SubmitTime).HasColumnType("datetime");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Register)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Register_GameOdds");
            });
        }
    }
}
