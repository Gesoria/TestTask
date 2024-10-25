﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderFilter.DAL;

#nullable disable

namespace OrderFilter.DAL.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20241024172639_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("OrderFilter.DAL.Entities.CityDistrict", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("CityDistricts");
                });

            modelBuilder.Entity("OrderFilter.DAL.Entities.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("CityDistrictId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DeliveryDateTime")
                        .HasColumnType("TEXT");

                    b.Property<double>("Weight")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("CityDistrictId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("OrderFilter.DAL.Entities.Order", b =>
                {
                    b.HasOne("OrderFilter.DAL.Entities.CityDistrict", "CityDistrict")
                        .WithMany()
                        .HasForeignKey("CityDistrictId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CityDistrict");
                });
#pragma warning restore 612, 618
        }
    }
}
