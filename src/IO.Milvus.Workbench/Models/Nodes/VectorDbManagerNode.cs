using IO.Milvus.Workbench.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IO.Milvus.Workbench.Models;

public enum VectorDatabaseType
{
    Milvus,
    Qdrant,
    Pinecone,
}

public enum ConnectionType
{
    gRPC,
    Rest,
}

public class VectorDatabaseInstanceConfig
{
    [JsonConstructor]
    public VectorDatabaseInstanceConfig(
        string name, 
        string host, 
        int port, 
        string username, 
        string password,
        VectorDatabaseType databaseType = VectorDatabaseType.Milvus,
        ConnectionType connectionType = ConnectionType.gRPC)
    {
        this.Name = name;
        this.Port = port;
        this.Host = host;
        this.Username = username;
        this.Password = password;
        this.DatabaseType = databaseType;
        this.ConnectionType= connectionType;
    }

    public string Name { get; set; }

    public string Host { get; set; }

    public int Port { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public VectorDatabaseType DatabaseType { get; set; }

    public ConnectionType ConnectionType { get; set;}
}

public class VectorDbManagerNode : Node<DatabaseNode>
{
    public string ConfigFilePathName = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "milvusworkbench", "vectordatabases.json");


    public VectorDbManagerNode(MainWindowViewModel mainWindowViewModel)
    {
        Parent = mainWindowViewModel;
    }

    public MainWindowViewModel Parent { get; }

    public async Task SaveAsync()
    {
        var configs = Children.Select(p => p.DbConfig);

        var str = JsonSerializer.Serialize(configs);

        var dir = Path.GetDirectoryName(ConfigFilePathName);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

#if NET461_OR_GREATER
        await Task.Run(() =>
        {
            File.WriteAllText(ConfigFilePathName, str);
        });
#else
        await File.WriteAllTextAsync(ConfigFilePathName, str);
#endif
    }

    public async Task<List<VectorDatabaseInstanceConfig>> ReadConfigAsync()
    {
        if (File.Exists(ConfigFilePathName))
        {
#if NET461_OR_GREATER
            var str = await Task.Run(() =>{
                return File.ReadAllText(ConfigFilePathName); 
            });               
#else
            var str = await File.ReadAllTextAsync(ConfigFilePathName);
#endif
            return JsonSerializer.Deserialize<List<VectorDatabaseInstanceConfig>>(str);
        }
        else
        {
            return new List<VectorDatabaseInstanceConfig>();
        }
    }
}
