
using System.Threading;

namespace Battleships
{
    static class GameInterface
    {
        public static void PlayGame()
        {
            var game = new GameLogic();
            PlayersTakeTurns(game);
        }

        // Main game loop.
        // Player 1 vs the computer.
        public static void PlayersTakeTurns(GameLogic game)
        {

            System.Console.WriteLine("The game begins !!!");

            // Game loop.
            while (game.Winner() == 0)
            {
                PlayerShotInput(game, 1);
                System.Console.Clear();

                if (game.Winner() != 0) { break; }

                PlayerShotInput(game, 2);
                System.Console.Clear();
            }

            // Announce the winner and print their ship board.
            if(game.Winner() == -1)
            {
                System.Console.WriteLine();
                System.Console.WriteLine(game.Player1Name + " won the game !!!");
                System.Console.WriteLine(game.Player1Name + "'s ship board:");
                PrintShipBoard(game.Player1ShipBoard);
                System.Console.WriteLine(game.Player1Name + "'s shot board:");
                PrintShotBoard(game.Player1ShotBoard);
                System.Console.WriteLine(game.Player2Name + "'s ship board:");
                PrintShipBoard(game.Player2ShipBoard);
                System.Console.WriteLine(game.Player2Name + "'s shot board:");
                PrintShotBoard(game.Player2ShotBoard);
            }
            else
            {
                System.Console.WriteLine();
                System.Console.WriteLine(game.Player2Name + " won the game !!!");
                System.Console.WriteLine(game.Player2Name + "'s ship board:");
                PrintShipBoard(game.Player2ShipBoard);
                System.Console.WriteLine(game.Player2Name + "'s shot board:");
                PrintShotBoard(game.Player2ShotBoard);
                System.Console.WriteLine(game.Player1Name + "'s ship board:");
                PrintShipBoard(game.Player1ShipBoard);
                System.Console.WriteLine(game.Player1Name + "'s shot board:");
                PrintShotBoard(game.Player1ShotBoard);
            }
        }
        //---------------------------------------------------------------------------------------------

        // Takes input from console and marks down the shot.
        // 'whichPlayer == 1' - player 1
        // 'whichPlayer != 1' - player 2
        // Returns the square that was hit.
        // NOTE: this function looks ugly and should be rewritten!
        private static GameLogic.BoardSquares PlayerShotInput(GameLogic game, byte whichPlayer)
        {
            string buff;
            do
            {
                // Print relevant information for the player.
                PrintGameInfo(game);
                if (whichPlayer == 1)
                {
                    System.Console.WriteLine(game.Player1Name + "'s turn.\n" + game.Player1Name + "'s shot board:");
                    PrintShotBoard(game.Player1ShotBoard);
                }
                else
                {
                    System.Console.WriteLine(game.Player2Name + "'s turn.\n" + game.Player2Name + "'s shot board:");
                    PrintShotBoard(game.Player2ShotBoard);
                }
                System.Console.WriteLine("Write the coordinates for the shot (ex: 'BD').");
                System.Console.Write("If you just want to mark the square write '+' after the coordinates (ex: 'BD+'): ");

                buff = System.Console.ReadLine();
                // Check if coordinates are valid.
                if (!System.String.IsNullOrEmpty(buff) && (buff.Length == 2 || buff.Length == 3) &&
                    game.IsValidCoord((byte)(buff[0] - 'A'), (byte)(buff[1] - 'A')))
                {
                    if (buff.Length == 2)   // Player wants to shoot.
                    {
                        if (whichPlayer == 1) { return game.Player1MakeAShot((byte)(buff[0] - 'A'), (byte)(buff[1] - 'A')); }
                        else { return game.Player2MakeAShot((byte)(buff[0] - 'A'), (byte)(buff[1] - 'A')); }
                    }
                    else if (buff.Length == 3 && buff[2] == '+')    // Player wants to mark the square.
                    {
                        System.Console.Clear();
                        System.Console.WriteLine(buff + " was marked.");
                        if (whichPlayer == 1) { game.Player1Mark((byte)(buff[0] - 'A'), (byte)(buff[1] - 'A')); }
                        else { game.Player2Mark((byte)(buff[0] - 'A'), (byte)(buff[1] - 'A')); }
                        continue;  // The player hasn't made a shot so continue.
                    }
                }
                System.Console.Clear();
                System.Console.WriteLine("Invalid coordinates! Try again.");
            } while (true);   
        }

