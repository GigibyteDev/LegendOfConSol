using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _2DEngine;

namespace LegendOfConSol
{
    class Program
    {
        public static Random rnd = new Random(Guid.NewGuid().GetHashCode());
        public static SaveData save = new SaveData();

        static void Main(string[] args)
        {
            Engine.StartUp();
            AddStarterInv();
            List<string> temp = new List<string>();

            int lvlSelect = 2;
            bool gameLoop = true;
            Action objects = LevelOneObjects;
            Func<int> interacts = LevelOneInteracts;
            while (gameLoop)
            {
                switch (lvlSelect)
                {
                    case 1:
                        objects = LevelOneObjects;
                        interacts = LevelOneInteracts;
                        break;
                    case 2:
                        objects = LevelTwoObjects;
                        interacts = LevelTwoInteracts;
                        break;
                    case 3:
                        objects = CastleLevelObjects;
                        interacts = CastleLevelInteracts;
                        break;
                    case 4:
                        objects = LevelFourObjects;
                        interacts = LevelFourInteracts;
                        break;
                    default: //Game Over Screen
                        Engine.player.Loc.X = Console.WindowWidth / 2 - 1;
                        Engine.player.Loc.Y = Console.WindowHeight / 2 + 4;
                        Engine.Clear();
                        Engine.Refresh();
                        Console.SetCursorPosition((Console.WindowWidth - 10) / 2, Console.WindowHeight / 2);
                        Console.WriteLine("GAME OVER.");
                        Console.ReadLine();
                        lvlSelect = 1;
                        objects = LevelOneObjects;
                        interacts = LevelOneInteracts;
                        save = new SaveData();
                        break;
                }
                lvlSelect = Engine.GameLoop(objects, interacts, lvlSelect);
                Engine.Clear();
            }
        }

