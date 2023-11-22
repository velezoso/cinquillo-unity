using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scripts.Cinquillo
{
    [RequireComponent(typeof(Collider2D))]
    public class CardSelectorController : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            MouseController mouseController = other.GetComponent<MouseController>();

            if (mouseController != null)
            {
                mouseController.CanBePlayed();
            }
        }

    }
}