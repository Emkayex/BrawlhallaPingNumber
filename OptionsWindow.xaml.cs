using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BrawlhallaPingNumber
{
	/// <summary>
	/// Interaction logic for OptionsWindow.xaml
	/// </summary>
	public partial class OptionsWindow : Window
	{
		PingOverlay _overlay;
		Config _config;

		public OptionsWindow(PingOverlay overlay)
		{
			_overlay = overlay;
			_config = new Config();
			_config.LoadConfig();

			InitializeComponent();
		}

		private void apply_settings()
		{
			_overlay.ping_addr = _config.PingAddr;
			_overlay.ping_update_interval_ms = _config.PingUpdateIntervalMs;
			_config.SaveConfig();
		}

		private void cancel_clicked(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void apply_clicked(object sender, RoutedEventArgs e)
		{
			this.apply_settings();
		}

		private void ok_clicked(object sender, RoutedEventArgs e)
		{
			this.apply_settings();
			this.Close();
		}

		private void server_location_checked(object sender, RoutedEventArgs e)
		{
			string addr = ((RadioButton)sender).Tag.ToString();
			_config.PingAddr = addr;
		}

		private void update_rate_checked(object sender, RoutedEventArgs e)
		{
			float rate = Convert.ToSingle(((RadioButton)sender).Tag.ToString());
			_config.PingUpdateIntervalMs = rate;
		}
	}
}
