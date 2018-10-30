using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoSelector
{
    public partial class Form1 : Form
    {
        List<string> imagePath = new List<string>();
        int currentIndex = 0;
        Image image = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSourceBrows_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSource.Text))
            {
                folderBrowserDialog1.SelectedPath = txtSource.Text;
            }

            folderBrowserDialog1.ShowDialog();
            if (string.IsNullOrEmpty(folderBrowserDialog1.SelectedPath))
                return;

            txtSource.Text = folderBrowserDialog1.SelectedPath;
            txtDes.Text = Path.Combine(txtSource.Text, "Selected");

            imagePath.Clear();

            foreach (string imgPath in Directory.GetFiles(txtSource.Text).Where(x => Path.GetExtension(x.ToLower()) == ".jpg" || Path.GetExtension(x.ToLower()) == ".jpeg"))
            {
                imagePath.Add(imgPath);
            }

            loadImage();
            pictureBox1.Select();
        }

        private void btnDesBrows_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDes.Text))
            {
                folderBrowserDialog1.SelectedPath = txtDes.Text;
            }

            folderBrowserDialog1.ShowDialog();
            txtDes.Text = folderBrowserDialog1.SelectedPath;
            pictureBox1.Select();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.KeyCode)
                {
                    case Keys.Right:
                        {
                            pictureBox1.Select();
                            if (imagePath.Count > currentIndex + 1)
                            {
                                currentIndex++;
                                loadImage();

                            }
                            else
                            {
                                currentIndex = 0;
                                loadImage();
                            }



                            break;
                        }

                    case Keys.Left:
                        {
                            pictureBox1.Select();
                            if (currentIndex == 0)
                            {
                                currentIndex = imagePath.Count - 1;
                                loadImage();
                            }
                            else
                            {
                                currentIndex--;
                                loadImage();
                            }


                            break;
                        }

                    case Keys.Enter:
                        {
                            pictureBox1.Select();
                            CopyImage();

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void CopyImage()
        {
            if (string.IsNullOrEmpty(txtDes.Text))
            {
                MessageBox.Show("Destination folder is empty");
            }

            string currentImage = imagePath[currentIndex];
            string desFileName = Path.Combine(txtDes.Text, Path.GetFileName(currentImage));

            if (File.Exists(desFileName))
            {
                ShowLabel(2);
            }
            else
            {
                File.Copy(currentImage, desFileName);
                ShowLabel(1);

            }

        }

        private void ShowLabel(int type)
        {
            if (type == 1)
            {
                lblCopied.Text = "Copied";
                lblCopied.ForeColor = Color.Green;
            }
            else
            {
                lblCopied.Text = "File already exists";
                lblCopied.ForeColor = Color.Red;
            }

            timer1.Enabled = true;
            timer1.Interval = 2000;
            lblCopied.Visible = true;
        }

        private void loadImage()
        {
            if (imagePath.Count == 0)
                return;

            image = Image.FromFile(imagePath[currentIndex]);

            // Compute thumbnail size.
            Size thumbnailSize = GetThumbnailSize(image);

            pictureBox1.Image = image;//.GetThumbnailImage(thumbnailSize.Width, thumbnailSize.Height, ThumbnailCallback, IntPtr.Zero);
            GC.Collect();
        }

        private Size GetThumbnailSize(Image original)
        {
            // Maximum size of any dimension.
            int maxPixels = Math.Max(pictureBox1.Width, pictureBox1.Height);

            // Width and height.
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            // Compute best factor to scale entire image based on larger dimension.
            double factor;
            if (originalWidth > originalHeight)
            {
                factor = (double)maxPixels / originalWidth;
            }
            else
            {
                factor = (double)maxPixels / originalHeight;
            }

            // Return thumbnail size.
            return new Size((int)(originalWidth * factor), (int)(originalHeight * factor));
        }

        public bool ThumbnailCallback()
        {
            return true;
        }

        private void txtDes_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDes.Text))
            {
                if (!Directory.Exists(txtDes.Text))
                {
                    Directory.CreateDirectory(txtDes.Text);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblCopied.Visible = false;
            timer1.Enabled = false;
        }
    }
}
