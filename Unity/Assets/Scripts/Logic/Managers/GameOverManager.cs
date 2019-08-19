using UnityEngine;

namespace Lockstep.Logic {
    public class GameOverManager : MonoBehaviour {
        Animator anim;
        void Awake(){
            anim = GetComponent<Animator>();
        }
        void Update(){
            //if (GameManager.player != null && GameManager.player.currentHealth <= 0) {
            //    anim.SetTrigger("GameOver");
            //}
        }
    }
}