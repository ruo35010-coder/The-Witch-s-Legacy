using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

// 挂在 Player 根对象上（和 ThirdPersonCharacter 同级）
[RequireComponent(typeof(ThirdPersonCharacter))]
public class FirstPersonuserControl : MonoBehaviour
{
    public Transform cameraPivot; // 拖入 CameraPivot 对象
    public float mouseSensitivity = 2f;
    public bool invertY = false;
    public Vector2 verticalLookLimit = new Vector2(-80f, 80f);

    private ThirdPersonCharacter m_Character;
    private float cameraPitch = 0f;

    void Start()
    {
        m_Character = GetComponent<ThirdPersonCharacter>();
        if (cameraPivot == null)
        {
            Debug.LogError("请指定 CameraPivot！");
            enabled = false;
        }
    }

    void Update()
    {
        HandleLook();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleLook()
    {
        if (cameraPivot == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 水平旋转：只转 CameraPivot（不转 Player 根）
        cameraPivot.Rotate(Vector3.up * mouseX);

        // 垂直旋转：限制俯仰角
        cameraPitch -= (invertY ? -mouseY : mouseY);
        cameraPitch = Mathf.Clamp(cameraPitch, verticalLookLimit.x, verticalLookLimit.y);
        cameraPivot.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal"); // A/D
        float v = Input.GetAxis("Vertical");   // W/S
        bool crouch = Input.GetKey(KeyCode.C);
        bool jump = Input.GetButtonDown("Jump");

        // 第一人称：移动方向 = 摄像机水平朝向
        Vector3 forward = Vector3.Scale(cameraPivot.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 right = Vector3.Scale(cameraPivot.right, new Vector3(1, 0, 1)).normalized;
        Vector3 move = v * forward + h * right;

        // 重要：将 move 转换为世界坐标（ThirdPersonCharacter 需要世界方向）
        // 注意：这里不需要 InverseTransformDirection，因为 move 已是世界向量

        m_Character.Move(move, crouch, jump);
    }
}