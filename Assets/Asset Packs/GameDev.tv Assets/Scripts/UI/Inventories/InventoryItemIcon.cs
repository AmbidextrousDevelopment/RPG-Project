using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Inventories;
using TMPro;
using UnityEngine.EventSystems;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// To be put on the icon representing an inventory item. Allows the slot to
    /// update the icon and number.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour, IPointerClickHandler
    {
        // CONFIG DATA
        [SerializeField] GameObject textContainer = null;
        [SerializeField] TextMeshProUGUI itemNumber = null;

        // PUBLIC
        bool itemExist;

        public void SetItem(InventoryItem item)
        {
            SetItem(item, 0);
        }

        public void SetItem(InventoryItem item, int number)
        {
            var iconImage = GetComponent<Image>();
            if (item == null)
            {
                iconImage.enabled = false;

                itemExist = false;
            }
            else
            {
                iconImage.enabled = true;
                iconImage.sprite = item.GetIcon();

                itemExist = true;
            }

            if (itemNumber)
            {
                if (number <= 1)
                {
                    textContainer.SetActive(false);
                }
                else
                {
                    textContainer.SetActive(true);
                    itemNumber.text = number.ToString();
                }
            }
        }

         public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (itemExist)
            {

                if (eventData.clickCount == 2)
                {
                    
                    // переместить слот 
                    Debug.Log("suka");
                }
           }
        }
    }
}