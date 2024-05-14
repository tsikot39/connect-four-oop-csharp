namespace ConenctFourGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            GameController gameController = new GameController();
            gameController.StartGame();
        }
    }
    public class GameController
    {
        private Board board;
        private Player currentPlayer;
        private HumanPlayer player1;
        private HumanPlayer player2;
        private GameView view;

        public GameController()
        {
            board = new Board();
            player1 = new HumanPlayer('X');
            player2 = new HumanPlayer('O');
            currentPlayer = player1;
            view = new GameView();
        }

        public void StartGame()
        {
            view.DisplayTeamName(); // Display the team name at the start

            bool gameRunning = true;
            while (gameRunning)
            {
                // Display the current state of the board
                view.DisplayBoard(board);

                // Announce the current player's turn
                view.DisplayTurn(currentPlayer);

                // Get the column choice from the current player and process the move
                int columnChoice = currentPlayer.MakeMove(board);
                bool moveSuccessful = board.DropDisc(columnChoice, currentPlayer.Symbol);

                if (moveSuccessful)
                {
                    // Check for a win or a draw
                    if (board.IsWinningMove(columnChoice, currentPlayer.Symbol))
                    {
                        view.DisplayWinner(currentPlayer);
                        RestartOrExitGame();
                        return; // Exit after the RestartOrExitGame() decision
                    }
                    else if (board.IsFull())
                    {
                        view.DisplayDraw();
                        RestartOrExitGame();
                        return; // Exit after the RestartOrExitGame() decision
                    }
                    else
                    {
                        // Switch the current player if the game is still going
                        currentPlayer = (currentPlayer == player1) ? player2 : player1;
                    }
                }
                else
                {
                    // If the move wasn't successful (column full or invalid), prompt again
                    view.DisplayInvalidMove();
                    // Note: You might want to adjust the logic to re-prompt within the currentPlayer.MakeMove method or handle it elegantly here
                }
            }
        }

        public void PlayTurn()
        {
            bool moveMade = false;
            while (!moveMade)
            {
                int column = currentPlayer.MakeMove(board); // Get column from player

                if (!board.DropDisc(column, currentPlayer.Symbol))
                {
                    view.DisplayInvalidMove(); // Show error message for invalid move
                    // The loop will continue, prompting the player for a new move
                }
                else
                {
                    moveMade = true; // Valid move was made, exit the loop
                    // Additional logic to check for a win or draw could follow here
                }
            }

            // After a successful move, you might check for a win or switch players
        }

        private void SwitchPlayer()
        {
            // Switch currentPlayer between player1 and player2
            currentPlayer = (currentPlayer == player1) ? player2 : player1;
        }

    private void RestartOrExitGame() {
        view.DisplayRestartPrompt(); // This would prompt the user with a message to restart or exit.
    
        string input = Console.ReadLine()!;
        switch (input) {
            case "1":
            case "yes":
            case "Yes":
            case "Y":
            case "y":
                // Clear the board and reset the game state
                board.Reset();
                currentPlayer = player1; // Or randomize starting player
                StartGame(); // Restart the game
                break;
            case "0":
            case "no":
            case "No":
            case "N":
            case "n":
                view.DisplayExitMessage(); // This would display a goodbye message.
                Environment.Exit(0); // Exit the game
                break;
            default:
                view.DisplayInvalidMove();
                RestartOrExitGame(); // Recursively call itself to handle invalid input
                break;
        }
    }

        // You would need to implement the corresponding display methods in GameView.


        private bool CheckGameOver()
        {
            // Check all win conditions for the current player
            if (board.HasWon(currentPlayer.Symbol))
            {
                view.DisplayWinner(currentPlayer);
                return true;
            }

            // Check if the board is full and therefore the game is a draw
            if (board.IsFull())
            {
                view.DisplayDraw();
                return true;
            }

            // If neither condition is met, the game is not over
            return false;
        }

    }

    public class Board
    {
        private char[,] grid;
        public static readonly int Rows = 6;
        public static readonly int Columns = 7;

        public Board()
        {
            grid = new char[Rows, Columns];
            // Initialize the grid with an empty value, assuming '\0' represents an empty cell
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    grid[i, j] = '\0';
                }
            }
        }

        // Attempt to drop a disc into the specified column
        public bool DropDisc(int column, char symbol)
        {
            if (column < 0 || column >= Columns)
            {
                return false; // Column out of bounds
            }

            for (int row = Rows - 1; row >= 0; row--)
            {
                if (grid[row, column] == '\0')
                {
                    grid[row, column] = symbol;
                    return true; // Successfully placed the disc
                }
            }

            return false; // The column is full, disc not placed
        }


        public bool IsWinningMove(int column, char symbol)
        {
            // Find the row index of the topmost disc in the column
            int row = -1;
            for (int i = 0; i < Rows; i++)
            {
                if (grid[i, column] == symbol)
                {
                    row = i;
                    break;
                }
            }

            // If no disc is found in the column for the symbol, return false
            if (row == -1) return false;

            // Check for a horizontal, vertical, or diagonal win starting from the position (row, column)
            return CheckHorizontalWin(row, column, symbol) ||
                   CheckVerticalWin(column, symbol) ||
                   CheckDiagonalWin(row, column, symbol);
        }


        private int FindRowForColumn(int column, char symbol)
        {
            for (int i = 0; i < Rows; i++)
            {
                if (grid[i, column] == symbol)
                {
                    return i;
                }
            }
            return -1; // No disc found in this column for the symbol, or the column is out of bounds
        }

        private bool CheckVerticalWin(int startColumn, char symbol)
        {
            // Starting from the bottom of the column where the last disc was placed
            // and moving up, check if there are 4 in a row.
            int consecutiveCount = 0;
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (grid[row, startColumn] == symbol)
                {
                    consecutiveCount++;
                    if (consecutiveCount == 4)
                    {
                        return true;
                    }
                }
                else
                {
                    consecutiveCount = 0; // Reset the count if the sequence is broken
                }
            }
            return false;
        }

        private bool CheckHorizontalWin(int row, int column, char symbol)
        {
            // Check left and right from the column
            int count = 0;
            for (int i = column; i >= 0 && grid[row, i] == symbol; i--) count++;
            for (int i = column + 1; i < Columns && grid[row, i] == symbol; i++) count++;
            return count >= 4;
        }

        private bool CheckDiagonalWin(int row, int column, char symbol)
        {
            // Check for a diagonal win in both directions
            int count = 0;
            // Check diagonal (bottom-left to top-right)
            for (int i = 0; i < 4; i++)
            {
                if (row - i < 0 || column + i >= Columns || grid[row - i, column + i] != symbol) break;
                count++;
            }
            if (count >= 4) return true;

            count = 0;
            // Check diagonal (top-left to bottom-right)
            for (int i = 0; i < 4; i++)
            {
                if (row + i >= Rows || column + i >= Columns || grid[row + i, column + i] != symbol) break;
                count++;
            }
            return count >= 4;
        }

        public bool HasWon(char symbol)
        {
            // Check horizontal lines for a win
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col <= Columns - 4; col++)
                {
                    if (grid[row, col] == symbol &&
                        grid[row, col + 1] == symbol &&
                        grid[row, col + 2] == symbol &&
                        grid[row, col + 3] == symbol)
                    {
                        return true;
                    }
                }
            }

            // Check vertical lines for a win
            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row <= Rows - 4; row++)
                {
                    if (grid[row, col] == symbol &&
                        grid[row + 1, col] == symbol &&
                        grid[row + 2, col] == symbol &&
                        grid[row + 3, col] == symbol)
                    {
                        return true;
                    }
                }
            }

            // Check diagonal lines (bottom-left to top-right) for a win
            for (int row = 3; row < Rows; row++)
            {
                for (int col = 0; col <= Columns - 4; col++)
                {
                    if (grid[row, col] == symbol &&
                        grid[row - 1, col + 1] == symbol &&
                        grid[row - 2, col + 2] == symbol &&
                        grid[row - 3, col + 3] == symbol)
                    {
                        return true;
                    }
                }
            }

            // Check diagonal lines (top-left to bottom-right) for a win
            for (int row = 0; row <= Rows - 4; row++)
            {
                for (int col = 0; col <= Columns - 4; col++)
                {
                    if (grid[row, col] == symbol &&
                        grid[row + 1, col + 1] == symbol &&
                        grid[row + 2, col + 2] == symbol &&
                        grid[row + 3, col + 3] == symbol)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        public bool IsFull()
        {
            // Check if all columns in the top row (the entry row for discs) are filled
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if (grid[0, col] == '\0')
                { // Assuming the default value of '\0' for empty cells
                    return false; // Found an empty space, so the board is not full
                }
            }
            return true; // No empty spaces found in the top row, so the board is full
        }

        // Assuming this method is inside the Board class
        private bool CheckHorizontalWin(char symbol)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns - 3; col++)
                {
                    if (grid[row, col] == symbol &&
                        grid[row, col + 1] == symbol &&
                        grid[row, col + 2] == symbol &&
                        grid[row, col + 3] == symbol)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckVerticalWin(char symbol)
        {
            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < Rows - 3; row++)
                {
                    if (grid[row, col] == symbol &&
                        grid[row + 1, col] == symbol &&
                        grid[row + 2, col] == symbol &&
                        grid[row + 3, col] == symbol)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckDiagonalWin(char symbol)
        {
            // Check for diagonals that go from the top-left to the bottom-right
            for (int row = 0; row < Rows - 3; row++)
            {
                for (int col = 0; col < Columns - 3; col++)
                {
                    if (grid[row, col] == symbol &&
                        grid[row + 1, col + 1] == symbol &&
                        grid[row + 2, col + 2] == symbol &&
                        grid[row + 3, col + 3] == symbol)
                    {
                        return true;
                    }
                }
            }

            // Check for diagonals that go from the bottom-left to the top-right
            for (int row = 3; row < Rows; row++)
            {
                for (int col = 0; col < Columns - 3; col++)
                {
                    if (grid[row, col] == symbol &&
                        grid[row - 1, col + 1] == symbol &&
                        grid[row - 2, col + 2] == symbol &&
                        grid[row - 3, col + 3] == symbol)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanPlaceDisc(int column)
        {
            return grid[0, column] == '\0';
        }

        public char GetCell(int row, int col)
        {
            return grid[row, col];
        }

        public void Reset()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    grid[i, j] = '\0'; // Assuming '\0' represents an empty cell
                }
            }
        }


    }

    public abstract class Player
    {
        protected char symbol;

        protected Player(char symbol)
        {
            this.symbol = symbol;
        }

        public char Symbol
        {
            get { return symbol; }
        }

        public abstract int MakeMove(Board board);
    }

    public class HumanPlayer : Player
    {
        public HumanPlayer(char symbol) : base(symbol) { }

        public override int MakeMove(Board board)
        {
            bool validInput = false;
            int column = -1;

            while (!validInput)
            {
                Console.WriteLine($"Player {symbol}, enter your column choice (1-{Board.Columns}): ");
                string input = Console.ReadLine()!;

                if (int.TryParse(input, out column) && column >= 1 && column <= Board.Columns)
                {
                    // Adjust for zero-based index used in the board array
                    column--;
                    if (board.CanPlaceDisc(column))
                    {
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("That column is full. Please try a different column.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number corresponding to a column.");
                }
            }

            return column;
        }
    }

    public class GameView
    {
        public void DisplayTeamName()
        {
            Console.WriteLine("Welcome to our Connect Four Game! (Object-Oriented Program (OOP))");
            Console.WriteLine("Developed by: Team Fourward Thinkers (Johnson Benedict Corpus, Cindy April Leochico)\n"); // Replace "Team XYZ" with your actual team name
        }

        public void DisplayBoard(Board board)
        {
            for (int row = 0; row < Board.Rows; row++)
            {
                for (int col = 0; col < Board.Columns; col++)
                {
                    char symbol = board.GetCell(row, col);
                    Console.Write(symbol == '\0' ? "." : symbol.ToString());
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("1 2 3 4 5 6 7");
            Console.WriteLine();
        }

        public void DisplayTurn(Player currentPlayer)
        {
            Console.WriteLine($"It is Player {currentPlayer.Symbol}'s turn.");
        }


        public void DisplayRestartPrompt()
        {
            Console.WriteLine("Game over. Would you like to play again? Press (Y/1 or N/0)");
        }

        public void DisplayExitMessage()
        {
            Console.WriteLine("Thank you for playing! Bye.");
        }

        public void DisplayInvalidMove()
        {
            Console.WriteLine("Invalid move. Please try again.");
        }

        public void DisplayWinner(Player currentPlayer)
        {
            Console.WriteLine($"Congratulations! Player {currentPlayer.Symbol} has won the game!");
        }

        public void DisplayDraw()
        {
            Console.WriteLine("The game is a draw. There are no more moves possible.");
        }
    }
}

