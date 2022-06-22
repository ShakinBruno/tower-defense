using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class Scenario : ScriptableObject
{
    [SerializeField] private Wave[] waves;
    
    private int TotalWaves => waves.Length;
    public State Begin() => new State(this);

    public struct State
    {
        private int waveNumber;
        private readonly Scenario scenario;

        public State(Scenario scenario)
        {
            this.scenario = scenario;
            waveNumber = 0;
        }

        public IEnumerator Progress(IEnumerator onScenarioFinished)
        {
            foreach (Wave wave in scenario.waves)
            {
                Wave.State state = wave.Begin();
                GameHUD.UpdateWaveText(++waveNumber, scenario.TotalWaves);
                
                yield return state.Progress();
            }
            
            yield return onScenarioFinished;
        }
    }
}