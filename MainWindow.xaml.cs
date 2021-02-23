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
		PingOverlay _overlay;
		OptionsWindow _optionsWindow;

		public MainWindow()
		{
			InitializeComponent();

			// Create an unused OptionsWindow object to ensure the logic in ShowOptionsWindow() works on the first execution
			_optionsWindow = new OptionsWindow(_overlay);

			// Instantiate and display the ping overlay
			_overlay = new PingOverlay();
			_overlay.Run();
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
			if (!_optionsWindow.IsVisible)
			{
				// Create the option window object and display it
				_optionsWindow = new OptionsWindow(_overlay);
				_optionsWindow.Show();
				System.Windows.Threading.Dispatcher.Run();
			}
		}
	}
}
