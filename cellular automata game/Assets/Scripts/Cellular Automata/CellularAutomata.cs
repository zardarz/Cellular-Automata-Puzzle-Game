using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellularAutomata : MonoBehaviour
{
    [SerializeField] private Vector2Int size;
    private Cell[,] cells;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap buildingAreaTilemap;
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile borderTile;

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

        int cellsBorn = 0;
        int cellsDie = 0;
        int cellsStay = 0;

        for(int x = 0; x < size.x; x++) {
            for(int y = 0; y < size.y; y++) {
                if(tilemap.GetTile(new(x,y,0)) == borderTile) continue;

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
                    if(cells[x,y].GetIsAlive())cellsDie++;
                    cells[x,y].SetIsAlive(false);
                } else if (stateOfCell.Equals("born")) {
                    if(!cells[x,y].GetIsAlive())cellsBorn++;
                    cells[x,y].SetIsAlive(true);
                } else {
                    cellsStay++;
                }

            }
        }

        int netCellGain = cellsBorn - cellsDie;
        print(cellsBorn + " : " + cellsDie);

        for(int i = 0; i < Mathf.Abs(netCellGain); i++) {
            if(netCellGain > 0) {
                SoundManager.Play("Cell Come To Life");
            } else {
                SoundManager.Play("Cell Die");
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