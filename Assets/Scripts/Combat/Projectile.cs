using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {

        [SerializeField] float speed = 1;
        [SerializeField] bool isHoming = true;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 0.2f;
        [SerializeField] UnityEvent onHit; 
        Health target = null;
        GameObject instigator = null;
        float damage = 0;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (target == null) return;

            if (isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }

            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.instigator = instigator;
            this.damage = damage;
            
            Destroy(gameObject, maxLifeTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height / 2;                  //Чтоб не стрелять в ноги, а в центр, берется высота капсулы цели и делится на 2

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target)  //если другой коллайдер не таргет, шлет всё нахуй   //я бы заменил на дестрой, чтоб через стены не проходило
            {
               // Instantiate(hitEffect, transform.position, transform.rotation);
               // Destroy(gameObject);
                return;
            }
            if (target.IsDead()) return;
            target.TakeDamage(instigator, damage);

            onHit.Invoke();
            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in destroyOnHit)           //хрень чтоб отдельно на тайминге дестроить отдельные части
            {
                Destroy(toDestroy);
            }

            speed = 0;

            Destroy(gameObject, lifeAfterImpact);
        }


    }
}