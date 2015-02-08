using System;

namespace Core
{
    public interface IPageProvider : IDisposable
    {
        string Get(string url);
        string Status();
    }
}