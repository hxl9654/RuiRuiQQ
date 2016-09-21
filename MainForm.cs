using Jurassic;
using Jurassic.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Windows.Forms;
// *   This program is free software: you can redistribute it and/or modify
// *   it under the terms of the GNU General Public License as published by
// *   the Free Software Foundation, either version 3 of the License, or
// *   (at your option) any later version.
// *
// *   This program is distributed in the hope that it will be useful,
// *   but WITHOUT ANY WARRANTY; without even the implied warranty of
// *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// *   GNU General Public License for more details.
// *
// *   You should have received a copy of the GNU General Public License
// *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// *
// * @author     Xianglong He
// * @copyright  Copyright (c) 2015 Xianglong He. (http://tec.hxlxz.com)
// * @license    http://www.gnu.org/licenses/     GPL v3
// * @version    1.0
// * @discribe   RuiRuiQQRobot服务端
// * 本软件作者是何相龙，使用GPL v3许可证进行授权。
namespace RuiRuiQQRobot
{

    public partial class MainForm : Form
    {
        //多个函数要用到的变量
        string pin = string.Empty;

        /// <summary>
        /// 更新主界面的QQ群列表
        /// </summary>
        internal void ReNewListBoxGroup()
        {
            listBoxGroup.Items.Clear();
            foreach (KeyValuePair<string, SmartQQ.GroupInfo> GroupList in SmartQQ.GroupList)
            {
                listBoxGroup.Items.Add(GroupList.Key + "::" + GroupList.Value.name);
            }
        }
        /// <summary>
        /// 更新主界面的QQ好友列表
        /// </summary>
        internal void ReNewListBoxFriend()
        {
            listBoxFriend.Items.Clear();
            foreach (KeyValuePair<string, SmartQQ.FriendInfo> FriendList in SmartQQ.FriendList)
            {
                listBoxFriend.Items.Add(FriendList.Key + ":" + SmartQQ.Info_RealQQ(FriendList.Key) + ":" + FriendList.Value.nick);
            }
        }
        /// <summary>
        /// 更新主界面的讨论组列表
        /// </summary>
        internal void ReNewListBoxDiscuss()
        {
            listBoxDiscuss.Items.Clear();
            foreach (KeyValuePair<string, SmartQQ.DiscussInfo> DiscussList in SmartQQ.DiscussList)
            {
                listBoxDiscuss.Items.Add(DiscussList.Key + ":" + DiscussList.Value.name);
            }
        }
        private void FormLogin_Load(object sender, EventArgs e)
        {
            bool NoFile = false;
            byte[] byData = new byte[100000];
            char[] charData = new char[100000];
            Control.CheckForIllegalCrossThreadCalls = false;
            System.Net.ServicePointManager.DefaultConnectionLimit = 500;
            Random rd = new Random();
            try
            {
                FileStream file = new FileStream(Environment.CurrentDirectory + "\\RuiRuiRobot.conf", FileMode.Open);
                file.Seek(0, SeekOrigin.Begin);
                file.Read(byData, 0, 1000);
                Decoder decoder = Encoding.Default.GetDecoder();
                decoder.GetChars(byData, 0, byData.Length, charData, 0);
                file.Close();
            }
            catch (System.IO.FileNotFoundException)
            {
                NoFile = true;
            }
            if (!NoFile)
            {
                string tmp = "";
                for (int i = 0; i < charData.Length; i++)
                    tmp += charData[i];
                tmp += '\0';
                JosnConfigFileModel dat = (JosnConfigFileModel)JsonConvert.DeserializeObject(tmp, typeof(JosnConfigFileModel));
                RuiRui.DicPassword = dat.DicPassword;
                RuiRui.DicServer = dat.DicServer;
                RuiRui.MasterQQ = dat.AdminQQ;
                GetInfo.YoudaoKey = dat.YoudaoKey;
                GetInfo.YoudaoKeyform = dat.YoudaoKeyfrom;
                GetInfo.TuLinKey = dat.TuLinKey;
            }
            if (RuiRui.DicServer == null || RuiRui.DicServer.Equals(""))
                RuiRui.DicServer = "https://ruiruiqq.hxlxz.com/";
            if (RuiRui.DicPassword == null || RuiRui.DicPassword.Equals(""))
                RuiRui.NoDicPassword = true;
            NoFile = false;
            try
            {
                FileStream file = new FileStream(Environment.CurrentDirectory + "\\badwords.txt", FileMode.Open);
                file.Seek(0, SeekOrigin.Begin);
                file.Read(byData, 0, 20000);
                Decoder decoder = Encoding.UTF8.GetDecoder();
                decoder.GetChars(byData, 0, byData.Length, charData, 0);
                file.Close();
            }
            catch (System.IO.FileNotFoundException)
            {
                NoFile = true;
            }
            if (!NoFile)
            {
                string tmp = "";
                for (int i = 0; i < charData.Length; i++)
                    if (charData[i] != '\0') tmp += charData[i];
                RuiRui.Badwords = tmp.Split('|');
            }
            else RuiRui.Badwords = new string[0];
            NoFile = false;
        }
        private void pictureBoxQRCode_Click(object sender, EventArgs e)
        {
            SmartQQ.Login_GetQRCode();
        }
        private void buttonFriendSend_Click(object sender, EventArgs e)
        {
            if (listBoxFriend.SelectedItem != null)
            {
                string[] tmp = listBoxFriend.SelectedItem.ToString().Split(':');
                string MessageToSend = textBoxFriendSend.Text;
                textBoxFriendSend.Clear();

                SmartQQ.Message_Send(0, tmp[0], MessageToSend, false);

                AddAndReNewTextBoxFriendChat(tmp[0], ("手动发送：" + Environment.NewLine + MessageToSend));
            }
        }

