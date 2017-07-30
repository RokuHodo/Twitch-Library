using System;

namespace TwitchLibrary.Enums.Debug
{
    [Flags]
    public enum LogLevel
    {
        None        = 0,
        Errors      = 1,
        Warnings    = 2,
        Success     = 4,
        Headers     = 8,
        Detailed    = 16,
        All         = 32
    }
}
