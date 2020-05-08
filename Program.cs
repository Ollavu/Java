using System;
using System.Text;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.VisualBasic.CompilerServices;
using Org.BouncyCastle.Ocsp;
using System.Runtime.ConstrainedExecution;

namespace TicTacToe

{
    public interface IObserver
    {
        void Update(Game g);
        void Update(string status,int id,int game_id);
    }
    public class Observer : IObserver
    {
        public int id;
        public void Update(Game g)
        {
            string connStr = "server=localhost;user=root;database=tictactoe;password=0000;";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            if (g.id==0)
            {
                //проверка номера игры
                string request = "SELECT MAX(id) FROM game";
                MySqlCommand comman31 = new MySqlCommand(request, conn);
                Console.WriteLine(comman31.ExecuteScalar().ToString());
                //были ли игры до этого
                if(comman31.ExecuteScalar().ToString()=="")
                {
                    g.id = 1;
                    g.game_id = 1;
                }
                else
                {
                    g.id = Convert.ToInt32(comman31.ExecuteScalar().ToString())+1;
                    request = "SELECT MAX(id) FROM game_status_log";
                     comman31 = new MySqlCommand(request, conn);
                    Console.WriteLine(comman31.ExecuteScalar().ToString());
                    g.game_id = Convert.ToInt32(comman31.ExecuteScalar().ToString())+1;
                }
                //изменение статус
                string new_request = "INSERT INTO game_status_log(id,game_id,status) VALUES("+g.game_id+"," + g.id + ",'" + g.status + "')";
                MySqlCommand comman3 = new MySqlCommand(new_request, conn);
                comman3.ExecuteNonQuery();
            }
            else
            {
                g.game_id++;
                string request = "INSERT INTO game_status_log(id,game_id,status) VALUES(" + g.game_id + "," + g.id + ",'" + g.status + "')";
                MySqlCommand comman31 = new MySqlCommand(request, conn);
                comman31.ExecuteNonQuery();
            }
            conn.Close();
        }
        public void Update(string status,int id,int game_id)
        {
            string connStr = "server=localhost;user=root;database=tictactoe;password=0000;";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string request = "INSERT INTO game_status_log(id,game_id,status) VALUES(" + game_id + "," + id + ",'" + status + "')";
            MySqlCommand comman31 = new MySqlCommand(request, conn);
            comman31.ExecuteNonQuery();
        }
    }
    public interface IMove
    {
        void MakeMove(Board B, char symb);
    }
    public class CPlayer : IMove
    {
        public void MakeMove(Board b, char symb)
        {
            int xpos = 0, ypos = 0;
            do
            {
                Console.Clear();
                b.PrintBoard(b);
                Console.WriteLine("Ход " + symb);
                Console.Write("Введите координату x: ");
                try
                {
                    xpos = Int32.Parse(Console.ReadLine());
                }
                catch
                {
                    continue;
                }
                Console.Write("Введите координату y: ");
                try
                {
                    ypos = Int32.Parse(Console.ReadLine());
                }
                catch
                {
                    continue;
                }
                Console.Clear();
            }
            while ((xpos < 1 || xpos > b.size) || (ypos < 1 || ypos > b.size) || b.board[xpos - 1, ypos - 1] != ' ');

            Console.Clear();
            b.board[xpos - 1, ypos - 1] = symb;
            b.PrintBoard(b);
        }
    }//console Player
    public class BPlayer : IMove
    {
        public void MakeMove(Board b, char symb)
        {
            int xpos = 0, ypos = 0;
            Random rnd = new Random(); ;
            do
            {
                xpos = rnd.Next(1, b.size + 1);
                ypos = rnd.Next(1, b.size + 1);
            }
            while (b.board[xpos - 1, ypos - 1] != ' ');

            Console.Clear();
            b.board[xpos - 1, ypos - 1] = symb;
            b.PrintBoard(b);
        }
    }//Bot player
    public class Board
    {
        public char[,] board;
        public string Line;
        public int size;
        public int winlength;
        public int[] stats;
        public int[] playersstats;
        public Board()
        {
            winlength = 0;
            Line = string.Empty;
            size = 0;
            board = new char[1, 1];
            stats = new int[3] { 0, 0, 0 };
            playersstats = new int[3] { 0, 0, 0 };
        }
        public void GetBoard(Board b)
        {
            StringBuilder line = new StringBuilder();
            do
            {
                Console.Write("Input board size:");
                try
                {
                    b.size = int.Parse(Console.ReadLine());
                }
                catch
                {
                    continue;
                }
                Console.Clear();
            }
            while (b.size < 3);
            if (b.size > 3)
            {
                do
                {
                    Console.Write("Введите длину комбы для победы:");
                    try
                    {
                        b.winlength = int.Parse(Console.ReadLine());
                    }
                    catch
                    {
                        continue;
                    }
                    Console.Clear();
                }
                while (b.winlength < 3 || b.winlength > b.size);
            }
            else b.winlength = 3;
            b.board = new char[b.size, b.size];
            for (int i = 0; i < b.size; i++)
            {
                line.Append("___\t ");
                for (int j = 0; j < b.size; j++)
                {
                    b.board[i, j] = ' ';
                }
            }
            b.Line = line.ToString();
        }
        public void PrintBoard(Board b)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("\t\t\t\t");
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < b.size; i++)
            {
                Console.WriteLine("\t\t " + b.Line + "\n");
                for (int j = 0; j < b.size; j++)
                {
                    if (j == 0) Console.Write("\t\t");
                    Console.Write("| {0} |   ", b.board[i, j]);
                }
                Console.WriteLine("\n\t\t " + b.Line);
                Console.WriteLine();
            }

        }
        public bool СheckLanes(Board b, char symb, int offsetX, int offsetY)
        {
            bool cols, rows;
            for (int col = offsetX; col < b.winlength + offsetX; col++)
            {
                cols = true;
                rows = true;
                for (int row = offsetY; row < b.winlength + offsetY; row++)
                {
                    cols &= (b.board[col, row] == symb);
                    rows &= (b.board[row, col] == symb);
                }

                if (cols || rows) return true;
            }

            return false;
        }
        public bool CheckDiagonal(Board b, char symb, int offsetX, int offsetY)
        {
            bool toright, toleft;
            toright = true;
            toleft = true;
            for (int i = 0; i < b.winlength; i++)
            {
                toright &= (b.board[i + offsetX, i + offsetY] == symb);
                toleft &= (b.board[b.winlength - i - 1 + offsetX, i + offsetY] == symb);
            }

            if (toright || toleft) return true;

            return false;
        }
        public bool CheckWin(Board b, char symb)
        {
            for (int col = 0; col < b.size - b.winlength + 1; col++)
            {
                for (int row = 0; row < b.size - b.winlength + 1; row++)
                {
                    if (CheckDiagonal(b, symb, col, row) || СheckLanes(b, symb, col, row)) return true;
                }
            }
            return false;
        }
        public bool IsFull(Board b)
        {
            for (int i = 0; i < b.size; i++)
            {
                for (int j = 0; j < b.size; j++)
                {
                    if (b.board[i, j] == ' ')
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool CheckUnion(Board b, string player)
        {
            if (CheckWin(b, '0'))
            {
                Console.WriteLine("Нолики победили");
                if (player == "bot") b.stats[0]++;
                else b.playersstats[0]++;
                return true;
            }
            else if (CheckWin(b, 'X'))
            {
                Console.WriteLine("Крестики победили");
                if (player == "bot") b.stats[2]++;
                else b.playersstats[2]++;
                return true;
            }
            else if (IsFull(b))
            {
                Console.WriteLine("Боевая ничья!");
                if (player == "bot") b.stats[1]++;
                else b.playersstats[1]++;
                return true;
            }
            return false;
        }//все проверки объединены ( тут все они вызываются)
    }
    public class Menu
    {
        char choice;
        string player;
        Board b = new Board();
        IMove p1 = new CPlayer();
        Game g = new Game();
        public Menu(Board old_menu,int id)
        {
            for (int i = 0; i < 3; i++)
            {
                b.stats[i] = old_menu.stats[i];
                b.playersstats[i] = old_menu.playersstats[i];
            }
            int X_wins = b.playersstats[2] + b.stats[2];
            int draw_times = b.playersstats[1] + b.stats[1];
            int Zero_wins = b.playersstats[0] + b.stats[0];
            Save_game_stats(id, X_wins, Zero_wins, draw_times);
        }
        public Menu() { }
        public void Menuu()
        {
            g.status = "CREATED";
            
            foreach (IObserver o in g.observers)
            {
                o.Update(g);
            }
            string vybor;
            Console.Clear();
            Console.WriteLine("Кулити");
            Console.WriteLine("1.Игра с AI_Avenger-bot\n2.Игра с Player2-player\n3.Игровая статистика с ботом-stats\n4.Игровая статистика людей-playersstats\n5.Выход-exit");
            do
            {
                Console.Write("Ваш выбор: ");
                vybor = Console.ReadLine();
                Console.Clear();
            }
            while (vybor != "bot" && vybor != "player" && vybor != "stats" && vybor != "exit" && vybor != "playersstats");
            if (vybor == "bot")
            {
                IMove p2 = new BPlayer();
                player = "bot";
                b.GetBoard(b);
                choice = Choice(choice);
                g.status = "STARTED";
                foreach (IObserver o in g.observers)
                {
                    o.Update(g);
                }
                int id = g.id;
                int game_id = g.game_id;
                Save_Game_inf(choice, g, vybor);
                g.Play(b, p1, p2, choice, player, id,game_id);
            }
            else if (vybor == "player")
            {
                IMove p2 = new CPlayer();
                player = "player";
                b.GetBoard(b);
                choice = Choice(choice);
                g.status = "STARTED";
                foreach (IObserver o in g.observers)
                {
                    o.Update(g);
                }
                int id = g.id;
                int game_id = g.game_id;
                Save_Game_inf(choice, g, vybor);
                g.Play(b, p1, p2, choice, player, id, game_id);
            }
            else if (vybor == "stats")
            {
                Statictics(b);
            }
            else if (vybor == "playersstats")
            {
                PlayersStatictics(b);
            }
            else if (vybor == "exit")
            {
                return;
            }
        }
        public void Statictics(Board b)
        {
            Console.WriteLine("\tПобеда ноликов\tБоевая ничья\tПобеда крестиков");
            Console.WriteLine("\t  {0}         \t      {1}   \t        {2} ", b.stats[0], b.stats[1], b.stats[2]);
            Console.WriteLine("Нажатие клавиши возвратит Вас в меню");
            Console.ReadKey(true);
            Menuu();
        }
        public void PlayersStatictics(Board b)
        {
            Console.WriteLine("\tПобеда ноликов\tБоевая ничья\tПобеда крестиков");
            Console.WriteLine("\t  {0}         \t      {1}   \t        {2} ", b.playersstats[0], b.playersstats[1], b.playersstats[2]);
            Console.WriteLine("Нажатие клавиши возвратит Вас в меню");
            Console.ReadKey(true);
            Menuu();
        }
        public char Choice(char choice)
        {
            do
            {
                Console.Write("Введите за кого будет тыкать 1-й игрок : 0 или X: ");
                choice = Char.Parse(Console.ReadLine());
                Console.Clear();
            }
            while (choice != '0' && choice != 'X');
            return choice;
        }
        public void Save_Game_inf(char choice,Game g,string vybor)//vybor - тип игрока
        {
            string connStr = "server=localhost;user=root;database=tictactoe;password=0000;";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string PlayerX_name;
            string Player0_name;
            if (vybor=="player")//если противник человек, то разницы нет, кто где, и там и там хьюман
            {
                 PlayerX_name = vybor;
                 Player0_name = vybor;
            }
            else if(vybor=="bot" && choice=='X')
            {
                Player0_name = vybor;
                PlayerX_name = "player";
            }
            else
            {
                PlayerX_name = vybor;
                Player0_name = "player";
            }

            string request = "INSERT INTO game(id,PlayerX_name,Player0_name) VALUES(" + g.id + ",'" + PlayerX_name + "','"+ Player0_name + "')";
            MySqlCommand comman31 = new MySqlCommand(request, conn);
            comman31.ExecuteNonQuery();
            conn.Close();
        }
        public void Save_game_stats(int id,int X_wins,int Zero_wins,int draw_times)//сейв статы
        {
            string connStr = "server=localhost;user=root;database=tictactoe;password=0000;";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            
            string request = "INSERT INTO game_stats(game_id,PlayerX_win,Player0_win,Draw) VALUES(" + id + "," +X_wins + "," + Zero_wins + ","+draw_times+")";
            MySqlCommand comman31 = new MySqlCommand(request, conn);
            comman31.ExecuteNonQuery();
            conn.Close();
        }
    }
    public class Game
    {
        public string status;
        public List<IObserver> observers;
        public int id;//номер игры
        public int game_id;//Это для счётчика id в game_status_log
        public Game()
        {
            observers = new List<IObserver>();
            status = string.Empty;
            IObserver observer = new Observer();
            observers.Add(observer);
            id = 0;
            game_id = 0;
        }

        public Game Play(Board b, IMove p1, IMove p2, char choice, string player,int id,int game_id)
        {

            if (choice == 'X')
            {
                while (true)
                {

                    p1.MakeMove(b, 'X');
                    if (b.CheckUnion(b, player))
                    {
                        status = "FINISHED";
                        game_id++;
                        foreach (IObserver o in observers)
                        {
                            o.Update(status,id,game_id);
                        }
                        
                        Console.WriteLine("Нажатие клавиши возвратит Вас в меню");
                        Console.ReadKey(true);
                        new Menu(b,id).Menuu();
                        break;
                    }
                    p2.MakeMove(b, '0');
                    if (b.CheckUnion(b, player))
                    {
                        status = "FINISHED";
                        game_id++;
                        foreach (IObserver o in observers)
                        {
                            o.Update(status, id, game_id);
                        }
                       
                        Console.WriteLine("Нажатие клавиши возвратит Вас в меню");
                        Console.ReadKey(true);
                        new Menu(b,id).Menuu();
                        break;
                    }
                }
            }
            else
            {
                while (true)
                {
                    p1.MakeMove(b, '0');
                    if (b.CheckUnion(b, player))
                    {
                        status = "FINISHED";
                        game_id++;
                        foreach (IObserver o in observers)
                        {
                            o.Update(status, id, game_id);
                        }
                        
                        Console.WriteLine("Нажатие клавиши возвратит Вас в меню");
                        Console.ReadKey(true);
                        new Menu(b,id).Menuu();
                        break;
                    }
                    p2.MakeMove(b, 'X');
                    if (b.CheckUnion(b, player))
                    {
                        status = "FINISHED";
                        game_id++;
                        foreach (IObserver o in observers)
                        {
                            o.Update(status, id, game_id);
                        }
                        
                        Console.WriteLine("Нажатие клавиши возвратит Вас в меню");
                        Console.ReadKey(true);
                        new Menu(b,id).Menuu();
                        break;
                    }
                }
            }
            return new Game();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {       
            new Menu().Menuu();
            Console.ReadKey(true);
        }
    }
    
}