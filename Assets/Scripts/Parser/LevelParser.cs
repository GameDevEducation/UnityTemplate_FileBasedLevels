using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelParser : MonoBehaviour
{
    enum ELevelSection
    {
        Unknown,
        Metadata,
        Layout
    }

    [SerializeField] Transform LevelRoot;
    [SerializeField] float TileSize = 1f;
    [SerializeField] List<BaseTileBehaviour> SupportedTiles;

    [System.NonSerialized] Dictionary<string, BaseTileBehaviour> TileRegistry = new Dictionary<string, BaseTileBehaviour>();

    Level ActiveLevel;

    private void Awake()
    {
        // build the tile registry
        foreach (var tile in SupportedTiles)
            TileRegistry[tile.UniqueID] = tile;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadLevel("Level0.lvl");
    }

    // Update is called once per frame
    void Update()
    {
        if (ActiveLevel != null)
            ActiveLevel.Tick();
    }

    void ClearExistingLevel()
    {
        // remove the existing level objects
        for (int index = LevelRoot.childCount - 1; index >= 0; index--)
        {
            Destroy(LevelRoot.GetChild(index).gameObject);
        }
    }

    const char Keyword_Comment = '#';
    const char Keyword_ColumnDelimiter = ',';
    const char Keyword_ElementDelimeter = '|';
    const char Keyword_ParameterDelimiter = '&';
    const char Keyword_KeyValueAssignment = '=';
    const string Keyword_MetadataStart = "METADATA";
    const string Keyword_MetadataEnd = "END METADATA";
    const string Keyword_LayoutStart = "LAYOUT";
    const string Keyword_LayoutEnd = "END LAYOUT";

    class MetadataFields
    {
        public static string Version = "VERSION";
    }

    public void LoadLevel(string fileName)
    {
        ClearExistingLevel();

        // construct the file path
        string filePath = Path.Combine(Application.streamingAssetsPath, 
                                       "Levels", fileName);

        // file not found?
        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);

        Level newLevel = new Level(filePath);

        // read and parse the file
        string[] fileContents = File.ReadAllLines(filePath);
        ELevelSection section = ELevelSection.Unknown;
        foreach(var fileLine in fileContents)
        {
            string lineToProcess = fileLine.Trim();

            // line has no processable content
            if (string.IsNullOrEmpty(lineToProcess) || lineToProcess.StartsWith(Keyword_Comment))
                continue;

            // are we in an unknown section
            if (section == ELevelSection.Unknown)
            {
                if (lineToProcess == Keyword_MetadataStart)
                    section = ELevelSection.Metadata;
                else if (lineToProcess == Keyword_LayoutStart)
                    section = ELevelSection.Layout;
            }
            else if (section == ELevelSection.Metadata)
            {
                if (lineToProcess != Keyword_MetadataEnd)
                    ParseLine_Metadata(lineToProcess, newLevel);
                else
                    section = ELevelSection.Unknown;
            }
            else if (section == ELevelSection.Layout)
            {
                if (lineToProcess != Keyword_LayoutEnd)
                    ParseLine_Layout(lineToProcess, newLevel);
                else
                    section = ELevelSection.Unknown;
            }
        }

        MakeLevelActive(newLevel);
    }

    void MakeLevelActive(Level newLevel)
    {
        // existing level present?
        if (ActiveLevel != null)
        {
            // run cleanup if needed
        }

        ActiveLevel = newLevel;
        ActiveLevel.PerformInstantiation(LevelRoot, TileSize);
    }

    void ParseLine_Metadata(string line, Level level)
    {
        var kvpComponents = line.Split('=');

        // check if the metadata is invalid
        if (kvpComponents.Length != 2)
            throw new System.Exception($"Invalid metadata. Expected format key=value found {line}");

        var key = kvpComponents[0].Trim();
        var value = kvpComponents[1].Trim();

        // check the key
        if (key == MetadataFields.Version)
            level.SetVersion(int.Parse(value));
    }

    void ParseLine_Layout(string line, Level level)
    {
        level.AddRow();

        // process each entry
        var rowElements = line.Split(Keyword_ColumnDelimiter);
        foreach(var rawTileString in rowElements)
        {
            var trimmedTileString = rawTileString.Trim();

            if (string.IsNullOrEmpty(trimmedTileString))
            {
                level.AddEmptyTile();
                continue;
            }

            level.BeginTile();

            // split up and parse each element
            var elementList = trimmedTileString.Split(Keyword_ElementDelimeter);
            foreach(var rawElement in elementList)
            {
                var element = rawElement.Trim();

                var elementComponents = element.Split(Keyword_ParameterDelimiter);
                var elementType = elementComponents[0].Trim();

                if (!TileRegistry.ContainsKey(elementType))
                    throw new System.Exception($"Unknown tile {elementType} found");

                Dictionary<string, string> parameters = ParseParameters(elementComponents);

                level.AddBehaviourToCurrentTile(TileRegistry[elementType], parameters);
            }

            level.EndTile();
        }

        level.EndRow();
    }

    Dictionary<string, string> ParseParameters(string[] elementComponents)
    {
        // do we have parameters?
        Dictionary<string, string> parameters = null;
        if (elementComponents.Length > 1)
        {
            parameters = new Dictionary<string, string>();

            // extract the parameters
            for (int paramIndex = 1; paramIndex < elementComponents.Length; paramIndex++)
            {
                var parameter = elementComponents[paramIndex];
                var parameterComponents = parameter.Split(Keyword_KeyValueAssignment);

                if (parameterComponents.Length != 2)
                    throw new System.Exception($"Invalid parameter {parameter}");

                // store the parameters
                parameters[parameterComponents[0].Trim()] = parameterComponents[1].Trim();
            }
        }

        return parameters;
    }
}
