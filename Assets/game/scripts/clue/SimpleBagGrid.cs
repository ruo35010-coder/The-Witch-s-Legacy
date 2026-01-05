using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleBagGrid : MonoBehaviour, IPointerClickHandler
{
    [Header("格子信息")]
    public int gridIndex;
    public ClueData storedClue;
    
    [Header("显示组件")]
    public Image clueIcon;
    public Image background;
    
    void Start()
    {
        UpdateDisplay();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (storedClue != null && ClueUIManager.Instance != null)
        {
            ClueUIManager.Instance.ShowClueGridUI(storedClue, this);
        }
    }
    
    void UpdateDisplay()
    {
        if (clueIcon != null)
        {
            if (storedClue != null && storedClue.clueSprite != null)
            {
                clueIcon.sprite = storedClue.clueSprite;
                clueIcon.color = Color.white;
                clueIcon.enabled = true;
            }
            else
            {
                clueIcon.sprite = null;
                clueIcon.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
                clueIcon.enabled = true;
            }
        }
        
        if (background != null)
        {
            background.color = IsEmpty() ? new Color(0.3f, 0.3f, 0.3f, 0.3f) : new Color(0.5f, 0.5f, 0.8f, 0.3f);
        }
    }
    
    public bool StoreClue(ClueData clue)
    {
        if (storedClue != null)
        {
            Debug.LogWarning($"格子 {gridIndex} 已有线索");
            return false;
        }
        
        storedClue = clue;
        UpdateDisplay();
        Debug.Log($"线索 {clue.clueName} 放入格子 {gridIndex}");
        return true;
    }
    
    public ClueData RemoveClue()
    {
        ClueData clue = storedClue;
        storedClue = null;
        UpdateDisplay();
        return clue;
    }
    
    public bool IsEmpty()
    {
        return storedClue == null;
    }
    
    // 添加这个方法
    public string GetClueName()
    {
        return storedClue != null ? storedClue.clueName : "空";
    }
}