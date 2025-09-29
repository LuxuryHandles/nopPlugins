using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.Outcome.Domain;

namespace Nop.Plugin.Widgets.Outcome.Mapping.Builders
{
    public class OutcomeRecordBuilder : NopEntityBuilder<OutcomeRecord>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OutcomeRecord.CustomerId)).AsInt32().NotNullable()
                .WithColumn(nameof(OutcomeRecord.ProjectId)).AsInt32().NotNullable()
                .WithColumn(nameof(OutcomeRecord.Cat_MentalHealth)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(OutcomeRecord.Cat_PhysicalHealth)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(OutcomeRecord.Cat_Employment)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(OutcomeRecord.Cat_SocialConnection)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(OutcomeRecord.Cat_Culture)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(OutcomeRecord.Cat_Housing)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(OutcomeRecord.Cat_Religious)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(OutcomeRecord.Cat_Environment)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(OutcomeRecord.CreatedOnUtc)).AsDateTime().NotNullable()
                .WithColumn(nameof(OutcomeRecord.UpdatedOnUtc)).AsDateTime().NotNullable();
        }
    }
}
