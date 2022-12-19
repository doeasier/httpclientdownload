using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace System.Net.Http.DownloadExtensions
{
    public static class DownloadExtensions
    {
        /// <summary>
        /// 通过 HttpClient 下载文件
        /// </summary>
        /// <param name="client">HttpClient 对象</param>
        /// <param name="url">下载地址</param>
        /// <param name="filename">保存文件名</param>
        /// <param name="isOverWrite">文件存在时是否覆盖</param>
        /// <param name="silent">是否抛出异常</param>
        /// <returns>成功返回true,失败返回false</returns>
        /// <exception cref="Exception"></exception>
        public static async Task<bool> DownloadFilesAsync(this HttpClient client, string url, string filename, bool isOverWrite=false, bool silent = true)
        {
            try
            {
                if(!isOverWrite && File.Exists(filename))
                {
                    if (silent) return false;
                    else throw new Exception($"{filename} is exists.");
                }
                var path = Path.GetDirectoryName(filename);
                var dir = new DirectoryInfo(path);
                if (!dir.Exists) dir.Create();
                const int LargeBufferSize = 1024 * 1024 * 1024;
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    using (Stream contentStream = await (await client.SendAsync(request)).Content.ReadAsStreamAsync(), stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, LargeBufferSize, true))
                    {
                        await contentStream.CopyToAsync(stream);
                        stream.Close();
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception($"DownloadFileAsync occur error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 通过 HttpClient 下载文件
        /// </summary>
        /// <param name="url">文件下载地址</param>
        /// <param name="filename">保存文件名</param>
        /// <param name="isOverWrite">文件存在时是否覆盖</param>
        /// <param name="silent">是否抛出弃常</param>
        /// <returns>成功返回true,失败返回false</returns>
        /// <exception cref="Exception"></exception>
        public static async Task<bool> DownloadFilesAsync(this string url, string filename, bool isOverWrite = false, bool silent = true)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    return await client.DownloadFilesAsync(url, filename, isOverWrite, silent);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"DownloadFileAsync occur error: {ex.Message}", ex);
            }

        }
    }
}
