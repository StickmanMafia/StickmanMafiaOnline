using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class SaveManagerStatic
{
    public static string Load()
    {
        string custom=PlayerPrefs.GetString("Customization");
        
        return custom;
    }
}

