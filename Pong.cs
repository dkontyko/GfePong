using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Windows.Forms;
using System.Timers;
using System;

namespace GfePong {
    public class Program {
        public static void Main() {
            var mainWindow = new MainForm {
                FormBorderStyle = FormBorderStyle.FixedSingle,
                Size = new Size(800, 400),
                Text = "GFE Pong",
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.Gray,
                Margin = Padding.Empty
            };

            mainWindow.CreateNewGame();

            Application.Run(mainWindow);
        }
    }

    partial class MainForm : Form {
        internal static readonly Brush BlackBrush = new SolidBrush(Color.Black);
        internal static readonly Brush BlueBrush = new SolidBrush(Color.Blue);

        private PongGame game;

        private readonly System.Timers.Timer movementTimer = new System.Timers.Timer {
            Interval = 50,
            AutoReset = true,
            Enabled = true
        };

        public MainForm() : base() {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        internal void CreateNewGame() {
            game = new PongGame(this);

            movementTimer.Elapsed += MovementTimerEventHandler;
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            game.Paint(e.Graphics);
        }

        private void MovementTimerEventHandler(object source, ElapsedEventArgs e) {
            game.UpdateFrame();
            this.Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Q:
                    game.UpdateBumperVector(PongGame.BumperSide.Left, PongGame.BumperDirection.Up);
                    break;
                case Keys.A:
                    game.UpdateBumperVector(PongGame.BumperSide.Left, PongGame.BumperDirection.Down);
                    break;
                case Keys.P:
                    game.UpdateBumperVector(PongGame.BumperSide.Right, PongGame.BumperDirection.Up);
                    break;
                case Keys.L:
                    game.UpdateBumperVector(PongGame.BumperSide.Right, PongGame.BumperDirection.Down);
                    break;
            }

            this.Invalidate();
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            //TODO: fix race condition that happens when rapidly switching
            // between up and down keys.
            if (e.KeyCode == Keys.Q || e.KeyCode == Keys.A) {
                game.UpdateBumperVector(PongGame.BumperSide.Left, PongGame.BumperDirection.None);
            } else if (e.KeyCode == Keys.P || e.KeyCode == Keys.L) {
                game.UpdateBumperVector(PongGame.BumperSide.Right, PongGame.BumperDirection.None);
            }

            this.Invalidate();
        }
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
        private readonly PongObject leftBumper;
        private readonly PongObject rightBumper;
        private readonly PongBall ball;

        internal PongGame(Form gameForm) {
            this.gameForm = gameForm;

            var bumperBoundaries = new Boundaries {
                Xmin = 0,
                Xmax = gameForm.ClientRectangle.Width,
                Ymin = 0,
                Ymax = gameForm.ClientRectangle.Height
            };

            var ballBoundaries = new Boundaries {
                Xmin = -ballDiameter,
                Xmax = gameForm.ClientRectangle.Width + ballDiameter,
                Ymin = (ballDiameter / 2),
                Ymax = gameForm.ClientRectangle.Height - (ballDiameter / 2)
            };

            // Need to use ClientRectangle so the calculations
            // aren't thrown off by the title bar, borders, etc.
            leftBumper = new PongObject(
                bumperPadding + (bumperWidth / 2),
                gameForm.ClientRectangle.Height / 2,
                bumperWidth,
                bumperHeight,
                bumperBoundaries
            );

            rightBumper = new PongObject(
                gameForm.ClientRectangle.Width - bumperPadding - (bumperWidth / 2),
                gameForm.ClientRectangle.Height / 2,
                bumperWidth,
                bumperHeight,
                bumperBoundaries
            );

            ball = new PongBall(
                gameForm.ClientRectangle.Width / 2,
                gameForm.ClientRectangle.Height / 2,
                ballDiameter,
                ballDiameter,
                ballBoundaries
            );

            var random = new Random();

            ball.XVelocity = random.Next(-5, 6);
            ball.YVelocity = random.Next(-5, 6);
        }

        internal void Paint(Graphics g) {
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.FillRectangle(MainForm.BlackBrush, leftBumper.Rect);
            g.FillRectangle(MainForm.BlueBrush, rightBumper.Rect);
            g.FillEllipse(MainForm.BlackBrush, ball.Rect);
        }

        internal void UpdateBumperVector(BumperSide side, BumperDirection direction) {
            var bumper = side == BumperSide.Left ? leftBumper : rightBumper;
            switch (direction) {
                case BumperDirection.Up:
                    bumper.YVelocity = -5;
                    break;
                case BumperDirection.Down:
                    bumper.YVelocity = 5;
                    break;
                case BumperDirection.None:
                    bumper.YVelocity = 0;
                    break;
            }
        }

        /// <summary>
        /// Updates the game's GUI frame (like advancing frames
        /// of a movie).
        /// </summary>
        internal void UpdateFrame() {
            leftBumper.MoveObject();
            rightBumper.MoveObject();
            ball.MoveObject();

            CheckCollision();
        }

        private void CheckCollision() {
            if (ball.Rect.IntersectsWith(leftBumper.Rect) || ball.Rect.IntersectsWith(rightBumper.Rect)) {
                ball.XVelocity = -ball.XVelocity;
            }
        }

        internal enum BumperSide { Left, Right }
        internal enum BumperDirection { Up, Down, None }

    }


    /// <summary>
    /// Wraps a Rectangle struct. The X and Y properties in this class
    /// represent the center of the rectangle. This properties automatically
    /// translate them to the correct values for the struct.
    /// </summary>
    class PongObject {
        protected readonly Boundaries boundaries;

        protected Rectangle _rect;
        public Rectangle Rect { get => _rect; }

        public int X {
            get => _rect.X + (Width / 2);
            set {
                if (value >= boundaries.Xmin && value <= boundaries.Xmax) {
                    _rect.X = value - (Width / 2);
                }

            }
        }
        public int Y {
            get => _rect.Y + (Height / 2);
            set {
                if (value >= boundaries.Ymin && value <= boundaries.Ymax) {
                    _rect.Y = value - (Height / 2);
                }
            }
        }

        [Range(-5, 5)]
        public int XVelocity { get; set; }
        [Range(-5, 5)]
        public int YVelocity { get; set; }

        [Range(0, 1000)]
        public int Height { get; }
        [Range(0, 1000)]
        public int Width { get; }

        internal PongObject(int x, int y, int width, int height, Boundaries boundaries) {
            // Order matters here, as X and Y depend on the boundaries being set properly.
            // Order also matters for _rect, depends on Height and Width.
            this.boundaries = boundaries;
            Height = height;
            Width = width;
            _rect = new Rectangle(0, 0, Width, Height);
            this.X = x;
            this.Y = y;
        }

        internal void MoveObject() {
            X += XVelocity;
            Y += YVelocity;
        }
    }

    class PongBall : PongObject {

        internal PongBall(int x, int y, int width, int height, Boundaries boundaries) : base(x, y, width, height, boundaries) { }

        internal new void MoveObject() {
            X += XVelocity;
            var newY = Y + YVelocity;

            if (newY < boundaries.Ymin || newY > boundaries.Ymax) {
                YVelocity = -YVelocity;
                newY = Y + YVelocity;
            }

            Y = newY;
        }
    }

    struct Boundaries {
        public int Xmin, Xmax, Ymin, Ymax;
    }

}
