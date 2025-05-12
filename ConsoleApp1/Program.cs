using System;
using System.IO;
using System.Linq;

namespace EnhancedFileExplorer
{
    class Program
    {
        private static DriveInfo[] allDrives;
        private static string currentPath;
        private static bool exitRequested;

        static void Main(string[] args)
        {
            Initialize();

            while (!exitRequested)
            {
                DisplayMainMenu();
                ProcessUserInput();
            }
        }

        static void Initialize()
        {
            Console.Title = "Расширенный консольный проводник";
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            allDrives = DriveInfo.GetDrives();
            currentPath = null;
            exitRequested = false;
        }

        static void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("=== РАСШИРЕННЫЙ КОНСОЛЬНЫЙ ПРОВОДНИК ===");
            Console.WriteLine("1. Просмотреть доступные диски");

            if (currentPath != null)
            {
                Console.WriteLine($"Текущий путь: {currentPath}");
                Console.WriteLine("2. Информация о текущем диске");
                Console.WriteLine("3. Просмотр содержимого");
                Console.WriteLine("4. Создать новый каталог");
                Console.WriteLine("5. Создать текстовый файл");
                Console.WriteLine("6. Удалить файл или каталог");
                Console.WriteLine("7. Сменить текущий диск");
            }
            else
            {
                Console.WriteLine("2. Выбрать диск для работы");
            }

            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие: ");
        }

