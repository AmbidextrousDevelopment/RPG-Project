
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using RPG.Utils;
using System;
using RPG.Inventories;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable,  IModifierProvider
    {
        
        [SerializeField] float Time_between_attacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;

        //public Vector3 Offset;

        // [SerializeField] string defaultWeaponName = "Unarmed";   //если в ресурсах оно в папке, стоит делать "folder/Unarmed"

        Health target;
        Equipment equipment;   //инвентарь 

        float time_since_last_attack = Mathf.Infinity;

        WeaponConfig currentWeaponConfig = null;
        LazyValue <Weapon> currentWeapon;

        Transform spine; //mine
        public Transform suka;
        private void Awake()
        {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);

            //spine = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Chest); 

            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        private Weapon SetupDefaultWeapon()
        {
            
            return AttachWeapon(defaultWeapon);
        }

        private void Start()
        {
            // Weapon weapon = Resources.Load<Weapon>(defaultWeaponName);    //берет прямо из папки ресурсов, чтоб загружать через сцені

            currentWeapon.ForceInit();
            
        }

        /*private void LateUpdate() //TO DO
        {
            if (suka == null) return;
            suka.rotation = suka.rotation * Quaternion.Euler(Offset);
            if (target == null) return;

            

            if (target.transform.position.y > transform.position.y)
            {
                Offset.z = 
            }
            //spine.LookAt(new Vector3(0, suka.transform.position.y, 0));
            //spine.LookAt(new Vector3(transform.position.x, transform.position.y, target.transform.position.z));
        }*/


        private  void Update()
        {
            time_since_last_attack += Time.deltaTime;

            if (target == null) return;

            if (target.IsDead()) return;
            

            if (!GetIsInRange()) // по сути продолжает идти к таргету, пока атака включена
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover>().Cancel();   //выключает NavMeshAgent, дабы таргет остановился до цели в ренже атаки
                AttackBehavior();
            }
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;    
            currentWeapon.value = AttachWeapon(weapon); //меняет аниматор на указанный //хотя уже не уверен, это было так давно что пиздец
        }

        private void UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weapon == null)
            {
                EquipWeapon(defaultWeapon);              //если нет оружки в слоте, она меняется на дефолтную (Unarmed)
            }
            else
            {
                EquipWeapon(weapon);
            }
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);  //смена оружки получается тут
        }

        public Health GetTarget()
        {
            return target;

        }

        private void AttackBehavior()  //собственно атака
        {
            /*Vector3 target_point;
            target_point = target.transform.position;
            target_point.y = 0;
            transform.LookAt(target_point);*/
           
            //transform.LookAt(target.transform); //проблема в этом
            transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z)); //способ без y, который надо исправить
            

            if (time_since_last_attack > Time_between_attacks)
            {
                TriggerAttack();
                time_since_last_attack = 0;

            }

        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetRange(); //собственно делает позицию МЕЖДУ собой и таргетом меньше ренжа оружия
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget) 
        {

            target = combatTarget.GetComponent<Health>();
            GetComponent<ActionScheduler>().StartAction(this);
        }

        public void Cancel()  //отрубает таргета, что отменяет режим атаки
        {
            target = null;
            StopAttack();
            GetComponent<Mover>().Cancel();
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {

            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }

        void Hit()  //animation event
        {
           if (target == null) { return; }

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage); //референс дамага
            //*
            BaseStats targetBaseStats = target.GetComponent<BaseStats>();  //берется из статов таргета
            if (targetBaseStats != null)
            {
                float defence = targetBaseStats.GetStat(Stat.Defence); //референс дефенса 
               /* if (defence == 0)
                {
                    defence = 1;
                }*/
                damage /= 1 + defence / damage;
            }
            //*
            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }
            
            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
               
                target.TakeDamage(gameObject, damage);
            }

          
        }

        void Shoot()
        {
            Hit();
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);    //аккуратнее с этой херней, если оружка не в ресурсах, пошлет тебя нахуй
            EquipWeapon(weapon);
        }

       
    }
}