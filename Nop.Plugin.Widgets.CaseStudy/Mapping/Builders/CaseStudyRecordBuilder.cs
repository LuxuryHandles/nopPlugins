using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.CaseStudy.Domain;

namespace Nop.Plugin.Widgets.CaseStudy.Mapping.Builders
{
    public class CaseStudyRecordBuilder : NopEntityBuilder<CaseStudyRecord>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CaseStudyRecord.CustomerId)).AsInt32().NotNullable()
                .WithColumn(nameof(CaseStudyRecord.ProjectId)).AsInt32().NotNullable()
                .WithColumn(nameof(CaseStudyRecord.CaseStudy)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(CaseStudyRecord.Published)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(CaseStudyRecord.Deleted)).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(nameof(CaseStudyRecord.CreatedOnUtc)).AsDateTime().NotNullable()
                .WithColumn(nameof(CaseStudyRecord.UpdatedOnUtc)).AsDateTime().NotNullable();
        }
    }
}
