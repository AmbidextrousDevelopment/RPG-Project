using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool AlreadyTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (!AlreadyTriggered && other.gameObject.tag == "Player") { 
            GetComponent<PlayableDirector>().Play();
                AlreadyTriggered = true;
            }
        }

    }
}