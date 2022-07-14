using System;
using System.Diagnostics.CodeAnalysis;

namespace IO.Milvus.Param.Alias
{
    public class AlterAliasParam
    {
        #region Ctor
        private AlterAliasParam(Builder builder)
        {
            this.CollectionName = builder.collectionName;
            this.Alias = builder.alias;
        }
        #endregion

        #region Properties
        public string CollectionName { get; }

        public string Alias { get; }
        #endregion

        #region Builder
        public static Builder NewBuilder()
        {
            return new Builder();
        }

        /// <summary>
        ///  Builder for <see cref="AlterAliasParam"/> class.
        /// </summary>
        public class Builder
        {
            internal string collectionName;
            internal string alias;


            internal Builder() { }

            /// <summary>
            /// Sets the collection name. Collection name cannot be empty or null.
            /// </summary>
            /// <param name="collectionName">collection name</param>
            /// <returns><see cref="Builder"/></returns>
            public Builder WithCollectionName(String collectionName)
            {
                this.collectionName = collectionName;
                return this;
            }

            /// <summary>
            ///  Sets the collection alias. Collection alias cannot be empty or null.
            /// </summary>
            /// <param name="alias">alias of the collection</param>
            /// <returns><see cref="Builder"/></returns>
            public Builder WithAlias(string alias)
            {
                this.alias = alias;
                return this;
            }

            /// <summary>
            /// Verifies parameters and creates a new {@link AlterAliasParam} instance.
            /// </summary>
            /// <returns></returns>
            public AlterAliasParam Build()             
            {
                ParamUtils.CheckNullEmptyString(collectionName, "Collection name");
                ParamUtils.CheckNullEmptyString(alias, "Alias");

                return new AlterAliasParam(this); 
            }
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{nameof(AlterAliasParam)}{{{nameof(Collection)}=/'{CollectionName}\', {nameof(Alias)}=\'{Alias}\'}}";
        }
        #endregion
    }

}
