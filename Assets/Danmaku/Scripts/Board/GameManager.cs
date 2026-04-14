using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    public DeckUIController BoardUIController;
    public BoardInputHandler InputHandler;
    public ShooterSpawner ShooterSpawner;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        BoardUIController.RegistReadyEvent(ReadyForBuild);
    }

    private void OnEnable()
    {
        InputHandler.OnCellClicked += SpawnShooter;
        InputHandler.OnMissClicked += InterruptSpawnShooter;
    }

    private void OnDisable()
    {
        InputHandler.OnCellClicked -= SpawnShooter;
        InputHandler.OnMissClicked -= InterruptSpawnShooter;
    }

    public void ReadyForBuild()
    {
        InputHandler.SetInputState(true);
    }

    public void SpawnShooter(Cell cell)
    {
        int id = BoardUIController.ReadySpawnID;
        ShooterSpawner.Spawn(cell.transform.position, id);
    }

    public void InterruptSpawnShooter()
    {
        BoardUIController.ResetSpawnID();
    }
}