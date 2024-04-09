using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public Vector3 position;
    [HideInInspector] public bool bFromShooter;
    [HideInInspector] public BGrid CurrentGrid;
    
    private Color color;

    [HideInInspector] public float speed = 13.0f;

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetColor(Color c)
    {
        color = c;
        GetComponent<SpriteRenderer>().color = c;
    }

    public Color GetColor()
    {
        return color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        speed = 0;
    
        Bubble otherObject = collision.GetComponent<Bubble>();
    
        if(bFromShooter)
        {
            if (otherObject.CurrentGrid != null)
            {
                //find any empty cell to attach the bubble
                CurrentGrid = otherObject.CurrentGrid;
                Vector2Int otherObjCellIndex = CurrentGrid.GetGridXY(otherObject.transform.position);
                Vector3 emptyCellPos = CurrentGrid.CheckEmptyCellsAround(otherObjCellIndex);
                emptyCellPos += CurrentGrid.offset;

                //add the bubble to the actual grid with other bubbles
                Vector2Int emptyCellIndex = CurrentGrid.GetGridXY(emptyCellPos);
                CurrentGrid.bubbles[emptyCellIndex.x, emptyCellIndex.y] = this;
                CurrentGrid.cellsArray[emptyCellIndex.x, emptyCellIndex.y] = true;
                GetComponent<CircleCollider2D>().isTrigger = false;

                bFromShooter = false;

                if(otherObject.GetColor() == this.color)
                { 
                    transform.position = emptyCellPos;
                    CurrentGrid.DestroySurroundingBubbles(otherObjCellIndex, this.color);
                }
                else
                {
                    transform.position = emptyCellPos;
                }
            }
        }
        Debug.Log("triggered!!");
    }



}
