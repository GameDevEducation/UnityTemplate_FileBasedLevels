using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTileBehaviour : ScriptableObject
{
    public string UniqueID;

    [SerializeField] protected GameObject _Prefab;

    public GameObject LinkedGO { get; protected set; }
    public InstantiatedTile LinkedTile { get; protected set; }

    public GameObject Prefab => _Prefab;
    public Vector2Int Location { get; protected set; }

    public void Bind(Vector2Int _Location, Dictionary<string, string> parameters)
    {
        Location = _Location;

        if (parameters != null)
            ParseParameters(parameters);
    }

    public virtual void ParseParameters(Dictionary<string, string> parameters)
    {

    }

    public virtual void InstantiateBehaviour(GameObject tileGO)
    {
        LinkedGO = GameObject.Instantiate(Prefab, tileGO.transform);
        LinkedTile = LinkedGO.GetComponent<InstantiatedTile>();
        LinkedTile.Bind(this);

        OnInstantiated();
    }

    public virtual void OnInstantiated()
    {

    }

    public virtual void Tick()
    {

    }
}
