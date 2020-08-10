using Everybody_Edits_CTF.Core.Bot.Enums;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class GameFund
    {
        /// <summary>
        /// The total amount of coins raised for the current Capture The Flag game.
        /// </summary>
        public static int CoinsRaised;

        /// <summary>
        /// Increases the <see cref="CoinsRaised"/> by a certain amount of coins, depending on the reason.
        /// </summary>
        /// <param name="reason">The reason why the game fund is being increased.</param>
        public static void Increase(GameFundIncreaseReason reason)
        {
            switch (reason)
            {
                case GameFundIncreaseReason.FlagCaptured:
                    CoinsRaised += 25;
                    break;
                case GameFundIncreaseReason.FlagReturned:
                case GameFundIncreaseReason.FlagTaken:
                    CoinsRaised += 10;
                    break;
                default:
                case GameFundIncreaseReason.PlayerKilledEnemy:
                    CoinsRaised += 5;
                    break;
            }
        }
    }
}