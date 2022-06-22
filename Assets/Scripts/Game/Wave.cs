using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class Wave : ScriptableObject
{
    [SerializeField, Range(0f, 60f)] private float waveCooldown = 5f;
    [SerializeField] private SpawnSequence[] spawnSequences;

    public State Begin() => new State(this);
    
    public struct State
    {
        private readonly Wave wave;

        public State(Wave wave)
        {
            this.wave = wave;
        }

        public IEnumerator Progress()
        {
            GameHUD.SetPromptLabelActive(true);
            GameHUD.UpdateWaveInfo("New wave starts in:");

            for (float i = wave.waveCooldown; i >= 0f; i -= Time.deltaTime)
            {
                if (Game.SkipTimer)
                {
                    Game.SkipTimer = false;
                    GameHUD.UpdateTimer(0, 0);
                    break;
                }
                
                int minutes = Mathf.RoundToInt(i / 60f);
                int seconds = Mathf.RoundToInt(i % 60f);
                GameHUD.UpdateTimer(minutes, seconds);
                yield return null;
            }
            
            GameHUD.SetPromptLabelActive(false);
            
            foreach (SpawnSequence sequence in wave.spawnSequences)
            {
                SpawnSequence.State state = sequence.Begin();
                yield return state.Progress();
            }
        }
    }
}