using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public class Localization : MonoBehaviour
{
    [SerializeField]
    private LocalizedStringTable _table;

    private readonly List<LocalizeObj> _elementList = new();

    private void Awake()
    {
        var document = GetComponent<UIDocument>();
        RegisterElements(document.rootVisualElement);
    }

    private void OnEnable() => _table.TableChanged += OnTableChanged;

    private void OnDisable() => _table.TableChanged -= OnTableChanged;

    // if you change the language, the values are updated
    private void OnTableChanged(StringTable stringTable)
    {
        var tableAsync = _table.GetTableAsync();
        tableAsync.Completed -= OnTableLoaded;
        tableAsync.Completed += OnTableLoaded;
    }

    // Updates values if language changes
    private void OnTableLoaded(AsyncOperationHandle<StringTable> table)
    {
        if (table.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load string table.");
            return;
        }

        StringTable stringTable = table.Result;
        if (stringTable == null)
        {
            Debug.LogError("stringTable is null in OnTableLoaded method.");
            return;
        }

        Localize(stringTable);
    }

    // Register all elements ( recursive )
    private void RegisterElements(VisualElement element)
    {
        if (element is TextElement textElement)
        {
            var key = textElement.text;

            if (!string.IsNullOrEmpty(key) && key[0] == '#')
            {
                key = key.TrimStart('#');
                _elementList.Add(new LocalizeObj
                {
                    Element = textElement,
                    Key = key
                });
            }
        }

        // if have child
        var hierarchy = element.hierarchy;
        var children = hierarchy.childCount;

        for (var i = 0; i < children; i++)
            RegisterElements(hierarchy.ElementAt(i));
    }

    // Change text values
    private void Localize(StringTable stringTable)
    {
        if (stringTable == null)
        {
            Debug.LogError("stringTable is null in Localize method.");
            return;
        }

        foreach (var item in _elementList)
        {
            if (item.Element == null)
            {
                Debug.LogError($"Element is null for key: '{item.Key}'");
                continue;
            }

            var entry = stringTable[item.Key];

            if (entry != null)
                item.Element.text = entry.LocalizedValue;
            else
                Debug.LogWarning($"No {stringTable.LocaleIdentifier.Code} translation for key: '{item.Key}'");
        }
    }

    // Used for translation
    private class LocalizeObj
    {
        public string Key;
        public TextElement Element;
    }
}
