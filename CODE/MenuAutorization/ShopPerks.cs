using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShopPerks : MonoBehaviour
{
    public User currentUser;
    public managerMenu menu;
    public DataBaseManager databaseManager;
    public BetMoney betMoney;

    

    void SaveUserData()
    {
        databaseManager.SaveDataURL(currentUser);
    }
    
    public void BuyPerk(string perkInfo)
    {
        string[] parts = perkInfo.Split(' ');
        if (parts.Length != 2)
        {
            Debug.LogError("Invalid perk info: " + perkInfo);
            return;
        }

        string perkName = parts[0];
        int cost;
        if (!int.TryParse(parts[1], out cost))
        {
            Debug.LogError("Invalid cost: " + parts[1]);
            return;
        }

        if (currentUser.money >= cost)
        {
            currentUser.money -= cost;

            switch (perkName)
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
                    Debug.LogError("Invalid perk name: " + perkName);
                    return;
            }
            UpdatePerkAmount(perkName);
            menu.currentUser = currentUser;
            menu.CheckMoney();
            SaveUserData();
        }
        else
        {
            Blink(ChildByName(ChildByName(transform, perkName), "Cost").gameObject.GetComponent<Text>());
        }
    }

    public Transform ChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(name))
            {
                return child;
            }
        }
        return null;
    }

    public void Blink(Text text)
    {
        StartCoroutine(BlinkCoroutine(text));
    }

    private IEnumerator BlinkCoroutine(Text text)
    {
        text.DOColor(Color.red, 0.1f);
        yield return new WaitForSeconds(0.1f);

        text.DOColor(Color.green, 0.1f);
        yield return new WaitForSeconds(0.1f);

        text.DOColor(Color.red, 0.1f);
        yield return new WaitForSeconds(0.1f);

        text.DOColor(Color.green, 0.1f);
    }

    public void LoadPerks()
    {
        UpdatePerkAmount("ActiveRole");
        UpdatePerkAmount("Wiretapping");
        UpdatePerkAmount("LieDetector");
        UpdatePerkAmount("Revenge");
        UpdatePerkAmount("Radio");
        UpdatePerkAmount("DoubleVoice");
        UpdatePerkAmount("Disguise");
        UpdatePerkAmount("Helicopter");
        UpdatePerkAmount("MineDetector");
        UpdatePerkAmount("Mine");
    }

    private void UpdatePerkAmount(string perkName)
    {
        int amount = 0;
        switch (perkName)
        {
            case "ActiveRole":
                amount = currentUser.perks.ActiveRoleCount;
                break;
            case "Wiretapping":
                amount = currentUser.perks.WiretappingCount;
                break;
            case "LieDetector":
                amount = currentUser.perks.LieDetectorCount;
                break;
            case "Revenge":
                amount = currentUser.perks.RevengeCount;
                break;
            case "Radio":
                amount = currentUser.perks.RadioCount;
                break;
            case "DoubleVoice":
                amount = currentUser.perks.DoubleVoiceCount;
                break;
            case "Disguise":
                amount = currentUser.perks.DisguiseCount;
                break;
            case "Helicopter":
                amount = currentUser.perks.HelicopterCount;
                break;
            case "MineDetector":
                amount = currentUser.perks.MineDetectorCount;
                break;
            case "Mine":
                amount = currentUser.perks.MineCount;
                break;
            default:
                Debug.LogError("Invalid perk name: " + perkName);
                return;
        }

        Transform amountTextTransform = ChildByName(ChildByName(transform, perkName), "amount");

        if (amountTextTransform != null)
        {
            amountTextTransform.gameObject.GetComponent<Text>().text = amount.ToString();
        }
    }
}
