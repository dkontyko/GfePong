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

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            var blackBrush = new SolidBrush(Color.Black);
            var leftBumper = new Rectangle(5, 5, 10, 50);
            e.Graphics.FillRectangle(blackBrush, leftBumper);
        }
    }
}
