using System.IO;
using UnityEngine;

public static class CustomizationSerializeService
{
    public static readonly string SavedFilePath = Application.persistentDataPath + "/Customizer.json";
    
    public static void Serialize(PlayerCustomization customization)
    {
        var jsonString = JsonUtility.ToJson(customization);
        File.WriteAllText(SavedFilePath, jsonString);
    }

    public static PlayerCustomization Deserialize()
    {
        var jsonString = File.ReadAllText(SavedFilePath);
        return File.Exists(SavedFilePath) ? JsonUtility.FromJson<PlayerCustomization>(jsonString) : null;
    }
    
    public static string Deserialize(bool returnString) => File.ReadAllText(SavedFilePath);

    public static PlayerCustomization Deserialize(string jsonString) =>
        JsonUtility.FromJson<PlayerCustomization>(jsonString);
}
