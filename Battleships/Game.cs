
namespace BattleshipGame
{
    class Game
    {
        public Game()
        {
            MakeDefaultGame();
        }
        //---------------------------------------------------------------------------------------------
        
        private void MakeDefaultGame()
        {
            boardSize = 20;
            shipCount = 10;
            shipsInGame = new (BoardSquares, byte)[5]
            {
                (BoardSquares.PATROL_BOAT, 3),
                (BoardSquares.SUBMARINE, 3),
                (BoardSquares.DESTROYER, 2),
                (BoardSquares.BATTLESHIP, 1),
                (BoardSquares.CARRIER, 1)
            };
            MakeDefaultPlayer(player1, "Player 1");
            MakeDefaultPlayer(player2, "Player 2");
        }
        // Requires 'boardSize', 'shipCount' and 'shipsInGame to be initialized.
        private void MakeDefaultPlayer(PlayerInfo p, string newName)
        {
            p.name = newName;
            p.shipsAlive = shipCount;
            InitPlayerBoards(p);

            // Go through all ships and place them randomly on the board.
            Random rand = new Random();
            foreach(var ship in shipsInGame)
            {
                for(int i = 0; i < ship.count; i++)
                {
                    // Find random place to put the current ship.
                    (byte row, byte col) coord;
                    byte orientation;
                    do
                    {
                        coord = ((byte)rand.Next(0, boardSize), (byte)rand.Next(0, boardSize));  // Pick random square on the board.
                        orientation = (byte)rand.Next(0, 2);    // Pick random orientation.

                    } while (CanBePlaced(p.shipBoard, ship.shipType, coord, orientation));

                    PlaceShip(p.shipBoard, ship.shipType, coord, orientation);
                }
            }
        }
        // Requires 'boardSize' to be initialized.
        private void InitPlayerBoards(PlayerInfo p)
        {
            p.shipBoard = new BoardSquares[boardSize, boardSize];
            p.shotBoard = new PlayerInfo.Shots[boardSize, boardSize];
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    p.shipBoard[i, j] = BoardSquares.WATER;         // No ships are placed.
                    p.shotBoard[i, j] = PlayerInfo.Shots.NO_SHOT;   // No shots are fired.
                }
            }
        }

        //---------------------------------------------------------------------------------------------

        /*
            Example of placing a ship:
                - Let the ship board be 6x6 and empty (every square is 'WATER').
                  The board will look like this ('_' means 'WATER'):
                 
                     0 1 2 3 4 5 
                   0 _ _ _ _ _ _
                   1 _ _ _ _ _ _
                   2 _ _ _ _ _ _
                   3 _ _ _ _ _ _
                   4 _ _ _ _ _ _
                   5 _ _ _ _ _ _

                - Let the given ship be long 3, 'coord == (1, 2)' and 'orientation == 1' (so orientation is vertical).
                  The function will place the ship like this (where 'S' is the ship type and 'X' is 'WATER_NO_PLACE'):
                    
                     0 1 2 3 4 5 
                   0 _ X X X _ _
                   1 _ X S X _ _
                   2 _ X S X _ _
                   3 _ X S X _ _
                   4 _ X X X _ _
                   5 _ _ _ _ _ _

                - Lets place another ship with length 2, 'coord == (5, 4)', 'orientation == 0'.
                  The board will look like this:
                   
                     0 1 2 3 4 5 
                   0 _ X X X _ _
                   1 _ X S X _ _
                   2 _ X S X _ _
                   3 _ X S X _ _
                   4 _ X X X X X
                   5 _ _ _ X S S
        */
        // Places a given ship on the given ship board. Expects a valid placement.
        // Does not check if the placement is valid. For validity check see 'CanBePlaced()'.
        private void PlaceShip(BoardSquares[,] shipBoard, BoardSquares shipType, (byte row, byte col) coord, byte orientation)
        {
            if (orientation == 0)    // Horizontal orientation.
            {
                // Place 'WATER_NO_PLACE' around the ship.
                for (int j = (int)coord.col - 1; j < coord.col + getShipLength(shipType) + 1; ++j)
                {
                    for (int i = (int)coord.row - 1; i < coord.row + 2; ++i)
                    {
                        if (i < 0 || j < 0 || i >= shipBoard.GetLength(0) || j >= shipBoard.GetLength(1))   // Check if square is in board.
                            continue;

                        shipBoard[i, j] = BoardSquares.WATER_NO_PLACE;
                    }
                }

                // Place ship.
                for (int i = coord.col; i < coord.col + getShipLength(shipType); ++i)
                    shipBoard[coord.row, i] = shipType;
            }
            else    // Vertical orientation.
            {
                // Place 'WATER_NO_PLACE' around the ship.
                for (int i = (int)coord.row - 1; i < coord.row + getShipLength(shipType) + 1; ++i)
                {
                    for (int j = (int)coord.col - 1; j < coord.col + 2; ++j)
                    {
                        if (i < 0 || j < 0 || i >= shipBoard.GetLength(0) || j >= shipBoard.GetLength(1))   // Check if square is in board.
                            continue;

                        shipBoard[i, j] = BoardSquares.WATER_NO_PLACE;
                    }
                }

                // Place ship.
                for (int i = coord.row; i < coord.row + getShipLength(shipType); ++i)
                    shipBoard[i, coord.col] = shipType;
            }
        }
        /*
           - The placement of a ship is determined by the coordinates of its top-left corner and its orientation (horizontal or vertical).
             'orientation == 0' means horizontal, 'orientation == 1' means vertical.
           - A ship can be placed only on squares of type 'BoardSquares.WATER' (every square the ship occupies has to be 'BoardSquares.WATER').
           - A ship cannot be placed partially or completely outside the board.
        */
        // Returns true if the given ship type can be placed in the given location on the given board.
        private bool CanBePlaced(BoardSquares[,] shipBoard, BoardSquares shipType, (byte row, byte col) coord, byte orientation)
        {
            // Check if part of the ship is outside the board. Only the bottom-right square of the ship is needed to determine this.
            if (coord.row + getShipLength(shipType) >= shipBoard.GetLength(0) ||
                coord.col + getShipLength(shipType) >= shipBoard.GetLength(1))
                return false;

            if (orientation == 0)    // Horizontal orientation.
            {
                // Check if squares are 'WATER'.
                for (int i = coord.col; i < coord.col + getShipLength(shipType); ++i)
                    if (shipBoard[coord.row, i] != BoardSquares.WATER)
                        return false;
            }
            else    // Vertical orientation.
            {
                // Check if squares are 'WATER'.
                for (int i = coord.row; i < coord.row + getShipLength(shipType); ++i)
                    if (shipBoard[i, coord.col] != BoardSquares.WATER)
                        return false;
            }
            
            return true;
        }
        // Returns the length of the given ship type.
        private byte getShipLength(BoardSquares shipType)
        {
            int i = 0;
            while (i < SHIP_LENGTHS.Length)
            {
                if (SHIP_LENGTHS[i].shipType == shipType)
                    return SHIP_LENGTHS[i].shipLength;
                ++i;
            }
            throw new Exception("Invalid ship type given to function 'getShipLength()'!");
        }

        //---------------------------------------------------------------------------------------------

        public BoardSquares[,] Player1ShipBoard { get { return player1.shipBoard; } }
        public BoardSquares[,] Player2ShipBoard { get { return player2.shipBoard; } }
        public PlayerInfo.Shots[,] Player1ShotBoard { get { return player1.shotBoard; } }

        //---------------------------------------------------------------------------------------------

        public enum BoardSquares : sbyte
        {
            WATER = -2,            // Ship can be placed only on this kind of squares.
            WATER_NO_PLACE = -1,   // Indicates a water square that a ship cannot be placed on because 
                                   // it is too close to another ship.
            // Ship types.
            PATROL_BOAT,
            SUBMARINE,
            DESTROYER,
            BATTLESHIP,
            CARRIER,
        }
        private static readonly (BoardSquares shipType, byte shipLength)[] SHIP_LENGTHS =
        {
            (BoardSquares.PATROL_BOAT, 2),
            (BoardSquares.SUBMARINE, 3),
            (BoardSquares.DESTROYER, 3),
            (BoardSquares.BATTLESHIP, 4),
            (BoardSquares.CARRIER, 5)
        };
        public struct PlayerInfo
        {
            /*
                This is a container for all the information a player has access to during the game.
                    - 'playerName' - name of the player in game.
                    - 'shipBoard' - board with the ships of the player.
                    - 'shotBoard' - board to mark down the shots of the player, and also to mark down the squares
                      of the board that a ship can definitely not be.
            */
            public enum Shots : sbyte { NO_SHOT, MARKED, WATER_SHOT, SHIP_SHOT }

            public string name;
            public byte shipsAlive;
            public BoardSquares[,] shipBoard;
            public Shots[,] shotBoard;
        }

        /*
           'shipCount' - total number of ships in the game.
           'shipsInGame' - the ship types and the number of ships from this type in the current game.
        */
        byte boardSize;
        byte shipCount;
        (BoardSquares shipType, byte count)[] shipsInGame;
        PlayerInfo player1;
        PlayerInfo player2;
    }
}
