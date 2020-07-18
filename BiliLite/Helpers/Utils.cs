﻿using BiliLite.Api;
using BiliLite.Controls;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using System.IO;

namespace BiliLite.Helpers
{
    public static class Utils
    {
        /// <summary>
        /// 发送请求，扩展方法
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public async static Task<HttpResults> Request(this ApiModel api)
        {
            if (api.method == RestSharp.Method.GET)
            {
                return await HttpHelper.Get(api.url, api.headers);
            }
            else
            {
                return await HttpHelper.Post(api.url, api.body, api.headers);
            }
        }

        /// <summary>
        /// 默认一些请求头
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, string> GetDefaultHeaders()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("user-agent", "Mozilla/5.0 BiliDroid/5.34.1 (bbcallen@gmail.com)");
            headers.Add("Referer", "https://www.bilibili.com/");
            return headers;
        }
        /// <summary>
        /// 将时间戳转为时间
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static DateTime TimestampToDatetime(long ts)
        {
            DateTime dtStart = new DateTime(1970, 1, 1, 8, 0, 0);
            long lTime = long.Parse(ts + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        /// <summary>
        /// 生成时间戳/秒
        /// </summary>
        /// <returns></returns>
        public static long GetTimestampS()
        {
            return Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0, 0)).TotalSeconds);
        }
        /// <summary>
        /// 生成时间戳/豪秒
        /// </summary>
        /// <returns></returns>
        public static long GetTimestampMS()
        {
            return Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0, 0)).TotalMilliseconds);
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToMD5(string input)
        {
            var provider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(input, BinaryStringEncoding.Utf8);
            var hashed = provider.HashData(buffer);
            var result = CryptographicBuffer.EncodeToHexString(hashed);
            return result;
        }
        public static void ShowMessageToast(string message)
        {
            MessageToast ms = new MessageToast(message, TimeSpan.FromSeconds(2));
            ms.Show();
        }
        public static void ShowMessageToast(string message, List<MyUICommand> commands, int seconds = 15)
        {
            MessageToast ms = new MessageToast(message, TimeSpan.FromSeconds(seconds), commands);
            ms.Show();
        }
        public static int ToInt32(this object obj)
        {

            if (int.TryParse(obj.ToString(), out var value))
            {
                return value;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 根据Epid取番剧ID
        /// </summary>
        /// <returns></returns>
        public async static Task<string> BangumiEpidToSid(string url)
        {
            try
            {
                if (!url.Contains("http"))
                {
                    url = "https://www.bilibili.com/bangumi/play/ep" + url;
                }

                var re = await HttpHelper.GetString(url);
                var data = RegexMatch(re, @"ss(\d+)");
                if (data != "")
                {
                    return data;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        private static bool dialogShowing = false;
        public async static Task<bool> ShowLoginDialog()
        {
            if (!dialogShowing)
            {
                LoginDialog login = new LoginDialog();
                dialogShowing = true;
                await login.ShowAsync();
                dialogShowing = false;
            }
            if (SettingHelper.Account.Logined)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string RegexMatch(string input, string regular)
        {
            var data = Regex.Match(input, regular);
            if (data.Groups.Count >= 2 && data.Groups[1].Value != "")
            {
                return data.Groups[1].Value;
            }
            else
            {
                return "";
            }
        }
        public static async Task<T> DeserializeJson<T>(this string results)
        {
            return await Task.Run<T>(() =>
            {
                return JsonConvert.DeserializeObject<T>(results);
            });
        }

        public static bool SetClipboard(string content)
        {
            try
            {
                Windows.ApplicationModel.DataTransfer.DataPackage pack = new Windows.ApplicationModel.DataTransfer.DataPackage();
                pack.SetText(content);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(pack);
                Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static string HandelTimestamp(string ts)
        {
            if (ts.Length == 10)
            {
                ts += "0000000";
            }
            DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0);
            long lTime = long.Parse(ts);
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dt = dtStart.Add(toNow).ToLocalTime();
            TimeSpan span = DateTime.Now.Date - dt.Date;
            if (span.TotalDays <= 0)
            {
                return "今天" + dt.ToString("HH:mm");
            }
            else if (span.TotalDays >= 1 && span.TotalDays < 2)
            {
                return "昨天" + dt.ToString("HH:mm");
            }
            else
            {
                return dt.ToString("MM-dd");
            }
        }

        public async static Task CheckVersion()
        {
            try
            {
                var url = $"https://pic.nsapps.cn/biliuwp/bilidev.json?ts{Utils.GetTimestampS()}";
                var result = await HttpHelper.GetString(url);
                var ver = JsonConvert.DeserializeObject<NewDevVersion>(result);
                var v= $"{ SystemInformation.ApplicationVersion.Major }.{ SystemInformation.ApplicationVersion.Minor}.{ SystemInformation.ApplicationVersion.Build}";
                if (v!=ver.version)
                {
                    var cd = new ContentDialog();
                    StackPanel stackPanel = new StackPanel();
                    cd.Title = "发现新版本";
                    TextBlock content = new TextBlock()
                    {
                        Text = ver.version_desc,
                        TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap,
                        IsTextSelectionEnabled = true
                    };
                    stackPanel.Children.Add(content);
                    cd.Content = stackPanel;
                    cd.PrimaryButtonText = "立即更新";
                    cd.SecondaryButtonText = "忽略";

                    cd.PrimaryButtonClick += new Windows.Foundation.TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs>(async (sender, e) =>
                    {
                        await Windows.System.Launcher.LaunchUriAsync(new Uri(ver.url));
                    });
                    await cd.ShowAsync();
                }
            }
            catch (Exception)
            {
            }
        }

        public static Color ToColor(this string obj)
        {
            obj = obj.Replace("#", "");
            if (int.TryParse(obj,out var c))
            {
                obj = c.ToString("X2");
            }
            Color color = new Color();
            if (obj.Length == 4)
            {
                obj = "00" + obj;
            }
            if (obj.Length == 6)
            {
                color.R = byte.Parse(obj.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                color.G = byte.Parse(obj.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                color.B = byte.Parse(obj.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                color.A = 255;
            }
            if (obj.Length == 8)
            {
                color.R = byte.Parse(obj.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                color.G = byte.Parse(obj.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                color.B = byte.Parse(obj.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                color.A = byte.Parse(obj.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return color;
        }
        public static void ReadB(this Stream stream, byte[] buffer, int offset, int count)
        {
            if (offset + count > buffer.Length)
                throw new ArgumentException();
            var read = 0;
            while (read < count)
            {
                var available = stream.Read(buffer, offset, count - read);
                if (available == 0)
                {
                    // throw new ObjectDisposedException(null);
                }
                read += available;
                offset += available;
            }
        }
    }
    public class NewDevVersion
    {
        public string version { get; set; }
        public string version_desc { get; set; }
        public string url { get; set; }
    }
}