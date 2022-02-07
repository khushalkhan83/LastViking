using System;
using UnityEngine;

public static class Logger
{
    public static string Label { get; } = @"[iLoggeri]";

    public static T Log<T>(this T value, string prompt = null)
    {
        Debug.Log(Label + prompt + "[" + value + "]");

        return value;
    }

    public static T Log<T, K>(this T value, Func<T, K> accessor, string prompt = null)
    {
        Debug.Log(Label + prompt + "[" + accessor(value) + "]");

        return value;
    }

    public static void LogFormat(this string format, params object[] args)
    {
        Debug.LogFormat(Label + format, args);
    }

    public static T Error<T>(this T value, string prompt = null)
    {
        Debug.LogError(Label + prompt + "[" + value + "]");

        return value;
    }

    public static T Warning<T>(this T value, string prompt = null)
    {
        Debug.LogWarning(Label + prompt + "[" + value + "]");

        return value;
    }
}
