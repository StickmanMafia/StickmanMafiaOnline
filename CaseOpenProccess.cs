using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Numerics;

public class CaseOpenProccess : MonoBehaviour
{
    public CanvasGroup group;
    public GameObject chest;
    public GameObject coinsPrefab;
    public GameObject gemsPrefab;
    public GameObject skinPrefab;
    public GameObject perkPrefab;
    public bool chestOpened = false;
    public Transform spawnPoint;

    private int gold;
    private int gems;
    private string skin;
    private string perk;
    public Image caseImage;
    public Sprite[] sprites;
    public void StartProcess(int rare,NewItems newItems)
    {
        gold = newItems.Gold;
        gems = newItems.Gems;
        skin = newItems.Skin;
        perk = newItems.Perk;
        caseImage.sprite = sprites[rare-1];
        UnityEngine.Vector3 startScale = new UnityEngine.Vector3(3.4395f,3.4395f,3.4395f);
        chest.transform.localScale = startScale;
        group.blocksRaycasts = true;
        chestOpened = false;
        chest.SetActive(true);
        group.DOFade(1,0.5f);
        
    }
    public void Cicked(){
        if(!chestOpened){
            
            StartCoroutine(OpenChest());
            
        }
        else{
            group.DOFade(0,0.5f);
            group.blocksRaycasts = false;
            foreach(Transform child in spawnPoint){
                Destroy(child.gameObject);
            }
        }
    }

    private IEnumerator OpenChest()
    {
        
        chestOpened = true;
        UnityEngine.Vector3 startScale = new UnityEngine.Vector3(3.4395f,3.4395f,3.4395f);
        UnityEngine.Vector3 endScale = startScale * 1.5f; 

        chest.transform.DOScale(endScale, 0.4f).OnComplete(()=>chest.transform.DOScale(0,0.5f).OnComplete(()=>chest.SetActive(false)));

        
        

        if (gold > 0)
        {
            
                GameObject money= Instantiate(coinsPrefab, spawnPoint);
                money.transform.GetChild(0).transform.GetComponent<Text>().text =  gold.ToString();
                yield return new WaitForSeconds(0.1f); 
            
        }

        if (gems > 0)
        {
            
                 GameObject gem=Instantiate(gemsPrefab, spawnPoint);
                  gem.transform.GetChild(0).transform.GetComponent<Text>().text =  gems.ToString();
                yield return new WaitForSeconds(0.1f); 
            
        }

        if (!string.IsNullOrEmpty(skin))
        {
            GameObject ourskin=Instantiate(skinPrefab, spawnPoint);
            CheckSkin(ourskin.GetComponent<Image>(),skin);

            yield return new WaitForSeconds(0.1f); 
        }

        if (!string.IsNullOrEmpty(perk))
        {
             GameObject outperk =Instantiate(perkPrefab, spawnPoint);
             CheckPerk(outperk.GetComponent<Image>(),perk);
            yield return new WaitForSeconds(0.1f); 
        }
        Debug.Log(chestOpened);
        
    }
    public void CheckSkin(Image skinimg,string name){
        Sprite skin = Resources.Load<Sprite>("Skins/"+name);
        skinimg.sprite = skin;
       
    }
    public void CheckPerk(Image perkimg,string name){
         Sprite perk = Resources.Load<Sprite>("Perks/"+name);
         perkimg.sprite = perk;
    }
}
