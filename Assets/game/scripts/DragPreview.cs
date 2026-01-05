// DragPreview.cs// DragPreview.cs
using UnityEngine;
using UnityEngine.UI;

public class DragPreview : MonoBehaviour
{
    public static DragPreview Instance;

    private Image previewImage;
    private Canvas canvas;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 自动创建 Image 子物体
        GameObject imgObj = new GameObject("PreviewIcon");
        imgObj.transform.SetParent(transform, false);
        previewImage = imgObj.AddComponent<Image>();
        previewImage.raycastTarget = false;
        previewImage.color = new Color(1, 1, 1, 0.8f); // 半透明

        // 获取父级 Canvas
        canvas = GetComponentInParent<Canvas>();
        Hide();
    }

    public void Show(Sprite icon)
    {
        if (previewImage != null)
        {
            previewImage.sprite = icon;
            previewImage.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (previewImage != null)
        {
            previewImage.gameObject.SetActive(false);
        }
    }

    public void UpdatePosition(Vector2 screenPos)
    {
        if (previewImage != null && previewImage.gameObject.activeSelf)
        {
            // 转换到 Canvas 空间
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                canvas.worldCamera,
                out Vector2 localPos
            );
            previewImage.rectTransform.anchoredPosition = localPos;
        }
    }
}