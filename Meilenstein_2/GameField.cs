namespace Meilenstein_2;

using System.Text;

public class GameField
{
    // class to create a game field for "Snakes and Ladders" as well as to players and the complete set of methods to play the game
    // construct the GameField first
    // after that use GameField.start() to start the game
    internal class FieldNode(FieldNode? prev, FieldNode? next, int number, bool snake = false, bool ladder = false)
    {
        // single FieldNode of the game field
        public bool Snake = snake;
        public readonly bool Ladder = ladder;
        public FieldNode? Next = next;
        public FieldNode? Prev = prev;
        public int Number = number;
    }

    internal class Player(string name, FieldNode? position)
    {
        // Players of the game
        private string _name = name;
        public int Throws; // number of dice rolls the player did
        public FieldNode? Position = position; // position of the player in the game field
        public bool Winner;

        public override string ToString()
        {
            // only writes the name of the player
            return $"{_name}";
        }
    }

    private FieldNode? _first; // first node of the double-linked list
    private FieldNode? _last; // last node of the double-linked list
    private int _nodeCount; // number of nodes the game field has
    private readonly Player _player1;
    private readonly Player _player2;
    private int _ladderCount; // var for maximum number of ladders in a row
    private int _snakeCount; // var for maximum number of snakes in a row
    private static readonly Random Random = new Random();

    private const int
        FieldIncrease = 5; // the number of nodes that the game field will get increased when rolling a 1 or a 6

    private const int MaxRowLength = 20; // the maximum length for a row printed on the Console

    private const int
        ProbabilityForSpecialField =
            5; // Probability to get a Snake or a Ladder on a field. The higher the number, the less probable. 5 = 50/50

    public GameField(int length, string name1, string name2)
    {
        // constructor for GameField Object
        _nodeCount = length;
        CreateFieldNodes(); // creates the double-linked list
        _player1 = new Player(name1, _first);
        _player2 = new Player(name2, _first);
    }

    private void CreateFieldNodes()
    {
        // Calls on the AddFieldNode() method for _field count number of times to create all FieldNodes when the GameField gets constructed the first time
        switch (_nodeCount)
        {
            // checks if the game field is too large or too small
            case <= 6: // too small
                throw new ArgumentException("Game field must be larger than 6 fields!");
            case > 200: // too large
                throw new ArgumentException("GameField must be smaller than 201 fields!");
        }

        _first = new FieldNode(null, null, 1); // creates the starting FieldNode
        _last = new FieldNode(_first, null, _nodeCount); // creates the finishing FieldNode
        _first.Next = _last;

        for (int i = 0; i < _nodeCount - 2; i++) // creates _fieldCount FieldNodes 
        {
            AddFieldNode();
        }

        DeleteInfiniteLoops();
    }

    private void AddFieldNode(FieldNode? mark = null)
    {
        // Adds one FieldNode to the double-linked list
        mark ??= _last; // If the mark is null, it adds the new FieldNode before the last. If the mark isn't null, it adds the new FieldNode before the mark
        FieldNode newFieldNode;
        int num = Random.Next(
            ProbabilityForSpecialField); // Determines the ratio between non-special and special fields. Its 50/50

        switch (num)
        {
            // Adds a snake to the node. Maximum for snakes in a row is 3
            case 1 when _snakeCount < 3:
                newFieldNode = new FieldNode(mark!.Prev, mark, mark.Prev!.Number + 1, true);
                _snakeCount++;
                _ladderCount = 0;
                break;
            // Adds a ladder to the node. Maximum for ladders in a row is 3
            case 2 when _ladderCount < 3:
                newFieldNode = new FieldNode(mark!.Prev, mark, mark.Prev!.Number + 1, false, true);
                _ladderCount++;
                _snakeCount = 0;
                break;
            // without anything special
            default:
                newFieldNode = new FieldNode(mark!.Prev, mark, mark.Prev!.Number + 1);
                _snakeCount = _ladderCount = 0;
                break;
        }

        // connects the new FieldNode with the rest
        mark.Prev!.Next = newFieldNode;
        mark.Prev = newFieldNode;
    }

