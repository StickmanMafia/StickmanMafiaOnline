using System.Diagnostics;
using UnityEngine;

public class MainCameraController : Controller
{
    private GameObject _table;
    private GameObject _targetPlayer;
    private GameObject _deathTarget;
    private GameObject _transformTarget;
    private GameObject _cageTarget;
    private GameObject _nightRevealTarget;
    private CameraTask _cameraTask;

    private Stopwatch _deathSiteStopwatch;

    // Rotate constants
    private const float RotateSpeedIncrement = 1f;
    private const float WideAngleCameraFov = 105;
    
    // Death animation constants
    private const float MaxDeathAnimationTime = 2200;
    private const float DeathTargetPositionOffset = 3;
    private const float CameraRotationIncrement = -0.8f;
    
    protected override void Start()
    {
        base.Start();
        FetchPlayers();
        _cameraTask = CameraTask.FollowBoat;
        _deathSiteStopwatch = new Stopwatch();
        StartFollowingBoat();
    }
    
    public void StartFollowingBoat()
    {
        _cameraTask = CameraTask.FollowBoat;
        
        var cameraContainer = GameObject.Find("CameraController");
        cameraContainer.GetComponent<Animator>().enabled = true;
        
        foreach (Transform child in cameraContainer.transform) 
            child.gameObject.GetComponent<Camera>().enabled = true;
        
        cameraContainer.GetComponent<Animator>().Play("CameraContainer");
        
        GetComponent<Camera>().enabled = false;
    }
    
    public void DisableCameraContainer()
    {
        var cameraContainer = transform.parent.Find("CameraController");

        cameraContainer.GetComponent<Animator>().enabled = false;
        
        foreach (Transform child in cameraContainer) 
            child.gameObject.GetComponent<Camera>().enabled = false;
        
        GetComponent<Camera>().enabled = true;
    }
    
    public void StartShowingDeath(GameObject target)
    {
        _deathTarget = target;
        _cameraTask = CameraTask.FocusDeath;
        
        transform.position = GetCamPositionForDeathSite();
        transform.LookAt(_deathTarget.transform);
        
        _deathSiteStopwatch.Start();
        
        DisableCameraContainer();
    }
    
    public void StartShowingTransformation(GameObject target)
    {
        _transformTarget = target;
        _cameraTask = CameraTask.FocusTransformation;
        transform.position = GetCamPositionForTransformSite();
        transform.LookAt(_transformTarget.transform);
        transform.Rotate(-20, 0, 0);
        GetComponent<Camera>().fieldOfView = WideAngleCameraFov;
        
        DisableCameraContainer();
    }
    
    public void StartShowingCage(GameObject target)
    {
        _cageTarget = target;
        _cameraTask = CameraTask.FocusCage;
        transform.position = GetCamPositionForCageSite();
        GetComponent<Camera>().fieldOfView = WideAngleCameraFov;
        
        DisableCameraContainer();
    } 
    
    public void StartShowingGreetingReveal(GameObject target)
    {
        _nightRevealTarget = target;
        _cameraTask = CameraTask.FocusGreetingReveal;
        transform.position = GetCamPositionForNightRevealSite();
        GetComponent<Camera>().fieldOfView = WideAngleCameraFov;
        
        DisableCameraContainer();
    }

    private void FixedUpdate()
    {
        switch (_cameraTask)
        {
            case CameraTask.FocusDeath:
            {
                transform.Rotate(0, CameraRotationIncrement, 0, 0);
                
                if (_deathSiteStopwatch.Elapsed.TotalMilliseconds >= MaxDeathAnimationTime)
                {
                    _cameraTask = CameraTask.FollowBoat;
                    _deathSiteStopwatch.Stop();
                    _deathSiteStopwatch.Reset();
                    _deathTarget = null;
                    StartFollowingBoat();
                }
                
                break;
            }
            
            case CameraTask.FocusTransformation:
            {
                PlayerAnimationEvents PlayerEvents = _transformTarget.GetComponent<PlayerAnimationEvents>();
                if(!_transformTarget) PlayerEvents.OnWeepCageAnimationOver(); 
                else RotateToRight(_transformTarget, Time.deltaTime * (RotateSpeedIncrement / 3));
                break;
                
            }
            
            case CameraTask.FocusGreetingReveal:
            {
                PlayerAnimationEvents PlayerEvents = _nightRevealTarget.GetComponent<PlayerAnimationEvents>();
                if(!_nightRevealTarget) PlayerEvents.OnWeepCageAnimationOver(); 
                else RotateToRight(_nightRevealTarget, Time.deltaTime * (RotateSpeedIncrement / 3));
                break;
                
            }
            
            case CameraTask.FocusCage:
            {
                PlayerAnimationEvents PlayerEvents = _cageTarget.GetComponent<PlayerAnimationEvents>();
                if(!_cageTarget) PlayerEvents.OnWeepCageAnimationOver(); 
                else RotateToRight(_cageTarget, Time.deltaTime * (RotateSpeedIncrement / 3));
                break;
            }
        }
    }
    
    // Initial positions for various camera task
    // starting methods.
    private Vector3 GetCamPositionForDeathSite()
    {
        var deathPosition = _deathTarget.transform.position;
        return deathPosition + _deathTarget.transform.right * DeathTargetPositionOffset 
                             + _deathTarget.transform.up * DeathTargetPositionOffset;
    }
    
    private Vector3 GetCamPositionForTransformSite()
    {
        var transformPosition = _transformTarget.transform.position;
        return transformPosition + _transformTarget.transform.forward * 2f 
                                 + _transformTarget.transform.up * 2.6f
                                 + _transformTarget.transform.right * 1.1f;
    }
    
    private Vector3 GetCamPositionForNightRevealSite()
    {
        var transformPosition = _nightRevealTarget.transform.position;
        return transformPosition + _nightRevealTarget.transform.forward * 2f 
                                 + _nightRevealTarget.transform.up * 2.5f
                                 + _nightRevealTarget.transform.right * 1.1f;
    }
    
    private Vector3 GetCamPositionForCageSite()
    {
        var transformPosition = _cageTarget.transform.position;
        return transformPosition + _cageTarget.transform.forward * 3f 
                                 + _cageTarget.transform.up * 1.8f
                                 + _cageTarget.transform.right * 1.2f;
    }
    
    private void RotateToRight(GameObject target, float speedIncrement)
    {
        transform.LookAt(target.transform);
        transform.Translate(Vector3.right * speedIncrement);
    }
}