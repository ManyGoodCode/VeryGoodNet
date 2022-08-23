using System;
using Newtonsoft.Json;
namespace mvp_in_csharp.data
{
    /// <summary>
    /// 信息实体结构体类型
    /// </summary>
    public struct Message
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("created")]
        public DateTime Created { get; set; }

        public Message(long id, string content, DateTime created)
        {
            this.Id = id;
            this.Content = content;
            this.Created = created;
        }

        public override string ToString()
        {
            return string.Format("[Message: Id={0}, Content={1}, Created={2}]", Id, Content, Created);
        }
    }
}
