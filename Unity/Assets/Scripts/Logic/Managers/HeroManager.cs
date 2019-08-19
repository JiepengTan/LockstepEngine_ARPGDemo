using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Logic {
    public class EntityView {
        public static GameObject CreateEntity(BaseEntity entity, int prefabId, LVector3 position, GameObject prefab,
            object config){
            var obj = (GameObject) GameObject.Instantiate(prefab, position.ToVector3(), Quaternion.identity);
            entity.engineTransform = obj.transform;
            entity.transform.Pos3 = position;
            config.CopyFiledsTo(entity);
            entity.animator = obj.GetComponent<CAnimation>();
            var views = obj.GetComponents<IView>();
            foreach (var view in views) {
                view.BindEntity(entity);
            }

            entity.PrefabId = prefabId;
            GameManager.Instance.collisionManager.RegisterEntity(prefab, obj, entity);
            entity.DoAwake();
            entity.DoStart();
            return obj;
        }
    }

    public class HeroManager : BaseManager {
        public LVector3 pos;
        public int prefabId;
        public static HeroManager Instance;

        public override void DoStart(){ }
        public static GameObject InstantiateEntity(Player entity, int prefabId, LVector3 position){
            var prefab = ResourceManager.LoadPrefab(prefabId);
            object config = ResourceManager.Instance.GetPlayerConfig(prefabId);
            var obj = EntityView.CreateEntity(entity, prefabId, position, prefab, config);
            //init mover
            GameManager.player = entity;
            GameManager.playerTrans = obj.transform;
            return obj;
        }


        public override void DoUpdate(LFloat deltaTime){
            foreach (var player in GameManager.allPlayers) {
                player.DoUpdate(deltaTime);
            }
        }
    }
}