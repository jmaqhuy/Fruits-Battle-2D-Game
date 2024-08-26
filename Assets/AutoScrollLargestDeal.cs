using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AutoScrollLargestDeal : MonoBehaviour
{
    public ScrollRect scrollRect;  // Reference to the ScrollRect component.
    public float delayTime = 2.0f;  // Time between scrolls.

    private float elapsedTime = 0f;  // Time elapsed since the last scroll.
    private int totalImages = 3;  // Total number of images in the content.
    private int currentIndex = 0;  // Current index of the image being displayed.

    void Update()
    {
        // Increase the elapsed time by the time since the last frame.
        elapsedTime += Time.deltaTime;

        // If the elapsed time exceeds the delay time, scroll to the next image.
        if (elapsedTime >= delayTime)
        {
            // Reset the elapsed time.
            elapsedTime = 0f;

            // Move to the next image.
            currentIndex++;

            // If the current index exceeds the number of images, reset to the first image.
            if (currentIndex >= totalImages)
            {
                currentIndex = 0;
            }

            // Calculate the target position based on the current index.
            float targetPosition = (float)currentIndex / (totalImages - 1);

            // Smoothly move the scrollRect's horizontal position to the target position.
            StartCoroutine(SmoothScroll(targetPosition));
        }
    }

    private IEnumerator SmoothScroll(float targetPosition)
    {
        float startTime = Time.time;
        float duration = 1.0f;  // Duration of the scroll.
        float startPosition = scrollRect.horizontalNormalizedPosition;

        while (Time.time - startTime < duration)
        {
            // Lerp the scroll position from the start position to the target position.
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startPosition, targetPosition, (Time.time - startTime) / duration);
            yield return null;
        }

        // Ensure the final position is set exactly to the target position.
        scrollRect.horizontalNormalizedPosition = targetPosition;
    }
}
