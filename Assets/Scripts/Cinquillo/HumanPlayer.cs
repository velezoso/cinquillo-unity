using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Scripts.Cinquillo
{
    public class HumanPlayer : AbstractPlayer
    {
        public override void Add(CardController cardController)
        {
            base.Add(cardController);
            var mouseController = cardController.card.GetComponent<MouseController>();
            mouseController.Setup(this, cardController);
        }

        public override void PlayConcreteTurn()
        {
            foreach (var cardController in cardsToPlay)
            {
                var mouseController = cardController.card.GetComponent<MouseController>();
                mouseController.Activate();
            }
        }

        public override void Play(CardController cardSelected)
        {
            base.Play(cardSelected);
            foreach (var cardController in cardsToPlay)
            {
                var mouseController = cardController.card.GetComponent<MouseController>();
                mouseController.Deactivate();
            }

        }

        public bool CanPlay(CardController cardSelected)
        {
            return worldManager.CanPlay(cardSelected);
        }
    }
}