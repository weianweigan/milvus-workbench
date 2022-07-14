using System.Diagnostics.CodeAnalysis;

namespace IO.Milvus.Param.Alias
{
    /// <summary>
    /// Parameters for <see cref="CreateAliasParam"/> interface.
    /// </summary>
    public class CreateAliasParam
    {
        #region Ctor
        private CreateAliasParam(Builder builder)
        {
            CollectionName = builder.collectionName;
            Alias = builder.alias;
        }
        #endregion

        #region Properties
        public string Alias { get; }

        public string CollectionName { get; }
        #endregion

        #region Builder
        public static Builder NewBuilder()
        {
            return new Builder();
        }

        /// <summary>
        /// Parameters for <see cref="CreateAliasParam"/> interface.
        /// </summary>
        public class Builder
        {
            internal string collectionName;
            internal string alias;

            /// <summary>
            /// Sets the collection name. Collection name cannot be empty or null.
            /// </summary>
            /// <param name="collectionName">collection name</param>
            /// <returns></returns>
            internal Builder WithCollectionName(string collectionName)
            {
                this.collectionName = collectionName;
                return this;
            }

            /// <summary>
            ///  Sets the collection alias. Collection alias cannot be empty or null.
            /// </summary>
            /// <param name="alias">alias of the collection</param>
            /// <returns></returns>
            public Builder WithAlias(string alias)
            {
                this.alias = alias;
                return this;
            }

            /// <summary>
            /// Verifies parameters and creates a new <see cref="CreateAliasParam"/> instance.
            /// </summary>
            /// <returns></returns>
            public CreateAliasParam Build()
            {
                ParamUtils.CheckNullEmptyString(collectionName, "Collection name");
                ParamUtils.CheckNullEmptyString(alias, "Alias");

                return new CreateAliasParam(this);
            }
        }
        #endregion
    }
}
