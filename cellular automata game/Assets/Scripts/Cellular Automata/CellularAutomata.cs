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

    [SerializeField] private Transform cameraPos;

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

        MakeNextGen();
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

        
    }

    private bool ConwaysGameOfLife(int numOfLiveCells) {
        return true;
    }
}