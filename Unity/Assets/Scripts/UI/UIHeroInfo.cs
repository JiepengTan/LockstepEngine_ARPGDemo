using Lockstep.Logic;
using Lockstep;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace {
    public class UIHeroInfo : UnityEngine.MonoBehaviour {
        [HideInInspector] public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
        [HideInInspector] public Slider healthSlider;
        [HideInInspector] public Image damageImage;
        [Header("Health")] public float flashSpeed = 5f;

        private void Start(){
            healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
            damageImage = GameObject.Find("DamageImage").GetComponent<Image>();
            EventHelper.AddListener(EEvent.OnPlayerBeAtk, OnPlayerBeAtk);
        }

        void OnPlayerBeAtk(object param){
            damageImage.color = flashColour;
            TakeDamage();
        }

        void Update(){
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        public void TakeDamage(){
            healthSlider.value = GameManager.player.currentHealth;
        }
    }
}