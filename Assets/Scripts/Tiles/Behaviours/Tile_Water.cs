using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/Water", fileName = "Tile_Water")]
public class Tile_Water : BaseTileBehaviour
{
    public enum EType
    {
        Moving,
        Still
    }

    [SerializeField] float BobAmount = 0.1f;
    [SerializeField] float BobInterval = 2f;
    [SerializeField] EType Type = EType.Moving;

    float BobProgress = 0f;
    Vector3 StartingPosition;

    const string Keyword_Type = "Type";

    public override void ParseParameters(Dictionary<string, string> parameters)
    {
        base.ParseParameters(parameters);

        // overriding type?
        if (parameters.ContainsKey(Keyword_Type))
        {
            if (!System.Enum.TryParse<EType>(parameters[Keyword_Type], out Type))
                throw new System.Exception($"Invalid water type {Type}");
        }
    }

    public override void OnInstantiated()
    {
        base.OnInstantiated();
        StartingPosition = LinkedGO.transform.localPosition;
        BobProgress = Type == EType.Moving ? Random.Range(0f, 1f) : 0f;
    }

    public override void Tick()
    {
        base.Tick();

        if (Type == EType.Moving)
        {
            // update the bob progress
            BobProgress = (BobProgress + Time.deltaTime / BobInterval) % 1f;

            // update the position
            LinkedGO.transform.localPosition = StartingPosition + BobAmount * Vector3.up * Mathf.Sin(BobProgress * Mathf.PI * 2f);
        }
    }
}
