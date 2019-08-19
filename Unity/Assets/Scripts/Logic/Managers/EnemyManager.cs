using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Logic {
    public class EnemyManager : BaseManager {
        public List<Spawner> spawners = new List<Spawner>();

        public static EnemyManager Instance { get; private set; }
        public List<Enemy> allEnemy = new List<Enemy>();

        public static int maxCount = 1;
        private static int curCount = 0;

        public override void DoAwake(){
            Instance = this;
        }


        public override void DoStart(){
            foreach (var spawner in spawners) {
                spawner.OnSpawnEvent += OnSpawn;
                spawner.DoStart();
            }
        }

        public override void DoUpdate(LFloat deltaTime){
            if (GameManager.player != null && GameManager.player.currentHealth <= 0f) {
                return;
            }

            foreach (var spawner in spawners) {
                spawner.DoUpdate(deltaTime);
            }

            foreach (var enemy in allEnemy) {
                enemy.DoUpdate(deltaTime);
            }
        }

        private static int enmeyID = 0;


        public void OnSpawn(int prefabId, LVector3 position){
            if (curCount >= maxCount) {
                return;
            }

            curCount++;
            var entity = InstantiateEntity(prefabId, position);
            Instance.AddEnemy(entity as Enemy);
        }

        public static BaseEntity InstantiateEntity(int prefabId, LVector3 position){
            var prefab = ResourceManager.LoadPrefab(prefabId);
            object config = ResourceManager.Instance.GetEnemyConfig(prefabId);
            Debug.Trace("CreateEnemy");
            var entity = new Enemy();
            var obj = EntityView.CreateEntity(entity, prefabId, position, prefab, config);
            entity.nav = obj.GetComponent<NavMeshAgentMono>();
            entity.nav.DoStart();
            entity.nav.enabled = false;
            entity.CBrain = new CBrain();
            entity.CBrain.Init(entity, 0);
            obj.name = obj.name + enmeyID++;
            obj.name = obj.name + enmeyID++;
            return entity;
        }


        public void AddEnemy(Enemy enemy){
            allEnemy.Add(enemy);
        }

        public void RemoveEnemy(Enemy enemy){
            allEnemy.Remove(enemy);
        }
    }
}