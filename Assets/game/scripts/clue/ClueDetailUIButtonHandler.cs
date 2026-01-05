using UnityEngine;
using UnityEngine.UI;

public class ClueDetailUIButtonHandler : MonoBehaviour
{
    [Header("按钮引用")]
    public Button addToBagButton;
    public Button closeButton;
    
    [Header("UI组件")]
    public Image clueImage;
    public Text clueNameText;
    public Text descriptionText;
    
    [Header("线索数据")]
    public ClueData clueData;
    
    void Start()
    {
        Debug.Log("ClueDetailUIButtonHandler Start");
        
        // 自动获取组件
        if (addToBagButton == null)
            addToBagButton = transform.Find("AddToBagButton")?.GetComponent<Button>();
        if (closeButton == null)
            closeButton = transform.Find("CloseButton")?.GetComponent<Button>();
        if (clueImage == null)
            clueImage = transform.Find("ClueImage")?.GetComponent<Image>();
        if (clueNameText == null)
            clueNameText = transform.Find("ClueNameText")?.GetComponent<Text>();
        if (descriptionText == null)
            descriptionText = transform.Find("DescriptionText")?.GetComponent<Text>();
        
        // 如果已经有数据，直接设置UI
        if (clueData != null)
        {
            UpdateUI();
        }
        
        SetupButtons();
    }
    
    void SetupButtons()
    {
        // 放入背包按钮
        if (addToBagButton != null)
        {
            addToBagButton.onClick.RemoveAllListeners();
            addToBagButton.onClick.AddListener(() => {
                Debug.Log("放入背包按钮被点击");
                
                if (clueData == null)
                {
                    Debug.LogError("当前线索数据为空，无法放入背包");
                    return;
                }
                
                Debug.Log($"尝试放入背包: {clueData.clueName}");
                AddClueToBagAndClose();
            });
        }
        
        // 关闭按钮
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => {
                Debug.Log("关闭按钮被点击");
                Destroy(gameObject);
            });
        }
    }
    
    void AddClueToBagAndClose()
    {
        if (clueData == null)
        {
            Debug.LogError("线索数据为空，无法放入背包");
            return;
        }
        
        if (ClueUIManager.Instance != null)
        {
            bool success = ClueUIManager.Instance.AddClueToBag(clueData);
            
            if (success)
            {
                Debug.Log($"线索 {clueData.clueName} 已放入背包");
            }
            else
            {
                Debug.LogWarning("放入背包失败（可能背包已满）");
            }
        }
        
        Destroy(gameObject);
    }
    
    // 外部调用初始化
    public void Initialize(ClueData data)
    {
        if (data == null)
        {
            Debug.LogError("传入的线索数据为空");
            return;
        }
        
        clueData = data;
        Debug.Log($"ClueDetailUIButtonHandler 初始化: {data.clueName}");
        
        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (clueData == null) return;
        
        if (clueImage != null && clueData.clueSprite != null)
        {
            clueImage.sprite = clueData.clueSprite;
            clueImage.preserveAspect = true;
        }
        
        if (clueNameText != null)
        {
            clueNameText.text = clueData.clueName;
        }
        
        if (descriptionText != null)
        {
            descriptionText.text = clueData.description;
        }
    }
}