using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

public class tsssttt : MonoBehaviour
{
    public InputField field;
    public Recorder recorder;
    void Start()
    {
        field.onSubmit.AddListener(OnSubmit);
    }
    public void OnSubmit(string value)
    {
        recorder.InterestGroup = (byte)(int.Parse(value));
        
    }

}
