using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShooterEnum
{
    Radial,
    Orbit
}

[System.Serializable]
public class ShooterBaseData
{
    public ShooterEnum ShooterType;
    public int ID;
}

[System.Serializable]
public class RadialShooterData : ShooterBaseData
{
    public int DirCount;
    public float FireRate;
    public float MoveSpeed;
    public float BulletLifeTime;
}

[System.Serializable]
public class OrbitShooterData : ShooterBaseData
{
    public int DirCount;
    public int ObjectCount;
    public float RotateSpeed;
    public float BulletLifeTime;
}

[CreateAssetMenu(fileName = "NewShooterDatabase", menuName = "Database/NewShooterDatabase")]
public class ShooterDatabase : ScriptableObject
{
    [SerializeReference]
    public List<ShooterBaseData> shooterDatas = new List<ShooterBaseData>();

    public bool TryGetShooterData(int id, out ShooterBaseData data)
    {
        data = null;

        foreach (var item in shooterDatas)
        {
            if (item.ID == id)
            {
                data = item;
                return true;
            }
        }
        return false;
    }
}
