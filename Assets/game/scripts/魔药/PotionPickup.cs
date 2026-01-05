using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    [Header("魔药数据")]
    public ItemData potionData;  // 这个魔药对应的ItemData
    
    [Header("拾取效果")]
    public AudioClip pickupSound;
    public ParticleSystem pickupEffect;
    public float destroyDelay = 0.2f;
    
    [Header("悬停提示")]
    public string hoverText = "点击拾取魔药";
    
    private bool canPickup = true;
    
    void Start()
    {
        // 自动添加碰撞器
        if (GetComponent<Collider>() == null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = false; // 不是触发器，需要OnMouseDown
        }
        
        // 如果没有设置potionData，尝试从名字匹配
        if (potionData == null)
        {
            FindPotionDataByName();
        }
    }
    
    void FindPotionDataByName()
    {
        // 根据物体名字查找对应的ItemData
        string objectName = gameObject.name.ToLower();
        
        // 这里可以根据你的命名规则来匹配
        if (objectName.Contains("health") || objectName.Contains("治疗"))
        {
            // 尝试加载治疗药水
            potionData = Resources.Load<ItemData>("Items/HealthPotion");
        }
        else if (objectName.Contains("mana") || objectName.Contains("魔法"))
        {
            // 尝试加载魔法药水
            potionData = Resources.Load<ItemData>("Items/ManaPotion");
        }
        
        if (potionData != null)
        {
            Debug.Log($"✅ 自动匹配魔药: {potionData.name}");
        }
    }
    
    void OnMouseEnter()
    {
        // 简单悬停提示
        Debug.Log($"🖱️ {hoverText}: {potionData?.name ?? "未知魔药"}");
    }
    
    void OnMouseDown()
    {
        if (!canPickup) return;
        
        TryPickupPotion();
    }
    
    void TryPickupPotion()
    {
        if (potionData == null)
        {
            Debug.LogError("❌ 魔药数据为空！");
            return;
        }
        
        // 尝试添加到背包
        if (BackpackManager.Instance != null)
        {
            bool added = BackpackManager.Instance.AddItem(potionData);
            
            if (added)
            {
                Debug.Log($"✅ 获得魔药: {potionData.name}");
                
                // 播放拾取效果
                PlayPickupEffects();
                
                // 标记为已拾取
                canPickup = false;
                
                // 延迟销毁物体
                Invoke("DestroyPotion", destroyDelay);
            }
            else
            {
                Debug.Log("🎒 背包已满！");
                // 可以播放背包满的音效
            }
        }
        else
        {
            Debug.LogError("❌ 找不到BackpackManager！");
        }
    }
    
    void PlayPickupEffects()
    {
        // 播放音效
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
        
        // 播放粒子效果
        if (pickupEffect != null)
        {
            ParticleSystem effect = Instantiate(pickupEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, 2f);
        }
        
        // 简单的缩放动画
        StartCoroutine(PickupAnimation());
    }
    
    System.Collections.IEnumerator PickupAnimation()
    {
        // 快速缩小消失
        float duration = destroyDelay;
        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = originalScale * (1f - t);
            yield return null;
        }
    }
    
    void DestroyPotion()
    {
        Destroy(gameObject);
    }
    
    // 公共方法：手动拾取（可以由其他脚本调用）
    public void Pickup()
    {
        TryPickupPotion();
    }
    
    // 设置魔药数据
    public void SetPotionData(ItemData data)
    {
        potionData = data;
    }
}