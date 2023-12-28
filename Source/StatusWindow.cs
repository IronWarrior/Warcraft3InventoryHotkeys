using Warcraft3InventoryHotkeys.Source;

namespace Warcraft3InventoryHotkeys
{
    class StatusWindow : Form
    {
        public event Action OnMoved;

        public enum IndicatorStatus { Disabled, Idle, Polling }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x08000000 | 0x00000080;

                return cp;
            }
        }

        public StatusWindow(Point location)
        {
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(1, 1);
            Size = new Size(32, 32);

            StartPosition = FormStartPosition.Manual;
            Location = location;

            MouseDown += OnMouseDown;
            ResizeEnd += OnResizeEnd;
        }

        private void OnResizeEnd(object sender, EventArgs e)
        {
            OnMoved?.Invoke();
        }

        public void SetIndicator(IndicatorStatus status)
        {
            if (status == IndicatorStatus.Disabled)
                BackColor = Color.FromArgb(122, 122, 122);
            else if (status == IndicatorStatus.Idle)
                BackColor = Color.FromArgb(173, 49, 49);
            else if (status == IndicatorStatus.Polling)
                BackColor = Color.FromArgb(96, 156, 73);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
