using System.Collections.Generic;
using System.IO;
using System.Globalization;
using UnityEngine;

public class CSVLoader : MonoBehaviour
{
    public SongData songData;
    public string fileName = "SongChart1.csv";

    void Start()
    {
        LoadCSVIntoSongData(songData);
    }

    void LoadCSVIntoSongData(SongData songData)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if (!File.Exists(path))
        {
            Debug.LogError("CSV not found: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);

        List<NoteData> noteList = new List<NoteData>();

        for (int i = 1; i < lines.Length; i++) // skip header
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] values = lines[i].Split(',');

            NoteData note = new NoteData();
            note.beat = float.Parse(values[0], CultureInfo.InvariantCulture);
            note.laneIndex = int.Parse(values[1]);
            note.duration = float.Parse(values[2], CultureInfo.InvariantCulture);

            noteList.Add(note);
        }

        songData.notes = noteList.ToArray();

        Debug.Log("Loaded " + noteList.Count + " notes!");
    }
}