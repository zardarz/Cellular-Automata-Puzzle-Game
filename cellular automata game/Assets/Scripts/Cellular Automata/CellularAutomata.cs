using UnityEngine;
using UnityEngine.Tilemaps;

public class CellularAutomata : MonoBehaviour
{
    [SerializeField] private Vector2Int size;
    private Cell[,] cells;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap buildingAreaTilemap;
    [SerializeField] private Tile aliveTile;

    [SerializeField] private string currRulesBeingUsed = "ConwaysGameOfLife";

    [SerializeField] private bool isPlaying = false;

    private float nextActionTime = 0.0f;
    public float period = 1f; // 10 times a second

    void Start() {
        cells = new Cell[size.y, size.x];

        for(int x = 0; x < size.x; x++) {
            for(int y = 0; y < size.y; y++) {
                cells[y, x] = new Cell(new(x,y));
            }
        }
    }

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            SetCell();
        }

        nextActionTime -= Time.deltaTime;

        if (nextActionTime <= 0 && isPlaying) {
            nextActionTime = period;

            MakeNextGen();
        }

        UpdateTileMap();
    }

    void SetCell() {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int roundedPos = (Vector2Int) tilemap.WorldToCell(mousePos);

        if(buildingAreaTilemap.GetTile((Vector3Int) roundedPos) == null || isPlaying == true) return;

        Cell clickedCell = cells[roundedPos.x, roundedPos.y];
        clickedCell.SetIsAlive(!clickedCell.GetIsAlive());
    }

    void UpdateTileMap() {
        for(int x = 0; x < size.x; x++) {
            for(int y = 0; y < size.y; y++) {
                Cell currCell = cells[y,x];

                if(currCell.GetIsAlive()) {
                    tilemap.SetTile(new(y,x), aliveTile);
                } else {
                    tilemap.SetTile(new(y,x), null);
                }
            }
        }
    }

    void MakeNextGen() {

        for(int x = 0; x < size.x; x++) {
            for(int y = 0; y < size.y; y++) {
                int numOfLiveCells = 0;

                if(tilemap.GetTile(new(x-1,y-1)) == aliveTile) numOfLiveCells++;
                if(tilemap.GetTile(new(x,y-1)) == aliveTile) numOfLiveCells++;
                if(tilemap.GetTile(new(x+1,y-1)) == aliveTile) numOfLiveCells++;

                if(tilemap.GetTile(new(x-1,y)) == aliveTile) numOfLiveCells++;
                if(tilemap.GetTile(new(x+1,y)) == aliveTile) numOfLiveCells++;

                if(tilemap.GetTile(new(x-1,y+1)) == aliveTile) numOfLiveCells++;
                if(tilemap.GetTile(new(x,y+1)) == aliveTile) numOfLiveCells++;
                if(tilemap.GetTile(new(x+1,y+1)) == aliveTile) numOfLiveCells++;

                string stateOfCell = RunRuleset(numOfLiveCells);

                if(stateOfCell.Equals("die")) {
                    cells[x,y].SetIsAlive(false);
                } else if (stateOfCell.Equals("born")) {
                    cells[x,y].SetIsAlive(true);
                }

            }
        }
    }

    private string RunRuleset(int numOfLiveCells) {
        if(currRulesBeingUsed.Equals("ConwaysGameOfLife")) {
            return ConwaysGameOfLife(numOfLiveCells);
        } else {
            return "stay";
        }
    }

    private string ConwaysGameOfLife(int numOfLiveCells) {
        if(numOfLiveCells == 2) {
            return "stay";
        } else if(numOfLiveCells == 3) {
            return "born";
        } else {
            return "die";
        }
    }

    public void Step() {
        MakeNextGen();
    }

    public void StartPlaying() {
        isPlaying = true;
    }

    public void StopPlaying() {
        isPlaying = false;
    }
}