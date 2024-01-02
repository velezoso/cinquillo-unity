using UnityEngine;
using UnityEngine.Assertions;
using DG.Tweening;
using System.Collections;

namespace Scripts.Cinquillo
{
    public class WorldManagerWithCoroutines : MonoBehaviour, IWorldManager
    {
        [SerializeField] UIManager uIManager;
        [SerializeField] CardsController cardsController;
        [SerializeField] Transform[] playerTransforms;
        [SerializeField] Transform heartsTransform;
        [SerializeField] Transform spadesTransform;
        [SerializeField] Transform diamondsTransform;
        [SerializeField] Transform cloversTransform;
        [SerializeField, Range(0.5f, 2f)] float movementAnimationTime = 2f;
        [SerializeField, Range(0.5f, 2f)] float showTextDelay = 1f;
        [SerializeField] AbstractPlayer humanPlayer1;
        [SerializeField] AbstractPlayer humanPlayer2;
        [SerializeField] AbstractPlayer aiPlayer1;
        [SerializeField] AbstractPlayer aiPlayer2;
        AbstractPlayer[] players = new AbstractPlayer[2];
        int playerTurnIndex;
        bool isGameFinished;

        void Awake()
        {
            Assert.IsNotNull(uIManager, "ERROR olvidaste UIManager");
            Assert.IsNotNull(cardsController, "ERROR olvidaste Cartas");
            Assert.IsNotNull(humanPlayer1, "ERROR olvidaste prefab human");
            Assert.IsNotNull(aiPlayer2, "ERROR olvidaste prefab ai");
            Assert.IsTrue(playerTransforms.Length > 0, "ERROR olvidaste Posiciones Jugadores");

            uIManager.Setup(this);
        }

        public void Play(int numberOfPlayers)
        {
            isGameFinished = false;

            SetupPlayers(numberOfPlayers);

            cardsController.Shuffle(players, playerTransforms);

            foreach (var player in players)
            {
                player.Setup(this);
            }

            ChooseRandomTurn();
            ChangeTurn();
        }

        void SetupPlayers(int numberOfPlayers)
        {
            switch (numberOfPlayers)
            {
                case 0:
                    players[0] = aiPlayer1;
                    players[1] = aiPlayer2;
                    break;
                case 1:
                    players[0] = humanPlayer1;
                    players[1] = aiPlayer2;
                    break;
                case 2:
                    players[0] = humanPlayer1;
                    players[1] = humanPlayer2;
                    break;
            }
        }

        void ChooseRandomTurn()
        {
            // Debug.Log($"-----------------INICIO------------------");
            playerTurnIndex = Random.Range(0, players.Length);
        }


        public void ChangeTurn()
        {
            if (!isGameFinished)
            {
                StartCoroutine(PlayTurnCoroutine());
            }
        }

        IEnumerator PlayTurnCoroutine()
        {
            uIManager.DisplayText($"Turno\n{players[playerTurnIndex].name}", showTextDelay);
            yield return new WaitForSeconds(showTextDelay);
            players[playerTurnIndex].PlayTurn();
            playerTurnIndex++;
            if (playerTurnIndex == players.Length)
            {
                playerTurnIndex = 0;
            }
        }

        public void GameFinished(string playerName)
        {
            // Debug.Log($"-----------------FIN------------------");
            isGameFinished = true;
            foreach (var player in players)
            {
                player.FinishGame();
            }

            uIManager.GameFinished($"Gana\n{playerName}", showTextDelay);
        }

        public bool CanPlay(CardController cardController)
        {
            return cardsController.CanPlay(cardController);
        }

        public void Play(string playerName, CardController cardController)
        {
            StartCoroutine(PlayCoroutine(playerName, cardController));
        }

        IEnumerator PlayCoroutine(string playerName, CardController cardController)
        {
            uIManager.DisplayText($"{playerName} juega {cardController}", showTextDelay);
            MoveCardToTablePosition(cardController);
            cardsController.UpdateCardsInTable(cardController);
            yield return new WaitForSeconds(showTextDelay);
            ChangeTurn();
        }

        void MoveCardToTablePosition(CardController cardController)
        {
            Transform parent = null;
            if (cardController.deckName == "Clovers")
            {
                parent = cloversTransform;
            }
            if (cardController.deckName == "Diamonds")
            {
                parent = diamondsTransform;
            }
            if (cardController.deckName == "Hearts")
            {
                parent = heartsTransform;
            }
            if (cardController.deckName == "Spades")
            {
                parent = spadesTransform;
            }

            int diff = cardController.numberCard - 5;
            GameObject card = cardController.card;
            card.GetComponent<SpriteRenderer>().sortingOrder = diff;

            card.transform.DOMove(parent.transform.position + diff * 0.4f * Vector3.up, movementAnimationTime).SetEase(Ease.OutBack);
        }

        public void Pass(string playerName)
        {
            StartCoroutine(PassCoroutine(playerName));
        }

        IEnumerator PassCoroutine(string playerName)
        {
            uIManager.DisplayText($"Pasa\n{playerName}", showTextDelay);
            yield return new WaitForSeconds(showTextDelay);
            ChangeTurn();
        }

        public void TextDisplayed() { }
    }
}