using Milvus.Workbench.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Milvus.Workbench.Utils;

internal static class VectorDbConfigUtils
{
    public static string ConfigFilePathName => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "milvusworkbench", 
        "vectordatabases.json");

    public static async Task<List<VectorDb>> ReadConfigAsync()
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
            return JsonSerializer.Deserialize<List<VectorDb>>(str) ?? new ();
        }
        else
        {
            return new List<VectorDb>();
        }
    }
}
