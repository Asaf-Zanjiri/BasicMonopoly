using System;

namespace Monopoly
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();

            // player stats and such
            string[] players = Init();
            string[] Ownership = new string[30];
            int[] Punishment = new int[players.Length];
            int[] PlayerPosition = new int[players.Length];

            //adds the start money for each player
            int[] bal = new int[players.Length];
            for (int i = 0; i < players.Length; i++)
                bal[i] = 1000;

            //inits the game board
            string[] GameBoard = new string[30];
            string[] assets = new string[30];
            int[] assetsPrices = new int[30];

            FillGameBoard(GameBoard, assets, assetsPrices);

            //gameplay
            PlayTurns(Punishment, players, Ownership, bal, rnd, PlayerPosition, GameBoard, assets, assetsPrices);

            //end
            GameOverScene(players, bal, Ownership, assets);
        }
        public static void PlayTurns(int[] Punishment, string[] players, string[] Ownership, int[] bal, Random rnd, int[] PlayerPosition, string[] GameBoard, string[] assets, int[] assetPrices)
        {
            ///This function is used to play a turn

            //resets varibles
            bool IsGameOver = false;
            int LoseCounter = 0;
            int CurrentPlayer = 0;
            int MoveCount = 0;

            while (!IsGameOver)
            {
                MoveCount = 0;

                //makes everything look nice
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write(" _-*");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(" Monopoly");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(" *-_");
                Console.WriteLine("__________________");
                Console.ForegroundColor = ConsoleColor.White;

                //Checks who's turn is it
                if (CurrentPlayer + 1 > players.Length)
                    CurrentPlayer = 1;
                else
                    CurrentPlayer++;

                //Checks if the player is able to play
                Console.Write($"This is ");
                WriteInColor(players[CurrentPlayer - 1], "Yellow", false);
                Console.Write("'s turn: ");
                if (Punishment[CurrentPlayer - 1] > 0)
                {
                    Console.WriteLine($"Unfortunately you are punished so you cant play for {Punishment[CurrentPlayer - 1]} turns.");
                    Console.WriteLine("Press 'ENTER' to continute");
                    Punishment[CurrentPlayer - 1]--;
                    Console.ReadKey();
                }
                else if (Punishment[CurrentPlayer - 1] == 0)
                {
                    //text info for the player
                    Console.Write("Press to");
                    WriteInColor(" 'ENTER'", "Yellow", false);
                    Console.WriteLine(" roll a die");
                    Console.ReadKey();
                    MoveCount = CubeToss(rnd);
                    PlayerPosition[CurrentPlayer - 1] = (PlayerPosition[CurrentPlayer - 1] + MoveCount) % 30;
                    Console.Write("You have rolled the number ");
                    WriteInColor("" + MoveCount, "Cyan", false);
                    Console.Write(" so now you're on cell ");
                    WriteInColor($"#{PlayerPosition[CurrentPlayer - 1]}", "Cyan", true);
                    Console.WriteLine();

                    //Checks what happens according to the player's position
                    switch (GameBoard[PlayerPosition[CurrentPlayer - 1]])
                    {
                        case "house":
                            LoseCounter = CheckOwnership(Ownership, players, CurrentPlayer, Punishment, bal, PlayerPosition, assets[PlayerPosition[CurrentPlayer - 1]], "House", assetPrices[PlayerPosition[CurrentPlayer - 1]], LoseCounter);
                            break;
                        case "factory":
                            LoseCounter = CheckOwnership(Ownership, players, CurrentPlayer, Punishment, bal, PlayerPosition, assets[PlayerPosition[CurrentPlayer - 1]], "Factory", assetPrices[PlayerPosition[CurrentPlayer - 1]], LoseCounter);
                            break;

                        case "highway":
                            Console.Write("Unfortunately you've been ");
                            WriteInColor("caught speeding :( ", "Red", false);
                            Console.WriteLine("so a fee of 50 ils has been deducted from you balance");
                            bal[CurrentPlayer - 1] -= 50;
                            LoseCounter = CheckBankrupcy(bal, Punishment, Ownership, players, LoseCounter, CurrentPlayer - 1);
                            break;

                        case "lottery":
                            Console.Write("wHOO whoo! Someone ");
                            WriteInColor("WON THE LOTTERY! ", "Green", false);
                            Console.WriteLine("so a total of 200 ils has been added to your balance");
                            bal[CurrentPlayer - 1] += 200;
                            break;

                        case "jail":
                            Console.Write("O'shit I feel sry 4 u cuz you landed on an ");
                            WriteInColor("A PRISON CELL! ", "Red", false);
                            Console.WriteLine("So now u're in jail for 2 turns hehe xd");
                            Punishment[CurrentPlayer - 1] += 2;
                            break;

                        case "fine":
                            Console.Write("It's not your lucky turn because you landed on an ");
                            WriteInColor("A FINE CELL! ", "Red", false);
                            Console.WriteLine("so a grand total of 200 ils has beed deducted from your balance");
                            bal[CurrentPlayer - 1] -= 200;
                            LoseCounter = CheckBankrupcy(bal, Punishment, Ownership, players, LoseCounter, CurrentPlayer - 1);
                            break;

                        case "skip5":
                            Console.Write("You got lucky and landed on ");
                            WriteInColor("JUMP 5 CELLS ", "Green", false);
                            Console.Write("And then you got semi-lucky again because You've safely landed on an ");
                            WriteInColor("EMPTY SPACE!", "Yellow", false);
                            PlayerPosition[CurrentPlayer - 1] += 5;
                            break;

                        default:
                            Console.Write("You've safely landed on an ");
                            WriteInColor("EMPTY SPACE!", "Yellow", true);
                            break;
                    }

                    //display stats
                    DisplayStats(players, bal, Ownership, assets);
                }

                //checking if the game is over
                if (LoseCounter > players.Length - 2)
                    IsGameOver = true;
            }
        }
        public static int CheckOwnership(string[] Ownership, string[] players, int CurrentPlayer, int[] Punishment, int[] bal, int[] PlayerPosition, string location, string place, int price, int LoseCounter)
        {
            ///Checks ownership for the place and applys things accordingly
            Console.Write("You reached a ");
            WriteInColor(place, "Yellow", false);
            Console.WriteLine($" in {location}");
            //checks if the place is avalible to purchase
            if (Ownership[PlayerPosition[CurrentPlayer - 1]] == null)
            {
                Console.Write($"a salesman seems to be trying to sell the {place}, would you like to purchase it for {price} ils? ");
                WriteInColor("(yes/no)", "Green", true);
                string input = "";
                while (input != "yes" && input != "no" && input != "hacks")
                    input = Console.ReadLine();
                Ownership[CurrentPlayer - 1] = input;
                switch (input)
                {
                    case "yes":
                        Console.WriteLine($"\n{"No take backs..."} said the salesman as he left, with YOUR {price} ils");
                        bal[CurrentPlayer - 1] -= price;
                        Ownership[PlayerPosition[CurrentPlayer - 1]] = players[CurrentPlayer - 1];
                        LoseCounter = CheckBankrupcy(bal, Punishment, Ownership, players, LoseCounter, CurrentPlayer - 1);
                        break;

                    case "no":
                        Console.WriteLine("\nThe salesman seemed disappoined by your reaction, and with that you left");
                        break;

                    case "hacks":
                        Console.WriteLine("\nwait 'hacks' was never an option wtf did u do");
                        bal[CurrentPlayer - 1] -= 10000;
                        LoseCounter = CheckBankrupcy(bal, Punishment, Ownership, players, LoseCounter, CurrentPlayer - 1);
                        break;
                }
            }
            else //if someone already owns the place
            {
                if (players[CurrentPlayer - 1] != Ownership[CurrentPlayer - 1])
                {
                    switch (place)
                    {
                        case "House":
                            Console.WriteLine($"This {place} is the property of {Ownership[PlayerPosition[CurrentPlayer - 1]]} and with no choice {Ownership[PlayerPosition[CurrentPlayer - 1]]} you have to stay and pay {(int)(price * 0.2)} ils");
                            //this makes it that if the player is going bankrupt then the other player would only get the money that the player paying has
                            if ((bal[CurrentPlayer - 1] - (price * 0.2)) <= 0)
                            {
                                bal[Array.IndexOf(players, Ownership[PlayerPosition[CurrentPlayer - 1]])] += bal[CurrentPlayer - 1];
                                bal[CurrentPlayer - 1] = 0;
                                LoseCounter = CheckBankrupcy(bal, Punishment, Ownership, players, LoseCounter, CurrentPlayer - 1);
                            }
                            else
                            {
                                bal[CurrentPlayer - 1] -= (int)(price * 0.2);
                                bal[Array.IndexOf(players, Ownership[PlayerPosition[CurrentPlayer - 1]])] += (int)(price * 0.2);
                                LoseCounter = CheckBankrupcy(bal, Punishment, Ownership, players, LoseCounter, CurrentPlayer - 1);
                            }
                            break;

                        case "Factory":
                            Console.WriteLine($"This {place} is the property of {Ownership[PlayerPosition[CurrentPlayer - 1]]} and with no choice {Ownership[PlayerPosition[CurrentPlayer - 1]]} you have to stay and work there for 1 turn and in return you'll get 50 ils");

                            bal[CurrentPlayer - 1] += 50;
                            Punishment[CurrentPlayer - 1] = 1;
                            break;

                    }
                }
                else
                    Console.WriteLine($"Oh its your {place}, nothing happens then");

            }

            return LoseCounter;
        }
        public static void DisplayStats(string[] players, int[] bal, string[] Ownership, string[] assets)
        {
            ///Displays the stats for each player (balance and estates)

            //display stats
            Console.WriteLine();
            string PrintEstate;
            //checks each player
            for (int i = 0; i < players.Length; i++)
            {
                //if the player is in the game or not
                if (bal[i] > 0)
                {
                    PrintEstate = "";
                    WriteInColor(players[i], "Cyan", false);
                    Console.Write($" has ");
                    if (bal[i] > 0)
                        WriteInColor("$" + bal[i], "Green", false);
                    Console.Write(" left, and owns assets in: ");

                    //checks what the player owns and ands it to the string PrintEstate, I used a string because I wanted to remove the last comma, knowing that it's on the last place I used string.remove(last char)
                    for (int k = 0; k < Ownership.Length; k++)
                        if (Ownership[k] == players[i])
                            PrintEstate += assets[k] + ",";

                    //checks no estate was found
                    if (PrintEstate != "")
                    {
                        PrintEstate = PrintEstate.Remove(PrintEstate.Length - 1);
                        WriteInColor(PrintEstate, "Green", true);
                    }
                    else
                        WriteInColor(" nothing yet", "Red", true);
                }
                else
                {
                    WriteInColor(players[i], "Cyan", false);
                    Console.Write($" has gone");
                    WriteInColor(" Bankrupt!", "Red", true);

                }

            }

            Console.Write("\nPress");
            WriteInColor(" 'ENTER'", "Cyan", false);
            Console.WriteLine(" to continue");
            Console.ReadKey();

        }
        public static void GameOverScene(string[] players, int[] bal, string[] Ownership, string[] assets)
        {
            ///The end scene

            //makes everything look nice
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(" _-*");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(" Monopoly");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(" *-_");
            Console.WriteLine("__________________");
            Console.ForegroundColor = ConsoleColor.White;
            WriteInColor("The game has ended and these are the stats:", "Red", true);
            DisplayStats(players, bal, Ownership, assets);

        }
        public static int CheckBankrupcy(int[] bal, int[] Punishment, string[] Ownership, string[] players, int LoseCounter, int CheckPlayer)
        {
            if (bal[CheckPlayer] <= 0)
            {
                //semi delete player
                LoseCounter += 1;
                Punishment[CheckPlayer] = -1;
                for (int k = 0; k < Ownership.Length; k++)
                    if (Ownership[k] == players[CheckPlayer])
                        Ownership[k] = null;
            }
            Console.WriteLine(LoseCounter);
            return LoseCounter;

        }
        public static void FillGameBoard(string[] GameBoard, string[] assets, int[] assetsPrices)
        {
            //init the board and the prices
            for (int i = 1; i <= GameBoard.Length; i++)
                switch (i)
                {
                    case 4:
                        GameBoard[i - 1] = "house";
                        assets[i - 1] = "Tel-Aviv centere";
                        assetsPrices[i - 1] = 250;
                        break;
                    case 5:
                        GameBoard[i - 1] = "house";
                        assets[i - 1] = "Tel-Aviv beach";
                        assetsPrices[i - 1] = 300;
                        break;
                    case 12:
                        GameBoard[i - 1] = "house";
                        assets[i - 1] = "Peta-Tikwa";
                        assetsPrices[i - 1] = 200;
                        break;
                    case 18:
                        GameBoard[i - 1] = "house";
                        assets[i - 1] = "Jerusalme";
                        assetsPrices[i - 1] = 200;
                        break;
                    case 19:
                        GameBoard[i - 1] = "house";
                        assets[i - 1] = "Haifa";
                        assetsPrices[i - 1] = 150;
                        break;
                    case 20:
                        GameBoard[i - 1] = "house";
                        assets[i - 1] = "Haifa2";
                        assetsPrices[i - 1] = 100;
                        break;
                    case 25:
                        GameBoard[i - 1] = "house";
                        assets[i - 1] = "Eilat";
                        assetsPrices[i - 1] = 200;
                        break;

                    case 9:
                        GameBoard[i - 1] = "highway";
                        break;

                    case 10:
                        GameBoard[i - 1] = "lottery";
                        break;

                    case 13:
                        GameBoard[i - 1] = "factory";
                        assets[i - 1] = "Peta-Tikwa";
                        assetsPrices[i - 1] = 250;
                        break;
                    case 26:
                        GameBoard[i - 1] = "factory";
                        assets[i - 1] = "Eilat";
                        assetsPrices[i - 1] = 300;
                        break;

                    case 16:
                        GameBoard[i - 1] = "jail";
                        break;

                    case 22:
                        GameBoard[i - 1] = "fine";
                        break;

                    case 28:
                        GameBoard[i - 1] = "skip5";
                        break;
                    default:
                        GameBoard[i - 1] = "null";
                        break;
                }
        }
        public static string[] Init()
        {
            //init the game
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Welcome to ");
            WriteInColor("MONOPOLY ", "Yellow", false);
            Console.Write("How many players are playing?");
            WriteInColor("(2-4)", "Yellow", true);

            //makes sure that the number of players is valid | this can work with more than 4 players if changed here
            string[] players = new string[ObtainValidInt(2, 4)];
            Console.Clear();
            Console.WriteLine("What is the name of the players?");

            //obtains the player names
            ObtainUniqueNames(players);

            return players;
        }
        public static void ObtainUniqueNames(string[] players)
        {
            ///obtains a unique name for each player

            bool DupeNames = true;
            for (int i = 0; i < players.Length; i++)
            {
                Console.Write($"\nPlayer ");
                WriteInColor($"#{i + 1}", "Yellow", false);
                Console.WriteLine(" what's your name?");
                players[i] = Console.ReadLine();
                DupeNames = true;
                if (i != 0)
                    while (DupeNames)
                    {
                        DupeNames = false;
                        for (int k = i - 1; k >= 0; k--)
                            if (players[i] == players[k])
                            {
                                Console.WriteLine("\nPlease enter a name that isnt in use already\n");
                                players[i] = Console.ReadLine();
                                DupeNames = true;
                                break;
                            }

                    }

            }
        }
        public static void WriteInColor(string str, string color, bool WriteLineFull)
        {

            ///This function writes in colors (text,color,writeline or write)
            switch (color)
            {
                case "Cyan":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case "Yellow":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case "Green":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case "Red":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

            }
            if (WriteLineFull)
                Console.WriteLine(str);
            else
                Console.Write(str);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static int ObtainValidInt(int min, int max)
        {
            //makes sure that the number of players is valid
            int input = int.Parse(Console.ReadLine());
            while (!(input >= min && input <= max))
            {
                Console.Write("Oh it seems like you've inputed an");
                WriteInColor(" INVALID ", "Red", false);
                Console.WriteLine("number, pls input again");
                input = int.Parse(Console.ReadLine());
            }
            return input;
        }
        public static int CubeToss(Random rnd)
        {
            ///return a random number between 1-6 to stimulate a cube toss
            return rnd.Next(1, 7);

        }

    }
}
//poorly made by Asaf Zanjiri :)
