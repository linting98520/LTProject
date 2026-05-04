using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    public float MoveSpeed = 5f;

    [Header("座標軸設定")]
    [Tooltip("勾選代表在 XZ 平面移動（俯視角）；不勾代表 XY 平面（2D / 平台）")]
    public bool MoveOnXZ = true;

    private Rigidbody rb;
    private Vector3 inputDir;   // Update 讀輸入、FixedUpdate 套用

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. 讀輸入（Update 跑得快，輸入反應準）
        float h = 0f;
        float v = 0f;
        if (Input.GetKey(KeyCode.A)) h -= 1f;
        if (Input.GetKey(KeyCode.D)) h += 1f;
        if (Input.GetKey(KeyCode.W)) v += 1f;
        if (Input.GetKey(KeyCode.S)) v -= 1f;

        Vector3 dir = MoveOnXZ
            ? new Vector3(h, 0f, v)
            : new Vector3(h, v, 0f);

        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        inputDir = dir;
    }

    void FixedUpdate()
    {
        // 2. 套用速度（FixedUpdate 跟物理引擎同步，避免抖動）
        Vector3 horizontalVel = inputDir * MoveSpeed;

        // 保留 Y 速度（重力 / 跳躍），只改水平
        Vector3 currentVel = rb.velocity;
        if (MoveOnXZ)
            rb.velocity = new Vector3(horizontalVel.x, currentVel.y, horizontalVel.z);
        else
            rb.velocity = new Vector3(horizontalVel.x, horizontalVel.y, currentVel.z);
    }
}