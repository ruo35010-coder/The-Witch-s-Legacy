using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class TelescopeController : MonoBehaviour
{
    [Header("摄像机设置")]
    [Tooltip("主摄像机")]
    public Camera mainCamera;
    
    [Tooltip("望远镜专用摄像机")]
    public Camera telescopeCamera;
    
    [Header("望远镜参数")]
    [Tooltip("望远镜的视野范围（FOV），越小放大倍数越高")]
    public float telescopeFOV = 15f;
    
    [Tooltip("正常视野范围")]
    public float normalFOV = 60f;
    
    [Tooltip("进入/退出望远镜的平滑过渡时间")]
    public float transitionTime = 0.5f;
    
    [Header("鼠标设置")]
    [Tooltip("鼠标灵敏度")]
    public float mouseSensitivity = 2f;
    
    [Tooltip("望远镜最大上下旋转角度")]
    public float maxLookAngle = 80f;
    
    [Header("音效")]
    [Tooltip("使用望远镜时的音效")]
    public AudioClip useTelescopeSound;
    
    [Tooltip("退出望远镜时的音效")]
    public AudioClip exitTelescopeSound;
    
    [Header("望远镜UI系统")]
    [Tooltip("望远镜的主UI画布")]
    public Canvas telescopeCanvas;
    
    // 私有变量
    private bool isUsingTelescope = false;
    private AudioSource audioSource;
    private float currentTransitionTime = 0f;
    private float currentTelescopeXRotation = 0f;
    private Vector3 originalTelescopeRotation;
    private Vector2 lastMousePosition;
    
    void Start()
    {
        // 初始化摄像机状态
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (telescopeCamera != null)
        {
            telescopeCamera.enabled = false;
            telescopeCamera.fieldOfView = normalFOV; // 初始化为正常FOV
            originalTelescopeRotation = telescopeCamera.transform.localEulerAngles;
        }
        
        // 获取或添加AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // 初始化UI - 禁用望远镜UI
        if (telescopeCanvas != null)
        {
            telescopeCanvas.gameObject.SetActive(false);
        }
        
        // 确保鼠标可见
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // 确保物体有碰撞体
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
            Debug.LogWarning("TelescopeController: 已自动添加BoxCollider组件");
        }
    }
    
    void Update()
    {
        // 处理进入望远镜的逻辑
        if (Input.GetMouseButtonDown(0) && !isUsingTelescope)
        {
            TryEnterTelescope();
        }
        
        // 处理退出望远镜的逻辑
        if (isUsingTelescope && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)))
        {
            ExitTelescope();
        }
        
        // 处理望远镜内的视角控制
        if (isUsingTelescope)
        {
            ControlTelescopeView();
        }
        
        // 处理过渡动画
        HandleTransition();
    }
    
    void TryEnterTelescope()
    {
        // 从主摄像机发射射线
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // 检测是否点击了望远镜
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                EnterTelescope();
            }
        }
    }
    
    void EnterTelescope()
    {
        if (telescopeCamera == null)
        {
            Debug.LogError("TelescopeController: 望远镜摄像机未设置！");
            return;
        }
        
        isUsingTelescope = true;
        currentTransitionTime = 0f;
        
        // 保存当前鼠标位置
        lastMousePosition = Input.mousePosition;
        
        // 切换摄像机
        mainCamera.enabled = false;
        telescopeCamera.enabled = true;
        
        // 确保鼠标可见且不锁定
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // 启用望远镜UI
        if (telescopeCanvas != null)
        {
            telescopeCanvas.gameObject.SetActive(true);
        }
        
        // 播放音效
        if (useTelescopeSound != null)
        {
            audioSource.PlayOneShot(useTelescopeSound);
        }
        
        // 重置望远镜旋转
        currentTelescopeXRotation = 0f;
        telescopeCamera.transform.localEulerAngles = originalTelescopeRotation;
        
        Debug.Log("进入望远镜模式 - 鼠标可见");
    }
    
    void ExitTelescope()
    {
        isUsingTelescope = false;
        currentTransitionTime = 0f;
        
        // 切换回主摄像机
        telescopeCamera.enabled = false;
        mainCamera.enabled = true;
        
        // 确保鼠标可见
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // 禁用望远镜UI
        if (telescopeCanvas != null)
        {
            telescopeCanvas.gameObject.SetActive(false);
        }
        
        // 播放音效
        if (exitTelescopeSound != null)
        {
            audioSource.PlayOneShot(exitTelescopeSound);
        }
        
        Debug.Log("退出望远镜模式");
    }
    
    void ControlTelescopeView()
    {
        if (telescopeCamera == null) return;
        
        // 鼠标拖拽控制望远镜视角
        if (Input.GetMouseButton(0))
        {
            // 获取鼠标移动增量
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            
            // 水平旋转（Y轴）
            telescopeCamera.transform.Rotate(Vector3.up, mouseX, Space.World);
            
            // 垂直旋转（X轴），限制角度
            currentTelescopeXRotation -= mouseY;
            currentTelescopeXRotation = Mathf.Clamp(currentTelescopeXRotation, -maxLookAngle, maxLookAngle);
            
            // 应用垂直旋转
            Vector3 currentRotation = telescopeCamera.transform.localEulerAngles;
            currentRotation.x = originalTelescopeRotation.x + currentTelescopeXRotation;
            telescopeCamera.transform.localEulerAngles = currentRotation;
        }
        
        // 滚轮缩放 - 调整FOV
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            telescopeCamera.fieldOfView = Mathf.Clamp(
                telescopeCamera.fieldOfView - scroll * 10f, 
                5f, 
                60f
            );
            
            // 更新UI显示当前放大倍数（如果UI需要）
            UpdateZoomDisplay();
        }
    }
    
    void UpdateZoomDisplay()
    {
        // 如果需要显示放大倍数，可以在这里添加代码
        // 例如：zoomText.text = $"放大: {GetZoomLevel():F1}x";
    }
    
    float GetZoomLevel()
    {
        // 计算放大倍数（相对于正常视野）
        return normalFOV / telescopeCamera.fieldOfView;
    }
    
    void HandleTransition()
    {
        if (currentTransitionTime < transitionTime && telescopeCamera != null)
        {
            currentTransitionTime += Time.deltaTime;
            float t = Mathf.Clamp01(currentTransitionTime / transitionTime);
            
            if (isUsingTelescope)
            {
                // 平滑过渡到望远镜FOV
                telescopeCamera.fieldOfView = Mathf.Lerp(normalFOV, telescopeFOV, t);
            }
            else
            {
                // 平滑过渡回正常FOV
                mainCamera.fieldOfView = Mathf.Lerp(telescopeFOV, normalFOV, t);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Gizmos.DrawWireCube(transform.position, collider.bounds.size);
        }
    }
    
    void OnMouseEnter()
    {
        // 鼠标悬停时可以做些效果，比如高亮望远镜
        Debug.Log("鼠标悬停在望远镜上");
    }
    
    void OnMouseExit()
    {
        Debug.Log("鼠标离开望远镜");
    }
    
    // 公开方法：强制进入/退出望远镜模式
    public void ToggleTelescope()
    {
        if (isUsingTelescope)
        {
            ExitTelescope();
        }
        else
        {
            EnterTelescope();
        }
    }
    
    // 检查是否正在使用望远镜
    public bool IsUsingTelescope()
    {
        return isUsingTelescope;
    }
    
    // 设置望远镜FOV
    public void SetTelescopeFOV(float fov)
    {
        telescopeFOV = Mathf.Clamp(fov, 1f, 100f);
        if (isUsingTelescope && telescopeCamera != null)
        {
            telescopeCamera.fieldOfView = telescopeFOV;
        }
    }
}