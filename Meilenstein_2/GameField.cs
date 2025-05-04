namespace Meilenstein_2;

using System.Text;

public class GameField
{
    internal class FieldNode(FieldNode? prev, FieldNode? next, int number, bool snake = false, bool ladder = false)
    {
        public bool Snake = snake;
        public bool Ladder = ladder;
        public FieldNode? Next = next;
        public FieldNode? Prev = prev;
        public int Number = number;
    }

    internal class Player(string name, FieldNode? position)
    {
        public string Name = name;
        public int Throws = 0;
        public FieldNode? Position = position;
        private static int _nextNum = 1;
        public int Num = _nextNum++;
        public bool Winner = false;

        public override string ToString()
        {
            return $"Player{Num}({Name})";
        }
    }

    private FieldNode? _first;
    private FieldNode? _last;
    private int _fieldCount;
    private Player _player1;
    private Player _player2;

    public GameField(int length, string name1, string name2)
    {
        _fieldCount = length;
        CreateFieldNodes();
        _player1 = new Player(name1, _first);
        _player2 = new Player(name2, _first);
    }

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
            AddFieldNodes();
        }
    }

    private void AddFieldNodes()
    {
        FieldNode newFieldNode;
        Random random = new Random();
        int num = random.Next(5); // TODO create boundary's for how many snakes and letters can be in a row

        if (num == 1) // with snake
        {
            newFieldNode = new FieldNode(_last!.Prev, _last, _last.Number - 1, true);
        }
        else if (num == 2) // with ladder
        {
            newFieldNode = new FieldNode(_last!.Prev, _last, _last.Number - 1, false, true);
        }
        else // without anything special
        {
            newFieldNode = new FieldNode(_last!.Prev, _last, _last.Number - 1);
        }

        _last.Prev!.Next = newFieldNode;
        _last.Prev = newFieldNode;
        _last.Number += 1;
    }

    public void PrintGameField() // TODO make it colorful
    {
        Console.OutputEncoding = Encoding.UTF8;

        const int constLength = 20;
        int height = _fieldCount / constLength + 1;

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
            int length;
            if (i == 0)
                length = _fieldCount % constLength;
            else
                length = constLength;

            for (int j = 0; j < length; j++)
            {
                Console.Write($"┌{fieldNodes[i, j]!.Number,3}┐");
            }

            Console.WriteLine();
            for (int j = 0; j < length; j++)
            {
                string p = " ";
                if (fieldNodes[i, j] == _player1.Position)
                    p = "1";
                else if (fieldNodes[i, j] == _player2.Position)
                    p = "2";
                if (fieldNodes[i, j]!.Ladder)
                    Console.Write($"│{p}L │");
                else if (fieldNodes[i, j]!.Snake)
                    Console.Write($"│{p}S │");
                else if (_player1.Position == _first && _player2.Position == _first && fieldNodes[i, j] == _first)
                    Console.Write($"│1 2│");
                else
                    Console.Write($"│ {p} │");
            }

            Console.WriteLine();
            for (int j = 0; j < length; j++)
            {
                Console.Write("└───┘");
            }

            Console.WriteLine();
        }
    }

    public void Start()
    {
        Player currentPlayer = _player2;
        while (!currentPlayer.Winner)
        {
            currentPlayer = currentPlayer == _player1 ? _player2 : _player1; // switch between player1 and player2
            Console.Clear();
            PrintGameField();

            Console.WriteLine($"{currentPlayer} is on the move. Press enter to roll the dice");
            Console.ReadKey();
            
            int turn = RollDice();
            Console.WriteLine($"You rolled a: {turn}. Press enter to make your turn");
            Console.ReadKey();
            
            MakeTurn(currentPlayer, turn);
            
            
        }
    }

    private void MakeTurn(Player player, int turn)
    {
        if (turn == 1)
        {
            IncreaseGameField();
        }
        
        MovePlayer(player, turn);
        
        if (player.Position!.Ladder)
        {
            Console.WriteLine("You land on a Ladder. Move an extra 3 fields forward");
            MovePlayer(player, 3);
        }
        else if (player.Position!.Snake)
        {
            Console.WriteLine("You land on a Snake. Move 3 fields back");
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

    private void IncreaseGameField()
    {
        _fieldCount += 5;
        for (int i = 0; i < 5; i++)
        {
            AddFieldNodes(); // TODO weis nicht obs stimmt
        }
    }

    private void MovePlayer(Player player, int num)
    {
        for (int i = 0; i < num; i++)
        {
            player.Position = player.Position!.Next;

            if (player.Position == _last)
                player.Winner = true;
        }
    }

    private static int RollDice()
    {
        Random random = new Random();
        return random.Next(1, 7);
    }
}