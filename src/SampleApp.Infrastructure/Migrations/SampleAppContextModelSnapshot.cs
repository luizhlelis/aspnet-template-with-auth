﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SampleApp.Infrastructure;

#nullable disable

namespace SampleApp.Infrastructure.Migrations
{
    [DbContext(typeof(SampleAppContext))]
    partial class SampleAppContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("SampleApp.Application.Domain.Entities.User", b =>
                {
                    b.Property<string>("Username")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("GivenName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Username");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Username = "admin-user",
                            Address = "5036 Tierra Locks Suite 158",
                            GivenName = "Admin User",
                            PasswordHash = "$2a$11$1W9N/zaUdmo.PXWAdU4L3Ov3E8suY75ZMVh19Do/fPjwT/mnaohNO",
                            Role = 0,
                            ZipCode = "980395900"
                        },
                        new
                        {
                            Username = "customer-user",
                            Address = "570 Hackett Bridge",
                            GivenName = "Customer User",
                            PasswordHash = "$2a$11$/UdRM0TQbDd8rOITPM7UQ.SVjtgkABxfhHNFE1QsVTnzv.s4mLbCy",
                            Role = 1,
                            ZipCode = "948019535"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
