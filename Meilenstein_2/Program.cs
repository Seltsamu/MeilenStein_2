namespace Meilenstein_2;

using System.Text;

static class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        GameField gameField = new GameField(90, "test1", "test2");

        gameField.PrintGameField();
    }
}