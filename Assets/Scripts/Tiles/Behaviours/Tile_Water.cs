using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/Water", fileName = "Tile_Water")]
public class Tile_Water : BaseTileBehaviour
{
    [SerializeField] float BobAmount = 0.1f;
    [SerializeField] float BobInterval = 2f;

    float BobProgress = 0f;
    Vector3 StartingPosition;

    public override void OnInstantiated()
    {
        base.OnInstantiated();
        StartingPosition = LinkedGO.transform.localPosition;
        BobProgress = Random.Range(0f, 1f);
    }

    public override void Tick()
    {
        base.Tick();

        // update the bob progress
        BobProgress = (BobProgress + Time.deltaTime / BobInterval) % 1f;

        // update the position
        LinkedGO.transform.localPosition = StartingPosition + BobAmount * Vector3.up * Mathf.Sin(BobProgress * Mathf.PI * 2f);
    }
}
