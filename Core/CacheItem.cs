using System;

namespace Core
{
    public class CacheItem
    {
        public string Data { get; set; }
        public DateTime Timestamp { get; set; }

        public CacheItem(string data)
        {
            Data = data;
            Timestamp = DateTime.Now;
        }
    }
}
