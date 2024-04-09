using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGrid
{
    int width;
    int height;
    float cellSize;

    [HideInInspector] public Vector3 offset;
    [HideInInspector] public bool[,] cellsArray;
    [HideInInspector] public Bubble[,] bubbles;

    public static Color[] colors = new Color[5] { Color.red, Color.yellow, Color.green, Color.blue, Color.magenta };
    
    public BGrid(int w, int h, float cs, Bubble prefab)
    {
        width = w;
        height = h;
        cellSize = cs;

        cellsArray = new bool[width, height]; //for "is cell empty or not?" purposes
        bubbles = new Bubble[width, height]; //actual bubbles
        VisitedCells = new bool[width, height]; //for search algo

        offset = new Vector3(cellSize, cellSize) * 0.5f;

        for (int x = 0; x < cellsArray.GetLength(0); x++)
        {
            for (int y = 0; y < cellsArray.GetLength(1); y++)
            {
                if(y >= 6)
                {
                    Bubble temp;
                    temp = Object.Instantiate(prefab, GetWorldPos(x, y) + offset, Quaternion.identity);
                    temp.SetColor(colors[Random.Range(0, 5)]);
                    temp.GetComponent<CircleCollider2D>().isTrigger = false;
                    temp.CurrentGrid = this;
                    
                    //temp.GetComponent<SpriteRenderer>().color = colors[Random.Range(0, 5)];

                    bubbles[x, y] = temp;
                    cellsArray[x, y] = true;
                }
                else
                {
                    bubbles[x, y] = null;
                    cellsArray[x, y] = false;
                }

                Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x, y + 1), Color.red, 120.0f);
                Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x + 1, y), Color.red, 120.0f);
            }
        }
    }

    public Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(x, y) * cellSize;
    }

    public Vector2Int GetGridXY(Vector3 pos)
    {
        int x, y;

        x = Mathf.FloorToInt(pos.x / cellSize);
        y = Mathf.FloorToInt(pos.y / cellSize);

        return new Vector2Int(x, y);
    }

    public Vector3 CheckEmptyCellsAround(Vector2Int CellIndex)
    {
        int x = CellIndex.x;
        int y = CellIndex.y;

        if (AreIndexesValid(CellIndex) && (!cellsArray[x - 1, y])) //left
        {
            return GetWorldPos(x - 1, y);
        } 
        else if (AreIndexesValid(CellIndex) && (!cellsArray[x + 1, y])) //right
        {
            return GetWorldPos(x + 1, y);
        }
        else if(AreIndexesValid(CellIndex) && (!cellsArray[x, y - 1])) //bottom
        {
            return GetWorldPos(x, y - 1);
        }
        else if (AreIndexesValid(CellIndex) && (!cellsArray[x, y + 1])) //up
        {
            return GetWorldPos(x, y + 1);
        }

        return Vector3.zero; 
    }

    bool AreIndexesValid(Vector2Int coords)
    {
        return coords.x >= 1 && coords.y >= 1 && coords.x <= 8 && coords.y <= 8;
    }

    public void DestroySurroundingBubbles(Vector2Int startIndex, Color colorToMatch)
    {
        SameColorBubbles.Clear();

        ClearVisitedCells();

        List<Vector2Int> indexesToVisit = new List<Vector2Int>();

        indexesToVisit.Add(startIndex);

        while(indexesToVisit.Count > 0)
        {
            int indexToRemove = indexesToVisit.Count - 1;
            Vector2Int currentCoords = indexesToVisit[indexToRemove];
            indexesToVisit.RemoveAt(indexToRemove);

            if(!cellsArray[currentCoords.x, currentCoords.y]) //skip if cell is empty!
            {
                continue;
            }

            if (VisitedCells[currentCoords.x, currentCoords.y]) 
            {
                continue;
            }

            VisitedCells[currentCoords.x, currentCoords.y] = true;

            if(bubbles[currentCoords.x, currentCoords.y].GetColor() != colorToMatch) 
            {
                continue;
            }

            SameColorBubbles.Add(bubbles[currentCoords.x, currentCoords.y]);
     
            AddCoordsIfNeeded(currentCoords, new Vector2Int(1, 0), ref indexesToVisit);  //right
            AddCoordsIfNeeded(currentCoords, new Vector2Int(-1, 0), ref indexesToVisit); //left
            AddCoordsIfNeeded(currentCoords, new Vector2Int(0, 1), ref indexesToVisit);  //up 
            AddCoordsIfNeeded(currentCoords, new Vector2Int(0, -1), ref indexesToVisit); //down
        }

        foreach(Bubble b in SameColorBubbles)
        {
            Vector3 pos = b.transform.position;
            Vector2Int bIndex = GetGridXY(pos);

            b.GetComponent<CircleCollider2D>().enabled = false;
            b.GetComponent<Rigidbody2D>().gravityScale = 1;
            b.GetComponent<SpriteRenderer>().color = Color.white;

            cellsArray[bIndex.x, bIndex.y] = false;
            bubbles[bIndex.x, bIndex.y] = null;
            GameObject.Destroy(b, 1.0f);
        }
    }

    void AddCoordsIfNeeded(Vector2Int coords, Vector2Int checkDir, ref List<Vector2Int> coordsToVisit)
    {
        Vector2Int nextCoords = coords + checkDir;

        if (AreCoordsValid(nextCoords))
        {
            coordsToVisit.Add(nextCoords);
        }
    }

    void ClearVisitedCells()
    {
        for(int x = 0; x < VisitedCells.GetLength(0); x++)
        {
            for (int y = 0; y < VisitedCells.GetLength(1); y++)
            {
                VisitedCells[x, y] = false;
            }
        }
    }

    bool AreCoordsValid(Vector2Int coords)
    {
        return coords.x >= 0 && coords.y >= 0 &&
            coords.x < cellsArray.GetLength(0) && coords.y < cellsArray.GetLength(1);
    }

    bool[,] VisitedCells;
    List<Bubble> SameColorBubbles = new List<Bubble>();
    //Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x, y + 1), Color.red, 120.0f);
    //Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x + 1, y), Color.red, 120.0f);
}
