namespace Data
{
	using System;
	using System.ComponentModel;
	using System.Threading;

	public class MapInfo : INotifyPropertyChanged
	{
		private string _name;
		public string author;
		public string desciptionExtended;
		public string descriptionBasic;
		public int edgeBottom;
		public int edgeLeft;
		public int edgeRight;
		public int edgeTop;
		public string filePath;
		public string filePath2;
		public int height;
		public int playableX;
		public int playableY;
		public event PropertyChangedEventHandler PropertyChanged;
		public int secondsElapsed;
		public int width;

		public event PropertyChangedEventHandler ePropertyChanged
		{
			add
			{
				PropertyChangedEventHandler handler2;
				PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
				do
				{
					handler2 = propertyChanged;
					PropertyChangedEventHandler handler3 = (PropertyChangedEventHandler) Delegate.Combine(handler2, value);
					propertyChanged = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this.PropertyChanged, handler3, handler2);
				}
				while (propertyChanged != handler2);
			}
			remove
			{
				PropertyChangedEventHandler handler2;
				PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
				do
				{
					handler2 = propertyChanged;
					PropertyChangedEventHandler handler3 = (PropertyChangedEventHandler) Delegate.Remove(handler2, value);
					propertyChanged = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this.PropertyChanged, handler3, handler2);
				}
				while (propertyChanged != handler2);
			}
		}

		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
				this.OnPropertyChanged("name");
			}
		}
	}
}

