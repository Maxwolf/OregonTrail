using TrailEntities;

namespace TrailGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            InitializeGame oregonTrailGame = new InitializeGame();
            oregonTrailGame.ChooseProfession();
            oregonTrailGame.BuyInitialItems();
            oregonTrailGame.StartGame();
        }
    }
}