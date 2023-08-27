using System;
using System.Text.Json.Serialization;

namespace Milvus.Workbench.Models;

[JsonDerivedType(typeof(MilvusVectorDb))]
[JsonDerivedType(typeof(ZillizServerlessVectorDb))]
public abstract class VectorDb
{
    public string? Name { get; set; }

    public string? Host { get; set; }

    public  int Port { get; set; }
}

public sealed class MilvusVectorDb : VectorDb
{
    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? DatabaseName { get; set; }

    internal void LoadAsync()
    {
        throw new NotImplementedException();
    }
}

public sealed class ZillizServerlessVectorDb : VectorDb
{
    public string? Apikey { get; set; }

    public string? DatabaseName { get; set; }
}