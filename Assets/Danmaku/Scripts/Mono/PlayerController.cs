using MCPForUnity.Editor.Tools;
using Unity.Entities;
using Unity.IO.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerController : MonoBehaviour
{
    private EntityManager entityManager;
    private Entity bulletEntity;
    private Entity playerEntity;

    [Header("移動")]
    [Tooltip("移動速度（公尺/秒）")]
    public float MoveSpeed = 5f;

    [Header("旋轉")]
    [Tooltip("每秒最多轉幾度")]
    public float RotationSpeed = 720f;

    private Rigidbody rb;
    private Vector3 inputDir;

    public float BulletSpeed;
    public float BulletDamage;
    public float BulletFireRate;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var archetype = entityManager.CreateArchetype(typeof(FireRequest), typeof(ShooterState));
        playerEntity = entityManager.CreateEntity(archetype);

        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(SpawnRegistry));
        SpawnRegistry config = entityQuery.GetSingleton<SpawnRegistry>();
        bulletEntity = config.PlayerBulletEntity;
        entityManager.SetComponentData(playerEntity, new FireRequest
        {
            Prefab = bulletEntity,
            Speed = BulletSpeed,
            BulletDamage = BulletDamage,
            BulletLifetime = 10,
            ElapsedTime = 0,
            FireRate = BulletFireRate
        });
    }

    void Update()
    {
        float h = 0f, v = 0f;
        if (Input.GetKey(KeyCode.A)) h -= 1f;
        if (Input.GetKey(KeyCode.D)) h += 1f;
        if (Input.GetKey(KeyCode.W)) v += 1f;
        if (Input.GetKey(KeyCode.S)) v -= 1f;

        Vector3 dir = new Vector3(h, 0f, v);
        if (dir.sqrMagnitude > 1f) dir.Normalize();   // 對角線 normalize
        inputDir = dir;

        entityManager.SetComponentData(playerEntity, new ShooterState
        {
            Position = (float3)transform.position,
            Direction = (float3)transform.forward
        });
    }

    void FixedUpdate()
    {
        Vector3 horizontalVel = inputDir * MoveSpeed;
        Vector3 currentVel = rb.velocity;

        rb.velocity = new Vector3(
            horizontalVel.x,
            currentVel.y,
            horizontalVel.z
        );

        if (inputDir.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(inputDir, Vector3.up);
            Quaternion newRot = Quaternion.RotateTowards(
                rb.rotation,
                target,
                RotationSpeed * Time.fixedDeltaTime
            );
            rb.MoveRotation(newRot);
        }
    }
}