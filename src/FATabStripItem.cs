using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FarsiLibrary.Win.Design;

namespace FarsiLibrary.Win
{
	[ToolboxItem(false)]
	[DefaultProperty("Title")]
	[DefaultEvent("Changed")]
	[Designer(typeof(FATabStripItemDesigner))]
	public class FATabStripItem : Panel
	{
		public event EventHandler Changed;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new Size Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				base.Size = value;
			}
		}

		[DefaultValue(true)]
		public new bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				if (this.visible == value)
				{
					return;
				}
				this.visible = value;
				this.OnChanged();
			}
		}

		internal RectangleF StripRect
		{
			get
			{
				return this.stripRect;
			}
			set
			{
				this.stripRect = value;
			}
		}

		[DefaultValue(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public bool IsDrawn
		{
			get
			{
				return this.isDrawn;
			}
			set
			{
				if (this.isDrawn == value)
				{
					return;
				}
				this.isDrawn = value;
			}
		}

		[DefaultValue(null)]
		public Image Image
		{
			get
			{
				return this.image;
			}
			set
			{
				this.image = value;
			}
		}

		[DefaultValue(true)]
		public bool CanClose
		{
			get
			{
				return this.canClose;
			}
			set
			{
				this.canClose = value;
			}
		}

		[DefaultValue("Name")]
		public string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				if (this.title == value)
				{
					return;
				}
				this.title = value;
				this.OnChanged();
			}
		}

		[DefaultValue(false)]
		[Browsable(false)]
		public bool Selected
		{
			get
			{
				return this.selected;
			}
			set
			{
				if (this.selected == value)
				{
					return;
				}
				this.selected = value;
			}
		}

		[Browsable(false)]
		public string Caption
		{
			get
			{
				return this.Title;
			}
		}

		public FATabStripItem() : this(string.Empty, null)
		{
		}

		public FATabStripItem(Control displayControl) : this(string.Empty, displayControl)
		{
		}

		public FATabStripItem(string caption, Control displayControl)
		{
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			base.SetStyle(ControlStyles.UserPaint, true);
			base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			base.SetStyle(ControlStyles.ContainerControl, true);
			this.selected = false;
			this.Visible = true;
			this.UpdateText(caption, displayControl);
			if (displayControl != null)
			{
				base.Controls.Add(displayControl);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && this.image != null)
			{
				this.image.Dispose();
			}
		}

		public bool ShouldSerializeIsDrawn()
		{
			return false;
		}

		public bool ShouldSerializeDock()
		{
			return false;
		}

		public bool ShouldSerializeControls()
		{
			return base.Controls != null && base.Controls.Count > 0;
		}

		public bool ShouldSerializeVisible()
		{
			return true;
		}

		private void UpdateText(string caption, Control displayControl)
		{
			if (displayControl != null && displayControl is ICaptionSupport)
			{
				ICaptionSupport captionSupport = displayControl as ICaptionSupport;
				this.Title = captionSupport.Caption;
				return;
			}
			if (caption.Length <= 0 && displayControl != null)
			{
				this.Title = displayControl.Text;
				return;
			}
			if (caption != null)
			{
				this.Title = caption;
				return;
			}
			this.Title = string.Empty;
		}

		public void Assign(FATabStripItem item)
		{
			this.Visible = item.Visible;
			this.Text = item.Text;
			this.CanClose = item.CanClose;
			base.Tag = item.Tag;
		}

		protected internal virtual void OnChanged()
		{
			if (this.Changed != null)
			{
				this.Changed(this, EventArgs.Empty);
			}
		}

		public override string ToString()
		{
			return this.Caption;
		}

		private RectangleF stripRect = Rectangle.Empty;

		private Image image;

		private bool canClose = true;

		private bool selected;

		private bool visible = true;

		private bool isDrawn;

		private string title = string.Empty;
	}
}
