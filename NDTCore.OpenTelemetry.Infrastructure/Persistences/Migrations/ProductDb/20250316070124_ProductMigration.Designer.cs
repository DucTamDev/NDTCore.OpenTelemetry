﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace NDTCore.OpenTelemetry.Infrastructure.Persistences.Migrations.ProductDb
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250316070124_ProductMigration")]
    partial class ProductMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.13");
#pragma warning restore 612, 618
        }
    }
}
