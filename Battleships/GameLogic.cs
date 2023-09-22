
namespace Battleships
{
    class GameLogic
    {
        /*
                --- Explanation ---
            
            This class represents the logic behind the Battleships game. An interface class can use this class for the logic.
            This class should be used as follows:
               
                1. First initialize: 'boardSize', 'shipCount', 'shipsInGame', 'player1.name', 'player2.name' and the positions
                                     of the ships of the two players.
                
                2. Second: call 'Player1MakeAShot()', 'Winner()', 'Player2MakeAShot()', 'Winner()' in this order.
                   So basically the two players take turns and you check if one of them won the game.
                   Note that you can for example let a player shoot again if they hit a ship. So the function call would look
                   something like this:
                       'Player1MakeAShot()'   // A ship was hit.
                       'Winner()'             // Check if all ships were killed.
                       'Player1MakeAShot()'   // Let them shoot again... this time they hit water.
                       'Winner()'             // Check if all ships were killed.
                       'Player2MakeAShot()'   // Player 2's turn, and so on...
                
                3. At any moment you can access the player boards to display them on the screen.
        */
        public GameLogic()
        {
            MakeDefaultGame();
        }

        //---------------------------------------------------------------------------------------------
        
        private void MakeDefaultGame()
        {
            boardSize = 11;   // Sometimes the board can be too small for all the ships to fit.
            shipCount = 5;
            shipsInGame = new (BoardSquares, byte)[NUMBER_OF_SHIP_TYPES]
            {
                (BoardSquares.PATROL_BOAT, 1),
                (BoardSquares.SUBMARINE, 1),
                (BoardSquares.DESTROYER, 1),
                (BoardSquares.BATTLESHIP, 1),
                (BoardSquares.CARRIER, 1)
            };
            MakeDefaultPlayer(ref player1, "Player 1");
            MakeDefaultPlayer(ref player2, "Player 2");
        }
        // Requires 'boardSize', 'shipCount' and 'shipsInGame to be initialized.
        private void MakeDefaultPlayer(ref PlayerInfo p, string newName)
        {
            p.name = newName;
            p.shipsAlive = shipCount;
            InitPlayerBoards(ref p);

            // Go through all ships and place them randomly on the board.
            var rand = new Random();
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

                    } while (!CanBePlaced(p.shipBoard, ship.shipType, coord, orientation));

