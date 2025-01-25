﻿namespace Plugin.AdMob;

public interface IAdError
{
    string Message { get; }
}

internal class AdError(string message) 
    : IAdError
{
    public string Message { get; } = message;
}
