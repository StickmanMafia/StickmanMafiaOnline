using System.Drawing;
using UnityEngine;

public class CaseBehaivour : MonoBehaviour
{
    public string id;
    public timer MyTimer;
    public bool CanOpen;
    public int gems;
   public void StartSharmanka()
{
    MyTimer.caseId = id;
    StartCoroutine(MyTimer.StartSharmankaCoroutine());

    
}
public void CheckMe(){
    MyTimer.Check();
}
public void Opened(){
    if(PlayerPrefs.GetString("OpeningCaseId")==id){
        PlayerPrefs.DeleteKey("OpeningCaseId");
    }
    
}
public void LoadMe(){
    MyTimer.caseId = id;
    MyTimer.LoadMe();

}
public void GemsSell(){
    MyTimer.gemsOpen.ClickedGems();
}
}
