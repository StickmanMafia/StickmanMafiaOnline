using UnityEngine.UIElements;

public abstract class UIController : Controller
{
    protected static void EnableElement(VisualElement element) => element.style.display = DisplayStyle.Flex;
    
    protected static void DisableElement(VisualElement element) => element.style.display = DisplayStyle.None;
    
    protected static void SetVisibility(bool visible, VisualElement element)
    {
        if (visible)
            EnableElement(element);
        else 
            DisableElement(element);
    }
}
