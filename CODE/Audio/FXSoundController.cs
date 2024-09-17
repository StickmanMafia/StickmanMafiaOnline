using UnityEngine;

public class FXSoundController : AudioController
{
    [SerializeField] 
    private AudioClip _splash;
    
    [SerializeField] 
    private AudioClip _cry;
    
    [SerializeField] 
    private AudioClip _happyMusic;
    
    [SerializeField] 
    private AudioClip _sadMusic;
    
    [SerializeField] 
    private AudioClip _deathScream;

    [SerializeField]
    private AudioClip _perkSound;

    [SerializeField]
    private AudioClip _heartBeatSound;

    [SerializeField]
    private AudioClip _notificationWarningSound;

    public void PlaySplash() => PlayClip(_splash);
    
    public void PlayCry() => PlayClip(_cry);

    public void PlayHappyMusic() => PlayClip(_happyMusic);
    
    public void PlaySadMusic() => PlayClip(_sadMusic);
    
    public void PlayDeathScream() => PlayClip(_deathScream);

    public void PlayPerkSound() => PlayClip(_perkSound);

    public void PlayHeartBeatSound() => PlayClip(_heartBeatSound);
   
    public void PlayNotificationWarningSound() => PlayClip(_notificationWarningSound);
}