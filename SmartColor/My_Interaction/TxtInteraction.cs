using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmartColor.My_Interaction
{
    /// <summary>
    /// Txt交互，支持key=value格式的增删改查（单个/多个/全部），自动处理文件不存在
    /// </summary>
    internal class TxtInteraction
    {
        /// <summary>
        /// 读取全部 key=value 到字典，文件不存在返回空字典
        /// </summary>
        public Dictionary<string, string> ReadAll(string path)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(path))
                return dict;
            try
            {
                foreach (var line in File.ReadAllLines(path))
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#")) continue;
                    var idx = trimmed.IndexOf('=');
                    if (idx > 0)
                    {
                        var key = trimmed.Substring(0, idx).Trim();
                        var value = trimmed.Substring(idx + 1).Trim();
                        dict[key] = value;
                    }
                }
            }
            catch
            {
                // 读取异常时返回空字典
            }
            return dict;
        }

        /// <summary>
        /// 写入全部 key=value（覆盖原文件），文件不存在自动创建
        /// </summary>
        public bool WriteAll(string path, Dictionary<string, string> dict)
        {
            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var lines = dict.Select(kv => $"{kv.Key}={kv.Value}").ToArray();
                File.WriteAllLines(path, lines);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ---------------- 查询 ----------------

        /// <summary>
        /// 查询单个key，文件不存在返回null
        /// </summary>
        public string Get(string path, string key)
        {
            var dict = ReadAll(path);
            return dict.TryGetValue(key, out var value) ? value : null;
        }

        /// <summary>
        /// 查询多个key，文件不存在返回空字典
        /// </summary>
        public Dictionary<string, string> GetMany(string path, IEnumerable<string> keys)
        {
            var dict = ReadAll(path);
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in keys)
            {
                if (dict.TryGetValue(key, out var value))
                    result[key] = value;
            }
            return result;
        }

        /// <summary>
        /// 查询全部，文件不存在返回空字典
        /// </summary>
        public Dictionary<string, string> GetAll(string path)
        {
            return ReadAll(path);
        }

        // ---------------- 新增/修改 ----------------

        /// <summary>
        /// 新增或修改单个key，文件不存在自动创建
        /// </summary>
        public bool Set(string path, string key, string value)
        {
            var dict = ReadAll(path);
            dict[key] = value;
            return WriteAll(path, dict);
        }

        /// <summary>
        /// 新增或修改多个key，文件不存在自动创建
        /// </summary>
        public bool SetMany(string path, Dictionary<string, string> kvs)
        {
            var dict = ReadAll(path);
            foreach (var kv in kvs)
            {
                dict[kv.Key] = kv.Value;
            }
            return WriteAll(path, dict);
        }

        /// <summary>
        /// 新增或修改全部（覆盖原有所有内容），文件不存在自动创建
        /// </summary>
        public bool SetAll(string path, Dictionary<string, string> kvs)
        {
            return WriteAll(path, kvs);
        }

        // ---------------- 删除 ----------------

        /// <summary>
        /// 删除单个key，文件不存在返回false
        /// </summary>
        public bool Remove(string path, string key)
        {
            var dict = ReadAll(path);
            if (dict.Remove(key))
                return WriteAll(path, dict);
            return false;
        }

        /// <summary>
        /// 删除多个key，文件不存在返回false
        /// </summary>
        public bool RemoveMany(string path, IEnumerable<string> keys)
        {
            var dict = ReadAll(path);
            bool changed = false;
            foreach (var key in keys)
            {
                if (dict.Remove(key))
                    changed = true;
            }
            if (changed)
                return WriteAll(path, dict);
            return false;
        }

        /// <summary>
        /// 删除全部，文件不存在自动创建空文件
        /// </summary>
        public bool RemoveAll(string path)
        {
            try
            {
                File.WriteAllText(path, string.Empty);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 创建空文件，如果已存在则不做任何操作
        /// </summary>
        public bool CreateFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    // 创建空文件
                    using (File.Create(path)) { }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}