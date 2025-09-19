using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellularAutomata : MonoBehaviour
{
    [SerializeField] private Vector2Int size;
    private Cell[,] cells;
    private Cell[,] prevGen;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile borderTile;

    [SerializeField] private string currRulesBeingUsed = "ConwaysGameOfLife";

    [SerializeField] private Transform cameraPos;

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

        SetBorderTiles();

        SetCameraPos();
    }

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            SetCell();
        }

        if (Time.time > nextActionTime && isPlaying) {
            nextActionTime += period;
            print(Time.time);

            MakeNextGen();
        }

        UpdateTileMap();
    }

    void SetCell() {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int roundedPos = (Vector2Int) tilemap.WorldToCell(mousePos);

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

    void SetBorderTiles() {
        for(int x = -1; x < size.x + 1; x++) {
            for(int y = -1; y < size.y + 1; y++) {
                if(x < 0 || y < 0 || x >= size.x || y >= size.y) {
                    tilemap.SetTile(new(y,x), borderTile);
                }
            }
        }
    }

    void SetCameraPos() {
        cameraPos.position = new Vector3(size.x/2, size.y/2, -10);

        cameraPos.gameObject.GetComponent<Camera>().orthographicSize = Math.Max(size.x, size.y)/2 + 2;
    }

    void MakeNextGen() {
        prevGen = (Cell[,]) cells.Clone();

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
                print("(" + x + ", " + y + ")" + " : " + numOfLiveCells + " : " + stateOfCell);

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