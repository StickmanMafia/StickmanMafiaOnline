using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Translator : MonoBehaviour
{
    [SerializeField] private string Translate;
    [SerializeField] private string Translate2;

    private string BaseString;

    // Start is called before the first frame update
    void Start()
    {
        BaseString = GetComponent<Text>().text;
        TranslateAll();
    }

    public void TranslateAll()
    {
        if (PlayerPrefs.GetInt("language") == 0)
        {
            GetComponent<Text>().text = Translate;
        }
        else if (PlayerPrefs.GetInt("language") == 2)
        {
            
            GetComponent<Text>().text = Translate2.Replace("і", "i");
        }
        else
        {
            GetComponent<Text>().text = BaseString;
        }
    }
}
