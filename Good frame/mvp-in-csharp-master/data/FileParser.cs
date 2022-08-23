using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace mvp_in_csharp.data
{
    /// <summary>
    /// 字符串和集合操作
    /// 1. 序列化
    /// 2. 反序列化
    /// </summary>
    public class FileParser
    {
        public string SerializeData(IList<Message> messages)
        {
            if (messages == null || messages.Count == 0)
                throw new ArgumentNullException();
            return Newtonsoft.Json.JsonConvert.SerializeObject(messages);
        }

        public IList<Message> DeserializeData(string text)
        {
            if (text == null || text.Trim().Equals(""))
                return null;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Message>>(text);
        }
    }
}
