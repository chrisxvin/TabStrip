using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FarsiLibrary.Win.BaseClasses;
using FarsiLibrary.Win.Design;

namespace FarsiLibrary.Win
{
	[DefaultEvent("TabStripItemSelectionChanged")]
	[DefaultProperty("Items")]
	[ToolboxItem(true)]
	[Designer(typeof(FATabStripDesigner))]
	[ToolboxBitmap("FATabStrip.bmp")]
	public class FATabStrip : BaseStyledPanel, ISupportInitialize, IDisposable
	{
		public event TabStripItemClosingHandler TabStripItemClosing;

		public event TabStripItemChangedHandler TabStripItemSelectionChanged;

		public event HandledEventHandler MenuItemsLoading;

		public event EventHandler MenuItemsLoaded;

		public event EventHandler TabStripItemClosed;

		public FATabStrip()
		{
			this.BeginInit();
			base.SetStyle(ControlStyles.ContainerControl, true);
			base.SetStyle(ControlStyles.UserPaint, true);
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			base.SetStyle(ControlStyles.Selectable, true);
			this.items = new FATabStripItemCollection();
			this.items.CollectionChanged += this.OnCollectionChanged;
			base.Size = new Size(350, 200);
			this.menu = new ContextMenuStrip();
			this.menu.Renderer = base.ToolStripRenderer;
			this.menu.ItemClicked += this.OnMenuItemClicked;
			this.menu.VisibleChanged += this.OnMenuVisibleChanged;
			this.closeButton = new FATabStripCloseButton(base.ToolStripRenderer);
			this.Font = FATabStrip.defaultFont;
			this.sf = new StringFormat();
			this.EndInit();
			this.UpdateLayout();
		}

		public HitTestResult HitTest(Point pt)
		{
			if (this.closeButton.IsVisible && this.closeButton.Rect.Contains(pt))
			{
				return HitTestResult.CloseButton;
			}
			if (this.GetTabItemByPoint(pt) != null)
			{
				return HitTestResult.TabItem;
			}
			return HitTestResult.None;
		}

		public void AddTab(FATabStripItem tabItem)
		{
			this.AddTab(tabItem, false);
		}

		public void AddTab(FATabStripItem tabItem, bool autoSelect)
		{
			tabItem.Dock = DockStyle.Fill;
			this.Items.Add(tabItem);
			if ((autoSelect && tabItem.Visible) || (tabItem.Visible && this.Items.DrawnCount < 1))
			{
				this.SelectedItem = tabItem;
				this.SelectItem(tabItem);
			}
		}

		public void RemoveTab(FATabStripItem tabItem)
		{
			int num = this.Items.IndexOf(tabItem);
			if (num >= 0)
			{
				this.UnSelectItem(tabItem);
				this.Items.Remove(tabItem);
			}
			if (this.Items.Count > 0)
			{
				if (this.Items[num - 1] != null)
				{
					this.SelectedItem = this.Items[num - 1];
					return;
				}
				this.SelectedItem = this.Items.FirstVisible;
			}
		}

		public FATabStripItem GetTabItemByPoint(Point pt)
		{
			FATabStripItem result = null;
			bool flag = false;
			for (int i = 0; i < this.Items.Count; i++)
			{
				FATabStripItem fatabStripItem = this.Items[i];
				if (fatabStripItem.StripRect.Contains(pt) && fatabStripItem.Visible && fatabStripItem.IsDrawn)
				{
					result = fatabStripItem;
					flag = true;
				}
				if (flag)
				{
					break;
				}
			}
			return result;
		}

		public virtual void ShowMenu()
		{
		}

		internal void UnDrawAll()
		{
			for (int i = 0; i < this.Items.Count; i++)
			{
				this.Items[i].IsDrawn = false;
			}
		}

		internal void SelectItem(FATabStripItem tabItem)
		{
			tabItem.Dock = DockStyle.Fill;
			tabItem.Visible = true;
			tabItem.Selected = true;
		}

		internal void UnSelectItem(FATabStripItem tabItem)
		{
			tabItem.Selected = false;
		}

		protected internal virtual void OnTabStripItemClosing(TabStripItemClosingEventArgs e)
		{
			if (this.TabStripItemClosing != null)
			{
				this.TabStripItemClosing(e);
			}
		}

		protected internal virtual void OnTabStripItemClosed(EventArgs e)
		{
			this.selectedItem = null;
			if (this.TabStripItemClosed != null)
			{
				this.TabStripItemClosed(this, e);
			}
		}

