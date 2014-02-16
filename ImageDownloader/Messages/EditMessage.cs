using System;

namespace ImageDownloader.Messages
{
    [Flags]
    public enum EditMessage
    {
        AllDisabled    = 0,
        EnablePrevious = 1 << 0,
        EnableNext     = 1 << 1,
        EnableDownload = 1 << 2
    };
}
