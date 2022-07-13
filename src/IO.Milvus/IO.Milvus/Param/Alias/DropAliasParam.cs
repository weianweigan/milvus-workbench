using System.Diagnostics.CodeAnalysis;

namespace IO.Milvus.Param.Alias
{
    /// <summary>
    /// Parameters for <code>dropAlias</code> interface.
    /// </summary>
    public class DropAliasParam
    {
        #region Ctor
        public DropAliasParam(Builder builder)
        {
            Alias = builder.alias;
        }
        #endregion

        #region Propeties
        public string Alias { get; }
        #endregion

        #region Builder
        public static Builder NewBuilder()
        {
            return new Builder();
        }

        /// <summary>
        /// Builder for {@link DropAliasParam} class.
        /// </summary>
        public class Builder
        {
            internal string alias;

            internal Builder()
            {
            }

            /// <summary>
            /// Sets collection alias. Collection alias cannot be empty or null.
            /// </summary>
            /// <param name="alias"></param>
            /// <returns></returns>
            public Builder withAlias([NotNull] string alias)
            {
                this.alias = alias;
                return this;
            }

            /// <summary>
            /// Verifies parameters and creates a new {@link DropAliasParam} instance.
            /// </summary>
            /// <returns></returns>
            public DropAliasParam build()
            {
                ParamUtils.CheckNullEmptyString(alias, "Alias");

                return new DropAliasParam(this);
            }
        }
        #endregion
    }
}
