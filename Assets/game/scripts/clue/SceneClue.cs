using System.Collections;  // 确保有这个命名空间
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneClue : MonoBehaviour, IPointerClickHandler
{
    [Header("线索数据")]
    public ClueData clueData;
    
    [Header("视觉反馈")]
    public Material highlightMaterial;
    public Material collectedMaterial;
    private Material originalMaterial;
    private Renderer objectRenderer;
    private Collider objectCollider;
    
    [Header("收集效果")]
    public GameObject collectEffect;
    public float fadeOutTime = 0.5f;
    
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        objectCollider = GetComponent<Collider>();
        
        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }
        
        UpdateVisualState();
    }
    
    void UpdateVisualState()
    {
        if (clueData == null) return;
        
        if (clueData.IsCollected())
        {
            DisableClue();
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        TryCollectClue();
    }
    
    void OnMouseDown()
    {
        TryCollectClue();
    }
    
    void TryCollectClue()
    {
        if (clueData == null || clueData.IsCollected()) return;
        
        if (ClueUIManager.Instance != null)
        {
            ClueUIManager.Instance.ShowClueDetail(clueData);
            HighlightTemporarily();
        }
    }
    
    public void CollectClue()
    {
        if (clueData == null || clueData.IsCollected()) return;
        
        Debug.Log($"收集线索: {clueData.clueName}");
        clueData.MarkAsCollected();
        
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }
        
        StartCoroutine(FadeOutAndDisable());
    }
    
    IEnumerator FadeOutAndDisable()  // 修正这里，去掉System.Collections.
    {
        if (objectCollider != null)
            objectCollider.enabled = false;
        
        if (objectRenderer != null)
        {
            float elapsedTime = 0f;
            Color originalColor = objectRenderer.material.color;
            
            while (elapsedTime < fadeOutTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutTime);
                
                Color newColor = originalColor;
                newColor.a = alpha;
                objectRenderer.material.color = newColor;
                
                yield return null;
            }
        }
        
        gameObject.SetActive(false);
    }
    
    void DisableClue()
    {
        if (objectCollider != null)
            objectCollider.enabled = false;
        
        if (objectRenderer != null && collectedMaterial != null)
        {
            objectRenderer.material = collectedMaterial;
        }
    }
    
    void HighlightTemporarily()
    {
        if (objectRenderer != null && highlightMaterial != null)
        {
            objectRenderer.material = highlightMaterial;
            Invoke("ResetMaterial", 0.5f);
        }
    }
    
    void ResetMaterial()
    {
        if (objectRenderer != null && originalMaterial != null)
        {
            objectRenderer.material = originalMaterial;
        }
    }
    
    void OnMouseEnter()
    {
        if (clueData != null && !clueData.IsCollected() && objectRenderer != null && highlightMaterial != null)
        {
            objectRenderer.material = highlightMaterial;
        }
    }
    
    void OnMouseExit()
    {
        if (clueData != null && !clueData.IsCollected() && objectRenderer != null && originalMaterial != null)
        {
            objectRenderer.material = originalMaterial;
        }
    }
    
    public void ResetClue()
    {
        if (clueData != null)
        {
            clueData.ResetForNewGame();
        }
        
        gameObject.SetActive(true);
        
        if (objectRenderer != null)
        {
            objectRenderer.material = originalMaterial;
            objectRenderer.material.color = Color.white;
        }
        
        if (objectCollider != null)
            objectCollider.enabled = true;
    }
}