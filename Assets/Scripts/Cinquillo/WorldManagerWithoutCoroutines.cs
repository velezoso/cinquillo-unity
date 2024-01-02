using UnityEngine;
using UnityEngine.Assertions;
using DG.Tweening;
using System.Collections;

namespace Scripts.Cinquillo
{
    enum GameState
    {
        NOTPLAYING, CHANGETURN, PASS, PLAY, FINISHED
    }

    enum PlayerText
    {
        NOTDISPLAYED, DISPLAYING, DISPLAYED
    }

    public class WorldManagerWithoutCoroutines : MonoBehaviour, IWorldManager
    {
        [SerializeField] UIManager uIManager;
        [SerializeField] CardsController cardsController;
        [SerializeField] Transform[] playerTransforms;
        [SerializeField] Transform heartsTransform;
        [SerializeField] Transform spadesTransform;
        [SerializeField] Transform diamondsTransform;
        [SerializeField] Transform cloversTransform;
        [SerializeField, Range(0.5f, 2f)] float movementAnimationTime = 2f;
        [SerializeField, Range(0.5f, 2f)] float displayTextDelay = 1f;
        [SerializeField] AbstractPlayer humanPlayer1;
        [SerializeField] AbstractPlayer humanPlayer2;
        [SerializeField] AbstractPlayer aiPlayer1;
        [SerializeField] AbstractPlayer aiPlayer2;
        AbstractPlayer[] players = new AbstractPlayer[2];
        int playerTurnIndex;
        GameState gameState;
        CardController cardController;
        PlayerText playerText;

        void Awake()
        {
            Assert.IsNotNull(uIManager, "ERROR olvidaste UIManager");
            Assert.IsNotNull(cardsController, "ERROR olvidaste Cartas");
            Assert.IsNotNull(humanPlayer1, "ERROR olvidaste prefab human");
            Assert.IsNotNull(aiPlayer2, "ERROR olvidaste prefab ai");
            Assert.IsTrue(playerTransforms.Length > 0, "ERROR olvidaste Posiciones Jugadores");

            uIManager.Setup(this);
            gameState = GameState.NOTPLAYING;
        }

        private void Update()
        {
            switch (gameState)
            {
                case GameState.NOTPLAYING:
                    break;
                case GameState.CHANGETURN:
                    switch (playerText)
                    {
                        case PlayerText.NOTDISPLAYED:
                            playerTurnIndex++;
                            if (playerTurnIndex == players.Length)
                            {
                                playerTurnIndex = 0;
                            }
                            uIManager.DisplayText($"Turno\n{players[playerTurnIndex].name}", displayTextDelay);
                            playerText = PlayerText.DISPLAYING;
                            break;
                        case PlayerText.DISPLAYED:
                            players[playerTurnIndex].PlayTurn();
                            playerText = PlayerText.NOTDISPLAYED;
                            break;
                    }
                    break;
                case GameState.PASS:
                    switch (playerText)
                    {
                        case PlayerText.NOTDISPLAYED:
                            uIManager.DisplayText($"Pasa\n{players[playerTurnIndex].name}", displayTextDelay);
                            playerText = PlayerText.DISPLAYING;
                            break;
                        case PlayerText.DISPLAYED:
                            gameState = GameState.CHANGETURN;
                            playerText = PlayerText.NOTDISPLAYED;
                            break;
                    }
                    break;
                case GameState.PLAY:
                    switch (playerText)
                    {
                        case PlayerText.NOTDISPLAYED:
                            uIManager.DisplayText($"{players[playerTurnIndex].name} juega {cardController}", displayTextDelay);
                            playerText = PlayerText.DISPLAYING;
                            break;
                        case PlayerText.DISPLAYED:
                            MoveCardToTablePosition(cardController);
                            cardsController.UpdateCardsInTable(cardController);
                            gameState = GameState.CHANGETURN;
                            playerText = PlayerText.NOTDISPLAYED;
                            break;
                    }
                    break;
                case GameState.FINISHED:
                    MoveCardToTablePosition(cardController);
                    cardsController.UpdateCardsInTable(cardController);
                    foreach (var player in players)
                    {
                        player.FinishGame();
                    }
                    uIManager.GameFinished($"Gana\n{players[playerTurnIndex].name}", displayTextDelay);
                    gameState = GameState.NOTPLAYING;
                    break;
            }
            // Debug.Log($"{gameState} - {playerText}");
        }

        public void Play(int numberOfPlayers)
        {
            SetupPlayers(numberOfPlayers);

            cardsController.Shuffle(players, playerTransforms);

            foreach (var player in players)
            {
                player.Setup(this);
            }

            ChooseRandomTurn();

            gameState = GameState.CHANGETURN;
            playerText = PlayerText.NOTDISPLAYED;
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

        public void GameFinished(string playerName)
        {
            // Debug.Log($"-----------------FIN------------------");
            gameState = GameState.FINISHED;
        }

        public bool CanPlay(CardController cardController)
        {
            return cardsController.CanPlay(cardController);
        }

        public void Play(string playerName, CardController cardController)
        {
            this.cardController = cardController;
            gameState = GameState.PLAY;
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
            gameState = GameState.PASS;
        }

        public void TextDisplayed()
        {
            playerText = PlayerText.DISPLAYED;
        }
    }
}