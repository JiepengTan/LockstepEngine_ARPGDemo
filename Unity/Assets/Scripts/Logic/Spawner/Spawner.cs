using System;
using Lockstep.Math;

namespace Lockstep.Logic {
    [Serializable]
    public class Spawner {
        public LFloat spawnTime;
        public LVector3 spawnPoint;
        public int prefabId;
        public Action<int, LVector3> OnSpawnEvent;

        private LFloat timer;
        public virtual void DoStart(){
            var prefab = ResourceManager.LoadPrefab(prefabId);
            CollisionManager.Instance.RigisterPrefab(prefab, (int) EColliderLayer.Enemy);
            timer = spawnTime;
        }

        public virtual void DoUpdate(LFloat deltaTime){
            timer += deltaTime;
            if (timer > spawnTime) {
                timer = LFloat.zero;
                Spawn();
            }
        }

        public void Spawn(){
            OnSpawnEvent(prefabId, spawnPoint);
        }
    }
}