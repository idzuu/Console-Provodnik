using System;
using System.IO;
using System.Diagnostics;

class FileExplorer
{
    static string currentDirectory = Directory.GetCurrentDirectory();

    static void Main(string[] args)
    {
        Console.WriteLine("Консольный файловый проводник");
        Console.WriteLine("----------------------------");

        while (true)
        {
            DisplayCurrentDirectory();
            DisplayFilesAndFolders();
            DisplayMenu();

            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            ProcessInput(input.ToLower());
        }
    }

    static void DisplayCurrentDirectory()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nТекущая директория: {currentDirectory}");
        Console.ResetColor();

        // Отображение информации о свободном месте
        DriveInfo drive = new DriveInfo(Path.GetPathRoot(currentDirectory));
        Console.WriteLine($"Свободно {drive.AvailableFreeSpace / (1024 * 1024)} MB из {drive.TotalSize / (1024 * 1024)} MB");
    }

    static void DisplayFilesAndFolders()
    {
        Console.WriteLine("\nСодержимое директории:");
        Console.WriteLine("----------------------");

        try
        {
            // Показываем подкаталоги
            foreach (var dir in Directory.GetDirectories(currentDirectory))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($" [Папка]  {Path.GetFileName(dir)}");
                Console.ResetColor();
            }

            // Показываем файлы
            foreach (var file in Directory.GetFiles(currentDirectory))
            {
                FileInfo fi = new FileInfo(file);
                Console.WriteLine($" [Файл]   {Path.GetFileName(file)} ({fi.Length / 1024} KB)");
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Нет доступа к этой директории");
        }
    }

    static void DisplayMenu()
    {
        Console.WriteLine("\nКоманды:");
        Console.WriteLine("1. Перейти в папку");
        Console.WriteLine("2. Подняться на уровень выше");
        Console.WriteLine("3. Открыть файл");
        Console.WriteLine("4. Создать папку");
        Console.WriteLine("5. Создать файл");
        Console.WriteLine("6. Удалить файл/папку");
        Console.WriteLine("7. Выход");
        Console.Write("Введите команду: ");
    }

    static void ProcessInput(string input)
    {
        switch (input)
        {
            case "1":
                NavigateToDirectory();
                break;
            case "2":
                NavigateUp();
                break;
            case "3":
                OpenFile();
                break;
            case "4":
                CreateDirectory();
                break;
            case "5":
                CreateFile();
                break;
            case "6":
                DeleteItem();
                break;
            case "7":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Неизвестная команда");
                break;
        }
    }

    static void NavigateToDirectory()
    {
        Console.Write("Введите имя папки: ");
        string folderName = Console.ReadLine();

        string newPath = Path.Combine(currentDirectory, folderName);

        if (Directory.Exists(newPath))
        {
            currentDirectory = newPath;
        }
        else
        {
            Console.WriteLine("Папка не найдена!");
        }
    }

    static void NavigateUp()
    {
        DirectoryInfo parent = Directory.GetParent(currentDirectory);
        if (parent != null)
        {
            currentDirectory = parent.FullName;
        }
        else
        {
            Console.WriteLine("Вы в корневой директории!");
        }
    }

    static void OpenFile()
    {
        Console.Write("Введите имя файла: ");
        string fileName = Console.ReadLine();

        string filePath = Path.Combine(currentDirectory, fileName);

        if (File.Exists(filePath))
        {
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при открытии файла: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Файл не найден!");
        }
    }

    static void CreateDirectory()
    {
        Console.Write("Введите имя новой папки: ");
        string folderName = Console.ReadLine();

        string newPath = Path.Combine(currentDirectory, folderName);

        try
        {
            Directory.CreateDirectory(newPath);
            Console.WriteLine("Папка успешно создана");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при создании папки: {ex.Message}");
        }
    }

    static void CreateFile()
    {
        Console.Write("Введите имя нового файла: ");
        string fileName = Console.ReadLine();

        string filePath = Path.Combine(currentDirectory, fileName);

        try
        {
            File.Create(filePath).Close();
            Console.WriteLine("Файл успешно создан");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при создании файла: {ex.Message}");
        }
    }

    static void DeleteItem()
    {
        Console.Write("Введите имя файла или папки для удаления: ");
        string itemName = Console.ReadLine();

        string itemPath = Path.Combine(currentDirectory, itemName);

        try
        {
            if (Directory.Exists(itemPath))
            {
                Directory.Delete(itemPath, true);
                Console.WriteLine("Папка успешно удалена");
            }
            else if (File.Exists(itemPath))
            {
                File.Delete(itemPath);
                Console.WriteLine("Файл успешно удален");
            }
            else
            {
                Console.WriteLine("Файл или папка не найдены!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении: {ex.Message}");
        }
    }
}