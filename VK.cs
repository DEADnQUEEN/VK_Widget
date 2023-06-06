using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;
using OpenQA.Selenium;
using System.Windows.Media;
using System.Windows;

namespace VK
{
    public class VKStruct
    {
        public bool Login { get; set; } = false;

        public readonly string URLLOG = "https://id.vk.com/auth?app_id=7913379&v=1.58.6&redirect_uri=https%3A%2F%2Fvk.com%2Flogin%3Fu%3D2%26to%3DL2FsX2ltLnBocA--&uuid=eCuCrSR3D4e3he6OZd5pu&scheme=space_gray&action=eyJuYW1lIjoicXJfYXV0aCJ9";
        public readonly string MessagesURL = "https://vk.com/im";
        public readonly string FeedURL = "https://vk.com/feed";
        public readonly string AfterLogURL = "https://vk.com/al_im.php";

        private readonly string DialogsLIKey = "#im_dialogs";
        private readonly string DNSet = "div.nim-dialog--content > div > div._im_dialog_title > div.nim-dialog--name > span.nim-dialog--name-w._im_dialog_name_w > span._im_dialog_link";
        private readonly string ValSet = "div.nim-dialog--content > div > div.nim-dialog--unread_container > div.nim-dialog--unread._im_dialog_unread_ct";
        private readonly string TagKey = "li"; 

        public ObservableCollection<string> Dialogs = new ObservableCollection<string>();
        public Dictionary<string, Label> DialogValuesDict = new Dictionary<string, Label>();
        public List<string> GridDialogs = new List<string>();
        public bool Cleaner { get; set; } = true;
        public async void DialogValsSetter(Dispatcher dsp, IWebDriver driver)
        {
            Thread t = new Thread(() =>
            {
                dsp.Invoke(() =>
                {
                    Dialogs.Clear();
                    DialogValuesDict.Clear();
                });

                while (Login)
                {
                    while (Login && !Cleaner)
                    {
                        ReadOnlyCollection<IWebElement> els = driver.FindElement(By.CssSelector(DialogsLIKey)).FindElements(By.TagName(TagKey));

                        for (int i = 0; i < els.Count; i++)
                        {
                            string txt = els[i].FindElement(By.CssSelector(DNSet)).Text;
                            string val = els[i].FindElement(By.CssSelector(ValSet)).Text;

                            dsp.Invoke(() =>
                            {
                                if (!Dialogs.Contains(txt) && !GridDialogs.Contains(txt))
                                {
                                    Dialogs.Add(txt);
                                    if (!DialogValuesDict.ContainsKey(txt))
                                        DialogValuesDict.Add(txt, new Label()
                                        {
                                            Foreground = new SolidColorBrush(Colors.White),
                                            Content = val,
                                            Height = 30,
                                            MinWidth = 30,
                                            Margin = new Thickness(5, 0, 5, 0),
                                            FontSize = 16
                                        });
                                }
                                DialogValuesDict[txt].Content = val;
                            });
                        }
                        int it = 0;
                        while (!Cleaner && it < 1000)
                        { 
                            Thread.Sleep(1);
                            it++;
                        }
                    }
                    while (Login && Cleaner)
                    {
                        dsp.Invoke(() =>
                        {
                            Dialogs.Clear();
                        });

                        ReadOnlyCollection<IWebElement> els = driver.FindElement(By.CssSelector(DialogsLIKey)).FindElements(By.TagName("li"));

                        for (int i = 0; i < els.Count; i++)
                        {
                            string txt = els[i].FindElement(By.CssSelector(DNSet)).Text;
                            string val = els[i].FindElement(By.CssSelector(ValSet)).Text;

                            dsp.Invoke(() =>
                            {
                                if (!Dialogs.Contains(txt) && !GridDialogs.Contains(txt))
                                {
                                    Dialogs.Add(txt);
                                    if (!DialogValuesDict.ContainsKey(txt))
                                        DialogValuesDict.Add(txt, new Label()
                                        {
                                            Foreground = new SolidColorBrush(Colors.White),
                                            Content = val,
                                            Height = 30,
                                            MinWidth = 30,
                                            Margin = new Thickness(5, 0, 5, 0),
                                            FontSize = 16
                                        });
                                }
                                DialogValuesDict[txt].Content = val;
                            });
                        }
                        Cleaner = false;
                    }
                }
            });
            t.Start();
            while (t.IsAlive) { await Task.Delay(1); }
        }
    }
}
