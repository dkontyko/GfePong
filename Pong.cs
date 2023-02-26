using System.Drawing;
using System.Windows.Forms;

namespace GfePong {
    public class Program {
        public static void Main() {
            var mainWindow = new MainForm {
                //FormBorderStyle = FormBorderStyle.FixedSingle,
                Size = new Size(800, 400),
                Text = "GFE Pong",
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.Gray
            };

            mainWindow.createNewGame();




            Application.Run(mainWindow);
        }
    }

    partial class MainForm : Form {
        internal static readonly Brush BlackBrush = new SolidBrush(Color.Black);
        internal static readonly Brush BlueBrush = new SolidBrush(Color.Blue);
 
        private PongGame game;

        public MainForm() : base() {
            //leftBumperMovementTimer.Tick += LeftBumperMovementTimer_Tick;

            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);


        }

        internal void createNewGame() {
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
        private const int bumperWidth = 10;
        private const int bumperHeight = 50;
        private const int bumperPadding = 5;
        private const int ballDiameter = 10;
        /// <summary>
        /// Holds a reference to MainForm for calculating positions
        /// of objects and boundaries.
        /// </summary>
        private readonly Form gameForm;

        private PongObject leftBumper;
        private int leftBumperMovement = 0;

        private PongObject rightBumper;
        private int rightBumperMovement = 0;

        private PongBall ball;
        private int ballCourse = 0;
        private int ballSpeed = 0;

        internal PongGame(Form gameForm) {
            this.gameForm = gameForm;

            // Need to use ClientRectangle so the calculations
            // aren't thrown off by the title bar, borders, etc.
            leftBumper = new PongObject(
                bumperPadding + (bumperWidth / 2),
                gameForm.ClientRectangle.Height / 2,
                bumperWidth,
                bumperHeight
            );

            rightBumper = new PongObject(
                gameForm.ClientRectangle.Width - bumperPadding - (bumperWidth / 2),
                gameForm.ClientRectangle.Height / 2,
                bumperWidth,
                bumperHeight
            );

            ball = new PongBall(
                gameForm.ClientRectangle.Width / 2,
                gameForm.ClientRectangle.Height / 2,
                ballDiameter,
                ballDiameter
            );
        }

        internal void Paint(Graphics g) {
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.FillRectangle(MainForm.BlackBrush, leftBumper.Rect);
            g.FillRectangle(MainForm.BlueBrush, rightBumper.Rect);
            g.FillEllipse(MainForm.BlackBrush, ball.Rect);
        }
    }


    /// <summary>
    /// Wraps a Rectangle struct. The X and Y properties in this class
    /// represent the center of the rectangle. This properties automatically
    /// translate them to the correct values for the struct.
    /// </summary>
    class PongObject {


        private Rectangle rect;
        public Rectangle Rect { get => rect; }

        public int X {
            get => rect.X + (Width / 2);
            set => rect.X = value - (Width / 2);
        }
        public int Y {
            get => rect.Y + (Height / 2);
            set => rect.Y = value - (Height / 2);
        }

        public int Height { get; }
        public int Width { get; }

        internal PongObject(int x, int y, int width, int height) {
            Height = height;
            Width = width;
            rect = new Rectangle(0, 0, Width, Height);
            this.X = x;
            this.Y = y;
        }
    }

    class PongBall : PongObject {
        internal PongBall(int x, int y, int width, int height) : base(x, y, width, height) { }
    }

}
