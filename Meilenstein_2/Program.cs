namespace Meilenstein_2;

using System.Text;

static class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.Write("How large should the game field be (7-200): ");
        int gameFieldLength = Convert.ToInt32(Console.ReadLine());
        Console.Write("\nPlayer 1  enter your name: ");
        string player1Name = Console.ReadLine()!; 
        Console.Write("\nPlayer 2 enter your name: ");
        string player2Name = Console.ReadLine()!;

        GameField gameField = new GameField(gameFieldLength, player1Name, player2Name);

        gameField.Start();
    }
}