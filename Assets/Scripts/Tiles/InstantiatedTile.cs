using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatedTile : MonoBehaviour
{
    public BaseTileBehaviour LinkedBehaviour { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Bind(BaseTileBehaviour behaviour)
    {
        LinkedBehaviour = behaviour;
    }
}
