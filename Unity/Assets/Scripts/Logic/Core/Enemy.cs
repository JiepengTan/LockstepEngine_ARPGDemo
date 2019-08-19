using Lockstep.Collision2D;
using Lockstep.Math;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Logic {
    public class Enemy : BaseEntity {
        public CBrain CBrain;
        public NavMeshAgentMono nav;
        public IEnemyView eventHandler;
        public Player player;
        public int scoreValue = 10;
        public int attackDamage = 10;
        public bool playerInRange;
        public LFloat timer;
        public bool isSinking;
        public LFloat timeBetweenAttacks = LFloat.half;


        public override void DoUpdate(LFloat deltaTime){
            base.DoUpdate(deltaTime);
            var minDist = LFloat.MaxValue;
            foreach (var item in GameManager.allPlayers) {
                var dist = (this.transform.pos - item.transform.pos).sqrMagnitude;
                if (minDist > dist && !item.isDead) {
                    minDist = dist;
                    player = item;
                }
            }

            if (player == null) return; // no target no action

            if (rigidbody.isOnFloor) {
                UpdateAttack(deltaTime);
                UpdateMovement();
                if (!isDead) {
                    CBrain?.DoUpdate(deltaTime);
                }
            }
        }

        private void UpdateMovement(){
            if (currentHealth > 0 && currentHealth > 0) {
                nav.SetDestination(player.transform.Pos3);
            }
            else {
                nav.enabled = false;
            }
        }

        void UpdateAttack(LFloat deltaTime){
            timer += deltaTime;
            if (timer >= timeBetweenAttacks && playerInRange && currentHealth > 0) {
                timer = LFloat.zero;
                if (player.currentHealth > 0) {
                    Debug.Trace($"{EntityId}Atk{player.EntityId}");
                    player.TakeDamage(attackDamage, player.transform.Pos3);
                }
            }

            if (player.currentHealth <= 0) {
                eventHandler.OnPlayerDied();
            }
        }

        protected override void OnTakeDamage(int amount, LVector3 hitPoint){
            currentHealth -= amount;
            OnBeAtked?.Invoke(amount, hitPoint);
            eventHandler.TakeDamage(amount, hitPoint);
            if (currentHealth <= 0) {
                isDead = true;
                CollisionManager.Instance.RemoveCollider(this);
                eventHandler.Death();
                nav.enabled = false;
                EnemyManager.Instance.RemoveEnemy(this);
                StartSinking();
            }
        }

        public void StartSinking(){
            isSinking = true;
            ScoreManager.score += scoreValue;
            eventHandler.StartSinking();
        }

        public override void OnLPTriggerEnter(ColliderProxy other){
            Debug.Trace($"{EntityId} OnLPTriggerEnter {other.Entity.EntityId}");
            playerInRange = true;
        }

        public override void OnLPTriggerStay(ColliderProxy other){ }

        public override void OnLPTriggerExit(ColliderProxy other){
            Debug.Trace($"{EntityId} OnLPTriggerExit {other.Entity.EntityId}");
            playerInRange = false;
        }
    }
}