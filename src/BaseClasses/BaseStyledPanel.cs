using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace FarsiLibrary.Win.BaseClasses
{
	[ToolboxItem(false)]
	public class BaseStyledPanel : ContainerControl
	{
		public event EventHandler ThemeChanged;

		public BaseStyledPanel()
		{
			base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			base.SetStyle(ControlStyles.UserPaint, true);
		}

		protected override void OnSystemColorsChanged(EventArgs e)
		{
			base.OnSystemColorsChanged(e);
			this.UpdateRenderer();
			base.Invalidate();
		}

		protected virtual void OnThemeChanged(EventArgs e)
		{
			if (this.ThemeChanged != null)
			{
				this.ThemeChanged(this, e);
			}
		}

		private void UpdateRenderer()
		{
			if (!this.UseThemes)
			{
				BaseStyledPanel.renderer.ColorTable.UseSystemColors = true;
				return;
			}
			BaseStyledPanel.renderer.ColorTable.UseSystemColors = false;
		}

		[Browsable(false)]
		public ToolStripProfessionalRenderer ToolStripRenderer
		{
			get
			{
				return BaseStyledPanel.renderer;
			}
		}

		[Browsable(false)]
		[DefaultValue(true)]
		public bool UseThemes
		{
			get
			{
				return VisualStyleRenderer.IsSupported && VisualStyleInformation.IsSupportedByOS && Application.RenderWithVisualStyles;
			}
		}

		private static ToolStripProfessionalRenderer renderer = new ToolStripProfessionalRenderer();
	}
}
