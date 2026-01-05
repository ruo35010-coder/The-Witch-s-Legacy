using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class PotionConfigUI : MonoBehaviour
{
    [Header("UI面板")]
    public GameObject configPanel;
    
    [Header("材料槽位")]
    public PotionSlot[] materialSlots = new PotionSlot[3]; // 最多3个槽位
    
    [Header("按钮")]
    public Button createPotionButton;
    public Button exitButton;
    public Button clearButton;
    
    [Header("配方系统")]
    public List<PotionRecipe> recipeList = new List<PotionRecipe>();
    
    [Header("魔药生成位置")]
    public Transform potionSpawnPoint;
    public float spawnHeight = 1.5f;
    
    [Header("音效")]
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip successSound;
    public AudioClip failSound;
    
    private AudioSource audioSource;
    private CauldronInteractable currentCauldron;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // 设置按钮事件
        if (createPotionButton != null)
            createPotionButton.onClick.AddListener(CreatePotion);
        if (exitButton != null)
            exitButton.onClick.AddListener(CloseUI);
        if (clearButton != null)
            clearButton.onClick.AddListener(ClearAllSlots);
        
        // 隐藏UI
        if (configPanel != null)
            configPanel.SetActive(false);
    }
    
    void Update()
    {
        if (configPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUI();
        }
    }
    
    // 打开UI
    public void OpenUI(CauldronInteractable cauldron)
    {
        currentCauldron = cauldron;
        
        if (configPanel != null)
        {
            configPanel.SetActive(true);
            PlaySound(openSound);
            Time.timeScale = 0f;
        }
    }
    
    // 关闭UI
    public void CloseUI()
    {
        if (configPanel != null && configPanel.activeSelf)
        {
            ClearAllSlots();
            configPanel.SetActive(false);
            PlaySound(closeSound);
            Time.timeScale = 1f;
        }
    }
    
    // 生成魔药 - 修改：不需要放满3个材料
    void CreatePotion()
    {
        // 收集当前使用的材料
        List<ItemData> currentMaterials = new List<ItemData>();
        List<string> materialIds = new List<string>();
        
        foreach (var slot in materialSlots)
        {
            if (slot != null && !slot.IsEmpty && slot.CurrentItem != null)
            {
                currentMaterials.Add(slot.CurrentItem);
                materialIds.Add(slot.CurrentItem.itemId);
            }
        }
        
        // 检查是否有材料（至少1个就可以）
        if (currentMaterials.Count == 0)
        {
            Debug.Log("❌ 请放入材料！");
            PlaySound(failSound);
            return;
        }
        
        // 检查配方 - 2个或3个材料都可以
        PotionRecipe recipe = CheckRecipe(materialIds);
        
        if (recipe != null && recipe.resultPotion != null)
        {
            // 成功
            PlaySound(successSound);
            SpawnPotion(recipe.resultPotion);
            ClearAllSlots();
            CloseUI();
            Debug.Log($"✅ 成功生成: {recipe.resultPotion.name} (使用{currentMaterials.Count}个材料)");
        }
        else
        {
            // 失败
            PlaySound(failSound);
            ClearAllSlots();
            Debug.Log($"❌ 配方错误，材料已清空 (尝试使用{currentMaterials.Count}个材料)");
        }
    }
    
    // 检查配方（支持1-3种材料）
    PotionRecipe CheckRecipe(List<string> materialIds)
    {
        if (recipeList.Count == 0)
        {
            Debug.Log("⚠️ 没有设置任何配方");
            return null;
        }
        
        Debug.Log($"检查配方，当前材料({materialIds.Count}个): {string.Join(", ", materialIds)}");
        
        // 首先检查精确匹配的配方
        foreach (var recipe in recipeList)
        {
            if (recipe == null) continue;
            
            // 如果配方要求精确匹配，检查材料数量是否相等
            if (recipe.exactMatch)
            {
                if (materialIds.Count == recipe.requiredMaterialIds.Count)
                {
                    bool allMatched = true;
                    foreach (var requiredId in recipe.requiredMaterialIds)
                    {
                        if (!materialIds.Contains(requiredId))
                        {
                            allMatched = false;
                            break;
                        }
                    }
                    
                    if (allMatched)
                    {
                        Debug.Log($"✅ 精确匹配配方: {recipe.recipeName}");
                        return recipe;
                    }
                }
            }
            else
            {
                // 不要求精确匹配，只要包含所有必需材料就可以
                bool allRequiredPresent = true;
                foreach (var requiredId in recipe.requiredMaterialIds)
                {
                    if (!materialIds.Contains(requiredId))
                    {
                        allRequiredPresent = false;
                        break;
                    }
                }
                
                if (allRequiredPresent)
                {
                    Debug.Log($"✅ 模糊匹配配方: {recipe.recipeName}");
                    return recipe;
                }
            }
        }
        
        Debug.Log("❌ 没有找到匹配的配方");
        return null;
    }
    
    // 清空所有槽位
    void ClearAllSlots()
    {
        foreach (var slot in materialSlots)
        {
            if (slot != null)
            {
                slot.ClearSlot();
            }
        }
    }
    
    // 生成魔药
    void SpawnPotion(ItemData potionData)
    {
        if (potionData == null) return;
    
        Vector3 spawnPos;

        // ✅ 优先使用你指定的空物体位置（这是你唯一的要求）
        if (potionSpawnPoint != null)
        {
            spawnPos = potionSpawnPoint.position;
        }
        else if (currentCauldron != null)
        {
            spawnPos = currentCauldron.transform.position + Vector3.up * spawnHeight;
        }
        else if (potionSpawnPoint != null)
        {
            spawnPos = potionSpawnPoint.position;
        }
        else
        {
            spawnPos = transform.position + Vector3.up * 2f;
        }
    
        // 生成魔药
        if (potionData.prefab != null)
        {
            GameObject potion = Instantiate(potionData.prefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.Log("⚠️ 魔药没有预制体，仅添加到背包");
        
            // 如果没有预制体，至少添加到背包
            if (BackpackManager.Instance != null)
            {
                BackpackManager.Instance.AddItem(potionData);
            }
        }
    }
    // 添加材料到槽位
    public bool AddMaterialToSlot(ItemData material, int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < materialSlots.Length)
        {
            if (materialSlots[slotIndex] != null)
            {
                materialSlots[slotIndex].SetItem(material);
                return true;
            }
        }
        return false;
    }
    
    // 获取第一个空槽位
    public int GetFirstEmptySlot()
    {
        for (int i = 0; i < materialSlots.Length; i++)
        {
            if (materialSlots[i] != null && materialSlots[i].IsEmpty)
            {
                return i;
            }
        }
        return -1;
    }
    
    // 播放音效
    void PlaySound(AudioClip sound)
    {
        if (sound != null && audioSource != null)
        {
            audioSource.PlayOneShot(sound);
        }
    }
}