using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Bubble BubblePrefab;
    void Start()
    {
        BGrid grid = new BGrid(10, 10, 1.0f, BubblePrefab);
    }
}
