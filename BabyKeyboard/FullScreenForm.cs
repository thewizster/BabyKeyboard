using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BabyKeyboard
{
    public partial class FullScreenForm : Form
    {
        private readonly Random random;
        private readonly Button exitButton;
        private readonly List<Shape> shapes;

        public FullScreenForm(Rectangle bounds)
        {
            InitializeComponent();
            random = new Random();
            shapes = new List<Shape>();

            this.FormBorderStyle = FormBorderStyle.None;
            this.Bounds = bounds;
            this.StartPosition = FormStartPosition.Manual;
            this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true;
            this.TopMost = true;

            // Initialize exit button
            exitButton = new Button
            {
                Text = "Exit",
                BackColor = Color.Red,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(75, 30),
                Location = new Point(this.Bounds.Width - 85, 10),
                Visible = false
            };
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.Click += ExitButton_Click;

            this.Controls.Add(exitButton);
            this.MouseMove += FullScreenForm_MouseMove;
            this.Paint += FullScreenForm_Paint;
        }

        private void FullScreenForm_MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.X > this.Bounds.Width - 100 && e.Y < 50)
            {
                exitButton.Visible = true;
            }
            else
            {
                exitButton.Visible = false;
            }
        }

        private void ExitButton_Click(object? sender, EventArgs e)
        {
            FormController.BroadcastExit();
            Application.Exit(); // Ensure the application exits completely
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Change background color for any key press
            FormController.BroadcastColorChange();
            FormController.BroadcastDrawShape();
            return true;
        }

        public void ChangeColor()
        {
            this.Invoke(new Action(() =>
            {
                this.BackColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
            }));
        }

        public void DrawShape()
        {
            this.Invoke(new Action(() =>
            {
                Color shapeColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                int shape = random.Next(3); // 0: Circle, 1: Square, 2: Triangle
                int size = random.Next(50, 200);
                int x = random.Next(this.ClientSize.Width - size);
                int y = random.Next(this.ClientSize.Height - size);

                Shape newShape = new Shape
                {
                    ShapeType = shape,
                    Color = shapeColor,
                    Size = size,
                    Location = new Point(x, y)
                };
                shapes.Add(newShape);

                // Force the form to repaint
                this.Invalidate();
            }));
        }

        private void FullScreenForm_Paint(object? sender, PaintEventArgs e)
        {
            using (Graphics g = e.Graphics)
            {
                foreach (var shape in shapes)
                {
                    Brush brush = new SolidBrush(shape.Color);

                    switch (shape.ShapeType)
                    {
                        case 0: // Circle
                            g.FillEllipse(brush, shape.Location.X, shape.Location.Y, shape.Size, shape.Size);
                            break;
                        case 1: // Square
                            g.FillRectangle(brush, shape.Location.X, shape.Location.Y, shape.Size, shape.Size);
                            break;
                        case 2: // Triangle
                            Point[] points = {
                                new Point(shape.Location.X + shape.Size / 2, shape.Location.Y),
                                new Point(shape.Location.X, shape.Location.Y + shape.Size),
                                new Point(shape.Location.X + shape.Size, shape.Location.Y + shape.Size)
                            };
                            g.FillPolygon(brush, points);
                            break;
                    }
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            FormController.UnregisterForm(this);
            base.OnFormClosed(e);
        }
    }

    public class Shape
    {
        public int ShapeType { get; set; }
        public Color Color { get; set; }
        public int Size { get; set; }
        public Point Location { get; set; }
    }
}
