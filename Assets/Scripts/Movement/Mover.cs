
using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Attributes;
using RPG.Saving;
using System;

namespace RPG.Movement   //больше не зависит от комбата
{
    public class Mover : MonoBehaviour, IAction, RPG.Saving.ISaveable
    {

        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6f;
        private NavMeshAgent navMeshAgent;
        Health health;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }


        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();

            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);    
          
            MoveTo(destination, speedFraction);

        }

        public void MoveTo(Vector3 destination, float speedFraction) //идет к цели
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        

        public void Cancel()
        {
       
            navMeshAgent.isStopped = true;  
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        #region SavePosition
        public object CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles); //representation of rotation (poker face)
            return data;
        }

        

        public void RestoreState(object state)                      
        {
            Dictionary<string, object> data = (Dictionary<string, object>)state;
            navMeshAgent.enabled = false;
            transform.position = ((SerializableVector3) data["position"]).ToVector();
            transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
            navMeshAgent.enabled = true;
        }
        #endregion         //после новых данных такого типа надо удалить save-файл, ибо он пошлет тебя нахуй
    }
}
