using ChessApp.game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private ChessGameController GameController { get; set; }
        public string CWAHost { get; private set; }

        public MainWindow()
        {
            CWAHost = "https://localhost:7263/";
            GameController = new ChessGameController();
            InitializeComponent();
            InitializeBoard();

            Image img = new Image();
            Thickness margin = img.Margin;
            margin.Left = margin.Top = margin.Right = margin.Bottom = 5;
            img.Margin = margin;
            Grid.SetColumn(img, 2);
            Grid.SetRow(img, 2);

            DrawingImage drawing = this.FindResource("pawn_blackDrawingImage") as DrawingImage;
            img.Source = drawing;
            board.Children.Add(img);
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

        public void LoginCallback(Guid uuid)
        {
            if(uuid != Guid.Empty)
            {
                GameController.Player.ReceivedPlayerUUID = uuid;
                menuAccountLogin.IsEnabled = false;
                menuAccountLogout.IsEnabled = true;
                menuGameFind.IsEnabled = true;
            }
        }


        private async void Logout(object sender, RoutedEventArgs e)
        {
            GameController.Player.ReceivedPlayerUUID = Guid.Empty;
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBox.Show($"TODO ws: localhost logout", "Logout", button, icon, MessageBoxResult.Yes);

            if (GameController.Player.ReceivedGameUUID != Guid.Empty)
            {
                GiveUp(null, null);
            }
            menuAccountLogin.IsEnabled = true;
            menuAccountLogout.IsEnabled = false;
            menuGameFind.IsEnabled = false;
        }

        private async void FindGame(object sender, RoutedEventArgs e)
        {
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBox.Show($"TODO ws: localhost player {GameController.Player.ReceivedPlayerUUID.ToString()} game get", "Find game", button, icon, MessageBoxResult.Yes);

            Guid uuid = Guid.NewGuid();
            if (uuid != Guid.Empty)
            {
                GameController.Player.ReceivedGameUUID = uuid;
                menuGameFind.IsEnabled = false;
                menuGameOpponent.IsEnabled = true;
                menuGameGiveUp.IsEnabled = true;
                menuGameLog.IsEnabled = true;
            }
        }

        private async void GiveUp(object sender, RoutedEventArgs e)
        {
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBox.Show($"TODO ws: localhost player {GameController.Player.ReceivedPlayerUUID.ToString()} game {GameController.Player.ReceivedGameUUID.ToString()} move abandon", "Find game", button, icon, MessageBoxResult.Yes);

            Guid uuid = Guid.NewGuid();
            if (uuid != Guid.Empty)
            {
                GameController.Player.ReceivedGameUUID = uuid;
                menuGameFind.IsEnabled = true;
                menuGameOpponent.IsEnabled = false;
                menuGameGiveUp.IsEnabled = false;
                menuGameLog.IsEnabled = false;
            }
        }

        private async void GameLog(object sender, RoutedEventArgs e)
        {
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBox.Show($"TODO ws: localhost player {GameController.Player.ReceivedPlayerUUID.ToString()} game {GameController.Player.ReceivedGameUUID.ToString()} log", "Find game", button, icon, MessageBoxResult.Yes);
        }

        private async void EnemyDetails(object sender, RoutedEventArgs e)
        {
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBox.Show($"TODO ws: localhost player {GameController.Player.ReceivedPlayerUUID.ToString()} game {GameController.Player.ReceivedGameUUID.ToString()} enemy", "Find game", button, icon, MessageBoxResult.Yes);
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
    }
}
