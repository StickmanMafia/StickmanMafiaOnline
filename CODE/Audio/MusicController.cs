using UnityEngine;

public class MusicController : AudioController
{
    [SerializeField] 
    private AudioClip _clip1;
    
    [SerializeField]
    private AudioClip _clip2;
    
    [SerializeField]
    private AudioClip _clip3;
    
    [SerializeField]
    private AudioClip _clip4;

    protected override void Awake()
    {
        base.Awake();
        AudioSource.loop = true;
    }

    public void PlayClip1() => PlayClip(_clip1);

    public void PlayClip2() => PlayClip(_clip2);
    
    public void PlayClip3() => PlayClip(_clip3);
    
    public void PlayClip4() => PlayClip(_clip4);
}