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
        int stages = 0;
        string fileName;
        Bitmap image;
        Bitmap imageSorted;
        float edgeThreshold;
        List<List<Color>> sortedPixels;
        private void sortButton_Click(object sender, EventArgs e)
        {
            edgeThreshold = (float)numericUpDown1.Value;
            image = new Bitmap(pictureBox1.Image);
            sortedPixels = new List<List<Color>>();
            sortButton.Enabled = false;
            loadButton.Enabled = false;
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            numericUpDown1.Enabled = false;
            label1.Enabled = false;
            saveButton.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
            stages++;
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            o.ShowDialog();
            try
            {
                pictureBox1.Image = new Bitmap(Image.FromFile(o.FileName));
                fileName = o.SafeFileName;
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
            s.FileName = fileName + "_sorted" + stages;
            s.DefaultExt = "png";
            s.Filter = "Image file (*.png) | *.png";
            if (s.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image.Save(s.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (radioButton2.Checked) image.RotateFlip(RotateFlipType.Rotate90FlipNone);
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
                int percentage = (int)((float)y / (float)image.Height * 100);
                if (percentage % 5 == 0)
                {
                    progressBar.Invoke(new Action(() => progressBar.Value = percentage));
                }
            }

            imageSorted = new Bitmap(image);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    imageSorted.SetPixel(x, y, sortedPixels[y][x]);
                }
            }
            if (radioButton2.Checked) imageSorted.RotateFlip(RotateFlipType.Rotate270FlipNone);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Image = imageSorted;
            progressBar.Value = 100;
            sortButton.Enabled = true;
            loadButton.Enabled = true;
            radioButton1.Enabled = true;
            radioButton2.Enabled = true;
            numericUpDown1.Enabled = true;
            label1.Enabled = true;
            saveButton.Enabled = true;
            undoButton.Enabled = true;
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = image;
            stages--;
            undoButton.Enabled = false;
        }
    }
}
