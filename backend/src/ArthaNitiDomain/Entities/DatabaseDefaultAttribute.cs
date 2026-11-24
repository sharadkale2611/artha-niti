using System;

namespace ArthaNiti.Domain.Entities
{
    /// <summary>
    /// Marks a column whose value comes from a SQL Server DEFAULT constraint.
    /// The DbContext maps it with HasDefaultValueSql, so EF Core omits the
    /// column from INSERT when the property is null and SQL Server applies the default.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DatabaseDefaultAttribute : Attribute
    {
        public DatabaseDefaultAttribute(string sql)
        {
            Sql = sql;
        }

        public string Sql { get; }
    }
}