using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DeckUIController : MonoBehaviour
{
    public int Card01 = 10001; //Radial
    public int Card02 = 10002; //Orbit

    public event Action BoardcastOfReadyBuild;

    [SerializeField] 
    private List<CellUIButton> boardUIButtons = new List<CellUIButton>();

    public int ReadySpawnID { get; private set; }

    private void Start()
    {
        ReadySpawnID = -1;
        for (int i = 0; i < boardUIButtons.Count; i++)
        {
            boardUIButtons[i].Init(1001 + i, OnClick);
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