/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
using System.Net.Http;

namespace ET
{
    public static partial class HttpHelper
    {
        public static async ETTask<string> Get(string url)
        {
            HttpClient client = new HttpClient();
            //发送Get请求
            HttpResponseMessage responseMessage = await client.GetAsync(url);
            responseMessage.EnsureSuccessStatusCode();
            return await responseMessage.Content.ReadAsStringAsync();
        }
    }
}