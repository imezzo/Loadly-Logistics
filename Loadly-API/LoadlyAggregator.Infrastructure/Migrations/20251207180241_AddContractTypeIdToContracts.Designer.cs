
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
    [Migration("20251207180241_AddContractTypeIdToContracts")]
    partial class AddContractTypeIdToContracts
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
                    b.Property<long>("CityId")
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
                    b.Property<long>("GovernerateId")
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
                        .IsRequired()
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Color", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<string>("ColorHash")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("character varying(7)")
                        .HasColumnName("color_hash");
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
                    b.HasIndex("ColorHash")
                        .IsUnique();
                    b.HasIndex("NameAr")
                        .IsUnique();
                    b.HasIndex("NameEn")
                        .IsUnique();
                    b.HasIndex("Status");
                    b.ToTable("colors", (string)null);
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
                        .IsRequired()
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
                        .IsRequired()
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
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("website_url");
                    b.HasKey("Id");
                    b.HasIndex("CompanyId");
                    b.HasIndex("NameAr")
                        .IsUnique();
                    b.HasIndex("RegistrationNumber")
                        .IsUnique();
                    b.HasIndex("VerificationStatus");
                    b.HasIndex("WebsiteUrl")
                        .IsUnique();
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Contract", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<int>("BillingCycle")
                        .HasColumnType("integer")
                        .HasColumnName("billing_cycle");
                    b.Property<long>("CompanyId")
                        .HasColumnType("bigint")
                        .HasColumnName("company_id");
                    b.Property<int>("ConsumptionType")
                        .HasColumnType("integer")
                        .HasColumnName("consumption_type");
                    b.Property<long>("ContractTypeId")
                        .HasColumnType("bigint")
                        .HasColumnName("contract_type_id");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end_date");
                    b.Property<decimal?>("MaxKms")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("max_kms");
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
                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("price");
                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_date");
                    b.Property<int?>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");
                    b.Property<string>("TermsAndConditions")
                        .HasColumnType("text")
                        .HasColumnName("terms_and_conditions");
                    b.Property<int?>("TripsPerDay")
                        .HasColumnType("integer")
                        .HasColumnName("trips_per_day");
                    b.Property<int?>("TripsPerMonth")
                        .HasColumnType("integer")
                        .HasColumnName("trips_per_month");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("CompanyId");
                    b.HasIndex("ContractTypeId");
                    b.HasIndex("NameAr")
                        .IsUnique();
                    b.HasIndex("NameEn")
                        .IsUnique();
                    b.HasIndex("Status");
                    b.ToTable("contracts", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ContractFuelType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<long>("ContractId")
                        .HasColumnType("bigint")
                        .HasColumnName("contract_id");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<long>("FuelTypeId")
                        .HasColumnType("bigint")
                        .HasColumnName("fuel_type_id");
                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(4,2)")
                        .HasColumnName("unit_price");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("ContractId");
                    b.HasIndex("FuelTypeId");
                    b.ToTable("contract_fuel_types", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ContractType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<string>("BestForAr")
                        .HasColumnType("text")
                        .HasColumnName("best_for_ar");
                    b.Property<string>("BestForEn")
                        .HasColumnType("text")
                        .HasColumnName("best_for_en");
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
                    b.HasIndex("Status");
                    b.ToTable("contract_types", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ContractVehicle", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<DateTime>("AssignedFrom")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("assigned_from");
                    b.Property<DateTime?>("AssignedTo")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("assigned_to");
                    b.Property<long>("ContractId")
                        .HasColumnType("bigint")
                        .HasColumnName("contract_id");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("status");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.Property<long>("VehicleId")
                        .HasColumnType("bigint")
                        .HasColumnName("vehicle_id");
                    b.HasKey("Id");
                    b.HasIndex("ContractId");
                    b.HasIndex("Status");
                    b.HasIndex("VehicleId");
                    b.ToTable("contract_vehicles", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ContractVehicleType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<bool>("AutoAssignMode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("auto_assign_mode");
                    b.Property<decimal>("ConsumptionRate")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("consumption_rate");
                    b.Property<long>("ContractId")
                        .HasColumnType("bigint")
                        .HasColumnName("contract_id");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<decimal>("DailyValue")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("daily_value");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<decimal?>("MinCapacity")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("min_capacity");
                    b.Property<decimal?>("OverCapacity")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("over_capacity");
                    b.Property<decimal>("OverConsumption")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("over_consumption");
                    b.Property<int>("Quantity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("quantity");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.Property<long>("VehicleTypeId")
                        .HasColumnType("bigint")
                        .HasColumnName("vehicle_type_id");
                    b.HasKey("Id");
                    b.HasIndex("ContractId");
                    b.HasIndex("VehicleTypeId");
                    b.ToTable("contract_vehicle_types", (string)null);
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
                    b.Property<int>("EntityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("entity_id");
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
                    b.HasIndex("EntityId");
                    b.HasIndex("NameAr")
                        .IsUnique();
                    b.HasIndex("NameEn")
                        .IsUnique();
                    b.HasIndex("Status");
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
                        .HasColumnName("fullname_en");
                    b.Property<string>("FullNameAr")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("fullname_ar");
                    b.Property<bool>("Mfa")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("mfa");
                    b.Property<string>("NationalId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("national_id");
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.DriverDetail", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<int?>("Age")
                        .HasColumnType("integer")
                        .HasColumnName("age");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_of_birth");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<long>("DriverId")
                        .HasColumnType("bigint")
                        .HasColumnName("driver_id");
                    b.Property<string>("Gender")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("gender");
                    b.Property<DateTime?>("LicenseExpirationDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("license_expiration_date");
                    b.Property<DateTime?>("LicenseIssueDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("license_issue_date");
                    b.Property<string>("LicenseNo")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("license_no");
                    b.Property<long?>("LicenseTypeId")
                        .HasColumnType("bigint")
                        .HasColumnName("license_type_id");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.Property<string>("VehicleTypeIds")
                        .HasColumnType("text")
                        .HasColumnName("vehicle_type_ids");
                    b.HasKey("Id");
                    b.HasIndex("DriverId")
                        .IsUnique();
                    b.HasIndex("LicenseTypeId");
                    b.ToTable("driver_details", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.DriverDocument", b =>
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
                    b.Property<long>("DriverId")
                        .HasColumnType("bigint")
                        .HasColumnName("driver_id");
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
                    b.HasIndex("DocumentId");
                    b.HasIndex("DriverId");
                    b.HasIndex("Status");
                    b.ToTable("driver_documents", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.DriverLicenseType", b =>
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
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("Status");
                    b.ToTable("driver_license_types", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.FuelType", b =>
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
                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("unit_price");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("NameAr")
                        .IsUnique();
                    b.HasIndex("NameEn")
                        .IsUnique();
                    b.ToTable("fuel_types", (string)null);
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
                    b.HasIndex("NameAr")
                        .IsUnique();
                    b.HasIndex("NameEn")
                        .IsUnique();
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.LookupDocumentStatus", b =>
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
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("NameAr")
                        .IsUnique();
                    b.HasIndex("NameEn")
                        .IsUnique();
                    b.ToTable("lookup_document_status", (string)null);
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Vehicle", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<decimal?>("Capacity")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("capacity");
                    b.Property<long?>("CapacityUnitId")
                        .HasColumnType("bigint")
                        .HasColumnName("capacity_unit_id");
                    b.Property<long?>("ColorId")
                        .HasColumnType("bigint")
                        .HasColumnName("color_id");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<long?>("DriverId")
                        .HasColumnType("bigint")
                        .HasColumnName("driver_id");
                    b.Property<decimal?>("InitialOdometerReading")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("initial_odometer_reading");
                    b.Property<long?>("InsuranceId")
                        .HasColumnType("bigint")
                        .HasColumnName("insurance_id");
                    b.Property<bool>("IsHead")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_head");
                    b.Property<int?>("MakeYear")
                        .HasColumnType("integer")
                        .HasColumnName("make_year");
                    b.Property<string>("PlateLetters")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("plate_letters");
                    b.Property<string>("PlateNo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("plate_no");
                    b.Property<DateTime?>("PurchaseDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("purchase_date");
                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("status");
                    b.Property<decimal?>("TankCapacity")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("tank_capacity");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.Property<string>("VehicleTypeSettings")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("vehicle_type_settings");
                    b.Property<string>("VinNo")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("vin_no");
                    b.HasKey("Id");
                    b.HasIndex("CapacityUnitId");
                    b.HasIndex("ColorId");
                    b.HasIndex("DriverId")
                        .IsUnique();
                    b.HasIndex("InsuranceId");
                    b.HasIndex("PlateNo");
                    b.HasIndex("Status");
                    b.ToTable("vehicles", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.VehicleBrand", b =>
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
                    b.Property<string>("Logo")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("logo");
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
                    b.HasIndex("Status");
                    b.ToTable("vehicle_brands", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.VehicleBrandModel", b =>
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
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.Property<long>("VehicleBrandId")
                        .HasColumnType("bigint")
                        .HasColumnName("vehicle_brand_id");
                    b.HasKey("Id");
                    b.HasIndex("Status");
                    b.HasIndex("VehicleBrandId");
                    b.ToTable("vehicle_brand_models", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.VehicleCapacityUnit", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<string>("CapacityTypeAr")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("capacity_type_ar");
                    b.Property<string>("CapacityTypeEn")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("capacity_type_en");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
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
                    b.ToTable("vehicles_capacity_units", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.VehicleChangeRequest", b =>
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
                    b.Property<long>("DriverId")
                        .HasColumnType("bigint")
                        .HasColumnName("driver_id");
                    b.Property<string>("Explanation")
                        .HasColumnType("text")
                        .HasColumnName("explanation");
                    b.Property<DateTime?>("PreferredEffectiveDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("prefered_effective_date");
                    b.Property<int?>("ReasonCategory")
                        .HasColumnType("integer")
                        .HasColumnName("reason_category");
                    b.Property<int?>("RequestPriority")
                        .HasColumnType("integer")
                        .HasColumnName("request_priority");
                    b.PrimitiveCollection<long[]>("RequestedVehicleIds")
                        .HasColumnType("bigint[]")
                        .HasColumnName("requested_vehicles_ids");
                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("status");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");
                    b.HasKey("Id");
                    b.HasIndex("DriverId");
                    b.HasIndex("Status");
                    b.ToTable("vehicle_change_requests", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.VehicleInsurance", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<long?>("AddedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("added_by");
                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("amount");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end_date");
                    b.Property<string>("InsuranceProvider")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("insurance_provider");
                    b.Property<string>("PolicyNo")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("policy_no");
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
                    b.HasIndex("EndDate");
                    b.HasIndex("PolicyNo");
                    b.HasIndex("StartDate");
                    b.HasIndex("Status");
                    b.ToTable("vehicles_insurance", (string)null);
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Contract", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.ContractType", "ContractType")
                        .WithMany()
                        .HasForeignKey("ContractTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Company");
                    b.Navigation("ContractType");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ContractFuelType", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Contract", "Contract")
                        .WithMany()
                        .HasForeignKey("ContractId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.FuelType", "FuelType")
                        .WithMany()
                        .HasForeignKey("FuelTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Contract");
                    b.Navigation("FuelType");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ContractVehicle", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Contract", "Contract")
                        .WithMany()
                        .HasForeignKey("ContractId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Vehicle", "Vehicle")
                        .WithMany()
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Contract");
                    b.Navigation("Vehicle");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ContractVehicleType", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Contract", "Contract")
                        .WithMany()
                        .HasForeignKey("ContractId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.VehicleType", "VehicleType")
                        .WithMany()
                        .HasForeignKey("VehicleTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Contract");
                    b.Navigation("VehicleType");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.DriverDetail", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Driver", "Driver")
                        .WithMany()
                        .HasForeignKey("DriverId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Driver");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.DriverDocument", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Document", "Document")
                        .WithMany()
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Driver", "Driver")
                        .WithMany()
                        .HasForeignKey("DriverId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Document");
                    b.Navigation("Driver");
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Vehicle", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.VehicleCapacityUnit", "CapacityUnit")
                        .WithMany()
                        .HasForeignKey("CapacityUnitId")
                        .OnDelete(DeleteBehavior.SetNull);
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Color", "Color")
                        .WithMany()
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.SetNull);
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Driver", "Driver")
                        .WithMany()
                        .HasForeignKey("DriverId")
                        .OnDelete(DeleteBehavior.SetNull);
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Document", "Insurance")
                        .WithMany()
                        .HasForeignKey("InsuranceId")
                        .OnDelete(DeleteBehavior.SetNull);
                    b.Navigation("CapacityUnit");
                    b.Navigation("Color");
                    b.Navigation("Driver");
                    b.Navigation("Insurance");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.VehicleBrandModel", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.VehicleBrand", "VehicleBrand")
                        .WithMany()
                        .HasForeignKey("VehicleBrandId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("VehicleBrand");
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.VehicleChangeRequest", b =>
                {
                    b.HasOne("LoadlyAggregator.Domain.Common.Entities.Driver", "Driver")
                        .WithMany()
                        .HasForeignKey("DriverId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Driver");
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