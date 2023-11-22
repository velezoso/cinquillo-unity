using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Scripts.Cinquillo
{
    public class CardsController : MonoBehaviour
    {
        [SerializeField] GameObject cardPrefab;
        [SerializeField] List<string> deckNames = new List<string> { "Clovers", "Diamonds", "Hearts", "Spades" };
        [SerializeField] int[] cardNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        [SerializeField] List<Sprite> cards;
        List<CardController> cardsToPlay = new List<CardController>();
        List<CardController> cardsInTable = new List<CardController>();

        private void Awake()
        {
            Assert.IsNotNull(cardPrefab, "ERROR olvidaste Prefab Carta");
            Assert.IsTrue(cards.Count > 0, "ERROR olvidaste las cartas");

            foreach (var deckName in deckNames)
            {
                foreach (var cardNumber in cardNumbers)
                {
                    Sprite sprite = cards.Find(card => card.name == deckName + "_" + cardNumber);
                    CardController item = new CardController(cardNumber, deckName, sprite, cardPrefab);
                    cardsToPlay.Add(item);
                }
            }
        }

        internal void Shuffle(AbstractPlayer[] players, Transform[] transforms)
        {
            ClearCardsInTable();

            int playerIndex = 0;

            // Hacemos una copia del mazo
            List<CardController> copy = new List<CardController>(cardsToPlay);

            while (copy.Count > 0)
            {
                // Seleccionamos la carta
                int ramdonIndex = Random.Range(0, copy.Count);
                CardController cardController = copy[ramdonIndex];

                // Obtenemos la ubicación inicial de la carta espacialmente
                Transform playerTransform = transforms[playerIndex];
                Transform cardTransform = cardController.card.transform;
                cardTransform.SetParent(playerTransform);
                int numberDiff = cardController.numberCard - 5;
                int index = deckNames.FindIndex(name => name == cardController.deckName);

                // Añadimos la carta al jugador
                cardController.initialPosition = playerTransform.position + (numberDiff * 0.2f + index * 4) * Vector3.right;
                cardTransform.position = cardController.initialPosition;
                players[playerIndex].Add(cardController);

                // Ubicamos la carta en el order in layer
                var spriteRenderer = cardController.card.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = numberDiff;

                playerIndex++;
                if (playerIndex == players.Length)
                {
                    playerIndex = 0;
                }

                // Eliminamos la carta seleccionada de la copia del mazo
                copy.Remove(copy[ramdonIndex]);
            }
        }

        void ClearCardsInTable()
        {
            foreach (var card in cardsInTable)
            {
                cardsToPlay.Add(card);
            }

            cardsInTable.Clear();
        }

        public bool CanPlay(CardController cardController)
        {
            if (cardController.IsFive(cardController))
            {
                return true;
            }

            foreach (var cardInTable in cardsInTable)
            {
                if (cardController.IsNext(cardInTable) || cardController.IsPrevious(cardInTable))
                {
                    return true;
                }
            }

            return false;
        }

        public void UpdateCardsInTable(CardController cardController)
        {
            cardsInTable.Add(cardController);
            cardsToPlay.Remove(cardController);
        }
    }
}