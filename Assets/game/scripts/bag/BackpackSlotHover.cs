using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BackpackSlotTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("背包槽位引用")]
    public BackpackSlot slot;
    
    [Header("工具提示设置")]
    [SerializeField] private Vector2 tooltipSize = new Vector2(200, 60);
    
    [Header("位置设置 - 重要！")]
    [SerializeField] private Vector2 offset = new Vector2(0, 0); // 从(0,50)改为(0,0)
    [SerializeField] private bool useWorldPosition = true; // 使用世界坐标计算
    
    [Header("字体设置")]
    [SerializeField] private TMP_FontAsset chineseFontAsset;
    [SerializeField] private int titleSize = 20;
    [SerializeField] private int descSize = 16;
    
    [Header("样式")]
    [SerializeField] private Color bgColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
    
    private GameObject tooltipObject;
    private TextMeshProUGUI tooltipText;
    private Canvas canvas;
    private RectTransform canvasRect;
    private bool isShowing = false;

    void Start()
    {
        if (slot == null)
            slot = GetComponent<BackpackSlot>();
        
        // 获取Canvas
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();
        
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        if (isShowing && tooltipObject != null)
        {
            UpdateTooltipPosition();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    void ShowTooltip()
    {
        if (slot == null || slot.itemData == null) return;
        
        if (tooltipObject == null)
        {
            CreateTooltip();
        }
        
        if (tooltipText != null)
        {
            if (chineseFontAsset != null)
            {
                tooltipText.font = chineseFontAsset;
            }
            
            // 只显示物品名称
            tooltipText.text = slot.itemData.name;
        }
        
        tooltipObject.SetActive(true);
        isShowing = true;
        UpdateTooltipPosition();
    }

    void CreateTooltip()
    {
        if (canvas == null) return;
        
        tooltipObject = new GameObject("SlotTooltip");
        tooltipObject.transform.SetParent(canvas.transform);
        tooltipObject.transform.SetAsLastSibling();
        
        RectTransform rect = tooltipObject.AddComponent<RectTransform>();
        rect.sizeDelta = tooltipSize;
        rect.pivot = new Vector2(0.5f, 0.5f); // 改为中心锚点
        
        Image bg = tooltipObject.AddComponent<Image>();
        bg.color = bgColor;
        bg.raycastTarget = false;
        
        GameObject textObj = new GameObject("TooltipText");
        textObj.transform.SetParent(tooltipObject.transform);
        
        tooltipText = textObj.AddComponent<TextMeshProUGUI>();
        tooltipText.color = Color.white;
        tooltipText.alignment = TextAlignmentOptions.Center;
        tooltipText.enableWordWrapping = true;
        tooltipText.raycastTarget = false;
        
        if (chineseFontAsset != null)
        {
            tooltipText.font = chineseFontAsset;
        }
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 10);
        textRect.offsetMax = new Vector2(-10, -10);
        
        tooltipObject.SetActive(false);
    }

    void UpdateTooltipPosition()
    {
        if (tooltipObject == null || canvas == null || canvasRect == null) return;
        
        // 获取背包格子的RectTransform
        RectTransform slotRect = GetComponent<RectTransform>();
        
        if (useWorldPosition)
        {
            // 方法1：使用世界坐标（更准确）
            UpdatePositionUsingWorldSpace(slotRect);
        }
        else
        {
            // 方法2：使用局部坐标
            UpdatePositionUsingLocalSpace(slotRect);
        }
    }

    void UpdatePositionUsingWorldSpace(RectTransform slotRect)
    {
        // 获取背包格子的世界位置
        Vector3 slotWorldPosition = slotRect.position;
        
        // 将世界位置转换为Canvas的局部位置
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, slotWorldPosition),
            canvas.worldCamera,
            out localPoint
        );
        
        // 应用你的偏移 (-1000, -1060)
        localPoint.x += offset.x;
        localPoint.y += offset.y;
        
        // 设置工具提示位置
        tooltipObject.GetComponent<RectTransform>().anchoredPosition = localPoint;
    }

    void UpdatePositionUsingLocalSpace(RectTransform slotRect)
    {
        // 直接使用背包格子的局部位置
        Vector2 slotLocalPosition = slotRect.anchoredPosition;
        
        // 应用偏移
        Vector2 tooltipPosition = new Vector2(
            slotLocalPosition.x + offset.x,
            slotLocalPosition.y + offset.y
        );
        
        // 设置工具提示位置
        tooltipObject.GetComponent<RectTransform>().anchoredPosition = tooltipPosition;
    }

    void HideTooltip()
    {
        if (tooltipObject != null)
        {
            tooltipObject.SetActive(false);
            isShowing = false;
        }
    }

    void OnDisable()
    {
        HideTooltip();
    }

    void OnDestroy()
    {
        if (tooltipObject != null)
        {
            Destroy(tooltipObject);
        }
    }
    
    #if UNITY_EDITOR
    [ContextMenu("应用正确偏移 (-1000, -1060)")]
    void ApplyCorrectOffset()
    {
        offset = new Vector2(0, 0);
        Debug.Log("已应用偏移: (-1000, -1060)");
    }
    
    [ContextMenu("测试位置")]
    void TestPosition()
    {
        if (slot != null && slot.itemData != null)
        {
            ShowTooltip();
            
            // 打印位置信息
            RectTransform slotRect = GetComponent<RectTransform>();
            Debug.Log($"背包格子位置: {slotRect.anchoredPosition}");
            
            if (tooltipObject != null)
            {
                RectTransform tooltipRect = tooltipObject.GetComponent<RectTransform>();
                Debug.Log($"工具提示位置: {tooltipRect.anchoredPosition}");
            }
        }
    }
    #endif
}