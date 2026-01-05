using UnityEngine;

public class HarvestableSource : MonoBehaviour
{
    [Header("物品设置")]
    public ItemData dropItem;
    
    [Header("状态设置")]
    public bool canRegenerate = false;  // 是否可再生
    public float regenerateTime = 60f;  // 再生时间（秒）
    
    [Header("视觉反馈")]
    public Material harvestedMaterial;  // 采集后的材质
    public ParticleSystem collectEffect;
    public AudioClip collectSound;
    
    private bool collected = false;
    private Renderer objectRenderer;
    private Material originalMaterial;
    private Collider objectCollider;
    private float regenerateTimer = 0f;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        objectCollider = GetComponent<Collider>();
        
        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }
        
        // 确保有 Collider
        if (objectCollider == null)
        {
            objectCollider = gameObject.AddComponent<BoxCollider>();
        }
    }

    void OnMouseDown()
    {
        if (collected || dropItem == null) return;

        if (BackpackManager.Instance.AddItem(dropItem))
        {
            collected = true;
            Debug.Log($"✅ 获得：{dropItem.name}");
            
            // 播放特效和音效
            PlayHarvestEffects();
            
            // 改变物体外观（变成已采集状态）
            ChangeToHarvestedAppearance();
            
            // 禁用碰撞器，防止再次点击
            if (objectCollider != null)
            {
                objectCollider.enabled = false;
            }
            
          
            // 如果可再生，开始再生计时
            if (canRegenerate)
            {
                regenerateTimer = regenerateTime;
            }
        }
        else
        {
            // 背包满了，给个提示
            Debug.Log("🎒 背包已满！");
        }
    }

    void Update()
    {
        // 处理再生逻辑
        if (collected && canRegenerate && regenerateTimer > 0)
        {
            regenerateTimer -= Time.deltaTime;
            
            // 显示再生进度（可选）
            float progress = 1f - (regenerateTimer / regenerateTime);
            UpdateRegenerationProgress(progress);
            
            if (regenerateTimer <= 0)
            {
                Regenerate();
            }
        }
    }

    void PlayHarvestEffects()
    {
        // 粒子特效
        if (collectEffect != null)
        {
            ParticleSystem effect = Instantiate(collectEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, 2f);
        }
        
        // 音效
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
        
        // 简单的动画效果
        StartCoroutine(HarvestAnimation());
    }

    System.Collections.IEnumerator HarvestAnimation()
    {
        // 轻微缩放效果
        Vector3 originalScale = transform.localScale;
        float duration = 0.2f;
        float elapsed = 0f;
        
        // 缩小一点
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = originalScale * Mathf.Lerp(1f, 0.8f, t);
            yield return null;
        }
        
        // 恢复
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = originalScale * Mathf.Lerp(0.8f, 1f, t);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }

    void ChangeToHarvestedAppearance()
    {
        if (objectRenderer != null)
        {
            if (harvestedMaterial != null)
            {
                // 使用指定的采集后材质
                objectRenderer.material = harvestedMaterial;
            }
            else
            {
                // 没有指定材质，就变暗或半透明
                Color darkColor = Color.gray;
                darkColor.a = 0.5f; // 半透明
                
                if (objectRenderer.material.HasProperty("_Color"))
                {
                    objectRenderer.material.color = darkColor;
                }
            }
        }
    }

    void UpdateRegenerationProgress(float progress)
    {
        // 这里可以添加再生进度显示
        // 比如：改变物体透明度或大小来显示进度
        if (objectRenderer != null)
        {
            Color color = objectRenderer.material.color;
            color.a = progress * 0.5f; // 慢慢变透明
            objectRenderer.material.color = color;
        }
    }

    void Regenerate()
    {
        collected = false;
        
        // 恢复外观
        if (objectRenderer != null && originalMaterial != null)
        {
            objectRenderer.material = originalMaterial;
        }
        
        // 启用碰撞器
        if (objectCollider != null)
        {
            objectCollider.enabled = true;
        }
        
       
        
        Debug.Log($"🌱 {dropItem.name} 已再生！");
    }

    // 公共方法：检查是否已采集
    public bool IsCollected()
    {
        return collected;
    }
    
    // 公共方法：手动重置（用于测试）
    public void ResetHarvestable()
    {
        collected = false;
        regenerateTimer = 0f;
        
        if (objectRenderer != null && originalMaterial != null)
        {
            objectRenderer.material = originalMaterial;
        }
        
        if (objectCollider != null)
        {
            objectCollider.enabled = true;
        }
     
    }
}