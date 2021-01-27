using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WindowsFormsApp1.MarchingLineAlgorithm;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        const int GridX = 20;
        const int GridY = 20;
        const int GridCellSize = 30;
        Face[] voxels = new Face[GridX * GridY];

        Point start;
        Point end;

        public Form1()
        {
            InitializeComponent();
            Bitmap img = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = img;
            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseMove += PictureBox1_MouseMove;
            for (int i = 0; i < voxels.Length; i++) voxels[i] = Face.Unknown;
            Draw();
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                PictureBox1_MouseDown(sender, e);
            }
        }

        Dictionary<Face, Color> colorMap = new Dictionary<Face, Color>{
            { Face.North, Color.Blue },
            { Face.South, Color.Green },
            { Face.East, Color.Red },
            { Face.West, Color.Orange },
            { Face.None, Color.Purple },
        };
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < voxels.Length; i++) voxels[i] = Face.Unknown;
            if (e.Button == MouseButtons.Left)
            {
                start = e.Location;
            }
            if (e.Button == MouseButtons.Right)
            {
                end = e.Location;
            }
            var alg = new MarchingLineAlgorithm();
            alg.VisitVoxel += Alg_VisitVoxel;
            Stopwatch s = new Stopwatch();
            var trans = new PointF(-GridX / 2f, -GridY / 2f);
            var p1 = new PointF(start.X / (float)GridCellSize, GridY-start.Y / (float)GridCellSize);
            var p2 = new PointF(end.X / (float)GridCellSize, GridY - end.Y / (float)GridCellSize);

            p1.X = Math.Min(p1.X + trans.X, GridX - 1);
            p1.Y = Math.Min(p1.Y + trans.Y, GridY - 1);
            p2.X = Math.Min(p2.X + trans.X, GridX - 1);
            p2.Y = Math.Min(p2.Y + trans.Y, GridY - 1);

          


            alg.March(p1, p2);
            label1.Text = $"({p1.X:0.##}; {p1.Y:0.##})     ({p2.X:0.##}; {p2.Y:0.##})";
            Draw();
        }

        private void Alg_VisitVoxel(int x, int y, MarchingLineAlgorithm.Face face)
        {
            x += GridX / 2;
            y += GridY / 2;
            voxels[x + y * GridY] = face;
        }

         private Face GetVoxel(int x, int y)
        {
            x += GridX / 2;
            y += GridY / 2;
            return voxels[x + y * GridX];
        }

        private void Draw()
        {
            var img = pictureBox1.Image;
            var g = Graphics.FromImage(img);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.Gray);
            for (int x = 0; x < GridX; x++)
            {
                for (int y = 0; y < GridY; y++)
                {
                    var voxel = GetVoxel(x - GridX / 2, ((GridY - y - 1) - GridY / 2));
                    if (voxel == Face.Unknown) continue;
                    var col = new SolidBrush(colorMap[voxel]);
                    g.FillRectangle(col, new Rectangle(x * GridCellSize, y * GridCellSize, GridCellSize, GridCellSize));
                }
            }

            var lineCol = Pens.DimGray;
            var lineColCenter = new Pen(Color.DimGray, 4);
            for (int x = 0; x < GridX; x++)
            {
                g.DrawLine(x == GridX/2 ? lineColCenter : lineCol, x * GridCellSize, 0, x * GridCellSize, GridY * GridCellSize);
            }
            for (int y = 0; y < GridY; y++)
            {
                g.DrawLine(y == GridY / 2 ? lineColCenter : lineCol, 0, y * GridCellSize, GridX * GridCellSize, y * GridCellSize);
            }
            int dotSize = GridCellSize / 2;
            g.DrawLine(new Pen(Color.Black, 2), start, end);
            g.FillEllipse(Brushes.White, start.X - dotSize / 2, start.Y - dotSize / 2, dotSize, dotSize);
            g.DrawEllipse(Pens.Black, start.X - dotSize / 2, start.Y - dotSize / 2, dotSize, dotSize);
            g.FillEllipse(Brushes.Black, end.X - dotSize / 2, end.Y - dotSize / 2, dotSize, dotSize);
            Invalidate(true);
        }
    }
}
