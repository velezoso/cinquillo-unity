using UnityEngine;

namespace Scripts.Cinquillo
{
    public class AIPlayer : AbstractPlayer
    {
        public override void PlayConcreteTurn()
        {
            CardController cardSelected = null;

            foreach (var card in cardsToPlay)
            {
                if (worldManager.CanPlay(card))
                {
                    cardSelected = card;
                    break;
                }
            }

            Play(cardSelected);
        }
    }
}