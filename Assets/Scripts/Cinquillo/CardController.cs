using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Cinquillo
{
    [Serializable]
    public class CardController
    {
        public int numberCard { get; private set; }
        public string deckName { get; private set; }
        public GameObject card { get; private set; }
        public Vector2 initialPosition { get; set; }

        public CardController(int cardNumber, string deckName, Sprite sprite, GameObject cardPrefab)
        {
            this.numberCard = cardNumber;
            this.deckName = deckName;
            card = GameObject.Instantiate(cardPrefab);
            var spriteRenderer = card.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
        }

        public bool IsNext(CardController cardController)
        {
            return cardController.deckName == deckName && cardController.numberCard == (numberCard + 1);
        }

        public bool IsPrevious(CardController cardController)
        {
            return cardController.deckName == deckName && cardController.numberCard == (numberCard - 1);
        }

        public bool IsFive(CardController cardController)
        {
            return cardController.numberCard == 5;
        }

        public override string ToString()
        {
            return $"{numberCard} {deckName}";
        }
    }
}