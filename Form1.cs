using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintClone
{
    public partial class Form1 : Form
    {
        Bitmap bm = new Bitmap(1920, 1080); //hình ảnh hiển thị
        Bitmap blankBM;
        Pen pen = new Pen(Color.Black, 5); //chất liệu cho graphics vẽ 
        Pen eraser = new Pen(Color.White, 5);
        bool IsDrawing = false;
        Color selectedColor = Color.Black;

        Graphics g; // tool vẽ lên bitmap

        // 0: pencil, 1: rectangle, 2: ellipse, 3: line, -1: Fill, -2: Eraser
        int DrawMode = 0;

        // hỗ trợ chức năng undo, redo
        const int MaxSize = 10;
        Stack<Bitmap> undoStack = new Stack<Bitmap>(MaxSize);
        Stack<Bitmap> redoStack = new Stack<Bitmap>(MaxSize);

        //save load system
        Image OpenedFile;

        MyPoint p1;
        MyPoint p2;
        public Form1()
        {
            InitializeComponent();

            //set background to white - for saving feature
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j= 0; j < bm.Height; j++)
                {
                    bm.SetPixel(i, j, Color.White);
                }
            }
            undoStack.Push(new Bitmap(bm));
            blankBM = new Bitmap(bm);

            board.Image = bm;
            g = Graphics.FromImage(board.Image);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            eraser.StartCap = eraser.EndCap = pen.StartCap = pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            selectedColor = ((Button) sender).BackColor;
            pen.Color = selectedColor;
            pbSelectedColor.BackColor = selectedColor;
        }

        private void board_MouseDown(object sender, MouseEventArgs e)
        {
            IsDrawing = true;

            p1 = new MyPoint(e.X, e.Y);

            if (DrawMode == -1) //fill
            {
                Color oldColor = bm.GetPixel(e.X, e.Y);
                if (oldColor.ToArgb() != selectedColor.ToArgb())
                {
                    FillRegion(e.X, e.Y, oldColor, selectedColor);

                    undoStack.Push(new Bitmap(bm));
                    redoStack.Clear();
                }
            }
        }
        private void FillRegion(int x, int y, Color oldColor, Color newColor)
        {
            Stack<MyPoint> points = new Stack<MyPoint>();
            points.Push(new MyPoint(x, y));

            while (points.Count > 0)
            {
                MyPoint p = points.Pop();
                if (p.X > 0 && p.X < board.Width - 1 && p.Y > 0 && p.Y < board.Height - 1 && bm.GetPixel(p.X, p.Y) == oldColor)
                {
                    bm.SetPixel(p.X, p.Y, newColor);
                    points.Push(new MyPoint(p.X - 1, p.Y));
                    points.Push(new MyPoint(p.X + 1, p.Y));
                    points.Push(new MyPoint(p.X, p.Y - 1));
                    points.Push(new MyPoint(p.X, p.Y + 1));
                }
            }
        }
        private void board_MouseUp(object sender, MouseEventArgs e)
        {
            IsDrawing = false;
            switch (DrawMode)
            {
                case 1:
                    MyRectangle rect = new MyRectangle(p1, p2, ckbFill.Checked);
                    rect.Draw(g, pen);
                    break;
                case 2:
                    MyEllipse ell = new MyEllipse(p1, p2, ckbFill.Checked);
                    ell.Draw(g, pen);
                    break;
                case 3:
                    MyLine line = new MyLine(p1, p2);
                    line.Draw(g, pen);
                    break;
            }
            if (DrawMode != -1) // != fill
            {
                undoStack.Push(new Bitmap(bm));
                redoStack.Clear();
            }
        }

        private void board_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDrawing)
            {
                MyLine line = new MyLine(p2, new MyPoint(e.X, e.Y));
                if (DrawMode == 0)
                    line.Draw(g, pen);
                if (DrawMode == -2)
                    line.Draw(g, eraser);
            }
            
            board.Refresh();

            p2 = new MyPoint(e.X, e.Y);
        }
        private void board_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (IsDrawing)
            {
                switch (DrawMode)
                {
                    case 1:
                        MyRectangle rect = new MyRectangle(p1, p2, ckbFill.Checked);
                        rect.Draw(g, pen);
                        break;
                    case 2:
                        MyEllipse ell = new MyEllipse(p1, p2, ckbFill.Checked);
                        ell.Draw(g, pen);
                        break;
                    case 3:
                        MyLine line = new MyLine(p1, p2);
                        line.Draw(g, pen);
                        break;
                }
            }
        }
        private void numSize_ValueChanged(object sender, EventArgs e)
        {
            eraser.Width = pen.Width = (float) numSize.Value;
        }

        private void btnPickColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            DialogResult dr = colorDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                selectedColor = colorDialog.Color;
                pen.Color = selectedColor;
                pbSelectedColor.BackColor = selectedColor;
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 1)
            {
                redoStack.Push(undoStack.Pop());
                bm = new Bitmap(undoStack.Peek());
                board.Image = bm;
                g = Graphics.FromImage(board.Image);
            }
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            if (redoStack.Count > 0)
            {
                undoStack.Push(redoStack.Peek());
                bm = new Bitmap(redoStack.Pop());
                board.Image = bm;
                g = Graphics.FromImage(board.Image);
            }
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("All unsaved works will be lost. Are you sure?", "Are you sure?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                g.Clear(Color.White);
                undoStack.Clear();
                undoStack.Push(new Bitmap(blankBM));
                redoStack.Clear();
                DrawMode = 0;
            }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Jpeg Image|*.jpg|Bitmap Image|*.bmp";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();
                switch (saveFileDialog.FilterIndex)
                {
                    case 1:
                        this.board.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case 2:
                        this.board.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                }
                fs.Close();
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("All unsaved works will be lost. Are you sure?", "Are you sure?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                //fix bug
                DrawMode = 0;

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Jpeg Image|*.jpg|Bitmap Image|*.bmp";
                DialogResult dr2 = openFileDialog.ShowDialog();

                if (dr2 == DialogResult.OK)
                {
                    OpenedFile = Image.FromFile(openFileDialog.FileName);
                    bm = new Bitmap(OpenedFile);
                    g = Graphics.FromImage(bm);
                    board.Image = bm;

                    undoStack.Clear();
                    undoStack.Push(new Bitmap(bm));
                    redoStack.Clear();
                }
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("All unsaved works will be lost. Are you sure?", "Are you sure?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
                this.Close();
        }
        
        private void btnPencil_Click(object sender, EventArgs e)
        {
            DrawMode = 0;
        }
        private void btnRectangle_Click(object sender, EventArgs e)
        {
            DrawMode = 1;
        }
        private void btnEclipse_Click(object sender, EventArgs e)
        {
            DrawMode = 2;
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            DrawMode = 3;
        }
        private void btnFill_Click(object sender, EventArgs e)
        {
            DrawMode = -1;
        }
        private void btnEraser_Click(object sender, EventArgs e)
        {
            DrawMode = -2;
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnUndo_Click(sender, e);
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnRedo_Click(sender, e);
        }
    }
}
