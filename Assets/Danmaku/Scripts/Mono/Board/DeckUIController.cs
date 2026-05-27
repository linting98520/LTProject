using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class DeckUIController : MonoBehaviour
{
    public int StartID = 1001;
    [ReadOnly] public int Card01 = 1001; //Radial
    [ReadOnly] public int Card02 = 1002; //Orbit
    [ReadOnly] public int Card03 = 1003; //Block

    public event Action BoardcastOfReadyBuild;

    [SerializeField] 
    private List<CellUIButton> boardUIButtons = new List<CellUIButton>();

    public int ReadySpawnID { get; private set; }

    private void Start()
    {
        ReadySpawnID = -1;
        for (int i = 0; i < boardUIButtons.Count; i++)
        {
            boardUIButtons[i].Init(StartID + i, OnClick);
        }
    }

    public void RegistReadyEvent(Action readyEvent)
    {
        BoardcastOfReadyBuild = readyEvent;
    }

    private void OnClick(int id)
    {
        ReadySpawnID = id;
        BoardcastOfReadyBuild?.Invoke();
    }

    public void ResetSpawnID()
    {
        ReadySpawnID = -1;
    }
}