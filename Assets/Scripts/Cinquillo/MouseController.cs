using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace
Scripts.Cinquillo
{
    [RequireComponent(typeof(Collider2D))]
    public class MouseController : MonoBehaviour
    {
        HumanPlayer humanPlayer;
        CardController cardController;
        new Camera camera;
        new Collider2D collider2D;
        bool canBePlayed;
        bool isAnimationFinished = true;

        private void Awake()
        {
            camera = Camera.main;
            collider2D = GetComponent<Collider2D>();
        }

        internal void Setup(HumanPlayer humanPlayer, CardController cardController)
        {
            this.humanPlayer = humanPlayer;
            this.cardController = cardController;
        }

        private void OnMouseEnter()
        {
            if (isAnimationFinished && cardController != null)
            {
                // Debug.Log($"Entra {humanPlayer.name}: {cardController}");
                isAnimationFinished = false;
                Vector3[] path = { transform.position + 0.5f * Vector3.up, transform.position };
                cardController.card.transform.DOPath(path, 0.5f).SetEase(Ease.OutBack).OnComplete(() => { isAnimationFinished = true; });
            }
        }

        private void OnMouseUp()
        {
            // Debug.Log("OnMouseUp");
            if (canBePlayed)
            {
                humanPlayer.Play(cardController);
                Deactivate();
            }
            else
            {
                transform.position = cardController.initialPosition;
            }
        }

        private void OnMouseDrag()
        {
            var aux = camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(aux.x, aux.y, transform.position.z);
        }

        private void OnDestroy()
        {
            Destroy(collider2D);
        }

        internal void Deactivate()
        {
            collider2D.enabled = false;
        }

        internal void Activate()
        {
            if (humanPlayer.CanPlay(cardController))
            {
                collider2D.enabled = true;
            }
        }

        internal void CanBePlayed()
        {
            canBePlayed = true;
        }
    }
}