        public static void LevelOneObjects()
        {
            Console.SetWindowPosition(0, 0);
            Engine.terrains.Add(new Terrain()
            {
                Name = "River",
                Color = ConsoleColor.DarkCyan,
                TextColor = ConsoleColor.DarkBlue,
                CornerUL = new Vector2(0,  0),
                CornerLR = new Vector2(Console.WindowWidth, Console.WindowHeight),
                Walkable = false
            });
            Engine.terrains.Add(new Terrain()
            {
                Name = "Beach",
                Color = ConsoleColor.DarkYellow,
                TextColor = ConsoleColor.Black,
                CornerUL = new Vector2(0, 0),
                CornerLR = new Vector2(Console.WindowWidth / 6 * 5 + 2, Console.WindowHeight - 5 + 3)

            });
            Engine.terrains.Add(new Terrain()
            {
                Name = "Grass",
                Color = ConsoleColor.DarkGreen,
                TextColor = ConsoleColor.Black,
                CornerUL = new Vector2(0, 0),
                CornerLR = new Vector2(Console.WindowWidth / 6 * 5 , Console.WindowHeight - 3)
            });
            Engine.terrains.Add(new Terrain()
            {
                Name = "Road",
                Color = ConsoleColor.DarkGray,
                TextColor = ConsoleColor.Black,
                CornerUL = new Vector2(Console.WindowWidth / 5 * 2, 0),
                CornerLR = new Vector2(Console.WindowWidth  / 5 * 3, Console.WindowHeight - 3)
            });

            Engine.terrains.Add(new Terrain()
            {
                Name = "Level Two Load",
                Color = ConsoleColor.DarkGray,
                TextColor = ConsoleColor.DarkGray,
                CornerUL = new Vector2(Console.WindowWidth - 10, Console.WindowHeight / 3 + 5),
                CornerLR = new Vector2(Console.WindowWidth, Console.WindowHeight - 10),
                Interactible = true
            });


            if (save.BridgeActive)
                Engine.terrains.Add(new Terrain
                {
                    Name = "Bridge",
                    Color = ConsoleColor.DarkGray,
                    TextColor = ConsoleColor.Black,
                    CornerUL = new Vector2(Console.WindowWidth / 6 * 5, Console.WindowHeight / 3 + 5),
                    CornerLR = new Vector2(Console.WindowWidth - 10, Console.WindowHeight - 10)
                });

            string[] tempIcon;

            string[] treeShape = new string[]
            {
                 @" /\",
                 @"/__\",
                 @" || "
            };
            int totalTrees = 10;
            if (!save.TreeLocations.ContainsKey("Tree #0"))
            {
                for (int i = 0; i < totalTrees; i++)
                {
                    int LR = 0, UD = 0;
                    do
                    {
                        LR = rnd.Next(1, Engine.width - 4);
                        UD = rnd.Next(1, Engine.height - 4);
                    } while (Engine.GetTerrainName(LR + 2, UD + 2) != "Grass");
                    WorldObject tempTree = new WorldObject($"Tree #{i}", LR, UD, treeShape, true);
                    save.TreeLocations.Add(tempTree.Name, new int[] { tempTree.LocLR, tempTree.LocUD });
                    Engine.obstacles.Add(tempTree);
                }
            }
            else
            {
                foreach(KeyValuePair<string, int[]> tree in save.TreeLocations)
                {
                    if (save.TreesCut.Contains(tree.Key))
                    {
                        treeShape = new string[]
                            {
                                @"",
                                @"",
                                @" || "
                            };
                    }
                    
                    Engine.obstacles.Add(new WorldObject(tree.Key, tree.Value[0], tree.Value[1], treeShape, true));
                }

            }

            tempIcon = new string[]
                {
                    @"  _   ",
                    @" / \||",
                    @"/___\|",
                    @"|[] |",
                    @"|[.]|",
                };
            Engine.obstacles.Add(new WorldObject("House", Engine.width / 2, 5, tempIcon, true));

            //Dialogue
            //Literally The First Guy You Saw

            Dictionary<string, List<string>> dialogue = new Dictionary<string, List<string>>()
            {
                { "Default", new List<string>(){
                    "Greetings!\nWelcome to Level One!"
                } },
                { "Trees", new List<string>(){
                    "My.... My trees!\nMy wonderful trees!\n... I planted those trees 57 years ago with my father...",
                    "Who could have done this?!\nWhy would someone do this?!",
                    "Wait...\nDid you do this?!\nI won't let you get away with this!!"
                } },
                { "Win", new List<string>()
                {
                    "shit\n...\nAlright you win. Have some Health Potions"
                } },
                { "BridgeOptionsDia", new List<string>()
                {
                    "That was weird...\nAnyway, what would you like to do?"
                } },
                { "BridgeOptions", new List<string>()
                {
                    "Nothing",
                    "Say Sike",
                    "Open Bridge"
                } },
                {
                    "Bridge", new List<string>()
                {
                    "There ya go!",
                } },
                {
                    "Sike", new List<string>()
                {
                    "Sike"

                } },
                {
                    "Bridge Open", new List<string>()
                {
                    "It's Already Open, Dingus"
                } }
            };

            tempIcon = new string[]
                {
                    @" O ",
                    @"/|\",
                    @"/ \",
                };
            Engine.obstacles.Add(new NPC("Literally The First Guy You Saw", Engine.width / 2 + 8, 8, tempIcon, dialogue, 50, 10));
        }
        public static int LevelOneInteracts()
        {
            IInteract interact = Engine.GetInteract();
            if (interact == null)
            {
                Console.SetCursorPosition(0, 0);
                return 1;
            }
            
            if (interact is NPC npc)
            {
                switch (npc.Name)
                {
                    case "Literally The First Guy You Saw":
                        if (save.TreesCut.Count() == 10)
                        {
                            save.TreesCut.Add("finished");
                            Engine.DrawTextBox(npc.Dialogue["Trees"], npc.Name);
                            int himBattleresult = Engine.Battle(npc);
                            switch (himBattleresult)
                            {
                                case 1:
                                    Engine.DrawTextBox(npc.Dialogue["Win"], npc.Name);
                                    Engine.DrawTextBox($"{interact.Name} Gave You Health Potions x10", "");
                                    Engine.player.AddItem("Health Potion", 10);
                                    break;
                            }
                        }
                        else if (save.HimFlag)
                        {
                            string answer = Engine.DrawTextBox(npc.Dialogue["BridgeOptionsDia"][0], npc.Name, npc.Dialogue["BridgeOptions"]);
                            if (answer == "Open Bridge")
                            {
                                if (!save.BridgeActive)
                                {
                                    Vector2 bridgeCoordsUL = new Vector2(Console.WindowWidth / 6 * 5, Console.WindowHeight / 3 + 5);
                                    Vector2 bridgeCoordsLR = new Vector2(Console.WindowWidth - 35, Console.WindowHeight - 10);
                                    Terrain bridge = new Terrain
                                    {
                                        Name = "Bridge",
                                        Color = ConsoleColor.DarkGray,
                                        TextColor = ConsoleColor.Black,
                                        CornerUL = bridgeCoordsUL,
                                        CornerLR = bridgeCoordsLR
                                    };
                                    for (int i = 0; i < 5; i++)
                                    {
                                        bridgeCoordsLR.X += 5;
                                        Engine.Draw(bridge);
                                        Thread.Sleep(500);
                                    }
                                    Engine.terrains.Add(bridge);
                                    save.BridgeActive = true;
                                    Engine.DrawTextBox(npc.Dialogue["Bridge"], npc.Name);
                                }
                                else
                                {
                                    Engine.DrawTextBox(npc.Dialogue["Bridge Open"], npc.Name);
                                }
                            }
                            else if (answer == "Say Sike")
                            {
                                Engine.DrawTextBox(npc.Dialogue["Sike"], npc.Name);
                            }
                        }
                        else
                            Engine.DrawTextBox(npc.Dialogue["Default"], npc.Name);
                        break;
                    case "HIM":
                        if (save.HimTalkCount < 2)
                        {
                            Engine.DrawTextBox(npc.Dialogue["Default"], npc.Name);
                            save.HimFlag = true;
                            save.HimTalkCount++;
                        }
                        else
                        {
                            Engine.DrawTextBox(npc.Dialogue["Wake Up"], npc.Name);
                            save.HimTalkCount = 0;
                        }
                        break;
                }
            }
            else if (interact is WorldObject obj)
            {
                if (obj.Name.Contains("Tree"))
                {
                    if (!save.TreesCut.Contains(interact.Name))
                    {
                        IObstacle tempTree = Engine.GetObstacle(interact.Name);
                        Engine.RemoveObstacle(tempTree);
                        tempTree.Icon = new string[]
                        {
                       @"    ",
                       @"    ",
                       @" || "
                        };
                        Engine.obstacles.Add(tempTree);
                        Engine.Draw(tempTree);
                        save.TreesCut.Add(interact.Name);
                    }
                }
                else if (obj.Name == "House")
                {
                    if (Engine.terrains.Count() > 0)
                    {
                        Engine.terrains.Clear();

                        IObstacle tempObstacle = Engine.GetObstacle("House");
                        Engine.obstacles.Clear();
                        tempObstacle.Icon = new string[]
                        {
                                @" ___",
                                @"|   |",
                                @"|  o|",
                                @"|   |",
                                @"|___|"
                        };
                        Engine.obstacles.Add(tempObstacle);

                        string[] tempIcon = new string[]
                            {
                                    @" \ /",
                                    @" O O",
                                    @"  Y",
                                    @"{___}"
                            };
                        Dictionary<string, List<string>> dialogue = new Dictionary<string, List<string>>()
                        {
                            { "Default", new List<string>(){
                                "Get Out"
                            } },
                            { "Wake Up", new List<string>(){
                                "Run Run Run Run Run Run Run Run\nRun Run Run Run Run Run Run Run\nRun Run Run Run Run Run Run Run"
                            } }
                        };
                        Engine.obstacles.Add(new NPC("HIM", Engine.width / 2 - 1, Engine.height / 2 - 2, tempIcon, dialogue, 10000, 10000));
                        Engine.Refresh();
                    }
                    else
                    {
                        Engine.terrains.Clear();
                        Engine.obstacles.Clear();
                        LevelOneObjects();
                        Engine.Refresh();
                    }
                }
            }
            else if (interact is Terrain loadZone)
            {
                switch (loadZone.Name)
                {
                    case "Level Two Load":
                        Engine.player.Loc.X = 12;
                        return 2;
                    default:
                        break;
                }
            }
            
            Console.SetCursorPosition(0, 0);
            return 1;
        }

