using FluentMigrator;
using Nop.Data.Migrations;

namespace Nop.Plugin.BusinessServices.Data.Migrations
{
    // Runs for already-installed plugins; safe to run multiple times
    [NopMigration("2025-09-17 12:00:00", "BusinessServices: Ensure tables exist (BusinessType, Services, BusinessTypeServices)", MigrationProcessType.Update)]
    public class V0002_EnsureTables : Migration
    {
        public override void Up()
        {
            // BUSINESS TYPE
            Execute.Sql(@"
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'BusinessType')
BEGIN
    CREATE TABLE [BusinessType](
        [Id] INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_BusinessType PRIMARY KEY,
        [Name] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [Published] BIT NOT NULL CONSTRAINT DF_BusinessType_Published DEFAULT(1),
        [CreatedOnUtc] DATETIME2 NOT NULL,
        [UpdatedOnUtc] DATETIME2 NOT NULL
    );
END
");

            // SERVICES
            Execute.Sql(@"
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Services')
BEGIN
    CREATE TABLE [Services](
        [Id] INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Services PRIMARY KEY,
        [Name] NVARCHAR(250) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [Published] BIT NOT NULL CONSTRAINT DF_Services_Published DEFAULT(1),
        [CreatedOnUtc] DATETIME2 NOT NULL,
        [UpdatedOnUtc] DATETIME2 NOT NULL
    );
END
");

            // BUSINESS TYPE SERVICES (mapping)
            Execute.Sql(@"
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'BusinessTypeServices')
BEGIN
    CREATE TABLE [BusinessTypeServices](
        [Id] INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_BusinessTypeServices PRIMARY KEY,
        [BusinessTypeId] INT NOT NULL,
        [ServiceItemId] INT NOT NULL,
        [Published] BIT NOT NULL CONSTRAINT DF_BTS_Published DEFAULT(1),
        [CreatedOnUtc] DATETIME2 NOT NULL,
        [UpdatedOnUtc] DATETIME2 NOT NULL
    );

    -- FKs (create only if the parent tables exist)
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'BusinessType')
       AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Services')
    BEGIN
        ALTER TABLE [BusinessTypeServices] WITH CHECK 
        ADD CONSTRAINT [FK_BTS_BT] FOREIGN KEY([BusinessTypeId]) REFERENCES [BusinessType]([Id]) ON DELETE CASCADE;

        ALTER TABLE [BusinessTypeServices] WITH CHECK 
        ADD CONSTRAINT [FK_BTS_SV] FOREIGN KEY([ServiceItemId]) REFERENCES [Services]([Id]) ON DELETE CASCADE;

        CREATE UNIQUE NONCLUSTERED INDEX [IX_BTS_Unique]
            ON [BusinessTypeServices]([BusinessTypeId] ASC, [ServiceItemId] ASC);
    END
END
");
        }

        public override void Down()
        {
            // No-op (we don't drop anything on update migrations)
        }
    }
}
