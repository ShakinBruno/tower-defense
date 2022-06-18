using System;
using System.Collections.Generic;

[Serializable]
public class GameBehaviorCollection
{
    private List<GameBehavior> behaviors = new List<GameBehavior>();
    
    public void GameUpdate()
    {
        for (var i = 0; i < behaviors.Count; i++)
        {
            if (!behaviors[i].GameUpdate())
            {
                int lastIndex = behaviors.Count - 1;
                behaviors[i] = behaviors[lastIndex];
                behaviors.RemoveAt(lastIndex);
                i--;
            }
        }
    }
    
    public void Add(GameBehavior behavior)
    {
        behaviors.Add(behavior);
    }
}