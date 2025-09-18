using UnityEngine;

public class Cell
{
    private bool alive = false;
    private Vector2Int position;

    public Cell(Vector2Int position) {
        this.position = position;
    }

    public void SetIsAlive(bool newVal) {
        alive = newVal;
    }

    public bool GetIsAlive() {
        return alive;
    }

    public Vector2Int GetPos() {
        return position;
    }

    public override string ToString() {
        return "Position: " + position.ToString() + " Is Alive: " + alive;
    }

}