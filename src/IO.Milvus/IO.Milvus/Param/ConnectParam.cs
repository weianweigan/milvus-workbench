using IO.Milvus.Exception;
using IO.Milvus.Utils;
using System;
using System.Diagnostics.CodeAnalysis;

namespace IO.Milvus.Param
{
    public class ConnectParam
    {
        #region Ctor
        private ConnectParam([NotNull] Builder builder)
        {
            Host = builder.host;
            Port = builder.port;
            ConnectTimeout = builder.connectTimeout;
            KeepAliveTime = builder.keepAliveTime;
            KeepAliveTimeout = builder.keepAliveTimeout;
            IsKeepAliveWithoutCalls = builder.keepAliveWithoutCalls;
            IdleTimeout = builder.idleTimeout;
            IsSecure = builder.secure;
            Authorization = builder.authorization;
        }
        #endregion

        #region Properties
        public string Host { get; }

        public int Port { get; }

        public TimeSpan ConnectTimeout { get; }

        public TimeSpan KeepAliveTime { get; }

        public TimeSpan KeepAliveTimeout { get; }

        public bool IsKeepAliveWithoutCalls { get; }

        public bool IsSecure { get; }

        public TimeSpan IdleTimeout { get; }

        public string Authorization { get; set; }
        #endregion

        #region Builder
        public static Builder NewBuilder()
        {

            return new Builder();
        }

        public class Builder
        {
            internal string host = "localhost";
            internal int port = 19530;
            internal TimeSpan connectTimeout = TimeSpan.FromMilliseconds(10000);
            internal TimeSpan keepAliveTime = TimeSpan.MaxValue; // Disabling keep alive
            internal TimeSpan keepAliveTimeout = TimeSpan.FromMilliseconds(20000);
            internal bool keepAliveWithoutCalls = false;
            internal bool secure = false;
            internal TimeSpan idleTimeout = TimeSpan.FromHours(24);
            internal string authorization ="root:milvus".ToBase64();

            internal Builder() { }

            public Builder WithHost([NotNull] string host)
            {
                this.host = host;
                return this;
            }

            public Builder WithPort(int port)
            {
                this.port = port;
                return this;
            }

            public Builder WithConnectTimeout(TimeSpan connectTimeout)
            {
                this.connectTimeout = connectTimeout;
                return this;
            }

            public Builder WithKeepAliveTime(TimeSpan keepAliveTime)
            {
                this.keepAliveTime = keepAliveTime;
                return this;
            }

            public Builder WithKeepAliveTimeout(TimeSpan keepAliveTimeout)
            {
                this.keepAliveTimeout = keepAliveTimeout;
                return this;
            }

            public Builder KeepAliveWithoutCalls(bool enable)
            {
                this.keepAliveWithoutCalls = enable;
                return this;
            }

            public Builder Secure(bool enable)
            {
                this.secure = enable;
                return this;
            }

            public Builder WithIdleTimeout(TimeSpan idleTimeout)
            {
                this.idleTimeout = idleTimeout;
                return this;
            }

            public Builder WithAuthorization(string username,string password)
            {
                this.authorization = $"{username}:{password}".ToBase64();
                return this;
            }

            public Builder WithSecure(bool secure)
            {
                this.secure = secure;
                return this;
            }

            public Builder WithAuthorization(string authorization)
            {
                this.authorization = authorization;
                return this;
            }

            public ConnectParam Build()
            {
                ParamUtils.CheckNullEmptyString(host, "Host name");

                if (port < 0 || port > 0xFFFF)
                {
                    throw new ParamException("Port is out of range!");
                }

                return new ConnectParam(this);
            }
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"ConnectParam{{host=/'{Host}/', port=/'{Port}/'}}";
        }
        #endregion
    }
}
