using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class managerCustomization : MonoBehaviour
{
    // Start is called before the first frame update
    public CustomizerLogic logic;
    public void SaveConfig(){
        logic.SaveConfigAsync();
        
    }
    public void ChangeSkin(string skinName){
       logic.ChangeSkinColour(skinName);
   }
   public void ChangeHat(string hatName){
       CustomizerLogic.ToggleCustomizedObject(hatName);
   }
}
