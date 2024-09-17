using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;

public class CustomizerLogic : UIController
{
    [SerializeField] 
    private GameObject _targetModel;

    private VisualElement _root;
    public SaveManager saver;
    public const string HatPrefabName = "Hat";
    public const string GlassesPrefabName = "Glasses";
    public const string MustachePrefabName = "Mustache";

    private const string USSColourButtonHeader = "colour-button-";
    
    public static void LoadConfig(GameObject skinParent, GameObject itemParent, string jsonString)
    {
        var config =JsonUtility.FromJson<PlayerCustomization>(jsonString);
        if (config == null) return;
        
        ChangeSkinColour(config.SkinColour.Split(" ")[0], skinParent);

        foreach (var item in config.ItemsEnabled) 
            EnableCustomizedObject(item, itemParent);
    }   
       
    public static void LoadConfig(GameObject skinParent, GameObject itemParent)
    {
        var config = JsonUtility.FromJson<PlayerCustomization>(PlayerPrefs.GetString("Customization"));
        
        if (config == null) return;
        
        ChangeSkinColour(config.SkinColour.Split(" ")[0], skinParent);

        foreach (var item in config.ItemsEnabled) 
            ToggleCustomizedObject(item, itemParent);
    }   

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

       // var whiteColourButton = GetColourButton("white");
       // var whiteSemiColourButton = GetColourButton("white-semi");
      //  var whiteDarkerColourButton = GetColourButton("white-darker");
      //  var whiteSemitannedColourButton = GetColourButton("white-semitanned");
      //  var whiteTannedColourButton = GetColourButton("white-tanned");
     //   var tannedColourButton = GetColourButton("tanned");
    //    var lightBrownColourButton = GetColourButton("light-brown");
    //    var darkerBrownColourButton = GetColourButton("darker-brown");
    //    var pureBrownColourButton = GetColourButton("pure-brown");
    //    var semiBlackColourButton = GetColourButton("semi-black");
     //   var nearlyBlackColourButton = GetColourButton("nearly-black");
    //    var blackColourButton = GetColourButton("black");
        
        //var toggleHatButton = _root.Q<VisualElement>("hat-button").Q<Button>();
        //var toggleGlassButton = _root.Q<VisualElement>("glasses-button").Q<Button>();
        //var toggleMustacheButton = _root.Q<VisualElement>("mustache-button").Q<Button>();

        var exitButton = _root.Q<Button>("exit-button");
        var saveButton = _root.Q<Button>("save-button");

      //  whiteColourButton.clicked += () => ChangeSkinColour("White");
      //  whiteSemiColourButton.clicked += () => ChangeSkinColour("WhiteSemi");
      //  whiteDarkerColourButton.clicked += () => ChangeSkinColour("WhiteDarker");
      //   whiteSemitannedColourButton.clicked += () => ChangeSkinColour("WhiteSemitanned");
     //   whiteTannedColourButton.clicked += () => ChangeSkinColour("WhiteTanned");
     //   tannedColourButton.clicked += () => ChangeSkinColour("Tanned");
      //  lightBrownColourButton.clicked += () => ChangeSkinColour("LightBrown");
      //  darkerBrownColourButton.clicked += () => ChangeSkinColour("DarkerBrown");
      //  pureBrownColourButton.clicked += () => ChangeSkinColour("PureBrown");
       // semiBlackColourButton.clicked += () => ChangeSkinColour("SemiBlack");
       // nearlyBlackColourButton.clicked += () => ChangeSkinColour("NearlyBlack");
      //  blackColourButton.clicked += () => ChangeSkinColour("Black");
        
        //toggleHatButton.clicked += () => ToggleCustomizedObject(HatPrefabName);
        //toggleGlassButton.clicked += () => ToggleCustomizedObject(GlassesPrefabName);
        //toggleMustacheButton.clicked += () => ToggleCustomizedObject(MustachePrefabName);

      //  exitButton.clicked += () => SceneManager.LoadScene("MainMenu");
      //  saveButton.clicked += async () => await SaveConfigAsync();

        LoadConfig();
    }
    
    private static void ChangeSkinColour(string materialName, GameObject target)
    {
        var material = Resources.Load(materialName, typeof(Material)) as Material;
        target.GetComponent<SkinnedMeshRenderer>().material = material;
    }
    
    private static void EnableCustomizedObject(string prefabName, GameObject parent)
    {
        if (GameObject.Find(prefabName) != null)
        {
            var targetObject = GameObject.Find(prefabName);
            Destroy(targetObject);
        }
        
        var prefab = Resources.Load(prefabName, typeof(GameObject)) as GameObject;
        var newInstance = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
        newInstance.name = prefabName;
        newInstance.transform.SetParent(parent.transform, false);
    }
    
    private static void ToggleCustomizedObject(string prefabName, GameObject parent)
    {
        if (GameObject.Find(prefabName) != null)
        {
            var targetObject = GameObject.Find(prefabName);
            Destroy(targetObject);
            return;
        }
        
        var prefab = Resources.Load(prefabName, typeof(GameObject)) as GameObject;
        var newInstance = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
        newInstance.name = prefabName;
        newInstance.transform.SetParent(parent.transform, false);
    }
    
    public void ChangeSkinColour(string materialName)
    {
        var material = Resources.Load(materialName, typeof(Material)) as Material;
        _targetModel.GetComponent<SkinnedMeshRenderer>().material = material;
    }
    
    public static void ToggleCustomizedObject(string prefabName, string parentName = "none")
    {
        if (GameObject.Find(prefabName) != null)
        {
            var targetObject = GameObject.Find(prefabName);
            Destroy(targetObject);
            return;
        }
        var targetParent = GameObject.Find("HeadMain");
         string[] splitName = prefabName.Split('_');
        string type = "_"+splitName[1].ToLower();
    foreach(Transform ch in targetParent.transform){
        if(ch.gameObject.name.Contains(type)){
            Destroy(ch.gameObject);
        }
    }
        var prefab = Resources.Load(prefabName, typeof(GameObject)) as GameObject;
        
        var newInstance = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
        newInstance.name = prefabName;
        newInstance.transform.SetParent(targetParent.transform, false);
    }

    private Button GetColourButton(string headerName) => 
        _root.Q<Button>(USSColourButtonHeader + headerName);
    
    private void LoadConfig()
    {
       PlayerCustomization config = saver.Load();
        
        if (config == null){
            config = JsonUtility.FromJson<PlayerCustomization>(PlayerPrefs.GetString("Customization"));
            if(config == null) return;
        }
        ;
        
        ChangeSkinColour(config.SkinColour.Split(" ")[0]);

        foreach (var item in config.ItemsEnabled) 
            ToggleCustomizedObject(item);
    }
    
    public async void SaveConfigAsync()
    {
       // if (File.Exists(CustomizationSerializeService.SavedFilePath))
     //       File.Delete(CustomizationSerializeService.SavedFilePath);
        
        var newConfig = new PlayerCustomization
        {
            SkinColour = _targetModel.GetComponent<SkinnedMeshRenderer>().material.name,
            ItemsEnabled = new List<string>()
        };
        
        var itemParent = GameObject.Find("HeadMain");

        foreach (Transform item in itemParent.transform){
            if(item.gameObject.name!="Head_tip")
                newConfig.ItemsEnabled.Add(item.gameObject.name);
        }
        PlayerPrefs.SetString("Customization", JsonUtility.ToJson(newConfig));
        if(await saver.Save(newConfig)){
            Debug.Log("Saved");
            Exit();

        }
    }
    private void Exit(){
      SceneManager.LoadScene("MainMenu");

    }
}