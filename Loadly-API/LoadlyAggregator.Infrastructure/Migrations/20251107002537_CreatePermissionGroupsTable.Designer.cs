
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
    [Migration("20251107002537_CreatePermissionGroupsTable")]
    partial class CreatePermissionGroupsTable
    {
        
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.AuditLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<string>("AdditionalData")
                        .HasColumnType("text");
                    b.Property<string>("Changes")
                        .IsRequired()
                        .HasColumnType("text");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");
                    b.Property<long?>("EntityId")
                        .HasColumnType("bigint");
                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");
                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");
                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");
                    b.Property<string>("UserAgent")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");
                    b.Property<string>("UserType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");
                    b.HasKey("Id");
                    b.HasIndex("CreatedAt");
                    b.HasIndex("EntityType", "EntityId");
                    b.HasIndex("UserId", "UserType");
                    b.ToTable("audit_logs", (string)null);
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
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ModelHasPermission", b =>
                {
                    b.Property<long>("PermissionId")
                        .HasColumnType("bigint");
                    b.Property<long>("ModelId")
                        .HasColumnType("bigint");
                    b.Property<string>("ModelType")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");
                    b.HasKey("PermissionId", "ModelId", "ModelType");
                    b.HasIndex("ModelId", "ModelType");
                    b.ToTable("model_has_permissions", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.ModelHasRole", b =>
                {
                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");
                    b.Property<long>("ModelId")
                        .HasColumnType("bigint");
                    b.Property<string>("ModelType")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");
                    b.HasKey("RoleId", "ModelId", "ModelType");
                    b.HasIndex("ModelId", "ModelType");
                    b.ToTable("model_has_roles", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.NotificationsLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text");
                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");
                    b.Property<string>("Recipient")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");
                    b.Property<string>("Response")
                        .HasColumnType("text");
                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);
                    b.Property<int>("Type")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1);
                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");
                    b.HasKey("Id");
                    b.HasIndex("CreatedAt");
                    b.HasIndex("Recipient");
                    b.HasIndex("Status");
                    b.HasIndex("Type");
                    b.HasIndex("Type", "Status", "CreatedAt");
                    b.ToTable("notifications_log", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Permission", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");
                    b.Property<string>("GuardName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");
                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");
                    b.Property<string>("NameAr")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");
                    b.Property<long>("PermissionGroupId")
                        .HasColumnType("bigint");
                    b.Property<int>("Status")
                        .HasColumnType("integer");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");
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
                        .HasColumnType("bigint");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");
                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");
                    b.Property<string>("NameAr")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");
                    b.Property<int>("Status")
                        .HasColumnType("integer");
                    b.Property<string>("Type")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasDefaultValue("company");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");
                    b.HasKey("Id");
                    b.HasIndex("Type");
                    b.ToTable("permission_groups", (string)null);
                });
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.Role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");
                    b.Property<string>("GuardName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");
                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");
                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");
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
                        .HasColumnType("bigint");
                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");
                    b.HasKey("PermissionId", "RoleId");
                    b.HasIndex("RoleId");
                    b.ToTable("role_has_permissions", (string)null);
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