using FluentMigrator;
using FluentMigrator.Builders.Alter.Table;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.CaseStudy.Domain;

namespace Nop.Plugin.Widgets.CaseStudy.Data
{
    [NopSchemaMigration("2025/08/20 10:00:00", "CaseStudy base schema + Steps.CaseStudyID", MigrationProcessType.Installation)]
    public class SchemaMigration : ForwardOnlyMigration
    {
        public override void Up()
        {
            // If table already exists (e.g., plugin reinstalled), do nothing
            if (Schema.Table(nameof(CaseStudyRecord)).Exists())
                return;

            // Create CaseStudyRecord table
            Create.TableFor<CaseStudyRecord>();

            // Add Steps.CaseStudyID if missing (nullable, so we can fill later)
            if (!Schema.Table("Steps").Column("CaseStudyID").Exists())
            {
                Alter.Table("Steps").AddColumn("CaseStudyID").AsInt32().Nullable();
            }
        }
    }
}
