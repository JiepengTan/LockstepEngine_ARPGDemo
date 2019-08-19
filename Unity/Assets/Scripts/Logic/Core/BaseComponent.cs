using Lockstep.Collision2D;
using Lockstep.FakeServer;
using Lockstep.Math;

namespace Lockstep.Logic {
    public partial class BaseComponent : ILifeCycle {
        public BaseEntity entity;
        public CTransform2D transform;

        public virtual void BindEntity(BaseEntity entity){
            this.entity = entity;
            transform = entity.transform;
        }

        public virtual void DoAwake(){ }
        public virtual void DoStart(){ }
        public virtual void DoUpdate(LFloat deltaTime){ }
        public virtual void DoDestroy(){ }
    }

    public partial class PlayerComponent : BaseComponent {
        public Player player;
        public PlayerInput input => player.InputAgent;

        public override void BindEntity(BaseEntity entity){
            base.BindEntity(entity);
            player = (Player) entity;
        }
    }
}