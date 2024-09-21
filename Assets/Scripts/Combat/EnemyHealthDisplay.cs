using RPG.Attributes;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>(); //берем скрипт конкретно игрока, чтоб отображать в дальнейшем конкретно его таргет
        }

        private void Update()
        {
            if (fighter.GetTarget() == null)
            {
                GetComponent<Text>().text = "N/A";
            }
            else
            {
                Health health = fighter.GetTarget();
                GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());  // конвертирует флоат в стринг таким образом, чтоб оно отображалось как инт (хз за счёт чего)
            }
        }
    }
}

