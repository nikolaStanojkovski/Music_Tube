using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicTube.Domain.Domain;
using MusicTube.Domain.Domain.Subdomain;
using MusicTube.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicTube.Repository
{
    public class ApplicationDbContext : IdentityDbContext<MusicTubeUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<Media> Media { get; set; }
        public virtual DbSet<Song> Songs { get; set; }
        public virtual DbSet<Video> Videos { get; set; }

        public virtual DbSet<PremiumPlan> PremiumPlans { get; set; }
        public virtual DbSet<UserFeedback> UserFeedbacks { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<EmailMessage> EmailMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(builder);

            // IDENTIFIERS

            builder.Entity<PremiumPlan>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<Review>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<UserFeedback>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<Album>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<Media>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<Song>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<Video>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            // RELATIONS

            // User relations

            builder.Entity<MusicTubeUser>()
                .HasOne(z => z.FavouriteArtist)
                .WithMany(t => t.Fans)
                .HasForeignKey(t => t.FavouriteArtistId);

            builder.Entity<UserFeedback>()
                .HasOne(z => z.User)
                .WithMany(z => z.Feedbacks)
                .HasForeignKey(t => t.UserId);

            builder.Entity<UserFeedback>()
                .HasOne(z => z.Media)
                .WithMany(z => z.Feedbacks)
                .HasForeignKey(t => t.MediaId);

            builder.Entity<Review>()
                .HasOne(z => z.Listener)
                .WithMany(t => t.Reviews)
                .HasForeignKey(t => t.ListenerId);

            builder.Entity<Review>()
                .HasOne(z => z.Media)
                .WithMany(t => t.Reviews)
                .HasForeignKey(t => t.MediaId);

            // Other relations

            builder.Entity<Creator>()
                .HasMany(z => z.Content)
                .WithOne(t => t.Creator)
                .HasForeignKey(t => t.CreatorId);

            builder.Entity<Creator>()
                .HasOne(z => z.PremiumPlan)
                .WithOne(t => t.Creator)
                .HasForeignKey<PremiumPlan>(z => z.CreatorId);

            builder.Entity<PremiumPlan>()
                .HasMany(z => z.Albums)
                .WithOne(t => t.PremiumUser)
                .HasForeignKey(z => z.PremiumUserId);

            builder.Entity<Album>()
                .HasMany(z => z.Songs)
                .WithOne(t => t.Album)
                .HasForeignKey(z => z.AlbumId);

            builder.Entity<Song>()
                .HasMany(z => z.VideosAppearedIn)
                .WithOne(t => t.Song)
                .HasForeignKey(z => z.SongId);
        }
    }
}
