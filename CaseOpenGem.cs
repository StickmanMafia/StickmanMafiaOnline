using System;
using UnityEngine;
using UnityEngine.UI;

public class CaseOpenGem : MonoBehaviour
{
    public Text text;
    public int gems;
    public CaseBehaivour caseBehaivour;
    public void CalcucatePrice(TimeSpan endTime){
        gems = (int)Math.Ceiling(endTime.TotalSeconds/360);
        text.text = gems.ToString();
    }
    public void CalcucatePrice(int endTime){
        gems = (int)Math.Ceiling(endTime/360.0);
        text.text = gems.ToString();
        caseBehaivour.gems = gems;
    }
     public void ClickedGems(){

     }
}
