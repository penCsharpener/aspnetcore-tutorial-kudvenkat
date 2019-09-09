﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using kudvenkat.DataAccess.Models;

namespace kudvenkat.DataAccess.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20190908124254_AddEmployeePhotoPathProperty")]
    partial class AddEmployeePhotoPathProperty
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("kudvenkat.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Department");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("PhotoPath");

                    b.HasKey("Id");

                    b.ToTable("Employees");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Department = 1,
                            Email = "mary@pragim.com",
                            Name = "Mary"
                        },
                        new
                        {
                            Id = 2,
                            Department = 2,
                            Email = "john@pragim.com",
                            Name = "John"
                        },
                        new
                        {
                            Id = 3,
                            Department = 2,
                            Email = "sam@pragim.com",
                            Name = "Sam"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
