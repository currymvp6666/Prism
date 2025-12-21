using Prism.Models;

using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Prism.Services
{
    public class DeepSeekService
    {
        private readonly string apiKey = "sk-787150a5c2ff44ac8e7d0c77f3d24e02";//输入自己的apikey。
        private readonly HttpClient client;
        private readonly string baseUrl = "https://api.deepseek.com";

        public DeepSeekService()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        // ===============================
        // ① 原始聊天接口（保留）
        // ===============================
        public async Task<string> GetReplyAsync(string userMessage)
        {
            var payload = BuildPayload(userMessage);

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/v1/chat/completions", content);

            if (!response.IsSuccessStatusCode)
                return "哎哟~网络好像出问题了";

            return await ExtractTextAsync(response);
        }

        // ===============================
        // ② 新增：AI → 指令解析（核心）
        // ===============================
        public async Task<(AiCommand command, string chatReply)> ParseCommandAsync(string userMessage)
        {
            var reply = await GetReplyAsync(userMessage);

            // 1. 处理可能存在的 Markdown 代码块包裹
            string cleanReply = reply.Trim();
            if (cleanReply.StartsWith("```"))
            {
                // 移除开头的 ```json 或 ```
                cleanReply = System.Text.RegularExpressions.Regex.Replace(cleanReply, @"^```[a-zA-Z]*\n?", "");
                // 移除结尾的 ```
                cleanReply = System.Text.RegularExpressions.Regex.Replace(cleanReply, @"\n?```$", "");
                cleanReply = cleanReply.Trim();
            }

            // 2. 检查是否确实以 { 开始（初步判断是否为 JSON）
            if (cleanReply.StartsWith("{"))
            {
                try
                {
                    // 3. 配置反序列化选项：忽略大小写
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var cmd = JsonSerializer.Deserialize<AiCommand>(cleanReply, options);

                    if (!string.IsNullOrWhiteSpace(cmd?.Action))
                        return (cmd, null);
                }
                catch (JsonException)
                {
                    // 解析失败，说明 AI 返回的不是有效的 JSON 格式，按普通聊天处理
                }
            }

            return (null, reply);
        }

        // ===============================
        // 工具方法
        // ===============================
        private object BuildPayload(string userMessage)
        {
            return new
            {
                model = "deepseek-chat",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                       content = @"你是一个桌面AI助手【坤坤】。
你的任务：判断用户是否想【新增备忘录/记事】。

如果是新增备忘录：
1. 必须只返回 JSON，禁止包含任何 Markdown 标签或解释文字。
2. 格式必须是：{""action"": ""add_memo"", ""title"": ""标题"", ""content"": ""内容""}。

如果不是新增备忘录：
正常口语化聊天。"
                    },
                    new { role = "user", content = userMessage }
                }
            };
        }

        private async Task<string> ExtractTextAsync(HttpResponseMessage response)
        {
            var result = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(result);

            return doc.RootElement
                      .GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString();
        }
    }
}
