namespace bits.Bits;

public static class Tui
{
  public static void ShowTui()
  {
    Console.WriteLine("Press any key to start!");
    Console.WriteLine("Press Esc key to exit!");

    var key = Console.ReadKey().Key;

    while(key!=ConsoleKey.Escape)
    {
      if (key == ConsoleKey.A)
        Console.WriteLine("Added Line");
      else if (key == ConsoleKey.D)
      {
        Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
        Console.WriteLine("Added Line but input key is deleted first");
      }
      else if (key == ConsoleKey.B)
      {
        // Clear a single line
        // This is not a builtin method but there are workarounds like this one
        // Found in https://stackoverflow.com/questions/8946808/can-console-clear-be-used-to-only-clear-a-line-instead-of-whole-console
        Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
        Console.Write("New Item Test: ");
        Console.ReadLine();
      }
      else if (key == ConsoleKey.C)
        // Clear entire terminal
        Console.Clear();

      // read new input key
      key = Console.ReadKey().Key;
    }
  }
}
