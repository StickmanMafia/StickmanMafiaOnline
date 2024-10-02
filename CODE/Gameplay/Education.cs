using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class Education : MonoBehaviour
{
    public EducationData data1;
    public EducationData data2;
    public EducationData data3;
    public EducationData data4;
    private Button[] AllButtons;
    public RectTransform circle;

    private EducationData currentData;
    private int currentStage = 0;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Education1"))
        {
            AllButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
            StartEducation(data1);
        }
        
        else if (!PlayerPrefs.HasKey("Education3"))
        {
            AllButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
            StartEducation(data3);
        }
    }
    public void StartEducationInGame(){
        if (!PlayerPrefs.HasKey("Education2"))
        {
           // AllButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
            StartEducation(data2);
        }
    }
     public void StartEducationInGame2(){
        if (!PlayerPrefs.HasKey("Education4"))
        {
           // AllButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
            StartEducation(data4);
        }
    }
    private void StartEducation(EducationData data)
    {
        currentData = data;
        currentStage = 0;
        NewStage();
    }
    public void CheckEducationPlay(){
        if(!PlayerPrefs.HasKey("Education1") ){
            NextStage();
        }
    }
    private void NewStage()
    {
        if (currentStage < currentData.points.Length)
        {
            // Перемещаем круг в текущую позицию и добавляем вращение одновременно
            circle.DOMove(currentData.points[currentStage].position, 1f)
                  .SetEase(Ease.InOutQuad);

            circle.DORotate(currentData.points[currentStage].rotation.eulerAngles, 1f, RotateMode.Fast)
                  .SetEase(Ease.Linear);

            if(currentData.ButtonsActive.Count()>currentStage){
                foreach (var button in AllButtons)
            {
                button.interactable = false;
            }
            currentData.ButtonsActive[currentStage].interactable = true;
            }
            
            

            if (currentStage > 0)
            {
          //      currentData.texts[currentStage - 1].DOScale(0, 1f);
            }
          //  currentData.texts[currentStage].DOScale(1, 1f);

            currentStage++;
        }
        else
        {
            EndEducation();
        }
    }

    public void EndEducation()
    {
        if (currentData.id == data1.id)
        {
            PlayerPrefs.SetInt("Education1", 1);
        }
        else if (currentData.id == data2.id)
        {
            PlayerPrefs.SetInt("Education2", 1);
        }
        else if (currentData.id == data3.id)
        {
            PlayerPrefs.SetInt("Education3", 1);
        }
        else if (currentData.id == data4.id)
        {
            PlayerPrefs.SetInt("Education4", 1);
        }
        foreach (var button in AllButtons)
        {
            button.interactable = true;
        }
        circle.GetComponent<arrowAnim>().enabled = false;
        circle.DOKill();
        circle.DOScale(0,0.5f);
    }

    public void NextStage()
    {
        NewStage();
    }
}

[System.Serializable]
public struct EducationData
{
    public string id;
    public RectTransform[] points;
    public Button[] ButtonsActive;
   // public Transform[] texts;
}
