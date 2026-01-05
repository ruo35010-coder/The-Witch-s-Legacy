using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ClueUIManager : MonoBehaviour
{
    [Header("默认UI预制体")]
    public GameObject defaultClueUIPrefab;    // 默认线索详情UI
    public GameObject defaultGridUIPrefab;    // 默认背包查看UI
    
    [Header("背包格子父对象")]
    public Transform bagGridsParent;
    
    [Header("UI父节点")]
    public Transform uiCanvas;
    
    [Header("线索数据库")]
    public List<ClueData> clueDatabase = new List<ClueData>();
    
    [Header("游戏管理")]
    public bool resetOnNewGame = true;
    
    private GameObject currentUI;
    private ClueData currentClueData;
    private SimpleBagGrid currentBagGrid;
    
    // 单例模式
    private static ClueUIManager _instance;
    public static ClueUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ClueUIManager>();
            }
            return _instance;
        }
    }
    
    void Start()
    {
        if (uiCanvas == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null) uiCanvas = canvas.transform;
        }
        
        InitializeBagGrids();
        
        if (resetOnNewGame)
        {
            ResetAllCluesForNewGame();
        }
    }
    
    void InitializeBagGrids()
    {
        if (bagGridsParent == null) return;
        
        int index = 0;
        foreach (Transform child in bagGridsParent)
        {
            SimpleBagGrid grid = child.GetComponent<SimpleBagGrid>();
            if (grid != null)
            {
                grid.gridIndex = index;
                index++;
            }
        }
        Debug.Log($"初始化了 {index} 个背包格子");
    }
    
    public void ResetAllCluesForNewGame()
    {
        Debug.Log("=== 开始新游戏，重置所有线索 ===");
        
        // 重置所有线索数据
        foreach (ClueData clue in clueDatabase)
        {
            clue.ResetForNewGame();
        }
        
        // 重置场景中所有线索物体
        SceneClue[] sceneClues = FindObjectsOfType<SceneClue>(true);
        foreach (SceneClue sceneClue in sceneClues)
        {
            sceneClue.ResetClue();
        }
        
        // 清空背包
        ClearAllBagGrids();
        
        Debug.Log($"重置完成: {sceneClues.Length} 个场景线索，{clueDatabase.Count} 个线索数据");
    }
    
    void ClearAllBagGrids()
    {
        if (bagGridsParent == null) return;
        
        foreach (Transform child in bagGridsParent)
        {
            SimpleBagGrid grid = child.GetComponent<SimpleBagGrid>();
            if (grid != null)
            {
                grid.RemoveClue();
            }
        }
    }
    
    // ==================== 显示线索详情 ====================
    public void ShowClueDetail(ClueData clueData)
    {
        if (clueData == null)
        {
            Debug.LogError("线索数据为空！");
            return;
        }
        
        // 检查是否已收集
        if (clueData.IsCollected())
        {
            Debug.Log($"线索 {clueData.clueName} 已经收集过了");
            return;
        }
        
        CloseCurrentUI();
        currentClueData = clueData;
        
        // 优先使用自定义UI，否则使用默认UI
        GameObject uiPrefab = clueData.customClueUIPrefab != null 
            ? clueData.customClueUIPrefab 
            : defaultClueUIPrefab;
        
        if (uiPrefab != null && uiCanvas != null)
        {
            // 创建UI实例
            currentUI = Instantiate(uiPrefab, uiCanvas);
            
            // 设置位置
            RectTransform rectTransform = currentUI.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.localScale = Vector3.one;
            }
            
            // 初始化UI
            InitializeClueUI(currentUI, clueData);
            
            Debug.Log($"显示线索详情: {clueData.clueName} (使用{(clueData.customClueUIPrefab != null ? "自定义" : "默认")}UI)");
        }
        else
        {
            Debug.LogError("UI预制体为空！");
        }
    }
    
    // 初始化UI内容
    void InitializeClueUI(GameObject uiInstance, ClueData clueData)
    {
        if (uiInstance == null || clueData == null) return;
        
        // 方法1：使用ClueDetailUIButtonHandler（推荐）
        ClueDetailUIButtonHandler handler = uiInstance.GetComponent<ClueDetailUIButtonHandler>();
        if (handler != null)
        {
            handler.Initialize(clueData);
            return;
        }
        
        // 方法2：如果没有处理器，手动设置通用UI组件
        SetupUniversalUI(uiInstance, clueData);
    }
    
    // 通用UI设置方法
    void SetupUniversalUI(GameObject uiInstance, ClueData clueData)
    {
        Debug.Log("使用通用UI设置方法");
        
        // 设置图片
        SetupImageComponent(uiInstance, "ClueImage", clueData.clueSprite);
        
        // 设置文本（尝试多个可能的名称）
        SetupTextComponent(uiInstance, "ClueNameText", clueData.clueName);
        SetupTextComponent(uiInstance, "ClueName", clueData.clueName);
        SetupTextComponent(uiInstance, "NameText", clueData.clueName);
        
        SetupTextComponent(uiInstance, "DescriptionText", clueData.description);
        SetupTextComponent(uiInstance, "Description", clueData.description);
        SetupTextComponent(uiInstance, "DescText", clueData.description);
        
        // 设置放入背包按钮
        SetupAddToBagButton(uiInstance, clueData);
        
        // 设置关闭按钮
        SetupCloseButton(uiInstance);
    }
    
    // 设置图片组件
    void SetupImageComponent(GameObject parent, string childName, Sprite sprite)
    {
        Transform child = parent.transform.Find(childName);
        if (child != null)
        {
            Image image = child.GetComponent<Image>();
            if (image != null && sprite != null)
            {
                image.sprite = sprite;
                image.preserveAspect = true;
            }
        }
    }
    
    // 设置文本组件
    void SetupTextComponent(GameObject parent, string childName, string text)
    {
        Transform child = parent.transform.Find(childName);
        if (child != null)
        {
            Text textComponent = child.GetComponent<Text>();
            if (textComponent != null && !string.IsNullOrEmpty(text))
            {
                textComponent.text = text;
            }
        }
    }
    
    // 设置放入背包按钮
    void SetupAddToBagButton(GameObject parent, ClueData clueData)
    {
        // 尝试常见的按钮名称
        string[] possibleButtonNames = { 
            "AddToBagButton", "AddButton", "CollectButton", 
            "BtnAddToBag", "PutButton", "TakeButton"
        };
        
        foreach (string buttonName in possibleButtonNames)
        {
            Transform buttonTransform = parent.transform.Find(buttonName);
            if (buttonTransform != null)
            {
                Button button = buttonTransform.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => {
                        if (clueData != null)
                        {
                            AddClueToBagAndClose(clueData);
                        }
                    });
                    
                    // 可选：更新按钮文本
                    Text buttonText = button.GetComponentInChildren<Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = "放入背包";
                    }
                    
                    Debug.Log($"找到放入背包按钮: {buttonName}");
                    return;
                }
            }
        }
        
        Debug.LogWarning("未找到放入背包按钮，用户将无法收集此线索");
    }
    
    // 设置关闭按钮
    void SetupCloseButton(GameObject parent)
    {
        string[] possibleButtonNames = { 
            "CloseButton", "ExitButton", "BackButton", 
            "BtnClose", "XButton", "CancelButton"
        };
        
        foreach (string buttonName in possibleButtonNames)
        {
            Transform buttonTransform = parent.transform.Find(buttonName);
            if (buttonTransform != null)
            {
                Button button = buttonTransform.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(CloseCurrentUI);
                    Debug.Log($"找到关闭按钮: {buttonName}");
                    return;
                }
            }
        }
        
        // 如果没有找到关闭按钮，允许点击UI背景关闭
        TryAddBackgroundClose(parent);
    }
    
    void TryAddBackgroundClose(GameObject parent)
    {
        Image background = parent.GetComponent<Image>();
        if (background == null)
        {
            // 如果没有背景图片，尝试查找面板
            Transform panel = parent.transform.Find("Panel");
            if (panel != null) background = panel.GetComponent<Image>();
        }
        
        if (background != null)
        {
            Button bgButton = background.gameObject.GetComponent<Button>();
            if (bgButton == null) bgButton = background.gameObject.AddComponent<Button>();
            
            bgButton.onClick.RemoveAllListeners();
            bgButton.onClick.AddListener(CloseCurrentUI);
            
            // 设置透明颜色
            ColorBlock colors = bgButton.colors;
            colors.normalColor = new Color(0, 0, 0, 0);
            colors.highlightedColor = new Color(0, 0, 0, 0.05f);
            colors.pressedColor = new Color(0, 0, 0, 0.1f);
            bgButton.colors = colors;
            
            Debug.Log("已添加背景点击关闭功能");
        }
    }
    
    // ==================== 显示背包中的线索 ====================
    public void ShowClueGridUI(ClueData clueData, SimpleBagGrid bagGrid)
    {
        if (clueData == null || bagGrid == null) return;
        
        CloseCurrentUI();
        currentClueData = clueData;
        currentBagGrid = bagGrid;
        
        // 优先使用自定义背包查看UI，否则使用默认
        GameObject uiPrefab = clueData.customGridUIPrefab != null 
            ? clueData.customGridUIPrefab 
            : defaultGridUIPrefab;
        
        if (uiPrefab != null && uiCanvas != null)
        {
            currentUI = Instantiate(uiPrefab, uiCanvas);
            
            RectTransform rectTransform = currentUI.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.localScale = Vector3.one;
            }
            
            SetupGridUI(currentUI, clueData);
        }
    }
    
    void SetupGridUI(GameObject uiInstance, ClueData clueData)
    {
        if (uiInstance == null || clueData == null) return;
        
        // 设置图片
        SetupImageComponent(uiInstance, "ClueImage", clueData.clueSprite);
        
        // 设置文本
        SetupTextComponent(uiInstance, "ClueNameText", clueData.clueName);
        SetupTextComponent(uiInstance, "ClueName", clueData.clueName);
        SetupTextComponent(uiInstance, "NameText", clueData.clueName);
        
        SetupTextComponent(uiInstance, "DescriptionText", clueData.description);
        SetupTextComponent(uiInstance, "Description", clueData.description);
        SetupTextComponent(uiInstance, "DescText", clueData.description);
        
        // 设置退出按钮
        string[] exitButtonNames = { "ExitButton", "CloseButton", "BackButton", "ReturnButton" };
        foreach (string buttonName in exitButtonNames)
        {
            Transform buttonTransform = uiInstance.transform.Find(buttonName);
            if (buttonTransform != null)
            {
                Button button = buttonTransform.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(CloseCurrentUI);
                    return;
                }
            }
        }
    }
    
    // ==================== 放入背包逻辑 ====================
    void AddClueToBagAndClose(ClueData clueData)
    {
        if (clueData == null) return;
        
        bool success = AddClueToBag(clueData);
        
        if (success)
        {
            Debug.Log($"线索 {clueData.clueName} 已成功放入背包");
            DisableCollectedClueInScene(clueData.clueId);
        }
        
        CloseCurrentUI();
    }
    
    public bool AddClueToBag(ClueData clueData)
    {
        if (clueData == null) 
        {
            Debug.LogError("AddClueToBag: 线索数据为空");
            return false;
        }
        
        Debug.Log($"尝试放入背包: {clueData.clueName}");
        
        SimpleBagGrid emptyGrid = FindEmptyBagGrid();
        if (emptyGrid != null)
        {
            bool stored = emptyGrid.StoreClue(clueData);
            if (stored)
            {
                clueData.MarkAsCollected();
                return true;
            }
        }
        
        Debug.LogWarning("背包已满，无法放入线索！");
        return false;
    }
    
    SimpleBagGrid FindEmptyBagGrid()
    {
        if (bagGridsParent == null) return null;
        
        foreach (Transform child in bagGridsParent)
        {
            SimpleBagGrid grid = child.GetComponent<SimpleBagGrid>();
            if (grid != null && grid.IsEmpty())
            {
                return grid;
            }
        }
        
        return null;
    }
    
    void DisableCollectedClueInScene(string clueId)
    {
        SceneClue[] sceneClues = FindObjectsOfType<SceneClue>();
        foreach (SceneClue sceneClue in sceneClues)
        {
            if (sceneClue.clueData != null && sceneClue.clueData.clueId == clueId)
            {
                sceneClue.CollectClue();
                Debug.Log($"禁用场景中的线索: {sceneClue.clueData.clueName}");
                break;
            }
        }
    }
    
    void CloseCurrentUI()
    {
        if (currentUI != null)
        {
            Destroy(currentUI);
            currentUI = null;
        }
        currentClueData = null;
        currentBagGrid = null;
        Debug.Log("关闭当前UI");
    }
    
    public ClueData GetClueDataById(string clueId)
    {
        foreach (ClueData clue in clueDatabase)
        {
            if (clue.clueId == clueId)
            {
                return clue;
            }
        }
        return null;
    }
    
    // 调试方法
    public void DebugClueInfo(ClueData clueData)
    {
        if (clueData == null) return;
        
        Debug.Log($"=== 线索信息 ===");
        Debug.Log($"名称: {clueData.clueName}");
        Debug.Log($"ID: {clueData.clueId}");
        Debug.Log($"自定义UI: {clueData.customClueUIPrefab?.name ?? "无"}");
        Debug.Log($"自定义背包UI: {clueData.customGridUIPrefab?.name ?? "无"}");
        Debug.Log($"已收集: {clueData.IsCollected()}");
    }
}