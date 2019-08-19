using System;
using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Logic {
    [Serializable]
    public class EnemyConfig {
        public string prefabPath;
        [Header("Health")] 
        public int startingHealth = 100; // The amount of health the enemy starts the game with.
        public int currentHealth; // The current health the enemy has.
        public LFloat sinkSpeed; // The speed at which the enemy sinks through the floor when dead.
        public int scoreValue = 10; // The amount added to the player's score when the enemy dies.
        public AudioClip deathClip; // The sound to play when the enemy dies.

        [Header("Attack")]
        public LFloat timeBetweenAttacks;
        public int attackDamage = 10;

        [Header("movement")] public CNavMesh agent = new CNavMesh();
    }

    [Serializable]
    public class PlayerConfig {
        public string prefabPath;
        [Header("Health")] public int startingHealth = 100; // The amount of health the player starts the game with.
        public int currentHealth; // Reference to an image to flash on the screen on being hurt.
        public AudioClip deathClip; // The audio clip to play when the player dies.
        public LFloat flashSpeed; // The speed the damageImage will fade at.
        public Color flashColour = new Color(1f, 0f, 0f, 0.1f); // The colour the damageImage is set to, to flash.

        [Header("Attack")] public int damagePerShot = 20; // The damage inflicted by each bullet.
        public LFloat timeBetweenBullets; // The time between each shot.
        public LFloat range; // The distance the gun can fire.
        public string faceLightName; // Duh
        public string attackTransName;

        [Header("movement")] public LFloat speed;
    }
    [CreateAssetMenu(menuName = "GameConfig")]
    public class GameConfig : ScriptableObject {


        public List<EnemyConfig> enemies = new List<EnemyConfig>();
        public List<PlayerConfig> player = new List<PlayerConfig>();

        public EnemyConfig GetEnemyConfig(int id){
            return enemies[id];
        }

        public PlayerConfig GetPlayerConfig(int id){
            return player[id];
        }
    }
}