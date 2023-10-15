using System.Diagnostics;
using The_BFME_API.BFME_Shared;

namespace MapSpotTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "PNG Image|*.png";
                ofd.Multiselect = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = new Bitmap(ofd.FileName);

                    Stopwatch sw = Stopwatch.StartNew();
                    List<Rectangle> spots = SpotDetectionEngine.GetMapSpots(new Bitmap(ofd.FileName));
                    sw.Stop();

                    pictureBox1.Controls.Clear();
                    int i = 0;
                    foreach (Rectangle rectangle in spots)
                    {
                        pictureBox1.Controls.Add(new Panel() { Left = rectangle.X, Top = rectangle.Y, Width = 1, Height = rectangle.Height, BorderStyle = BorderStyle.None, BackColor = Color.Red });
                        pictureBox1.Controls.Add(new Panel() { Left = rectangle.X, Top = rectangle.Y, Width = rectangle.Width, Height = 1, BorderStyle = BorderStyle.None, BackColor = Color.Red });
                        pictureBox1.Controls.Add(new Panel() { Left = rectangle.X + rectangle.Width, Top = rectangle.Y, Width = 1, Height = rectangle.Height, BorderStyle = BorderStyle.None, BackColor = Color.Red });
                        pictureBox1.Controls.Add(new Panel() { Left = rectangle.X, Top = rectangle.Y + rectangle.Height, Width = rectangle.Width, Height = 1, BorderStyle = BorderStyle.None, BackColor = Color.Red });
                        pictureBox1.Controls.Add(new Label() { Text = i.ToString(), Left = rectangle.X, Top = rectangle.Y - 15, BackColor = Color.Red, ForeColor = Color.White, AutoSize = true });
                        i++;
                    }

                    label1.Text = $"Generating spot indexes took {sw.Elapsed.TotalMilliseconds}ms";
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Bitmap b = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height);
                    pictureBox1.DrawToBitmap(b, new Rectangle(0, 0, b.Width, b.Height));

                    b.Save(sfd.FileName);
                }
            }
        }
    }
}