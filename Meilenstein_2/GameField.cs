namespace Meilenstein_2;

using System.Text;

public class GameField
{
    internal class FieldNode(FieldNode? prev, FieldNode? next, int number, bool snake = false, bool ladder = false)
    {
        public readonly bool Snake = snake;
        public readonly bool Ladder = ladder;
        public FieldNode? Next = next;
        public FieldNode? Prev = prev;
        public int Number = number;
    }

    internal class Player(string name, FieldNode? position)
    {
        private string _name = name;
        public int Throws;
        public FieldNode? Position = position;
        public bool Winner;

        public override string ToString()
        {
            return $"{_name}";
        }
    }

    private FieldNode? _first;
    private FieldNode? _last;
    private int _fieldCount;
    private readonly Player _player1;
    private readonly Player _player2;

    public GameField(int length, string name1, string name2)
    {
        _fieldCount = length;
        CreateFieldNodes();
        _player1 = new Player(name1, _first);
        _player2 = new Player(name2, _first);
    } // constructor

    private void CreateFieldNodes()
    {
        if (_fieldCount <= 6)
            throw new ArgumentException("Game field must be larger than 6 fields!");
        if (_fieldCount > 300)
            throw new ArgumentException("GameField must be smaller than 201 fields!");

        _first = new FieldNode(null, null, 1);
        _last = new FieldNode(_first, null, _fieldCount);
        _first.Next = _last;

        for (int i = 0; i < _fieldCount - 2; i++) // create FieldNodes with a random chance for snake or ladder
        {
            AddFieldNode();
        }
    }

    private void AddFieldNode(FieldNode? mark = null)
    {
        mark = mark ?? _last;
        FieldNode newFieldNode;
        Random random = new Random();
        int num = random.Next(5); // TODO create boundary's for how many snakes and letters can be in a row

        if (num == 1) // with snake
        {
            newFieldNode = new FieldNode(mark!.Prev, mark, mark.Prev!.Number + 1, true);
        }
        else if (num == 2) // with ladder
        {
            newFieldNode = new FieldNode(mark!.Prev, mark, mark.Prev!.Number + 1, false, true);
        }
        else // without anything special
        {
            newFieldNode = new FieldNode(mark!.Prev, mark, mark.Prev!.Number + 1);
        }

        mark.Prev!.Next = newFieldNode;
        mark.Prev = newFieldNode;
    }

    private void PrintGameField() // TODO make it colorful
    {
        Console.OutputEncoding = Encoding.UTF8;

        const int constLength = 20;
        int height = (_fieldCount + constLength - 1) / constLength;

        FieldNode?[,] fieldNodes = new FieldNode?[height, constLength];
        FieldNode? currentFieldNode = _first!;
        bool startLeft = true;

        for (int i = fieldNodes.GetLength(0) - 1; i >= 0; i--) // fill fieldNodes array in the correct order
        {
            if (startLeft)
            {
                for (int j = 0; j < fieldNodes.GetLength(1); j++)
                {
                    fieldNodes[i, j] = currentFieldNode;
                    currentFieldNode = currentFieldNode?.Next;
                    startLeft = false;
                }
            }
            else
            {
                for (int j = fieldNodes.GetLength(1) - 1; j >= 0; j--)
                {
                    fieldNodes[i, j] = currentFieldNode;
                    currentFieldNode = currentFieldNode?.Next;
                    startLeft = true;
                }
            }
        }

        for (int i = 0; i < height; i++) // print the Game field with all symbols
        {
            for (int j = 0; j < constLength; j++)
            {
                if (fieldNodes[i, j] == null)
                    Console.Write("     ");
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
            for (int j = 0; j < constLength; j++)
            {
                if (fieldNodes[i, j] == null)
                    Console.Write("     ");
                else
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

                    if (fieldNodes[i, j]!.Ladder)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("L");
                        Console.ResetColor();
                        Console.Write(" │");
                    }
                    else if (fieldNodes[i, j]!.Snake)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("S");
                        Console.ResetColor();
                        Console.Write(" │");
                    }
                    else if (_player1.Position == _first && _player2.Position == _first && fieldNodes[i, j] == _first)
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
            for (int j = 0; j < constLength; j++)
            {
                Console.Write(fieldNodes[i, j] == null ? "     " : "└───┘");
            }

            Console.WriteLine();
        }
    }

    public void Start()
    {
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
        while (!currentPlayer.Winner)
        {
            currentPlayer = currentPlayer == _player1 ? _player2 : _player1; // switch between player1 and player2
            Console.Clear();
            PrintGameField();

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

            int turn = RollDice();
            currentPlayer.Throws++;
            if (turn == 1)
                Console.WriteLine(
                    $"You rolled: {turn}. The Game field will increase by 5 additional nodes. Press enter to make your turn.");
            else if (turn == 6 && currentPlayer.Position != _first)
                Console.WriteLine(
                    $"You rolled: {turn}. The Game field will increase by 5 additional nodes behind you. Press enter to make your turn.");
            else
                Console.WriteLine($"You rolled: {turn}. Press enter to make your turn.");
            keyInfo = Console.ReadKey();
            if (StopGame(keyInfo)) return;

            MakeTurn(currentPlayer, turn);
        }

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("**************************");
        Console.ResetColor();
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
    }

    private static bool StopGame(ConsoleKeyInfo keyInfo)
    {
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
        if (turn == 1)
        {
            IncreaseGameField();
        }
        else if (turn == 6 && player.Position != _first)
        {
            IncreaseGameFieldBehindPlayer(player);
        }

        MovePlayer(player, turn);

        if (player.Position!.Ladder)
        {
            Console.Clear();
            PrintGameField();
            Console.WriteLine("You land on a Ladder. Move an extra 3 fields forward.");
            Console.ReadKey();
            MovePlayer(player, 3);
        }
        else if (player.Position!.Snake)
        {
            Console.Clear();
            PrintGameField();
            Console.WriteLine("You land on a Snake. Move 3 fields back.");
            Console.ReadKey();
            for (int i = 0; i < 3; i++)
            {
                player.Position = player.Position!.Prev;
                if (player.Position == _first)
                    break;
            }
        }

        if (_player1.Position == _player2.Position && _player1.Position != _first)
            player.Position = player.Position!.Prev;
    }

    private void IncreaseGameFieldBehindPlayer(Player player)
    {
        _fieldCount += 5;
        const int nodeCount = 5;
        for (int i = 0; i < nodeCount; i++)
        {
            AddFieldNode(player.Position);
        }

        FieldNode currentNode = player.Position!;
        while (currentNode != _last)
        {
            currentNode.Number = currentNode.Prev!.Number + 1;
            currentNode = currentNode.Next!;
        }

        _last.Number = _last.Prev!.Number + 1;
    }

    private void IncreaseGameField()
    {
        _fieldCount += 5;
        for (int i = 0; i < 5; i++)
        {
            AddFieldNode();
        }

        _last!.Number = _fieldCount;
    }

    private void MovePlayer(Player player, int num)
    {
        for (int i = 0; i < num; i++)
        {
            player.Position = player.Position!.Next;

            if (player.Position == _last)
            {
                player.Winner = true;
                break;
            }
        }
    }

    private static int RollDice()
    {
        Random random = new Random();
        return random.Next(1, 7);
    }
}