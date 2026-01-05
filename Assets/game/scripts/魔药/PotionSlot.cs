using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotionSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("槽位显示")]
    public Image iconDisplay;
    
    [Header("槽位状态")]
    public Color emptyColor = new Color(1f, 1f, 1f, 0.3f);
    public Color filledColor = Color.white;
    
    public ItemData CurrentItem { get; private set; }
    public bool IsEmpty => CurrentItem == null;
    
    void Start()
    {
        if (iconDisplay == null)
        {
            // 尝试自动获取Image组件
            iconDisplay = GetComponentInChildren<Image>();
        }
        ClearSlot();
    }
    
    // 点击槽位：有物品时清空，空的时候不操作
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsEmpty)
        {
            Debug.Log($"点击清空槽位: {CurrentItem.name}");
            ClearSlot();
        }
    }
    
    // 设置物品
    public void SetItem(ItemData item)
    {
        CurrentItem = item;
        UpdateDisplay();
    }
    
    // 更新显示
    void UpdateDisplay()
    {
        if (iconDisplay == null) return;
        
        if (CurrentItem != null && CurrentItem.icon != null)
        {
            iconDisplay.sprite = CurrentItem.icon;
            iconDisplay.color = filledColor;
        }
        else
        {
            ClearSlot();
        }
    }
    
    // 清空槽位
    public void ClearSlot()
    {
        CurrentItem = null;
        if (iconDisplay != null)
        {
            iconDisplay.sprite = null;
            iconDisplay.color = emptyColor;
        }
    }
}