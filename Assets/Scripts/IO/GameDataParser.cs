using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameDataParser : MonoBehaviour
{
    [SerializeField] private string saveFileName;
    
    public string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

    [ContextMenu("Save")]
    public void Save()
    {
        Dictionary<string, object> state = LoadFile();
        SaveState(state);
        SaveFile(state);
    }
    
    [ContextMenu("Load")]
    public void Load()
    {
        Dictionary<string, object> state = LoadFile();
        LoadState(state);
    }

    private void SaveFile(object state)
    {
        using FileStream stream = File.Open(SavePath, FileMode.Create);
        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, state);
    }

    private Dictionary<string, object> LoadFile()
    {
        if (!File.Exists(SavePath)) return new Dictionary<string, object>();
        using FileStream stream = File.Open(SavePath, FileMode.Open);
        var formatter = new BinaryFormatter();
        return (Dictionary<string, object>)formatter.Deserialize(stream);
    }

    private void SaveState(Dictionary<string, object> state)
    {
        foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
        {
            state[saveable.ID] = saveable.SaveState();
        }
    }

    private void LoadState(Dictionary<string, object> state)
    {
        foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
        {
            if (state.TryGetValue(saveable.ID, out object savedState)) saveable.LoadState(savedState);
        }
    }
}