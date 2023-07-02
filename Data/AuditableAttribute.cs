namespace GeoLocationTest.Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AuditableAttribute : Attribute
    {
        public readonly static string CreateDateFieldName = "CreateDate";
        public readonly static string CreatedByFieldName = "CreatedBy";
        public readonly static string EditDateFieldName = "EditDate";
        public readonly static string EditedByFieldName = "EditedBy";
    }
}
