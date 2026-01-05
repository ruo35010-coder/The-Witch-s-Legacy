using UnityEngine;
using UnityEngine.SceneManagement; // 新增：用于场景跳转

public class ClickToLoadScene : MonoBehaviour
{
    [Header("结局跳转")]
    public string targetSceneName = "HappyEnding"; // 可在 Inspector 设置场景名
    
    [Header("拾取效果")]
    public AudioClip pickupSound;
    public ParticleSystem pickupEffect;
    public float destroyDelay = 0.2f;
    
    [Header("悬停提示")]
    public string hoverText = "点击完成仪式";
    
    private bool canInteract = true; // 改名为更通用的交互标志
    
    void Start()
    {
        // 自动添加碰撞器（必须是非 Trigger 的 Collider 才能响应 OnMouseDown）
        if (GetComponent<Collider>() == null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = false; // ⚠️ 必须为 false！OnMouseDown 需要物理碰撞体
        }
    }
    
    void OnMouseEnter()
    {
        Debug.Log($"🖱️ {hoverText}");
    }
    
    void OnMouseDown()
    {
        if (!canInteract) return;
        
        InteractWithPotion();
    }
    
    void InteractWithPotion()
    {
        Debug.Log("✅ 灵药被点击！即将跳转至结局场景...");
        
        // 播放拾取效果（保留反馈）
        PlayPickupEffects();
        
        // 标记为已交互，防止重复点击
        canInteract = false;
        
        // 延迟跳转，让效果播放完
        Invoke("LoadEndingScene", destroyDelay);
    }
    
    void PlayPickupEffects()
    {
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
        
        if (pickupEffect != null)
        {
            ParticleSystem effect = Instantiate(pickupEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, 2f);
        }
        
        StartCoroutine(PickupAnimation());
    }
    
    System.Collections.IEnumerator PickupAnimation()
    {
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
    
    void LoadEndingScene()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}