using IO.Milvus.Common.ClientEnum;
using IO.Milvus.Exception;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace IO.Milvus.Param.Dml
{
    public class SearchParam<TVector>
    {
        internal string collectionName;
        internal List<string> partitionNames;
        internal string metricType;
        internal string vectorFieldName;
        internal int topK;
        internal string expr;
        internal List<string> outFields;
        internal List<TVector> vectors;
        internal int roundDecimal;
        internal string @params;
        internal long travelTimestamp;
        internal long guaranteeTimestamp;
        internal long gracefulTime;
        internal ConsistencyLevelEnum consistencyLevel;

        private SearchParam([NotNull] Builder builder)
        {
            this.collectionName = builder.collectionName;
            this.partitionNames = builder.partitionNames;
            this.metricType = builder.metricType.ToString();
            this.vectorFieldName = builder.vectorFieldName;
            this.topK = builder.topK;
            this.expr = builder.expr;
            this.outFields = builder.outFields;
            this.vectors = builder.vectors;
            this.roundDecimal = builder.roundDecimal;
            this.@params = builder.@params;
            this.travelTimestamp = builder.travelTimestamp;
            this.guaranteeTimestamp = builder.guaranteeTimestamp;
            this.gracefulTime = builder.gracefulTime;
            this.consistencyLevel = builder.consistencyLevel;
        }

        public static Builder NewBuilder()
        {
            return new Builder();
        }

        public class Builder
        {
            internal string collectionName;
            internal List<string> partitionNames = new List<string>();
            internal MetricType metricType = MetricType.L2;
            internal string vectorFieldName;
            internal int topK;
            internal string expr = "";
            internal List<string> outFields = new List<string>();
            internal List<TVector> vectors;
            internal int roundDecimal = -1;
            internal string @params = "{}";
            internal long travelTimestamp = 0L;
            internal long guaranteeTimestamp = Constant.GUARANTEE_EVENTUALLY_TS;
            internal long gracefulTime = 5000L;
            internal ConsistencyLevelEnum consistencyLevel;

            internal Builder()
            {
            }
                                     
            public Builder WithCollectionName([NotNull] string collectionName)
            {
                this.collectionName = collectionName;
                return this;
            }

            public Builder WithPartitionNames([NotNull] List<string> partitionNames)
            {
                foreach (var name in partitionNames)
                {
                    AddPartitionName(name);
                }
                return this;
            }


            public Builder WithConsistencyLevel(ConsistencyLevelEnum consistencyLevel)
            {
                this.consistencyLevel = consistencyLevel;
                return this;
            }


            public Builder WithGracefulTime(long gracefulTime)
            {
                this.gracefulTime = gracefulTime;
                return this;
            }


            public Builder AddPartitionName([NotNull] string partitionName)
            {
                if (!this.partitionNames.Contains(partitionName))
                {
                    this.partitionNames.Add(partitionName);
                }
                return this;
            }

            public Builder WithMetricType([NotNull] MetricType metricType)
            {
                this.metricType = metricType;
                return this;
            }


            public Builder WithVectorFieldName([NotNull] string vectorFieldName)
            {
                this.vectorFieldName = vectorFieldName;
                return this;
            }


            public Builder WithTopK([NotNull] int topK)
            {
                this.topK = topK;
                return this;
            }

            
            public Builder withExpr([NotNull] string expr)
            {
                this.expr = expr;
                return this;
            }

  
            public Builder withOutFields([NotNull] List<string> outFields)
            {
                foreach (var outField in outFields)
                {
                    outFields.Add(outField);
                }
                return this;
            }


            public Builder AddOutField([NotNull] string fieldName)
            {
                if (!this.outFields.Contains(fieldName))
                {
                    this.outFields.Add(fieldName);
                }
                return this;
            }


            public Builder WithVectors([NotNull] List<TVector> vectors)
            {
                this.vectors = vectors;
                return this;
            }

            public Builder WithRoundDecimal([NotNull] int @decimal)
            {
                this.roundDecimal = @decimal;
                return this;
            }

            public Builder WithParams([NotNull] string @params)
            {
                this.@params = @params;
                return this;
            }

            public Builder WithTravelTimestamp([NotNull] long ts)
            {
                this.travelTimestamp = ts;
                return this;
            }

            public Builder WithGuaranteeTimestamp([NotNull] long ts)
            {
                this.guaranteeTimestamp = ts;
                return this;
            }

            public SearchParam<TVector> Build()
            {
                ParamUtils.CheckNullEmptyString(collectionName, "Collection name");
                ParamUtils.CheckNullEmptyString(vectorFieldName, "Target field name");

                if (topK <= 0)
                {
                    throw new ParamException("T opK value is illegal");
                }

                if (travelTimestamp < 0)
                {
                    throw new ParamException("The travel timestamp must be greater than 0");
                }

                if (guaranteeTimestamp < 0)
                {
                    throw new ParamException("The guarantee timestamp must be greater than 0");
                }

                if (metricType == MetricType.INVALID)
                {
                    throw new ParamException("Metric type is invalid");
                }

                if (vectors == null || vectors.Count == 0)
                {
                    throw new ParamException("Target vectors can not be empty");
                }

                if (typeof(TVector) == typeof(List<float>))
                {
                    int dim = (vectors.First() as List<float>).Count;
                    for (int i = 1; i < vectors.Count; ++i)
                    {
                        List<float> temp = vectors[i] as List<float>;
                        if (dim != temp.Count)
                        {
                            throw new ParamException("Target vector dimension must be equal");
                        }
                    }

                    if (!ParamUtils.IsFloatMetric(metricType))
                    {
                        throw new ParamException("Target vector is float but metric type is incorrect");
                    }
                }
                else if (typeof(TVector) == typeof(MemoryStream))
                {
                    // binary vectors
                    MemoryStream first = vectors[0] as MemoryStream;
                    var dim = first.Position;
                    for (int i = 1; i < vectors.Count; ++i)
                    {
                        MemoryStream temp = vectors[i] as MemoryStream;
                        if (dim != temp.Position)
                        {
                            throw new ParamException("Target vector dimension must be equal");
                        }
                    }

                    if (!ParamUtils.IsBinaryMetric(metricType))
                    {
                        throw new ParamException("Target vector is binary but metric type is incorrect");
                    }
                }
                else
                {
                    throw new ParamException("Target vector type must be List<Float> or ByteBuffer");
                }
                
                return new SearchParam<TVector>(this);
            }
        }

    }
}
