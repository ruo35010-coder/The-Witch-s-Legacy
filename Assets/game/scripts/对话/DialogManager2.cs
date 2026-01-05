using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 引入场景管理
using System.Collections;

public class DialogManager2: MonoBehaviour
{
    [Header("UI 组件引用")]
    public Image dialogBackgroundImage;
    public Text dialogText;
    public Button confirmButton;

    [Header("对话框设置")]
    public float fadeDuration = 1.5f; // 对话框图片浮现时长
    public float typingSpeed = 0.05f; // 打字间隔（秒），值越小打字越快

    [Header("台词内容")]
    [TextArea(3, 10)] // 在Inspector中提供一个更大的文本编辑框
    public string[] dialogLines; // 存放所有台词的数组

    [Header("场景切换")]
    public string nextSceneName = "MainMenu"; // 对话结束后要加载的场景

    private int currentLineIndex = 0; // 当前播放到第几句台词
    private bool isTyping = false; // 是否正在打字
    private bool isDialogComplete = false; // 是否所有对话都已播放完毕

    void Start()
    {
        // 1. 初始状态：隐藏所有UI元素
        SetAllElementsAlpha(0f);
        confirmButton.interactable = false; // 按钮不可点击

        // 2. 绑定按钮点击事件
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);

        // 3. 启动整个对话流程
        StartCoroutine(StartDialogSequence());
    }

    /// <summary>
    /// 整个对话流程的主协程
    /// </summary>
    IEnumerator StartDialogSequence()
    {
        // --- 阶段一：对话框图片淡入 ---
        yield return StartCoroutine(FadeIn(dialogBackgroundImage, fadeDuration));

        // --- 新增：恢复Text和Button的透明度 ---
        if (dialogText != null)
        {
            Color textColor = dialogText.color;
            textColor.a = 1f;
            dialogText.color = textColor;
        }
        if (confirmButton != null)
        {
            foreach (var img in confirmButton.GetComponentsInChildren<Image>())
            {
                Color c = img.color; c.a = 1f; img.color = c;
            }
            foreach (var txt in confirmButton.GetComponentsInChildren<Text>())
            {
                Color c = txt.color; c.a = 1f; txt.color = c;
            }
        }

        // --- 阶段二：开始播放第一句台词 ---
        StartCoroutine(TypeLine(dialogLines[currentLineIndex]));
    }

    /// <summary>
    /// 逐字打印单句台词的协程
    /// </summary>
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogText.text = ""; // 清空文本框

        foreach (char c in line.ToCharArray())
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        // 检查是否是最后一句台词
        if (currentLineIndex >= dialogLines.Length - 1)
        {
            isDialogComplete = true;
        }

        // 台词打印完毕，激活按钮
        confirmButton.interactable = true;
    }

    /// <summary>
    /// 确认按钮的点击事件
    /// </summary>
    void OnConfirmButtonClicked()
    {
        // 如果正在打字，则立即显示整句
        if (isTyping)
        {
            StopAllCoroutines(); // 停止打字协程
            dialogText.text = dialogLines[currentLineIndex]; // 显示完整句子
            isTyping = false;
            confirmButton.interactable = true; // 确保按钮可点击
            return;
        }

        // 如果对话已全部结束，则加载下一个场景
        if (isDialogComplete)
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        // 准备播放下一句台词
        currentLineIndex++;
        confirmButton.interactable = false; // 点击后禁用按钮
        StartCoroutine(TypeLine(dialogLines[currentLineIndex]));
    }

    /// <summary>
    /// 统一设置所有UI元素的透明度
    /// </summary>
    void SetAllElementsAlpha(float alpha)
    {
        // 设置背景图片
        if (dialogBackgroundImage != null)
        {
            Color imgColor = dialogBackgroundImage.color;
            imgColor.a = alpha;
            dialogBackgroundImage.color = imgColor;
        }

        // 设置文本
        if (dialogText != null)
        {
            Color textColor = dialogText.color;
            textColor.a = alpha;
            dialogText.color = textColor;
        }

        // 设置按钮及其子元素
        if (confirmButton != null)
        {
            foreach (var img in confirmButton.GetComponentsInChildren<Image>())
            {
                Color c = img.color; c.a = alpha; img.color = c;
            }
            foreach (var txt in confirmButton.GetComponentsInChildren<Text>())
            {
                Color c = txt.color; c.a = alpha; txt.color = c;
            }
        }
    }

    /// <summary>
    /// 淡入协程
    /// </summary>
    IEnumerator FadeIn(Image targetImage, float duration)
    {
        float elapsedTime = 0f;
        Color tempColor = targetImage.color;
        tempColor.a = 0f;
        targetImage.color = tempColor;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            tempColor.a = alpha;
            targetImage.color = tempColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        tempColor.a = 1f;
        targetImage.color = tempColor;
    }
}