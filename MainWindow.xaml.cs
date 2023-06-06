using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using OpenQA.Selenium;
using OpenCvLogicWarp;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using Browse;
using VK;
using System.Threading;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;

namespace my_app_1
{
    public partial class MainWindow : Window
    {
        private readonly Warper WRP = new Warper();
        private readonly List<Button> Buttons;
        private readonly double TranceparentOpacity = 0.6;
        private readonly double Step = 0.03;
        
        private readonly VKStruct VK = new VKStruct();

        private readonly List<Grid> Grids = new List<Grid>();
        private readonly Browser Browser = new Browser();
        private Thread LogThread;

        private void CloseApp()
        {
            LogThread.Abort();
            Browser.Driver.Quit();
            Thread.Sleep(10);
            Close();
        }
        private void Cleaner(Grid grid, int colomnDefinitions, int rowDefinitions)
        {
            Cleaner(grid);
            int min = Math.Min(rowDefinitions, colomnDefinitions);
            for (int i = 0; i < min; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            }
            for (int i = 0; i < (colomnDefinitions - min); i++) grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            for (int i = 0; i < (rowDefinitions - min); i++) grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
        }
        private void Cleaner(Grid grid)
        {
            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
        }
        private void UISetter(ObservableCollection<string> dialogs, List<string> gridDial, bool clean)
        {
            Button adder = new Button()
            {
                Width = 30,
                Height = 30,
                Style = (Style)win.Resources["Adder"],
                Content = new Image()
                {
                    Source = new BitmapImage(new Uri("uiicons/plus_white.png", UriKind.Relative)),
                },
                Margin = new Thickness(5),
                Padding = new Thickness(3),
            };

            ComboBox comboBox = new ComboBox()
            {
                Style = (Style)win.Resources["ComboBoxDialogsStyle"],
                ItemContainerStyle = (Style)win.Resources["itemsStyle"],
                Width = 250,
                Height = adder.Height,
                ItemsSource = dialogs,
                MaxDropDownHeight = 400,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5),
            };

            Grid.SetColumn(adder, 0);
            Grid.SetRow(adder, int.MaxValue);
            MainView.Children.Add(adder);

            Grid.SetColumn(comboBox, 1);
            Grid.SetRow(comboBox, int.MaxValue);
            MainView.Children.Add(comboBox);

            adder.Click += (s, e) =>
            {
                if (comboBox.Text == "") return;

                Grid textGrid = new Grid()
                {
                    Name = 'g' + Grids.Count.ToString(),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition() { Width = new GridLength(30), },
                        new ColumnDefinition() { Width = GridLength.Auto, },
                        new ColumnDefinition() { Width = GridLength.Auto, }
                    },
                    RowDefinitions =
                    {
                        new RowDefinition() { Height = new GridLength(adder.Height) },
                    },
                    Margin = new Thickness(5),
                };

                Grids.Add(textGrid);

                Button delete = new Button()
                {
                    Name = 'b' + (Grids.Count - 1).ToString(),
                    Width = 30,
                    Height = 30,
                    Style = (Style)win.Resources["Adder"],
                    Content = new Image()
                    {
                        Source = new BitmapImage(new Uri("uiicons/plus45_white.png", UriKind.Relative)),
                    },
                };

                Label lbl = new Label()
                {
                    Name = 'l' + (Grids.Count - 1).ToString(),
                    Height = 30,
                    Content = comboBox.Text,
                    Width = comboBox.Width - 30,
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                };

                delete.Click += (ob, ev) =>
                {
                    int id = int.Parse(delete.Name.Substring(1));

                    MainView.Children.Remove(Grids[id]);
                    gridDial.Remove((string)((Label)Grids[id].Children[2]).Content);

                    Grids[id].Children.Clear();

                    Grids.RemoveAt(id);
                    MainView.RowDefinitions.RemoveAt(MainView.RowDefinitions.Count - 1);

                    for (int i = id; i < Grids.Count; i++)
                    {
                        foreach (FrameworkElement el in Grids[i].Children)
                        {
                            if (el.Name != string.Empty)
                                el.Name = el.Name[0] + i.ToString();
                        }
                        Grid.SetRow(Grids[i], i);
                    }
                    clean = true;
                };

                Grid.SetColumn(delete, 0);
                Grid.SetRow(delete, 0);
                textGrid.Children.Add(delete);

                Grid.SetRow(VK.DialogValuesDict[comboBox.Text], 0);
                Grid.SetColumn(VK.DialogValuesDict[comboBox.Text], 1);
                textGrid.Children.Add(VK.DialogValuesDict[comboBox.Text]);

