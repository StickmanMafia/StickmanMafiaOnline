using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CaseLogic : MonoBehaviour
{
    public List<CaseUISave> caseUIs = new List<CaseUISave>();
    public Transform[] points;
    public Sprite[] cases;
    private Game game;
    public DataBaseManager databaseManager;
    public managerMenu menu;
    public CaseOpenProccess OpenProcess;
    public Color ColorA = Color.red; // Цвет, когда кейс не открывается
    public Color ColorB = Color.yellow; // Цвет, когда кейс открывается
    public Color ColorC = Color.green; // Цвет, когда кейс открыт
    public User currentUser;
    public bool YesNo = false;

    public void LoadPlayer()
    {

    }

    void Start()
    {
        game = new Game();
        LoadCases();
        if(PlayerPrefs.GetInt("CaseRecieve",0)==1){
            PlayerPrefs.SetInt("CaseRecieve",0);
            SpawnRandomCase();

        }

    }

    [Button("спавним кейс рандомный")]
    public void SpawnRandomCase()
    {
        GameCaseClass lol = game.GiveCase();
        SpawnCase(lol);
    }

    public void LoadCases()
    {
        string casesJson = PlayerPrefs.GetString("Cases", "");
        if (!string.IsNullOrEmpty(casesJson))
        {
            Wrapper wrapper = JsonUtility.FromJson<Wrapper>(casesJson);
            for (int i = 0; i < wrapper.cases.Count; i++)
            {
                GameCaseClass gameCase = game.cases[wrapper.cases[i].rarity-1];
                CaseUI caseUI = new CaseUI(gameCase, cases[gameCase.Rarity - 1], wrapper.cases[i].pos, this, wrapper.cases[i].id);
                caseUIs.Add(caseUI.saveUI);
            }
        }
    }

    public void SaveCases()
    {
        List<CaseUISave> jsonStrings = new List<CaseUISave>();
        for (int i = 0; i < caseUIs.Count; i++)
        {
            CaseUISave newClass = caseUIs[i];
            CaseUISave jsonString = newClass;
            jsonStrings.Add(jsonString);
        }
        string casesJson = JsonUtility.ToJson(new Wrapper { cases = jsonStrings });
        PlayerPrefs.SetString("Cases", casesJson);
    }

    [Button("спавним кейс")]
    public void SpawnCase(int Rarity)
    {
        GameCaseClass yuo = game.GiveCaseByRarity(Rarity);
        SpawnCase(yuo);
    }

    public void SpawnCase(GameCaseClass gameCase)
    {
        if (caseUIs.Count < points.Length)
        {
            CaseUI caseUI = new CaseUI(gameCase, cases[gameCase.Rarity - 1], CheckFreeSpace(), this);
            caseUIs.Add(caseUI.saveUI);
            CheckIdAll();
        }
        else
        {
            Debug.Log("No more space to spawn cases");
        }
    }

    public int CheckFreeSpace()
    {
        if (caseUIs.Count < points.Length)
        {
            List<int> Free = new List<int>();
            foreach (CaseUISave caseUI in caseUIs)
            {
                Free.Add(caseUI.pos);
            }
            for (int i = 0; i < points.Length; i++)
            {
                if (!Free.Contains(i))
                {
                    return i;
                }
            }
            return -1;
        }
        else
        {
            return -1;
        }
    }

    public void BuyCaseByGems(CaseBehaivour behaivour)
    {
        if (!YesNo)
        {
            YesNo = true;
            GameObject BuyMenu = Resources.Load<GameObject>("GemsYesNo");
            GameObject OurGemsMenu = Instantiate(BuyMenu, transform);
            OurGemsMenu.transform.localScale = Vector3.zero;
            OurGemsMenu.transform.DOScale(1, 0.5f);
            OurGemsMenu.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
            {
                GemsReturn(true, OurGemsMenu, behaivour.gems, behaivour);
            });
            OurGemsMenu.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
            {
                GemsReturn(false, OurGemsMenu, behaivour.gems, behaivour);
            });
        }
    }

    public void GemsReturn(bool Yes, GameObject menu, int gems, CaseBehaivour behaivour)
    {
        if (Yes)
        {
             if(currentUser.gems >= gems){
                BuyByGemsFinish(gems, behaivour);
             }

            menu.transform.DOScale(0, 0.3f).OnComplete(() => Destroy(menu));
            YesNo = false;
        }
        else
        {
            menu.transform.DOScale(0, 0.3f).OnComplete(() => Destroy(menu));
            YesNo = false;
        }
    }

    public void BuyByGemsFinish(int gems, CaseBehaivour behaviour)
    {

        currentUser.gems -= gems;
        databaseManager.SaveDataURL(currentUser);
        menu.currentUser = currentUser;
        menu.LoadPlayer();
        PlayerPrefs.SetString("EndTime_" + behaviour.id, DateTime.Now.ToString());
        behaviour.MyTimer.endTime = DateTime.Now;
        behaviour.CanOpen = true;

    }

    public void StartOpeningCase(CaseBehaivour behaivour)
    {
        if (!PlayerPrefs.HasKey("OpeningCaseId"))
        {
            PlayerPrefs.SetString("OpeningCaseId", behaivour.id);
            behaivour.StartSharmanka();
        }
        else
        {
            BuyCaseByGems(behaivour);
        }
    }

    public void CheckIdAll()
    {
        for (int i = 0; i < caseUIs.Count; i++)
        {

        }
        SaveCases();
    }

    public void Clicked(CaseBehaivour behaivour)
    {
        if (!behaivour.CanOpen)
        {
            StartOpeningCase(behaivour);
        }
        else
        {
            OpenCaseCoroutine(behaivour);
        }
    }

    private void OpenCaseCoroutine(CaseBehaivour behaivour)
    {
        int pos = caseUIs.FindIndex(x => x.id == behaivour.id);
        NewItems newItems = game.GenerateCaseDrop(caseUIs[pos].rarity);
        OpenProcess.StartProcess(caseUIs[pos].rarity, newItems);
        Debug.Log("Gold: " + newItems.Gold);
        Debug.Log("Gems: " + newItems.Gems);
        Debug.Log("Skin: " + newItems.Skin);
        Debug.Log("Perk: " + newItems.Perk);
        currentUser.money += newItems.Gold;
        menu.
        currentUser.gems += newItems.Gems;
        menu.currentUser = currentUser;
        SavePerk(newItems.Perk);
        behaivour.MyTimer.UpdateParentParentImageColor(behaivour.MyTimer.ColorA);
        caseUIs.RemoveAt(pos);
        PlayerPrefs.DeleteKey("OpeningCaseId");
        PlayerPrefs.DeleteKey("Time_" + behaivour.id);
        PlayerPrefs.DeleteKey("EndTime_" + behaivour.id);
        Destroy(behaivour.gameObject);
        SaveCases();
        menu.LoadPlayer();
        databaseManager.SaveDataURL(currentUser);
    }
    private void SavePerk(string perk){
        switch (perk)
            {
                case "ActiveRole":
                    currentUser.perks.ActiveRoleCount++;
                    break;
                case "Wiretapping":
                    currentUser.perks.WiretappingCount++;
                    break;
                case "LieDetector":
                    currentUser.perks.LieDetectorCount++;
                    break;
                case "Revenge":
                    currentUser.perks.RevengeCount++;
                    break;
                case "Radio":
                    currentUser.perks.RadioCount++;
                    break;
                case "DoubleVoice":
                    currentUser.perks.DoubleVoiceCount++;
                    break;
                case "Disguise":
                    currentUser.perks.DisguiseCount++;
                    break;
                case "Helicopter":
                    currentUser.perks.HelicopterCount++;
                    break;
                case "MineDetector":
                    currentUser.perks.MineDetectorCount++;
                    break;
                case "Mine":
                    currentUser.perks.MineCount++;
                    break;
                default:
                    return;
            }

            menu.currentUser = currentUser;
            menu.perks.currentUser = currentUser;
            menu.perks.LoadPerks();
    }

}

