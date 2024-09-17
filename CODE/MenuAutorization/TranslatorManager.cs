using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslatorManager : MonoBehaviour
{
    public static void TranslateAll(){
        foreach (var item in FindObjectsOfType<Translator>())
        {
            item.TranslateAll();
        }
    }
    
}
