using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace _2DEngine
{
    public static class Engine
    {
        //Window Size
        public static int width = 200, height = 50;

        //Collection Declarations
        public static List<IObstacle> activeObstacles = new List<IObstacle>();
        public static List<int[]> obstacleLocations = new List<int[]>();
        public static Dictionary<int[], string> interactLocations = new Dictionary<int[], string>();
        public static List<Terrain> activeTerrains = new List<Terrain>();
        public static List<Terrain> terrains = new List<Terrain>();
        public static List<IObstacle> obstacles = new List<IObstacle>();

        //Player Declaration
        public static char LastDir { get; set; } = 'u';
        public static Player player = new Player();

        /// <summary>
        /// Set Up Console For Project Start Up
        /// </summary>
        public static void StartUp()
        {
            Console.Title = "2D Engine";
            Console.SetBufferSize(width, height);
            Console.WindowWidth = width;
            Console.WindowHeight = height;
            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            player.Loc.X = width / 2;
            player.Loc.Y = height / 2;
        }

        /// <summary>
        /// Clears All Stored Objects
        /// </summary>
        public static void Clear()
        {
            activeTerrains.Clear();
            interactLocations.Clear();
            activeObstacles.Clear();
            obstacleLocations.Clear();
            terrains.Clear();
            obstacles.Clear();
        }

        /// <summary>
        /// Clears Console, Redraws all stored objects
        /// </summary>
        public static void Refresh()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            activeTerrains.Clear();
            interactLocations.Clear();
            activeObstacles.Clear();
            obstacleLocations.Clear();
            foreach (Terrain terrain in terrains)
                Draw(terrain);
            foreach (IObstacle obstacle in obstacles)
                Draw(obstacle);
            Draw(player.Loc.X, player.Loc.Y);
            Console.BackgroundColor = GetColor(0, 0)[0];
            Console.SetCursorPosition(0, 0);
        }

        /// <summary>
        /// Takes Methods for Level Object and Interacts, loops until new level is called
        /// </summary>
        /// <param name="Objects"></param>
        /// <param name="Interacts"></param>
        /// <param name="lvl"></param>
        /// <returns></returns>
        public static int GameLoop(Action Objects, Func<int> Interacts, int lvl)
        {
            int current = lvl;
            Objects();
            Refresh();
            char instruction = ' ';
            while (current == lvl)
            {
                if (!MovePlayer(out instruction))
                {
                    switch (instruction)
                    {
                        case 'e':
                            current = Interacts();
                            break;
                        case 'x':
                            PauseMenu();
                            break;
                        default:
                            break;
                    }
                }
            }
            return current;
        }

        /// <summary>
        /// Draws PauseMenu
        /// </summary>
        public static void PauseMenu()
        {
            Terrain pauseMenuBackground = new Terrain()
            {
                Name = "Pause Menu Background",
                Color = ConsoleColor.Black,
                TextColor = ConsoleColor.White,
            };
            Terrain pauseMenu = new Terrain()
            {
                Name = "Pause Menu",
                Color = ConsoleColor.DarkYellow,
                TextColor = ConsoleColor.Black,
                CornerUL = new Vector2(2, 1),
                CornerLR = new Vector2(width - 2, height - 1)
            };
            Terrain playerStats = new Terrain()
            {
                Name = "Player Stats",
                Color = ConsoleColor.Yellow,
                TextColor = ConsoleColor.Black,
                CornerUL = new Vector2(4, 2),
                CornerLR = new Vector2(width - 4, 11)
            };
            Terrain playerEquips = new Terrain()
            {
                Name = "Player Equips",
                Color = ConsoleColor.Yellow,
                TextColor = ConsoleColor.Black,
                CornerUL = new Vector2(4, 12),
                CornerLR = new Vector2(width / 2 - 2, height - 2)
            };
            Terrain playerItems = new Terrain()
            {
                Name = "Player Items",
                Color = ConsoleColor.Yellow,
                TextColor = ConsoleColor.Black,
                CornerUL = new Vector2(width / 2 + 2, 12),
                CornerLR = new Vector2(width - 4, height - 2)
            };
            Terrain pHealthBarBackGround = new Terrain()
            {
                Name = "PlayerHealthBarBackground",
                Color = ConsoleColor.DarkGreen,
                CornerUL = new Vector2(6, 6),
                CornerLR = new Vector2(width - 6, 10)
            };
            Terrain pHealthBar = new Terrain()
            {
                Name = "PlayerHealthBar",
                Color = ConsoleColor.Green,
                CornerUL = new Vector2(6, 6),
                CornerLR = new Vector2(width - 6, 10)
            };

            Draw(pauseMenuBackground);
            Draw(pauseMenu);
            Draw(playerStats);
            Draw(playerEquips);
            Draw(playerItems);
            Draw(pHealthBarBackGround);

            double currentPHealthRatio = 0;
            currentPHealthRatio = player.Health / (double)player.MaxHealth;
            pHealthBar.CornerLR.X = Convert.ToInt32(pHealthBarBackGround.CornerUL.X + (pHealthBarBackGround.CornerLR.X - pHealthBarBackGround.CornerUL.X) * currentPHealthRatio);
            Draw(pHealthBar);


            List<BattleItem> bIOptions = new List<BattleItem>();
            List<Equippable> eOptions = new List<Equippable>();

            foreach (IItem item in player.Inventory)
            {
                if (item is BattleItem battleItem)
                {
                    bIOptions.Add(battleItem);
                }
                else if (item is Equippable equippable)
                {
                    eOptions.Add(equippable);
                }
            }

            BattleItem selectedBattleItem = bIOptions[0];
            Equippable selectedEquippable = eOptions[0];

            int bIIterator = 0;
            int bIPointer = 0;
            bool bIScroll = false;
            int eIterator = 0;
            int ePointer = 0;
            bool eScroll = false;

            bool menuSwitch = false;

            List<Equippable> eFullOptions = new List<Equippable>();
            foreach (Equippable item in eOptions)
            {
                eFullOptions.Add(item);
            }

            if (eOptions.Count() > 8)
            {
                eScroll = true;
                eOptions.Clear();
                for (int j = 0; j < 8; j++)
                {
                    eOptions.Add(eFullOptions[j]);
                }
            }

            int eTextPlacementX = width / 4 - 7;
            int eTextPlacementY = height / 6 * 2 + 1;

            List<BattleItem> bIFullOptions = new List<BattleItem>();
            foreach(BattleItem item in bIOptions)
            {
                bIFullOptions.Add(item);
            }

            if (bIOptions.Count() > 8)
            {
                bIScroll = true;
                bIOptions.Clear();
                for (int j = 0; j < bIFullOptions.Count() - 1; j++)
                {
                    bIOptions.Add(bIFullOptions[j]);
                }
            }

            int bItextPlacementX = width / 2 + (width / 4 - 9);
            int bItextPlacementY = height / 6 * 2 + 1;
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;

            foreach (Equippable option in eOptions)
            {
                Console.SetCursorPosition(eTextPlacementX, eTextPlacementY + eIterator);
                int letterCounter = 0;
                foreach(char letter in option.Name)
                {
                    Console.Write(letter);
                    if (letterCounter > ((width / 2 - 4) / 2))
                    {
                        Console.Write("...");
                        break;
                    }
                    else
                        letterCounter++;
                }

                Console.CursorTop++;
                eIterator += 4;
            }
            eIterator = 0;
            Console.SetCursorPosition(eTextPlacementX - 3, eTextPlacementY + eIterator);
            Console.Write("►");

            foreach (BattleItem option in bIOptions)
            {
                Console.SetCursorPosition(bItextPlacementX, bItextPlacementY + bIIterator);
                int letterCounter = 0;
                foreach(char letter in option.Name)
                {
                    Console.Write(letter);
                    if (letterCounter > ((width - 6) - (width / 2 + 8)) / 2)
                    {
                        Console.Write("...");
                        break;
                    }
                    else
                        letterCounter++;
                }

                if (option.Amt != 0)
                    Console.Write($" x{option.Amt}");

                Console.CursorTop++;
                bIIterator += 4;             
            }
            bIIterator = 0;

            char userChoice = ' ';
            do
            {
                userChoice = GetDir();
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                switch (userChoice)
                {
                    case 'u':
                        if (menuSwitch)
                        {
                            if (bIIterator / 4 > 0)
                            {
                                Console.SetCursorPosition(bItextPlacementX - 3, bItextPlacementY + bIIterator);
                                Console.Write(" ");
                                bIIterator -= 4;
                                Console.SetCursorPosition(bItextPlacementX - 3, bItextPlacementY + bIIterator);
                                Console.Write("►");
                            }
                            else
                            {
                                if (bIScroll)
                                {
                                    if (bIPointer > 0)
                                    {
                                        foreach (BattleItem option in bIOptions)
                                        {
                                            Console.SetCursorPosition(bItextPlacementX, bItextPlacementY + bIIterator);
                                            string erase = "";
                                            for (int x = 0; x < 53; x++)
                                                erase += " ";
                                            Console.Write(erase);
                                            Console.CursorTop++;
                                            bIIterator += 4;
                                        }
                                        bIIterator = 0;
                                        bIPointer--;
                                        bIOptions.Clear();
                                        for (int j = bIPointer; j < bIPointer + 8; j++)
                                        {
                                            bIOptions.Add(bIFullOptions[j]);
                                        }

                                        foreach (BattleItem option in bIOptions)
                                        {
                                            Console.SetCursorPosition(bItextPlacementX, bItextPlacementY + bIIterator);
                                            int letterCounter = 0;
                                            foreach (char letter in option.Name)
                                            {
                                                Console.Write(letter);
                                                if (letterCounter > ((width - 6) - (width / 2 + 8)) / 2)
                                                {
                                                    Console.Write("...");
                                                    break;
                                                }
                                                else
                                                    letterCounter++;
                                            }

                                            if (option.Amt != 0)
                                                Console.Write($" x{option.Amt}");
                                            Console.CursorTop++;
                                            bIIterator += 4;
                                        }
                                        bIIterator = 0;
                                    }
                                }
                            }
                            selectedBattleItem = bIOptions[bIIterator / 4];
                            Console.SetCursorPosition(0, 0);
                            Console.BackgroundColor = GetColor(0, 0)[0];
                            break;
                        }
                        else
                        {
                            if (eIterator / 4 > 0)
                            {
                                Console.SetCursorPosition(eTextPlacementX - 3, eTextPlacementY + eIterator);
                                Console.Write(" ");
                                eIterator -= 4;
                                Console.SetCursorPosition(eTextPlacementX - 3, eTextPlacementY + eIterator);
                                Console.Write("►");
                            }
                            else
                            {
                                if (eScroll)
                                {
                                    if (ePointer > 0)
                                    {
                                        foreach (Equippable option in eOptions)
                                        {
                                            Console.SetCursorPosition(eTextPlacementX, eTextPlacementY + eIterator);
                                            string erase = "";
                                            for (int x = 0; x < 53; x++)
                                                erase += " ";
                                            Console.Write(erase);
                                            Console.CursorTop++;
                                            eIterator += 4;
                                        }
                                        eIterator = 0;
                                        ePointer--;
                                        eOptions.Clear();
                                        for (int j = ePointer; j < ePointer + 8; j++)
                                        {
                                            eOptions.Add(eFullOptions[j]);
                                        }

                                        foreach (Equippable option in eOptions)
                                        {
                                            Console.SetCursorPosition(eTextPlacementX, eTextPlacementY + eIterator);
                                            int letterCounter = 0;
                                            foreach (char letter in option.Name)
                                            {
                                                Console.Write(letter);
                                                if (letterCounter > ((width - 6) - (width / 2 + 8)) / 2)
                                                {
                                                    Console.Write("...");
                                                    break;
                                                }
                                                else
                                                    letterCounter++;
                                            }
                                            Console.CursorTop++;
                                            eIterator += 4;
                                        }
                                        eIterator = 0;
                                    }
                                }
                            }
                            selectedEquippable = eOptions[eIterator / 4];
                            Console.SetCursorPosition(0, 0);
                            Console.BackgroundColor = GetColor(0, 0)[0];
                            break;
                        }
                    case 'd':
                        if (menuSwitch)
                        {
                            if (bIIterator / 4 < bIOptions.Count() - 1)
                            {
                                Console.SetCursorPosition(bItextPlacementX - 3, bItextPlacementY + bIIterator);
                                Console.Write(" ");
                                bIIterator += 4;
                                Console.SetCursorPosition(bItextPlacementX - 3, bItextPlacementY + bIIterator);
                                Console.Write("►");
                            }
                            else
                            {
                                if (bIScroll)
                                {
                                    if (bIPointer < bIFullOptions.Count() - 8)
                                    {
                                        int lastIt = bIIterator;
                                        bIIterator = 0;
                                        foreach (BattleItem option in bIOptions)
                                        {
                                            Console.SetCursorPosition(bItextPlacementX, bItextPlacementY + bIIterator);
                                            string erase = "";
                                            for (int x = 0; x < 53; x++)
                                                erase += " ";
                                            Console.Write(erase);

                                            bIIterator += 4;
                                            Console.CursorTop++;
                                        }
                                        bIIterator = 0;
                                        bIPointer++;
                                        bIOptions.Clear();
                                        for (int j = bIPointer; j <= (bIPointer + 7); j++)
                                        {
                                            bIOptions.Add(bIFullOptions[j]);
                                        }

                                        foreach (BattleItem option in bIOptions)
                                        {
                                            Console.SetCursorPosition(bItextPlacementX, bItextPlacementY + bIIterator);
                                            int letterCounter = 0;
                                            foreach (char letter in option.Name)
                                            {
                                                Console.Write(letter);
                                                if (letterCounter > ((width - 6) - (width / 2 + 8)) / 2)
                                                {
                                                    Console.Write("...");
                                                    break;
                                                }
                                                else
                                                    letterCounter++;
                                            }

                                            if (option.Amt != 0)
                                                Console.Write($" x{option.Amt}");

                                            Console.CursorTop++;
                                            bIIterator += 4;
                                        }
                                        bIIterator = lastIt;
                                    }
                                    Console.SetCursorPosition(0, 0);
                                }
                            }
                            selectedBattleItem = bIOptions[bIIterator / 4];
                            Console.SetCursorPosition(0, 0);
                            Console.BackgroundColor = GetColor(0, 0)[0];
                            break;
                        }
                        else
                        {
                            if (eIterator / 4 < eOptions.Count() - 1)
                            {
                                Console.SetCursorPosition(eTextPlacementX - 3, eTextPlacementY + eIterator);
                                Console.Write(" ");
                                eIterator += 4;
                                Console.SetCursorPosition(eTextPlacementX - 3, eTextPlacementY + eIterator);
                                Console.Write("►");
                            }
                            else
                            {
                                if (eScroll)
                                {
                                    if (ePointer < eFullOptions.Count() - 8)
                                    {
                                        int lastIt = eIterator;
                                        eIterator = 0;
                                        foreach (Equippable option in eOptions)
                                        {
                                            Console.SetCursorPosition(eTextPlacementX, eTextPlacementY + eIterator);
                                            string erase = "";
                                            for (int x = 0; x < 53; x++)
                                                erase += " ";
                                            Console.Write(erase);

                                            eIterator += 4;
                                            Console.CursorTop++;
                                        }
                                        eIterator = 0;
                                        ePointer++;
                                        eOptions.Clear();
                                        for (int j = ePointer; j <= (ePointer + 7); j++)
                                        {
                                            eOptions.Add(eFullOptions[j]);
                                        }

                                        foreach (Equippable option in eOptions)
                                        {
                                            Console.SetCursorPosition(eTextPlacementX, eTextPlacementY + eIterator);
                                            int letterCounter = 0;
                                            foreach (char letter in option.Name)
                                            {
                                                Console.Write(letter);
                                                if (letterCounter > ((width / 2 - 4) / 2))
                                                {
                                                    Console.Write("...");
                                                    break;
                                                }
                                                else
                                                    letterCounter++;
                                            }

                                            Console.CursorTop++;
                                            eIterator += 4;
                                        }
                                        eIterator = lastIt;
                                    }
                                    Console.SetCursorPosition(0, 0);
                                }
                            }
                        }
                        selectedEquippable = eOptions[eIterator / 4];
                        Console.SetCursorPosition(0, 0);
                        Console.BackgroundColor = GetColor(0, 0)[0];
                        break;
                    case 'l':
                        if (menuSwitch)
                        {
                            Console.SetCursorPosition(bItextPlacementX - 3, bItextPlacementY + bIIterator);
                            Console.Write(" ");
                            Console.SetCursorPosition(eTextPlacementX - 3, eTextPlacementY + eIterator);
                            Console.Write("►");                            
                            menuSwitch = false;
                        }
                        break;
                    case 'r':
                        if (!menuSwitch)
                        {
                            Console.SetCursorPosition(eTextPlacementX - 3, eTextPlacementY + eIterator);
                            Console.Write(" ");
                            Console.SetCursorPosition(bItextPlacementX - 3, bItextPlacementY + bIIterator);
                            Console.Write("►");
                            menuSwitch = true;
                        }
                        break;
                    case 'e':
                        if (menuSwitch)
                        {

                        }
                        else
                        {

                        }
                        Console.SetCursorPosition(0, 0);
                        Console.BackgroundColor = GetColor(0, 0)[0];
                        break;
                    default:
                        break;
                }
            } while (userChoice != 'x');

            terrains.Remove(playerItems);
            terrains.Remove(playerEquips);
            terrains.Remove(playerStats);
            terrains.Remove(pauseMenu);
            terrains.Remove(pauseMenuBackground);
            terrains.Remove(pHealthBarBackGround);
            terrains.Remove(pHealthBar);
            Refresh();
        }

        /// <summary>
        /// Draws A Single TextBox
        /// </summary>
        /// <param name="text"></param>
        /// <param name="speaker"></param>
        public static void DrawTextBox(string text, string speaker)
        {
            int longestStringLength = 0;
            foreach (string line in text.Split('\n'))
            {
                if (line.Length > longestStringLength)
                {
                    longestStringLength = line.Length;
                }
            }

            longestStringLength += 25;
            Terrain textBoxBorder = new Terrain()
            {
                Name = "TextBoxBorder",
                Color = ConsoleColor.Black,
                CornerUL = new Vector2((width - longestStringLength) / 2 - 2, height / 6 * 5 - 2),
                CornerLR = new Vector2((width + longestStringLength) / 2 + 2, height - 1)
            };
            Terrain textBox = new Terrain()
            {
                Name = "TextBox",
                Color = ConsoleColor.Gray,
                CornerUL = new Vector2((width - longestStringLength) / 2, height / 6 * 5 - 1),
                CornerLR = new Vector2((width + longestStringLength) / 2, height - 2)
            };

            Draw(textBoxBorder);
            Draw(textBox);

            Console.SetCursorPosition((width - speaker.Length) / 2, height / 6 * 5);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            string speakerLine = "";
            foreach (char letter in speaker)
                speakerLine += "-";
            Console.Write(speaker);
            Console.SetCursorPosition((width - speaker.Length) / 2, height / 6 * 5 + 1);
            Console.Write(speakerLine);
            int linePosUD = 3;

            foreach (string line in text.Split('\n'))
            {
                Console.SetCursorPosition((width - line.Length) / 2, height / 6 * 5 + linePosUD);
                foreach (char letter in line)
                {
                    Console.Write(letter);
                    Thread.Sleep(25);
                }
                Console.Write("     ▼");;
                while (GetDir() != 'e') { }
                Console.SetCursorPosition((width / 2) + (line.Length / 2) + 5, height / 5 * 4 + linePosUD);
                Console.Write(" ");
                linePosUD++;
            }
            activeTerrains.Remove(textBox);
            RemoveTerrain(textBoxBorder);
        }

        /// <summary>
        /// Draws And Removes Terrain Over Battle Text Box
        /// </summary>
        /// <param name="text"></param>
        /// <param name="speaker"></param>
        /// <param name="cornerUL"></param>
        /// <param name="cornerLR"></param>
        public static void ClearTextArea(string text, string speaker, Vector2 cornerUL, Vector2 cornerLR)
        {
            int longestStringLength = 0;
            foreach (string line in text.Split('\n'))
            {
                if (line.Length > longestStringLength)
                {
                    longestStringLength = line.Length;
                }
            }

            longestStringLength += 25;

            Terrain textBox = new Terrain()
            {
                Name = "TextBox",
                Color = ConsoleColor.Gray,
                CornerUL = cornerUL,
                CornerLR = cornerLR
            };

            Draw(textBox);

            Console.SetCursorPosition((width - speaker.Length) / 2, height / 6 * 5);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            string speakerLine = "";
            foreach (char letter in speaker)
                speakerLine += "-";
            Console.Write(speaker);
            Console.SetCursorPosition((width - speaker.Length) / 2, height / 6 * 5 + 1);
            Console.Write(speakerLine);
            int linePosUD = 3;

            foreach (string line in text.Split('\n'))
            {
                Console.SetCursorPosition((width - line.Length) / 2, height / 6 * 5 + linePosUD);

                foreach (char letter in line)
                {
                    Console.Write(letter);
                    Thread.Sleep(25);
                }
                Console.Write("     ▼");

                while (GetDir() != 'e') { }
                Console.SetCursorPosition((width / 2) + (line.Length / 2) + 5, height / 5 * 4 + linePosUD);
                Console.Write(" ");
                linePosUD++;
            }
            RemoveTerrain(textBox, false);
        }

        /// <summary>
        /// Draws Multiple Textboxes, Automatically resizes them based on longest line of text
        /// </summary>
        /// <param name="fullText"></param>
        /// <param name="speaker"></param>
        public static void DrawTextBox(List<string> fullText, string speaker)
        {
            foreach (string text in fullText)
            {
                int longestStringLength = 0;
                foreach (string line in text.Split('\n'))
                {
                    if (line.Length > longestStringLength)
                    {
                        longestStringLength = line.Length;
                    }
                }
                if (speaker.Length > longestStringLength)
                    longestStringLength = speaker.Length;

                longestStringLength += 25;
                Terrain textBoxBorder = new Terrain()
                {
                    Name = "TextBoxBorder",
                    Color = ConsoleColor.Black,
                    CornerUL = new Vector2((width - longestStringLength) / 2 - 2, height / 6 * 5 - 2),
                    CornerLR = new Vector2((width + longestStringLength) / 2 + 2, height - 1)
                };
                Terrain textBox = new Terrain()
                {
                    Name = "TextBox",
                    Color = ConsoleColor.Gray,
                    CornerUL = new Vector2((width - longestStringLength) / 2, height / 6 * 5 - 1),
                    CornerLR = new Vector2((width + longestStringLength) / 2, height - 2)
                };

                Draw(textBoxBorder);
                Draw(textBox);

                Console.SetCursorPosition((width - speaker.Length) / 2, height / 6 * 5);
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                string speakerLine = "";
                foreach (char letter in speaker)
                    speakerLine += "-";
                Console.Write(speaker);
                Console.SetCursorPosition((width - speaker.Length) / 2, height / 6 * 5 + 1);
                Console.Write(speakerLine);
                int linePosUD = 3;

                foreach (string line in text.Split('\n'))
                {
                    Console.SetCursorPosition((width - line.Length) / 2, height / 6 * 5 + linePosUD);
                    foreach (char letter in line)
                    {
                        Console.Write(letter);
                        Thread.Sleep(25);
                    }
                    Console.Write("     ▼"); ;
                    while (GetDir() != 'e') { }
                    Console.SetCursorPosition((width / 2) + (line.Length / 2) + 5, height / 5 * 4 + linePosUD);
                    Console.Write(" ");
                    linePosUD++;
                }
                activeTerrains.Remove(textBox);
                RemoveTerrain(textBoxBorder); 
            }
        }

        /// <summary>
        /// Multiple TextBox (Old implementation)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="speaker"></param>
        public static void DrawTextBox(string[] text, string speaker)
        {
            int longestStringLength = 0;
            foreach (string dialogue in text)
            {
                foreach (string line in dialogue.Split('\n'))
                {
                    if (line.Length > longestStringLength)
                    {
                        longestStringLength = line.Length;
                    }
                }
            }

            longestStringLength += 25;
            Terrain textBoxBorder = new Terrain()
            {
                Name = "TextBoxBorder",
                Color = ConsoleColor.Black,
                CornerUL = new Vector2((width - longestStringLength) / 2 - 2, height / 6 * 5 - 2),
                CornerLR = new Vector2((width + longestStringLength) / 2 + 2, height - 1)
            };

            Terrain textBox = new Terrain()
            {
                Name = "TextBox",
                Color = ConsoleColor.Gray,
                CornerUL = new Vector2((width - longestStringLength) / 2, height / 6 * 5 - 1),
                CornerLR = new Vector2((width + longestStringLength) / 2, height - 2)
            };

            Draw(textBoxBorder);
            Draw(textBox);

            Console.SetCursorPosition((width - speaker.Length) / 2, height / 6 * 5);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            string speakerLine = "";
            foreach (char letter in speaker)
                speakerLine += "-";
            Console.Write(speaker);
            Console.SetCursorPosition((width - speaker.Length) / 2, height / 6 * 5 + 1);
            Console.Write(speakerLine);
            foreach (string dialogue in text)
            {
                int linePosUD = 3;
                textBox.CornerUL.Y = height / 6 * 5 + 2;
                activeTerrains.Remove(textBox);
                Draw(textBox);
                foreach (string line in dialogue.Split('\n'))
                {
                    Console.SetCursorPosition((width - line.Length) / 2, height / 6 * 5 + linePosUD);
                    foreach (char letter in line)
                    {
                        Console.Write(letter);
                        Thread.Sleep(25);
                    }
                    Console.Write("     ▼");
                    while (GetDir() != 'e') { }
                    Console.SetCursorPosition((width / 2) + (line.Length / 2) + 5, height / 5 * 4 + linePosUD);
                    Console.Write(" ");
                    linePosUD++;
                }
            }
            activeTerrains.Remove(textBox);
            RemoveTerrain(textBoxBorder);
        }

        /// <summary>
        /// Draws Textbox With Options Select
        /// </summary>
        /// <param name="text"></param>
        /// <param name="speaker"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string DrawTextBox(string text, string speaker, List<string> options)
        {
            int longestStringLength = 0;
            foreach (string line in text.Split('\n'))
            {
                if (line.Length > longestStringLength)
                    longestStringLength = line.Length;
            }

            int longestOptionLength = 0;
            List<string> optionNames = new List<string>();
            foreach (string line in options)
            {
                if (line.Length > longestOptionLength)
                    longestOptionLength = line.Length;
            }

            longestStringLength += 25;
            longestOptionLength += 10;

            Terrain textBoxBorder = new Terrain()
            {
                Name = "TextBoxBorder",
                Color = ConsoleColor.Black,
                CornerUL = new Vector2((Console.WindowWidth - longestStringLength) / 2 - longestOptionLength - 2, Console.WindowHeight / 6 * 5 - 2),
                CornerLR = new Vector2((Console.WindowWidth + longestStringLength) / 2 + 2 + longestOptionLength, Console.WindowHeight - 1)
            };
            Terrain textBox = new Terrain()
            {
                Name = "TextBox",
                Color = ConsoleColor.Gray,
                CornerUL = new Vector2((Console.WindowWidth - longestStringLength ) / 2 - longestOptionLength, Console.WindowHeight / 6 * 5 - 1),
                CornerLR = new Vector2((Console.WindowWidth + longestStringLength) / 2 + longestOptionLength, Console.WindowHeight  - 2)
            };
            Draw(textBoxBorder);
            Draw(textBox);

            Console.SetCursorPosition((width - speaker.Length) / 2, height / 6 * 5);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            string speakerLine = "";
            foreach (char letter in speaker)
                speakerLine += "-";
            Console.Write(speaker);
            Console.SetCursorPosition((width - speaker.Length) / 2, height / 6 * 5 + 1);
            Console.Write(speakerLine);
            int linePosUD = 3;
            string[] lines = text.Split('\n');

            for (int i = 0; i < lines.Count(); i++)
            {
                Console.SetCursorPosition((width - lines[i].Length) / 2, height / 6 * 5 + linePosUD);
                foreach (char letter in lines[i])
                {
                    Console.Write(letter);
                    Thread.Sleep(25);
                }
                if (i == lines.Count() - 1)
                {
                    string selected = options[0];
                    int iterator = 0;
                    int optionsOffset = 0;
                    int pointer = 0;
                    bool scroll = false;
                    List<string> fullOptions = new List<string>();
                    if (options.Count() > 4)
                    {
                        scroll = true;
                        fullOptions = new List<string>();
                        options.Clear();
                        for (int j = 0; j < 4; j++)
                        {
                            options.Add(fullOptions[j]);
                        }
                    }
                    else
                    {
                        if (options.Count() == 2)
                            optionsOffset = 2;
                        else if (options.Count() == 3)
                            optionsOffset = 1;
                        else if (options.Count() % 2 == 0)
                            optionsOffset = 0;
                    }

                    foreach (string option in options)
                    {
                        Console.SetCursorPosition((width + longestStringLength) / 2, height / 6 * 5 + optionsOffset + iterator);
                        int letterCounter = 0;
                        foreach (char letter in option)
                        {
                            Console.Write(letter);
                            if (letterCounter > 19)
                            {
                                Console.Write("...");
                                break;
                            }
                            else
                                letterCounter++;
                        }
                        Console.CursorTop++;
                        iterator += 2;
                    }
                    iterator = 0;
                    Console.SetCursorPosition((width + longestStringLength) / 2 - 3, height / 6 * 5 + optionsOffset + iterator);
                    Console.Write("►");
                    while (true)
                    {
                        char userInput = GetDir();
                        switch (userInput)
                        {
                            case 'u':
                                if (iterator / 2 > 0)
                                {
                                    Console.SetCursorPosition((width + longestStringLength) / 2 - 3, height / 6 * 5 + optionsOffset + iterator);
                                    Console.Write(" ");
                                    iterator -= 2;
                                    Console.SetCursorPosition((width + longestStringLength) / 2 - 3, height / 6 * 5 + optionsOffset + iterator);
                                    Console.Write("►");
                                }
                                else
                                {
                                    if (scroll)
                                    {
                                        if (pointer > 0)
                                        {
                                            foreach (string option in options)
                                            {
                                                Console.SetCursorPosition((width + longestStringLength) / 2, height / 6 * 5 + optionsOffset + iterator);
                                                string erase = "";
                                                for (int x = 0; x < 28; x++)
                                                    erase += " ";
                                                Console.Write(erase);
                                                Console.CursorTop++;
                                                iterator += 2;
                                            }
                                            iterator = 0;
                                            pointer--;
                                            options.Clear();
                                            for (int j = pointer; j < pointer + 4; j++)
                                            {
                                                options.Add(fullOptions[j]);
                                            }

                                            foreach (string option in options)
                                            {
                                                Console.SetCursorPosition((width + longestStringLength) / 2, height / 6 * 5 + optionsOffset + iterator);
                                                int letterCounter = 0;
                                                foreach (char letter in option)
                                                {
                                                    Console.Write(letter);
                                                    if (letterCounter > 19)
                                                    {
                                                        Console.Write("...");
                                                        break;
                                                    }
                                                    else
                                                        letterCounter++;
                                                }
                                                Console.CursorTop++;
                                                iterator += 2;
                                            }
                                            iterator = 0;
                                        }
                                    }
                                }
                                selected = options[iterator / 2];
                                break;
                            case 'd':
                                if (iterator / 2 < options.Count() - 1)
                                {
                                    Console.SetCursorPosition((width + longestStringLength) / 2 - 3, height / 6 * 5 + optionsOffset + iterator);
                                    Console.Write(" ");
                                    iterator += 2;
                                    Console.SetCursorPosition((width + longestStringLength) / 2 - 3, height / 6 * 5 + optionsOffset + iterator);
                                    Console.Write("►");
                                }
                                else
                                {
                                    if (scroll)
                                    {
                                        if (pointer < fullOptions.Count() - 4)
                                        {
                                            int lastIt = iterator;
                                            iterator = 0;
                                            foreach (string option in options)
                                            {
                                                Console.SetCursorPosition((width + longestStringLength) / 2, height / 6 * 5 + optionsOffset + iterator);
                                                string erase = "";
                                                for (int x = 0; x < 28; x++)
                                                    erase += " ";
                                                Console.Write(erase);

                                                iterator += 2;
                                                Console.CursorTop++;
                                            }
                                            iterator = 0;
                                            pointer++;
                                            options.Clear();
                                            for (int j = pointer; j <= (pointer + 3); j++)
                                            {
                                                options.Add(fullOptions[j]);
                                            }

                                            foreach (string option in options)
                                            {
                                                Console.SetCursorPosition((width + longestStringLength) / 2, height / 6 * 5 + optionsOffset + iterator);
                                                int letterCounter = 0;
                                                foreach (char letter in option)
                                                {
                                                    Console.Write(letter);
                                                    if (letterCounter > 19)
                                                    {
                                                        Console.Write("...");
                                                        break;
                                                    }
                                                    else
                                                        letterCounter++;
                                                }
                                                Console.CursorTop++;
                                                iterator += 2;
                                            }
                                            iterator = lastIt;
                                        }
                                        Console.SetCursorPosition(0, 0);
                                    }
                                }
                                selected = options[iterator / 2];
                                break;
                            case 'e':
                                activeTerrains.Remove(textBox);
                                RemoveTerrain(textBoxBorder);
                                return selected;
                        }
                    }
                }
                else
                {
                    Console.Write("     ▼");
                    while (GetDir() != 'e') { }
                    Console.SetCursorPosition((width / 2) + (lines[i].Length / 2) + 5, height / 5 * 4 + linePosUD);
                    Console.Write(" ");
                    linePosUD++;
                }
            }
            activeTerrains.Remove(textBox);
            RemoveTerrain(textBoxBorder);
            return null;
        }

        /// <summary>
        /// Draws Item List Selection Menu for Battles
        /// </summary>
        /// <param name="text"></param>
        /// <param name="speaker"></param>
        /// <param name="cornerUL"></param>
        /// <param name="cornerLR"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static BattleItem DrawBattleItemsList(string text, string speaker, Vector2 cornerUL, Vector2 cornerLR, List<BattleItem> options)
        {
            int longestStringLength = 0;
            foreach (string line in text.Split('\n'))
            {
                if (line.Length > longestStringLength)
                    longestStringLength = line.Length;
            }

            int longestOptionLength = 0;
            List<string> optionNames = new List<string>();
            foreach (string line in optionNames)
            {
                if (line.Length > longestOptionLength)
                    longestOptionLength = line.Length;
            }

            longestStringLength += 25;
            longestOptionLength += 10;

            Terrain textBox = new Terrain()
            {
                Name = "TextBox",
                Color = ConsoleColor.Gray,
                CornerUL = cornerUL,
                CornerLR = cornerLR
            };

            Draw(textBox);

            Console.SetCursorPosition((width - speaker.Length) / 2, height / 6 * 5);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            string speakerLine = "";
            foreach (char letter in speaker)
                speakerLine += "-";
            Console.Write(speaker);
            Console.SetCursorPosition((width - speaker.Length) / 2, height / 6 * 5 + 1);
            Console.Write(speakerLine);
            int linePosUD = 3;
            string[] lines = text.Split('\n');

            for (int i = 0; i < lines.Count(); i++)
            {
                Console.SetCursorPosition((width - lines[i].Length) / 2, height / 6 * 5 + linePosUD);
                foreach (char letter in lines[i])
                {
                    Console.Write(letter);
                    Thread.Sleep(25);
                }
                if (i == lines.Count() - 1)
                {
                    BattleItem selected = options[0];
                    int iterator = 0;
                    int optionsOffset = 0;
                    int pointer = 0;
                    bool scroll = false;
                    List<BattleItem> fullOptions = new List<BattleItem>();
                    if (options.Count() > 4)
                    {
                        scroll = true;
                        fullOptions = new List<BattleItem>();
                        foreach (BattleItem bItem in options)
                        {
                            if (bItem.Amt > 0)
                            {
                                fullOptions.Add(bItem);
                            }
                            else if (bItem.Name == "[EXIT]")
                            {
                                fullOptions.Add(bItem);
                            }
                        }
                        options.Clear();
                        for (int j = 0; j < 4; j++)
                        {
                            options.Add(fullOptions[j]);
                        }
                    }
                    else
                    {
                        if (options.Count() == 2)
                            optionsOffset = 2;
                        else if (options.Count() == 3)
                            optionsOffset = 1;
                        else if (options.Count() % 2 == 0)
                            optionsOffset = 0;
                    }

                    foreach (BattleItem option in options)
                    {
                        Console.SetCursorPosition((width + longestStringLength) / 2 - 6, height / 6 * 5 + optionsOffset + iterator);
                        int letterCounter = 0;
                        foreach (char letter in option.Name)
                        {
                            Console.Write(letter);
                            if (letterCounter > 19)
                            {
                                Console.Write("...");
                                break;
                            }
                            else
                                letterCounter++;
                        }
                        if (option.Amt != 0)
                            Console.Write($" x{option.Amt}");
                        Console.CursorTop++;
                        iterator += 2;
                    }
                    iterator = 0;
                    Console.SetCursorPosition((width + longestStringLength) / 2 - 9, height / 6 * 5 + optionsOffset + iterator);
                    Console.Write("►");
                    while (true)
                    {
                        char userInput = GetDir();
                        switch (userInput)
                        {
                            case 'u':
                                if (iterator / 2 > 0)
                                {
                                    Console.SetCursorPosition((width + longestStringLength) / 2 - 9, height / 6 * 5 + optionsOffset + iterator);
                                    Console.Write(" ");
                                    iterator -= 2;
                                    Console.SetCursorPosition((width + longestStringLength) / 2 - 9, height / 6 * 5 + optionsOffset + iterator);
                                    Console.Write("►");
                                }
                                else
                                {
                                    if (scroll)
                                    {
                                        if (pointer > 0)
                                        {
                                            foreach (BattleItem option in options)
                                            {
                                                Console.SetCursorPosition((width + longestStringLength) / 2 - 6, height / 6 * 5 + optionsOffset + iterator);
                                                string erase = "";
                                                for (int x = 0; x < 28; x++)
                                                    erase += " ";
                                                Console.Write(erase);
                                                Console.CursorTop++;
                                                iterator += 2;
                                            }
                                            iterator = 0;
                                            pointer--;
                                            options.Clear();
                                            for (int j = pointer; j < pointer + 4; j++)
                                            {
                                                options.Add(fullOptions[j]);
                                            }

                                            foreach (BattleItem option in options)
                                            {
                                                Console.SetCursorPosition((width + longestStringLength) / 2 - 6, height / 6 * 5 + optionsOffset + iterator);
                                                int letterCounter = 0;
                                                foreach (char letter in option.Name)
                                                {
                                                    Console.Write(letter);
                                                    if (letterCounter > 19)
                                                    {
                                                        Console.Write("...");
                                                        break;
                                                    }
                                                    else
                                                        letterCounter++;
                                                }
                                                if (option.Amt != 0)
                                                    Console.Write($" x{option.Amt}");
                                                Console.CursorTop++;
                                                iterator += 2;
                                            }
                                            iterator = 0;
                                        }
                                    }
                                }
                                selected = options[iterator / 2];
                                break;
                            case 'd':
                                if (iterator / 2 < options.Count() - 1)
                                {
                                    Console.SetCursorPosition((width + longestStringLength) / 2 - 9, height / 6 * 5 + optionsOffset + iterator);
                                    Console.Write(" ");
                                    iterator += 2;
                                    Console.SetCursorPosition((width + longestStringLength) / 2 - 9, height / 6 * 5 + optionsOffset + iterator);
                                    Console.Write("►");
                                }
                                else
                                {
                                    if (scroll)
                                    {
                                        if (pointer < fullOptions.Count() - 4)
                                        {
                                            int lastIt = iterator;
                                            iterator = 0;
                                            foreach (BattleItem option in options)
                                            {
                                                Console.SetCursorPosition((width + longestStringLength) / 2 - 6, height / 6 * 5 + optionsOffset + iterator);
                                                string erase = "";
                                                for (int x = 0; x < 28; x++)
                                                    erase += " ";
                                                Console.Write(erase);

                                                iterator += 2;
                                                Console.CursorTop++;
                                            }
                                            iterator = 0;
                                            pointer++;
                                            options.Clear();
                                            for (int j = pointer; j <= (pointer + 3); j++)
                                            {
                                                options.Add(fullOptions[j]);
                                            }

                                            foreach (BattleItem option in options)
                                            {
                                                Console.SetCursorPosition((width + longestStringLength) / 2 - 6, height / 6 * 5 + optionsOffset + iterator);
                                                int letterCounter = 0;
                                                foreach (char letter in option.Name)
                                                {
                                                    Console.Write(letter);
                                                    if (letterCounter > 19)
                                                    {
                                                        Console.Write("...");
                                                        break;
                                                    }
                                                    else
                                                        letterCounter++;
                                                }
                                                if (option.Amt != 0)
                                                    Console.Write($" x{option.Amt}");
                                                Console.CursorTop++;
                                                iterator += 2;
                                            }
                                            iterator = lastIt;
                                        }
                                        Console.SetCursorPosition(0, 0);
                                    }
                                }
                                selected = options[iterator / 2];
                                break;
                            case 'e':
                                activeTerrains.Remove(textBox);
                                return selected;
                        }
                    }
                }
                else
                {
                    Console.Write("    ▼");
                    while (GetDir() != 'e') { }
                    Console.SetCursorPosition((width / 2) + (lines[i].Length / 2) + 5, height / 5 * 4 + linePosUD);
                    Console.Write(" ");
                    linePosUD++;
                }
            }
            activeTerrains.Remove(textBox);
            return new BattleItem("NULL", "Misc", 0, 0);
        }

        /// <summary>
        /// Begins Battle Sequence With Sent NPC
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        public static int Battle(NPC enemy)
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string attacker = enemy.Name;

            int enemyAtk = enemy.Strength;
            int enemyHealth = enemy.Health;

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;

            int enemyMaxHealth = enemyHealth;
            string text = "What would you like to do?";

            int longestStringLength = text.Length;
            string[] options = new string[]
            {
                "Attack",
                "Block",
                "Use Item",
                "Flee"
            };

            int longestOptionLength = 0;
            foreach (string line in options)
            {
                if (line.Length > longestOptionLength)
                    longestOptionLength = line.Length;
            }

            longestStringLength += 25;
            longestOptionLength += 10;

            Vector2 textBoxCornersUL = new Vector2((width - longestStringLength) / 2 - longestOptionLength, height / 6 * 5 - 1);
            Vector2 textBoxCornersLR = new Vector2((width + longestStringLength) / 2 + longestOptionLength, height - 2);

            Terrain textBoxBorder = new Terrain()
            {
                Name = "TextBoxBorder",
                Color = ConsoleColor.Black,
                CornerUL = new Vector2((width - longestStringLength) / 2 - longestOptionLength - 2, height / 6 * 5 - 3),
                CornerLR = new Vector2((width + longestStringLength) / 2 + longestOptionLength + 2, height - 1)
            };

            Terrain textBox = new Terrain()
            {
                Name = "TextBox",
                Color = ConsoleColor.Gray,
                CornerUL = textBoxCornersUL,
                CornerLR = textBoxCornersLR
            };

            textBoxCornersUL = new Vector2((width - longestStringLength) / 2 - longestOptionLength, height / 6 * 5);
            textBoxCornersLR = new Vector2((width + longestStringLength) / 2 + longestOptionLength, height - 3);

            Terrain pHealthBarBackGround = new Terrain()
            {
                Name = "PlayerHealthBarBackground",
                Color = ConsoleColor.DarkGreen,
                CornerUL = new Vector2((width - longestStringLength) / 2 - longestOptionLength, height / 6 * 5 - 2),
                CornerLR = new Vector2((width + longestStringLength) / 2 + longestOptionLength, height / 6 * 5 - 1)
            };

            Terrain pHealthBar = new Terrain()
            {
                Name = "PlayerHealthBar",
                Color = ConsoleColor.Green,
                CornerUL = new Vector2((width - longestStringLength) / 2 - longestOptionLength, height / 6 * 5 - 2),
                CornerLR = new Vector2((width + longestStringLength) / 2 + longestOptionLength, height / 6 * 5 - 1)
            };

            Terrain eHealthBarBackground = new Terrain()
            {
                Name = "EnemyHealthBarBackgroundd",
                Color = ConsoleColor.DarkGreen,
                CornerUL = new Vector2((width - longestStringLength) / 2 - 15, 1),
                CornerLR = new Vector2((width + longestStringLength) / 2 + 15, 2)
            };

            Terrain eHealthBarBorder = new Terrain()
            {
                Name = "EnemyHealthBarBorder",
                Color = ConsoleColor.Black,
                CornerUL = new Vector2((width - longestStringLength) / 2 - 17, 0),
                CornerLR = new Vector2((width + longestStringLength) / 2 + 17, 3)
            };

            Terrain eStatsBorder = new Terrain()
            {
                Name = "EnemyStatsBorder",
                Color = ConsoleColor.Black,
                CornerUL = new Vector2(eHealthBarBorder.CornerLR.X - attacker.Length - 24, 3),
                CornerLR = new Vector2(eHealthBarBorder.CornerLR.X, 8)
            };

            Terrain eStats = new Terrain()
            {
                Name = "EnemyStats",
                Color = ConsoleColor.Gray,
                CornerUL = new Vector2(eHealthBarBorder.CornerLR.X - attacker.Length - 22, 3),
                CornerLR = new Vector2(eHealthBarBorder.CornerLR.X - 2, 7)
            };

            Terrain eHealthBar = new Terrain()
            {
                Name = "EnemyHealthBar",
                Color = ConsoleColor.Green,
                CornerUL = new Vector2((width - longestStringLength) / 2 - 15, 1),
                CornerLR = new Vector2((width + longestStringLength) / 2 + 15, 2)
            };

            Terrain pNameBackground = new Terrain()
            {
                Name = "PlayerNameBackground",
                Color = ConsoleColor.Black,
                CornerUL = new Vector2((width - longestStringLength) / 2 - longestOptionLength - 2, height / 6 * 5 - 8),
                CornerLR = new Vector2((width - longestStringLength) / 2 - longestOptionLength + player.Name.Length + 27, height / 6 * 5 - 2)
            };

            Terrain pName = new Terrain()
            {
                Name = "Player Name",
                Color = ConsoleColor.Gray,
                CornerUL = new Vector2((width - longestStringLength) / 2 - longestOptionLength, height / 6 * 5 - 7),
                CornerLR = new Vector2((width - longestStringLength) / 2 - longestOptionLength + player.Name.Length + 25, Console.WindowHeight / 6 * 5 - 2)
            };

            Draw(pNameBackground);
            Draw(pName);
            Draw(eStatsBorder);
            Draw(eStats);

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition((width - longestStringLength) / 2 - longestOptionLength + 3, height / 6 * 5 - 6);
            Console.Write(player.Name);
            Console.SetCursorPosition(eHealthBarBorder.CornerLR.X - attacker.Length - 19, 4);
            Console.Write(attacker);

            Draw(textBoxBorder);
            Draw(textBox);

            Draw(pHealthBarBackGround);
            double currentPHealthRatio = 0;
            currentPHealthRatio = player.Health / (double)player.MaxHealth;
            pHealthBar.CornerLR.X = Convert.ToInt32(pHealthBarBackGround.CornerUL.X + (pHealthBarBackGround.CornerLR.X - pHealthBarBackGround.CornerUL.X) * currentPHealthRatio);
            Draw(pHealthBar);

            Draw(eHealthBarBorder);
            Draw(eHealthBarBackground);
            double currentEHealthBarRatio = 0;
            currentEHealthBarRatio = enemyHealth / (double)enemyMaxHealth;
            eHealthBar.CornerLR.X = Convert.ToInt32(eHealthBarBackground.CornerUL.X + (eHealthBarBackground.CornerLR.X - eHealthBarBackground.CornerUL.X) * currentEHealthBarRatio);
            Draw(eHealthBar);

            while (player.Health > 0 && enemyHealth > 0)
            {
                int roundPlayerAtk = player.Attack + rnd.Next(player.Randomness * -1, player.Randomness + 1);
                int roundEnemyAtk = enemyAtk + rnd.Next(enemy.Randomness * -1, enemy.Randomness + 1);
                string selected = options[0];
                int iterator = 0;
                int optionsOffset = 1;
                string speakerLine = "------";
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition((width - longestStringLength) / 2 - longestOptionLength + 3, height / 6 * 5 - 5);
                Console.Write($"Health: {player.Health} / {player.MaxHealth}   ");
                Console.SetCursorPosition((width - 6) / 2, height / 6 * 5);
                Console.Write("BATTLE");
                Console.SetCursorPosition((width - 6) / 2, height / 6 * 5 + 1);
                Console.Write(speakerLine);
                Console.SetCursorPosition(eHealthBarBorder.CornerLR.X - attacker.Length - 19, 5);
                Console.Write($"Health: {enemyHealth} / {enemyMaxHealth}   ");
                int linePosUD = 3;
                Console.SetCursorPosition((width - text.Length) / 2, height / 6 * 5 + linePosUD);
                foreach (char letter in text)
                {
                    Console.Write(letter);
                    Thread.Sleep(25);
                }

                if (options.Count() == 2)
                    optionsOffset = 2;
                else if (options.Count() % 2 == 0)
                    optionsOffset = 0;

                foreach (string option in options)
                {
                    Console.SetCursorPosition((width + longestStringLength) / 2 + 1, height / 6 * 5 + optionsOffset + iterator);
                    Console.Write(option);
                    Console.CursorTop++;
                    iterator += 2;
                }

                iterator = 0;
                Console.SetCursorPosition((width + longestStringLength) / 2 - 1, height / 6 * 5 + optionsOffset + iterator);
                Console.Write("►");
                while (true)
                {
                    char userInput = GetDir();
                    bool validInput = false;
                    bool exited = false;
                    switch (userInput)
                    {
                        case 'u':
                            if (iterator / 2 > 0)
                            {
                                Console.SetCursorPosition((width + longestStringLength) / 2 - 1, height / 6 * 5 + optionsOffset + iterator);
                                Console.Write(" ");
                                iterator -= 2;
                                Console.SetCursorPosition((width + longestStringLength) / 2 - 1, height / 6 * 5 + optionsOffset + iterator);
                                Console.Write("►");
                            }
                            selected = options[iterator / 2];
                            break;
                        case 'd':
                            if (iterator / 2 < options.Count() - 1)
                            {
                                Console.SetCursorPosition((width + longestStringLength) / 2 - 1, height / 6 * 5 + optionsOffset + iterator);
                                Console.Write(" ");
                                iterator += 2;
                                Console.SetCursorPosition((width + longestStringLength) / 2 - 1, height / 6 * 5 + optionsOffset + iterator);
                                Console.Write("►");
                            }
                            selected = options[iterator / 2];
                            break;
                        case 'e':
                            validInput = true;
                            switch (selected)
                            {
                                case "Attack":
                                    enemyHealth -= roundPlayerAtk;
                                    RemoveTerrain(eHealthBar, false);
                                    if (enemyHealth > 0)
                                    {
                                        currentEHealthBarRatio = enemyHealth / (double)enemyMaxHealth;
                                        eHealthBar.CornerLR.X = Convert.ToInt32(eHealthBarBackground.CornerUL.X + (eHealthBarBackground.CornerLR.X - eHealthBarBackground.CornerUL.X) * currentEHealthBarRatio);
                                        Draw(eHealthBar);
                                    }
                                    else
                                    {
                                        enemyHealth = 0;
                                    }

                                    Console.SetCursorPosition(eHealthBarBorder.CornerLR.X- attacker.Length - 19, 5);
                                    Console.BackgroundColor = ConsoleColor.Gray;
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    Console.Write($"Health: {enemyHealth} / {enemyMaxHealth}   ");
                                    ClearTextArea($"You Attacked! {roundPlayerAtk} DMG", "BATTLE", textBoxCornersUL, textBoxCornersLR);
                                    break;
                                case "Block":
                                    ClearTextArea("You Blocked!", "BATTLE", textBoxCornersUL, textBoxCornersLR);
                                    if (enemyAtk - player.Attack > 0)
                                        roundEnemyAtk = enemyAtk - player.Attack;
                                    else
                                        roundEnemyAtk = 0;
                                    break;
                                case "Use Item":
                                    List<BattleItem> bItems = new List<BattleItem>();
                                    foreach (IItem pItem in player.Inventory)
                                    {
                                        if (pItem is BattleItem bInvItem)
                                        {
                                            bItems.Add(bInvItem);
                                        }
                                    }
                                    if (bItems.Count() == 0)
                                    {
                                        validInput = false;
                                        continue;
                                    }
                                    else
                                    {
                                        bItems.Add(new BattleItem("[EXIT]", "EXIT", 0, 0));

                                    }
                                    BattleItem item = DrawBattleItemsList("Which Item To Use?", "BATTLE", textBoxCornersUL, textBoxCornersLR, bItems);
                                    switch (item.Type)
                                    {
                                        case "Health":
                                            if (item.Strength >= 0)
                                                ClearTextArea($"You used a {item.Name}!\n+{item.Strength} Health", "BATTLE", textBoxCornersUL, textBoxCornersLR);
                                            else
                                                ClearTextArea($"You used a {item.Name}!\nYou Lose {item.Strength * -1} Health", "BATTLE", textBoxCornersUL, textBoxCornersLR);

                                            if (player.Health + item.Strength > player.MaxHealth)
                                                player.Health = player.MaxHealth;
                                            else if (player.Health + item.Strength < 0)
                                                player.Health = 0;
                                            else
                                                player.Health += item.Strength;

                                            Console.SetCursorPosition((width - 6) / 2, height / 6 * 5 + 1);
                                            Console.Write(speakerLine);
                                            Console.BackgroundColor = ConsoleColor.Gray;
                                            Console.SetCursorPosition((width - longestStringLength) / 2 - longestOptionLength + 3, height / 6 * 5 - 5);
                                            Console.Write($"Health: {player.Health} / {player.MaxHealth}   ");
                                            RemoveTerrain(pHealthBar, false);
                                            currentPHealthRatio = player.Health / (double)player.MaxHealth;
                                            pHealthBar.CornerLR.X = Convert.ToInt32(pHealthBarBackGround.CornerUL.X + (pHealthBarBackGround.CornerLR.X - pHealthBarBackGround.CornerUL.X) * currentPHealthRatio);
                                            Draw(pHealthBar);
                                            break;
                                        case "Damage":
                                            if (item.Strength >= 0)
                                                ClearTextArea($"You used a {item.Name}!\n{attacker} Takes {item.Strength} Damage!", "BATTLE", textBoxCornersUL, textBoxCornersLR);
                                            else
                                                ClearTextArea($"You used a {item.Name}!\n{attacker} is healed for {item.Strength * -1} Health!", "BATTLE", textBoxCornersUL, textBoxCornersLR);

                                            if (enemyHealth - item.Strength <= 0)
                                                enemyHealth = 0;
                                            else if (enemyHealth - item.Strength >= enemyMaxHealth)
                                                enemyHealth = enemyMaxHealth;
                                            else
                                                enemyHealth -= item.Strength;

                                            RemoveTerrain(eHealthBar, false);
                                            if (enemyHealth > 0)
                                            {
                                                currentEHealthBarRatio = enemyHealth / (double)enemyMaxHealth;
                                                eHealthBar.CornerLR.X = Convert.ToInt32(eHealthBarBorder.CornerUL.X + (eHealthBarBackground.CornerLR.X- eHealthBarBackground.CornerUL.X) * currentEHealthBarRatio);
                                                Draw(eHealthBar);
                                                Console.ForegroundColor = ConsoleColor.Black;
                                                Console.BackgroundColor = ConsoleColor.Gray;
                                                Console.SetCursorPosition(eHealthBarBorder.CornerLR.X - attacker.Length - 19, 5);
                                                Console.Write($"Health: {enemyHealth} / {enemyMaxHealth}   ");
                                            }
                                            break;
                                        case "EXIT":
                                            exited = true;
                                            selected = options[0];
                                            iterator = 0;
                                            optionsOffset = 1;
                                            validInput = false;
                                            Terrain tempTextBox = new Terrain()
                                            {
                                                Name = "TextBox",
                                                Color = ConsoleColor.Gray,
                                                CornerUL = textBoxCornersUL,
                                                CornerLR = textBoxCornersLR
                                            };
                                            activeTerrains.Remove(tempTextBox);
                                            RemoveTerrain(tempTextBox, false);

                                            Console.BackgroundColor = ConsoleColor.Gray;
                                            Console.ForegroundColor = ConsoleColor.Black;
                                            Console.SetCursorPosition((width - longestStringLength) / 2 - longestOptionLength + 3, height / 6 * 5 - 5);
                                            Console.Write($"Health: {player.Health} / {player.MaxHealth}   ");
                                            Console.SetCursorPosition((width - 6) / 2, height / 6 * 5);
                                            Console.Write("BATTLE");
                                            Console.SetCursorPosition((width - 6) / 2, height / 6 * 5 + 1);
                                            Console.Write(speakerLine);
                                            Console.SetCursorPosition(eHealthBarBorder.CornerLR.X - attacker.Length - 19, 5);
                                            Console.Write($"Health: {enemyHealth} / {enemyMaxHealth}   ");
                                            linePosUD = 3;
                                            Console.SetCursorPosition((width - text.Length) / 2, height / 6 * 5 + linePosUD);
                                            foreach (char letter in text)
                                            {
                                                Console.Write(letter);
                                                Thread.Sleep(25);
                                            }

                                            if (options.Count() == 2)
                                                optionsOffset = 2;
                                            else if (options.Count() % 2 == 0)
                                                optionsOffset = 0;

                                            foreach (string option in options)
                                            {
                                                Console.SetCursorPosition((width + longestStringLength) / 2 + 1, height / 6 * 5 + optionsOffset + iterator);
                                                Console.Write(option);
                                                Console.CursorTop++;
                                                iterator += 2;
                                            }

                                            iterator = 0;
                                            Console.SetCursorPosition((width + longestStringLength) / 2 - 1, height / 6 * 5 + optionsOffset + iterator);
                                            Console.Write("►");
                                            break;
                                        default:
                                            ClearTextArea($"You used a {item.Name}!\n...\nBut nothing happened!", "BATTLE", textBoxCornersUL, textBoxCornersLR);
                                            break;
                                    }
                                    player.RemoveItem(item.Name);
                                    break;
                                case "Flee":
                                    if (rnd.Next(1, player.MaxHealth) < player.Health)
                                    {
                                        ClearTextArea("You Attempted To Flee...\nAnd Got Away Safely!", "BATTLE", textBoxCornersUL, textBoxCornersLR);
                                        activeTerrains.Remove(textBox);
                                        activeTerrains.Remove(eStats);
                                        activeTerrains.Remove(pHealthBar);
                                        activeTerrains.Remove(eHealthBar);
                                        activeTerrains.Remove(pName);
                                        activeTerrains.Remove(pHealthBarBackGround);
                                        RemoveTerrain(pNameBackground, false);
                                        RemoveTerrain(eStatsBorder, false);
                                        RemoveTerrain(eHealthBarBackground, false);
                                        RemoveTerrain(eHealthBarBorder, false);
                                        RemoveTerrain(textBoxBorder);
                                        return 2;
                                    }
                                    ClearTextArea("You Attempted To Flee...\nBut Nothing Happened!", "BATTLE", textBoxCornersUL, textBoxCornersLR);
                                    break;
                            }
                            if (exited)
                                continue;

                            if (enemyHealth <= 0)
                                break;

                            ClearTextArea($"{attacker} Attacks!\nYou Take {roundEnemyAtk} Damage!", "BATTLE", textBoxCornersUL, textBoxCornersLR);
                            if (player.Health - roundEnemyAtk > 0)
                            {
                                player.Health -= roundEnemyAtk;
                            }
                            else
                            {
                                player.Health = 0;
                            }
                            RemoveTerrain(pHealthBar, false);
                            if (player.Health > 0)
                            {
                                currentPHealthRatio = player.Health / (double)player.MaxHealth;
                                pHealthBar.CornerLR.X = Convert.ToInt32(pHealthBarBackGround.CornerUL.X + (pHealthBarBackGround.CornerLR.X - pHealthBarBackGround.CornerUL.X) * currentPHealthRatio);
                                Draw(pHealthBar);
                            }
                            break;
                    }
                    if (validInput)
                    {
                        break;
                    }
                }
            }

            activeTerrains.Remove(textBox);
            activeTerrains.Remove(eStats);
            activeTerrains.Remove(pHealthBar);
            activeTerrains.Remove(eHealthBar);
            activeTerrains.Remove(pName);
            activeTerrains.Remove(pHealthBarBackGround);
            RemoveTerrain(pNameBackground, false);
            RemoveTerrain(eStatsBorder, false);
            RemoveTerrain(eHealthBarBackground, false);
            RemoveTerrain(eHealthBarBorder, false);
            RemoveTerrain(textBoxBorder);

            if (player.Health <= 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Draw Obstacle & Add To ActiveObstacle List
        /// </summary>
        /// <param name="obstacle"></param>
        public static void Draw(IObstacle obstacle)
        {
            string[] obstacleParts = obstacle.Icon;
            Console.BackgroundColor = GetColor(Console.CursorLeft, Console.CursorTop)[0];
            Console.ForegroundColor = GetColor(Console.CursorLeft, Console.CursorTop)[1];
            Console.SetCursorPosition(obstacle.LocLR, obstacle.LocUD);
            foreach (string line in obstacleParts)
            {
                foreach (char character in line)
                {
                    Console.BackgroundColor = GetColor(Console.CursorLeft, Console.CursorTop)[0];
                    Console.ForegroundColor = GetColor(Console.CursorLeft, Console.CursorTop)[1];
                    Console.Write(character);
                }
                Console.CursorTop++;
                Console.CursorLeft = obstacle.LocLR;
            }
            activeObstacles.Add(obstacle);
            Console.SetCursorPosition(obstacle.LocLR, obstacle.LocUD);
            foreach (string line in obstacle.Icon)
            {
                foreach (char letter in line)
                {
                    int[] coords = new int[2];
                    coords[0] = Console.CursorLeft;
                    coords[1] = Console.CursorTop;
                    if (letter != ' ')
                        obstacleLocations.Add(coords);
                    if (obstacle.Interactible)
                    {
                        if (letter != ' ')
                            interactLocations.Add(coords, obstacle.Name);
                    }
                    Console.CursorLeft++;
                }
                Console.CursorTop++;
                Console.CursorLeft = obstacle.LocLR;
            }
        }

        /// <summary>
        /// Draw Terrain & Add To ActiveTerrain List
        /// </summary>
        /// <param name="terrain"></param>
        public static void Draw(Terrain terrain)
        {
            if (terrain.Visible)
            {
                ConsoleColor prevColor = Console.BackgroundColor;
                Console.BackgroundColor = terrain.Color;
                int startLeft = terrain.CornerUL.X;
                int startUp = terrain.CornerUL.Y;
                Console.SetCursorPosition(startLeft, startUp);
                for (int i = 0; i < terrain.CornerLR.Y - terrain.CornerUL.Y; i++)
                {
                    Console.SetCursorPosition(startLeft, startUp);
                    string fill = "";
                    for (int j = 0; j < (terrain.CornerLR.X - terrain.CornerUL.X); j++)
                    {
                        fill += " ";
                    }

                    Console.CursorLeft = startLeft;

                    Console.Write(fill);
                    startUp++;
                }
                Console.BackgroundColor = prevColor;
                Console.BackgroundColor = GetColor(0, 0)[0];
                Console.SetCursorPosition(0, 0);
            }
            activeTerrains.Add(terrain);
        }

        /// <summary>
        /// Draw Player
        /// </summary>
        /// <param name="prevLocLR"></param>
        /// <param name="prevLocUD"></param>
        public static void Draw(int prevLocLR, int prevLocUD)
        {
            string[] playerParts = player.Icon;
            Console.SetCursorPosition(prevLocLR, prevLocUD);
            Console.BackgroundColor = GetColor(Console.CursorLeft, Console.CursorTop)[0];
            Console.ForegroundColor = GetColor(Console.CursorLeft, Console.CursorTop)[1];
            foreach (string line in playerParts)
            {
                string clear = "";
                foreach (char space in line)
                    clear += " ";

                foreach (char space in clear)
                {
                    Console.BackgroundColor = GetColor(Console.CursorLeft, Console.CursorTop)[0];
                    Console.ForegroundColor = GetColor(Console.CursorLeft, Console.CursorTop)[1];
                    Console.Write(space);
                }
                Console.CursorTop++;
                Console.CursorLeft -= line.Length;
            }

            Console.SetCursorPosition(player.Loc.X, player.Loc.Y);
            Console.BackgroundColor = GetColor(Console.CursorLeft, Console.CursorTop)[0];
            Console.ForegroundColor = GetColor(Console.CursorLeft, Console.CursorTop)[1];
            foreach (string line in playerParts)
            {
                foreach (char character in line)
                {
                    Console.BackgroundColor = GetColor(Console.CursorLeft, Console.CursorTop)[0];
                    Console.ForegroundColor = GetColor(Console.CursorLeft, Console.CursorTop)[1];
                    Console.Write(character);
                }
                Console.CursorTop++;
                Console.CursorLeft -= line.Length;
            }
        }

        /// <summary>
        /// Returns Valid User Input
        /// </summary>
        /// <returns></returns>
        public static char GetDir()
        {
            string[] validKeys = new string[]
            {
                "uparrow",
                "downarrow",
                "leftarrow",
                "rightarrow",
                "w",
                "s",
                "a",
                "d",
                "enter",
            };
            ConsoleKey key;
            while (true)
            {
                try
                {
                    key = Console.ReadKey().Key;
                    if (key == ConsoleKey.Escape)
                    {
                        Console.CursorLeft--;
                        Console.Write(" ");
                        Console.CursorLeft--;
                        return 'x';
                    }
                    if (validKeys.Contains(key.ToString().ToLower()))
                    {
                        if (key.ToString().ToLower() == "enter")
                            break;
                        Console.CursorLeft--;
                        Console.Write(" ");
                        Console.CursorLeft--;
                        break;
                    }
                    else
                    {
                        Console.CursorLeft--;
                        Console.Write(" ");
                        Console.CursorLeft--;
                        throw new Exception();
                    }
                }
                catch { }
            }
            char dir;
            switch (key.ToString().ToLower())
            {
                case "uparrow":
                case "w":
                    dir = 'u';
                    break;
                case "downarrow":
                case "s":
                    dir = 'd';
                    break;
                case "leftarrow":
                case "a":
                    dir = 'l';
                    break;
                case "rightarrow":
                case "d":
                    dir = 'r';
                    break;
                case "enter":
                    dir = 'e';
                    break;
                default:
                    dir = ' ';
                    break;
            }
            return dir;
        }

        /// <summary>
        /// Move Player or Return False if Unable To Move
        /// </summary>
        /// <returns></returns>
        public static bool MovePlayer(out char instruction)
        {
            int prevLocLR = player.Loc.X;
            int prevLocUD = player.Loc.Y;
            char dir = GetDir();
            bool walked = true;
            if (dir == 'e' || dir == 'x')
            {
                walked = false;
            }
            else
            {
                LastDir = dir;
            }

            if (WalkCheck(dir, ref walked) && walked)
            {
                player.Walk(dir);
                Draw(prevLocLR, prevLocUD);
            }

            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = GetColor(0, 0)[0];
            instruction = dir;
            return walked;
        }

        /// <summary>
        /// Check If Player Can Walk In Desired Direction
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="walked"></param>
        /// <returns></returns>
        public static bool WalkCheck(char dir, ref bool walked)
        {
            Player playerGhost = new Player()
            {
                Loc = new Vector2(player.Loc.X, player.Loc.Y),
                Icon = player.Icon
            };
            playerGhost.Walk(dir);
            if (Console.WindowWidth != width)
            {
                Console.WindowWidth = width;
                Refresh();
                Console.CursorVisible = false;
                return false;
            }
            if (Console.WindowHeight != height)
            {
                Console.WindowHeight = height;
                Refresh();
                Console.CursorVisible = false;
                return false;
            }
            Console.SetCursorPosition(playerGhost.Loc.X, playerGhost.Loc.Y);

            foreach (string line in playerGhost.Icon)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    foreach (int[] coord in obstacleLocations)
                    {
                        if (coord[0] == Console.CursorLeft)
                        {
                            if (coord[1] == Console.CursorTop)
                            {
                                return false;
                            }
                        }
                    }
                    if (GetLoadZone(Console.CursorLeft, Console.CursorTop) != "")
                        walked = false;
                    if (!GetWalkability(Console.CursorLeft, Console.CursorTop))
                        return false;
                    Console.CursorLeft++;
                }
                Console.CursorTop++;
                Console.CursorLeft = playerGhost.Loc.X;
            }
            return true;
        }

        /// <summary>
        /// Return Boolean If Terrain Is Walkable
        /// </summary>
        /// <param name="locLR"></param>
        /// <param name="locUD"></param>
        /// <returns></returns>
        public static bool GetWalkability(int locLR, int locUD)
        {
            for (int i = activeTerrains.Count() - 1; i >= 0; i--)
            {
                if (locLR >= activeTerrains[i].CornerUL.X && locLR < activeTerrains[i].CornerLR.X)
                {
                    if (locUD >= activeTerrains[i].CornerUL.Y && locUD < activeTerrains[i].CornerLR.Y)
                    {
                        if (activeTerrains[i].Interactible)
                            return false;
                        return activeTerrains[i].Walkable;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Return Name Of Terrain Marked As LoadZone That Player Is Interacting With
        /// </summary>
        /// <param name="locLR"></param>
        /// <param name="locUD"></param>
        /// <returns></returns>
        public static string GetLoadZone(int locLR, int locUD)
        {
            for (int i = activeTerrains.Count() - 1; i >= 0; i--)
            {
                if (locLR >= activeTerrains[i].CornerUL.X && locLR < activeTerrains[i].CornerLR.X)
                {
                    if (locUD >= activeTerrains[i].CornerUL.Y && locUD < activeTerrains[i].CornerLR.Y)
                    {
                        if (activeTerrains[i].Interactible)
                        {
                            return activeTerrains[i].Name;
                        }
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// Returns color of position based on last-placed visible terrain
        /// </summary>
        /// <param name="locLR"></param>
        /// <param name="locUD"></param>
        /// <returns></returns>
        public static List<ConsoleColor> GetColor(int locLR = -1, int locUD = -1)
        {
            if (locLR < 0 || locUD < 0)
            {
                locLR = Console.CursorLeft;
                locUD = Console.CursorTop;
            }
            for (int i = activeTerrains.Count() - 1; i >= 0; i--)
            {
                if (activeTerrains[i].Visible)
                {
                    if (locLR >= activeTerrains[i].CornerUL.X && locLR < activeTerrains[i].CornerLR.X)
                    {
                        if (locUD >= activeTerrains[i].CornerUL.Y && locUD < activeTerrains[i].CornerLR.Y)
                        {
                            return new List<ConsoleColor>() { activeTerrains[i].Color, activeTerrains[i].TextColor };
                        }
                    }
                }
            }
            return new List<ConsoleColor>() { ConsoleColor.Black, ConsoleColor.White };
        }

        /// <summary>
        /// Return Name Of Terrain At Location
        /// </summary>
        /// <param name="locLR"></param>
        /// <param name="locUD"></param>
        /// <returns></returns>
        public static string GetTerrainName(int locLR, int locUD)
        {
            for (int i = terrains.Count() - 1; i >= 0; i--)
            {
                if (locLR >= terrains[i].CornerUL.X && locLR < terrains[i].CornerLR.X)
                {
                    if (locUD >= terrains[i].CornerUL.Y && locUD < terrains[i].CornerLR.Y)
                    {
                        return terrains[i].Name;
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// Return Interactible Obstacle In Range Of Player
        /// </summary>
        /// <returns></returns>
        public static IInteract GetInteract()
        {
            Player playerGhost = new Player()
            {
                Loc = new Vector2(player.Loc.X, player.Loc.Y),
                Icon = player.Icon
            };
            playerGhost.Walk(LastDir);
            Console.SetCursorPosition(playerGhost.Loc.X, playerGhost.Loc.Y);

            foreach (string line in playerGhost.Icon)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    foreach (KeyValuePair<int[], string> coord in interactLocations)
                    {
                        if (coord.Key[0] == Console.CursorLeft)
                        {
                            if (coord.Key[1] == Console.CursorTop)
                            {
                                IObstacle tempObstacle =  GetObstacle(coord.Value);
                                if (tempObstacle is IInteract interactor)
                                    return interactor;
                            }
                        }
                    }
                    string tempLoadCheck = GetLoadZone(Console.CursorLeft, Console.CursorTop);
                    if (tempLoadCheck != "")
                    {
                        Terrain tempTerrain = GetTerrain(tempLoadCheck);
                        if (tempTerrain is IInteract interactor)
                        {
                            return interactor;
                        }
                    }
                    Console.CursorLeft++;
                }
                Console.CursorTop++;
                Console.CursorLeft = playerGhost.Loc.X;
            }
            return null;
        }

        /// <summary>
        /// Removes Obstacle from active obstacles, Erases From Level
        /// </summary>
        /// <param name="obstacle"></param>
        public static void RemoveObstacle(IObstacle obstacle)
        {
            for (int x = 0; x < obstacleLocations.Count(); x++)
            {
                Console.SetCursorPosition(obstacle.LocLR, obstacle.LocUD);
                foreach (string line in obstacle.Icon)
                {
                    while (true)
                    {
                        try
                        {
                            if (obstacleLocations[x][1] == Console.CursorTop)
                            {
                                foreach (char character in line)
                                {
                                    Console.BackgroundColor = GetColor(obstacleLocations[x][0], obstacleLocations[x][1])[0];
                                    while (true)
                                    {
                                        try
                                        {
                                            if (obstacleLocations[x][0] == Console.CursorLeft)
                                            {
                                                obstacleLocations.Remove(obstacleLocations[x]);
                                                Console.Write(" ");
                                                Console.CursorLeft--;
                                            }
                                            Console.CursorLeft++;
                                            break;
                                        }
                                        catch
                                        {
                                            if (Console.CursorLeft > obstacle.LocLR + line.Length)
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                Console.CursorLeft++;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                        catch
                        {
                            if (Console.CursorTop > obstacle.LocUD + obstacle.Icon.Count())
                            {
                                break;
                            }
                            else
                            {
                                Console.CursorTop++;
                            }
                        }
                    }
                    Console.CursorLeft = obstacle.LocLR;
                    Console.CursorTop++;
                }
            }
            activeObstacles.Remove(obstacle);
            obstacles.Remove(obstacle);
        }

        /// <summary>
        /// Removes Terrain from active Terrains, Erases From Level
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="redrawObjects"></param>
        public static void RemoveTerrain(Terrain terrain, bool redrawObjects = true)
        {
            activeTerrains.Remove(terrain);
            ConsoleColor prevColor = Console.BackgroundColor;
            Console.BackgroundColor = GetColor(Console.CursorLeft, Console.CursorTop)[0];
            int startLeft = terrain.CornerUL.X;
            int startUp = terrain.CornerUL.Y - 1;
            Console.SetCursorPosition(startLeft, startUp + 1);
            for (int i = 0; i < (terrain.CornerLR.Y - terrain.CornerUL.Y); i++)
            {
                Console.WriteLine();
                startUp++;
                Console.SetCursorPosition(startLeft, startUp);
                string fill = "";
                for (int j = 0; j < (terrain.CornerLR.X - terrain.CornerUL.X); j++)
                {
                    fill += " ";
                }

                Console.CursorLeft = startLeft;
                foreach (char space in fill)
                {
                    Console.BackgroundColor = GetColor(Console.CursorLeft, startUp)[0];
                    Console.Write(space);
                }
            }

            if (redrawObjects)
            {
                obstacleLocations.Clear();
                activeObstacles.Clear();
                foreach (IObstacle obstacle in obstacles)
                {
                    Draw(obstacle);
                }
                Draw(player.Loc.X, player.Loc.Y);
            }

            Console.BackgroundColor = GetColor(0, 0)[0];
            Console.SetCursorPosition(0, 0);
        }

        /// <summary>
        /// Returns Terrain Object by Name Reference
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Terrain GetTerrain(string name)
        {
            foreach (Terrain terrain in terrains)
            {
                if (terrain.Name == name)
                {
                    return terrain;
                }
            }
            return new Terrain();
        }

        /// <summary>
        /// Returns Obstacle Object by Name Reference
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IObstacle GetObstacle(string name)
        {
            foreach (IObstacle obstacle in obstacles)
            {
                if (obstacle.Name == name)
                {
                    return obstacle;
                }
            }
            return null;
        }
    }
}