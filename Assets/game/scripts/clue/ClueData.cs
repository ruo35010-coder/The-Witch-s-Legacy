using UnityEngine;

[CreateAssetMenu(fileName = "NewClueData", menuName = "Clue System/Clue Data")]
public class ClueData : ScriptableObject
{
    [Header("基本信息")]
    public string clueId = "CLUE_001";
    public string clueName = "线索名称";
    
    [Header("显示内容")]
    public Sprite clueSprite;
    
    [TextArea(2, 4)]
    public string description = "线索描述...";
    
    [Header("自定义UI（可选）")]
    public GameObject customClueUIPrefab;  // 自定义线索详情UI
    public GameObject customGridUIPrefab;  // 自定义背包查看UI
    
    [Header("游戏数据")]
    public bool isImportant = false;
    public int clueValue = 1;
    
    private bool _isCollectedInThisSession = false;
    
    public bool IsCollected() => _isCollectedInThisSession;
    
    public void MarkAsCollected() => _isCollectedInThisSession = true;
    
    public void ResetForNewGame() => _isCollectedInThisSession = false;
}