// CauldronInteractable.cs
using UnityEngine;

public class CauldronInteractable : MonoBehaviour
{
    [Header("UI Reference")]
    public PotionConfigUI potionConfigUI;
    
    [Header("视觉效果")]
    public ParticleSystem steamEffect;
    public AudioClip interactSound;
    public Material highlightMaterial;
    
    private Renderer objectRenderer;
    private Material originalMaterial;
    private bool isHighlighted = false;
    
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }
        
        // 确保有Collider
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider>();
        }
        
        // 确保potionConfigUI已设置
        if (potionConfigUI == null)
        {
            potionConfigUI = FindObjectOfType<PotionConfigUI>(true);
        }
    }
    
    void OnMouseEnter()
    {
        // 鼠标悬停高亮
        if (objectRenderer != null && highlightMaterial != null)
        {
            objectRenderer.material = highlightMaterial;
            isHighlighted = true;
        }
    }
    
    void OnMouseExit()
    {
        // 取消高亮
        if (isHighlighted && objectRenderer != null && originalMaterial != null)
        {
            objectRenderer.material = originalMaterial;
            isHighlighted = false;
        }
    }
    
    void OnMouseDown()
    {
        // 点击打开魔药配置UI
        if (potionConfigUI != null)
        {
            potionConfigUI.OpenUI(this);
            
            // 播放音效
            if (interactSound != null)
            {
                AudioSource.PlayClipAtPoint(interactSound, transform.position);
            }
            
            // 播放蒸汽效果
            if (steamEffect != null)
            {
                steamEffect.Play();
            }
        }
        else
        {
            Debug.LogError("❌ 未设置PotionConfigUI！");
        }
    }
}