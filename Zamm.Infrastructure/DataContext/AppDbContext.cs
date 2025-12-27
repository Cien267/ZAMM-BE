using Microsoft.EntityFrameworkCore;
using Zamm.Domain.Entities;
using System.Threading.Tasks;

namespace Zamm.Infrastructure.DataContext
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Permissions> Permissions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<ConfirmEmail> ConfirmEmails { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Person> People => Set<Person>();
        public DbSet<Asset> Assets => Set<Asset>();
        public DbSet<AssetCompany> AssetCompanies => Set<AssetCompany>();
        public DbSet<AssetPerson> AssetPeople => Set<AssetPerson>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<CompanyPerson> CompanyPeople => Set<CompanyPerson>();
        public DbSet<Liability> Liabilities => Set<Liability>();
        public DbSet<LiabilityAsset> LiabilityAssets => Set<LiabilityAsset>();
        public DbSet<LiabilityCompany> LiabilityCompanies => Set<LiabilityCompany>();
        public DbSet<LiabilityPerson> LiabilityPeople => Set<LiabilityPerson>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<EventFile> EventFiles => Set<EventFile>();
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<FixedRatePeriod> FixedRatePeriods => Set<FixedRatePeriod>();
        public DbSet<InterestRate> InterestRates => Set<InterestRate>();
        public DbSet<Invitation> Invitations => Set<Invitation>();
        public DbSet<Lender> Lenders => Set<Lender>();
        public DbSet<Brokerage> Brokerages => Set<Brokerage>();
        public DbSet<BrokerageLogo> BrokerageLogos => Set<BrokerageLogo>();
        public DbSet<Loan> Loans => Set<Loan>();
        public DbSet<Dependent> Dependents => Set<Dependent>();
        public async Task<int> CommitChangeAsync()
        {
            return await SaveChangesAsync();
        }

        public DbSet<TEntity> SetEntity<TEntity>() where TEntity : class
        {
            return Set<TEntity>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<RefreshToken>()
                .HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConfirmEmail>()
                .HasOne(c => c.User)
                .WithMany(u => u.ConfirmEmails)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Permissions>()
                .HasOne(p => p.User)
                .WithMany(u => u.Permissions)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Permissions>()
                .HasOne(p => p.Role)
                .WithMany(r => r.Permissions)
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade);*/

            base.OnModelCreating(modelBuilder);
            modelBuilder.BuildUserModel();
            modelBuilder.BuildRoleModel();
            modelBuilder.BuildPermissionsModel();
            modelBuilder.BuildRefreshTokenModel();
            modelBuilder.BuildConfirmEmailModel();
            modelBuilder.BuildAddressModel();
            modelBuilder.BuildAssetModel();
            modelBuilder.BuildAssetCompanyModel();
            modelBuilder.BuildAssetPersonModel();
            modelBuilder.BuildCompanyModel();
            modelBuilder.BuildCompanyPersonModel();
            modelBuilder.BuildLiabilityModel();
            modelBuilder.BuildLiabilityAssetModel();
            modelBuilder.BuildLiabilityCompanyModel();
            modelBuilder.BuildLiabilityPersonModel();
            modelBuilder.BuildEventModel();
            modelBuilder.BuildEventFileModel();
            modelBuilder.BuildNoteModel();
            modelBuilder.BuildFixedRatePeriodModel();
            modelBuilder.BuildInterestRateModel();
            modelBuilder.BuildBrokerageModel();
            modelBuilder.BuildBrokerageLogoModel();
            modelBuilder.BuildLoanModel();
            modelBuilder.BuildLenderModel();
            modelBuilder.BuildInvitationModel();
            modelBuilder.BuildPersonModel();
            modelBuilder.BuildDependentModel();
        }
    }
}
