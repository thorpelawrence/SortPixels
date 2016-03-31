using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void sortButton_Click(object sender, EventArgs e)
        {
            float edgeThreshold = (float)numericUpDown1.Value;
            Bitmap image = new Bitmap(pictureBox1.Image);
            List<List<Color>> sortedPixels = new List<List<Color>>();
            if (radioButton2.Checked) image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            progressBar.Maximum = image.Height;
            Cursor = Cursors.WaitCursor;
            for (int y = 0; y < image.Height; y++)
            {
                Dictionary<int, Color> row = new Dictionary<int, Color>();
                Dictionary<int, Color> rowPart = new Dictionary<int, Color>();
                for (int x = 0; x < image.Width; x++)
                {
                    if (x > 0)
                    {
                        float leftPixelBrightness = image.GetPixel(x - 1, y).GetBrightness();
                        if (leftPixelBrightness - edgeThreshold > image.GetPixel(x, y).GetBrightness() || leftPixelBrightness + edgeThreshold < image.GetPixel(x, y).GetBrightness())
                        {
                            rowPart.Add(x, image.GetPixel(x, y));
                        }
                        else
                        {
                            row.Add(x, image.GetPixel(x, y));
                        }
                    }
                    else
                    {
                        row.Add(x, image.GetPixel(x, y));
                    }
                    progressBar.Value = y;
                }
                List<Color> rowPartValList = rowPart.Values.ToList();
                List<int> rowPartKeyList = rowPart.Keys.ToList();
                rowPartValList.Sort(delegate (Color left, Color right)
                {
                    return left.GetBrightness().CompareTo(right.GetBrightness());
                });
                for (int p = 0; p < rowPartValList.Count; p++)
                {
                    row.Add(rowPartKeyList[p], rowPartValList[p]);
                }
                sortedPixels.Add(row.Values.ToList());
            }

            Bitmap imageSorted = image;
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    imageSorted.SetPixel(x, y, sortedPixels[y][x]);
                }
            }
            progressBar.Value = image.Height;
            if (radioButton2.Checked) imageSorted.RotateFlip(RotateFlipType.Rotate270FlipNone);
            pictureBox1.Image = imageSorted;
            Cursor = Cursors.Default;
            saveButton.Enabled = true;
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            o.ShowDialog();
            try
            {
                pictureBox1.Image = new Bitmap(Image.FromFile(o.FileName));
                sortButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.FileName = "Sorted.jpg";
            s.DefaultExt = "jpg";
            s.Filter = "Image file (*.jpg) | *.jpg";
            s.ShowDialog();
            try
            {
                pictureBox1.Image.Save(s.FileName);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
