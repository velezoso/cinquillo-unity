using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Cinquillo
{
    public abstract class AbstractPlayer : MonoBehaviour
    {
        [SerializeField] protected List<CardController> cardsToPlay = new List<CardController>();

        protected IWorldManager worldManager;

        public virtual void Add(CardController cardController)
        {
            // Debug.Log($"{name}: {cardController}");
            cardsToPlay.Add(cardController);
            var mouseController = cardController.card.GetComponent<MouseController>();
            mouseController.Deactivate();
        }

        public void PlayTurn()
        {
            if (CanPlay())
            {
                PlayConcreteTurn();
            }
            else
            {
                worldManager.Pass(name);
            }
        }

        bool CanPlay()
        {
            foreach (var card in cardsToPlay)
            {
                if (worldManager.CanPlay(card))
                {
                    return true;
                }
            }

            return false;
        }

        public abstract void PlayConcreteTurn();

        public virtual void Play(CardController cardSelected)
        {
            worldManager.Play(name, cardSelected);
            cardsToPlay.Remove(cardSelected);

            if (cardsToPlay.Count == 0)
            {
                worldManager.GameFinished(name);
            }
        }

        internal void Setup(IWorldManager worldManager)
        {
            this.worldManager = worldManager;
        }

        internal void FinishGame()
        {
            cardsToPlay.Clear();
        }
    }
}