                Grid.SetColumn(lbl, textGrid.ColumnDefinitions.Count - 1);
                Grid.SetRow(lbl, 0);
                textGrid.Children.Add(lbl);

                Grid.SetColumn(textGrid, 0);
                Grid.SetColumnSpan(textGrid, MainView.ColumnDefinitions.Count);
                Grid.SetRow(textGrid, MainView.RowDefinitions.Count - 1);

                MainView.RowDefinitions.Add(new RowDefinition());
                MainView.Children.Add(textGrid);
                gridDial.Add(comboBox.Text);
                dialogs.Remove(comboBox.Text);
            };

            MainView.ColumnDefinitions[0].Width = new GridLength(adder.Width + adder.Margin.Right + adder.Margin.Left);
            MainView.ColumnDefinitions[1].Width = new GridLength(comboBox.Width + comboBox.Margin.Left + comboBox.Margin.Right);
            MainView.RowDefinitions[0].Height = new GridLength(adder.Height + adder.Margin.Top + adder.Margin.Bottom);
        }
        private void VkUIset()
        {
            Cleaner(MainView, 2, 1);

            VK.DialogValsSetter(Dispatcher, Browser.Driver);
            UISetter(VK.Dialogs, VK.GridDialogs, VK.Cleaner);
        }
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Buttons = new List<Button>()
                {
                    pinButton,
                    closeButton,
                    minimizeButton,
                    homeButton,
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!win.Topmost && e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            VK.Login = false;
            CloseApp();
        }
        async private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            if (win.Topmost)
            {
                win.Topmost = false;
                PinButtonImg.Source = new BitmapImage(new Uri("uiicons/unlock_white.png", UriKind.Relative));
                for (double i = TranceparentOpacity; i < 1; i += Step / 5)
                    foreach (Button button in Buttons)
                    {
                        button.Opacity += Step / 5; 
                        await Task.Delay(1);
                    }
            }
            else 
            {
                win.Topmost = true;
                PinButtonImg.Source = new BitmapImage(new Uri("uiicons/lock_white.png", UriKind.Relative));
                for (double i = 1; i > TranceparentOpacity; i -= Step)
                    foreach (Button btn in Buttons)
                    {
                        btn.Opacity -= Step;
                        await Task.Delay(1);
                    }
            }
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            win.WindowState = WindowState.Minimized;
        }
        async private void VkClick(object sender, RoutedEventArgs e)
        {
            Thread t1 = new Thread(() =>
            {
                Browser.Driver.Navigate().GoToUrl(VK.URLLOG);
            });
            t1.Start();
            while (t1.IsAlive) { await Task.Delay(100); }

            if (!VK.Login)
            {
                byte[] ss = ((ITakesScreenshot)Browser.Driver).GetScreenshot().AsByteArray;

                Thread t2 = new Thread(() =>
                {
                    while (WRP.Detect(ss).Length == 0)
                    {
                        ss = ((ITakesScreenshot)Browser.Driver).GetScreenshot().AsByteArray;
                    }
                });
                t2.Start();
                while (t2.IsAlive) { await Task.Delay(100); }

                vk.Padding = new Thickness(10);
                vk.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffffff"));
                System.Drawing.Bitmap bit = WRP.QrFounder(ss);
                bit.MakeTransparent(System.Drawing.Color.White);
                vk.Style = (Style)win.Resources["VKupd"];
                vk.Content = new Image() { Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bit.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()) };

                LogThread = new Thread(() =>
                {
                    while (WRP.Detect(ss).Length == 4)
                    {
                        WRP.QrFounder(((ITakesScreenshot)Browser.Driver).GetScreenshot().AsByteArray);
                        bit.MakeTransparent(System.Drawing.Color.White);

                        Dispatcher.Invoke(() =>
                        {
                            vk.Content = new Image()
                            {
                                Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bit.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
                            };
                        });

                        Thread.Sleep(100);
                    }
                })
                { 
                    IsBackground = true
                };
                LogThread.Start();
                while (LogThread.IsAlive) { await Task.Delay(100); }
                LogThread.Abort();
            }
            VkUIset();
            VK.Login = true;

            return;
        }
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Cleaner(MainView, 3, 1);

            VK.Login = false;
            vk.Style = (Style)win.Resources["VK"];
            vk.Content = new Image() { Source = new BitmapImage(new Uri("uiicons/vkLogM.png", UriKind.Relative)), Name = "vkImg" };
            vk.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#26ffffff"));
            MainView.Children.Add(vk);
        }
    }
}