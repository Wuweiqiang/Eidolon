using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace WmWeiXin
{
    public partial class MainForm : Form
    {
        private Dictionary<string, TabPage> tps = new Dictionary<string, TabPage>();

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("Accounts.xml");
                XmlNodeList nodeList = xmlDoc.SelectNodes("/accounts/account");
                foreach (XmlNode node in nodeList)
                {
                    string acc = node.Attributes["acc"].Value;
                    string pass = node.Attributes["pass"].Value;
                    string caption = node.Attributes["caption"].Value;
                    lvTree.Nodes.Add(new TreeNode(caption) {Tag = acc + "|" + pass});
                }
                lvTree.DoubleClick += lvTree_DoubleClick;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lvTree_DoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs mea = e as MouseEventArgs;
            if (mea == null) return;
            TreeNode treeNode = lvTree.GetNodeAt(new Point(mea.X, mea.Y));
            if (treeNode == null) return;
            if (!tps.ContainsKey(treeNode.Text))
            {
                TabPage tabPage = new TabPage(treeNode.Text);
                tabPage.Text = treeNode.Text;
                tcMain.TabPages.Add(tabPage);
                WebBrowser webBrowser = new WebBrowser();
                tabPage.Controls.Add(webBrowser);
                webBrowser.Dock = DockStyle.Fill;
                webBrowser.DocumentCompleted += (s, ea) =>
                {
                    if (treeNode.Tag == null) return;
                    string[] ts = treeNode.Tag.ToString().Split('|');
                    WebBrowser browser = s as WebBrowser;
                    if (browser == null) return;
                    browser.Document.GetElementById("account").SetAttribute("Value", ts[0]); //= "weixin@lonntec.com";
                    browser.Document.GetElementById("pwd").SetAttribute("Value", ts[1]); // = "mipesoft.Com0";
                    HtmlElement btnSubmit = browser.Document.GetElementById("loginBt");
                    btnSubmit.InvokeMember("click");
                };
                webBrowser.Navigate(new Uri("https://mp.weixin.qq.com/"));
                tps.Add(treeNode.Text, tabPage);
            }

            tcMain.SelectedTab = tps[treeNode.Text];
        }

        void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser browser = sender as WebBrowser;
            if (browser == null) return;
            browser.Document.GetElementById("account").SetAttribute("Value", "weixin@lonntec.com"); //= "weixin@lonntec.com";
            browser.Document.GetElementById("pwd").SetAttribute("Value", "mipesoft.Com0");// = "mipesoft.Com0";
            HtmlElement formLogin = browser.Document.Forms["login_form"];
            HtmlElement btnSubmit = browser.Document.GetElementById("loginBt");
            btnSubmit.InvokeMember("click");
        }
    }
}
