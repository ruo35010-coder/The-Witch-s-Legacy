using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleGridsWithAnimation : MonoBehaviour
{
    [Header("拖拽 grids 父对象到这里")]
    public RectTransform gridsContainer; // UI 版本
    
    [Header("动画设置")]
    public float animationDuration = 0.3f;
    public bool startVisible = false; // false 表示初始隐藏
    
    [Header("按钮状态")]
    public Text stateText;
    
    private bool isGridsVisible;
    private Coroutine animationCoroutine;
    
    void Start()
    {
        isGridsVisible = startVisible;
        
        // 设置初始状态
        if (gridsContainer != null)
        {
            // 初始设为完全隐藏
            gridsContainer.localScale = new Vector3(1f, 0f, 1f);
            // 或者直接禁用
            // gridsContainer.gameObject.SetActive(false);
        }
        
        // 如果是按钮，添加点击事件
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ToggleWithAnimation);
        }
        
        UpdateButtonText();
    }
    
    public void ToggleWithAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        if (isGridsVisible)
        {
            animationCoroutine = StartCoroutine(HideAnimation());
        }
        else
        {
            animationCoroutine = StartCoroutine(ShowAnimation());
        }
        
        isGridsVisible = !isGridsVisible;
        UpdateButtonText();
    }
    
    IEnumerator HideAnimation()
    {
        // 确保对象是激活的
        if (!gridsContainer.gameObject.activeSelf)
            gridsContainer.gameObject.SetActive(true);
            
        float elapsed = 0f;
        Vector3 startScale = gridsContainer.localScale;
        Vector3 endScale = new Vector3(startScale.x, 0f, startScale.z);
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            gridsContainer.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        
        gridsContainer.localScale = endScale;
        // 可选：动画完成后禁用对象节省性能
        // gridsContainer.gameObject.SetActive(false);
    }
    
    IEnumerator ShowAnimation()
    {
        // 确保对象是激活的
        if (!gridsContainer.gameObject.activeSelf)
            gridsContainer.gameObject.SetActive(true);
        
        float elapsed = 0f;
        Vector3 startScale = gridsContainer.localScale;
        Vector3 endScale = new Vector3(startScale.x, 1f, startScale.z);
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            gridsContainer.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        
        gridsContainer.localScale = endScale;
    }
    
    void UpdateButtonText()
    {
        if (stateText != null)
        {
            stateText.text = isGridsVisible ? "收起" : "展开";
        }
    }
}