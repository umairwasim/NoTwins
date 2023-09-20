using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";
    private const string SAVE_EXTENSION = "txt";

    public static void Initialize()
    {
        if (!Directory.Exists(SAVE_FOLDER))
            Directory.CreateDirectory(SAVE_FOLDER);
    }

    public static void SaveData(string saveString)
    {
        File.WriteAllText(SAVE_FOLDER + "save." + SAVE_EXTENSION, saveString);
    }

    public static bool FileExists() => File.Exists(SAVE_FOLDER + "save." + SAVE_EXTENSION);

    public static string LoadData()
    {
        if (File.Exists(SAVE_FOLDER + "save." + SAVE_EXTENSION))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + "save." + SAVE_EXTENSION);
            return saveString;
        }
        else
            return null;
    }
}
