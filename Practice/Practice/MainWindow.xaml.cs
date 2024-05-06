using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace Practice
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private string _leftImagePath;
		private string _rightImagePath;

		public MainWindow()
		{
			InitializeComponent();
		}


		[DllImport("CalculateLib.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void alignAndSaveImages(byte[] imageData1, int imageDataSize1, byte[] imageData2, int imageDataSize2, 
				out IntPtr alignedImageData1, out int alignedImageDataSize1, out IntPtr alignedImageData2, out int alignedImageDataSize2);

		private void Border_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{

				try
				{
					this.DragMove();
				}
				catch (Exception)
				{

				}
			}
		}

		private void ButtonExit_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}

		private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
		{
			Window window = Window.GetWindow(this);
			if (window != null)
			{
				window.WindowState = WindowState.Minimized;
			}
		}

		private void leftImage_MouseLeftButtonDown(object sender, RoutedEventArgs e)
		{
			OpenFileDialog op = new OpenFileDialog();
			op.Title = "Select a picture";
			op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
			  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
			  "Portable Network Graphic (*.png)|*.png";
			if (op.ShowDialog() == true)
			{
				_leftImagePath = op.FileName;
				leftImage.Source = new BitmapImage(new Uri(_leftImagePath));
			}
		}

		private void leftImageBorder_MouseEnter(object sender, MouseEventArgs e)
		{
			// Увеличение прозрачности иконки
			DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));
			leftHoverIcon.BeginAnimation(OpacityProperty, fadeInAnimation);

			// Отображение иконки
			leftHoverIcon.Visibility = Visibility.Visible;
		}

		private void leftImageBorder_MouseLeave(object sender, MouseEventArgs e)
		{
			// Уменьшение прозрачности иконки
			DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));
			leftHoverIcon.BeginAnimation(OpacityProperty, fadeOutAnimation);

			// Скрытие иконки
			leftHoverIcon.Visibility = Visibility.Collapsed;
		}

		private void rightImage_MouseLeftButtonDown(object sender, RoutedEventArgs e)
		{
			OpenFileDialog op = new OpenFileDialog();
			op.Title = "Select a picture";
			op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
			  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
			  "Portable Network Graphic (*.png)|*.png";
			if (op.ShowDialog() == true)
			{
				_rightImagePath = op.FileName;
				rightImage.Source = new BitmapImage(new Uri(_rightImagePath));
			}
		}

		private void rightImageBorder_MouseEnter(object sender, MouseEventArgs e)
		{
			// Увеличение прозрачности иконки
			DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));
			rightHoverIcon.BeginAnimation(OpacityProperty, fadeInAnimation);

			// Отображение иконки
			rightHoverIcon.Visibility = Visibility.Visible;
		}

		private void rightImageBorder_MouseLeave(object sender, MouseEventArgs e)
		{
			// Уменьшение прозрачности иконки
			DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));
			rightHoverIcon.BeginAnimation(OpacityProperty, fadeOutAnimation);

			// Скрытие иконки
			rightHoverIcon.Visibility = Visibility.Collapsed;
		}

		private void ButtonCalc_Click(object sender, RoutedEventArgs e)
		{

			loadIcon.Visibility = Visibility.Visible;

			byte[] imageData1 = ConvertImageToByteArray(_leftImagePath);
			byte[] imageData2 = ConvertImageToByteArray(_rightImagePath);

			IntPtr alignedImageData1Ptr, alignedImageData2Ptr;
			int alignedImageDataSize1, alignedImageDataSize2;
			alignAndSaveImages(imageData1, imageData1.Length, imageData2, imageData2.Length, out alignedImageData1Ptr, out alignedImageDataSize1, out alignedImageData2Ptr, out alignedImageDataSize2);

			byte[] alignedImageData1 = new byte[alignedImageDataSize1];
			Marshal.Copy(alignedImageData1Ptr, alignedImageData1, 0, alignedImageDataSize1);
			byte[] alignedImageData2 = new byte[alignedImageDataSize2];
			Marshal.Copy(alignedImageData2Ptr, alignedImageData2, 0, alignedImageDataSize2);

			BitmapImage leftImageBmp = ByteArrayToBitmapImage(alignedImageData1);
			BitmapImage rightImageBmp = ByteArrayToBitmapImage(alignedImageData2);

			leftImage.Source = leftImageBmp;
			rightImage.Source = rightImageBmp;

			loadIcon.Visibility = Visibility.Hidden;
		}

		public byte[] ConvertImageToByteArray(string imagePath)
		{
			byte[] byteArray;
			using (FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					byteArray = binaryReader.ReadBytes((int)fileStream.Length);
				}
			}

			return byteArray;
		}

		public static BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
		{
			using (MemoryStream stream = new MemoryStream(byteArray))
			{
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.StreamSource = stream;
				bitmapImage.EndInit();
				bitmapImage.Freeze(); // Необходимо для использования в многопоточной среде
				return bitmapImage;
			}
		}
	}
}