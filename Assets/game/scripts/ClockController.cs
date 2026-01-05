using UnityEngine;
using TMPro;

public class ClockController : MonoBehaviour
{
    [Header("UI References")]
    public Canvas clockCanvas;
    public TMP_InputField hourInput;
    public TMP_InputField minuteInput;
    public UnityEngine.UI.Button confirmButton;
    public UnityEngine.UI.Button exitButton;

    [Header("Correct Time (24-hour)")]
    public int correctHour = 15;
    public int correctMinute = 45;

    [Header("Clue Prefab to Spawn")]
    public GameObject cluePrefab;           // 拖入 Project 中的 Prefab
    public Transform spawnPoint;            // 可选生成点

    [Header("Optional Audio")]
    public AudioClip openSound;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip closeSound;
    public AudioClip appearSound;

    private AudioSource audioSource;
    private Collider clockCollider;         // 👈 用于后续禁用点击
    private bool isClockOpen = false;
    private bool hasBeenSolved = false;     // 👈 关键：是否已成功

    void Start()
    {
        if (!TryGetComponent(out audioSource))
            audioSource = gameObject.AddComponent<AudioSource>();

        // 缓存 Collider（用于禁用点击）
        clockCollider = GetComponent<Collider>();
        if (clockCollider == null)
        {
            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            clockCollider = col;
        }

        if (clockCanvas != null)
            clockCanvas.gameObject.SetActive(false);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmClick);
        if (exitButton != null)
            exitButton.onClick.AddListener(CloseClock);
    }

    void OnMouseDown()
    {
        // 🔒 如果已经解过，不再响应点击
        if (hasBeenSolved)
        {
            Debug.Log("[Clock] ⛔ 已完成，不再响应点击");
            return;
        }

        if (!isClockOpen)
            OpenClock();
    }

    public void OpenClock()
    {
        isClockOpen = true;
        if (clockCanvas != null)
            clockCanvas.gameObject.SetActive(true);
        if (hourInput != null)
        {
            hourInput.text = "";
            hourInput.ActivateInputField();
        }
        if (minuteInput != null)
            minuteInput.text = "";
        PlaySound(openSound);
    }

    public void CloseClock()
    {
        isClockOpen = false;
        if (clockCanvas != null)
            clockCanvas.gameObject.SetActive(false);
        PlaySound(closeSound);
    }

    public void OnConfirmClick()
    {
        string h = (hourInput?.text ?? "").Trim();
        string m = (minuteInput?.text ?? "").Trim();

        if (!int.TryParse(h, out int hour) || !int.TryParse(m, out int minute))
        {
            PlaySound(wrongSound);
            return;
        }

        if (hour == correctHour && minute == correctMinute)
        {
            Debug.Log("[Clock] ✅ 时间正确！生成线索并锁定钟表...");
            
            SpawnClue();
            PlaySound(correctSound);
            CloseClock();

            // 🔒 核心：标记为已完成，并禁用点击
            hasBeenSolved = true;

            // 可选：禁用 Collider 彻底阻止 OnMouseDown
            if (clockCollider != null)
                clockCollider.enabled = false;

            // 可选：隐藏钟表（或改变外观）
            // GetComponent<Renderer>()?.material.color = Color.gray;
        }
        else
        {
            PlaySound(wrongSound);
        }
    }

    void SpawnClue()
    {
        if (cluePrefab == null)
        {
            Debug.LogError("[Clock] ❌ Clue Prefab 未赋值！", this);
            return;
        }

        Vector3 position = spawnPoint != null 
            ? spawnPoint.position 
            : Camera.main != null 
                ? Camera.main.transform.position + Camera.main.transform.forward * 2f + Vector3.up * 0.8f
                : transform.position + Vector3.up * 2f;

        Instantiate(cluePrefab, position, Quaternion.identity);
        PlaySound(appearSound);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    void Update()
    {
        if (!isClockOpen || hasBeenSolved) return;

        if (Input.GetKeyDown(KeyCode.Escape))
            CloseClock();
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            OnConfirmClick();
    }
}