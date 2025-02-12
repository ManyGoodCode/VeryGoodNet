﻿namespace EasyHttp.Codecs
{
    public interface IDecoder
    {
        T DecodeToStatic<T>(string input, string contentType);
        dynamic DecodeToDynamic(string input, string contentType);
        bool ShouldRemoveAtSign { get; set; }
    }
}