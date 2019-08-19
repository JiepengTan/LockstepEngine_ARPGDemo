using Lockstep.Logic;

namespace Lockstep.Collision2D {
    public partial class ColliderProxy {
        public BaseEntity Entity => (BaseEntity) EntityObject;
    }
}