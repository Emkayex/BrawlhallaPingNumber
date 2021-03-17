using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.Timers;
using System.Threading;

using GameOverlay.Drawing;
using GameOverlay.Windows;

/*
 * US-E: pingtest-atl.brawlhalla.com
 * US-W: pingtest-cal.brawlhalla.com
 * EU: pingtest-ams.brawlhalla.com
 * SEA: pingtest-sgp.brawlhalla.com
 * AUS: pingtest-aus.brawlhalla.com
 * BRAZIL: pingtest-brs.brawlhalla.com
 * JAPAN: pingtest-jpn.brawlhalla.com
 */

namespace BrawlhallaPingNumber
{
	public class PingOverlay : IDisposable
	{
		private readonly GraphicsWindow _window;

		private readonly Dictionary<string, SolidBrush> _brushes;
		private readonly Dictionary<string, Font> _fonts;
		private readonly Dictionary<string, Image> _images;

		public string PingAddr;
		public float PingUpdateIntervalMs;

		private string _pingNum;

		private double _screenHeight;
		private double _screenWidth;

		const float FONT_SIZE = 16.0F;

		public PingOverlay()
		{
			// Load the settings from the config when starting the overlay
			var config = new Config();
			config.LoadConfig();
			PingAddr = config.PingAddr;
			PingUpdateIntervalMs = config.PingUpdateIntervalMs;

			// Start a timer to update the ping number regularly
			UpdatePingNum(null, null);
			var ping_update_timer = new System.Timers.Timer(PingUpdateIntervalMs);
			ping_update_timer.Elapsed += UpdatePingNum;
			ping_update_timer.AutoReset = false;
			ping_update_timer.Enabled = true;

			_screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
			_screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;

			_brushes = new Dictionary<string, SolidBrush>();
			_fonts = new Dictionary<string, Font>();
			_images = new Dictionary<string, Image>();

			var gfx = new Graphics()
			{
				MeasureFPS = true,
				PerPrimitiveAntiAliasing = true,
				TextAntiAliasing = true
			};

			_window = new GraphicsWindow(0, 0, (int)_screenWidth, (int)_screenHeight, gfx)
			{
				FPS = 15,
				IsTopmost = true,
				IsVisible = true
			};

			_window.DestroyGraphics += _window_DestroyGraphics;
			_window.DrawGraphics += _window_DrawGraphics;
			_window.SetupGraphics += _window_SetupGraphics;
		}

		private void UpdatePingNum(Object source, ElapsedEventArgs e)
		{
			Ping pingSender = new Ping();
			PingOptions options = new PingOptions();
			options.DontFragment = true;
			string data = "00000000000000000000000000000000";
			byte[] buffer = Encoding.ASCII.GetBytes(data);
			int timeout = 999;

			try
			{
				PingReply reply = pingSender.Send(PingAddr, timeout, buffer, options);
				var tempPingNum = reply.RoundtripTime.ToString();

				// The Microsoft Ping class is unusual and instead of throwing an error on timeout, it can skip a ping attempt, say it timed out, but return a time of 0
				// In that case, wait a short period and call the ping function again
				if (tempPingNum != "0")
				{
					_pingNum = tempPingNum;

					// When the ping is successful, schedule the next ping
					var ping_update_timer = new System.Timers.Timer(PingUpdateIntervalMs);
					ping_update_timer.Elapsed += UpdatePingNum;
					ping_update_timer.AutoReset = false;
					ping_update_timer.Enabled = true;
				}
				else
				{
					Thread.Sleep(500);
					UpdatePingNum(null, null);
				}
			}
			catch (PingException)
			{
				_pingNum = ">999";
			}
		}

		private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
		{
			var gfx = e.Graphics;

			if (e.RecreateResources)
			{
				foreach (var pair in _brushes) pair.Value.Dispose();
				foreach (var pair in _images) pair.Value.Dispose();
			}

			_brushes["black"] = gfx.CreateSolidBrush(0, 0, 0);
			_brushes["white"] = gfx.CreateSolidBrush(255, 255, 255);
			_brushes["red"] = gfx.CreateSolidBrush(255, 0, 0);
			_brushes["green"] = gfx.CreateSolidBrush(0, 255, 0);
			_brushes["blue"] = gfx.CreateSolidBrush(0, 0, 255);
			_brushes["background"] = gfx.CreateSolidBrush(0x00, 0x00, 0x00, 0x00);

			if (e.RecreateResources) return;

			_fonts["consolas"] = gfx.CreateFont("Consolas", FONT_SIZE);
		}

		private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
		{
			foreach (var pair in _brushes) pair.Value.Dispose();
			foreach (var pair in _fonts) pair.Value.Dispose();
			foreach (var pair in _images) pair.Value.Dispose();
		}

		private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
		{
			var gfx = e.Graphics;
			
			var padding = 16;
			var infoText = new StringBuilder()
				.Append("Ping: ").Append(_pingNum).Append(" ms".PadRight(padding))
				.ToString();

			gfx.ClearScene(_brushes["background"]);

			// Determine where to draw the text
			var text_size = gfx.MeasureString(_fonts["consolas"], FONT_SIZE, infoText);
			float centered_text_x = ((float)_screenWidth / 2.0F) - (text_size.X / 2.0F);

			gfx.DrawTextWithBackground(_fonts["consolas"], _brushes["green"], _brushes["black"], centered_text_x, 20, infoText);
		}

		public void Run()
		{
			_window.Create();
			_window.Join();
		}

		public void Close()
		{
			_window.Dispose();
			_window.Join();
		}

		~PingOverlay()
		{
			Dispose(false);
		}

		#region IDisposable Support
		private bool disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				_window.Dispose();

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
