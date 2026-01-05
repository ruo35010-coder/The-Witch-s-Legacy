using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeInStart : MonoBehaviour
{
    [Header("UI 元素")]
    public Image titleImage;
    public Image touchImage;
    public Button startButton; // 用于捕获初始点击的全屏按钮
    public Button singlePlayerButton; // 单人模式按钮
    public Button multiPlayerButton;  // 多人模式按钮
    public Image blackFadePanel;      // 黑屏面板

    [Header("动画设置")]
    public float titleFadeDuration = 2.0f;
    public float touchFadeDuration = 1.5f;
    public float delayBetweenFades = 1.0f;
    public float fadeOutDuration = 1.0f;
    public float blackFadeInDuration = 1.0f;

    [Header("场景设置")]
    public string singlePlayerSceneName = "MainMenu"; // 单人模式要进入的场景
    public string multiPlayerSceneName = "MultiPlayerLobby"; // 多人模式要进入的场景

    void Start()
    {
        // 初始状态：隐藏不需要的UI
        touchImage.gameObject.SetActive(false);
        singlePlayerButton.gameObject.SetActive(false);
        multiPlayerButton.gameObject.SetActive(false);
        
        // 初始化黑屏面板
        if (blackFadePanel != null)
        {
            Color c = blackFadePanel.color;
            c.a = 0f;
            blackFadePanel.color = c;
            blackFadePanel.raycastTarget = false;
        }

        // 绑定按钮点击事件
        startButton.onClick.AddListener(OnStartButtonClicked);
        singlePlayerButton.onClick.AddListener(OnSinglePlayerClicked);
        multiPlayerButton.onClick.AddListener(OnMultiPlayerClicked);

        StartCoroutine(PlayOpeningSequence());
    }

    IEnumerator PlayOpeningSequence()
    {
        yield return StartCoroutine(FadeIn(titleImage, titleFadeDuration));
        yield return new WaitForSeconds(delayBetweenFades);

        touchImage.gameObject.SetActive(true);
        yield return StartCoroutine(FadeIn(touchImage, touchFadeDuration));
        
        // 开场动画结束后，激活全屏按钮等待点击
        startButton.gameObject.SetActive(true);
        Debug.Log("开场动画完毕，等待玩家点击...");
    }

    // 当玩家点击全屏按钮时调用
    void OnStartButtonClicked()
    {
        // 禁用全屏按钮，防止重复点击
        startButton.gameObject.SetActive(false);
        // 启动过渡到选择界面的协程
        StartCoroutine(TransitionToSelectionScreen());
    }

    // 过渡到选择界面的协程
    IEnumerator TransitionToSelectionScreen()
    {
        Debug.Log("检测到点击，开始过渡到选择界面...");

        // 淡出 "Touch" 文字
        yield return StartCoroutine(FadeOut(touchImage, fadeOutDuration));
        touchImage.gameObject.SetActive(false);

        // 淡出标题文字
        yield return StartCoroutine(FadeOut(titleImage, fadeOutDuration));
        titleImage.gameObject.SetActive(false);

        // 显示单人/多人模式按钮
        singlePlayerButton.gameObject.SetActive(true);
        multiPlayerButton.gameObject.SetActive(true);
        Debug.Log("显示选择按钮，等待玩家选择模式...");
    }

    // 当玩家选择单人模式时调用
    void OnSinglePlayerClicked()
    {
        Debug.Log("选择了单人模式，准备进入场景: " + singlePlayerSceneName);
        // 禁用选择按钮
        singlePlayerButton.gameObject.SetActive(false);
        multiPlayerButton.gameObject.SetActive(false);
        // 启动黑屏并切换场景的协程
        StartCoroutine(FadeToBlackAndLoadScene(singlePlayerSceneName));
    }

    // 当玩家选择多人模式时调用
    void OnMultiPlayerClicked()
    {
        Debug.Log("选择了多人模式，准备进入场景: " + multiPlayerSceneName);
        // 禁用选择按钮
        singlePlayerButton.gameObject.SetActive(false);
        multiPlayerButton.gameObject.SetActive(false);
        // 启动黑屏并切换场景的协程
        StartCoroutine(FadeToBlackAndLoadScene(multiPlayerSceneName));
    }

    // 通用的“黑屏并切换场景”协程
    IEnumerator FadeToBlackAndLoadScene(string sceneName)
    {
        // 黑屏面板淡入
        blackFadePanel.raycastTarget = true;
        yield return StartCoroutine(FadeIn(blackFadePanel, blackFadeInDuration));

        // 加载指定场景
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeIn(Image targetImage, float duration)
    {
        Color tempColor = targetImage.color;
        tempColor.a = 0f;
        targetImage.color = tempColor;

        float elapsedTime = 0f;
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

    IEnumerator FadeOut(Image targetImage, float duration)
    {
        Color tempColor = targetImage.color;
        tempColor.a = 1f;
        targetImage.color = tempColor;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float alpha = Mathf.Clamp01(1f - (elapsedTime / duration));
            tempColor.a = alpha;
            targetImage.color = tempColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        tempColor.a = 0f;
        targetImage.color = tempColor;
    }
}