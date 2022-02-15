using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile
{
    List<BaseTileBehaviour> Behaviours { get; set; } = new List<BaseTileBehaviour>();

    public void AddBehaviour(BaseTileBehaviour newBehaviour)
    {
        Behaviours.Add(newBehaviour);
    }

    public void InstantiateBehaviours(GameObject tileGO)
    {
        foreach(var behaviour in Behaviours)
            behaviour.InstantiateBehaviour(tileGO);
    }

    public void Tick()
    {
        foreach(var behaviour in Behaviours)
            behaviour.Tick();
    }
}