                    PlaceShip(p.shipBoard, ship.shipType, coord, orientation);
                }
            }
        }
        // Requires 'boardSize' to be initialized.
        private void InitPlayerBoards(ref PlayerInfo p)
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
        private static void PlaceShip(BoardSquares[,] shipBoard, BoardSquares shipType, (byte row, byte col) coord, byte orientation)
        {
            if (orientation == 0)    // Horizontal orientation.
            {
                // Place 'WATER_NO_PLACE' around the ship.
                for (int j = (int)coord.col - 1; j < coord.col + GetShipLength(shipType) + 1; ++j)
                {
                    for (int i = (int)coord.row - 1; i < coord.row + 2; ++i)
                    {
                        if (i < 0 || j < 0 || i >= shipBoard.GetLength(0) || j >= shipBoard.GetLength(1))   // Check if square is in board.
                            continue;

                        shipBoard[i, j] = BoardSquares.WATER_NO_PLACE;
                    }
                }

                // Place ship.
                for (int i = coord.col; i < coord.col + GetShipLength(shipType); ++i)
                    shipBoard[coord.row, i] = shipType;
            }
            else    // Vertical orientation.
            {
                // Place 'WATER_NO_PLACE' around the ship.
                for (int i = (int)coord.row - 1; i < coord.row + GetShipLength(shipType) + 1; ++i)
                {
                    for (int j = (int)coord.col - 1; j < coord.col + 2; ++j)
                    {
                        if (i < 0 || j < 0 || i >= shipBoard.GetLength(0) || j >= shipBoard.GetLength(1))   // Check if square is in board.
                            continue;

                        shipBoard[i, j] = BoardSquares.WATER_NO_PLACE;
                    }
                }

                // Place ship.
                for (int i = coord.row; i < coord.row + GetShipLength(shipType); ++i)
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
        private static bool CanBePlaced(BoardSquares[,] shipBoard, BoardSquares shipType, (byte row, byte col) coord, byte orientation)
        {
            // Check if part of the ship is outside the board. Only the bottom-right square of the ship is needed to determine this.
            if (coord.row + GetShipLength(shipType) >= shipBoard.GetLength(0) ||
                coord.col + GetShipLength(shipType) >= shipBoard.GetLength(1))
                return false;

            if (orientation == 0)    // Horizontal orientation.
            {
                // Check if squares are 'WATER'.
                for (int i = coord.col; i < coord.col + GetShipLength(shipType); ++i)
                    if (shipBoard[coord.row, i] != BoardSquares.WATER)
                        return false;
            }
            else    // Vertical orientation.
            {
                // Check if squares are 'WATER'.
                for (int i = coord.row; i < coord.row + GetShipLength(shipType); ++i)
                    if (shipBoard[i, coord.col] != BoardSquares.WATER)
                        return false;
            }
            
            return true;
        }

        //---------------------------------------------------------------------------------------------

        /*
            Returns true if the ship at the given square is dead.
            Function expects valid coordinates.

            A ship on the given square is considered alive if at least one of the adjacent left, right, up, and down squares is of a ship type.
            Note that the given square is not evaluated!
            Example:
                _ 1 _      Let the given square be S.
                2 S 3        - If squares 1, 2, 3 and 4 are 'WATER' for example, the ship is considered dead and the function returns false.
                _ 4 _        - If square 3 is 'DESTROYER', then the ship is considered alive and functions returns false.
        */
        private static bool IsShipAlive(BoardSquares[,] shipBoard, byte row, byte col)
        {
            int r = (int)row, c = (int)col;    // Cast so no underflow when subtracting.

            return (r - 1 >= 0 && shipBoard[r - 1, c] >= 0) ||                        // Up.
                   (c - 1 >= 0 && shipBoard[r, c - 1] >= 0) ||                        // Left.
                   (r + 1 < shipBoard.GetLength(0) && shipBoard[r + 1, c] >= 0) ||    // Down.
                   (c + 1 < shipBoard.GetLength(1) && shipBoard[r, c + 1] >= 0);      // Right.
        }

        //---------------------------------------------------------------------------------------------

        public byte BoardSize { get { return boardSize; } }
        public byte ShipCount { get { return shipCount; } }
        public (BoardSquares shipType, byte count)[] ShipsInGame { get { return shipsInGame; } }
        public string Player1Name { get { return player1.name; } }
        public string Player2Name { get { return player2.name; } }
        public BoardSquares[,] Player1ShipBoard { get { return player1.shipBoard; } }
        public BoardSquares[,] Player2ShipBoard { get { return player2.shipBoard; } }
        public PlayerInfo.Shots[,] Player1ShotBoard { get { return player1.shotBoard; } }
        public PlayerInfo.Shots[,] Player2ShotBoard { get { return player2.shotBoard; } }
        // Returns the length of the given ship type.
        // Throws and exception if the given paremeter is not a ship type.
        public static byte GetShipLength(BoardSquares shipType)
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
        // Returns true if the given coordinate is in the board.
        public bool IsValidCoord(byte row, byte col) { return row < boardSize && col < boardSize; }

        //---------------------------------------------------------------------------------------------

        /*
            The functions 'Player1MakeAShot()' and 'Player2MakeAShot()' do the same thing but for different players.
            Maybe should make them one function ???
        */
        // Returns the type of the square that was hit.
        public BoardSquares Player1MakeAShot(byte row, byte col)
        {
            if (!IsValidCoord(row, col))
                throw new Exception("Given coordinates are outside the board!");

            if (player2.shipBoard[row, col] >= 0)  // A ship was hit.
            {
                player1.shotBoard[row, col] = PlayerInfo.Shots.SHIP_SHOT;   // Mark that 'player1' hit a ship.

                if (!IsShipAlive(player2.shipBoard, row, col))   // Check if the ship was killed.
                    --player2.shipsAlive;

                BoardSquares square = player2.shipBoard[row, col];  // Return value.

                // Mark that the ship was hit.
                // Note that 'DEAD_<ship_type> == <ship_type> - NUMBER_OF_SHIP_TYPES'.
                player2.shipBoard[row, col] = (BoardSquares)((int)player2.shipBoard[row, col] - (int)NUMBER_OF_SHIP_TYPES);

                return square;    
            }
            else    // A ship was not hit.
            {
                // Mark down the shot if it was not marked already.
                if (player1.shotBoard[row, col] == PlayerInfo.Shots.NO_SHOT)
                    player1.shotBoard[row, col] = PlayerInfo.Shots.WATER_SHOT;
                
                return player2.shipBoard[row, col];
            }
        }
        // Returns the type of the square that was hit.
        public BoardSquares Player2MakeAShot(byte row, byte col)
        {
            if (!IsValidCoord(row, col))
                throw new Exception("Given coordinates are outside the board!");

            if (player1.shipBoard[row, col] >= 0)  // A ship was hit.
            {
                player2.shotBoard[row, col] = PlayerInfo.Shots.SHIP_SHOT;   // Mark that 'player2' hit a ship.

                if (!IsShipAlive(player1.shipBoard, row, col))   // Check if the ship was killed.
                    --player1.shipsAlive;

                BoardSquares square = player1.shipBoard[row, col];  // Return value.

                // Mark that the ship was hit.
                // Note that 'DEAD_<ship_type> == <ship_type> - NUMBER_OF_SHIP_TYPES'.
                player1.shipBoard[row, col] = (BoardSquares)((int)player1.shipBoard[row, col] - (int)NUMBER_OF_SHIP_TYPES);

                return square;
            }
            else    // A ship was not hit.
            {
                // Mark down the shot if it was not marked already.
                if (player2.shotBoard[row, col] == PlayerInfo.Shots.NO_SHOT)
                    player2.shotBoard[row, col] = PlayerInfo.Shots.WATER_SHOT;

                return player1.shipBoard[row, col];
            }
        }
        // Marks the given square on player1's shot board.
        public void Player1Mark(byte row, byte col)
        {
            if (!IsValidCoord(row, col))
                throw new Exception("Given coordinates are outside the board!");

            if (player1.shotBoard[row, col] == PlayerInfo.Shots.NO_SHOT)
                player1.shotBoard[row, col] = PlayerInfo.Shots.MARKED;
        }
        // Marks the given square on player2's shot board.
        public void Player2Mark(byte row, byte col)
        {
            if (!IsValidCoord(row, col))
                throw new Exception("Given coordinates are outside the board!");

            if (player2.shotBoard[row, col] == PlayerInfo.Shots.NO_SHOT)
                player2.shotBoard[row, col] = PlayerInfo.Shots.MARKED;
        }

        // Returns -1 if 'player1' won the game, 1 if 'player2' won the game, 0 if the game is not finished.
        // The game has a winner if one of one of the players' 'shipsAlive' is 0.
        public int Winner()
        {
            if (player1.shipsAlive == 0) { return 1; }    // Player 2 won the game.
            if (player2.shipsAlive == 0) { return -1; }   // Player 1 won the game.
            return 0;                                     // No winner yet. The game is in progress.
        }

        public enum BoardSquares : sbyte
        {
            WATER = -7,       // Ship can be placed only on this kind of squares.
            WATER_NO_PLACE,   // Indicates a water square that a ship cannot be placed on because 
                              // it is too close to another ship.

            // Ship types that are hit.
            // Note that 'DEAD_<ship_type> == <ship_type> - NUMBER_OF_SHIP_TYPES'.
            DEAD_PATROL_BOAT,
            DEAD_SUBMARINE,
            DEAD_DESTROYER,
            DEAD_BATTLESHIP,
            DEAD_CARRIER,

            // Ship types (start from 0).
            PATROL_BOAT = 0,
            SUBMARINE,
            DESTROYER,
            BATTLESHIP,
            CARRIER,
        }
        private const byte NUMBER_OF_SHIP_TYPES = 5;
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
                NOTE: Maybe should off load some of the functions in class GameLogic to this class?

                This is a container for all the information a player has access to during the game.
                    - 'playerName' - name of the player in game.
                    - 'shipBoard' - board with the ships of the player.
                    - 'shotBoard' - board to mark down the shots of the player, and also to mark down the squares
                      of the board that a ship can definitely not be.
            */
            public enum Shots : sbyte
            {   
                NO_SHOT,       // No shot was fired at this square.
                MARKED,        // The player has marked this square (for example if they think a ship cannot be on it).
                WATER_SHOT,    // The player hit water.
                SHIP_SHOT      // The player hit a ship.
            }

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
