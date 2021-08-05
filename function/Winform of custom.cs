using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Theif
{
    
    public partial class MainForm : Form
    {
        bool flag;
        public MainForm()
        {
            InitializeComponent();
            notifyIcon1.Icon = Icon;
            ((Control)webBrowser1).MouseWheel += new MouseEventHandler(WebBrowser_MouseWheel);
            MouseWheel += new MouseEventHandler(WebBrowser_MouseWheel);
        }

        private void WebBrowser_MouseWheel(object sender, MouseEventArgs e)
        {

            if (e.Delta > 0)
            {
                comboBox1.Visible = true;
                button1.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
            }
            else
            {
                comboBox1.Visible = false;
                button1.Visible = false;
                button2.Visible = false;
                button3.Visible = false;
            }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)//当用户点击窗体右上角X按钮或(Alt + F4)时 发生          
            {
                e.Cancel = true;
                pause();
            }
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (flag)
            {
                resume();
            }
            else
            {
                pause();
            }
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void MainForm_Activated(object sender, EventArgs e)
        {  
            //注册热键Shift+S，Id号为100。HotKey.KeyModifiers.Shift也可以直接使用数字4来表示。  
            //HotKey.RegisterHotKey(Handle, 100, HotKey.KeyModifiers.Shift, Keys.S);
            //注册热键Ctrl+B，Id号为101。HotKey.KeyModifiers.Ctrl也可以直接使用数字2来表示。  
            HotKey.RegisterHotKey(Handle, 101, HotKey.KeyModifiers.Ctrl, Keys.Q);
            HotKey.RegisterHotKey(Handle, 105, HotKey.KeyModifiers.Ctrl, Keys.Left);
            //注册热键Alt+D，Id号为102。HotKey.KeyModifiers.Alt也可以直接使用数字1来表示。  
            //HotKey.RegisterHotKey(Handle, 102, HotKey.KeyModifiers.Alt, Keys.D);
            resume();
        }

        private void MainForm_Leave(object sender, EventArgs e)
        {
            //注销Id号为100的热键设定  
            //HotKey.UnregisterHotKey(Handle, 100);
            //注销Id号为101的热键设定  
            //HotKey.UnregisterHotKey(Handle, 101);
            //注销Id号为102的热键设定  
            //HotKey.UnregisterHotKey(Handle, 102);
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;//如果m.Msg的值为0x0312那么表示用户按下了热键
                                         // 按快捷键 
            const int WM_KEYDOWN = 0x0100;

            const int WM_NCHITTEST = 0x0084;
            const int HTLEFT = 0x000a;      //左边界
            const int HTRIGHT = 0x000b;     //右边界
            const int HTTOP = 0x000c;       //上边界
            const int HTTOPLEFT = 0x000d;   //左上角
            const int HTTOPRIGHT = 0x000e;  //右上角
            const int HTBOTTOM = 0x000f;    //下边界
            const int HTBOTTOMLEFT = 0x0010;    //左下角
            const int HTBOTTOMRIGHT = 0x0011;     //右下角
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    Point vPoint = new Point((int)m.LParam & 0xFFFF,
                        (int)m.LParam >> 16 & 0xFFFF);
                    vPoint = PointToClient(vPoint);
                    if (vPoint.X <= 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)HTTOPLEFT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)HTBOTTOMLEFT;
                        else m.Result = (IntPtr)HTLEFT;
                    else if (vPoint.X >= ClientSize.Width - 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)HTTOPRIGHT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)HTBOTTOMRIGHT;
                        else m.Result = (IntPtr)HTRIGHT;
                    else if (vPoint.Y <= 5)
                        m.Result = (IntPtr)HTTOP;
                    else if (vPoint.Y >= ClientSize.Height - 5)
                        m.Result = (IntPtr)HTBOTTOM;
                    break;
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:     // 按下的是Shift+S
                                      // 此处填写快捷键响应代码         
                            break;
                        case 101:     // 按下的是Ctrl+Q
                                      // 此处填写快捷键响应代码
                            if (flag)
                            {
                                resume();
                            }
                            else
                            {
                                pause();
                            }
                            break;
                        case 102:     // 按下的是Alt+D
                                      // 此处填写快捷键响应代码

                            break;
                        case 105:
                            if (flag)
                            {
                                resume();
                            }
                            else
                            {
                                pause();
                            }
                            break;

                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void resume()
        {
            flag = false;
            TopMost = true;
            Show();
            /* this call of this function is critical important */
            Activate();
        }

        private void pause()
        {
            flag = true;
            TopMost = false;
            Hide();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            webBrowser1.Url = new Uri(comboBox1.Text);
            webBrowser1.Refresh(); //刷新页面
            //HttpDownloadFile(comboBox1.Text, @"D:\IndividualFile\master.zip");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            pause();
        }

        private void webBrowser1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode != Keys.Up && e.KeyCode != Keys.Down
                && e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.C)
            {
                pause();
            }
        }

    }
    

}
