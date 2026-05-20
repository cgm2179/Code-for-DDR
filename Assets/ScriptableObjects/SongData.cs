using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SongData", menuName = "Scriptable Objects/Song Data", order = 1)]
public class SongData : ScriptableObject
{
    public AudioClip audioClip;
    public string songTitle;
    public float bpm;
    public float beatsShownInAdvance;
    public NoteData[] notes;
}

[System.Serializable]
public struct NoteData
{
    public float beat;
    public int laneIndex;
    public float duration;
}
