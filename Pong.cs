using System.Drawing;
using System.Windows.Forms;

namespace GfePong {
    public class Program {
        public static void Main() {
            var mainWindow = new MainForm {
                Size = new Size(800, 400),
                Text = "GFE Pong",
                StartPosition = FormStartPosition.CenterScreen
            };

            mainWindow.ShowDialog();
        }
    }

    partial class MainForm : Form {

        private readonly Brush blackBrush = new SolidBrush(Color.Black);
        private Rectangle leftBumper = new Rectangle(5, 5, 10, 50);

        private int leftMovement = 0;

        private readonly Timer leftBumperMovementTimer = new Timer();

        public MainForm() : base() {
            leftBumperMovementTimer.Enabled = true;
            leftBumperMovementTimer.Interval = 50;
            leftBumperMovementTimer.Tick += LeftBumperMovementTimer_Tick;

            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

        }

        private void LeftBumperMovementTimer_Tick(object sender, System.EventArgs e) {
            leftBumper.Y += leftMovement;

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            e.Graphics.FillRectangle(blackBrush, leftBumper);
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Down) {
                leftMovement = 5;
            } else if (e.KeyCode == Keys.Up) {
                leftMovement = -5;
            }

            this.Invalidate();
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);

            if(e.KeyCode == Keys.Down || e.KeyCode == Keys.Up) {
                leftMovement = 0;
            }

            this.Invalidate();
        }
    }
}
