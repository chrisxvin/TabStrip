using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace FarsiLibrary.Win.Design
{
	public class FATabStripDesigner : ParentControlDesigner
	{
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			this.changeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
			this.changeService.ComponentRemoving += this.OnRemoving;
			this.Verbs.Add(new DesignerVerb("Add TabStrip", new EventHandler(this.OnAddTabStrip)));
			this.Verbs.Add(new DesignerVerb("Remove TabStrip", new EventHandler(this.OnRemoveTabStrip)));
		}

		protected override void Dispose(bool disposing)
		{
			this.changeService.ComponentRemoving -= this.OnRemoving;
			base.Dispose(disposing);
		}

		private void OnRemoving(object sender, ComponentEventArgs e)
		{
			IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			if (e.Component is FATabStripItem)
			{
				FATabStripItem fatabStripItem = e.Component as FATabStripItem;
				if (this.Control.Items.Contains(fatabStripItem))
				{
					this.changeService.OnComponentChanging(this.Control, null);
					this.Control.RemoveTab(fatabStripItem);
					this.changeService.OnComponentChanged(this.Control, null, null, null);
					return;
				}
			}
			if (e.Component is FATabStrip)
			{
				for (int i = this.Control.Items.Count - 1; i >= 0; i--)
				{
					FATabStripItem fatabStripItem2 = this.Control.Items[i];
					this.changeService.OnComponentChanging(this.Control, null);
					this.Control.RemoveTab(fatabStripItem2);
					designerHost.DestroyComponent(fatabStripItem2);
					this.changeService.OnComponentChanged(this.Control, null, null, null);
				}
			}
		}

		private void OnAddTabStrip(object sender, EventArgs e)
		{
			IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			DesignerTransaction designerTransaction = designerHost.CreateTransaction("Add TabStrip");
			FATabStripItem fatabStripItem = (FATabStripItem)designerHost.CreateComponent(typeof(FATabStripItem));
			this.changeService.OnComponentChanging(this.Control, null);
			this.Control.AddTab(fatabStripItem);
			fatabStripItem.Title = "TabStrip Page " + (this.Control.Items.IndexOf(fatabStripItem) + 1).ToString();
			this.Control.SelectItem(fatabStripItem);
			this.changeService.OnComponentChanged(this.Control, null, null, null);
			designerTransaction.Commit();
		}

		private void OnRemoveTabStrip(object sender, EventArgs e)
		{
			IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			DesignerTransaction designerTransaction = designerHost.CreateTransaction("Remove Button");
			this.changeService.OnComponentChanging(this.Control, null);
			FATabStripItem fatabStripItem = this.Control.Items[this.Control.Items.Count - 1];
			this.Control.UnSelectItem(fatabStripItem);
			this.Control.Items.Remove(fatabStripItem);
			this.changeService.OnComponentChanged(this.Control, null, null, null);
			designerTransaction.Commit();
		}

		protected override bool GetHitTest(Point point)
		{
			return this.Control.HitTest(point) == HitTestResult.CloseButton;
		}

		protected override void PreFilterProperties(IDictionary properties)
		{
			base.PreFilterProperties(properties);
			properties.Remove("DockPadding");
			properties.Remove("DrawGrid");
			properties.Remove("Margin");
			properties.Remove("Padding");
			properties.Remove("BorderStyle");
			properties.Remove("ForeColor");
			properties.Remove("BackColor");
			properties.Remove("BackgroundImage");
			properties.Remove("BackgroundImageLayout");
			properties.Remove("GridSize");
			properties.Remove("ImeMode");
		}

		protected override void WndProc(ref Message msg)
		{
			if (msg.Msg == 513)
			{
				Point pt = this.Control.PointToClient(Cursor.Position);
				FATabStripItem tabItemByPoint = this.Control.GetTabItemByPoint(pt);
				if (tabItemByPoint != null)
				{
					this.Control.SelectedItem = tabItemByPoint;
					ArrayList arrayList = new ArrayList();
					arrayList.Add(tabItemByPoint);
					ISelectionService selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
					selectionService.SetSelectedComponents(arrayList);
				}
			}
			base.WndProc(ref msg);
		}

		public override ICollection AssociatedComponents
		{
			get
			{
				return this.Control.Items;
			}
		}

		public new virtual FATabStrip Control
		{
			get
			{
				return base.Control as FATabStrip;
			}
		}

		private IComponentChangeService changeService;
	}
}
