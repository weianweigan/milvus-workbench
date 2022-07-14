using IO.Milvus.Exception;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using IO.Milvus.Utils;

namespace IO.Milvus.Param.Collection
{
    /// <summary>
    /// Param for <see cref="IO.Milvus.Client.IMilvusClient.CreateCollection(CreateCollectionParam)"/>
    /// </summary>
    public class CreateCollectionParam
    {
        #region Ctor
        private CreateCollectionParam(Builder builder)
        {
            CollectionName = builder.collectionName;
            ShardsNum = builder.shardsNum;
            Description = builder.description;
            FieldTypes = builder.fieldTypes;
        }
        #endregion

        #region Properties
        public string CollectionName { get; }

        public int ShardsNum { get; }

        public string Description { get; set; }

        public List<FieldType> FieldTypes { get; }
        #endregion

        #region Builder

        public static Builder NewBuilder() => new Builder();

        /// <summary>
        /// Builder for <see cref="CreateCollectionParam"/> class
        /// </summary>
        public sealed class Builder
        {
            internal string collectionName;
            internal int shardsNum = 2;
            internal string description = "";
            internal List<FieldType> fieldTypes = new List<FieldType>();

            internal Builder() { }

            /// <summary>
            /// Sets the collection name. Collection name cannot be empty or null.
            /// </summary>
            /// <param name="collectionName">collection Name</param>
            /// <returns><see cref="Builder"/></returns>
            public Builder WithCollectionName([NotNull] string collectionName)
            {
                this.collectionName = collectionName;
                return this;
            }

            /// <summary>
            /// Sets the shards number. The number must be greater than zero. The default value is 2.
            /// </summary>
            /// <param name="shardsNum">shards number to distribute insert data into multiple data nodes and query nodes.</param>
            /// <returns></returns>
            public Builder WithShardsNum(int shardsNum)
            {
                this.shardsNum = shardsNum;
                return this;
            }

            /// <summary>
            /// Sets the collection description. The description can be empty. The default is "".
            /// </summary>
            /// <param name="description">description of the collection</param>
            /// <returns><see cref="Builder"/></returns>
            public Builder WithDescription([NotNull] string description)
            {
                this.description = description;
                return this;
            }

            /// <summary>
            /// Sets the schema of the collection. The schema cannot be empty or null.
            /// </summary>
            /// <param name="fieldTypes">a <code>List</code> of <see cref="FieldType"/></param>
            /// <returns><see cref="Builder"/></returns>
            public Builder WithFieldTypes([NotNull] List<FieldType> fieldTypes)
            {
                this.fieldTypes.AddRange(fieldTypes);
                return this;
            }

            /// <summary>
            /// Adds a field schema.
            /// <see cref="FieldType"/>
            /// </summary>
            /// <param name="fieldType"><see cref="FieldType"/></param>
            /// <returns><see cref="Builder"/></returns>
            public Builder AddFieldType([NotNull] FieldType fieldType)
            {
                this.fieldTypes.Add(fieldType);
                return this;
            }

            /// <summary>
            /// Verifies parameters and creates a new <see cref="CreateCollectionParam"/> instance.
            /// </summary>
            /// <returns><see cref="CreateCollectionParam"/></returns>
            /// <exception cref="ParamException"></exception>
            public CreateCollectionParam Build()
            {
                ParamUtils.CheckNullEmptyString(collectionName, "Collection name");

                if (shardsNum <= 0)
                {
                    throw new ParamException("ShardNum must be larger than 0");
                }

                if (fieldTypes.IsEmpty())
                {
                    throw new ParamException("Field numbers must be larger than 0");
                }

                if (fieldTypes.Any(p => p == null))
                {
                    throw new ParamException("Collection field cannot be null");
                }

                return new CreateCollectionParam(this);
            }
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Constructs a <code>String</code> by <see cref="CreateCollectionParam"/> instance.
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return $"{nameof(CreateCollectionParam)}{{{nameof(CollectionName)}=\'{CollectionName}\', {nameof(ShardsNum)}=\'{ShardsNum}\', {nameof(Description)}=\'{Description}\', {nameof(FieldTypes)}=\'{FieldTypes}\', }}";
        }
        #endregion
    }
}
