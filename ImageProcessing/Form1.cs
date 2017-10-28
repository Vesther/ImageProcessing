using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessing
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Add our filter selection strings to the ComboBox element
            comboBox1.Items.AddRange(new Object[] { "Blur", "Sharpen", "Edges", "Emboss" });
            // And preselect the first one (blur)
            comboBox1.SelectedIndex = 0;
        }

        private Bitmap convolute(Bitmap img)
        {
            // 3x3 matrices defining the image processing kernel that will be convoluted with our bitmap
            float[,] blur = new float[3, 3] {
                { 0.109634f, 0.111842f, 0.109634f },
                { 0.111842f, 0.114094f, 0.111842f },
                { 0.109634f, 0.111842f, 0.109634f }
            };

            float[,] edge = new float[3, 3] {
                { -1, -1, -1 },
                { -1, 8, -1 },
                { -1, -1, -1 }
            };

            float[,] sharpen = new float[3, 3] {
                { 0, -1, 0 },
                { -1, 5, -1 },
                { 0, -1, 0 }
            };

            float[,] emboss = new float[3, 3] {
                { -2, -1, 0 },
                { -1, 1, 1 },
                { 0, 1, 2 }
            };

            int selected = comboBox1.SelectedIndex;

            float[,] matrix;

            switch (selected)
            {
                case 0:
                    matrix = blur;
                    break;
                case 1:
                    matrix = sharpen;
                    break;
                case 2:
                    matrix = edge;
                    break;
                case 3:
                    matrix = emboss;
                    break;
                default:
                    matrix = blur;
                    break;
            }

            Bitmap result = new Bitmap(img.Width, img.Height);

            for (int i = 1; i < img.Width-1; i++)
            {
                for (int j = 1; j < img.Height-1; j++)
                {
                    Color color = img.GetPixel(i, j);
                    float red = 0;
                    float green = 0;
                    float blue = 0;
                    // float alpha = 0;

                    for (int ii = 0; ii < 3; ii++)
                    {
                        for (int jj = 0; jj < 3; jj++)
                        {
                            Color temp = img.GetPixel(i - 1 + ii, j - 1 + jj);
                            red += temp.R * matrix[ii, jj];
                            green += temp.G * matrix[ii, jj];
                            blue += temp.B * matrix[ii, jj];
                            // alpha += temp.A * matrix[ii, jj];
                        }
                    }

                    if (red > 255) red = 255;
                    if (blue > 255) blue = 255;
                    if (green > 255) green = 255;
                    // if (alpha > 255) alpha = 255;


                    if (red < 0) red = 0;
                    if (blue < 0) blue = 0;
                    if (green < 0) green = 0;
                    // if (alpha < 0) alpha = 0;

                    result.SetPixel(i, j, Color.FromArgb((int)red, (int)green, (int)blue));
                }
            }

            return result;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Guard against unloaded image
            if (pictureBox1.Image == null) return;

            PictureBox pb1 = pictureBox1;
            Bitmap bmp = new Bitmap(pb1.Image);
            pb1.Image = convolute(bmp);
            pb1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string path;
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Image files (*.jpg, *.png)|*.jpg; *.png";
            if (file.ShowDialog() == DialogResult.OK)
            {
                path = file.FileName;
                pictureBox1.Image = Image.FromFile(path);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            // Guard against unloaded image
            if (pictureBox1.Image == null) return;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Jpeg Image|*.jpg|PNG Image|*.png";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.FileName = "test";
            saveFileDialog1.ShowDialog();

            // Guard against empty filenames
            if (saveFileDialog1.FileName == "") return;

            System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();

            switch (saveFileDialog1.FilterIndex)
            {
                case 1:
                    // JPEG
                    pictureBox1.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                case 2:
                    // PNG
                    pictureBox1.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                    break;
            }

            fs.Close(); 
        }
    }
}
