using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GrayScale
{
    public partial class frmMain : Form
    {
        private Bitmap mobjFormBitmap;
        private Graphics mobjBitmapGraphics;
        private int mintFormWidth;
        private int mintFormHeight;
        private Boolean mblnDoneOnce = false;
        private Bitmap mimgColour;
        private byte mbytState = 0; //colour image (as opposed to black and white)
        private OpenFileDialog mobjDialog;
        private DialogResult mresult;
        
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            mobjDialog = new OpenFileDialog();
            mobjDialog.InitialDirectory = "C:\\";
            mobjDialog.DefaultExt = ".jpg";
            mobjDialog.Filter = "Images|*.jpg";

            mresult = mobjDialog.ShowDialog();
            if (mresult == DialogResult.OK)
            {
                mimgColour = (Bitmap)Image.FromFile(mobjDialog.FileName);
            }
            else
                Application.Exit();

        }

        private void frmMain_Activated(object sender, EventArgs e)
        {
            if (!mblnDoneOnce)
            {
                mblnDoneOnce = true;
                mintFormWidth = this.Width;
                mintFormHeight = this.Height;
                mobjFormBitmap = new Bitmap(mintFormWidth, mintFormHeight, this.CreateGraphics());
                mobjBitmapGraphics = Graphics.FromImage(mobjFormBitmap);
                this.WindowState = FormWindowState.Minimized;
                RefreshImage();
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                mintFormWidth = this.Width;
                mintFormHeight = this.Height;
                mobjFormBitmap = new Bitmap(mintFormWidth, mintFormHeight, this.CreateGraphics());
                mobjBitmapGraphics = Graphics.FromImage(mobjFormBitmap);
                RefreshImage();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //Do nothing
        }

        private void frmMain_Paint(object sender, PaintEventArgs e)
        {
            if (mobjFormBitmap != null)
                e.Graphics.DrawImage(mobjFormBitmap, 0, 0);
        }

        private void RefreshImage()
        {
            Color objColor;
            int intDepth;
            mobjBitmapGraphics.FillRectangle(Brushes.White, 0, 0, mintFormWidth, mintFormHeight);

            if (mbytState == 0)  // original colour image
                mobjBitmapGraphics.DrawImage(mimgColour, (mintFormWidth / 2) - (mimgColour.Width / 2), (mintFormHeight / 2) - (mimgColour.Height / 2));            
            else
            {
                Bitmap objGrayImage;
                Graphics objImageGraphics;
                Pen objPen;
                
                objGrayImage = (Bitmap)mimgColour.Clone();
                objImageGraphics = Graphics.FromImage(objGrayImage);

                objPen = new Pen(Brushes.White);
                for (int intX = 0; intX < objGrayImage.Width; intX++)
                    for (int intY = 0; intY < objGrayImage.Height; intY++)
                    {
                        objColor = mimgColour.GetPixel(intX, intY);
                        intDepth = (objColor.R + objColor.G + objColor.B ) / 3;
                        objPen.Color = Color.FromArgb(intDepth, intDepth, intDepth);
                        objImageGraphics.DrawRectangle(objPen, intX, intY, 1, 1);
                    }
                mobjBitmapGraphics.DrawImage(objGrayImage, (mintFormWidth / 2) - (objGrayImage.Width / 2), (mintFormHeight / 2) - (objGrayImage.Height / 2));            
            }
            this.Invalidate();
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (mbytState == 0)
                    mbytState = 1;  // switch to gray
                else
                    mbytState = 0;  // switch back to colour

                RefreshImage();
            }           
        }

    }
}
