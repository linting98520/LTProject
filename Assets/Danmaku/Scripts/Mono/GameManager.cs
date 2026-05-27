using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    public LinkBrokenDispatcher BrokenDispatcher;
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

        LoadPlayer loadPlayer = new LoadPlayer(LoadPlayerPath, null);
        await loadPlayer.ExecuteAsync();
        GameObject playerObj = Instantiate(loadPlayer.Asset.gameObject);
        playerObj.transform.position = new Vector3(0, 8, 0);
    }

    private void OnEnable()
    {
        InputHandler.OnCellClicked += SpawnShooter;
        InputHandler.OnMissClicked += InterruptSpawnShooter;
        BrokenDispatcher.Register(LinkType.BuildingCell, DestroyShooter);
    }

    private void OnDisable()
    {
        InputHandler.OnCellClicked -= SpawnShooter;
        InputHandler.OnMissClicked -= InterruptSpawnShooter;
        BrokenDispatcher.Unregister(LinkType.BuildingCell, DestroyShooter);
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
            bool isbuild = ShooterSpawner.TryToSpawn(cell.ID, cell.transform.position, id);
            cell.SetBuildingState(isbuild);
            Debug.Log($"Build=> Cell ID = {cell.ID}");
        }
    }

    public void DestroyShooter(int id)
    {
        Cell cell = BoardManager.GetCell(id);
        if (cell != null)
        {
            cell.SetBuildingState(false);
            Debug.Log($"Destroy=> Cell ID = {cell.ID}");
        }
    }

    public void InterruptSpawnShooter()
    {
        BoardUIController.ResetSpawnID();
    }
}