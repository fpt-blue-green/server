using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BusinessObjects.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminAction> AdminActions { get; set; }

    public virtual DbSet<AdsCampaign> AdsCampaigns { get; set; }

    public virtual DbSet<BannedUser> BannedUsers { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Campaign> Campaigns { get; set; }

    public virtual DbSet<CampaignTag> CampaignTags { get; set; }

    public virtual DbSet<Channel> Channels { get; set; }

    public virtual DbSet<Deal> Deals { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Influencer> Influencers { get; set; }

    public virtual DbSet<InfluencerJobHistory> InfluencerJobHistories { get; set; }

    public virtual DbSet<InfluencerTag> InfluencerTags { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<Offer> Offers { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<SystemSetting> SystemSettings { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User Id=postgres.irjktobnibxfmjarpmxj;Password=rNK67MXUaWgviJdz;Server=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("CChannelType", new[] { "None", "Tiktok", "Instagram", "Youtube" })
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("pgsodium", "key_status", new[] { "default", "valid", "invalid", "expired" })
            .HasPostgresEnum("pgsodium", "key_type", new[] { "aead-ietf", "aead-det", "hmacsha512", "hmacsha256", "auth", "shorthash", "generichash", "kdf", "secretbox", "secretstream", "stream_xchacha20" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "pgjwt")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("pgsodium", "pgsodium")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<AdminAction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AdminAction_pkey");

            entity.ToTable("AdminAction");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ActionDate).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.User).WithMany(p => p.AdminActions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AdminAction_AdminId_fkey");
        });

        modelBuilder.Entity<AdsCampaign>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AdsCampaign_pkey");

            entity.ToTable("AdsCampaign");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Campaign).WithMany(p => p.AdsCampaigns)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AdsCampaign_CampaignId_fkey");
        });

        modelBuilder.Entity<BannedUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BannedUsers_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.BanDate).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.BannedBy).WithMany(p => p.BannedUserBannedBies)
                .HasForeignKey(d => d.BannedById)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BannedUsers_BannedById_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.BannedUserUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BannedUsers_UserId_fkey");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Brands_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");
            entity.Property(e => e.IsPremium).HasDefaultValueSql("false");

            entity.HasOne(d => d.User).WithMany(p => p.Brands)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Brands_UserId_fkey");
        });

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Campaigns_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Budget).HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("false");

            entity.HasOne(d => d.Brand).WithMany(p => p.Campaigns)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Campaigns_BrandId_fkey");
        });

        modelBuilder.Entity<CampaignTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("CampaignTags_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Campaign).WithMany(p => p.CampaignTags)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CampaignTags_CampaignId_fkey");

            entity.HasOne(d => d.Tag).WithMany(p => p.CampaignTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CampaignTags_TagId_fkey");
        });

        modelBuilder.Entity<Channel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Channels_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");
            entity.Property(e => e.UserName).HasMaxLength(255);

            entity.HasOne(d => d.Influencer).WithMany(p => p.Channels)
                .HasForeignKey(d => d.InfluencerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Channels_InfluencerId_fkey");
        });

        modelBuilder.Entity<Deal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Deal_pkey");

            entity.ToTable("Deal");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Influencer).WithMany(p => p.Deals)
                .HasForeignKey(d => d.InfluencerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Deal_InfluencerId_fkey");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Feedbacks_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.Influencer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.InfluencerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Feedbacks_InfluencerId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Feedbacks_UserId_fkey");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Images_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.Brand).WithMany(p => p.Images)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("Images_BrandId_fkey");

            entity.HasOne(d => d.Influencer).WithMany(p => p.Images)
                .HasForeignKey(d => d.InfluencerId)
                .HasConstraintName("Images_InfluencerId_fkey");
        });

        modelBuilder.Entity<Influencer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Influencers_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("false");
            entity.Property(e => e.IsPublish).HasDefaultValueSql("true");
            entity.Property(e => e.RateAverage).HasDefaultValueSql("'0'::numeric");

            entity.HasOne(d => d.User).WithMany(p => p.Influencers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Influencers_UserId_fkey");
        });

        modelBuilder.Entity<InfluencerJobHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("InfluencerJobHistories_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Campaign).WithMany(p => p.InfluencerJobHistories)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("InfluencerJobHistories_CampaignId_fkey");

            entity.HasOne(d => d.Influencer).WithMany(p => p.InfluencerJobHistories)
                .HasForeignKey(d => d.InfluencerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("InfluencerJobHistories_InfluencerId_fkey");

            entity.HasOne(d => d.Job).WithMany(p => p.InfluencerJobHistories)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("InfluencerJobHistories_JobId_fkey");
        });

        modelBuilder.Entity<InfluencerTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("InfluencerTags_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Influencer).WithMany(p => p.InfluencerTags)
                .HasForeignKey(d => d.InfluencerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("InfluencerTags_InfluencerId_fkey");

            entity.HasOne(d => d.Tag).WithMany(p => p.InfluencerTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("InfluencerTags_TagId_fkey");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Jobs_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Campaign).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Jobs_CampaignId_fkey");

            entity.HasOne(d => d.Package).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Jobs_PackageId_fkey");
        });

        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Offer_pkey");

            entity.ToTable("Offer");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.Job).WithMany(p => p.Offers)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Offer_JobId_fkey");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Packages_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Influencer).WithMany(p => p.Packages)
                .HasForeignKey(d => d.InfluencerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Packages_InfluencerId_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Payments_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.Brand).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("Payments_BrandId_fkey");
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SystemSetting_pkey");

            entity.ToTable("SystemSetting");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ModifiedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Tags_pkey");

            entity.HasIndex(e => e.TagName, "Tags_TagName_key").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.IsPremiumTag).HasDefaultValueSql("false");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsBanned).HasDefaultValueSql("false");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("false");
            entity.Property(e => e.Password).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
