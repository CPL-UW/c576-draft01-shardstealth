using System;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
[Serializable]
public class Report
{
    public string version = "0.0.1";
    public string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    public double epoch = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    public GameData gameData;
    public string userId;
    public Report(string userId, GameData gameData)
    {
        this.userId = userId;
        this.gameData = gameData;
    } 
}
public static class Analytics
{
    private const string path = "Assets/Logs/userLog.json";
    private static readonly StreamWriter Writer = new(path, true);
    public static void ReportNewDay(string userId, GameData gameData)
    {
        Writer.WriteLine(JsonUtility.ToJson(new Report(userId, gameData)));
        Writer.Flush();
    }

    public static void Close() => Writer.Close();
}

