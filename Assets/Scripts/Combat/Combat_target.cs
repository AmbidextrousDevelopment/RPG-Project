
using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]     //делает зависимость от другого скрипта, за счет чего другой скрипт невозможно убрать
    public class Combat_target : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
           


            if (!callingController.GetComponent<Fighter>().CanAttack(gameObject))
            {
                return false;
            }


            if (Input.GetMouseButton(0))
            {
                callingController.GetComponent<Fighter>().Attack(gameObject); //проблема 
            }
           
            return true;
        }
    }
}