    private void PrintGameField()
    {
        // prints the game field
        Console.OutputEncoding = Encoding.UTF8; // enables usage of ASCII symbols

        int height = (_nodeCount + MaxRowLength - 1) / MaxRowLength; // calculates the height of the game field

        FieldNode?[,]
            fieldNodes = new FieldNode?[height, MaxRowLength]; // creates an array to sort the FieldNodes easier
        FieldNode? currentFieldNode = _first!;
        bool
            startLeft = true; // bool the switch between filling the array from the right or left to get the snakelike game field pattern

        for (int i = fieldNodes.GetLength(0) - 1; i >= 0; i--) // fill fieldNodes array in the correct order
        {
            if (startLeft) // fills from the left
            {
                for (int j = 0; j < fieldNodes.GetLength(1); j++)
                {
                    fieldNodes[i, j] = currentFieldNode;
                    currentFieldNode = currentFieldNode?.Next;
                    startLeft = false;
                }
            }
            else // fills from the right
            {
                for (int j = fieldNodes.GetLength(1) - 1; j >= 0; j--)
                {
                    fieldNodes[i, j] = currentFieldNode;
                    currentFieldNode = currentFieldNode?.Next;
                    startLeft = true;
                }
            }
        }

        // print the Game field with all symbols
        // goes through the FieldNodes in the array
        for (int i = 0; i < height; i++) // loop for the height
        {
            // loop for the upper parts of the FieldNodes
            for (int j = 0; j < MaxRowLength; j++)
            {
                if (fieldNodes[i, j] == null) // prints spaces if there is no FieldNode
                    Console.Write("     ");
                // adjusts the bars to match with the length of the number
                else if (fieldNodes[i, j]!.Number.ToString().Length == 1)
                {
                    Console.Write("┌─");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(fieldNodes[i, j]!.Number);
                    Console.ResetColor();
                    Console.Write("─┐");
                }
                else if (fieldNodes[i, j]!.Number.ToString().Length == 2)
                {
                    Console.Write("┌─");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(fieldNodes[i, j]!.Number);
                    Console.ResetColor();
                    Console.Write("┐");
                }
                else
                {
                    Console.Write("┌");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(fieldNodes[i, j]!.Number);
                    Console.ResetColor();
                    Console.Write("┐");
                }
            }

            Console.WriteLine();

            // loop for the middle parts of the FieldNodes
            for (int j = 0; j < MaxRowLength; j++)
            {
                if (fieldNodes[i, j] == null)
                    Console.Write("     ");
                else // checks for players on the node
                {
                    string p;
                    if (fieldNodes[i, j] == _player1.Position && _player1.Position != _player2.Position)
                    {
                        p = "1";
                        Console.Write("│");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(p);
                        Console.ResetColor();
                    }
                    else if (fieldNodes[i, j] == _player2.Position && _player1.Position != _player2.Position)
                    {
                        p = "2";
                        Console.Write("│");
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write(p);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("│ ");
                    }

                    if (fieldNodes[i, j]!.Ladder) // checks for ladders on the node
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("L");
                        Console.ResetColor();
                        Console.Write(" │");
                    }
                    else if (fieldNodes[i, j]!.Snake) // checks for snakes on the node
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("S");
                        Console.ResetColor();
                        Console.Write(" │");
                    }
                    else if (_player1.Position == _first && _player2.Position == _first &&
                             fieldNodes[i, j] == _first) // checks if both players are on the starting node
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("1");
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write("2");
                        Console.ResetColor();
                        Console.Write("│");
                    }
                    else
                    {
                        Console.Write("  │");
                    }
                }
            }

            Console.WriteLine();

            // loop for the lower parts of the FieldNodes
            for (int j = 0; j < MaxRowLength; j++)
            {
                Console.Write(fieldNodes[i, j] == null ? "     " : "└───┘");
            }

            Console.WriteLine();
        }
    }

    public void Start()
    {
        // starts the game
        // prints instructions and rules first
        Console.Clear();
        Console.WriteLine("Welcome to Snakes and Ladders.");
        Console.WriteLine("You roll the dice to determine how many nodes you can walk forward.");
        Console.WriteLine("If you roll a 1 the game field  will increase by 5 additional nodes at the end.");
        Console.WriteLine(
            "If you roll a 6 (without standing on the starting node) the game field will increase by 5 additional nodes behind you.");
        Console.WriteLine("If you land on a Snake, you have to go 3 nodes backwards.");
        Console.WriteLine("If you land on a ladder, you have to go 3 nodes forwards.");
        Console.WriteLine("If you land on a node with another player on it, you will be placed one node fewer.");
        Console.WriteLine("If you want to quit midgame press Q.");
        Console.WriteLine("Press Enter to start.");
        ConsoleKeyInfo keyInfo = Console.ReadKey();
        if (StopGame(keyInfo)) return;

        Player currentPlayer = _player2;
        while (!currentPlayer.Winner) // loops until a player wins or "Q" is pressed
        {
            currentPlayer = currentPlayer == _player1 ? _player2 : _player1; // switch between player1 and player2
            Console.Clear();
            PrintGameField();

            // change colors for player 1 and 2
            Console.Write($"");
            if (currentPlayer == _player1)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write($"{currentPlayer}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write($"{currentPlayer}");
            }

            Console.ResetColor();
            Console.WriteLine(" is on the move. Press enter to roll the dice.");

            keyInfo = Console.ReadKey();
            if (StopGame(keyInfo)) return;

            // dice roll and checks for 1 and 6
            int turn = RollDice();
            currentPlayer.Throws++;
            if (turn == 1) // rolled a 1
                Console.WriteLine(
                    $"You rolled: {turn}. The Game field will increase by 5 additional nodes. Press enter to make your turn.");
            else if (turn == 6 && currentPlayer.Position != _first) // rolled a 6 and is not on the first node
                Console.WriteLine(
                    $"You rolled: {turn}. The Game field will increase by 5 additional nodes behind you. Press enter to make your turn.");
            else // rolled anything besides 1 and 6
                Console.WriteLine($"You rolled: {turn}. Press enter to make your turn.");
            keyInfo = Console.ReadKey();
            if (StopGame(keyInfo)) return;

            MakeTurn(currentPlayer, turn); // game logic
        }

        // print winner screen
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("**************************");
        Console.ResetColor();
        // switch colors for player 1 and 2
        if (currentPlayer == _player1)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"{currentPlayer}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write($"{currentPlayer}");
        }

        Console.ResetColor();
        Console.WriteLine($" wins with {currentPlayer.Throws} dice-rolls!");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("**************************");
        Console.ResetColor();
        Console.ReadKey();
    }

    private static bool StopGame(ConsoleKeyInfo keyInfo)
    {
        // stops the game if the player entered "Q" 
        if (keyInfo.Key == ConsoleKey.Q)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\nstopping...");
            Console.ResetColor();
            return true;
        }

        return false;
    }

    private void MakeTurn(Player player, int turn)
    {
        // is the actual game logic 
        switch (turn)
        {
            // if turn is 1, the game field gets increased by 5
            case 1:
                IncreaseGameField();
                break;
            // if turn is 6, the game field gets increased behind the player by 5
            case 6 when player.Position != _first:
                IncreaseGameFieldBehindPlayer(player);
                break;
        }

        MovePlayer(player, turn);

        HandleLadderAndSnakeEvents(player); // Check for Snake or Ladder

        // set a player one node back if he lands on a node with another player
        if (_player1.Position == _player2.Position && _player1.Position != _first)
            player.Position = player.Position!.Prev;
    }

    private void HandleLadderAndSnakeEvents(Player player)
    {
        // repeat as long as the player lands on a special node for chaining events,
        while (true)
        {
            if (player.Position!.Ladder) // player lands on a ladder
            {
                Console.Clear();
                PrintGameField();
                Console.WriteLine("You land on a Ladder. Move an extra 3 fields forward.");
                Console.ReadKey();
                MovePlayer(player, 3);
            }
            else if (player.Position!.Snake) // player lands on a snake
            {
                Console.Clear();
                PrintGameField();
                Console.WriteLine("You land on a Snake. Move 3 fields back.");
                Console.ReadKey();
                MovePlayerBackward(player, 3);
            }
            else // player lands on a normal node
                return;
        }
    }

    private void MovePlayerBackward(Player player, int num)
    {
        // Moves the player backwards for num nodes, or until he lands on the first node
        for (int i = 0; i < num; i++)
        {
            player.Position = player.Position!.Prev;
            if (player.Position == _first)
                return;
        }
    }

    private void IncreaseGameFieldBehindPlayer(Player player)
    {
        // increases the game field by the constant number FieldIncrease behind the player
        _nodeCount += FieldIncrease;
        for (int i = 0; i < FieldIncrease; i++) // adds the nodes
        {
            AddFieldNode(player.Position);
        }

        FieldNode currentNode = player.Position!;
        while (currentNode != _last) // corrects the numbers of the following nodes until the last one
        {
            currentNode.Number = currentNode.Prev!.Number + 1;
            currentNode = currentNode.Next!;
        }

        _last.Number = _last.Prev!.Number + 1; // corrects the number of the last node

        DeleteInfiniteLoops();
    }

    private void IncreaseGameField()
    {
        // increases the Game field by the constant number FieldIncrease
        _nodeCount += FieldIncrease;
        for (int i = 0; i < FieldIncrease; i++)
        {
            AddFieldNode();
        }

        _last!.Number = _nodeCount; // corrects the number of the last node

        DeleteInfiniteLoops();
    }

    private void MovePlayer(Player player, int num)
    {
        // Moves the player by num nodes
        for (int i = 0; i < num; i++)
        {
            player.Position = player.Position!.Next;

            if (player.Position == _last) // checks if the player lands on the last node
            {
                // checks if the dice roll was perfect
                if (i == num - 1) // roll was perfect
                {
                    player.Winner = true;
                    return;
                }

                // roll was not perfect and gets set to the node he started, but he can trigger Snake and Ladder events on that node
                Console.WriteLine(
                    "You have to land on the last node with a perfect dice roll! You land on the node where you started.");
                Console.ReadKey();
                MovePlayerBackward(player, i + 1);
                return;
            }
        }
    }

    private static int RollDice()
    {
        // rolls the dice for a random number between 1 and 6
        return Random.Next(1, 7);
    }

    private void DeleteInfiniteLoops()
    {
        // Deletes infinite loops between a ladder and a snake
        FieldNode currentNode = _first ?? throw new ArgumentException("_first is null, something went wrong");

        while (currentNode != _last!.Prev!.Prev!.Prev) // iterates through every FieldNode until the 4th last
        {
            currentNode = currentNode.Next!;

            if (currentNode.Ladder)
            {
                FieldNode nextFieldNode = currentNode;

                for (int i = 0; i < 3; i++) // moves 3 FieldNodes ahead
                    nextFieldNode = nextFieldNode.Next ??
                                    throw new ArgumentException("Calculation with DeleteLoops went wrong");

                nextFieldNode.Snake = false; // deletes the snake on this FieldNode 
            }
        }
    }
}