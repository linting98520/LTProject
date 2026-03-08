using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListTest : MonoBehaviour
{
    public int Count = 100000;
    public int TargetTimes = 100;
    public int CurrentTimes = 0;

    public bool isForBool = false;
    public bool isForEachBool = false;
    public bool isFindEachBool = false;

    private List<int>NumList = new List<int>();
    System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();

    private void Start()
    {
        for (int i = 0; i < Count; i++)
        {
            NumList.Add(i);
        }
        NumList.Add(Count);
    }

    public void InitStopWatch()
    {
        Stopwatch.Restart();
    }

    public void EndStopWatch(string funcName)
    {
        Stopwatch.Stop();
        Debug.Log($"{funcName} => {Stopwatch.ElapsedMilliseconds}, {Stopwatch.ElapsedTicks}");
    }

    private void Update()
    {
        if (isForBool)
        {
            if (!Stopwatch.IsRunning)
            {
                InitStopWatch();
            }
            ForProcess();
        }

        if (isForEachBool)
        {
            if (!Stopwatch.IsRunning)
            {
                InitStopWatch();
            }
            ForEachProcess();
        }

        if (isFindEachBool)
        {
            if (!Stopwatch.IsRunning)
            {
                InitStopWatch();
            }
            FindProcess();
        }
    }

    public void ForProcess()
    {
        for (int i = 0; i < NumList.Count; i++)
        {
            if (NumList[i] == Count)
            {
                CurrentTimes += 1;
                if(CurrentTimes >= TargetTimes)
                {
                    EndStopWatch("For");
                    isForBool = false;
                }
            }
        }
    }

    public void ForEachProcess()
    {
        NumList.ForEach((int index) =>
        {
            if (index == Count)
            {
                CurrentTimes += 1;
                if (CurrentTimes >= TargetTimes)
                {
                    EndStopWatch("ForEach");
                    isForEachBool = false;
                }
            }
        });
    }

    public void FindProcess()
    {
        int tmp = NumList.Find(T => T == Count);
        if(tmp == Count)
        {
            CurrentTimes += 1;
            if (CurrentTimes >= TargetTimes)
            {
                EndStopWatch("Find");
                isFindEachBool = false;
            }
        }
    }
}
