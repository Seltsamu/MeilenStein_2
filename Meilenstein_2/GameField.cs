namespace Meilenstein_2;

using System.Text;

public class GameField
{
    internal class FieldNode(FieldNode? prev, FieldNode? next, bool snake = false, bool ladder = false)
    {
        public bool Snake = snake;
        public bool Ladder = ladder;
        public FieldNode? Next = next;
        public FieldNode? Prev = prev;
    }

    internal class Player(string name)
    {
        public string Name = name;
        public int Throws = 0;
        public FieldNode? Position;
    }

    private FieldNode? _first;
    private FieldNode? _last;

    public GameField(int length, string name1, string name2)
    {
        CreateGameField(length);
        Player player1 = new Player(name1);
        Player player2 = new Player(name2);
    }

    private void CreateGameField(int length)
    {
        if (length <= 6)
            throw new ArgumentException("Game field must be larger than 6 fields!");
        if (length > 200)
            throw new ArgumentException("GameField must be smaller than 201 fields!");

        _first = new FieldNode(null, null);
        _last = new FieldNode(_first, null);
        _first.Next = _last;

        for (int i = 0; i < length - 2; i++)
        {
            FieldNode newFieldNode;
            Random random = new Random();
            int num = random.Next(6);

            if (num == 1) // with snake
            {
                newFieldNode = new FieldNode(_last.Prev, _last, true);
            }
            else if (num == 2) // with ladder
            {
                newFieldNode = new FieldNode(_last.Prev, _last, false, true);
            }
            else // without anything special
            {
                newFieldNode = new FieldNode(_last.Prev, _last);
            }

            if (_last.Prev == _first)
                _first.Next = newFieldNode;
            _last.Prev = newFieldNode;
        }
    }

    public void PrintGameField()
    {
        Console.OutputEncoding = Encoding.UTF8;
        
        Console.Write("┌───┐");
        Console.Write("┐"); // obere rechte Ecke
        Console.WriteLine();

        Console.Write("│");
        Console.Write("  "); // Leerzeichen oder Spielfeldinhalt
        Console.Write("│");
        Console.WriteLine();

        Console.Write("└");
        Console.Write("──");
        Console.Write("┘");
        Console.WriteLine();

    }
}