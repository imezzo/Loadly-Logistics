
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using LoadlyAggregator.Infrastructure.Data;
#nullable disable
namespace LoadlyAggregator.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20251111193534_CreateDistrictsTable")]
    partial class CreateDistrictsTable
    {
        
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Address", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<string>("BuildingNumber")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("building_number");
                    b.Property<string>("BuildingType")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("building_type");
                    b.Property<long?>("CityId")
                        .HasColumnType("bigint")
                        .HasColumnName("city_id");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<long?>("DistrictId")
                        .HasColumnType("bigint")
                        .HasColumnName("district_id");
                    b.Property<long?>("GovernerateId")
                        .HasColumnType("bigint")
                        .HasColumnName("governerate_id");
                    b.Property<long>("ModelId")
                        .HasColumnType("bigint")
                        .HasColumnName("model_id");
                    b.Property<string>("ModelType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("model_type");
                    b.Property<string>("PostalCode")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("postal_code");
                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("status");
                    b.Property<string>("StreetName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("street_name");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("Status");
                    b.HasIndex("ModelId", "ModelType");
                    b.ToTable("addresses", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Admin", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("email");
                    b.Property<bool>("EmailVerified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("email_verified");
                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("full_name");
                    b.Property<bool>("Mfa")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("mfa");
                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("password");
                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("phone");
                    b.Property<bool>("PhoneVerified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("phone_verified");
                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");
                    b.Property<int>("Type")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(2)
                        .HasColumnName("type");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("username");
                    b.HasKey("Id");
                    b.HasIndex("Email")
                        .IsUnique();
                    b.HasIndex("Phone")
                        .IsUnique();
                    b.HasIndex("Status");
                    b.HasIndex("Type");
                    b.HasIndex("UserName")
                        .IsUnique();
                    b.ToTable("admins", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.City", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<long>("GovernerateId")
                        .HasColumnType("bigint")
                        .HasColumnName("governerate_id");
                    b.Property<string>("NameAr")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_ar");
                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_en");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("GovernerateId");
                    b.ToTable("cities", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Company", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("email");
                    b.Property<bool>("EmailVerified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("email_verified");
                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("full_name");
                    b.Property<bool>("Mfa")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("mfa");
                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("password");
                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("phone");
                    b.Property<bool>("PhoneVerified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("phone_verified");
                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");
                    b.Property<int>("Type")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("type");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("username");
                    b.HasKey("Id");
                    b.HasIndex("Email")
                        .IsUnique();
                    b.HasIndex("Phone")
                        .IsUnique();
                    b.HasIndex("Status");
                    b.HasIndex("Type");
                    b.HasIndex("UserName")
                        .IsUnique();
                    b.ToTable("companies", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.CompanyDetail", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<long?>("BusinessHourId")
                        .HasColumnType("bigint")
                        .HasColumnName("business_hour_id");
                    b.Property<long?>("CommunicationsMethodId")
                        .HasColumnType("bigint")
                        .HasColumnName("communications_method_id");
                    b.Property<long>("CompanyId")
                        .HasColumnType("bigint")
                        .HasColumnName("company_id");
                    b.Property<string>("CompanySize")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("company_size");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<long?>("IndustryTypeId")
                        .HasColumnType("bigint")
                        .HasColumnName("industry_type_id");
                    b.Property<string>("NameAr")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_ar");
                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_en");
                    b.Property<string>("QrCode")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("qr_code");
                    b.Property<string>("RegistrationNumber")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("registration_number");
                    b.Property<string>("ServiceRequirementsIds")
                        .HasColumnType("text")
                        .HasColumnName("service_requirements_ids");
                    b.Property<string>("TaxId")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("tax_id");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.Property<int>("VerificationStatus")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("verification_status");
                    b.Property<string>("WebsiteUrl")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("website_url");
                    b.HasKey("Id");
                    b.HasIndex("CompanyId");
                    b.HasIndex("VerificationStatus");
                    b.ToTable("company_details", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.District", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<long>("CityId")
                        .HasColumnType("bigint")
                        .HasColumnName("city_id");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<string>("NameAr")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_ar");
                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_en");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("CityId");
                    b.ToTable("districts", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Driver", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("email");
                    b.Property<bool>("EmailVerified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("email_verified");
                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("full_name");
                    b.Property<bool>("Mfa")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("mfa");
                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("password");
                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("phone");
                    b.Property<bool>("PhoneVerified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("phone_verified");
                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("username");
                    b.HasKey("Id");
                    b.HasIndex("Email")
                        .IsUnique();
                    b.HasIndex("Phone")
                        .IsUnique();
                    b.HasIndex("Status");
                    b.HasIndex("UserName")
                        .IsUnique();
                    b.ToTable("drivers", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Governerate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<string>("NameAr")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_ar");
                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_en");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.ToTable("governerates", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ModelHasPermission", b =>
                {
                    b.Property<long>("PermissionId")
                        .HasColumnType("bigint")
                        .HasColumnName("permission_id");
                    b.Property<long>("ModelId")
                        .HasColumnType("bigint")
                        .HasColumnName("model_id");
                    b.Property<string>("ModelType")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("model_type");
                    b.HasKey("PermissionId", "ModelId", "ModelType");
                    b.HasIndex("ModelId", "ModelType");
                    b.ToTable("model_has_permissions", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ModelHasRole", b =>
                {
                    b.Property<long>("RoleId")
                        .HasColumnType("bigint")
                        .HasColumnName("role_id");
                    b.Property<long>("ModelId")
                        .HasColumnType("bigint")
                        .HasColumnName("model_id");
                    b.Property<string>("ModelType")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("model_type");
                    b.HasKey("RoleId", "ModelId", "ModelType");
                    b.HasIndex("ModelId", "ModelType");
                    b.ToTable("model_has_roles", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Permission", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<string>("GuardName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("guard_name");
                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name");
                    b.Property<string>("NameAr")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_ar");
                    b.Property<long>("PermissionGroupId")
                        .HasColumnType("bigint")
                        .HasColumnName("permission_group_id");
                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("PermissionGroupId");
                    b.HasIndex("Name", "GuardName")
                        .IsUnique();
                    b.ToTable("permissions", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.PermissionGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name");
                    b.Property<string>("NameAr")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_ar");
                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");
                    b.Property<string>("Type")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasDefaultValue("company")
                        .HasColumnName("type");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("Type");
                    b.ToTable("permission_groups", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<string>("GuardName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("guard_name");
                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("GuardName");
                    b.HasIndex("Name");
                    b.HasIndex("Name", "GuardName")
                        .IsUnique();
                    b.ToTable("roles", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.RoleHasPermission", b =>
                {
                    b.Property<long>("PermissionId")
                        .HasColumnType("bigint")
                        .HasColumnName("permission_id");
                    b.Property<long>("RoleId")
                        .HasColumnType("bigint")
                        .HasColumnName("role_id");
                    b.HasKey("PermissionId", "RoleId");
                    b.HasIndex("RoleId");
                    b.ToTable("role_has_permissions", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.CompanyDetail", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Company");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ModelHasPermission", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Admin", null)
                        .WithMany("DirectPermissions")
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Company", null)
                        .WithMany("DirectPermissions")
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Driver", null)
                        .WithMany("DirectPermissions")
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Permission", "Permission")
                        .WithMany("ModelPermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.Navigation("Permission");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ModelHasRole", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Admin", null)
                        .WithMany("UserRoles")
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Company", null)
                        .WithMany("UserRoles")
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Driver", null)
                        .WithMany("UserRoles")
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.Navigation("Role");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Permission", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.PermissionGroup", "PermissionGroup")
                        .WithMany("Permissions")
                        .HasForeignKey("PermissionGroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("PermissionGroup");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.RoleHasPermission", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.Navigation("Permission");
                    b.Navigation("Role");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Admin", b =>
                {
                    b.Navigation("DirectPermissions");
                    b.Navigation("UserRoles");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Company", b =>
                {
                    b.Navigation("DirectPermissions");
                    b.Navigation("UserRoles");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Driver", b =>
                {
                    b.Navigation("DirectPermissions");
                    b.Navigation("UserRoles");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Permission", b =>
                {
                    b.Navigation("ModelPermissions");
                    b.Navigation("RolePermissions");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.PermissionGroup", b =>
                {
                    b.Navigation("Permissions");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Role", b =>
                {
                    b.Navigation("RolePermissions");
                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}