[System.Serializable]
public class Wrapper
{
    public List<CaseUISave> cases;
}
[System.Serializable]
public struct CaseUISave{
    public int rarity;
    public string id;
    public int pos;
}
[System.Serializable]
public class CaseUI : MonoBehaviour
{
    public CaseUISave saveUI;
    public Image image;
    public Button button;
    public Text timerText;
    private CaseLogic caseLogic;

    public CaseUI(GameCaseClass gameCase, Sprite sprite, int point, CaseLogic caseLogic)
    {
        saveUI.pos = point;
        this.caseLogic = caseLogic;
        GameObject casePrefab = Resources.Load<GameObject>("CaseBrefab");
        if (casePrefab != null)
        {
            GameObject casik = Instantiate(casePrefab, caseLogic.points[point]);
            casik.name = DateTime.Now.ToString(@"hh\:mm\:ss");
            image = casik.GetComponent<Image>();
            image.sprite = sprite;
            image.transform.position = caseLogic.points[point].position;
            button = image.gameObject.AddComponent<Button>();
            CaseBehaivour behaivour = casik.GetComponent<CaseBehaivour>();
            button.onClick.AddListener(() => { caseLogic.Clicked(behaivour); });
            saveUI.id = Guid.NewGuid().ToString();
            saveUI.pos = point;
            behaivour.id = saveUI.id;
            saveUI.rarity = gameCase.Rarity;
            if (!PlayerPrefs.HasKey("Time_" + saveUI.id))
            {
                PlayerPrefs.SetString("Time_" + saveUI.id, gameCase.OpeningTime.ToString());
                Debug.Log("тайм для " + saveUI.id);
                behaivour.LoadMe();
            }
        }
    }

    public CaseUI(GameCaseClass gameCase, Sprite sprite, int point, CaseLogic caseLogic, string idOur)
    {

        saveUI.rarity = gameCase.Rarity;
        this.caseLogic = caseLogic;
        GameObject casePrefab = Resources.Load<GameObject>("CaseBrefab");
        if (casePrefab != null)
        {
            GameObject casik = Instantiate(casePrefab, caseLogic.points[point]);
            casik.name = DateTime.Now.ToString(@"hh\:mm\:ss");
            image = casik.GetComponent<Image>();
            image.sprite = sprite;
            image.transform.position = caseLogic.points[point].position;
            button = image.gameObject.AddComponent<Button>();
            CaseBehaivour behaivour = casik.GetComponent<CaseBehaivour>();
            saveUI.pos = point;
            button.onClick.AddListener(() => { caseLogic.Clicked(behaivour); });
            saveUI.id = idOur;
            behaivour.id = saveUI.id;
            if (PlayerPrefs.GetString("OpeningCaseId") == idOur)
            {
                behaivour.StartSharmanka();
            }
            else
            {
                behaivour.LoadMe();
            }
        }
    }
}
