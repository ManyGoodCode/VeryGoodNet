using System;
using System.IO;
using System.Collections.Generic;

namespace mvp_in_csharp.data
{
    /// <summary>
    /// 通过构造器注入 序列化，反序列化解析工具 FileParser 实现文件与集合之间的操作
    /// 1. 从文件中加载集合对象
    /// 2. 添加一个对象到文件中
    /// 3. 从文件中移除一个指定Id的对象
    /// 4. 往文件中写入一个集合对象
    /// </summary>
    public class InFileSavingHelper
    {
        private readonly FileParser parser;

        public string FilePath { get; set; }

        public InFileSavingHelper(FileParser parser, string filePath)
        {
            this.parser = parser;
            FilePath = filePath;
        }

        public IList<Message> LoadMessagesFromFile()
        {
            if (!File.Exists(FilePath))
                return null;
            TextReader reader = null;
            try
            {
                reader = new StreamReader(FilePath);
                string json = reader.ReadToEnd();
                return parser.DeserializeData(json);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public void AppendMessageToFile(Message message)
        {
            IList<Message> messages = LoadMessagesFromFile();
            if (messages == null)
                messages = new List<Message>();
            messages.Add(message);
            SaveMessagesToFile(messages);
        }

        public void RemoveMessageFromFile(long messageId)
        {
            throw new NotImplementedException();
        }

        public void SaveMessagesToFile(IList<Message> messages)
        {
            TextWriter writer = null;
            string jsonText = parser.SerializeData(messages);
            try
            {
                writer = new StreamWriter(FilePath);
                writer.WriteLine(jsonText);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }
    }
}
