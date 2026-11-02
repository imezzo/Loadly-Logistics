
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using LoadlyAggregator.Infrastructure.Data;
#nullable disable
namespace LoadlyAggregator.Infrastructure.Migrations.LogsDb
{
    [DbContext(typeof(LogsDbContext))]
    [Migration("20251106185440_CreateNotificationsLogTable")]
    partial class CreateNotificationsLogTable
    {
        
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);
            modelBuilder.Entity("LoadlyAggregator.Domain.Common.Entities.NotificationsLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");
                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("body");
                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP")
                        .HasColumnName("created_at");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");
                    b.Property<string>("Recipient")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("recipient");
                    b.Property<string>("Response")
                        .HasColumnType("text")
                        .HasColumnName("response");
                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("status");
                    b.Property<int>("Type")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("type");
                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP")
                        .HasColumnName("updated_at");
                    b.HasKey("Id")
                        .HasName("PK_notifications_log");
                    b.HasIndex("CreatedAt")
                        .HasDatabaseName("IX_notifications_log_created_at");
                    b.HasIndex("Recipient")
                        .HasDatabaseName("IX_notifications_log_recipient");
                    b.HasIndex("Status")
                        .HasDatabaseName("IX_notifications_log_status");
                    b.HasIndex("Type")
                        .HasDatabaseName("IX_notifications_log_type");
                    b.HasIndex("Type", "Status", "CreatedAt")
                        .HasDatabaseName("IX_notifications_log_type_status_created_at");
                    b.ToTable("notifications_log", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}