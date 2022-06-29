using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveableEntity : MonoBehaviour
{
    [SerializeField] private string entityID;
    
    public string ID => entityID;

    [ContextMenu("Generate ID")]
    private void GenerateID()
    {
        entityID = Guid.NewGuid().ToString();
    }

    public object SaveState()
    {
        var state = new Dictionary<string, object>();
        
        foreach (ISaveable saveable in GetComponents<ISaveable>())
        {
            state[saveable.GetType().ToString()] = saveable.SaveState();
        }

        return state;
    }

    public void LoadState(object state)
    {
        var stateDictionary = (Dictionary<string, object>)state;

        foreach (ISaveable saveable in GetComponents<ISaveable>())
        {
            var typeName = saveable.GetType().ToString();
            if (stateDictionary.TryGetValue(typeName, out object savedState)) saveable.LoadState(savedState);
        }
    }
}