        private void buttonGroupSend_Click(object sender, EventArgs e)
        {
            if (listBoxGroup.SelectedItem != null)
            {
                string[] tmp = listBoxGroup.SelectedItem.ToString().Split(':');
                string MessageToSend = textBoxGroupSend.Text;
                textBoxGroupSend.Clear();

                SmartQQ.Message_Send(1, tmp[0], MessageToSend, false);

                AddAndReNewTextBoxGroupChat(tmp[0], ("手动发送：" + Environment.NewLine + MessageToSend));
            }
        }

        private void buttonDiscussSend_Click(object sender, EventArgs e)
        {
            if (listBoxDiscuss.SelectedItem != null)
            {
                string[] tmp = listBoxDiscuss.SelectedItem.ToString().Split(':');
                string MessageToSend = textBoxDiscussSend.Text;
                textBoxDiscussSend.Clear();

                SmartQQ.Message_Send(2, tmp[0], MessageToSend, false);

                AddAndReNewTextBoxDiscussChat(tmp[0], ("手动发送：" + Environment.NewLine + MessageToSend));
            }
        }

        internal void AddAndReNewTextBoxFriendChat(string uin, string str = "", bool ChangeCurrentUin = false)
        {
            SmartQQ.FriendList[uin].Messages += str + Environment.NewLine;
            if (ChangeCurrentUin || (listBoxFriend.SelectedItem != null && uin.Equals(listBoxFriend.SelectedItem.ToString().Split(':')[0])))
                textBoxFriendChat.Text = SmartQQ.FriendList[uin].Messages;
        }

        internal void AddAndReNewTextBoxGroupChat(string gid, string str = "", bool ChangeCurrentGid = false)
        {
            SmartQQ.GroupList[gid].Messages += str + Environment.NewLine;
            if (ChangeCurrentGid || (listBoxGroup.SelectedItem != null && gid.Equals(listBoxGroup.SelectedItem.ToString().Split(':')[0])))
                textBoxGroupChat.Text = SmartQQ.GroupList[gid].Messages;
        }

        internal void AddAndReNewTextBoxDiscussChat(string did, string str = "", bool ChangeCurrentDid = false)
        {
            SmartQQ.DiscussList[did].Messages += str + Environment.NewLine;
            if (ChangeCurrentDid || (listBoxDiscuss.SelectedItem != null && did.Equals(listBoxDiscuss.SelectedItem.ToString().Split(':')[0])))
                textBoxDiscussChat.Text = SmartQQ.DiscussList[did].Messages;
        }

        private void buttonLogIn_Click(object sender, EventArgs e)
        {
            SmartQQ.Login();
            HTTP.Get("https://ruiruiqq.hxlxz.com/infreport.php?qq=" + SmartQQ.QQNum + "&adminqq=" + RuiRui.MasterQQ);
        }

        private void listBoxLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxLog.Text = listBoxLog.Items[listBoxLog.SelectedIndex].ToString();
        }
        private void listBoxLog_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxLog.SelectedIndex != -1)
                Clipboard.SetDataObject(listBoxLog.Items[listBoxLog.SelectedIndex].ToString());
        }
        private void listBoxFriend_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxFriend.SelectedIndex != -1)
                Clipboard.SetDataObject(listBoxFriend.Items[listBoxFriend.SelectedIndex].ToString());
        }

        private void listBoxGroup_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxGroup.SelectedIndex != -1)
                Clipboard.SetDataObject(listBoxGroup.Items[listBoxGroup.SelectedIndex].ToString());
        }
        public MainForm()
        {
            InitializeComponent();
        }

        private void listBoxFriend_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] tmp = listBoxFriend.SelectedItem.ToString().Split(':');
            AddAndReNewTextBoxFriendChat(tmp[0], "", true);
        }

        private void listBoxGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] tmp = listBoxGroup.SelectedItem.ToString().Split(':');
            AddAndReNewTextBoxGroupChat(tmp[0], "", true);
        }

        private void listBoxDiscuss_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] tmp = listBoxDiscuss.SelectedItem.ToString().Split(':');
            AddAndReNewTextBoxDiscussChat(tmp[0], "", true);
        }
    }
    public class WindowObject : ObjectInstance
    {
        public WindowObject(ScriptEngine engine)
            : base(engine.Global)
        {
            this.PopulateFunctions();
        }
    }
}
