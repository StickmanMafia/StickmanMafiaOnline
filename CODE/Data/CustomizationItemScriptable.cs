using UnityEngine;

[CreateAssetMenu(fileName = "CustomizationItem", menuName = "Items/New Customization Item")]
public class CustomizationItemScriptable : ScriptableObject
{
    [SerializeField]
    private string _name;

    [SerializeField] 
    private GameObject _prefab;

    [SerializeField] 
    private Sprite _icon;

    [SerializeField]
    private int _price;

    [SerializeField]
    private ItemArea _itemArea;

    public string Name => _name;
    public GameObject Prefab => _prefab;
    public Sprite Icon => _icon;
    public int Price => _price;
    public ItemArea ItemArea => _itemArea;
}