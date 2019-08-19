using Lockstep.Math;

namespace Lockstep.Logic {
    public interface IPlayerView :IView {
        void TakeDamage(int amount, LVector3 hitPoint);
        void OnDead();
        void SetLinePosition(int index, LVector3 position);
        void Shoot();
        void DisableEffects();
        void Animating(bool isIdle);
    }
}