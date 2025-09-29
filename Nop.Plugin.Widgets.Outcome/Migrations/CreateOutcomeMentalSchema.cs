using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.Outcome.Domain;

namespace Nop.Plugin.Widgets.Outcome.Migrations
{
    [NopMigration("2025-09-16 12:00:00", "Widgets.Outcome – create master + mental tables (modular)")]
    public class CreateOutcomeMentalSchema : AutoReversingMigration
    {
        public override void Up()
        {
            if (!Schema.Table(nameof(OutcomeRecord)).Exists())
                Create.TableFor<OutcomeRecord>(); // uses OutcomeRecordBuilder

            if (!Schema.Table(nameof(MentalHealthRecord)).Exists())
                Create.TableFor<MentalHealthRecord>(); // uses MentalHealthRecordBuilder
        }
    }
}
