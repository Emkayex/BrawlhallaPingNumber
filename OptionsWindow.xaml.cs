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

		private void ApplySettings()
		{
			_overlay.PingAddr = _config.PingAddr;
			_overlay.PingUpdateIntervalMs = _config.PingUpdateIntervalMs;
			_config.SaveConfig();
		}

		private void CancelClicked(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void ApplyClicked(object sender, RoutedEventArgs e)
		{
			this.ApplySettings();
		}

		private void OkClicked(object sender, RoutedEventArgs e)
		{
			this.ApplySettings();
			this.Close();
		}

		private void ServerLocationChecked(object sender, RoutedEventArgs e)
		{
			string addr = ((RadioButton)sender).Tag.ToString();
			_config.PingAddr = addr;
		}

		private void UpdateRateChecked(object sender, RoutedEventArgs e)
		{
			float rate = Convert.ToSingle(((RadioButton)sender).Tag.ToString());
			_config.PingUpdateIntervalMs = rate;
		}
	}
}
