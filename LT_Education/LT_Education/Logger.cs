
using System;
using System.IO;
using TaleWorlds.Library;


#nullable enable
namespace LT_Education
{
  public static class Logger
  {
    public const string ModuleId = "LT_Education";
    private static readonly string LOG_PATH = "..\\\\..\\\\Modules\\\\LT_Education\\logs\\";
    private static readonly string ERROR_FILE = Logger.LOG_PATH + "error.log";
    private static readonly string DEBUG_FILE = Logger.LOG_PATH + "debug.log";
    public static bool logToFile = false;
    public static bool IsDebug = false;
    public static string? PrePrend = (string) null;
    public static string ModVersion = "";

    static Logger()
    {
      if (!Directory.Exists(Logger.LOG_PATH))
        Directory.CreateDirectory(Logger.LOG_PATH);
      if (!File.Exists(Logger.ERROR_FILE))
        File.Create(Logger.ERROR_FILE);
      if (File.Exists(Logger.DEBUG_FILE))
        return;
      File.Create(Logger.DEBUG_FILE);
    }

    public static void LogDebug(string log)
    {
      using (StreamWriter streamWriter = new StreamWriter(Logger.DEBUG_FILE, true))
        streamWriter.WriteLine(log);
      Logger.DisplayInfoMsg("DEBUG | " + log);
    }

    public static void LogError(string log)
    {
      using (StreamWriter streamWriter = new StreamWriter(Logger.ERROR_FILE, true))
        streamWriter.WriteLine(log);
    }

    public static void LogError(Exception exception)
    {
      Logger.LogError("Message " + exception.Message);
      Logger.LogError("Error at " + exception.Source.ToString() + " in function " + exception.Message);
      Logger.LogError("With stacktrace :\n" + exception.StackTrace);
      Logger.LogError("----------------------------------------------------");
      Logger.DisplayInfoMsg(exception.Message);
      Logger.DisplayInfoMsg(exception.Source);
      Logger.DisplayInfoMsg(exception.StackTrace);
    }

    public static void DisplayInfoMsg(string message) => InformationManager.DisplayMessage(new InformationMessage(message));

    public static void IM(string message) => Logger.DisplayColorInfoMessage(message, Color.ConvertStringToColor("#FFFFFFFF"), Logger.logToFile);

    public static void IMGreen(string message) => Logger.DisplayColorInfoMessage(message, Color.ConvertStringToColor("#42FF00FF"), Logger.logToFile);

    public static void IMRed(string message) => Logger.DisplayColorInfoMessage(message, Color.ConvertStringToColor("#FF0042FF"), Logger.logToFile);

    public static void IMBlue(string message) => Logger.DisplayColorInfoMessage(message, Color.ConvertStringToColor("#4242FFFF"), Logger.logToFile);

    public static void IMGrey(string message) => Logger.DisplayColorInfoMessage(message, Color.ConvertStringToColor("#AAAAAAFF"), Logger.logToFile);

    private static void DisplayColorInfoMessage(string message, Color messageColor, bool logToFile = false)
    {
      string log = message;
      if (!string.IsNullOrWhiteSpace(Logger.PrePrend))
        log = Logger.PrePrend + " : " + message;
      try
      {
        if (logToFile)
          Logger.LogDebug(log);
        InformationManager.DisplayMessage(new InformationMessage(log, messageColor));
      }
      catch (Exception ex)
      {
        Logger.LogError("messageStrLength: " + log.Length.ToString());
        Logger.LogError("messageStr: " + log);
        Logger.LogError(ex);
      }
    }
  }
}
