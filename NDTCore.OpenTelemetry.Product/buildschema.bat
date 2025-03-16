::rmdir /S /Q "NDTCore.OpenTelemetry.Infrastructure/Persistences/Migrations"

::dotnet ef migrations add ProductMigration -c ApplicationDbContext -o Persistences/Migrations/ProductDb --project ../NDTCore.OpenTelemetry.Infrastructure

::dotnet ef migrations script -c ApplicationDbContext -o ../NDTCore.OpenTelemetry.Infrastructure/Persistences/Migrations/ProductDb.sql

::# Using Package Manager Console (PMC) #

::Add-Migration ProductMigration -Context ApplicationDbContext -OutputDir Persistences/Migrations/ProductDb -Project NDTCore.OpenTelemetry.Infrastructure

::Script-Migration -Context ApplicationDbContext -Output ./NDTCore.OpenTelemetry.Infrastructure/Persistences/Migrations/ProductDb.sql

::Update-Database -Context ApplicationDbContext
