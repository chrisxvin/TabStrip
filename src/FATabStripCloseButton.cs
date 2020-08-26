using System;
using System.Drawing;
using System.Windows.Forms;

namespace FarsiLibrary.Win
{
	internal class FATabStripCloseButton
	{
		internal FATabStripCloseButton(ToolStripProfessionalRenderer renderer)
		{
			this.Renderer = renderer;
		}

		public void CalcBounds(FATabStripItem tab)
		{
			this.Rect = new Rectangle((int)tab.StripRect.Right - 20, (int)tab.StripRect.Top + 5, 15, 15);
			this.RedrawRect = new Rectangle(this.Rect.X - 2, this.Rect.Y - 2, this.Rect.Width + 4, this.Rect.Height + 4);
		}

		public void Draw(Graphics g)
		{
			if (this.IsVisible)
			{
				Color color = this.IsMouseOver ? Color.White : Color.DarkGray;
				g.FillRectangle(Brushes.White, this.Rect);
				if (this.IsMouseOver)
				{
					g.FillEllipse(Brushes.IndianRed, this.Rect);
				}
				int num = 4;
				using (Pen pen = new Pen(color, 1.6f))
				{
					g.DrawLine(pen, this.Rect.Left + num, this.Rect.Top + num, this.Rect.Right - num, this.Rect.Bottom - num);
					g.DrawLine(pen, this.Rect.Right - num, this.Rect.Top + num, this.Rect.Left + num, this.Rect.Bottom - num);
				}
			}
		}

		public Rectangle Rect = Rectangle.Empty;

		public Rectangle RedrawRect = Rectangle.Empty;

		public bool IsMouseOver;

		public bool IsVisible;

		public ToolStripProfessionalRenderer Renderer;
	}
}
