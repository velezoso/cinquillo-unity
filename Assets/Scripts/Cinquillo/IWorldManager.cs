
namespace Scripts.Cinquillo
{
    public interface IWorldManager
    {
        bool CanPlay(CardController card);
        void GameFinished(string playerName);
        void Pass(string playerName);
        void Play(string playerName, CardController cardSelected);
        void Play(int numberOfPlayers);
        void TextDisplayed();
    }
}