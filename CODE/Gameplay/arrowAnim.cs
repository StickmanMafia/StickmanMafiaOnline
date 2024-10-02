using UnityEngine;
using DG.Tweening;

public class arrowAnim: MonoBehaviour
{
    public float scaleDuration = 0.5f; // Длительность анимации увеличения и уменьшения
    public float scaleFactor = 1.2f; // Коэффициент увеличения

    private Vector3 originalScale;
    

    void Start()
    {
        // Сохраняем оригинальный масштаб объекта
        originalScale = transform.localScale;

        // Запускаем бесконечную последовательность анимаций
        StartInfiniteScaleAnimation();
    }

    void StartInfiniteScaleAnimation()
    {
        // Увеличиваем объект
        transform.DOScale(originalScale * scaleFactor, scaleDuration)
            .OnComplete(() =>
            {
                // Уменьшаем объект обратно до оригинального размера
                transform.DOScale(originalScale, scaleDuration)
                    .OnComplete(() =>
                    {
                        // Запускаем анимацию снова
                        StartInfiniteScaleAnimation();
                    });
            });
    }
}
