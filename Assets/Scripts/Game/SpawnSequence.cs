using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class SpawnSequence
{
    [SerializeField] private EnemyFactory factory;
    [SerializeField] private EnemyType type = EnemyType.Medium;
    [SerializeField, Range(1, 100)] private int amount = 1;
    [SerializeField, Range(0f, 30f)] private float spawnCooldown;
    [SerializeField, Range(0f, 10f)] private float cooldown = 1f;
    
    public State Begin() => new State(this);

    public struct State
    {
        private readonly SpawnSequence sequence;

        public State(SpawnSequence sequence)
        {
            this.sequence = sequence;
        }

        public IEnumerator Progress()
        {
            GameHUD.UpdateWaveInfo($"{sequence.type.ToString()} is coming in:");
            var cooldown = new WaitForSeconds(sequence.cooldown);

            for (float i = sequence.spawnCooldown; i >= 0f; i -= Time.deltaTime)
            {
                int minutes = Mathf.RoundToInt(i / 60f);
                int seconds = Mathf.RoundToInt(i % 60f);
                GameHUD.UpdateTimer(minutes, seconds);
                yield return null;
            }

            for (var i = 0; i < sequence.amount; i++)
            {
                Game.SpawnEnemy(sequence.factory, sequence.type);
                yield return cooldown;
            }
        }
    }
}