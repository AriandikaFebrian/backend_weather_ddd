﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetCa.Infrastructure.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NetCa.Infrastructure.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250326091146_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("NetCa.Domain.Entities.Changelog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ChangeBy")
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)");

                    b.Property<DateTimeOffset>("ChangeDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("KeyValues")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Method")
                        .HasMaxLength(6)
                        .HasColumnType("varchar(6)");

                    b.Property<string>("NewValues")
                        .HasColumnType("text");

                    b.Property<string>("OldValues")
                        .HasColumnType("text");

                    b.Property<string>("TableName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Changelogs");
                });

            modelBuilder.Entity("NetCa.Domain.Entities.Crypto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("Change24h")
                        .HasColumnType("numeric");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal>("MarketCap")
                        .HasColumnType("numeric");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<string>("Symbol")
                        .HasColumnType("text");

                    b.Property<Guid>("TrackingId")
                        .HasColumnType("uuid");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("Volume24h")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("Cryptos");
                });

            modelBuilder.Entity("NetCa.Domain.Entities.MessageBroker", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Acknowledged")
                        .HasColumnType("bool");

                    b.Property<bool>("IsSend")
                        .HasColumnType("bool");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<DateTime?>("StoredDate")
                        .HasColumnType("Timestamp");

                    b.Property<string>("Topic")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("MessageBroker");
                });

            modelBuilder.Entity("NetCa.Domain.Entities.ReceivedMessageBroker", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Error")
                        .HasColumnType("text");

                    b.Property<string>("InnerMessage")
                        .HasColumnType("text");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<long>("Offset")
                        .HasColumnType("bigint");

                    b.Property<int>("Partition")
                        .HasColumnType("int");

                    b.Property<string>("StackTrace")
                        .HasColumnType("text");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TimeFinish")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("TimeIn")
                        .HasColumnType("timestamp");

                    b.Property<DateTime?>("TimeProcess")
                        .HasColumnType("timestamp");

                    b.Property<string>("Topic")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("ReceivedMessageBroker");
                });

            modelBuilder.Entity("NetCa.Domain.Entities.Tiket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AffectedVersion")
                        .HasColumnType("text");

                    b.Property<string>("AssignedDivision")
                        .HasColumnType("text");

                    b.Property<string>("AssignedTo")
                        .HasColumnType("text");

                    b.Property<string>("AttachmentUrl")
                        .HasColumnType("text");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Environment")
                        .HasColumnType("text");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<string>("Priority")
                        .HasColumnType("text");

                    b.Property<DateTime>("ReportedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ReportedByDivision")
                        .HasColumnType("text");

                    b.Property<string>("Resolution")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ResolvedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Severity")
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<Guid>("TrackingId")
                        .HasColumnType("uuid");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Tikets");
                });

            modelBuilder.Entity("NetCa.Domain.Entities.TodoItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)");

                    b.Property<DateTimeOffset?>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<bool?>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bool")
                        .HasDefaultValue(true);

                    b.Property<bool?>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bool")
                        .HasDefaultValue(false);

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<string>("Title")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<Guid>("TrackingId")
                        .HasColumnType("uuid");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)");

                    b.Property<DateTimeOffset?>("UpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.HasKey("Id");

                    b.HasIndex("IsActive");

                    b.HasIndex("IsDeleted");

                    b.ToTable("TodoItems");
                });

            modelBuilder.Entity("NetCa.Domain.Entities.Weather", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("City")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<int>("CloudCoverage")
                        .HasColumnType("int");

                    b.Property<string>("Country")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)");

                    b.Property<DateTimeOffset?>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Humidity")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<bool?>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bool")
                        .HasDefaultValue(true);

                    b.Property<bool?>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bool")
                        .HasDefaultValue(false);

                    b.Property<decimal>("Temperature")
                        .HasColumnType("decimal(10,2)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp");

                    b.Property<Guid>("TrackingId")
                        .HasColumnType("uuid");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)");

                    b.Property<DateTimeOffset?>("UpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("WeatherDescription")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<decimal>("WindSpeed")
                        .HasColumnType("decimal(10,2)");

                    b.HasKey("Id");

                    b.HasIndex("IsActive");

                    b.HasIndex("IsDeleted");

                    b.ToTable("Weathers");
                });
#pragma warning restore 612, 618
        }
    }
}
