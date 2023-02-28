using System.Runtime.Serialization.Json;
using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using Unity.Mathematics;

namespace PrototypeLib
{
    namespace Multiplayer
    {
        using Photon;
        using Photon.Pun;
        using Photon.Realtime;
        using Hashtable = ExitGames.Client.Photon.Hashtable;
        namespace LocalPlayerIO
        {
            public static class PlayerManipulaton
            {
                public static bool Save(Hashtable h)
                {
                    return PhotonNetwork.LocalPlayer.SetCustomProperties(h);
                }
                public static bool SaveParameters(string[] keys, object[] parameters = null)
                {
                    if (parameters == null) return false;
                    if (keys.Length == 0) return false;
                    Hashtable hash = new();
                    for (int i = 0; i < keys.Length; i++)
                    {
                        hash.Add(keys[i], parameters[i]);
                    }
                    return Save(hash);
                }
                public static bool Read(Hashtable hash)
                {
                    return false;
                }
                public static bool ReadParameters(string[] keys)
                {
                    return false;
                }
            }
        }
    }
    namespace Modules
    {
        namespace FileOpsIO
        {
            using System;
            using System.Threading.Tasks;
            using System.Collections;
            using System.Collections.Generic;
            using UnityEngine;
            using Unity.Mathematics;
            using System.Text;
            using System.IO;
            public class WritingData
            {
                public string filePath;
                public bool initializeIfEmpty;
                public bool jsonFormat;
                public bool cleanJson;
                public bool overwriteExisted;
                public Encoding encode;
                public WritingData()
                {
                    filePath = "";
                    initializeIfEmpty = true;
                    jsonFormat = true;
                    cleanJson = true;
                    overwriteExisted = true;
                    encode = Encoding.Default;
                }
                public WritingData(string fp, Encoding ec)
                {
                    filePath = fp;
                    initializeIfEmpty = true;
                    jsonFormat = true;
                    cleanJson = true;
                    overwriteExisted = true;
                    encode = ec ?? Encoding.Default;
                }
                public WritingData(string fp, bool iie, bool jf, bool cj, bool oe, Encoding ec)
                {
                    filePath = fp;
                    initializeIfEmpty = iie;
                    jsonFormat = jf;
                    cleanJson = cj;
                    overwriteExisted = oe;
                    encode = ec ?? Encoding.Default;
                }
            }
            public class ReadingData
            {
                public string filePath;
                public bool initializeIfEmpty;
                public bool convertFromJson;
                public Encoding encode;
                public ReadingData()
                {
                    filePath = "";
                    initializeIfEmpty = true;
                    convertFromJson = true;
                    encode = Encoding.Default;
                }
                public ReadingData(string fp, Encoding ec)
                {
                    filePath = fp;
                    initializeIfEmpty = true;
                    convertFromJson = true;
                    encode = ec ?? Encoding.Default;
                }
                public ReadingData(string filePath, bool initializeIfEmpty, bool convertFromJson, Encoding encode)
                {
                    this.filePath = filePath;
                    this.initializeIfEmpty = initializeIfEmpty;
                    this.convertFromJson = convertFromJson;
                    this.encode = encode ?? Encoding.Default;
                }
            }
            public static class FileOps<T> where T : new()
            {
                public delegate void FileOperate();
                public delegate void FileOperateAsync();
                public static event FileOperate OperatedFile;
                public static event FileOperateAsync OperatedFileAsync;
                public async static Task<bool> WriteFileAsync(T content, WritingData data) { return await WriteFileAsync(content, data.filePath, data.initializeIfEmpty, data.jsonFormat, data.cleanJson, data.overwriteExisted, data.encode ?? Encoding.Default); }
                public async static Task<bool> WriteFileAsync(T content, string filePath, bool initializeIfEmpty = true, bool jsonFormat = true, bool cleanJson = true, bool overwriteExisted = true, Encoding encode = null)
                {
                    bool success = false;
                    if (typeof(T) == null)
                    {
                        Debug.LogWarning($"The Data Type {typeof(T).FullName} is null. Please make sure the type is not null.");
                        return false;
                    }
                    if (!typeof(T).IsSerializable)
                    {
                        Debug.LogWarning($"The Data Type {typeof(T).FullName} is not serialized. Please make sure to serialize the data type before performing any writing operations regarding the type.");
                        return false;
                    }
                    if (File.Exists(filePath))
                    {
                        if (overwriteExisted)
                            success = await ImprintToFileAsync(content, new WritingData(filePath, initializeIfEmpty, jsonFormat, cleanJson, overwriteExisted, encode ?? Encoding.Default));
                    }
                    else
                    {
                        if (initializeIfEmpty) ImprintToFile(content, new WritingData(filePath, initializeIfEmpty, jsonFormat, cleanJson, overwriteExisted, encode ?? Encoding.Default));
                        Debug.LogWarning($"The File Path {filePath} does not exist. Please make sure the file is created and initialized.");
                    }
                    return success;
                }
                public static bool WriteFile(T content, WritingData data) { return WriteFile(content, data.filePath, data.initializeIfEmpty, data.jsonFormat, data.cleanJson, data.overwriteExisted, data.encode ?? Encoding.Default); }
                public static bool WriteFile(T content, string filePath, bool initializeIfEmpty = true, bool jsonFormat = true, bool cleanJson = true, bool overwriteExisted = true, Encoding encode = null)
                {
                    bool success = false;
                    if (typeof(T) == null)
                    {
                        Debug.LogWarning($"The Data Type {typeof(T).FullName} is null. Please make sure the type is not null.");
                        return success;
                    }
                    if (!typeof(T).IsSerializable)
                    {
                        Debug.LogWarning($"The Data Type {typeof(T).FullName} is not serialized. Please make sure to serialize the data type before performing any writing operations regarding the type.");
                        return success;
                    }
                    if (File.Exists(filePath))
                    {
                        if (overwriteExisted)
                            success = ImprintToFile(content, new WritingData(filePath, initializeIfEmpty, jsonFormat, cleanJson, overwriteExisted, encode ?? Encoding.Default));
                    }
                    else
                    {
                        if (initializeIfEmpty) ImprintToFile(content, new WritingData(filePath, initializeIfEmpty, jsonFormat, cleanJson, overwriteExisted, encode ?? Encoding.Default));
                        Debug.LogWarning($"The File Path {filePath} does not exist. Please make sure the file is created and initialized.");
                    }
                    return success;
                }
                public async static Task<T> ReadFileAsync(ReadingData data) { return await ReadFileAsync(data.filePath, data.initializeIfEmpty, data.convertFromJson, data.encode ?? Encoding.Default); }
                public async static Task<T> ReadFileAsync(string filePath, bool initializeIfEmpty = true, bool convertFromJson = true, Encoding encode = null)
                {
                    object obj = null;
                    if (typeof(T) == null) return default;
                    if (File.Exists(filePath))
                    {
                        obj = await File.ReadAllTextAsync(filePath, encode ?? Encoding.Default);
                        if (convertFromJson)
                            obj = JsonUtility.FromJson((string)obj, typeof(T));
                        else
                            return default;
                    }
                    else
                    {
                        if (initializeIfEmpty)
                        {
                            WriteFile(new T(), new WritingData(filePath, initializeIfEmpty, convertFromJson, convertFromJson, convertFromJson, encode ?? Encoding.Default));
                            obj = File.ReadAllText(filePath, encode ?? Encoding.Default);
                            if (convertFromJson)
                                obj = JsonUtility.FromJson((string)obj, typeof(T));
                            else
                                return default;
                        }
                    }
                    OperatedFileAsync?.Invoke();
                    return (T)obj;
                }
                public static T ReadFile(ReadingData data) { return ReadFile(data.filePath, data.initializeIfEmpty, data.convertFromJson, data.encode ?? Encoding.Default); }
                public static T ReadFile(string filePath, bool initializeIfEmpty = true, bool convertFromJson = true, Encoding encode = null)
                {
                    object obj = null;
                    if (typeof(T) == null) return default;
                    if (File.Exists(filePath))
                    {
                        obj = File.ReadAllText(filePath, encode ?? Encoding.Default);
                        if (convertFromJson)
                            obj = JsonUtility.FromJson((string)obj, typeof(T));
                        else
                            return default;
                    }
                    else
                    {
                        if (initializeIfEmpty)
                        {
                            WriteFile(new T(), new WritingData(filePath, initializeIfEmpty, convertFromJson, convertFromJson, convertFromJson, encode ?? Encoding.Default));
                            obj = File.ReadAllText(filePath, encode ?? Encoding.Default);
                            if (convertFromJson)
                                obj = JsonUtility.FromJson((string)obj, typeof(T));
                            else
                                return default;
                        }
                    }
                    OperatedFile?.Invoke();
                    return (T)obj;
                }
                private static bool ImprintToFile(T content, WritingData data)
                {
                    try
                    {
                        if (File.Exists(data.filePath))
                        {
                            File.WriteAllText(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(content, true) : JsonUtility.ToJson(content, false) : content.ToString(), data.encode ?? Encoding.Default);
                        }
                        else
                        {
                            if (data.initializeIfEmpty)
                            {
                                File.Create(data.filePath).Close();
                                File.WriteAllText(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(new T(), true) : JsonUtility.ToJson(new T(), false) : new T().ToString(), data.encode ?? Encoding.Default);
                            }
                            else
                            {
                                return false;
                            }
                            File.WriteAllText(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(content, true) : JsonUtility.ToJson(content, false) : content.ToString(), data.encode ?? Encoding.Default);
                        }
                        OperatedFile?.Invoke();
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                    return true;
                }
                private async static Task<bool> ImprintToFileAsync(T content, WritingData data)
                {
                    try
                    {
                        if (File.Exists(data.filePath))
                        {
                            await File.WriteAllTextAsync(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(content, true) : JsonUtility.ToJson(content, false) : content.ToString(), data.encode ?? Encoding.Default);
                        }
                        else
                        {
                            if (data.initializeIfEmpty)
                            {
                                File.Create(data.filePath).Close();
                                await File.WriteAllTextAsync(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(new T(), true) : JsonUtility.ToJson(new T(), false) : new T().ToString(), data.encode ?? Encoding.Default);
                            }
                            else
                            {
                                return false;
                            }
                            await File.WriteAllTextAsync(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(content, true) : JsonUtility.ToJson(content, false) : content.ToString(), data.encode ?? Encoding.Default);
                        }
                        OperatedFileAsync?.Invoke();
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }
        namespace OnlineServices.MieServices
        {
            public static class MieServicesConfig
            {
                public static string CloudFetchLink { get { return "https://cloud.smartsheep.studio/api/serverless-functions/1/execute"; } }
            }
            public static class MieCloudOps<T> where T : new()
            {
                public static async Task<T?> FetchCloudKey(string key)
                {
                    HttpClient client = new();
                    var res = await client.GetStringAsync(MieServicesConfig.CloudFetchLink);
                    var releases = JsonUtility.FromJson<string?>(res);
                    if (releases == null)
                    {
                        Debug.LogError("Fetched Releases is null.");
                        return new T();
                    }
                    else
                        return JsonUtility.FromJson<T>(releases);
                }
            }
        }
    }
}