        //---------------------------------------------------------------------------------------------

        // The coordinates are represented as english alphabet letters to if 'howManyColumns' is more than 26 (the number
        // of letters ih the alphabet) the output won't be nice looking.
        private static void PrintColumnCoordinates(int howManyColumns)
        {
            System.Console.Write("   ");
            char letter = 'A';
            for (int i = 0; i < howManyColumns; i++, letter++) {
                System.Console.Write(letter + " ");
            }
            System.Console.Write("\n  ");
            for (int i = 0; i < howManyColumns; i++) {
                System.Console.Write("__");
            }
            System.Console.Write('\n');
        }
        private static void PrintShipBoard(GameLogic.BoardSquares[,] shipBoard)
        {
            PrintColumnCoordinates(shipBoard.GetLength(1));

            char letter = 'A';
            for (int i = 0; i < shipBoard.GetLength(0); ++i)
            {
                System.Console.Write((letter++) + "| ");
                for (int j = 0; j < shipBoard.GetLength(1); ++j)
                {
                    char symbol;
                    switch (shipBoard[i, j])
                    {
                        case GameLogic.BoardSquares.PATROL_BOAT: symbol = 'P'; break;
                        case GameLogic.BoardSquares.SUBMARINE:   symbol = 'S'; break;
                        case GameLogic.BoardSquares.DESTROYER:   symbol = 'D'; break;
                        case GameLogic.BoardSquares.BATTLESHIP:  symbol = 'B'; break;
                        case GameLogic.BoardSquares.CARRIER:     symbol = 'C'; break;
                        default:                                 symbol = '_'; break;
                    }
                    System.Console.Write(symbol + " ");
                }
                System.Console.Write('\n');
            }
        }
        private static void PrintShotBoard(GameLogic.PlayerInfo.Shots[,] shotBoard)
        {
            PrintColumnCoordinates(shotBoard.GetLength(1));

            char letter = 'A';
            for (int i = 0; i < shotBoard.GetLength(0); ++i)
            {
                System.Console.Write((letter++) + "| ");
                for (int j = 0; j < shotBoard.GetLength(1); ++j)
                {
                    char symbol = '\0';
                    switch (shotBoard[i, j])
                    {
                        case GameLogic.PlayerInfo.Shots.NO_SHOT:    symbol = '_'; break;
                        case GameLogic.PlayerInfo.Shots.MARKED:     symbol = 'o'; break;
                        case GameLogic.PlayerInfo.Shots.WATER_SHOT: symbol = 'O'; break;
                        case GameLogic.PlayerInfo.Shots.SHIP_SHOT:  symbol = 'X'; break;
                    }
                    System.Console.Write(symbol + " ");
                }
                System.Console.Write('\n');
            }
        }
        private static void PrintGameInfo(GameLogic game)
        {
            System.Console.WriteLine("Game info:");
            System.Console.WriteLine("\tBoard size: " + game.BoardSize + "x" + game.BoardSize);
            System.Console.WriteLine("\tNumber of ships: " + game.ShipCount);
            System.Console.WriteLine("\tShip types in the game: ");
            foreach (var ship in game.ShipsInGame)
                System.Console.WriteLine("\t\t" + ship.shipType + ": count " + ship.count + "; length " + GameLogic.GetShipLength(ship.shipType));
        }
    }
}
