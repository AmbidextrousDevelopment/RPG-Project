
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;
using RPG.Utils;
using UnityEngine.Events;
namespace RPG.Attributes
{
    public class Health : MonoBehaviour, RPG.Saving.ISaveable
    {
        LazyValue<float> health;         //там был баг, хп после 2 сейвов возвращалось к дефолту, минус это фиксит
        [SerializeField] float regenerationPercentage = 100;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>     //serializefield не может работать с таким типом данных, так что ивент юзается с флоатом из другого класса
        {
        }

        bool isDead = false;

        private void Awake()
        {
            health = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start()
        {
            health.ForceInit();
            
             
        }

        

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenateHealth;
        }

        public bool IsDead()
        {
            return isDead;
        }

       

        public void TakeDamage(GameObject instigator, float damage)
        {
           

            health.value = Mathf.Max(health.value - damage, 0);
            takeDamage.Invoke(damage);

            if (health.value == 0)
            {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }

        public void Heal(float healthToRestore)
        {
            health.value = Mathf.Min(health.value + healthToRestore, GetMaxHealthPoints());
        }

        public float GetHealthPoints()
        {
            return health.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return 100 * (GetFraction());
        }

        public float GetFraction()
        {
            return  (health.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        private void Die()
        {
            if (isDead) return;   //неебу что конкретно делает этот return
            isDead = true;

            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AwardExperience(GameObject instigator)
        {
           Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
      
        }

        private void RegenateHealth()
        {
            float  regenHealth = GetComponent<BaseStats>().GetStat(Stat.Health) *(regenerationPercentage / 100);
            health.value = Mathf.Max(health.value, regenHealth);
        }

        #region Save Health

        public object CaptureState()
        {
            return health.value;
        }


        public void RestoreState(object state)
        {
            health.value = (float)state;

            if (health.value <= 0)
            {
                Die();
            }
        }

        #endregion

    }
}

