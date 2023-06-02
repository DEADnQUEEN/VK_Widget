using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace VK
{
    public class VKStruct
    {
        public bool Login { get; set; } = false;

        public readonly string URLLOG = "https://id.vk.com/auth?app_id=7913379&v=1.58.6&redirect_uri=https%3A%2F%2Fvk.com%2Flogin%3Fu%3D2%26to%3DL2FsX2ltLnBocA--&uuid=eCuCrSR3D4e3he6OZd5pu&scheme=space_gray&action=eyJuYW1lIjoicXJfYXV0aCJ9";
        public readonly string MessagesURL = "https://vk.com/im";
        public readonly string FeedURL = "https://vk.com/feed";
        public readonly string AfterLogURL = "https://vk.com/al_im.php";

        public readonly string DialogsLIKey = "#im_dialogs";
        public readonly string DNSet = "div.nim-dialog--content > div > div._im_dialog_title > div.nim-dialog--name > span.nim-dialog--name-w._im_dialog_name_w > span._im_dialog_link";
        public readonly string ValSet = "div.nim-dialog--content > div > div.nim-dialog--unread_container > div.nim-dialog--unread._im_dialog_unread_ct";

        public ObservableCollection<string> Dialogs = new ObservableCollection<string>();
        public Dictionary<string, Label> DialogValuesDict = new Dictionary<string, Label>();
        public ObservableCollection<string> GridDialogs = new ObservableCollection<string>();
    }
}
