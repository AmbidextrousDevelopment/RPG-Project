using RPG.Core;
using UnityEngine;
using RPG.Attributes;
using RPG.Inventories;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Combat //по идее, из-за невозможности инвентарь эквипа, у врагов не работает модифаер отсюда, но не уверен
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem //IModifierProvider //до интерграции инвентаря это был ScriptableObject 
     //IModifierProvider работает ток у гг, (cтаты оружки нахуй шлют) так что он перемещен в файтер
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] float weapon_damage = 5f;
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] float weapon_range = 2f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null; // АААААААААААААААААААААААААААААА УБЕЙТЕ БЛЯТЬ, крч эта поебень нужна, чтоб войд превращать в weapon в этой функции, чтоб ебаный return в файтере срабатывал 

            if (equippedPrefab != null)               //если есть префаб - оружие появляется
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;            //не спрашивай, я нихуя не понял, в теории это фикс того, что оверрайд не менялся, когда есть другой оверрайд

            if (animatorOverride != null)  //если есть оверрайд анимация - она проигрывается 
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)                 
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
                
            }
            return weapon;
            
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null)
            {
                return;
            }
            oldWeapon.name = "DESTROYING"; //смена имени чтоб не было проблем с подбиранием новых слотов
            Destroy(oldWeapon.gameObject);
            
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, /*weapon_damage*/ calculatedDamage); //дмг от статов вместо оружки
        }


        public float GetDamage()
        {
            return weapon_damage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public float GetRange()
        {
            return weapon_range;
        }

      /*  public IEnumerable<float> GetAdditiveModifiers(Stat stat)       
        {
            if (stat == Stat.Damage)
            {
                yield return weapon_damage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return percentageBonus;
            }
        }*/
    }
}