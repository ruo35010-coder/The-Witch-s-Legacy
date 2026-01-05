using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DialogManager : MonoBehaviour
{
    [Header("UI 组件引用")]
    public Image dialogBackgroundImage;
    public Text dialogText;
    public Button fullScreenConfirmButton; // 全屏继续按钮
    public Button optionButtonA;
    public Button optionButtonB;
    public Text optionAText;
    public Text optionBText;
    public Image nameBox; // 人物名字框背景
    public Text nameText; // 人物名字文本

    [Header("对话框设置")]
    public float fadeDuration = 0.5f;
    public float typingSpeed = 0.05f;

    [Header("台词内容")]
    [TextArea(3, 10)]
    public string[] dialogLines;
    public string speakerName = "女巫"; // 人物名字

    [Header("选项内容")]
    [TextArea(1, 2)]
    public string optionATextContent = "立即喝下";
    [TextArea(1, 2)]
    public string optionBTextContent = "谨慎后退";

    [Header("选项触发的操作")]
    public string badEndingSceneName = "BadEnding";
    public GameObject secondDialogObj; // 拖入 Dialog2

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isInChoicePhase = false;
    private NPCDialogueTrigger triggerRef;

    // 注意：不再在 Start() 中 SetActive(false)

    public void StartDialog(NPCDialogueTrigger trigger)
    {
        Debug.Log("第一段对话已激活");
        gameObject.SetActive(true);
        currentLineIndex = 0;
        isInChoicePhase = false;
        triggerRef = trigger;

        // 绑定全屏按钮事件
        fullScreenConfirmButton.onClick.RemoveAllListeners();
        fullScreenConfirmButton.onClick.AddListener(OnConfirmButtonClicked);
        optionButtonA.onClick.RemoveAllListeners();
        optionButtonA.onClick.AddListener(() => OnOptionSelected(0));
        optionButtonB.onClick.RemoveAllListeners();
        optionButtonB.onClick.AddListener(() => OnOptionSelected(1));

        StartCoroutine(StartDialogSequence());
    }

    // --- 这是修正后的核心协程 ---
    IEnumerator StartDialogSequence()
    {
        // 1. 设置所有UI元素的初始状态（都隐藏）
        dialogBackgroundImage.gameObject.SetActive(false);
        dialogText.gameObject.SetActive(false);
        fullScreenConfirmButton.gameObject.SetActive(false);
        optionButtonA.gameObject.SetActive(false);
        optionButtonB.gameObject.SetActive(false);
        nameBox.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);

        // 2. 显示需要淡入的元素（背景和名字框），并设置为透明
        dialogBackgroundImage.gameObject.SetActive(true);
        nameBox.gameObject.SetActive(true);
        nameText.gameObject.SetActive(true);
        nameText.text = speakerName;
        
        SetAlpha(dialogBackgroundImage, 0f);
        SetAlpha(nameBox, 0f);
        SetAlpha(nameText, 0f);

        // 3. 执行淡入动画
        yield return StartCoroutine(FadeIn(dialogBackgroundImage, fadeDuration));
        yield return StartCoroutine(FadeIn(nameBox, fadeDuration));
        SetAlpha(nameText, 1f); // 名字文本不需要淡入，直接显示

        // 4. 显示文本，并激活全屏按钮以接收点击
        dialogText.gameObject.SetActive(true);
        fullScreenConfirmButton.gameObject.SetActive(true);

        fullScreenConfirmButton.interactable = false;
        StartCoroutine(TypeLine(dialogLines[currentLineIndex]));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (char c in line.ToCharArray())
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        fullScreenConfirmButton.interactable = true;
    }

    void OnConfirmButtonClicked()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogText.text = dialogLines[currentLineIndex];
            isTyping = false;
            fullScreenConfirmButton.interactable = true;
            return;
        }

        currentLineIndex++;
        if (currentLineIndex < dialogLines.Length)
        {
            fullScreenConfirmButton.interactable = false;
            StartCoroutine(TypeLine(dialogLines[currentLineIndex]));
        }
        else
        {
            TransitionToChoicePhase();
        }
    }

    void TransitionToChoicePhase()
    {
        isInChoicePhase = true;
        // 隐藏对话相关元素
        dialogBackgroundImage.gameObject.SetActive(false);
        dialogText.gameObject.SetActive(false);
        fullScreenConfirmButton.gameObject.SetActive(false); // 禁用全屏按钮
        nameBox.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);

        // 显示选项按钮
        optionButtonA.gameObject.SetActive(true);
        optionButtonB.gameObject.SetActive(true);

        optionAText.text = optionATextContent;
        optionBText.text = optionBTextContent;
    }

    void OnOptionSelected(int choiceIndex)
    {
        if (!isInChoicePhase) return;
        isInChoicePhase = false;

        optionButtonA.gameObject.SetActive(false);
        optionButtonB.gameObject.SetActive(false);

        if (choiceIndex == 0)
        {
            SceneManager.LoadScene(badEndingSceneName);
        }
        else if (choiceIndex == 1)
        {
            Debug.Log("选项B被选中，准备启动第二段对话");

            if (secondDialogObj == null)
            {
                Debug.LogError("secondDialogObj 未赋值！");
                FinishAndNotify();
                return;
            }

            DialogManager3 secondDialog = secondDialogObj.GetComponent<DialogManager3>();
            if (secondDialog == null)
            {
                Debug.LogError("Dialog2 上缺少 DialogManager3 脚本！");
                FinishAndNotify();
                return;
            }

            gameObject.SetActive(false); // 立即隐藏自己
            secondDialog.StartDialog();  // 启动第二段

            FinishAndNotify();
        }
    }

    void FinishAndNotify()
    {
        if (triggerRef != null)
        {
            triggerRef.OnDialogueFinished();
        }
    }

    #region 辅助方法
    void SetAlpha(Graphic graphic, float alpha)
    {
        if (graphic == null) return;
        Color c = graphic.color;
        c.a = alpha;
        graphic.color = c;
    }

    IEnumerator FadeIn(Image targetImage, float duration)
    {
        float elapsedTime = 0f;
        Color tempColor = targetImage.color;
        tempColor.a = 0f;
        targetImage.color = tempColor;

        while (elapsedTime < duration)
        {
            tempColor.a = Mathf.Clamp01(elapsedTime / duration);
            targetImage.color = tempColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        tempColor.a = 1f;
        targetImage.color = tempColor;
    }
    #endregion
}