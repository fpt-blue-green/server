using BusinessObjects.Helper;
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

    // Override SaveChangesAsync
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Gọi hàm tùy chỉnh để cập nhật ModifiedAt trước khi lưu thay đổi
        UpdateModifiedAt();

        // Gọi SaveChangesAsync của base để lưu thay đổi vào cơ sở dữ liệu
        return await base.SaveChangesAsync(cancellationToken);
    }

    // Hàm riêng để kiểm tra và cập nhật thuộc tính ModifiedAt
    private void UpdateModifiedAt()
    {
        // Lặp qua tất cả các entry trong ChangeTracker (bao gồm các thực thể bị thêm mới hoặc thay đổi)
        foreach (var entry in ChangeTracker.Entries())
        {
            // Chỉ kiểm tra các thực thể có trạng thái Modified và Added
            if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
            {
                // Kiểm tra xem thực thể có thuộc tính "ModifiedAt" không
                var modifiedAtProperty = entry.Entity.GetType().GetProperty("ModifiedAt");

                if (modifiedAtProperty != null && modifiedAtProperty.PropertyType == typeof(DateTime?))
                {
                    // Nếu có, cập nhật giá trị của thuộc tính này với thời gian hiện tại
                    modifiedAtProperty.SetValue(entry.Entity, DateTime.UtcNow);
                }
            }
        }
    }
    public virtual DbSet<AdminAction> AdminActions { get; set; }

    public virtual DbSet<BannedUser> BannedUsers { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Campaign> Campaigns { get; set; }

    public virtual DbSet<CampaignContent> CampaignContents { get; set; }

    public virtual DbSet<CampaignImage> CampaignImages { get; set; }

    public virtual DbSet<Channel> Channels { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Influencer> Influencers { get; set; }

    public virtual DbSet<InfluencerImage> InfluencerImages { get; set; }

    public virtual DbSet<InfluencerReport> InfluencerReports { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobDetail> JobDetails { get; set; }

    public virtual DbSet<Offer> Offers { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<PaymentBooking> PaymentBookings { get; set; }

    public virtual DbSet<PaymentHistory> PaymentHistories { get; set; }

    public virtual DbSet<SystemSetting> SystemSettings { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDevice> UserDevices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User Id=postgres.umpnoqjoitnafyrqqcoq;Password=Wn5n3WVMGnk98UKj;Server=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        DateTimeConverter.ConfigureDateTimeConversion(modelBuilder);
        modelBuilder.Entity<User>().HasQueryFilter(u => u.IsDeleted == false); 
        modelBuilder.Entity<Campaign>().HasQueryFilter(u => u.IsDeleted == false);

        modelBuilder
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
            entity.HasKey(e => e.Id).HasName("adminaction_pkey");

            entity.ToTable("AdminAction");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ActionDate).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.User).WithMany(p => p.AdminActions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adminaction_adminid_fkey");
        });

        modelBuilder.Entity<BannedUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bannedusers_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.BanDate).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.BannedBy).WithMany(p => p.BannedUserBannedBies)
                .HasForeignKey(d => d.BannedById)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bannedusers_bannedbyid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.BannedUserUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bannedusers_userid_fkey");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("brands_pkey");

            entity.HasIndex(e => e.UserId, "Brands_UserId_key").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.User).WithOne(p => p.Brand)
                .HasForeignKey<Brand>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("brands_userid_fkey");
        });

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("campaigns_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Budget).HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");
            entity.Property(e => e.Status).HasDefaultValueSql("0");

            entity.HasOne(d => d.Brand).WithMany(p => p.Campaigns)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("campaigns_brandid_fkey");

            entity.HasMany(d => d.Tags).WithMany(p => p.Campaigns)
                .UsingEntity<Dictionary<string, object>>(
                    "CampaignTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("campaigntags_tagid_fkey"),
                    l => l.HasOne<Campaign>().WithMany()
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("campaigntags_campaignid_fkey"),
                    j =>
                    {
                        j.HasKey("CampaignId", "TagId").HasName("CampaignTags_pkey");
                        j.ToTable("CampaignTags");
                    });
        });

        modelBuilder.Entity<CampaignContent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("CampaignContents_pkey");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Price).HasDefaultValueSql("0");
            entity.Property(e => e.TargetReaction).HasDefaultValueSql("0");

            entity.HasOne(d => d.Campaign).WithMany(p => p.CampaignContents)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CampaignContents_CampaignId_fkey");
        });

        modelBuilder.Entity<CampaignImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("images1_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.Campaign).WithMany(p => p.CampaignImages)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("images_campaignid_fkey");
        });

        modelBuilder.Entity<Channel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("channels_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");
            entity.Property(e => e.UserName).HasMaxLength(255);

            entity.HasOne(d => d.Influencer).WithMany(p => p.Channels)
                .HasForeignKey(d => d.InfluencerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("channels_influencerid_fkey");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("feedbacks_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.Influencer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.InfluencerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("feedbacks_influencerid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("feedbacks_userid_fkey");
        });

        modelBuilder.Entity<Influencer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("influencers_pkey");

            entity.HasIndex(e => e.UserId, "Influencers_UserId_key").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");
            entity.Property(e => e.RateAverage).HasDefaultValueSql("'0'::numeric");

            entity.HasOne(d => d.User).WithOne(p => p.Influencer)
                .HasForeignKey<Influencer>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Influencers_UserId_fkey");

            entity.HasMany(d => d.Tags).WithMany(p => p.Influencers)
                .UsingEntity<Dictionary<string, object>>(
                    "InfluencerTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("influencertags_tagid_fkey"),
                    l => l.HasOne<Influencer>().WithMany()
                        .HasForeignKey("InfluencerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("influencertags_influencerid_fkey"),
                    j =>
                    {
                        j.HasKey("InfluencerId", "TagId").HasName("InfluencerTags_pkey");
                        j.ToTable("InfluencerTags");
                    });
        });

        modelBuilder.Entity<InfluencerImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("images_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.Influencer).WithMany(p => p.InfluencerImages)
                .HasForeignKey(d => d.InfluencerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("images_influencerid_fkey");
        });

        modelBuilder.Entity<InfluencerReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("influencerreports_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");
            entity.Property(e => e.Reason).HasDefaultValueSql("0");
            entity.Property(e => e.ReportStatus).HasDefaultValueSql("0");

            entity.HasOne(d => d.Influencer).WithMany(p => p.InfluencerReports)
                .HasForeignKey(d => d.InfluencerId)
                .HasConstraintName("InfluencerReports_InfluencerId_fkey");

            entity.HasOne(d => d.Reporter).WithMany(p => p.InfluencerReports)
                .HasForeignKey(d => d.ReporterId)
                .HasConstraintName("InfluencerReports_ReporterId_fkey");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("jobs_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Campaign).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("jobs_campaignid_fkey");

            entity.HasOne(d => d.Influencer).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.InfluencerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("jobs_influencerid_fkey");
        });

        modelBuilder.Entity<JobDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("jobdetails_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.UpdateDate).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.Job).WithMany(p => p.JobDetails)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("jobdetails_jobid_fkey");
        });

        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("offer_pkey");

            entity.ToTable("Offer");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.Job).WithMany(p => p.Offers)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("offer_jobid_fkey");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("packages_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Influencer).WithMany(p => p.Packages)
                .HasForeignKey(d => d.InfluencerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("packages_influencerid_fkey");
        });

        modelBuilder.Entity<PaymentBooking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payments_pkey");

            entity.ToTable("PaymentBooking");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");
            entity.Property(e => e.Type).HasDefaultValueSql("0");

            entity.HasOne(d => d.Job).WithMany(p => p.PaymentBookings)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("payments_brandid_fkey");
        });

        modelBuilder.Entity<PaymentHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payments1_pkey");

            entity.ToTable("PaymentHistory");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Date).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.User).WithMany(p => p.PaymentHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("payments_userid_fkey");
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("systemsetting_pkey");

            entity.ToTable("SystemSetting");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ModifiedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tags_pkey");

            entity.HasIndex(e => e.Name, "tags_tagname_key").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
        });

        modelBuilder.Entity<UserDevice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserDevices_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.LastLoginTime).HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)");

            entity.HasOne(d => d.User).WithMany(p => p.UserDevices)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("UserDevices_UserId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
