using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellularAutomata : MonoBehaviour
{
    [SerializeField] private Vector2Int size;
    private Cell[,] cells;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap buildingAreaTilemap;
    [SerializeField] private Tilemap areaNeededToWinTileMap;
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile borderTile;

    [SerializeField] private int amountOfTilesNeededToBeFilled;
    private int tilesFilled;

    [SerializeField] private GameObject canvas;

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

        if(amountOfTilesNeededToBeFilled != -1) {
            amountOfTilesNeededToBeFilled = GetAreaNeededToBeFilledCount();
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
            CheckIfWon();
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
                if(tilemap.GetTile(new(x,y,0)) == borderTile) continue;

                Cell currCell = cells[x,y];

                if(currCell.GetIsAlive()) {
                    tilemap.SetTile(new(x,y), aliveTile);
                } else {
                    tilemap.SetTile(new(x,y), null);
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
                    if(cells[x,y].GetIsAlive()) {
                        cellsDie++;

                        if(areaNeededToWinTileMap.HasTile(new(x,y,0))) {
                            tilesFilled--;
                        }
                    }
                    cells[x,y].SetIsAlive(false);
                } else if (stateOfCell.Equals("born")) {
                    if(!cells[x,y].GetIsAlive()) {
                        cellsBorn++;

                        if(areaNeededToWinTileMap.HasTile(new(x,y,0))) {
                            tilesFilled++;
                        }
                    }

                    cells[x,y].SetIsAlive(true);
                } else {
                    cellsStay++;
                }

            }
        }

        int netCellGain = cellsBorn - cellsDie;

        for(int i = 0; i < Mathf.Abs(netCellGain); i++) {
            if(netCellGain > 0) {
                SoundManager.Play("Cell Come To Life");
            } else {
                SoundManager.Play("Cell Die");
            }
        }
    }

    private string RunRuleset(int numOfLiveCells) {
        if (currRulesBeingUsed.Equals("ConwaysGameOfLife")) {
            return ConwaysGameOfLife(numOfLiveCells);
        } else if (currRulesBeingUsed.Equals("LevelOne")) {
            return LevelOne(numOfLiveCells);
        } else if (currRulesBeingUsed.Equals("LevelTwo")) {
            return LevelTwo(numOfLiveCells);
        } else if (currRulesBeingUsed.Equals("LevelThree")) {
            return LevelThree(numOfLiveCells);
        } else if (currRulesBeingUsed.Equals("LevelFour")) {
            return LevelFour(numOfLiveCells);
        } else if (currRulesBeingUsed.Equals("LevelFive")) {
            return LevelFive(numOfLiveCells);
        } else if (currRulesBeingUsed.Equals("LevelSix")) {
            return LevelSix(numOfLiveCells);
        } else if (currRulesBeingUsed.Equals("LevelSeven")) {
            return LevelSeven(numOfLiveCells);
        } else if (currRulesBeingUsed.Equals("LevelEight")) {
            return LevelEight(numOfLiveCells);
        } else if (currRulesBeingUsed.Equals("LevelNine")) {
            return LevelNine(numOfLiveCells);
        } else {
            Debug.LogError(currRulesBeingUsed + " does not exsist");
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

    private string LevelOne(int numOfLiveCells) {
        if(numOfLiveCells >= 1) {
            return "born";
        } else {
            return "stay";
        }
    }

    private string LevelTwo(int numOfLiveCells) {
        if(numOfLiveCells >= 3) {
            return "born";
        }

        return "stay";
    }

    private string LevelThree(int numOfLiveCells) {
        if(numOfLiveCells == 1) {
            return "born";
        } else if (numOfLiveCells > 1) {
            return "stay";
        } else {
            return "die";
        }
    }

    private string LevelFour(int numOfLiveCells) {
        return ConwaysGameOfLife(numOfLiveCells);
    }

    private string LevelFive(int numOfLiveCells) {
        // TODO: Implement LevelFive rules
        return $"LevelFive executed with {numOfLiveCells} live cells.";
    }

    private string LevelSix(int numOfLiveCells) {
        // TODO: Implement LevelSix rules
        return $"LevelSix executed with {numOfLiveCells} live cells.";
    }

    private string LevelSeven(int numOfLiveCells) {
        // TODO: Implement LevelSeven rules
        return $"LevelSeven executed with {numOfLiveCells} live cells.";
    }

    private string LevelEight(int numOfLiveCells) {
        // TODO: Implement LevelEight rules
        return $"LevelEight executed with {numOfLiveCells} live cells.";
    }

private string LevelNine(int numOfLiveCells) {
    // TODO: Implement LevelNine rules
    return $"LevelNine executed with {numOfLiveCells} live cells.";
}


    private void CheckIfWon() {

        if(tilesFilled > amountOfTilesNeededToBeFilled) {
            Debug.LogError("oh no");
        }
        
        if(tilesFilled == amountOfTilesNeededToBeFilled) {
            print("you won");
            StopPlaying();

            canvas.transform.GetChild(1).gameObject.SetActive(false);
            canvas.transform.GetChild(2).gameObject.SetActive(false);
            canvas.transform.GetChild(3).gameObject.SetActive(!canvas.transform.GetChild(3).gameObject.activeSelf);
            canvas.transform.GetChild(4).gameObject.SetActive(!canvas.transform.GetChild(4).gameObject.activeSelf);

            SoundManager.Play("yay" + Random.Range(1,3).ToString());
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

    private int GetAreaNeededToBeFilledCount() {
        int count = 0;

        for(int x = 0; x < size.x; x++) {
            for(int y = 0; y < size.y; y++) {
                if(areaNeededToWinTileMap.GetTile(new(x,y,0)) != null) {
                    count++;
                }
            }
        }

        return count;
    }
}