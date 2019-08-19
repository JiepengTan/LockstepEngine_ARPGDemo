using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Lockstep.Logic {
    public class ResourceManager : BaseManager {
        public static ResourceManager Instance { get; private set; }
        [HideInInspector] public GameConfig config;
        public string configPath = "GameConfig";


        public override void DoAwake(){
            Instance = this;
            config = Resources.Load<GameConfig>(configPath);
        }

        public Dictionary<int, GameObject> id2Prefab = new Dictionary<int, GameObject>();
        private string pathPrefix = "Prefabs/";

        public static GameObject LoadPrefab(int id){
            return Instance._LoadPrefab(id);
        }

        public PlayerConfig GetPlayerConfig(int id){
            return config.GetPlayerConfig(id);
        }

        public EnemyConfig GetEnemyConfig(int id){
            return config.GetEnemyConfig(id - 10);
        }

        GameObject _LoadPrefab(int id){
            if (id2Prefab.TryGetValue(id, out var val)) {
                return val;
            }

            if (id < 10) {
                var config = this.config.GetPlayerConfig(id);
                var prefab = (GameObject) Resources.Load(pathPrefix + config.prefabPath);
                id2Prefab[id] = prefab;
                return prefab;
            }

            if (id >= 10) {
                var config = this.config.GetEnemyConfig(id - 10);
                var prefab = (GameObject) Resources.Load(pathPrefix + config.prefabPath);
                id2Prefab[id] = prefab;
                return prefab;
            }

            return null;
        }
    }
}