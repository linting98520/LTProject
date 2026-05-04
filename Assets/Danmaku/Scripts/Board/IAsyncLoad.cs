using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using System;
using UnityObject = UnityEngine.Object;

public interface IAsyncLoad<T> where T : UnityObject
{
    string ServiceName { get; }
    Task ExecuteAsync();
    T Asset { get; }
}

public abstract class ResourceAsyncLoad<T> : IAsyncLoad<T> where T : UnityObject
{
    public virtual string ServiceName => throw new System.NotImplementedException();

    public T Asset { get; protected set; }

    protected readonly string path;

    protected Action<T> injectAction;

    public ResourceAsyncLoad(string path, Action<T> action)
    {
        this.path = path;
        injectAction = action;
    }

    public virtual async Task ExecuteAsync()
    {
        var req = Resources.LoadAsync<T>(path);
        while (!req.isDone) await Task.Yield();

        Asset = req.asset as T;
        Debug.Log($"[{ServiceName}] Loading Successful");

        injectAction?.Invoke(Asset);
    }
}

public class LoadShooterDatabase : ResourceAsyncLoad<ShooterDatabase>
{
    public override string ServiceName => "ShooterDatabase";

    public LoadShooterDatabase(string path, Action<ShooterDatabase> action) : base(path, action) { }
}

public class LoadPlayer : ResourceAsyncLoad<PlayerController>
{
    public override string ServiceName => "LoadPlayer";
    public LoadPlayer(string path, Action<PlayerController> action) : base(path, action) { }
}
