using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public int Version { get; set; }
    public string FilePath { get; set; }

    List<List<GridTile>> Tiles = new List<List<GridTile>>();

    public Level(string _FilePath)
    {
        FilePath = _FilePath;
    }

    public void SetVersion(int _Version)
    {
        Version = _Version;
    }

    Vector2Int CurrentInsertionPoint = Vector2Int.zero;

    public void AddRow()
    {
        Tiles.Add(new List<GridTile>());
    }

    public void EndRow()
    {
        CurrentInsertionPoint = new Vector2Int(0, CurrentInsertionPoint.y + 1);
    }

    public void BeginTile()
    {
        Tiles[CurrentInsertionPoint.y].Add(new GridTile());
    }

    public void EndTile()
    {
        CurrentInsertionPoint.x += 1;
    }

    public BaseTileBehaviour AddBehaviourToCurrentTile(BaseTileBehaviour template, Dictionary<string, string> parameters)
    {
        // instantiate the new tile and add to the level
        var newBehaviour = ScriptableObject.Instantiate(template);
        Tiles[CurrentInsertionPoint.y][CurrentInsertionPoint.x].AddBehaviour(newBehaviour);

        // set the location
        newBehaviour.Bind(CurrentInsertionPoint, parameters);

        return newBehaviour;
    }

    public void AddEmptyTile()
    {
        Tiles[CurrentInsertionPoint.y].Add(null);

        EndTile();
    }

    public void PerformInstantiation(Transform levelRoot, float tileSize)
    {
        for (int rowIndex = 0; rowIndex < Tiles.Count; rowIndex++)
        {
            // create the row game object
            var rowGO = new GameObject($"Row_{(rowIndex + 1)}");
            rowGO.transform.SetParent(levelRoot);
            rowGO.transform.localPosition = new Vector3(0f, 0f, -rowIndex * tileSize);

            // create the individual tiles
            var rowTiles = Tiles[rowIndex];
            for (int tileIndex = 0; tileIndex < rowTiles.Count; ++tileIndex)
            {
                var tile = rowTiles[tileIndex];

                if (tile == null)
                    continue;

                // create the tile game object
                var tileGO = new GameObject($"Tile_{(rowIndex + 1)},{(tileIndex + 1)}");
                tileGO.transform.SetParent(rowGO.transform);
                tileGO.transform.localPosition = new Vector3(tileIndex * tileSize, 0f, 0f);

                tile.InstantiateBehaviours(tileGO);
            }
        }
    }

    public void Tick()
    {
        foreach(var rowTiles in Tiles)
        {
            foreach (var tile in rowTiles)
            {
                if (tile == null)
                    continue;

                tile.Tick();
            }
        }
    }
}
