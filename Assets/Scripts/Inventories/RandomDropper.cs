using RPG.Inventories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        [Tooltip("How far can pickups be scattered from dropper")]
        [SerializeField] float scatterDistance;
        [SerializeField] InventoryItem[] dropLibrary;
        [SerializeField] int numberOfDrops = 2;

        const int ATTEMPTS = 30;

        public void RandomDrop()   //для врагов           //вызывается в ивенте "OnDie" в инспекторе
        {
            for (int i = 0; i < numberOfDrops; i++)
            {
                var item = dropLibrary[Random.Range(0, dropLibrary.Length)]; //ренж от первого (0) до числа длины массива (dropLibrary.Length)
                DropItem(item, 1);
            }
             
        }

        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < ATTEMPTS; i++)
            {

                Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return transform.position;
        }
    }
}