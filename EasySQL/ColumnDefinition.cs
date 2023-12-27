using System;
using System.Collections.Generic;
using System.Text;

namespace EasySQL
{
    public class ColumnDefinition
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsAutoIncrement { get; set; }
        public string DefaultValue { get; set; }

        public ColumnDefinition(string name, string type, bool isPrimaryKey = false, bool isAutoIncrement = false, string defaultValue = null)
        {
            Name = name;
            Type = type;
            IsPrimaryKey = isPrimaryKey;
            IsAutoIncrement = isAutoIncrement;
            DefaultValue = defaultValue;
        }

        public override string ToString()
        {
            string columnDefinition = $"{Name} {Type}";

            if (IsPrimaryKey)
            {
                columnDefinition += " PRIMARY KEY";
            }

            if (IsAutoIncrement)
            {
                columnDefinition += " AUTO_INCREMENT";
            }

            if (!string.IsNullOrEmpty(DefaultValue))
            {
                columnDefinition += $" DEFAULT {DefaultValue}";
            }

            return columnDefinition;
        }
    }
}
