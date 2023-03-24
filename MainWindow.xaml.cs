using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AngleSharp.Dom;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace my_app_1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public IWebDriver Driver_gen()
        {

            ChromeOptions options = new ChromeOptions();
            var chrome_service = ChromeDriverService.CreateDefaultService();
            chrome_service.HideCommandPromptWindow = true;
            IWebDriver driver = new ChromeDriver(options: options, service: chrome_service);

            return driver;
        }

        public MainWindow()
        {
            IWebDriver driver = Driver_gen();

            Method(driver);
            InitializeComponent();
        }

        public string Find_name_of_dialog(IWebElement element)
        {
            string out_txt = "";

            element = element.FindElement(By.CssSelector("div[class*='nim-dialog--content']"));
            element = element.FindElement(By.CssSelector("div[class*='nim-dialog--cw']"));
            element = element.FindElement(By.CssSelector("div[class*='_im_dialog_title']"));
            element = element.FindElement(By.CssSelector("div[class*='nim-dialog--name']"));
            element = element.FindElement(By.TagName("span"));
            element = element.FindElement(By.CssSelector("span[class*='_im_dialog_link']"));

            out_txt += element.Text;

            return out_txt;
        }

        public void Add_border(int i, int j, (int, int, int, int) rounder)
        {
            Border new_border = new Border
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Width = 10,
                Height = 10,
                Margin = new System.Windows.Thickness(i * 10 + 30, j * 10 + 30, 0, 0),
                CornerRadius = new System.Windows.CornerRadius(rounder.Item1, rounder.Item2, rounder.Item3, rounder.Item4),
                Background = System.Windows.Media.Brushes.Black
            };

            qr.Children.Add(new_border);
        }

        public static List<int> Transform_parse(string input_str)
        {
            List<int> doubles = new List<int>();
            string current = "";
            for (int i = 1; i < input_str.Length; i++)
            {
                if (char.IsLetter(input_str[i]) == false && char.IsDigit(input_str[i]))
                    current += input_str[i];
                else if (char.IsDigit(input_str[i - 1]))
                {
                    doubles.Add((Convert.ToInt32(string.Join("", current))));
                    current = "";
                }
            }

            if (current.Length != 0)
                doubles.Add((Convert.ToInt32(string.Join("", current))) / 97);

            return doubles;
        }

        public static void Filler(bool[,] matrix, int start_i, int start_j, int size)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    matrix[start_i + i, start_j + j] = true;
        }

        public static void Perimetr(bool[,] matrix, int start_i, int start_j, int size)
        {
            for (int i = 0; i < size; i++)
            {
                matrix[start_i + i, start_j] = true;
                matrix[start_i + i, start_j + size - 1] = true;
            }

            for (int j = 0; j < size; j++)
            {
                matrix[start_i, start_j + j] = true;
                matrix[start_i + size - 1, start_j + j] = true;
            }
        }

        public static void Create_rectangle_of_QR(ref bool[,] matrix, int i = 0, int j = 0)
        {
            Perimetr(matrix, start_i: i + 1, start_j: j + 1, size: 7);
            Filler(matrix, start_i: i + 3, start_j: i + 3, size: 3);

            Perimetr(matrix, start_i: matrix.GetLength(0) - 8, start_j: j + 1, size: 7);
            Filler(matrix, start_i: matrix.GetLength(0) - 6, start_j: j + 3, size: 3);

            Perimetr(matrix, start_i: i + 1, start_j: matrix.GetLength(1) - 8, size: 7);
            Filler(matrix, start_i: i + 3, start_j: matrix.GetLength(0) - 6, size: 3);
        }

        async public void Qr_code_out(bool[,] matrix)
        {
            Border brd_of_qr = new Border
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Margin = new System.Windows.Thickness(20),
                Width = 270,
                Height = 270,
                CornerRadius = new System.Windows.CornerRadius(10),
                Background = System.Windows.Media.Brushes.White
            };

            int rnd_num = 3;
            qr.Children.Add(brd_of_qr);

            for (int i = 1; i < matrix.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < matrix.GetLength(1) - 1; j++)
                {
                    await Task.Delay(1);
                    if (matrix[i, j])
                    {
                        (int, int, int, int) tuple_rnd = (rnd_num, rnd_num, rnd_num, rnd_num);

                        if (matrix[i, j - 1] || matrix[i - 1, j])   // Left n Top
                            tuple_rnd.Item1 = 0;
                        
                        if (matrix[i - 1, j] || matrix[i, j + 1])   // Top n right
                            tuple_rnd.Item4 = 0;
                        
                        if (matrix[i, j + 1] || matrix[i + 1, j])   // Right n Bottom
                            tuple_rnd.Item3 = 0;
                        
                        if (matrix[i + 1, j] || matrix[i, j - 1])   //Left n Bottom
                            tuple_rnd.Item2 = 0;

                        Add_border(i - 1, j - 1, tuple_rnd);
                    }
                }
            }
        }

        public void Qr_Code_Compiler(IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement g = wait.Until(e => e.FindElement(By.CssSelector("g[transform*='translate(0,0)']")));
            var inside_g = g.FindElements(By.TagName("g"));
            bool[,] matrix = new bool[27, 27];
            Create_rectangle_of_QR(ref matrix);
            string url = driver.Url.ToString();

            for (int i = 0; i < inside_g.Count; i++)
            {
                if (inside_g[i].FindElement(By.TagName("use")).GetAttribute("xlink:href") != "#n_rb-0")
                {
                    List<int> dbl_lst = Transform_parse(inside_g[i].GetAttribute("transform"));

                    int x = dbl_lst[0] + 1;
                    int y = dbl_lst[1] + 1;

                    matrix[x, y] = true;
                }
            }

            Qr_code_out(matrix);
        }

        async public void Method(IWebDriver driver)
        {

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            int max_len_of_dialogs = 10;
            await Task.Delay(50);
            try
            {
                driver.Navigate().GoToUrl("https://vk.com/im");
                string current_url = driver.Url.ToString();

                win.Visibility = System.Windows.Visibility.Visible;
                qr.Visibility = System.Windows.Visibility.Visible;

                var el = driver.FindElement(By.CssSelector("button[class*='FlatButton FlatButton--accent-outline FlatButton--size-l FlatButton--wide VkIdForm__button VkIdForm__signInQRButton']"));

                if (el != null)
                {
                    el.Click();

                    Qr_Code_Compiler(driver);

                    current_url = driver.Url.ToString();
                    while (driver.Url.ToString() == current_url)
                    {
                        await Task.Delay(10);
                    }

                    qr.Visibility = System.Windows.Visibility.Hidden;
                    win.UpdateLayout();
                    Thread.Sleep(100);
                }


                while (true)
                {
                    while (driver.Url == "https://vk.com/im" || driver.Url == "https://vk.com/al_im.php")
                    {
                        string out_txt = "";
                        List<string> list_of_string = new List<string>();
                        var dialog_list = driver.FindElements(By.CssSelector("ul[id*='im_dialogs'] li"));
                        for (int num_of_dialog = 0; num_of_dialog < max_len_of_dialogs && num_of_dialog < dialog_list.Count; num_of_dialog++)
                        {
                            list_of_string.Add(Find_name_of_dialog(dialog_list[num_of_dialog]));
                            await Task.Delay(100);
                        }

                        out_txt = string.Join("\n", list_of_string);

                        Lb1.Content = out_txt;

                        await Task.Delay(500);
                    }
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                for (int i = 0; i < qr.Children.Count; i++)
                    qr.Children.Clear();
                Lb1.Content = ex.Message;
                await Task.Delay(10000);
                driver.Quit();
                this.Close();

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "taskkill /IM \"chromedriver.exe\" /f";
                process.StartInfo = startInfo;
                process.Start();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();

            if (e.RightButton == MouseButtonState.Pressed)
            {
                this.Close();
                throw new Exception("хуяк");
            }
        }

        /// бан по причине пидорас
    }
}