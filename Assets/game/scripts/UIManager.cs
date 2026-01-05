// UIManager.cs
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject tooltipPanel;
    public Text itemNameText;
    public Text itemDescText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        tooltipPanel.SetActive(false);
    }

    public void ShowTooltip(ItemData item)
    {
        if (item != null)
        {
            itemNameText.text = item.name;
            itemDescText.text = item.description;
            tooltipPanel.SetActive(true);
            UpdateTooltipPosition(); // 立即更新一次位置
        }
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    // 新增这个方法
    public void UpdateTooltipPosition()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            tooltipPanel.transform.position = Input.mousePosition + new Vector3(10, 10, 0);
        }
    }

    void Update()
    {
        // 让 Tooltip 跟随鼠标
        UpdateTooltipPosition();
    }
}