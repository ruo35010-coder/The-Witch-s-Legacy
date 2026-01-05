using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NextImageButton : MonoBehaviour
{
    [Header("UI组件")]
    public Image targetImage;              // 要切换的图片组件
    public Text pageText;                  // 页码显示文本（可选）
    
    [Header("图片列表")]
    public List<Sprite> images = new List<Sprite>();  // 要切换的图片列表
    
    [Header("按钮设置")]
    public string lastPageText = "完成";   // 最后一张时的按钮文字
    
    private int currentIndex = 0;
    private Button button;
    private Text buttonText;
    
    void Start()
    {
        // 获取按钮组件
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("这个脚本需要挂在Button上！");
            return;
        }
        
        // 获取按钮上的文本（如果有）
        buttonText = GetComponentInChildren<Text>();
        
        // 如果没有手动指定图片组件，尝试自动查找
        if (targetImage == null)
        {
            // 尝试查找常见的图片名称
            GameObject imageObj = GameObject.Find("ClueImage") ?? 
                                  GameObject.Find("Image") ?? 
                                  GameObject.Find("MainImage");
            
            if (imageObj != null) targetImage = imageObj.GetComponent<Image>();
        }
        
        // 设置按钮点击事件
        button.onClick.AddListener(OnClickNext);
        
        // 初始显示第一张图片
        ShowCurrentImage();
    }
    
    // 按钮点击事件
    void OnClickNext()
    {
        if (images.Count == 0 || targetImage == null) return;
        
        // 切换到下一张
        currentIndex++;
        
        // 如果超过最后一张
        if (currentIndex >= images.Count)
        {
            // 循环回到第一张
            currentIndex = 0;
            
            // 或者：如果是最后一张，执行特殊操作
            // OnLastImageReached();
            // return;
        }
        
        // 显示当前图片
        ShowCurrentImage();
    }
    
    // 显示当前图片
    void ShowCurrentImage()
    {
        if (currentIndex < images.Count && images[currentIndex] != null)
        {
            targetImage.sprite = images[currentIndex];
            targetImage.preserveAspect = true;
        }
        
        // 更新页码显示
        if (pageText != null)
        {
            pageText.text = $"{(currentIndex + 1)}/{images.Count}";
        }
        
        // 如果是最后一张，改变按钮文字
        if (buttonText != null && currentIndex == images.Count - 1)
        {
            buttonText.text = lastPageText;
        }
        else if (buttonText != null)
        {
            buttonText.text = "下一张";
        }
    }
    
    // 重置到第一张（如果需要）
    public void ResetToFirst()
    {
        currentIndex = 0;
        ShowCurrentImage();
    }
    
    // 添加图片到列表
    public void AddImage(Sprite sprite)
    {
        if (sprite != null)
        {
            images.Add(sprite);
        }
    }
    
    // 清空图片列表
    public void ClearImages()
    {
        images.Clear();
        currentIndex = 0;
    }
}