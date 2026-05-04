using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    public BoardManager BoardManager;
    public DeckUIController BoardUIController;
    public BoardInputHandler InputHandler;
    public ShooterSpawner ShooterSpawner;

    public string ShooterDBPath;
    public string LoadPlayerPath;

    private void Awake()
    {
        _instance = this;
    }

    private async void Start()
    {
        await new LoadShooterDatabase(ShooterDBPath, ShooterSpawner.InjectShooterDB).ExecuteAsync();

        BoardUIController.RegistReadyEvent(ReadyForBuild);
        BoardManager.GenerateBoard();

        //await new LoadPlayer(LoadPlayerPath, null).ExecuteAsync();
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
        if (cell?.IsBuild == false)
        {
            bool isbuild = ShooterSpawner.TryToSpawn(cell.transform.position, id);
            cell.SetBuildingState(isbuild);
        }
    }

    public void InterruptSpawnShooter()
    {
        BoardUIController.ResetSpawnID();
    }
}