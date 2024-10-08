using UnityEngine.UI;
using Photon.Voice.Unity;
using UnityEngine;
using Photon.Realtime;

public class Pushtalker : MonoBehaviour
{
    public bool CanTalk;
    public bool Talking;
    public Recorder recorder;
    public Image img;
    public Color[] audioModes; // 0 = off, 1 = on
    public GameObject[] pushText;// 0 = off, 1 = on

    private void Start() {
        CanTalk= true;
        recorder.TransmitEnabled = Talking;

    }
    public void Push(){
        if (CanTalk)
        {
                Talking = !Talking;
                recorder.TransmitEnabled = Talking;
                img.color = audioModes[Talking ? 1 : 0];
                pushText[Talking ? 1 : 0].SetActive(true);
                pushText[!Talking ? 1 : 0].SetActive(false);
                
            
        }

    }
}
