using System;

namespace FarsiLibrary.Win
{
	public class TabStripItemChangedEventArgs : EventArgs
	{
		public TabStripItemChangedEventArgs(FATabStripItem item, FATabStripItemChangeTypes type)
		{
			this.changeType = type;
			this.itm = item;
		}

		public FATabStripItemChangeTypes ChangeType
		{
			get
			{
				return this.changeType;
			}
		}

		public FATabStripItem Item
		{
			get
			{
				return this.itm;
			}
		}

		private FATabStripItem itm;

		private FATabStripItemChangeTypes changeType;
	}
}
