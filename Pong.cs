using System.CodeDom;
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
        internal static readonly Brush BlackBrush = new SolidBrush(Color.Black);
        //private Rectangle leftBumper = new Rectangle(5, 5, 10, 50);

        //private int leftBumperMovement = 0;
        /*
        private readonly Timer leftBumperMovementTimer = new Timer {
            Enabled = true,
            Interval = 50
        };
        */

        private PongGame game;

        public MainForm() : base() {
            //leftBumperMovementTimer.Tick += LeftBumperMovementTimer_Tick;

            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            game = new PongGame(this);

        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            game.Paint(e.Graphics);
        }
        /*
        private void LeftBumperMovementTimer_Tick(object sender, System.EventArgs e) {
            leftBumper.Y += leftBumperMovement;

            this.Invalidate();
        }

        

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);

            switch (e.KeyCode) {
                case Keys.Q:
                    leftBumperMovement = -5;
                    break;
                case Keys.A:
                    leftBumperMovement = 5;
                    break;
            }

            this.Invalidate();
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);

            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up) {
                leftBumperMovement = 0;
            }

            this.Invalidate();
        }
        */
    }

    class PongGame {
        /// <summary>
        /// Holds a reference to MainForm for calculating positions
        /// of objects and boundaries.
        /// </summary>
        private readonly Form gameForm;

        private PongBumper leftBumper;
        private int leftBumperMovement = 0;

        private PongBumper rightBumper;
        private int rightBumperMovement = 0;

        internal PongGame(Form gameForm) {
            this.gameForm = gameForm;

            leftBumper = new PongBumper(
                PongBumper.bumperPadding + (PongBumper.bumperWidth / 2),
                gameForm.Height / 2
            );

            rightBumper = new PongBumper(
                gameForm.Width - PongBumper.bumperPadding - (PongBumper.bumperWidth / 2),
                gameForm.Height / 2
            );
        }

        internal void Paint(Graphics g) {
            g.FillRectangle(MainForm.BlackBrush, leftBumper.Bumper);
            g.FillRectangle(MainForm.BlackBrush, rightBumper.Bumper);
        }
    }

    /// <summary>
    /// Wraps a Rectangle struct. The X and Y properties in this class
    /// represent the center of the rectangle. This properties automatically
    /// translate them to the correct values for the struct.
    /// </summary>
    class PongBumper {
        public const int bumperWidth = 10;
        public const int bumperHeight = 50;
        public const int bumperPadding = 5;

        private Rectangle bumper;
        public Rectangle Bumper { get => bumper; }

        public int X {
            get => bumper.X + (bumperWidth / 2);
            set => bumper.X = value - (bumperWidth / 2);
        }
        public int Y {
            get => bumper.Y + (bumperHeight / 2);
            set => bumper.Y = value - (bumperHeight / 2);
        }

        internal PongBumper(int x, int y) {
            bumper = new Rectangle(x, y, bumperWidth, bumperHeight);
        }
    }
}
