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
            UseHttps = builder.useHttps;
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

        public string Authorization { get; }

        public bool UseHttps { get; }
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
            internal bool useHttps = false;

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

            public Builder WithHttps(bool enable)
            {
                useHttps = enable;
                return this;
            }

            [Obsolete("Useless")]
            public Builder WithConnectTimeout(TimeSpan connectTimeout)
            {
                this.connectTimeout = connectTimeout;
                return this;
            }

            [Obsolete("Useless")]
            public Builder WithKeepAliveTime(TimeSpan keepAliveTime)
            {
                this.keepAliveTime = keepAliveTime;
                return this;
            }

            [Obsolete("Useless")]
            public Builder WithKeepAliveTimeout(TimeSpan keepAliveTimeout)
            {
                this.keepAliveTimeout = keepAliveTimeout;
                return this;
            }

            [Obsolete("Useless")]
            public Builder KeepAliveWithoutCalls(bool enable)
            {
                this.keepAliveWithoutCalls = enable;
                return this;
            }

            [Obsolete("Useless")]
            public Builder Secure(bool enable)
            {
                this.secure = enable;
                return this;
            }

            [Obsolete("Useless")]
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

            [Obsolete("Useless")]
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
