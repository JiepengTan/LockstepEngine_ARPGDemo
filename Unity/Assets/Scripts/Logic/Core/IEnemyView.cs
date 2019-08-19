using Lockstep.Math;

namespace Lockstep.Logic {
    public interface IEnemyView : IView {
        void OnPlayerDied();
        void TakeDamage(int amount, LVector3 hitPoint);
        void Death();
        void StartSinking();
    }
}