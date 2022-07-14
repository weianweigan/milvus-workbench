using IO.Milvus.Exception;
using IO.Milvus.Grpc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace IO.Milvus.Param.Collection
{
    /// <summary>
    /// Parameters for a collection field.
    /// <see cref="CreateCollectionParam"/>
    /// </summary>
    public class FieldType
    {
        #region Fields
        #endregion

        #region Ctor
        public FieldType([NotNull] Builder builder)
        {
            this.Name = builder.name;
            this.IsPrimaryKey = builder.isPrimaryKey;
            this.Description = builder.description;
            this.DataType = builder.dataType;
            this.TypeParams = builder.typeParams;
            this.IsAutoID = builder.isAutoID;
        }
        #endregion

        #region Properties
        public int Dimension { get; }

        public int MaxLength { get; }

        public string Description { get; }

        public bool IsPrimaryKey { get; }

        public string Name { get; }

        public DataType DataType { get; }

        public Dictionary<string, string> TypeParams { get; }

        public bool IsAutoID { get; }
        #endregion

        public static Builder NewBuilder()
        {
            return new Builder();
        }

        /// <summary>
        /// Builder for <see cref="FieldType"/> class.
        /// </summary>
        public sealed class Builder
        {
            internal string name;
            internal bool isPrimaryKey = false;
            internal string description = "";
            internal DataType dataType;
            internal Dictionary<string, string> typeParams = new Dictionary<string, string>();
            internal bool isAutoID = false;

            internal Builder()
            {
            }

            public Builder WithName([NotNull] string name)
            {
                this.name = name ?? throw new ArgumentNullException(nameof(name));
                return this;
            }

            /// <summary>
            ///  Sets the field as the primary key field.
            ///  Note that the current release of Milvus only support<code> Long</code> data type as primary key.
            /// </summary>
            /// <param name="primaryKey">true is primary key, false is not</param>
            /// <returns><see cref="Builder"/></returns>
            public Builder WithIsPrimaryKey(bool primaryKey)
            {
                this.isPrimaryKey = primaryKey;
                return this;
            }

            /// <summary>
            /// Sets the field description. The description can be empty. The default is "".
            /// </summary>
            /// <param name="description">description of the field</param>
            /// <returns><see cref="Builder"/></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public Builder WithDescription([NotNull] string description)
            {
                this.description = description ?? throw new ArgumentNullException(nameof(description));
                return this;
            }

            /// <summary>
            /// Sets the data type for the field.
            /// </summary>
            /// <param name="dataType">data type of the field</param>
            /// <returns><see cref="Builder"/></returns>
            public Builder WithDataType([NotNull] DataType dataType)
            {
                this.dataType = dataType;
                return this;
            }

            /// <summary>
            /// Adds a parameter pair for the field.
            /// </summary>
            /// <param name="key">parameter key</param>
            /// <param name="value">parameter value</param>
            /// <returns><see cref="Builder"/></returns>
            /// <exception cref="ParamException"></exception>
            public Builder AddTypeParam([NotNull] string key, [NotNull] string value)
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentException($"“{nameof(key)}”Cannot be null or empty.", nameof(key));
                }

                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException($"“{nameof(value)}”Cannot be null or empty.", nameof(value));
                }

                typeParams.Add(key, value);
                return this;
            }

            /// <summary>
            /// Sets more parameters for the field.
            /// </summary>
            /// <param name="typeParams">parameters of the field</param>
            /// <returns><see cref="Builder"/></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public Builder WithTypeParams([NotNull] Dictionary<string, string> typeParams)
            {
                if (typeParams is null)
                {
                    throw new ArgumentNullException(nameof(typeParams));
                }

                foreach (var typeParam in typeParams)
                {
                    this.typeParams[typeParam.Key] = typeParam.Value;
                }
                return this;
            }

            /// <summary>
            ///  Sets the dimension of a vector field. Dimension value must be greater than zero.
            /// </summary>
            /// <param name="dimension">dimension of the field</param>
            /// <returns><see cref="Builder"/></returns>
            public Builder WithDimension([NotNull] int dimension)
            {
                this.typeParams.Add(Constant.VECTOR_DIM, dimension.ToString());
                return this;
            }

            /// <summary>
            /// Sets the max length of a varchar field. The value must be greater than zero.
            /// </summary>
            /// <param name="maxLength">max length of a varchar field</param>
            /// <returns><see cref="Builder"/></returns>
            public Builder WithMaxLength([NotNull] int maxLength)
            {
                this.typeParams.Add(Constant.VARCHAR_MAX_LENGTH, maxLength.ToString());
                return this;
            }

            /// <summary>
            /// Enables auto-id function for the field. Note that the auto-id function can only be enabled on primary key field.
            /// If auto-id function is enabled, Milvus will automatically generate unique ID for each entity,
            /// thus you do not need to provide values for the primary key field when inserting.
            ///
            /// If auto-id is disabled, you need to provide values for the primary key field when inserting.
            /// </summary>
            /// <param name="autoID">true enable auto-id, false disable auto-id</param>
            /// <returns><see cref="Builder"/></returns>
            public Builder WithIsAutoID(bool autoID)
            {
                this.isAutoID = autoID;
                return this;
            }

            /// <summary>
            ///  Verifies parameters and creates a new <see cref="FieldType"/> instance.
            /// </summary>
            /// <returns><see cref="FieldType"/></returns>
            /// <exception cref="ParamException"></exception>
            public FieldType Build()
            {
                ParamUtils.CheckNullEmptyString(name, "Field name");

                if (dataType == DataType.None)
                {
                    throw new ParamException("Field data type is illegal");
                }

                //TODO: Need Check
                //if (dataType == DataType.String)
                //{
                //    throw new ParamException("String type is not supported, use VarChar instead");
                //}

                if (dataType == DataType.FloatVector || dataType == DataType.BinaryVector)
                {
                    if (!typeParams.ContainsKey(Constant.VECTOR_DIM))
                    {
                        throw new ParamException("Vector field dimension must be specified");
                    }

                    try
                    {
                        int dim = int.Parse(typeParams[Constant.VECTOR_DIM]);
                        if (dim <= 0)
                        {
                            throw new ParamException("Vector field dimension must be larger than zero");
                        }
                    }
                    catch (FormatException)
                    {
                        throw new ParamException("Vector field dimension must be an integer number");
                    }
                }

                if (dataType == DataType.String)
                {
                    if (!typeParams.ContainsKey(Constant.VARCHAR_MAX_LENGTH))
                    {
                        throw new ParamException("Varchar field max length must be specified");
                    }

                    try
                    {
                        int len = int.Parse(typeParams[Constant.VARCHAR_MAX_LENGTH]);
                        if (len <= 0)
                        {
                            throw new ParamException("Varchar field max length must be larger than zero");
                        }
                    }
                    catch (FormatException)
                    {
                        throw new ParamException("Varchar field max length must be an integer number");
                    }
                }

                return new FieldType(this);
            }
        }

        /// <summary>
        /// Construct a <code>string</code> by <see cref="FieldType"/> instance.
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return $"FieldType{{name={Name}\', type={DataType}\', primaryKey={IsPrimaryKey}, autoID={IsAutoID}, params={TypeParams}}}";
        }
    }
}
