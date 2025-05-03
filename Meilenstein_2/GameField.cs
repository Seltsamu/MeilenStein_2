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

    internal class Player(string name)
    {
        public string Name = name;
        public int Throws = 0;
        public FieldNode? Position;
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
        _player1 = new Player(name1);
        _player2 = new Player(name2);
    }

    private void CreateFieldNodes()
    {
        if (_fieldCount <= 6)
            throw new ArgumentException("Game field must be larger than 6 fields!");
        if (_fieldCount > 200)
            throw new ArgumentException("GameField must be smaller than 201 fields!");

        int number = 1;
        _first = new FieldNode(null, null, number++);
        _last = new FieldNode(_first, null, _fieldCount);
        _first.Next = _last;

        for (int i = 0; i < _fieldCount - 2; i++) // create FieldNodes with a random chance for snake or ladder
        {
            FieldNode newFieldNode;
            Random random = new Random();
            int num = random.Next(5); // TODO create boundary's for how many snakes and letters can be in a row

            if (num == 1) // with snake
            {
                newFieldNode = new FieldNode(_last.Prev, _last, number++, true);
            }
            else if (num == 2) // with ladder
            {
                newFieldNode = new FieldNode(_last.Prev, _last, number++, false, true);
            }
            else // without anything special
            {
                newFieldNode = new FieldNode(_last.Prev, _last, number++);
            }

            _last.Prev!.Next = newFieldNode;
            _last.Prev = newFieldNode;
        }
    }

    public void PrintGameField() // TODO make it colorful and find a way to use this method
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

        for (int i = 0; i < height; i++)
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
}