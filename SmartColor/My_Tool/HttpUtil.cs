using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SmartColor.My_Tool
{
    /// <summary>
    /// HttpUtil 提供常用的 HTTP GET/POST 请求方法和 Base64 编码工具。
    /// </summary>
    internal class HttpUtil
    {
        // 默认 UserAgent 字符串
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        /// <summary>
        /// 创建并发送 HTTP GET 请求，返回 HttpWebResponse。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="timeout">超时时间（毫秒），可为 null</param>
        /// <param name="userAgent">自定义 UserAgent，可为 null</param>
        /// <param name="cookies">Cookie 集合，可为 null</param>
        /// <returns>HttpWebResponse 响应对象</returns>
        public static  HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            // 创建 HttpWebRequest 对象
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = string.IsNullOrEmpty(userAgent) ? DefaultUserAgent : userAgent;
            if (timeout.HasValue)
                request.Timeout = timeout.Value;
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            // 返回响应对象
            return (HttpWebResponse)request.GetResponse();
        }

        /// <summary>
        /// 创建并发送 HTTP POST 请求，返回 HttpWebResponse。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">POST 参数字典，可为 null</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <param name="userAgent">自定义 UserAgent，可为 null</param>
        /// <param name="cookies">Cookie 集合，可为 null</param>
        /// <returns>HttpWebResponse 响应对象</returns>
        public static  HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int timeout, string userAgent, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            // 创建 HttpWebRequest 对象
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = string.IsNullOrEmpty(userAgent) ? DefaultUserAgent : userAgent;
            request.Timeout = timeout;

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }

            // 组装 POST 数据
            if (parameters != null && parameters.Count > 0)
            {
                var buffer = new StringBuilder();
                foreach (var kvp in parameters)
                {
                    if (buffer.Length > 0)
                        buffer.Append('&');
                    buffer.AppendFormat("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value));
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            // 返回响应对象
            return (HttpWebResponse)request.GetResponse();
        }

        /// <summary>
        /// 对字符串进行 Base64 编码。
        /// </summary>
        /// <param name="str">待编码字符串</param>
        /// <returns>Base64 编码结果</returns>
        public static  string Base64Encrypt(string str)
        {
            if (str == null) return null;
            byte[] encbuff = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(encbuff);
        }
    }
}