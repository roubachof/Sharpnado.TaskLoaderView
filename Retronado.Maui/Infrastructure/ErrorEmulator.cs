﻿namespace Sample.Infrastructure
{
    public enum ErrorType
    {
        None = 0,
        Unknown = 1,
        Network = 2,
        Server = 3,
        NoData = 4,
        ErrorOnRefresh = 5,
    }

    public class ErrorEmulator
    {
        public ErrorType ErrorType { get; set; }
    }
}
