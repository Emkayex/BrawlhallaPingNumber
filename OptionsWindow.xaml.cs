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

			InitializeComponent();

			// Change the selected buttons on the overlay based on the _config
			_config.LoadConfig();

			// First set the server location
			foreach (var child in ServerLocationStackPanel.Children)
			{
				// If the tag text is equal to the PingAddr, then set that button as checked
				var childRadioButton = (RadioButton)child;
				if (childRadioButton.Tag.ToString() == _config.PingAddr)
				{
					childRadioButton.IsChecked = true;
				}
			}

			// Next set the update rate
			foreach (var child in UpdateRateStackPanel.Children)
			{
				var childRadioButton = (RadioButton)child;
				if (Convert.ToSingle(childRadioButton.Tag.ToString()) == _config.PingUpdateIntervalMs)
				{
					childRadioButton.IsChecked = true;
				}
			}
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
