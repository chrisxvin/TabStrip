using System;
using System.ComponentModel;
using FarsiLibrary.Win.Helpers;

namespace FarsiLibrary.Win
{
	public class FATabStripItemCollection : CollectionWithEvents
	{
		[Browsable(false)]
		public event CollectionChangeEventHandler CollectionChanged;

		public FATabStripItemCollection()
		{
			this.lockUpdate = 0;
		}

		public FATabStripItem this[int index]
		{
			get
			{
				if (index < 0 || base.List.Count - 1 < index)
				{
					return null;
				}
				return (FATabStripItem)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		[Browsable(false)]
		public virtual int DrawnCount
		{
			get
			{
				int count = base.Count;
				int num = 0;
				if (count == 0)
				{
					return 0;
				}
				for (int i = 0; i < count; i++)
				{
					if (this[i].IsDrawn)
					{
						num++;
					}
				}
				return num;
			}
		}

		public virtual FATabStripItem LastVisible
		{
			get
			{
				for (int i = base.Count - 1; i > 0; i--)
				{
					if (this[i].Visible)
					{
						return this[i];
					}
				}
				return null;
			}
		}

		public virtual FATabStripItem FirstVisible
		{
			get
			{
				for (int i = 0; i < base.Count; i++)
				{
					if (this[i].Visible)
					{
						return this[i];
					}
				}
				return null;
			}
		}

		[Browsable(false)]
		public virtual int VisibleCount
		{
			get
			{
				int count = base.Count;
				int num = 0;
				if (count == 0)
				{
					return 0;
				}
				for (int i = 0; i < count; i++)
				{
					if (this[i].Visible)
					{
						num++;
					}
				}
				return num;
			}
		}

		protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, e);
			}
		}

		protected virtual void BeginUpdate()
		{
			this.lockUpdate++;
		}

		protected virtual void EndUpdate()
		{
			if (--this.lockUpdate == 0)
			{
				this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
			}
		}

		public virtual void AddRange(FATabStripItem[] items)
		{
			this.BeginUpdate();
			try
			{
				foreach (FATabStripItem value in items)
				{
					base.List.Add(value);
				}
			}
			finally
			{
				this.EndUpdate();
			}
		}

		public virtual void Assign(FATabStripItemCollection collection)
		{
			this.BeginUpdate();
			try
			{
				base.Clear();
				for (int i = 0; i < collection.Count; i++)
				{
					FATabStripItem item = collection[i];
					FATabStripItem fatabStripItem = new FATabStripItem();
					fatabStripItem.Assign(item);
					this.Add(fatabStripItem);
				}
			}
			finally
			{
				this.EndUpdate();
			}
		}

		public virtual int Add(FATabStripItem item)
		{
			int num = this.IndexOf(item);
			if (num == -1)
			{
				num = base.List.Add(item);
			}
			return num;
		}

		public virtual void Remove(FATabStripItem item)
		{
			if (base.List.Contains(item))
			{
				base.List.Remove(item);
			}
		}

		public virtual FATabStripItem MoveTo(int newIndex, FATabStripItem item)
		{
			int num = base.List.IndexOf(item);
			if (num >= 0)
			{
				base.RemoveAt(num);
				this.Insert(0, item);
				return item;
			}
			return null;
		}

		public virtual int IndexOf(FATabStripItem item)
		{
			return base.List.IndexOf(item);
		}

		public virtual bool Contains(FATabStripItem item)
		{
			return base.List.Contains(item);
		}

		public virtual void Insert(int index, FATabStripItem item)
		{
			if (this.Contains(item))
			{
				return;
			}
			base.List.Insert(index, item);
		}

		protected override void OnInsertComplete(int index, object item)
		{
			FATabStripItem fatabStripItem = item as FATabStripItem;
			fatabStripItem.Changed += this.OnItem_Changed;
			this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}

		protected override void OnRemove(int index, object item)
		{
			base.OnRemove(index, item);
			FATabStripItem fatabStripItem = item as FATabStripItem;
			fatabStripItem.Changed -= this.OnItem_Changed;
			this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}

		protected override void OnClear()
		{
			if (base.Count == 0)
			{
				return;
			}
			this.BeginUpdate();
			try
			{
				for (int i = base.Count - 1; i >= 0; i--)
				{
					base.RemoveAt(i);
				}
			}
			finally
			{
				this.EndUpdate();
			}
		}

		protected virtual void OnItem_Changed(object sender, EventArgs e)
		{
			this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, sender));
		}

		private int lockUpdate;
	}
}
