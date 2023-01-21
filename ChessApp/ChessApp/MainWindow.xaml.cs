using ChessApp.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessApp
{
    public partial class MainWindow : Window
    {
        private readonly SolidColorBrush oddColor = new SolidColorBrush(Color.FromArgb(0xFF, 0xF9, 0xE7, 0x9F));
        private readonly SolidColorBrush evenColor = new SolidColorBrush(Color.FromArgb(0xFF, 0x64, 0x1E, 0x16));
        private readonly SolidColorBrush highlightColor = new SolidColorBrush(Color.FromArgb(0xBF, 0xFF, 0x00, 0x00));

        private ChessGameController GameController { get; set; }
        public string CWAHost { get; private set; }
        private ClientWebSocket ws = null;
        private Task wslistener;

        private HashSet<ChessFigure> figures = new HashSet<ChessFigure>();
        private ChessFigure selectedFigure = null;
        private HashSet<Tuple<int, int>> highlights = new HashSet<Tuple<int, int>>();
        private int figureSize = 0;
        private int highlightSize = 0;
        private readonly int timeout = 1000;

        private async Task PopMessage(string message)
        {
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBox.Show(message, "Chess App", button, icon, MessageBoxResult.Yes);
        }

        private void RefreshHighlights()
        {
            for (int i = 0; i < highlightSize; i++)
            {
                Canvas child = board.FindName($"highlight{i}") as Canvas;
                if (child != null)
                {
                    board.Children.Remove(child);
                    UnregisterName($"highlight{i}");
                }
            }

            highlightSize = 0;
            foreach (Tuple<int, int> highlight in highlights)
            {
                Canvas canvas = new Canvas();
                canvas.Background = highlightColor;
                canvas.Name = $"highlight{highlightSize}";
                RegisterName(canvas.Name, canvas);
                canvas.MouseLeftButtonDown += new MouseButtonEventHandler(delegate (Object obj, MouseButtonEventArgs args)
                {
                    WSMessageHandler.SendGameMoveMessage(ws, selectedFigure.Row, selectedFigure.Column, highlight.Item1, highlight.Item2);
                });
                Grid.SetColumn(canvas, highlight.Item2 + 1);
                Grid.SetRow(canvas, highlight.Item1 + 1);
                board.Children.Add(canvas);
                highlightSize++;
            }

        }

        private void RefreshBoard()
        {
            selectedFigure = null;
            RefreshHighlights();

            for (int i = 0; i < figureSize; i++)
            {
                Image child = board.FindName($"figure{i}") as Image;
                if (child != null)
                {
                    board.Children.Remove(child);
                    UnregisterName($"figure{i}");
                }
            }

            figureSize = 0;
            foreach (ChessFigure chessFigure in figures)
            {
                Image img = new Image();
                img.Name = $"figure{figureSize}";
                RegisterName(img.Name, img);
                img.MouseLeftButtonDown += new MouseButtonEventHandler(delegate (Object obj, MouseButtonEventArgs args)
                {
                    selectedFigure = chessFigure;
                    WSMessageHandler.SendGameGetMoveMessage(ws, chessFigure.Row, chessFigure.Column);
                });

                Thickness margin = img.Margin;
                margin.Left = margin.Top = margin.Right = margin.Bottom = 1;
                img.Margin = margin;
                Grid.SetColumn(img, chessFigure.Column + 1);
                Grid.SetRow(img, chessFigure.Row + 1);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                string local = Directory.GetCurrentDirectory();
                local = local.Replace("\\", "/");
                bitmap.UriSource = new Uri($"file://{local}/resources/{chessFigure.ImageSource}");
                bitmap.EndInit();
                img.Source = bitmap;
                board.Children.Add(img);
                figureSize++;
            }
        }

        public MainWindow()
        {
            CWAHost = "localhost:7263";
            GameController = new ChessGameController();
            InitializeComponent();
            InitializeBoard();
            loginStatus.Text = "Login status: OFFLINE";

            /*
                        string figname = ChessPiecesEnumTranslator.TrasnslateShortToImage((short)2);
                        figures.Add(new ChessFigure(figname, 0, 0));
                        figures.Add(new ChessFigure(figname, 0, 2));
                        figname = ChessPiecesEnumTranslator.TrasnslateShortToImage((short)-5);
                        highlights.Add(new Tuple<int, int>(5, 5));
                        highlights.Add(new Tuple<int, int>(6, 6));
                        figures.Add(new ChessFigure(figname, 1, 1));

                        RefreshBoard();
                        RefreshHighlights();

                        highlights.Clear();
                        figures.Clear();

                        figures.Add(new ChessFigure(figname, 1, 3));
                        highlights.Add(new Tuple<int, int>(3, 3));
                        highlights.Add(new Tuple<int, int>(2, 3));

                        RefreshBoard();
                        RefreshHighlights();
            */
        }

        private void addFieldLabel(char label, int col, int row) {
            Label labComp = new Label();
            labComp.HorizontalContentAlignment = HorizontalAlignment.Center;
            labComp.VerticalContentAlignment = VerticalAlignment.Center;
            labComp.Content = label.ToString();
            labComp.Background = new SolidColorBrush(Colors.Black);
            labComp.Foreground = new SolidColorBrush(Colors.White);
            labComp.HorizontalAlignment = HorizontalAlignment.Stretch;
            labComp.VerticalAlignment = VerticalAlignment.Stretch;
            Grid.SetColumn(labComp, col);
            Grid.SetRow(labComp, row);
            board.Children.Add(labComp);
        }

        private void InitializeBoard()
        {
            Canvas canvas;
            bool odd = false;
            for (int i = 0; i < GameController.chessboardSize; i++)
            {
                for(int j = 0; j < GameController.chessboardSize; j++)
                {
                    canvas = new Canvas();
                    canvas.Background = odd ? oddColor : evenColor;
                    Grid.SetColumn(canvas, j + 1);
                    Grid.SetRow(canvas, i + 1);
                    board.Children.Add(canvas);

                    odd = !odd;
                }
                odd = !odd;
            }

            for (int i = 0; i < GameController.chessboardSize; i++)
            {
                addFieldLabel(GameController.boardRowNames[i], 0, i+1);
                addFieldLabel(GameController.boardRowNames[i], GameController.boardRowNames.Length+1, i+1);
            }

            for (int i = 0; i < GameController.boardColNames.Length; i++)
            {
                addFieldLabel(GameController.boardColNames[i], i+1, 0);
                addFieldLabel(GameController.boardColNames[i], i+1, GameController.boardColNames.Length + 1);
            }

            addFieldLabel(' ', 0, 0);
            addFieldLabel(' ', GameController.boardRowNames.Length + 1, 0);
            addFieldLabel(' ', GameController.boardRowNames.Length + 1, GameController.boardColNames.Length + 1);
            addFieldLabel(' ', 0, GameController.boardColNames.Length + 1);
        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            WindowLogin win = new WindowLogin(this);
            win.ShowDialog();
        }

        public async Task LoginCallback(string login, string password)
        {
            menuAccountLogin.IsEnabled = false;
            loginStatus.Text = "Login status: CONNECTING";
            wslistener = Receive(login, password);
        }


        private void Logout(object sender, RoutedEventArgs e)
        {
            WSMessageHandler.SendUserLogoutMessage(ws, "Logout");

/*            if (GameController.Player.ReceivedGameUUID != Guid.Empty)
            {
                GiveUp(null, null);
            }*/
        }

        private async void FindGame(object sender, RoutedEventArgs e)
        {
            menuGameFind.IsEnabled = false;
            WSMessageHandler.SendUserFindGameMessage(ws);
        }

        private async void GiveUp(object sender, RoutedEventArgs e)
        {
            WSMessageHandler.SendGameGiveUpMessage(ws);
        }

        private async void EnemyDetails(object sender, RoutedEventArgs e)
        {
            WSMessageHandler.SendGameOppDetMessage(ws);
        }

        private void EditHost(object sender, RoutedEventArgs e)
        {
            WindowEditHost win = new WindowEditHost(this);
            win.Show();
        }

        public void EditHostCallback(string host)
        {
            CWAHost = host;
        }

        

        private void setOffline()
        {
            ws = null;

            menuAccountLogin.IsEnabled = true;
            menuAccountLogout.IsEnabled = false;
            menuGameFind.IsEnabled = false;

            menuGameOpponent.IsEnabled = false;
            menuGameGiveUp.IsEnabled = false;

            loginStatus.Text = "Login status: OFFLINE";
        }

        private void setOnline()
        {
            menuAccountLogin.IsEnabled = false;
            menuAccountLogout.IsEnabled = true;
            menuGameFind.IsEnabled = true;

            loginStatus.Text = "Login status: ONLINE";
        }

        private void setIngame()
        {
            menuGameOpponent.IsEnabled = true;
            menuGameGiveUp.IsEnabled = true;
            menuGameFind.IsEnabled = false;

            loginStatus.Text = "Login status: ONLINE - INGAME";
        }

        private void setOffgame()
        {
            menuGameOpponent.IsEnabled = false;
            menuGameGiveUp.IsEnabled = false;
            menuGameFind.IsEnabled = true;

            loginStatus.Text = "Login status: ONLINE";
        }

        private async void ApplicationExit(object sender, CancelEventArgs e)
        {
            if (ws != null)
            {
                WSMessageHandler.SendUserLogoutMessage(ws, "Exit");
                await Task.Delay(timeout);
            }
        }

        private async Task Receive(string login, string password)
        {
            ws = new ClientWebSocket();

            var task = ws.ConnectAsync(new Uri($"wss://{CWAHost}/ws"), CancellationToken.None);
            if(await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {

            } 
            else
            {
                PopMessage("Connection Closed:\nServer not responding");
                setOffline();
                return;
            }

            WSMessageHandler.SendUserLoginMessage(ws, login, password);

            while (true)
            {
                try
                {
                    var result = await WSMessageHandler.ReceiveAsync(ws);

                    Tuple<bool, string> exit = WSMessageHandler.HandleExit(result);
                    if (exit.Item1)
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                        PopMessage("Connection Closed:\n" + exit.Item2);
                        setOffline();
                        return;
                    }

                    Tuple<bool, HashSet<ChessFigure>> gameChessboardData = WSMessageHandler.HandleGameChessboardData(result);
                    if (gameChessboardData.Item1)
                    {
                        figures.Clear();
                        figures = gameChessboardData.Item2;
                        RefreshBoard();
                        continue;
                    }

                    Tuple<bool, HashSet<Tuple<int, int>>> gameMovesData = WSMessageHandler.HandleGameMovesData(result);
                    if (gameMovesData.Item1)
                    {
                        highlights.Clear();
                        highlights = gameMovesData.Item2;
                        RefreshHighlights();
                        continue;
                    }

                    Tuple<bool, string> gameCustomMessage = WSMessageHandler.HandleGameCustomMessage(result);
                    if (gameCustomMessage.Item1)
                    {
                        PopMessage(gameCustomMessage.Item2);
                        continue;
                    }

                    Tuple<bool> serviceLoginOk = WSMessageHandler.HandleServiceLoginOk(result);
                    if (serviceLoginOk.Item1)
                    {
                        PopMessage("Successful Login!");
                        setOnline();
                        continue;
                    }
                }
                catch (Exception e)
                {
                    PopMessage("Connection Closed:\nException:" + e.ToString());
                    return;
                }
            }
        }
    }
}
