using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HoverInteraction : MonoBehaviour
{
    public ItemData itemData;
    
    [Header("UI尺寸（屏幕像素）")]
    [SerializeField] private Vector2 panelSize = new Vector2(400, 200);
    [SerializeField] private Vector2 screenOffset = new Vector2(0, 30);
    
    [Header("字体大小")]
    [SerializeField] private int titleSize = 35;
    [SerializeField] private int descSize = 28;
    
    [Header("其他")]
    [SerializeField] private TMP_FontAsset fontAsset;
    [SerializeField] private Color bgColor = new Color(0, 0, 0, 0.8f);
    
    [Header("UI引用（可选）")]
    [SerializeField] private GameObject existingItemInfo; // 如果已经有ItemInfo，拖到这里
    
    private GameObject infoUI;
    private TextMeshProUGUI infoText;
    private Camera mainCamera;
    private Canvas parentCanvas;

    void Start()
    {
        mainCamera = Camera.main;
        
        // 自动添加碰撞器（仅用于鼠标检测）
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();
        
        // 查找现有的ItemInfo
        if (existingItemInfo != null)
        {
            infoUI = existingItemInfo;
            infoText = existingItemInfo.GetComponentInChildren<TextMeshProUGUI>();
            
            if (infoUI != null)
            {
                infoUI.SetActive(false);
            }
        }
    }

    void OnMouseEnter()
    {
        ShowInfo();
    }

    void OnMouseExit()
    {
        HideInfo();
    }

    void ShowInfo()
    {
        if (itemData == null || mainCamera == null) return;
        
        // 如果UI不存在，创建它
        if (infoUI == null)
        {
            if (!CreateUI())
            {
                Debug.LogError("无法创建UI！");
                return;
            }
        }
        
        // 确保UI在最前面
        infoUI.transform.SetAsLastSibling();
        
        // 更新文本
        if (infoText != null)
        {
            infoText.text = $"<size={titleSize}><b>{itemData.name}</b></size>\n<size={descSize}>{itemData.description}</size>";
        }
        
        // 显示UI
        infoUI.SetActive(true);
        UpdatePosition();
        
        Debug.Log($"显示物品信息: {itemData.name}");
    }

    bool CreateUI()
    {
        // 查找现有的Canvas（优先使用bag的Canvas）
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        
        // 优先使用bag的Canvas
        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.name.Contains("bag") || canvas.name.Contains("Bag"))
            {
                parentCanvas = canvas;
                break;
            }
        }
        
        // 如果没找到，使用第一个Canvas
        if (parentCanvas == null && allCanvases.Length > 0)
        {
            parentCanvas = allCanvases[0];
        }
        
        // 如果还是没找到，创建新的
        if (parentCanvas == null)
        {
            GameObject canvasObj = new GameObject("HoverCanvas");
            parentCanvas = canvasObj.AddComponent<Canvas>();
            parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            parentCanvas.sortingOrder = 999; // 最高层级
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // 创建UI面板
        infoUI = new GameObject("ItemInfo_Hover");
        infoUI.transform.SetParent(parentCanvas.transform);
        
        // 设置RectTransform
        RectTransform rect = infoUI.AddComponent<RectTransform>();
        rect.sizeDelta = panelSize;
        rect.pivot = new Vector2(0.5f, 0);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        
        // 添加背景
        Image bg = infoUI.AddComponent<Image>();
        bg.color = bgColor;
        bg.raycastTarget = false; // 重要：不阻挡鼠标事件
        
        // 创建文本
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(infoUI.transform);
        
        infoText = textObj.AddComponent<TextMeshProUGUI>();
        infoText.color = Color.white;
        infoText.alignment = TextAlignmentOptions.Center;
        infoText.enableWordWrapping = true;
        infoText.raycastTarget = false; // 重要：不阻挡鼠标事件
        
        if (fontAsset != null)
        {
            infoText.font = fontAsset;
        }
        
        // 设置文本位置
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 10);
        textRect.offsetMax = new Vector2(-10, -10);
        
        // 确保Canvas在Game视图中可见
        parentCanvas.gameObject.SetActive(true);
        
        return true;
    }

    void UpdatePosition()
    {
        if (infoUI == null || mainCamera == null) return;
        
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
        
        // 如果物体在相机后面，隐藏UI
        if (screenPos.z < 0)
        {
            infoUI.SetActive(false);
            return;
        }
        
        // 添加偏移
        screenPos.x += screenOffset.x;
        screenPos.y += screenOffset.y;
        
        // 获取UI尺寸
        RectTransform rect = infoUI.GetComponent<RectTransform>();
        float halfWidth = rect.sizeDelta.x / 2;
        float height = rect.sizeDelta.y;
        
        // 确保在屏幕内
        screenPos.x = Mathf.Clamp(screenPos.x, halfWidth, Screen.width - halfWidth);
        screenPos.y = Mathf.Clamp(screenPos.y, height + 10, Screen.height - 10);
        
        // 设置位置
        rect.anchoredPosition = new Vector2(screenPos.x, screenPos.y);
    }

    void HideInfo()
    {
        if (infoUI != null)
        {
            infoUI.SetActive(false);
        }
    }

    void Update()
    {
        if (infoUI != null && infoUI.activeSelf)
        {
            UpdatePosition();
        }
    }

    void OnDestroy()
    {
        // 只销毁动态创建的UI，不销毁预制的
        if (infoUI != null && existingItemInfo == null)
        {
            Destroy(infoUI);
        }
    }
    
    // 调试方法
    void OnGUI()
    {
        if (infoUI != null && infoUI.activeSelf)
        {
            // 在Game视图中显示调试信息
            RectTransform rect = infoUI.GetComponent<RectTransform>();
            Vector2 pos = rect.anchoredPosition;
            
            GUI.Label(new Rect(10, 10, 300, 100), 
                $"UI位置: {pos}\n" +
                $"UI激活: {infoUI.activeSelf}\n" +
                $"父Canvas: {parentCanvas?.name}");
        }
    }
    
    #if UNITY_EDITOR
    [ContextMenu("测试UI显示")]
    void TestUIShow()
    {
        ShowInfo();
    }
    
    [ContextMenu("测试UI隐藏")]
    void TestUIHide()
    {
        HideInfo();
    }
    #endif
}