        static void ProcessUserInput()
        {
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    DisplayAvailableDrives();
                    break;
                case "2":
                    if (currentPath == null) SelectDrive();
                    else DisplayDriveInfo();
                    break;
                case "3":
                    if (currentPath != null) BrowseDirectory();
                    break;
                case "4":
                    if (currentPath != null) CreateDirectory();
                    break;
                case "5":
                    if (currentPath != null) CreateTextFile();
                    break;
                case "6":
                    if (currentPath != null) DeleteItem();
                    break;
                case "7":
                    if (currentPath != null) SelectDrive();
                    break;
                case "0":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Неверная команда! Нажмите любую клавишу...");
                    Console.ReadKey();
                    break;
            }
        }

        static void DisplayAvailableDrives()
        {
            Console.Clear();
            Console.WriteLine("ДОСТУПНЫЕ ДИСКИ:");
            Console.WriteLine("----------------");

            foreach (var drive in allDrives)
            {
                if (drive.IsReady)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{drive.Name}".PadRight(10));
                    Console.ResetColor();

                    Console.Write($"{drive.DriveType}".PadRight(15));
                    Console.Write($"{drive.VolumeLabel}".PadRight(20));
                    Console.Write($"ФС: {drive.DriveFormat}".PadRight(15));

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"Свободно: {drive.TotalFreeSpace / (1024 * 1024 * 1024):N1}GB");
                    Console.ResetColor();

                    Console.WriteLine();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{drive.Name} [Диск не готов]");
                    Console.ResetColor();
                }
            }

            WaitForUser();
        }

        static void SelectDrive()
        {
            Console.Clear();
            Console.WriteLine("ВЫБОР ДИСКА:");
            Console.WriteLine("-----------");

            for (int i = 0; i < allDrives.Length; i++)
            {
                var drive = allDrives[i];
                Console.Write($"{i + 1}. {drive.Name}".PadRight(10));

                if (drive.IsReady)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[{drive.DriveType}] {drive.VolumeLabel}");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[Не готов]");
                    Console.ResetColor();
                }
            }

            Console.Write("\nВведите номер диска (0 для отмены): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= allDrives.Length)
            {
                var selectedDrive = allDrives[choice - 1];
                if (selectedDrive.IsReady)
                {
                    currentPath = selectedDrive.RootDirectory.FullName;
                    Console.WriteLine($"Выбран диск: {selectedDrive.Name}");
                    WaitForUser();
                }
                else
                {
                    Console.WriteLine("Диск не готов к работе!");
                    WaitForUser();
                }
            }
        }

        static void DisplayDriveInfo()
        {
            var drive = new DriveInfo(Path.GetPathRoot(currentPath));

            Console.Clear();
            Console.WriteLine($"ИНФОРМАЦИЯ О ДИСКЕ {drive.Name}:");
            Console.WriteLine("-------------------------------");

            Console.WriteLine($"Метка тома: {drive.VolumeLabel}");
            Console.WriteLine($"Тип диска: {drive.DriveType}");
            Console.WriteLine($"Файловая система: {drive.DriveFormat}");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nОбщий размер: {drive.TotalSize / (1024 * 1024 * 1024):N1} GB");
            Console.WriteLine($"Свободно: {drive.TotalFreeSpace / (1024 * 1024 * 1024):N1} GB");
            Console.WriteLine($"Занято: {(drive.TotalSize - drive.TotalFreeSpace) / (1024 * 1024 * 1024):N1} GB");
            Console.ResetColor();

            WaitForUser();
        }

        static void BrowseDirectory()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"СОДЕРЖИМОЕ: {currentPath}");
                Console.WriteLine("----------------------------------");

                try
                {
                
                    var directories = Directory.GetDirectories(currentPath);
                    Console.ForegroundColor = ConsoleColor.Blue;
                    foreach (var dir in directories)
                    {
                        var dirInfo = new DirectoryInfo(dir);
                        Console.WriteLine($"[DIR]  {Path.GetFileName(dir).PadRight(40)} {dirInfo.LastWriteTime}");
                    }
                    Console.ResetColor();

               
                    var files = Directory.GetFiles(currentPath);
                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        Console.WriteLine($"[FILE] {Path.GetFileName(file).PadRight(40)} {fileInfo.Length / 1024} KB\t{fileInfo.LastWriteTime}");
                    }

                    Console.WriteLine("\nКОМАНДЫ:");
                    Console.WriteLine("1. Перейти в папку");
                    Console.WriteLine("2. Подняться на уровень выше");
                    Console.WriteLine("3. Вернуться в главное меню");
                    Console.Write("Выберите действие: ");

                    var input = Console.ReadLine();

                    if (input == "1")
                    {
                        Console.Write("Введите имя папки: ");
                        var folderName = Console.ReadLine();
                        var newPath = Path.Combine(currentPath, folderName);

                        if (Directory.Exists(newPath))
                        {
                            currentPath = newPath;
                        }
                        else
                        {
                            Console.WriteLine("Папка не найдена!");
                            WaitForUser();
                        }
                    }
                    else if (input == "2")
                    {
                        var parent = Directory.GetParent(currentPath);
                        if (parent != null)
                        {
                            currentPath = parent.FullName;
                        }
                        else
                        {
                            Console.WriteLine("Вы в корневой директории диска!");
                            WaitForUser();
                        }
                    }
                    else if (input == "3")
                    {
                        break;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Ошибка доступа к этой директории!");
                    WaitForUser();
                    break;
                }
            }
        }

        static void CreateDirectory()
        {
            Console.Clear();
            Console.WriteLine($"СОЗДАНИЕ НОВОЙ ПАПКИ В: {currentPath}");
            Console.Write("Введите имя новой папки: ");
            var dirName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(dirName))
            {
                Console.WriteLine("Имя папки не может быть пустым!");
                WaitForUser();
                return;
            }

            try
            {
                var newDirPath = Path.Combine(currentPath, dirName);
                Directory.CreateDirectory(newDirPath);
                Console.WriteLine($"\nПапка '{dirName}' успешно создана!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка при создании папки: {ex.Message}");
            }

            WaitForUser();
        }

        static void CreateTextFile()
        {
            Console.Clear();
            Console.WriteLine($"СОЗДАНИЕ ТЕКСТОВОГО ФАЙЛА В: {currentPath}");
            Console.Write("Введите имя файла (с расширением .txt): ");
            var fileName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(fileName))
            {
                Console.WriteLine("Имя файла не может быть пустым!");
                WaitForUser();
                return;
            }

           
            if (!fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".txt";
            }

            Console.WriteLine("\nВведите содержимое файла (для завершения введите пустую строку):");
            Console.WriteLine("------------------------------------------------------------");

            var contentLines = new System.Collections.Generic.List<string>();
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;
                contentLines.Add(line);
            }

            try
            {
                var filePath = Path.Combine(currentPath, fileName);
                File.WriteAllLines(filePath, contentLines);
                Console.WriteLine($"\nФайл '{fileName}' успешно создан!");
                Console.WriteLine($"Размер: {new FileInfo(filePath).Length} байт");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка при создании файла: {ex.Message}");
            }

            WaitForUser();
        }

        static void DeleteItem()
        {
            Console.Clear();
            Console.WriteLine($"УДАЛЕНИЕ ФАЙЛА ИЛИ ПАПКИ В: {currentPath}");
            Console.Write("Введите имя файла или папки для удаления: ");
            var itemName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(itemName))
            {
                Console.WriteLine("Имя не может быть пустым!");
                WaitForUser();
                return;
            }

            var itemPath = Path.Combine(currentPath, itemName);

            if (!Directory.Exists(itemPath) && !File.Exists(itemPath))
            {
                Console.WriteLine("Файл или папка не найдены!");
                WaitForUser();
                return;
            }

            Console.Write($"\nВы уверены, что хотите удалить '{itemName}'? (y/n): ");
            var confirm = Console.ReadLine().ToLower();

            if (confirm == "y")
            {
                try
                {
                    if (Directory.Exists(itemPath))
                    {
                        Directory.Delete(itemPath, true);
                        Console.WriteLine($"\nПапка '{itemName}' успешно удалена!");
                    }
                    else
                    {
                        File.Delete(itemPath);
                        Console.WriteLine($"\nФайл '{itemName}' успешно удален!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nОшибка при удалении: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("\nУдаление отменено.");
            }

            WaitForUser();
        }

        static void WaitForUser()
        {
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}
