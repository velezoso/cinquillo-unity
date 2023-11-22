using UnityEngine;
using UnityEngine.Assertions;
using DG.Tweening;
using System.Collections;

namespace Scripts.Cinquillo
{
    public class WorldManager : MonoBehaviour
    {
        [SerializeField] UIManager uIManager;
        [SerializeField] CardsController cardsController;
        [SerializeField] Transform[] playerTransforms;
        [SerializeField] Transform heartsTransform;
        [SerializeField] Transform spadesTransform;
        [SerializeField] Transform diamondsTransform;
        [SerializeField] Transform cloversTransform;
        [SerializeField, Range(0.5f, 2f)] float movementAnimationTime = 2f;
        [SerializeField, Range(0.5f, 2f)] float showTextTime = 1f;
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
            PlayTurn();
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
            // Debug.Log($"-----------------INICIO------------------");s
            playerTurnIndex = Random.Range(0, players.Length);
        }


        public void PlayTurn()
        {
            if (!isGameFinished)
            {
                StartCoroutine(PlayTurnCoroutine());
            }
        }

        IEnumerator PlayTurnCoroutine()
        {
            uIManager.ShowText($"Turno\n{players[playerTurnIndex].name}", showTextTime);
            yield return new WaitForSeconds(showTextTime);
            players[playerTurnIndex].PlayTurn();
            playerTurnIndex++;
            if (playerTurnIndex == players.Length)
            {
                playerTurnIndex = 0;
            }
        }

        internal void GameFinished(string playerName)
        {
            // Debug.Log($"-----------------FIN------------------");
            isGameFinished = true;
            foreach (var player in players)
            {
                player.FinishGame();
            }

            uIManager.GameFinished($"Gana\n{playerName}", showTextTime);
        }

        public bool CanPlay(CardController cardController)
        {
            return cardsController.CanPlay(cardController);
        }

        internal void Play(string name, CardController cardController)
        {
            StartCoroutine(PlayCoroutine(name, cardController));
        }

        IEnumerator PlayCoroutine(string name, CardController cardController)
        {
            uIManager.ShowText($"{name} juega {cardController}", showTextTime);
            MoveCardToTablePosition(cardController);
            cardsController.UpdateCardsInTable(cardController);
            yield return new WaitForSeconds(showTextTime);
            StartCoroutine(NextTurnCoroutine());
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

        internal void Pass(string name)
        {
            StartCoroutine(PassCoroutine(name));
        }

        IEnumerator PassCoroutine(string name)
        {
            uIManager.ShowText($"Pasa\n{name}", showTextTime);
            yield return new WaitForSeconds(showTextTime);
            StartCoroutine(NextTurnCoroutine());
        }

        IEnumerator NextTurnCoroutine()
        {
            yield return new WaitForSeconds(showTextTime);
            PlayTurn();
        }
    }
}