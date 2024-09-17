using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    public int currentImage;
    public Image image;
    public Sprite[] spritesRu;
    public Sprite[] spritesEn;
   public void StartScroll(){
    currentImage = 0;
    Sprite[] sprites;
    if(PlayerPrefs.GetInt("language")==1){
        sprites = spritesEn;
    }
    else{
         sprites = spritesRu;
    }
    image.sprite = sprites[currentImage];
   }
   public void Scroll(){
    currentImage++;
     Sprite[] sprites;
    if(PlayerPrefs.GetInt("language")==1){
        sprites = spritesEn;
    }
    else{
         sprites = spritesRu;
    }
    if(currentImage >= sprites.Length){
        currentImage = 0;
    }
    image.sprite = sprites[currentImage];
   }
    
}
