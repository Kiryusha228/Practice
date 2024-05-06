using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Practice
{
	public class ImageSourceViewModel : INotifyPropertyChanged
	{
		private ImageSource _leftImageSource;

		public ImageSource LeftImageSource
		{
			get { return _leftImageSource; }
			set
			{
				_leftImageSource = value;
				OnPropertyChanged(nameof(LeftImageSource));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
