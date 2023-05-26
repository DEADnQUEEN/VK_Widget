using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OpenQA.Selenium;
using System.Windows.Controls;

namespace my_app_1
{
    public class VKStruct
    {
        public bool Login { get; set; } = false;
        public readonly string URLLOG = "https://id.vk.com/auth?app_id=7913379&v=1.58.6&redirect_uri=https%3A%2F%2Fvk.com%2Flogin%3Fu%3D2%26to%3DL2FsX2ltLnBocA--&uuid=eCuCrSR3D4e3he6OZd5pu&scheme=space_gray&action=eyJuYW1lIjoicXJfYXV0aCJ9";
        public readonly string MessagesURL = "https://vk.com/im";
        public readonly string FeedURL = "https://vk.com/feed";
        public readonly string AfterLogURL = "https://vk.com/al_im.php";
        public ObservableCollection<string> Dialogs = new ObservableCollection<string>();
        public List<string> AllDialogs = new List<string>();
        public ObservableCollection<string> Numbers = new ObservableCollection<string>();
        async public void DialogNamesSetter(ReadOnlyCollection<IWebElement> webElements)
        {
            if (AllDialogs.Count == 0)
            {
                for (int i = 1; i < webElements.Count; i++)
                {
                    string txt = webElements[i].FindElement(By.CssSelector("div.nim-dialog--content > div > div._im_dialog_title > div.nim-dialog--name > span.nim-dialog--name-w._im_dialog_name_w > span._im_dialog_link")).Text;
                    string val = webElements[i].FindElement(By.CssSelector("div.nim-dialog--content > div > div.nim-dialog--unread_container > div.nim-dialog--unread._im_dialog_unread_ct")).Text;

                    await Task.Delay(5);

                    if (txt == "") break;
                    if (val == "") val = "0";

                    AllDialogs.Add(txt);
                    Dialogs.Add(txt);
                    Numbers.Add(val);
                }
            }
        }
        async public void ValOfDialogsCheck(ReadOnlyCollection<IWebElement> webElements)
        {
            for (int i = 1; i < webElements.Count; i++)
            {
                await Task.Delay(5);

                string txt = webElements[i].FindElement(By.CssSelector("div.nim-dialog--content > div > div._im_dialog_title > div.nim-dialog--name > span.nim-dialog--name-w._im_dialog_name_w > span._im_dialog_link")).Text;
                await Task.Delay(5);
                string val = webElements[i].FindElement(By.CssSelector("div.nim-dialog--content > div > div.nim-dialog--unread_container > div.nim-dialog--unread._im_dialog_unread_ct")).Text;

                if (txt == "") break;
                if (val == "") val = "0";

                if (Numbers[AllDialogs.IndexOf(txt)] != val)
                {
                    Numbers[AllDialogs.IndexOf(txt)] = val;
                }
            }
        }
    }
}
