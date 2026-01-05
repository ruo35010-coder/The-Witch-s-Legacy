using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackpackSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("背包物品")]
    public ItemData itemData;
    
    [Header("UI显示")]
    public Image iconImage;
    
    void Start()
    {
        UpdateDisplay();
    }
    
    // 更新显示
    void UpdateDisplay()
    {
        if (itemData != null && itemData.icon != null)
        {
            iconImage.sprite = itemData.icon;
            iconImage.color = Color.white;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.color = Color.clear;
        }
    }
    
    // 点击时放入第一个空魔药槽位
    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemData == null) return;
        
        Debug.Log($"点击背包物品: {itemData.name}");
        
        // 查找魔药配置UI
        PotionConfigUI potionUI = FindObjectOfType<PotionConfigUI>();
        if (potionUI != null && potionUI.configPanel.activeSelf)
        {
            // 查找第一个空槽位
            int emptySlotIndex = potionUI.GetFirstEmptySlot();
            if (emptySlotIndex >= 0)
            {
                // 创建物品副本放入槽位
                potionUI.AddMaterialToSlot(itemData, emptySlotIndex);
                Debug.Log($"✅ 物品已放入槽位 {emptySlotIndex + 1}");
            }
            else
            {
                Debug.Log("❌ 所有槽位都已满");
            }
        }
        else
        {
            Debug.Log("❌ 魔药配置UI未打开");
        }
    }
    
    // 设置物品
    public void SetItem(ItemData item)
    {
        itemData = item;
        UpdateDisplay();
    }
}