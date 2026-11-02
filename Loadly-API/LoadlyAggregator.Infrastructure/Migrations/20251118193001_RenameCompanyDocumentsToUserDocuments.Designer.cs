
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
    [Migration("20251118193001_RenameCompanyDocumentsToUserDocuments")]
    partial class RenameCompanyDocumentsToUserDocuments
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.CommunicationMethod", b =>
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
                    b.ToTable("communication_methods", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Company", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("CompanyId")
                        .HasColumnType("bigint")
                        .HasColumnName("company_id");
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
                    b.HasIndex("CompanyId");
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.CompanyContract", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<long>("CompanyId")
                        .HasColumnType("bigint")
                        .HasColumnName("company_id");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end_date");
                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_date");
                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("status");
                    b.Property<string>("TermsAndConditions")
                        .HasColumnType("text")
                        .HasColumnName("terms_and_conditions");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.Property<string>("VehicleTypeIds")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("vehicle_type_ids");
                    b.HasKey("Id");
                    b.HasIndex("CompanyId");
                    b.HasIndex("EndDate");
                    b.HasIndex("StartDate");
                    b.HasIndex("Status");
                    b.ToTable("company_contracts", (string)null);
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.CompanyDocument", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<long>("CompanyId")
                        .HasColumnType("bigint")
                        .HasColumnName("company_id");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<long>("DocumentId")
                        .HasColumnType("bigint")
                        .HasColumnName("document_id");
                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expiry_date");
                    b.Property<string>("Path")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("path");
                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("status");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("CompanyId");
                    b.HasIndex("DocumentId");
                    b.HasIndex("ExpiryDate");
                    b.HasIndex("Status");
                    b.ToTable("company_documents", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Contact", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<long?>("CommunicationMethodId")
                        .HasColumnType("bigint")
                        .HasColumnName("communication_method_id");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("email");
                    b.Property<long>("ModelId")
                        .HasColumnType("bigint")
                        .HasColumnName("model_id");
                    b.Property<string>("ModelType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("model_type");
                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name");
                    b.Property<string>("Phone")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("phone");
                    b.Property<string>("Position")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("position");
                    b.Property<bool>("Primary")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("primary");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("CommunicationMethodId");
                    b.HasIndex("Primary");
                    b.HasIndex("ModelId", "ModelType");
                    b.ToTable("contacts", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ContractPackage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<bool>("AutoRenew")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("auto_renew");
                    b.Property<int>("CalculationMethod")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("calculation_method");
                    b.Property<long>("ContractId")
                        .HasColumnType("bigint")
                        .HasColumnName("contract_id");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<long?>("DiscountId")
                        .HasColumnType("bigint")
                        .HasColumnName("discount_id");
                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end_date");
                    b.Property<int?>("ExpiryDateAlert")
                        .HasColumnType("integer")
                        .HasColumnName("expiry_date_alert");
                    b.Property<string>("KmConsumptionAlert")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("km_consumption_alert");
                    b.Property<long>("PackageId")
                        .HasColumnType("bigint")
                        .HasColumnName("package_id");
                    b.Property<bool>("Rollover")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("rollover");
                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_date");
                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("status");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("ContractId");
                    b.HasIndex("EndDate");
                    b.HasIndex("PackageId");
                    b.HasIndex("StartDate");
                    b.HasIndex("Status");
                    b.ToTable("contract_packages", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Discount", b =>
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
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_ar");
                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_en");
                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("status");
                    b.Property<int>("Type")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("type");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("value");
                    b.HasKey("Id");
                    b.HasIndex("Status");
                    b.HasIndex("Type");
                    b.ToTable("discounts", (string)null);
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Document", b =>
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
                    b.Property<long>("ModelId")
                        .HasColumnType("bigint")
                        .HasColumnName("model_id");
                    b.Property<string>("ModelType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("model_type");
                    b.Property<string>("NameAr")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_ar");
                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_en");
                    b.Property<bool>("Required")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("required");
                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("status");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("Status");
                    b.HasIndex("ModelId", "ModelType");
                    b.ToTable("documents", (string)null);
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.IndustryType", b =>
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
                    b.ToTable("industry_types", (string)null);
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Package", b =>
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
                    b.Property<string>("DescAr")
                        .HasColumnType("text")
                        .HasColumnName("desc_ar");
                    b.Property<string>("DescEn")
                        .HasColumnType("text")
                        .HasColumnName("desc_en");
                    b.Property<long?>("DiscountId")
                        .HasColumnType("bigint")
                        .HasColumnName("discount_id");
                    b.Property<decimal>("GrandTotal")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("grand_total");
                    b.Property<int?>("IncludedKm")
                        .HasColumnType("integer")
                        .HasColumnName("included_km");
                    b.Property<decimal?>("KmUnitPrice")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("km_unit_price");
                    b.Property<decimal?>("MaxRollover")
                        .HasColumnType("decimal(5,2)")
                        .HasColumnName("max_rollover");
                    b.Property<string>("NameAr")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_ar");
                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_en");
                    b.Property<decimal?>("OverageRate")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("overage_rate");
                    b.Property<int>("PeriodType")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("period_type");
                    b.Property<bool>("Recommended")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("recommended");
                    b.Property<string>("RolloverPolicy")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("rollover_policy");
                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("status");
                    b.Property<decimal>("SubTotal")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("sub_total");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.Property<long?>("VatId")
                        .HasColumnType("bigint")
                        .HasColumnName("vat_id");
                    b.HasKey("Id");
                    b.HasIndex("PeriodType");
                    b.HasIndex("Recommended");
                    b.HasIndex("Status");
                    b.ToTable("packages", (string)null);
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ServiceRequirement", b =>
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
                    b.ToTable("service_requirements", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Tax", b =>
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
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_ar");
                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_en");
                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("status");
                    b.Property<int>("Type")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("type");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("value");
                    b.HasKey("Id");
                    b.HasIndex("Status");
                    b.HasIndex("Type");
                    b.ToTable("taxes", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.UserDocument", b =>
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
                    b.Property<long>("DocumentId")
                        .HasColumnType("bigint")
                        .HasColumnName("document_id");
                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expiry_date");
                    b.Property<long>("ModelId")
                        .HasColumnType("bigint")
                        .HasColumnName("model_id");
                    b.Property<string>("ModelType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("model_type");
                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("status");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("DocumentId");
                    b.HasIndex("ExpiryDate");
                    b.HasIndex("Status");
                    b.HasIndex("ModelId", "ModelType");
                    b.ToTable("user_documents", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.VehicleType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<int>("Category")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("category");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<string>("NameAr")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_ar");
                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name_en");
                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("status");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("Category");
                    b.HasIndex("Status");
                    b.ToTable("vehicle_types", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Company", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Company", "ParentCompany")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Restrict);
                    b.Navigation("ParentCompany");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.CompanyContract", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Company");
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.CompanyDocument", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Document", "Document")
                        .WithMany()
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Company");
                    b.Navigation("Document");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Contact", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.CommunicationMethod", "CommunicationMethod")
                        .WithMany()
                        .HasForeignKey("CommunicationMethodId")
                        .OnDelete(DeleteBehavior.Restrict);
                    b.Navigation("CommunicationMethod");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ContractPackage", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.CompanyContract", "Contract")
                        .WithMany()
                        .HasForeignKey("ContractId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Package", "Package")
                        .WithMany()
                        .HasForeignKey("PackageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Contract");
                    b.Navigation("Package");
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.UserDocument", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Document", "Document")
                        .WithMany()
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Document");
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