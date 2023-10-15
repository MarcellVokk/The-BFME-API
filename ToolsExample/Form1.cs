using System.Diagnostics;
using The_BFME_API_by_MarcellVokk.Tools;

namespace ToolsExample
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
                    Stopwatch sw = Stopwatch.StartNew();
                    pictureBox1.Image = MapSpotPreviewTool.DrawMapSpotsPreview(new Bitmap(ofd.FileName));
                    sw.Stop();

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
                    pictureBox1.Image.Save(sfd.FileName);
                }
            }
        }
    }
}