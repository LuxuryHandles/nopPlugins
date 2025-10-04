using FluentMigrator;
using Nop.Data.Migrations;

namespace Nop.Plugin.BusinessServices.Data.Migrations
{
    [NopMigration("2025-09-14 00:00:00", "BusinessServices: Create BusinessType, Services, BusinessTypeServices", MigrationProcessType.Installation)]
    public class V0001_CreateBusinessTypeServices : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("BusinessType")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString(200).NotNullable()
                .WithColumn("Description").AsString(int.MaxValue).Nullable()
                .WithColumn("Published").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CreatedOnUtc").AsDateTime().NotNullable()
                .WithColumn("UpdatedOnUtc").AsDateTime().NotNullable();

            Create.Table("Services")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString(250).NotNullable()
                .WithColumn("Description").AsString(int.MaxValue).Nullable()
                .WithColumn("Published").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CreatedOnUtc").AsDateTime().NotNullable()
                .WithColumn("UpdatedOnUtc").AsDateTime().NotNullable();

            Create.Table("BusinessTypeServices")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("BusinessTypeId").AsInt32().NotNullable()
                .WithColumn("ServiceItemId").AsInt32().NotNullable()
                .WithColumn("Published").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CreatedOnUtc").AsDateTime().NotNullable()
                .WithColumn("UpdatedOnUtc").AsDateTime().NotNullable();

            Create.Index("IX_BTS_Unique")
                .OnTable("BusinessTypeServices")
                .OnColumn("BusinessTypeId").Ascending()
                .OnColumn("ServiceItemId").Ascending()
                .WithOptions().Unique();

            Create.ForeignKey("FK_BTS_BT")
                .FromTable("BusinessTypeServices").ForeignColumn("BusinessTypeId")
                .ToTable("BusinessType").PrimaryColumn("Id")
                .OnDeleteOrUpdate(System.Data.Rule.Cascade);

            Create.ForeignKey("FK_BTS_SV")
                .FromTable("BusinessTypeServices").ForeignColumn("ServiceItemId")
                .ToTable("Services").PrimaryColumn("Id")
                .OnDeleteOrUpdate(System.Data.Rule.Cascade);
        }
    }
}
