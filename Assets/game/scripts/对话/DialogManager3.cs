using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogManager3 : MonoBehaviour
{
    [Header("UI 组件引用")]
    public Image dialogBackgroundImage;
    public Text dialogText;
    public Button fullScreenConfirmButton; // 全屏继续按钮
    public Image nameBox; // 人物名字框背景
    public Text nameText; // 人物名字文本

    [Header("对话框设置")]
    public float fadeDuration = 0.5f;
    public float typingSpeed = 0.05f;

    [Header("台词内容")]
    [TextArea(3, 10)]
    public string[] dialogLines;
    public string speakerName = "女巫"; // 人物名字

    [Header("药方生成设置")]
    public GameObject prescriptionPrefab;      // 拖入药方 Prefab
    public Transform npcSpawnPoint;           // ← 拖入 NPC 对象（或其子物体，如空物体）

    private int currentLineIndex = 0;
    private bool isTyping = false;

    // 注意：不再使用 Start() 中的 SetActive(false)

    public void StartDialog()
    {
        Debug.Log("第二段对话已激活");
        gameObject.SetActive(true);
        currentLineIndex = 0;

        // 绑定全屏按钮事件
        fullScreenConfirmButton.onClick.RemoveAllListeners();
        fullScreenConfirmButton.onClick.AddListener(OnConfirmButtonClicked);

        StartCoroutine(StartDialogSequence());
    }

    // --- 这是修改后的核心协程 ---
    IEnumerator StartDialogSequence()
    {
        // 1. 设置所有UI元素的初始状态（都隐藏）
        dialogBackgroundImage.gameObject.SetActive(false);
        dialogText.gameObject.SetActive(false);
        fullScreenConfirmButton.gameObject.SetActive(false);
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
            // 对话结束，隐藏所有UI
            dialogBackgroundImage.gameObject.SetActive(false);
            dialogText.gameObject.SetActive(false);
            fullScreenConfirmButton.gameObject.SetActive(false);
            nameBox.gameObject.SetActive(false);
            nameText.gameObject.SetActive(false);

            // 生成药方
            SpawnPrescription();
        }
    }

    void SpawnPrescription()
    {
        if (prescriptionPrefab == null)
        {
            Debug.LogError("【错误】药方 Prefab 未赋值！请在 Inspector 中拖入药方 Prefab。");
            return;
        }

        if (npcSpawnPoint == null)
        {
            Debug.LogError("【错误】NPC 生成点未赋值！请在 Inspector 中将 NPC 对象拖到 'Npc Spawn Point' 字段。");
            return;
        }

        // 在指定 NPC 位置上方 0.5 米生成，避免嵌入地面
        Vector3 spawnPosition = npcSpawnPoint.position + Vector3.up * 0.5f;
        GameObject prescription = Instantiate(prescriptionPrefab, spawnPosition, Quaternion.identity);

        Debug.Log($"✅ 药方已从 {spawnPosition} 生成（基于 {npcSpawnPoint.name}）");
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