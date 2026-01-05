using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class DialogManager4 : MonoBehaviour
{
    [Header("UI 容器")]
    public RectTransform scrollContent; // 拖入你的DialogueArea对象
    public Text templateText;           // 拖入你的TextTemplate对象

    [Header("动画参数")]
    public float scrollSpeed = 50f;      // 整体滚动速度 (像素/秒)
    public float fadeInOutTime = 0.5f;   // 淡入淡出时长
    public float delayBetweenLines = 1.5f; // 每句话生成的间隔时间
    public float lineHeight = 40f;       // 预估的每行文字高度 (像素)

    [Header("台词内容")]
    [TextArea(3, 10)]
    public string[] dialogLines;

    [Header("场景切换")]
    public string nextSceneName = "MainMenu";

    private bool isPlaying = false;
    private List<Text> activeTexts = new List<Text>(); // 用于存放所有活跃的文字对象
    private float contentHeight;

    void Start()
    {
        if (templateText == null || scrollContent == null)
        {
            Debug.LogError("❌ 请在 Inspector 中指定 templateText 和 scrollContent！");
            return;
        }
        templateText.gameObject.SetActive(false); // 隐藏模板

        // 记录“幕布”的高度，用于计算淡入淡出的位置
        contentHeight = scrollContent.rect.height;

        StartCoroutine(GenerateTextContinuously());
        StartCoroutine(ScrollAllTexts());
    }

    // 这个协程负责不断生成新的文字
    IEnumerator GenerateTextContinuously()
    {
        isPlaying = true;
        for (int i = 0; i < dialogLines.Length; i++)
        {
            // 1. 复制文字模板
            Text newText = Instantiate(templateText, scrollContent);
            newText.text = dialogLines[i];
            newText.gameObject.SetActive(true);

            // 2. 设置初始位置：在“幕布”的正中央
            RectTransform rt = newText.rectTransform;
            rt.anchoredPosition = new Vector2(0, 0);
            SetAlpha(newText, 0f); // 初始透明

            // 3. 将新文字加入列表，并执行淡入动画
            activeTexts.Add(newText);
            StartCoroutine(FadeTo(newText, 1f, fadeInOutTime));

            // 4. 等待一段时间再生成下一句
            yield return new WaitForSeconds(delayBetweenLines);
        }

        // 所有文字都生成完毕后，等待最后一句也滚动出屏幕
        yield return new WaitForSeconds(contentHeight / scrollSpeed + fadeInOutTime);
        
        // 全部播放完，跳转场景
        SceneManager.LoadScene(nextSceneName);
    }

    // 这个协程负责让所有文字一起向上滚动
    IEnumerator ScrollAllTexts()
    {
        while (isPlaying || activeTexts.Count > 0)
        {
            float deltaY = scrollSpeed * Time.deltaTime;

            // 使用 for 循环从后往前遍历，这样在移除元素时不会出错
            for (int i = activeTexts.Count - 1; i >= 0; i--)
            {
                Text text = activeTexts[i];
                RectTransform rt = text.rectTransform;

                // 向上移动
                rt.anchoredPosition += new Vector2(0, deltaY);

                // --- 淡出逻辑 ---
                float textCenterY = rt.anchoredPosition.y;

                // 1. 从底部淡入
                if (textCenterY < -contentHeight / 2 + lineHeight)
                {
                    SetAlpha(text, 0f);
                }
                else if (textCenterY < -contentHeight / 2 + lineHeight + fadeInOutTime * scrollSpeed)
                {
                    float fadeT = Mathf.InverseLerp(-contentHeight / 2 + lineHeight, -contentHeight / 2 + lineHeight + fadeInOutTime * scrollSpeed, textCenterY);
                    SetAlpha(text, fadeT);
                }

                // 2. 到顶部淡出
                if (textCenterY > contentHeight / 2 - lineHeight)
                {
                    float fadeT = Mathf.InverseLerp(contentHeight / 2 - lineHeight, contentHeight / 2, textCenterY);
                    SetAlpha(text, 1f - fadeT);
                }

                // 3. 如果文字完全移出屏幕顶部，则销毁它
                if (textCenterY > contentHeight / 2 + lineHeight)
                {
                    activeTexts.RemoveAt(i);
                    Destroy(text.gameObject);
                }
            }
            yield return null; // 等待下一帧
        }
    }
    
    // 设置文字透明度
    void SetAlpha(Text text, float alpha)
    {
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }

    // 淡入淡出动画
    IEnumerator FadeTo(Text text, float targetAlpha, float duration)
    {
        Color startColor = text.color;
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            SetAlpha(text, Mathf.Lerp(startColor.a, targetAlpha, t));
            yield return null;
        }
        SetAlpha(text, targetAlpha);
    }
}