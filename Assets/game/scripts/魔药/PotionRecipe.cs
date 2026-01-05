using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PotionRecipe
{
    [Header("配方名称")]
    public string recipeName = "新配方";
    
    [Header("所需材料ID")]
    public List<string> requiredMaterialIds = new List<string>();
    
    [Header("结果魔药")]
    public ItemData resultPotion;
    
    [Header("配方设置")]
    public bool exactMatch = true;  // 是否需要精确匹配（材料数量必须相等）
    
    // 检查材料是否匹配（支持2或3种材料）
    public bool MatchMaterialsById(List<string> providedMaterialIds)
    {
        if (requiredMaterialIds == null || requiredMaterialIds.Count == 0)
            return false;
        
        // 如果要求精确匹配，材料数量必须相等
        if (exactMatch && providedMaterialIds.Count != requiredMaterialIds.Count)
            return false;
        
        // 检查每个必需材料是否都有
        foreach (var requiredId in requiredMaterialIds)
        {
            if (!providedMaterialIds.Contains(requiredId))
                return false;
        }
        
        return true;
    }
}