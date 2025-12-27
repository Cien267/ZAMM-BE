using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zamm.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Building = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UnitNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    StreetNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    StreetName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Suburb = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Postcode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OffPlan = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "brokerages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AuthorisedDomain = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsMasterAccount = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brokerages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lenders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lenders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AddressOffPlan = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PropertyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ZoningType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ValueIsCertified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ValuationDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsInvestment = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RentalIncomeValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RentalIncomeFrequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RentalHasAgent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RentalAgentContact = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsUnencumbered = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_assets_addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "brokerage_logos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrokerageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brokerage_logos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_brokerage_logos_brokerages_BrokerageId",
                        column: x => x.BrokerageId,
                        principalTable: "brokerages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    HasOnboarded = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BrokerageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_brokerages_BrokerageId",
                        column: x => x.BrokerageId,
                        principalTable: "brokerages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "loans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_loans_lenders_LenderId",
                        column: x => x.LenderId,
                        principalTable: "lenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TradingName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Abn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Acn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "date", nullable: true),
                    PhoneWork = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Industry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActingOnTrust = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TrustName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExternalContactName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExternalContactEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExternalContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsContactExistingPerson = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BrokerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_companies_addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_companies_users_BrokerId",
                        column: x => x.BrokerId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "confirm_emails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfirmCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_confirm_emails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_confirm_emails_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BrokerageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InviterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_invitations_brokerages_BrokerageId",
                        column: x => x.BrokerageId,
                        principalTable: "brokerages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_invitations_users_InviterId",
                        column: x => x.InviterId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "people",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PreferredName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: true),
                    NotifyOfBirthday = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Gender = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaritalStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PhoneWork = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PhoneMobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PhonePreference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ActingOnTrust = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TrustName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SpouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BrokerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_people", x => x.Id);
                    table.ForeignKey(
                        name: "FK_people_addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_people_people_SpouseId",
                        column: x => x.SpouseId,
                        principalTable: "people",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_people_users_BrokerId",
                        column: x => x.BrokerId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_permissions_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_permissions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "interest_rates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RateType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_interest_rates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_interest_rates_loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "liabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LoanTerm = table.Column<int>(type: "int", nullable: true),
                    InterestOnlyTerm = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: true),
                    FinancePurpose = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InitialBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IntroRateYears = table.Column<int>(type: "int", nullable: true),
                    IntroRatePercent = table.Column<decimal>(type: "decimal(5,4)", nullable: true),
                    RepaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RepaymentFrequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,4)", nullable: true),
                    SettlementRate = table.Column<decimal>(type: "decimal(5,4)", nullable: true),
                    BankAccountName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BankAccountBsb = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OffsetAccountBsb = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    OffsetAccountNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_liabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_liabilities_loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asset_companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Percent = table.Column<decimal>(type: "decimal(5,2)", nullable: false, comment: "Ownership percentage (0-100)"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asset_companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_asset_companies_assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_asset_companies_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "asset_people",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Percent = table.Column<decimal>(type: "decimal(5,2)", nullable: false, comment: "Ownership percentage (0-100)"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asset_people", x => x.Id);
                    table.ForeignKey(
                        name: "FK_asset_people_assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_asset_people_people_PersonId",
                        column: x => x.PersonId,
                        principalTable: "people",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "company_people",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_people", x => x.Id);
                    table.ForeignKey(
                        name: "FK_company_people_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_company_people_people_PersonId",
                        column: x => x.PersonId,
                        principalTable: "people",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsRepeating = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RepeatNumber = table.Column<int>(type: "int", nullable: true),
                    RepeatUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDismissed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RepeatingDateDismissed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedValuesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedValuesObject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LiabilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_events_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_events_liabilities_LiabilityId",
                        column: x => x.LiabilityId,
                        principalTable: "liabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_events_people_PersonId",
                        column: x => x.PersonId,
                        principalTable: "people",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_events_users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fixed_rate_periods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LiabilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    Term = table.Column<int>(type: "int", nullable: false),
                    CustomRate = table.Column<decimal>(type: "decimal(5,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fixed_rate_periods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fixed_rate_periods_liabilities_LiabilityId",
                        column: x => x.LiabilityId,
                        principalTable: "liabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "liability_assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LiabilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_liability_assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_liability_assets_assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_liability_assets_liabilities_LiabilityId",
                        column: x => x.LiabilityId,
                        principalTable: "liabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "liability_companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LiabilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Percent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_liability_companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_liability_companies_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_liability_companies_liabilities_LiabilityId",
                        column: x => x.LiabilityId,
                        principalTable: "liabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "liability_people",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LiabilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Percent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_liability_people", x => x.Id);
                    table.ForeignKey(
                        name: "FK_liability_people_liabilities_LiabilityId",
                        column: x => x.LiabilityId,
                        principalTable: "liabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_liability_people_people_PersonId",
                        column: x => x.PersonId,
                        principalTable: "people",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "event_files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Filename = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_files_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EditedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LiabilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notes_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_notes_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_notes_liabilities_LiabilityId",
                        column: x => x.LiabilityId,
                        principalTable: "liabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_notes_people_PersonId",
                        column: x => x.PersonId,
                        principalTable: "people",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_notes_users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notes_users_EditedById",
                        column: x => x.EditedById,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_Country",
                table: "addresses",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_Address_Postcode_State",
                table: "addresses",
                columns: new[] { "Postcode", "State" });

            migrationBuilder.CreateIndex(
                name: "IX_Address_Suburb",
                table: "addresses",
                column: "Suburb");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCompany_AssetId",
                table: "asset_companies",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCompany_AssetId_CompanyId_Unique",
                table: "asset_companies",
                columns: new[] { "AssetId", "CompanyId" },
                unique: true,
                filter: "[DeletedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCompany_CompanyId",
                table: "asset_companies",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCompany_DeletedAt",
                table: "asset_companies",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPerson_AssetId",
                table: "asset_people",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPerson_AssetId_PersonId_Unique",
                table: "asset_people",
                columns: new[] { "AssetId", "PersonId" },
                unique: true,
                filter: "[DeletedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPerson_DeletedAt",
                table: "asset_people",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPerson_PersonId",
                table: "asset_people",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_AddressId",
                table: "assets",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_IsInvestment",
                table: "assets",
                column: "IsInvestment");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Name",
                table: "assets",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_PropertyType",
                table: "assets",
                column: "PropertyType");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Unencumbered_Value",
                table: "assets",
                columns: new[] { "IsUnencumbered", "Value" });

            migrationBuilder.CreateIndex(
                name: "IX_Asset_ValuationDate",
                table: "assets",
                column: "ValuationDate");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerageLogo_BrokerageId",
                table: "brokerage_logos",
                column: "BrokerageId");

            migrationBuilder.CreateIndex(
                name: "IX_Brokerage_AuthorisedDomain",
                table: "brokerages",
                column: "AuthorisedDomain");

            migrationBuilder.CreateIndex(
                name: "IX_Brokerage_IsMasterAccount",
                table: "brokerages",
                column: "IsMasterAccount");

            migrationBuilder.CreateIndex(
                name: "IX_Brokerage_Name",
                table: "brokerages",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Brokerage_Slug",
                table: "brokerages",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Company_Abn",
                table: "companies",
                column: "Abn");

            migrationBuilder.CreateIndex(
                name: "IX_Company_Acn",
                table: "companies",
                column: "Acn");

            migrationBuilder.CreateIndex(
                name: "IX_Company_ActingOnTrust",
                table: "companies",
                column: "ActingOnTrust");

            migrationBuilder.CreateIndex(
                name: "IX_Company_AddressId",
                table: "companies",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_BrokerId",
                table: "companies",
                column: "BrokerId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_Email",
                table: "companies",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Company_Name",
                table: "companies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPerson_CompanyId",
                table: "company_people",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPerson_CompanyId_PersonId_Unique",
                table: "company_people",
                columns: new[] { "CompanyId", "PersonId" },
                unique: true,
                filter: "[DeletedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPerson_DeletedAt",
                table: "company_people",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPerson_PersonId",
                table: "company_people",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmEmail_ConfirmCode",
                table: "confirm_emails",
                column: "ConfirmCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmEmail_ExpiryTime",
                table: "confirm_emails",
                column: "ExpiryTime");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmEmail_UserId",
                table: "confirm_emails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmEmail_UserId_IsConfirmed",
                table: "confirm_emails",
                columns: new[] { "UserId", "IsConfirmed" });

            migrationBuilder.CreateIndex(
                name: "IX_EventFile_DeletedAt",
                table: "event_files",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EventFile_EventId",
                table: "event_files",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_AddedByUserId",
                table: "events",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_CompanyId",
                table: "events",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Date",
                table: "events",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Event_DeletedAt",
                table: "events",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Event_IsDismissed_Date",
                table: "events",
                columns: new[] { "IsDismissed", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Event_IsRepeating_Date",
                table: "events",
                columns: new[] { "IsRepeating", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Event_LiabilityId",
                table: "events",
                column: "LiabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_PersonId",
                table: "events",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Type",
                table: "events",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_FixedRatePeriod_DeletedAt",
                table: "fixed_rate_periods",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FixedRatePeriod_LiabilityId",
                table: "fixed_rate_periods",
                column: "LiabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedRatePeriod_LiabilityId_StartDate",
                table: "fixed_rate_periods",
                columns: new[] { "LiabilityId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_InterestRate_LoanId",
                table: "interest_rates",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestRate_LoanId_CreatedAt",
                table: "interest_rates",
                columns: new[] { "LoanId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InterestRate_RateType",
                table: "interest_rates",
                column: "RateType");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_BrokerageId",
                table: "invitations",
                column: "BrokerageId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_Email",
                table: "invitations",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_Email_BrokerageId_IsComplete",
                table: "invitations",
                columns: new[] { "Email", "BrokerageId", "IsComplete" });

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_InviterId",
                table: "invitations",
                column: "InviterId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_IsComplete_CreatedAt",
                table: "invitations",
                columns: new[] { "IsComplete", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Lender_DeletedAt",
                table: "lenders",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Lender_Name",
                table: "lenders",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Lender_Slug_Unique",
                table: "lenders",
                column: "Slug",
                unique: true,
                filter: "[DeletedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Liability_FinancePurpose",
                table: "liabilities",
                column: "FinancePurpose");

            migrationBuilder.CreateIndex(
                name: "IX_Liability_LoanId",
                table: "liabilities",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Liability_StartDate",
                table: "liabilities",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityAsset_AssetId",
                table: "liability_assets",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityAsset_DeletedAt",
                table: "liability_assets",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityAsset_LiabilityId",
                table: "liability_assets",
                column: "LiabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityAsset_LiabilityId_AssetId_Unique",
                table: "liability_assets",
                columns: new[] { "LiabilityId", "AssetId" },
                unique: true,
                filter: "[DeletedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityCompany_CompanyId",
                table: "liability_companies",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityCompany_DeletedAt",
                table: "liability_companies",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityCompany_LiabilityId",
                table: "liability_companies",
                column: "LiabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityCompany_LiabilityId_CompanyId_Unique",
                table: "liability_companies",
                columns: new[] { "LiabilityId", "CompanyId" },
                unique: true,
                filter: "[DeletedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityPerson_DeletedAt",
                table: "liability_people",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityPerson_LiabilityId",
                table: "liability_people",
                column: "LiabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityPerson_LiabilityId_PersonId_Unique",
                table: "liability_people",
                columns: new[] { "LiabilityId", "PersonId" },
                unique: true,
                filter: "[DeletedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityPerson_PersonId",
                table: "liability_people",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Loan_DeletedAt",
                table: "loans",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Loan_LenderId",
                table: "loans",
                column: "LenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Loan_Name",
                table: "loans",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Note_AuthorId",
                table: "notes",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Note_CompanyId",
                table: "notes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Note_CreatedAt",
                table: "notes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Note_DeletedAt",
                table: "notes",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Note_EventId",
                table: "notes",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Note_LiabilityId",
                table: "notes",
                column: "LiabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Note_PersonId",
                table: "notes",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_notes_EditedById",
                table: "notes",
                column: "EditedById");

            migrationBuilder.CreateIndex(
                name: "IX_Person_ActingOnTrust",
                table: "people",
                column: "ActingOnTrust");

            migrationBuilder.CreateIndex(
                name: "IX_Person_AddressId",
                table: "people",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_BrokerId",
                table: "people",
                column: "BrokerId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_DateOfBirth",
                table: "people",
                column: "DateOfBirth");

            migrationBuilder.CreateIndex(
                name: "IX_Person_Email",
                table: "people",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Person_FirstName_LastName",
                table: "people",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Person_SpouseId",
                table: "people",
                column: "SpouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleId",
                table: "permissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_UserId",
                table: "permissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_UserId_RoleId_Unique",
                table: "permissions",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_ExpiryTime",
                table: "refresh_tokens",
                column: "ExpiryTime");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_Token",
                table: "refresh_tokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "refresh_tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId_IsRevoked_ExpiryTime",
                table: "refresh_tokens",
                columns: new[] { "UserId", "IsRevoked", "ExpiryTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Role_RoleCode",
                table: "roles",
                column: "RoleCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_RoleName",
                table: "roles",
                column: "RoleName");

            migrationBuilder.CreateIndex(
                name: "IX_User_BrokerageId",
                table: "users",
                column: "BrokerageId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_UserName",
                table: "users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asset_companies");

            migrationBuilder.DropTable(
                name: "asset_people");

            migrationBuilder.DropTable(
                name: "brokerage_logos");

            migrationBuilder.DropTable(
                name: "company_people");

            migrationBuilder.DropTable(
                name: "confirm_emails");

            migrationBuilder.DropTable(
                name: "event_files");

            migrationBuilder.DropTable(
                name: "fixed_rate_periods");

            migrationBuilder.DropTable(
                name: "interest_rates");

            migrationBuilder.DropTable(
                name: "invitations");

            migrationBuilder.DropTable(
                name: "liability_assets");

            migrationBuilder.DropTable(
                name: "liability_companies");

            migrationBuilder.DropTable(
                name: "liability_people");

            migrationBuilder.DropTable(
                name: "notes");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "assets");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "companies");

            migrationBuilder.DropTable(
                name: "liabilities");

            migrationBuilder.DropTable(
                name: "people");

            migrationBuilder.DropTable(
                name: "loans");

            migrationBuilder.DropTable(
                name: "addresses");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "lenders");

            migrationBuilder.DropTable(
                name: "brokerages");
        }
    }
}
