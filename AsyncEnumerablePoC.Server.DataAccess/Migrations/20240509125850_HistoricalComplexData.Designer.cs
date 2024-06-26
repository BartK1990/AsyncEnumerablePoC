﻿// <auto-generated />
using System;
using AsyncEnumerablePoC.Server.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AsyncEnumerablePoC.Server.DataAccess.Migrations
{
    [DbContext(typeof(ReadDataDbContext))]
    [Migration("20240509125850_HistoricalComplexData")]
    partial class HistoricalComplexData
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AsyncEnumerablePoC.Server.DataAccess.Model.HistoricalComplexData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<double>("Value1")
                        .HasColumnType("float");

                    b.Property<double>("Value2")
                        .HasColumnType("float");

                    b.Property<double>("Value3")
                        .HasColumnType("float");

                    b.Property<double>("Value4")
                        .HasColumnType("float");

                    b.Property<double>("Value5")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("HistoricalComplexData");
                });

            modelBuilder.Entity("AsyncEnumerablePoC.Server.DataAccess.Model.HistoricalData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<double>("Value")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("HistoricalData");
                });
#pragma warning restore 612, 618
        }
    }
}
