using System.Text.Json;

List<Todo> todoList = [];
// todoList.Add(new Todo{ title = "Hello", done = false});
//
// todoList.Add(new Todo{ title = "World", done = false});
string fileName = "todo_list.json";
if (File.Exists(fileName))
  todoList = JsonSerializer.Deserialize<List<Todo>>(File.ReadAllText(fileName))!;

string help =
@"     Terminal Todo
  Quit using the Escape key
  Navigate Using The Up and Down arrow keys
  Confirm using the Enter key
  Show this message using ?
";

Console.WriteLine(help);
var idx = todoList.Count-1;
var listview = "";
listview += CreateListView();
Console.WriteLine(listview);

var key = Console.ReadKey().Key;
while(key != ConsoleKey.Escape)
{
  switch (key)
  {
  case ConsoleKey.UpArrow when idx > 0:
      idx--;
      break;
  case ConsoleKey.DownArrow when idx < todoList.Count - 1:
      idx++;
      break;
  case ConsoleKey.Spacebar:
      todoList[idx].done = !todoList[idx].done;
      break;
  case ConsoleKey.A:
      // ClearLastLine();
      Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
      Console.Write("New Todo: ");
      string title = Console.ReadLine()!;
      if (title is not null)
        todoList.Add(new Todo{ title = title, done = false});
        if(todoList.Count==1)
          idx = 0;
      break;
  case ConsoleKey.D:
      todoList.RemoveAt(idx);
      break;
  case ConsoleKey.C:
      Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
      Console.Write("New Title: ");
      string newTitle = Console.ReadLine()!;
      if (newTitle is not null)
        todoList[idx].title = newTitle;
      break;

  }
  listview = CreateListView();
  Console.Clear();
  Console.WriteLine(listview);
  key = Console.ReadKey().Key;
}
string jsonString = JsonSerializer.Serialize(todoList);
File.WriteAllText(fileName, jsonString);


string CreateListView()
{
  string s = "  To-do List:\n";
  for (int i = 0; i<todoList.Count; i++)
  {
    bool check = todoList[i].done?true:false;
    string box = "";
    if (check && idx==i)
      box = "* [x]";
    else if (check && idx!=i)
      box = "[x]";
    if (!check && idx==i)
      box = "* []";
    if (!check && idx!=i)
      box = "[]";
    s+=
@$"  {box}: {todoList[i].title}
";
//     s+=
// @$"  {(i==idx?$"[{(check?"x":"")} ]":$"[{(check?"x":"")}]")}: {todoList[i].title}
// ";
  }

  if (todoList.Count == 0)
    s+="\tPress a to add a new item";
  return s;
}

class Todo
{
  public string title { get; set; }
  public bool done { get; set; }
}