        public static void LevelTwoObjects()
        {
            Engine.terrains.Add(new Terrain
            {
                Name = "Magma",
                Color = ConsoleColor.DarkMagenta,
                TextColor = ConsoleColor.Gray,
                Walkable = false
            });
            Engine.terrains.Add(new Terrain
            {
                Name = "path",
                Color = ConsoleColor.DarkGray,
                TextColor = ConsoleColor.Black,
                CornerUL = new Vector2(0, Console.WindowHeight / 3 + 5),
                CornerLR = new Vector2(Console.WindowWidth - 10, Console.WindowHeight - 10)
            });
            Engine.terrains.Add(new Terrain()
            {
                Name = "Path 2",
                Color = ConsoleColor.DarkGray,
                TextColor = ConsoleColor.Black,
                CornerUL = new Vector2(Console.WindowWidth / 5 * 2, 0),
                CornerLR = new Vector2(Console.WindowWidth  / 5 * 3, Console.WindowHeight / 3 + 5)
            });
            Engine.terrains.Add(new Terrain()
            {
                Name = "Castle Level Load",
                Color = ConsoleColor.DarkGreen,
                TextColor = ConsoleColor.Black,
                CornerUL = new Vector2(Console.WindowWidth / 2 - 3, Console.WindowHeight / 3 - 1),
                CornerLR = new Vector2(Console.WindowWidth  / 5 * 3 - 16, Console.WindowHeight / 3 + 2),
                Visible = false,
                Interactible = true
            });
            Engine.terrains.Add(new Terrain()
            {
                Name = "Level One Load",
                Color = ConsoleColor.DarkGreen,
                TextColor = ConsoleColor.Black,
                CornerUL = new Vector2(0, Console.WindowHeight / 3 + 5),
                CornerLR = new Vector2(10, Console.WindowHeight - 10),
                Interactible = true
            });

            if (save.KingTalked)
            {
                if (!save.WatchedLvl2Anim)
                {
                    Engine.terrains.Add(new Terrain()
                    {
                        Name = "BridgeTrigger",
                        Color = ConsoleColor.Black,
                        CornerUL = new Vector2(Console.WindowWidth / 2 - 10, Console.WindowHeight / 3 + 7),
                        CornerLR = new Vector2(Console.WindowWidth / 2 + 10, Console.WindowHeight / 3 + 20),
                        Visible = false,
                        Interactible = true
                    });
                }
                else
                {
                    Engine.terrains.Add(new Terrain()
                    {
                        Name = "Bridge",
                        Color = ConsoleColor.DarkGray,
                        TextColor = ConsoleColor.Black,
                        CornerUL = new Vector2(Console.WindowWidth - 10, Console.WindowHeight / 3 + 5),
                        CornerLR = new Vector2(Console.WindowWidth, Console.WindowHeight - 10),
                        Interactible = true
                    });
                }
            }

            string[] rockShape = new string[]
            {
                @"  _",
                @" / \",
                @"/___\"
            };
            string[] tempIcon;
            int rockAmt = 10;
            if (!save.RockLocations.ContainsKey("Rock #0"))
            {
                for (int i = 0; i < rockAmt; i++)
                {
                    int LR = 0, UD = 0;
                    do
                    {
                        LR = rnd.Next(1, Engine.width - 6);
                        UD = rnd.Next(1, Engine.height - 6);
                    } while (Engine.GetTerrainName(LR + 2, UD + 2) != "Magma");
                    WorldObject tempRock = new WorldObject($"Rock #{i}", LR, UD, rockShape);
                    Engine.obstacles.Add(tempRock);
                    save.RockLocations.Add(tempRock.Name, new int[] { tempRock.LocLR, tempRock.LocUD });
                }
            }
            else
            {
                foreach(KeyValuePair<string, int[]> rock in save.RockLocations)
                {
                    Engine.obstacles.Add(new WorldObject(rock.Key, rock.Value[0], rock.Value[1], rockShape));
                }
            }

            int guardOffset = 0;
            if (!save.GuardBlock)
                guardOffset = 4;

            tempIcon = new string[]
                {
                    @"            |>",
                    @"            |",
                    @"           / \ ",
                    @"          /   \",
                    @"    __   /     \      __",
                    @"   |__| /       \    |__|",
                    @"   |   /         \   |",
                    @"  /\  /           \  /\",
                    @" /  \/             \/  \",
                    @"/_______________________\",
                    @"|  _                 _  |",
                    @"| |_|               |_| |",
                    @"|          ---          |",
                    @"|        /     \        |",
                    @"|       |       |       |",
                    @"|_______|       |_______|",
                };
            Engine.obstacles.Add(new WorldObject("Castle", Engine.width / 2 - 12, 2, tempIcon));

            tempIcon = new string[]
            {
                @" _",
                @" O ^",
                @"/|\|",
                @"/ \|",
            };
            Engine.obstacles.Add(new NPC("Guard #1", Engine.width / 2 - 4 - guardOffset, 18, tempIcon, 200, 32));

            Engine.obstacles.Add(new NPC("Guard #2", Engine.width / 2 + 1 + guardOffset, 18, tempIcon, 200, 32));
        }
        public static int LevelTwoInteracts()
        {
            IInteract interact = Engine.GetInteract();
            if (interact == null)
            {
                Console.SetCursorPosition(0, 0);
                return 2;
            }
            switch (interact.Name)
            {
                case "Level One Load":
                    save.BridgeActive = true;
                    Engine.player.Loc.X = Console.WindowWidth - 15;
                    return 1;
                case "Castle Level Load":
                    Engine.player.Loc.X = Console.WindowWidth / 2;
                    Engine.player.Loc.Y = Console.WindowHeight - 10;
                    return 3;
                case "Guard #1":
                case "Guard #2":
                    if (save.GuardBlock)
                    {
                        List<string> tempDialogue = new List<string>()
                        {
                            "To Save The World",
                            "I Will Kill Him",
                            "*Poke Guard's Eyes*",
                            "Nothing"
                        };
                        string response = Engine.DrawTextBox("HALT!\nWhat Business Do You Have With The King?", interact.Name, tempDialogue);
                        switch (response)
                        {
                            case "To Save The World":
                                Engine.DrawTextBox("Well, by all means then!\nEnjoy Your Stay!", interact.Name);
                                IObstacle guard1 = Engine.GetObstacle("Guard #1");
                                IObstacle guard2 = Engine.GetObstacle("Guard #2");
                                Engine.RemoveObstacle(guard1);
                                Engine.RemoveObstacle(guard2);
                                guard1.LocLR -= 4;
                                guard2.LocLR += 4;
                                Engine.obstacles.Add(guard1);
                                Engine.obstacles.Add(guard2);
                                Engine.Draw(guard1);
                                Engine.Draw(guard2);
                                save.GuardBlock = false;
                                break;
                            case "I Will Kill Him":
                                string[] dialogue = new string[]
                                {
                                    "Wh...\nWhat?\n...\nDid you really think I would let you pass after that?",
                                    "No but like...\nReally.\nWhy did you say that?",
                                    "Like\n...\nI think I have to arrest you now\n...",
                                    "Bye..."
                                };
                                Engine.DrawTextBox(dialogue, interact.Name);
                                return 0;
                            case "*Poke Guard's Eyes*":
                                switch (save.GuardPoke)
                                {
                                    case 0:
                                        Engine.DrawTextBox("Ow Haha\nStop~", interact.Name);
                                        save.GuardPoke++;
                                        break;
                                    case 1:
                                        Engine.DrawTextBox("Wh- OW\nAlright, stop that...", interact.Name);
                                        save.GuardPoke++;
                                        break;
                                    case 2:
                                        Engine.DrawTextBox("OW\nAlright Mister\nNo More Mr. Nice. Guard\nIf you do that again, you aren't going to like it...", interact.Name);
                                        save.GuardPoke++;
                                        break;
                                    case 3:
                                        Engine.DrawTextBox("ALRIGHT, that's it.\nTake This!", interact.Name);
                                        if (interact is NPC npc)
                                        {
                                            switch (Engine.Battle(npc))
                                            {
                                                case 0:
                                                    return 0;
                                                case 1:
                                                    Engine.DrawTextBox("Alright! Alright, you win!", npc.Name);
                                                    guard1 = Engine.GetObstacle("Guard #1");
                                                    guard2 = Engine.GetObstacle("Guard #2");
                                                    Engine.RemoveObstacle(guard1);
                                                    Engine.RemoveObstacle(guard2);
                                                    guard1.LocLR -= 4;
                                                    guard2.LocLR += 4;
                                                    Engine.obstacles.Add(guard1);
                                                    Engine.obstacles.Add(guard2);
                                                    Engine.Draw(guard1);
                                                    Engine.Draw(guard2);
                                                    save.GuardBlock = false;
                                                    break;
                                            }

                                        }
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        List<string> tempDialogue = new List<string>()
                        {
                            "Do Nothing",
                            "*Poke Guard's Eyes*"
                        };
                        string response = Engine.DrawTextBox("*The Guard Is Glaring At You...*", "", tempDialogue);
                        switch (response)
                        {
                            case "*Poke Guard's Eyes*":
                                switch (save.GuardPoke)
                                {
                                    case 0:
                                        Engine.DrawTextBox("Ow Haha\nStop~", interact.Name);
                                        save.GuardPoke++;
                                        break;
                                    case 1:
                                        Engine.DrawTextBox("Wh- OW\nAlright, stop that...", interact.Name);
                                        save.GuardPoke++;
                                        break;
                                    case 2:
                                        Engine.DrawTextBox("OW\nAlright Mister\nNo More Mr. Nice. Guard\nIf you do that again, you aren't going to like it...", interact.Name);
                                        save.GuardPoke++;
                                        break;
                                    case 3:
                                        Engine.DrawTextBox("ALRIGHT, that's it.\nTake This!", interact.Name);

                                        if (interact is NPC npc)
                                        {
                                            switch (Engine.Battle(npc))
                                            {
                                                case 0:
                                                    return 0;
                                                case 1:
                                                    Engine.DrawTextBox("Alright! Alright, you win!", npc.Name);
                                                    IObstacle guard1 = Engine.GetObstacle("Guard #1");
                                                    IObstacle guard2 = Engine.GetObstacle("Guard #2");
                                                    Engine.RemoveObstacle(guard1);
                                                    Engine.RemoveObstacle(guard2);
                                                    guard1.LocLR -= 4;
                                                    guard2.LocLR += 4;
                                                    Engine.obstacles.Add(guard1);
                                                    Engine.obstacles.Add(guard2);
                                                    Engine.Draw(guard1);
                                                    Engine.Draw(guard2);
                                                    save.GuardBlock = false;
                                                    break;
                                            }

                                        }
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "BridgeTrigger":
                    Terrain bridge = (new Terrain()
                    {
                        Name = "Bridge",
                        Color = ConsoleColor.DarkGray,
                        TextColor = ConsoleColor.Black,
                        CornerUL = new Vector2(Console.WindowWidth - 10, Console.WindowHeight / 3 + 5),
                        CornerLR = new Vector2(Console.WindowWidth, Console.WindowHeight - 10),
                        Interactible = true
                    });
                    Engine.Draw(bridge);
                    Engine.RemoveTerrain(Engine.GetTerrain("BridgeTrigger"));
                    save.WatchedLvl2Anim = true;
                    break;
                case "Bridge":
                    Engine.player.Loc.X = 15;
                    return 4;
                default:
                    break;
            }
            Console.SetCursorPosition(0, 0);
            return 2;
        }

        public static void CastleLevelObjects()
        {
            Engine.terrains.Add(new Terrain()
            {
                Name = "Tile",
                Color = ConsoleColor.Gray,
                TextColor = ConsoleColor.Black,
                Walkable = false
            });
            Engine.terrains.Add(new Terrain()
            {
                Name = "Pathway",
                Color = ConsoleColor.Red,
                TextColor = ConsoleColor.Black,
                CornerUL = new Vector2(Console.WindowWidth / 5 * 2, 0),
                CornerLR = new Vector2(Console.WindowWidth  / 5 * 3, Console.WindowHeight)
            });
            Engine.terrains.Add(new Terrain()
            {
                Name = "PodiumShadow",
                Color = ConsoleColor.DarkYellow,
                TextColor = ConsoleColor.Black,
                CornerUL = new Vector2(Console.WindowWidth / 10  - 1, 0),
                CornerLR = new Vector2(Console.WindowWidth / 10 * 9 + 1, 11)
            });
            Engine.terrains.Add(new Terrain()
            {
                Name = "Podium",
                Color = ConsoleColor.Yellow,
                TextColor = ConsoleColor.Black,
                CornerUL = new Vector2(Console.WindowWidth / 10, 0),
                CornerLR = new Vector2(Console.WindowWidth / 10 * 9, 9),
                Walkable = false
            });
            
            Engine.terrains.Add(new Terrain()
            {
                Name = "Level Two Load",
                Color = ConsoleColor.DarkMagenta,
                TextColor = ConsoleColor.DarkRed,
                CornerUL = new Vector2(Console.WindowWidth / 2 - 15, Console.WindowHeight - 3),
                CornerLR = new Vector2(Console.WindowWidth / 2 + 15, Console.WindowHeight),
                Interactible = true
            });

            string[] tempString = new string[]
                {
                    @"    ^",
                    @"   _O_",
                    @"  |/|\|",
                    @" _/_|_\_",
                    @"| |   | |",
                    @"|_|___|_|",
                    @"  /   \",
                };
            Engine.obstacles.Add(new NPC("King", Engine.width / 2 - 4, 2, tempString, 500, 45));
        }
        public static int CastleLevelInteracts()
        {
            IInteract interact = Engine.GetInteract();
            if (interact == null)
            {
                Console.SetCursorPosition(0, 0);
                return 3;
            }
            switch (interact.Name)
            {
                case "Level Two Load":
                    save.GuardBlock = false;
                    Engine.player.Loc.X = Console.WindowWidth / 2;
                    Engine.player.Loc.Y = Console.WindowHeight / 2 - 6;
                    return 2;
                case "King":
                    Engine.DrawTextBox("Why my head so gottdamn\nsmall", "King");
                    save.KingTalked = true;
                    break;
                default:
                    break;
            }
            Console.SetCursorPosition(0, 0);
            return 3;
        }

        public static void LevelFourObjects()
        {
            Engine.terrains.Add(new Terrain()
            {
                Name = "Void",
                Color = ConsoleColor.DarkCyan,
                TextColor = ConsoleColor.Black,
            });

            Engine.terrains.Add(new Terrain()
            {
                Name = "Bridge",
                Color = ConsoleColor.DarkGray,
                TextColor = ConsoleColor.Black,
                CornerUL = new Vector2(0, Console.WindowHeight / 3 + 5),
                CornerLR = new Vector2(10, Console.WindowHeight - 10),
                Interactible = true
            });

            string[] tempIcon = new string[]
            {
                @"╭----╮",
                @"|o o |",
                @"| ^ v|",
                @"╰----->",
            };
            Engine.obstacles.Add(new NPC("Ghost", Engine.width / 2 + 5, Engine.height / 2 + 3, tempIcon, 150, 15));

            Engine.obstacles.Add(new NPC("Ghost 2", Engine.width / 2 - 8, Engine.height / 2 - 3, tempIcon, 150, 15));
        }
        public static int LevelFourInteracts()
        {
            IInteract interact = Engine.GetInteract();
            if (interact == null)
            {
                return 4;
            }
            switch (interact.Name)
            {
                case "Bridge":
                    save.GuardBlock = false;
                    save.KingTalked = true;
                    save.WatchedLvl2Anim = true;
                    Engine.player.Loc.X = Engine.width - 15;
                    return 2;
            }
            if (interact.Name.Contains("Ghost"))
            {
                if (interact is NPC npc)
                {
                    int result = Engine.Battle(npc);
                    switch (result)
                    {
                        case 1:
                            Engine.RemoveObstacle(Engine.GetObstacle(interact.Name));
                            Engine.DrawTextBox("You Won!", "");
                            break;
                        case 0:
                            return 0;
                    }
                }
            }
            return 4;
        }

        /// <summary>
        /// Adds Starter Inventory On Project Startup
        /// </summary>
        private static void AddStarterInv()
        {
            Engine.player.AddItem(new BattleItem("Health Potion", "Health", 25, 5));

            Engine.player.AddItem(new BattleItem("Damage Potion", "Damage", 25, 3));

            Engine.player.AddItem(new BattleItem("Cool Potion", "Health", 10, 5));

            Engine.player.AddItem(new BattleItem("Evil Potion", "Health", -25, 2));

            Engine.player.AddItem(new BattleItem("Cool-Looking Pebble", "Damage", 1, 10));

            Engine.player.AddItem(new BattleItem("Cat President!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", "Damage", 50, 1));

            Engine.player.AddItem(new BattleItem("Weird Potion", "Damage", -5, 2));

            Engine.player.AddItem(new BattleItem("YuGiOh Card", "Damage", 5, 40));

            Engine.player.AddItem(new BattleItem("Pointy Stick", "Damage", 100, 5));

            Engine.player.AddItem(new Equippable("Programmer Socks", "Armor", 20));

            Engine.player.AddItem(new Equippable("Programmer Socks", "Armor", 20));

            Engine.player.AddItem(new Equippable("Head Empty", "Armor", 20));

            Engine.player.AddItem(new Equippable("Gauntlets Of Ech", "Armor", 20));

            Engine.player.AddItem(new Equippable("Long-Ass Sword", "Weapon", 20));

            Engine.player.AddItem(new Equippable("Long-Ass Sword", "Weapon", 20));

            Engine.player.AddItem(new Equippable("Short-Ass Sword", "Weapon", 20));

            Engine.player.AddItem(new Equippable("Medium-Ass Sword", "Weapon", 20));

            Engine.player.AddItem(new Equippable("Ass Sword", "Weapon", 20));

            Engine.player.AddItem(new Equippable("Boots Of Tiny Feet", "Armor", 20));

            Engine.player.AddItem(new Equippable("Meat Breastplate", "Armor", 20));

            Engine.player.AddItem(new Equippable("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", "Weapon", 20));

        }
    }
}