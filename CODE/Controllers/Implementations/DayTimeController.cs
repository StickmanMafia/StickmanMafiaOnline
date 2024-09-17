using UnityEngine;

public class DayTimeController : Controller
{
    [SerializeField]
    private GameObject _directionalLightObject;

    [SerializeField]
    private GameObject _gameplayControllerObject;

    [SerializeField]
    private Material _dayMaterial;
    
    [SerializeField]
    private Material _nightMaterial;

    private bool _nightSwitchPrompted;
    private bool _daySwitchPrompted;

    private Quaternion _dayRotation;
    private Quaternion _nightRotation;

    private GameplayController _gameplayController;
    private MusicController _musicController;
    private GameplayUIBehaviour _uiScript;
    
    public TimeOfDay CurrentDayTime { get; private set; }
    public int NightPassed { get; private set; }
    public int DaysPassed { get; private set; }
    
    private Quaternion Rotation
    {
        get => _directionalLightObject.transform.rotation;
        set => _directionalLightObject.transform.rotation = value;
    } 
    
    protected override void Start()
    {
        base.Start();   
        NightPassed = 0;
        DaysPassed = 0;
        CurrentDayTime = TimeOfDay.Day;
        _gameplayController = _gameplayControllerObject.GetComponent<GameplayController>();
        _musicController = GameObject.Find("MusicController").GetComponent<MusicController>();
        _dayRotation = new Quaternion(155.05f, 2009.509f, 272.328f, 0);
        _nightRotation = new Quaternion(267.4f, 2009.509f, 272.328f, 0);
        _uiScript = GameObject.Find("ui-document").GetComponent<GameplayUIBehaviour>();
        // AdjustFog(new Color(128, 184, 229, 255), 4.7f, 193.42f);
        _nightSwitchPrompted = false;
        _nightSwitchPrompted = false;
    }
    
    public void StartSwitchToDay()
    {
        if (CurrentDayTime == TimeOfDay.Day)
            return;
        
        RenderSettings.skybox = _dayMaterial;
        _daySwitchPrompted = true;
    }
    
    public void StartSwitchToNight()
    {
        if (CurrentDayTime == TimeOfDay.Night)
            return;
        
        RenderSettings.skybox = _nightMaterial;
        _nightSwitchPrompted = true;
    }

    private void Update()
    {
        if (_daySwitchPrompted)
            SwitchToDay();
        else if (_nightSwitchPrompted) 
            SwitchToNight();
    }
    
    private void SwitchToNight()
    {
        DaysPassed++;
        Rotation = _nightRotation;
        _nightSwitchPrompted = false;
        CurrentDayTime = TimeOfDay.Night;
        
        _musicController.PlayClip3();
        // AdjustFog(new Color(45, 73, 120, 255), -39.2f, 35.98f);
            
        _gameplayController.InitializeActionQueue();
    }
    
    private void SwitchToDay()
    {
        NightPassed++;
        Rotation = _dayRotation;
        CurrentDayTime = TimeOfDay.Day;
        _daySwitchPrompted = false;
        // AdjustFog(new Color(128, 184, 229, 255), 4.7f, 193.42f);

        _uiScript.HideWarning();
        _musicController.PlayClip2();
    }
    
    private static void AdjustFog(Color color, float start, float end)
    {
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = color;
        RenderSettings.fogStartDistance = start;
        RenderSettings.fogEndDistance = end;
    }
}