using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace BrawlhallaPingNumber
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		OptionsWindow optionsWindow;

		public MainWindow()
		{
			InitializeComponent();

			// Create an unused OptionsWindow object to ensure the logic in ShowOptionsWindow() works on the first execution
			optionsWindow = new OptionsWindow();

			// Instantiate and display the ping overlay
			PingOverlay overlay = new PingOverlay();
			overlay.Run();
		}

		private void TrayIconClicked(object sender, RoutedEventArgs e)
		{
			// Use a separate thread to run the window otherwise it will freeze
			Application.Current.Dispatcher.Invoke((Action)(() =>
			{
				ShowOptionsWindow();
			}));
		}

		private void ShowOptionsWindow()
		{
			if (!optionsWindow.IsVisible)
			{
				// Create the option window object and display it
				optionsWindow = new OptionsWindow();
				optionsWindow.Show();
				System.Windows.Threading.Dispatcher.Run();
			}
		}
	}
}
