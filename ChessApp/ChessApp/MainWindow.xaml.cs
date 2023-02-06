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
                    if (selectedFigure != null)
                    {
                        WSMessageHandler.SendGameMoveMessage(ws, selectedFigure.Row, selectedFigure.Column, highlight.Item1, highlight.Item2);
                    }
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
            highlights.Clear();
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
                addFieldLabel(' ', 0, i + 1);
                addFieldLabel(' ', GameController.boardRowNames.Length + 1, i + 1);
            }

            for (int i = 0; i < GameController.boardColNames.Length; i++)
            {
                addFieldLabel(' ', i + 1, 0);
                addFieldLabel(' ', i + 1, GameController.boardColNames.Length + 1);
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
        }

        private async void FindGame(object sender, RoutedEventArgs e)
        {
            figures.Clear();
            RefreshBoard();
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

        

        private void SetOffline()
        {
            ws = null;

            menuAccountLogin.IsEnabled = true;
            menuAccountLogout.IsEnabled = false;

            menuGameFind.IsEnabled = false;
            menuGameOpponent.IsEnabled = false;
            menuGameGiveUp.IsEnabled = false;

            loginStatus.Text = "Login status: OFFLINE";
        }

        private void SetOnline(string login)
        {
            menuAccountLogin.IsEnabled = false;
            menuAccountLogout.IsEnabled = true;

            SetOffgame(login);
        }

        private void SetIngame(string login)
        {
            menuGameFind.IsEnabled = false;
            menuGameOpponent.IsEnabled = true;
            menuGameGiveUp.IsEnabled = true;

            loginStatus.Text = $"Login status: ONLINE AS {login} - INGAME";
        }

        private void SetOffgame(string login)
        {
            menuGameFind.IsEnabled = true;
            menuGameOpponent.IsEnabled = false;
            menuGameGiveUp.IsEnabled = false;

            loginStatus.Text = $"Login status: ONLINE AS {login}";
        }

        private void ApplicationExit(object sender, CancelEventArgs e)
        {
            if (ws != null)
            {
                string header = $"LO:Exit";
                ws.SendAsync(Encoding.ASCII.GetBytes(header), WebSocketMessageType.Text, true, CancellationToken.None);
                Thread.Sleep(500);
            }
        }

        private async Task Receive(string login, string password)
        {
            ws = new ClientWebSocket();

            var task = ws.ConnectAsync(new Uri($"wss://{CWAHost}/ws"), CancellationToken.None);
            if(await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                WSMessageHandler.SendUserLoginMessage(ws, login, password);

                while (true)
                {
                    try
                    {
                        var result = await WSMessageHandler.ReceiveAsync(ws);

                        if (result == "")
                        {
                            PopMessage("Connection Closed with no reason given.\nProbably closed by remote host.");
                            SetOffline();
                            return;
                        }

                        var exit = WSMessageHandler.HandleExit(result);
                        if (exit.Item1)
                        {
                            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                            PopMessage("Connection Closed:\n" + exit.Item2);
                            SetOffline();
                            return;
                        }

                        var gameChessboardData = WSMessageHandler.HandleGameChessboardData(result);
                        if (gameChessboardData.Item1)
                        {
                            figures.Clear();
                            figures = gameChessboardData.Item2;
                            RefreshBoard();
                            continue;
                        }

                        var gameMovesData = WSMessageHandler.HandleGameMovesData(result);
                        if (gameMovesData.Item1)
                        {
                            highlights.Clear();
                            highlights = gameMovesData.Item2;
                            RefreshHighlights();
                            continue;
                        }

                        var gameCustomMessage = WSMessageHandler.HandleGameCustomMessage(result);
                        if (gameCustomMessage.Item1)
                        {
                            PopMessage(gameCustomMessage.Item2);
                            continue;
                        }

                        var gameTurn = WSMessageHandler.HandleGameTurn(result);
                        if (gameTurn.Item1)
                        {
                            if (gameTurn.Item2)
                            {
                                turnStatus.Text = "Your turn";
                            }
                            else
                            {
                                turnStatus.Text = "Not your turn";
                            }
                        }

                        var gameStart = WSMessageHandler.HandleGameStartPosition(result);
                        if (gameStart.Item1)
                        {
                            if (gameStart.Item2)
                            {
                                WSMessageHandler.SwappedPos = true;
                                for (int i = 0; i < GameController.chessboardSize; i++)
                                {
                                    addFieldLabel(GameController.boardRowNames[7-i], 0, i + 1);
                                    addFieldLabel(GameController.boardRowNames[7-i], GameController.boardRowNames.Length + 1, i + 1);
                                }

                                for (int i = 0; i < GameController.boardColNames.Length; i++)
                                {
                                    addFieldLabel(GameController.boardColNames[7-i], i + 1, 0);
                                    addFieldLabel(GameController.boardColNames[7-i], i + 1, GameController.boardColNames.Length + 1);
                                }
                            }
                            else
                            {
                                WSMessageHandler.SwappedPos = false;
                                for (int i = 0; i < GameController.chessboardSize; i++)
                                {
                                    addFieldLabel(GameController.boardRowNames[i], 0, i + 1);
                                    addFieldLabel(GameController.boardRowNames[i], GameController.boardRowNames.Length + 1, i + 1);
                                }

                                for (int i = 0; i < GameController.boardColNames.Length; i++)
                                {
                                    addFieldLabel(GameController.boardColNames[i], i + 1, 0);
                                    addFieldLabel(GameController.boardColNames[i], i + 1, GameController.boardColNames.Length + 1);
                                }
                            }
                        }

                        var gameStatusMessage = WSMessageHandler.HandleGameStatusMessage(result);
                        if (gameStatusMessage.Item1)
                        {
                            if (gameStatusMessage.Item2)
                            {
                                SetIngame(login);
                                PopMessage(gameStatusMessage.Item3);
                            }
                            else
                            {
                                SetOffgame(login);
                                turnStatus.Text = "";
                                PopMessage(gameStatusMessage.Item3);
                            }
                            continue;
                        }

                        var serviceLoginOk = WSMessageHandler.HandleServiceLoginOk(result);
                        if (serviceLoginOk.Item1)
                        {
                            PopMessage("Successful Login!");
                            SetOnline(login);
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
            else
            {
                PopMessage("Connection Closed:\nServer not responding");
                SetOffline();
                return;
            }
        }
    }
}
