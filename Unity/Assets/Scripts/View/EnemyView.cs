using System.Collections;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Logic {
    public class EnemyView : MonoBehaviour, IEnemyView {
        public Enemy owner;

        private bool _isSinking = false;
        private float _sinkSpeed = 1f;

        Animator anim;
        GameObject player;
        [HideInInspector] public AudioClip deathClip;

        AudioSource enemyAudio;
        ParticleSystem hitParticles;

        public int scoreValue => owner.scoreValue;

        public void BindEntity(BaseEntity entity){
            owner = entity as Enemy;
            owner.eventHandler = this;
        }

        void Awake(){
            // Setting up the references.
            anim = GetComponent<Animator>();
            enemyAudio = GetComponent<AudioSource>();
            hitParticles = GetComponentInChildren<ParticleSystem>();
        }

        void Update(){
            if (!_isSinking) {
                var pos = owner.transform.Pos3.ToVector3();
                transform.position = Vector3.Lerp(transform.position, pos, 0.3f);
                var deg = owner.transform.deg.ToFloat();
                //deg = Mathf.Lerp(transform.rotation.eulerAngles.y, deg, 0.3f);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, deg, 0), 0.3f);
            }
        }

        public void OnPlayerDied(){
            anim.SetTrigger("PlayerDead");
        }

        public void TakeDamage(int amount, LVector3 hitPoint){
            enemyAudio.Play();
            hitParticles.transform.position = hitPoint.ToVector3();
            hitParticles.Play();
        }


        public void Death(){
            anim.SetTrigger("Dead");
            enemyAudio.clip = deathClip;
            enemyAudio.Play();
        }


        public void StartSinking(){
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            ScoreManager.score += scoreValue;
            _isSinking = true;
            StartCoroutine(Sinking());
        }

        IEnumerator Sinking(){
            yield return null;
            var animatorInfo = anim.GetCurrentAnimatorStateInfo(0);
            while (true) {
                if (animatorInfo.normalizedTime >= 1.0f) 
                {
                    break;
                }
                yield return null;
            }

            float timer = 0;
            while (timer < 3) {
                timer += Time.deltaTime;
                transform.Translate(-Vector3.up * _sinkSpeed * Time.deltaTime);
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}