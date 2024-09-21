using RPG.Attributes;
using RPG.Control;
using RPG.Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float healthToRestore = 0;
        [SerializeField] float respawnTime = 5;

        public bool canPickUp = false;
        private void OnTriggerEnter(Collider other)
        {
            
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.gameObject); // old

                



                //playerController.GetComponent<Mover>().StartMoveAction(transform.position); // new
            }
          
        }

        private void Pickup(GameObject subject)
        {
            if (weapon != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }
            if (healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }
            
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds (float seconds)
        {
            ShowPickup(false);
            yield return new  WaitForSeconds(seconds);
            ShowPickup(true);
                
        }

        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            transform.GetChild(0).gameObject.SetActive(shouldShow);
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
             if (Input.GetMouseButtonDown(0))
             {
                 Pickup(callingController.gameObject);

             }
             return true;

            




        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}