		protected virtual void OnMenuItemsLoading(HandledEventArgs e)
		{
			if (this.MenuItemsLoading != null)
			{
				this.MenuItemsLoading(this, e);
			}
		}

		protected virtual void OnMenuItemsLoaded(EventArgs e)
		{
			if (this.MenuItemsLoaded != null)
			{
				this.MenuItemsLoaded(this, e);
			}
		}

		protected virtual void OnTabStripItemChanged(TabStripItemChangedEventArgs e)
		{
			if (this.TabStripItemSelectionChanged != null)
			{
				this.TabStripItemSelectionChanged(e);
			}
		}

		protected virtual void OnMenuItemsLoad(EventArgs e)
		{
			this.menu.RightToLeft = this.RightToLeft;
			this.menu.Items.Clear();
			for (int i = 0; i < this.Items.Count; i++)
			{
				FATabStripItem fatabStripItem = this.Items[i];
				if (fatabStripItem.Visible)
				{
					ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(fatabStripItem.Title);
					toolStripMenuItem.Tag = fatabStripItem;
					toolStripMenuItem.Image = fatabStripItem.Image;
					this.menu.Items.Add(toolStripMenuItem);
				}
			}
			this.OnMenuItemsLoaded(EventArgs.Empty);
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged(e);
			this.UpdateLayout();
			base.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			this.SetDefaultSelected();
			Rectangle clientRectangle = base.ClientRectangle;
			clientRectangle.Width--;
			clientRectangle.Height--;
			this.DEF_START_POS = 10;
			e.Graphics.DrawRectangle(SystemPens.ControlDark, clientRectangle);
			e.Graphics.FillRectangle(Brushes.White, clientRectangle);
			e.Graphics.FillRectangle(SystemBrushes.GradientInactiveCaption, new Rectangle(clientRectangle.X, clientRectangle.Y, clientRectangle.Width, 28));
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			for (int i = 0; i < this.Items.Count; i++)
			{
				FATabStripItem fatabStripItem = this.Items[i];
				if (fatabStripItem.Visible || base.DesignMode)
				{
					this.OnCalcTabPage(e.Graphics, fatabStripItem);
					fatabStripItem.IsDrawn = false;
					this.OnDrawTabButton(e.Graphics, fatabStripItem);
				}
			}
			if (this.selectedItem != null)
			{
				this.OnDrawTabButton(e.Graphics, this.selectedItem);
			}
			if (this.Items.DrawnCount == 0 || this.Items.VisibleCount == 0)
			{
				e.Graphics.DrawLine(SystemPens.ControlDark, new Point(0, 28), new Point(base.ClientRectangle.Width, 28));
			}
			else if (this.SelectedItem != null && this.SelectedItem.IsDrawn)
			{
				int num = (int)(this.SelectedItem.StripRect.Height / 4f);
				Point point = new Point((int)this.SelectedItem.StripRect.Left - num, 28);
				e.Graphics.DrawLine(SystemPens.ControlDark, new Point(0, 28), point);
				point.X += (int)this.SelectedItem.StripRect.Width + num * 2;
				e.Graphics.DrawLine(SystemPens.ControlDark, point, new Point(base.ClientRectangle.Width, 28));
			}
			if (this.SelectedItem != null && this.SelectedItem.CanClose)
			{
				this.closeButton.IsVisible = true;
				this.closeButton.CalcBounds(this.selectedItem);
				this.closeButton.Draw(e.Graphics);
				return;
			}
			this.closeButton.IsVisible = false;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			HitTestResult hitTestResult = this.HitTest(e.Location);
			if (hitTestResult == HitTestResult.TabItem)
			{
				FATabStripItem tabItemByPoint = this.GetTabItemByPoint(e.Location);
				if (tabItemByPoint != null)
				{
					this.SelectedItem = tabItemByPoint;
					base.Invalidate();
				}
				return;
			}
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			if (hitTestResult == HitTestResult.CloseButton)
			{
				if (this.SelectedItem != null)
				{
					TabStripItemClosingEventArgs tabStripItemClosingEventArgs = new TabStripItemClosingEventArgs(this.SelectedItem);
					this.OnTabStripItemClosing(tabStripItemClosingEventArgs);
					if (!tabStripItemClosingEventArgs.Cancel && this.SelectedItem.CanClose)
					{
						this.RemoveTab(this.SelectedItem);
						this.OnTabStripItemClosed(EventArgs.Empty);
					}
				}
				base.Invalidate();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (this.closeButton.IsVisible)
			{
				if (this.closeButton.Rect.Contains(e.Location))
				{
					this.closeButton.IsMouseOver = true;
					base.Invalidate(this.closeButton.RedrawRect);
					return;
				}
				if (this.closeButton.IsMouseOver)
				{
					this.closeButton.IsMouseOver = false;
					base.Invalidate(this.closeButton.RedrawRect);
				}
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			this.closeButton.IsMouseOver = false;
			if (this.closeButton.IsVisible)
			{
				base.Invalidate(this.closeButton.RedrawRect);
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if (this.isIniting)
			{
				return;
			}
			this.UpdateLayout();
		}

		private void SetDefaultSelected()
		{
			if (this.selectedItem == null && this.Items.Count > 0)
			{
				this.SelectedItem = this.Items[0];
			}
			for (int i = 0; i < this.Items.Count; i++)
			{
				FATabStripItem fatabStripItem = this.Items[i];
				fatabStripItem.Dock = DockStyle.Fill;
			}
		}

		private void OnMenuItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			FATabStripItem fatabStripItem = (FATabStripItem)e.ClickedItem.Tag;
			this.SelectedItem = fatabStripItem;
		}

		private void OnMenuVisibleChanged(object sender, EventArgs e)
		{
			if (!this.menu.Visible)
			{
				this.menuOpen = false;
			}
		}

		private void OnCalcTabPage(Graphics g, FATabStripItem currentItem)
		{
			Font font = this.Font;
			int num;
			if (currentItem.Title == "+")
			{
				num = this.AddButtonWidth;
			}
			else
			{
				num = (base.Width - (this.AddButtonWidth + 20)) / (this.items.Count - 1);
				if (num > this.MaxTabSize)
				{
					num = this.MaxTabSize;
				}
			}
			RectangleF stripRect = new RectangleF((float)this.DEF_START_POS, 3f, (float)num, 28f);
			currentItem.StripRect = stripRect;
			this.DEF_START_POS += num;
		}

		private SizeF MeasureTabWidth(Graphics g, FATabStripItem currentItem, Font currentFont)
		{
			SizeF result = g.MeasureString(currentItem.Title, currentFont, new SizeF(200f, 28f), this.sf);
			result.Width += 25f;
			return result;
		}

		private void OnDrawTabButton(Graphics g, FATabStripItem currentItem)
		{
			this.Items.IndexOf(currentItem);
			Font font = this.Font;
			RectangleF stripRect = currentItem.StripRect;
			GraphicsPath graphicsPath = new GraphicsPath();
			float left = stripRect.Left;
			float right = stripRect.Right;
			float num = 3f;
			float num2 = stripRect.Bottom - 1f;
			float width = stripRect.Width;
			float height = stripRect.Height;
			float num3 = height / 4f;
			graphicsPath.AddLine(left - num3, num2, left + num3, num);
			graphicsPath.AddLine(right - num3, num, right + num3, num2);
			graphicsPath.CloseFigure();
			SolidBrush brush = new SolidBrush((currentItem == this.SelectedItem) ? Color.White : SystemColors.GradientInactiveCaption);
			g.FillPath(brush, graphicsPath);
			g.DrawPath(SystemPens.ControlDark, graphicsPath);
			if (currentItem == this.SelectedItem)
			{
				g.DrawLine(new Pen(brush), left - 9f, height + 2f, left + width - 1f, height + 2f);
			}
			PointF location = new PointF(left + 15f, 5f);
			RectangleF layoutRectangle = stripRect;
			layoutRectangle.Location = location;
			layoutRectangle.Width = width - (layoutRectangle.Left - left) - 4f;
			if (currentItem == this.selectedItem)
			{
				layoutRectangle.Width -= 15f;
			}
			layoutRectangle.Height = 23f;
			if (currentItem == this.SelectedItem)
			{
				g.DrawString(currentItem.Title, font, new SolidBrush(this.ForeColor), layoutRectangle, this.sf);
			}
			else
			{
				g.DrawString(currentItem.Title, font, new SolidBrush(this.ForeColor), layoutRectangle, this.sf);
			}
			currentItem.IsDrawn = true;
		}

		private void UpdateLayout()
		{
			this.sf.Trimming = StringTrimming.EllipsisCharacter;
			this.sf.FormatFlags |= StringFormatFlags.NoWrap;
			this.sf.FormatFlags &= StringFormatFlags.DirectionRightToLeft;
			this.stripButtonRect = new Rectangle(0, 0, base.ClientSize.Width - 40 - 2, 10);
			base.DockPadding.Top = 29;
			base.DockPadding.Bottom = 1;
			base.DockPadding.Right = 1;
			base.DockPadding.Left = 1;
		}

		private void OnCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			FATabStripItem fatabStripItem = (FATabStripItem)e.Element;
			if (e.Action == CollectionChangeAction.Add)
			{
				this.Controls.Add(fatabStripItem);
				this.OnTabStripItemChanged(new TabStripItemChangedEventArgs(fatabStripItem, FATabStripItemChangeTypes.Added));
			}
			else if (e.Action == CollectionChangeAction.Remove)
			{
				this.Controls.Remove(fatabStripItem);
				this.OnTabStripItemChanged(new TabStripItemChangedEventArgs(fatabStripItem, FATabStripItemChangeTypes.Removed));
			}
			else
			{
				this.OnTabStripItemChanged(new TabStripItemChangedEventArgs(fatabStripItem, FATabStripItemChangeTypes.Changed));
			}
			this.UpdateLayout();
			base.Invalidate();
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(null)]
		public FATabStripItem SelectedItem
		{
			get
			{
				return this.selectedItem;
			}
			set
			{
				if (this.selectedItem == value)
				{
					return;
				}
				if (value == null && this.Items.Count > 0)
				{
					FATabStripItem fatabStripItem = this.Items[0];
					if (fatabStripItem.Visible)
					{
						this.selectedItem = fatabStripItem;
						this.selectedItem.Selected = true;
						this.selectedItem.Dock = DockStyle.Fill;
					}
				}
				else
				{
					this.selectedItem = value;
				}
				foreach (object obj in this.Items)
				{
					FATabStripItem fatabStripItem2 = (FATabStripItem)obj;
					if (fatabStripItem2 == this.selectedItem)
					{
						this.SelectItem(fatabStripItem2);
						fatabStripItem2.Dock = DockStyle.Fill;
						fatabStripItem2.Show();
					}
					else
					{
						this.UnSelectItem(fatabStripItem2);
						fatabStripItem2.Hide();
					}
				}
				this.SelectItem(this.selectedItem);
				base.Invalidate();
				if (!this.selectedItem.IsDrawn)
				{
					this.Items.MoveTo(0, this.selectedItem);
					base.Invalidate();
				}
				this.OnTabStripItemChanged(new TabStripItemChangedEventArgs(this.selectedItem, FATabStripItemChangeTypes.SelectionChanged));
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FATabStripItemCollection Items
		{
			get
			{
				return this.items;
			}
		}

		[DefaultValue(typeof(Size), "350,200")]
		public new Size Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				if (base.Size == value)
				{
					return;
				}
				base.Size = value;
				this.UpdateLayout();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Control.ControlCollection Controls
		{
			get
			{
				return base.Controls;
			}
		}

		public bool ShouldSerializeFont()
		{
			return this.Font != null && !this.Font.Equals(FATabStrip.defaultFont);
		}

		public bool ShouldSerializeSelectedItem()
		{
			return true;
		}

		public bool ShouldSerializeItems()
		{
			return this.items.Count > 0;
		}

		public new void ResetFont()
		{
			this.Font = FATabStrip.defaultFont;
		}

		public void BeginInit()
		{
			this.isIniting = true;
		}

		public void EndInit()
		{
			this.isIniting = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.items.CollectionChanged -= this.OnCollectionChanged;
				this.menu.ItemClicked -= this.OnMenuItemClicked;
				this.menu.VisibleChanged -= this.OnMenuVisibleChanged;
				foreach (object obj in this.items)
				{
					FATabStripItem fatabStripItem = (FATabStripItem)obj;
					if (fatabStripItem != null && !fatabStripItem.IsDisposed)
					{
						fatabStripItem.Dispose();
					}
				}
				if (this.menu != null && !this.menu.IsDisposed)
				{
					this.menu.Dispose();
				}
				if (this.sf != null)
				{
					this.sf.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private const int TEXT_LEFT_MARGIN = 15;

		private const int TEXT_RIGHT_MARGIN = 10;

		private const int DEF_HEADER_HEIGHT = 28;

		private const int DEF_BUTTON_HEIGHT = 28;

		private const int DEF_GLYPH_WIDTH = 40;

		private int DEF_START_POS = 10;

		private Rectangle stripButtonRect = Rectangle.Empty;

		private FATabStripItem selectedItem;

		private ContextMenuStrip menu;

		private FATabStripCloseButton closeButton;

		private FATabStripItemCollection items;

		private StringFormat sf;

		private static Font defaultFont = new Font("Tahoma", 8.25f, FontStyle.Regular);

		private bool isIniting;

		private bool menuOpen;

		public int MaxTabSize = 200;

		public int AddButtonWidth = 40;
	}
}
