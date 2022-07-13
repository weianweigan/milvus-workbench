using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace IO.Milvus.Param.Collection
{
    public class FieldType
    {
        private string name;

        public int Dimension { get; set; }

        public int MaxLength { get; set; }

        public static Builder NewBuilder()
        {
            return new Builder();
        }

        public sealed class Builder
        {
            private String name;
            private bool primaryKey = false;
            private String description = "";
            //private DataType dataType;
            //private Map<String, String> typeParams = new HashMap<>();
            private bool autoID = false;

            internal Builder()
            {
            }

            public Builder WithName([NotNull] string name)
            {
                this.name = name ?? throw new ArgumentNullException(nameof(name));
                return this;
            }

            public Builder WithPrimaryKey(bool primaryKey)
            {
                this.primaryKey = primaryKey;
                return this;
            }

            public Builder WithDescription([NotNull] string description)
            {
                this.description = description ?? throw new ArgumentNullException(nameof(description));
                return this;
            }
        }

        public override string ToString()
        {
            return "";
            //return "FieldType{" +
            //        "name='" + name + '\'' +
            //        ", type='" + dataType.name() + '\'' +
            //        ", primaryKey=" + primaryKey +
            //        ", autoID=" + autoID +
            //        ", params=" + typeParams +
            //        '}';
        }
    }
}
