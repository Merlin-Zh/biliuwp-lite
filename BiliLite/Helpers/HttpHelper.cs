﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using System.IO;
using Windows.Web.Http;
using Windows.Storage.Streams;
using Windows.Web.Http.Filters;
using BiliLite.Models;
using System.Runtime.InteropServices.WindowsRuntime;

namespace BiliLite.Helpers
{
    /// <summary>
    /// 网络请求方法封装
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// 发送一个GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public async static Task<HttpResults> Get(string url, IDictionary<string, string> headers = null)
        {
            try
            {
                HttpBaseProtocolFilter fiter = new HttpBaseProtocolFilter();
               
                fiter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);
                using (var client = new HttpClient(fiter))
                {
                    if (headers != null)
                    {
                        foreach (var item in headers)
                        {
                            client.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }
                   
                    var response = await client.GetAsync(new Uri(url));
                    if (!response.IsSuccessStatusCode)
                    {
                        return new HttpResults()
                        {
                            code = (int)response.StatusCode,
                            status = false,
                            message = StatusCodeToMessage((int)response.StatusCode)
                        };
                    }
                    response.EnsureSuccessStatusCode();
                    var buffer = await response.Content.ReadAsBufferAsync();
                    var byteArray = buffer.ToArray();
                    HttpResults httpResults = new HttpResults()
                    {
                        code = (int)response.StatusCode,
                        status = response.StatusCode == HttpStatusCode.Ok,
                        results = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length),
                        message = StatusCodeToMessage((int)response.StatusCode)
                    };
                    return httpResults;
                }



            }

            catch (Exception ex)
            {
                LogHelper.Log("GET请求失败" + url, LogType.ERROR, ex);
                return new HttpResults()
                {
                    code = ex.HResult,
                    status = false,
                    message = "网络请求出现错误(GET)"
                };
            }
        }

        /// <summary>
        /// 发送一个GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public async static Task<Stream> GetStream(string url, IDictionary<string, string> headers = null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    if (headers != null)
                    {
                        foreach (var item in headers)
                        {
                            client.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }

                    var response = await client.GetAsync(new Uri(url));
                    response.EnsureSuccessStatusCode();
                    return (await response.Content.ReadAsInputStreamAsync()).AsStreamForRead();
                }


            }
            catch (Exception ex)
            {
                LogHelper.Log("GET请求Stream失败" + url, LogType.ERROR, ex);
                return null;

            }



        }
        /// <summary>
        /// 发送一个GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public async static Task<IBuffer> GetBuffer(string url, IDictionary<string, string> headers = null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    if (headers != null)
                    {
                        foreach (var item in headers)
                        {
                            client.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }

                    var response = await client.GetAsync(new Uri(url));
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsBufferAsync();
                }


            }
            catch (Exception ex)
            {
                LogHelper.Log("GET请求Stream失败" + url, LogType.ERROR, ex);
                return null;

            }



        }
        /// <summary>
        /// 发送一个GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public async static Task<String> GetString(string url, IDictionary<string, string> headers = null, IDictionary<string, string> cookie = null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    if (headers != null)
                    {
                        foreach (var item in headers)
                        {
                            client.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }

                    var response = await client.GetAsync(new Uri(url));
                    response.EnsureSuccessStatusCode();
                    var buffer = await response.Content.ReadAsBufferAsync();
                    return  Encoding.UTF8.GetString(buffer.ToArray());
                }


            }
            catch (Exception ex)
            {
                LogHelper.Log("GET请求String失败" + url, LogType.ERROR, ex);

                return null;

            }



        }

        /// <summary>
        /// 发送一个POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="headers"></param>
        /// <param name="cookie"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async static Task<HttpResults> Post(string url, string body, IDictionary<string, string> headers = null, string contentType = "application/x-www-form-urlencoded")
        {
            try
            {
                using (var client = new HttpClient())
                {
                    if (headers != null)
                    {
                        foreach (var item in headers)
                        {
                            client.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }
                    var response = await client.PostAsync(new Uri(url), new HttpStringContent(body, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded"));
                    if (!response.IsSuccessStatusCode)
                    {
                        return new HttpResults()
                        {
                            code = (int)response.StatusCode,
                            status = false,
                            message = StatusCodeToMessage((int)response.StatusCode)
                        };
                    }
                    var buffer = await response.Content.ReadAsBufferAsync();
                    var byteArray = buffer.ToArray();
                 
                    HttpResults httpResults = new HttpResults()
                    {
                        code = (int)response.StatusCode,
                        status = response.StatusCode == HttpStatusCode.Ok,
                        results = Encoding.UTF8.GetString(byteArray),
                        message = StatusCodeToMessage((int)response.StatusCode)
                    };
                    return httpResults;
                }



            }
            catch (Exception ex)
            {
                LogHelper.Log("POST请求失败" + url, LogType.ERROR, ex);
                return new HttpResults()
                {
                    code = ex.HResult,
                    status = false,
                    message ="网络请求出现错误(POST)"
                };
            }
        }




        private static string StatusCodeToMessage(int code)
        {

            switch (code)
            {
                case 0:
                case 200:
                    return "请求成功";
                case 504:
                    return "请求超时了";
                case 301:
                case 302:
                case 303:
                case 305:
                case 306:
                case 400:
                case 401:
                case 402:
                case 403:
                case 404:
                case 500:
                case 501:
                case 502:
                case 503:
                case 505:
                    return "网络请求失败，代码:" + code;
                case -2147012867:
                case -2147012889:
                    return "请检查的网络连接";
                default:
                    return "未知错误";
            }
        }
    }


    public class HttpResults
    {
        public int code { get; set; }
        public string message { get; set; }
        public string results { get; set; }
        public bool status { get; set; }
        public async Task<T> GetJson<T>()
        {
            return await Task.Run<T>(() =>
             {
                 return JsonConvert.DeserializeObject<T>(results);
             });

        }
        public JObject GetJObject()
        {
            try
            {
                var obj = JObject.Parse(results);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }

        }
        public async Task<ApiDataModel<T>> GetData<T>()
        {
            try
            {
                return await GetJson<ApiDataModel<T>>();
            }
            catch (Exception)
            {
                return null;
            }

        }
        public async Task<ApiResultModel<T>> GetResult<T>()
        {
            try
            {
                return await GetJson<ApiResultModel<T>>();
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
