using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour
{
    public Image lowImage;
    public Image mediumImage;
    public Image highImage;

    private int graphicsIndex;

    void Start()
    {
        // Загружаем сохраненные настройки графики
        graphicsIndex = PlayerPrefs.GetInt("GraphicsIndex", 1);

        // Устанавливаем текущие настройки графики
        QualitySettings.SetQualityLevel(graphicsIndex);

        // Устанавливаем цвет изображений в соответствии с текущими настройками
        SetImageColor(lowImage, graphicsIndex == 0);
        SetImageColor(mediumImage, graphicsIndex == 1);
        SetImageColor(highImage, graphicsIndex == 2);
    }

    public void SetGraphicsQuality(int index)
    {
        // Устанавливаем новые настройки графики
        QualitySettings.SetQualityLevel(index);

        // Сохраняем выбранные настройки графики
        PlayerPrefs.SetInt("GraphicsIndex", index);

        // Обновляем цвет изображений в соответствии с текущими настройками
        SetImageColor(lowImage, index == 0);
        SetImageColor(mediumImage, index == 1);
        SetImageColor(highImage, index == 2);
    }

    private void SetImageColor(Image image, bool isSelected)
    {
        image.color = isSelected ? Color.green : Color.white;
    }
}
