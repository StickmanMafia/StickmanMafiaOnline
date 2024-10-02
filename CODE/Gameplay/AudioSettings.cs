using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioSettings : MonoBehaviour
{
    public Image audioIcon;
    public Sprite[] audioModes;
    public Slider sliderAudio;
    public Slider sliderMusic;
    public AudioMixer ourMixer;
    private void Start() {
     
        if(sliderAudio!=null) {sliderAudio.value = PlayerPrefs.GetFloat("audio");
      sliderAudio.onValueChanged.AddListener(delegate { AudioSet(sliderAudio.value); });
      CheckIcons(PlayerPrefs.GetFloat("audio"));
      AudioMixerSetValue();}
      if(sliderMusic!=null) {sliderMusic.value = PlayerPrefs.GetFloat("music");
      sliderMusic.onValueChanged.AddListener(delegate { MusicSet(sliderMusic.value); });
      CheckIcons(PlayerPrefs.GetFloat("music"));
      MusicMixerSetValue();}
      
        
      
      

    }
  public void AudioSet(float value){
    CheckIcons(value);
    PlayerPrefs.SetFloat("audio",value);
    AudioMixerSetValue();

    }
    public void MusicSet(float value){
    CheckIcons(value);
    PlayerPrefs.SetFloat("music",value);
    MusicMixerSetValue();

    }
    private void AudioMixerSetValue(){
      ourMixer.SetFloat("Param", Mathf.Log10(PlayerPrefs.GetFloat("audio")) * 26);
    }
    private void MusicMixerSetValue(){
      ourMixer.SetFloat("Param", Mathf.Log10(PlayerPrefs.GetFloat("music")) * 26);
    }
    private void CheckIcons(float value){
      if(value==0.001f){
      
      audioIcon.sprite = audioModes[0];
    }
    else{
      audioIcon.sprite = audioModes[1];
    }
    }
}