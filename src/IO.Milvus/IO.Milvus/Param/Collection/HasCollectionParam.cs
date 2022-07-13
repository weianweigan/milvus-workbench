using System;
using System.Diagnostics.CodeAnalysis;

namespace IO.Milvus.Param.Collection
{
    /// <summary>
    /// Parameters for <code>hasCollection</code> interface.
    /// </summary>
    public class HasCollectionParam
    {
        private HasCollectionParam([NotNull] Builder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            this.CollectionName = builder.collectionName;
        }

        public string CollectionName { get;}

        public static Builder NewBuilder()
        {
            return new Builder();
        }

        public sealed class Builder
        {
            internal string collectionName;

            internal Builder() { }

            /// <summary>
            /// Sets the collection name. Collection name cannot be empty or null.
            /// </summary>
            /// <param name="collectionName">collection name</param>
            /// <returns><code>Builder</code></returns>
            public Builder WithCollcetionName([NotNull] string collectionName)
            {
                this.collectionName = collectionName;
                return this;
            }

            /// <summary>
            /// Verifies parameters and creates a new {@link HasCollectionParam} instance.
            /// </summary>
            /// <returns><see cref="HasCollectionParam"/></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public HasCollectionParam Build()
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    throw new ArgumentNullException(nameof(collectionName));
                }

                return new HasCollectionParam(this);
            }
        }

        /// <summary>
        /// Constructs a <code>String</code> by <see cref="HasCollectionParam"/> instance.
        /// </summary>
        /// <returns><code>string</code></returns>
        public override string ToString()
        {
            return $"{nameof(HasCollectionParam)}{{{nameof(CollectionName)}=\'{CollectionName}\'}}";
        }
    }
}
