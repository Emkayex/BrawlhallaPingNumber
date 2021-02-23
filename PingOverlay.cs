using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.Timers;

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

		private string ping_num;
		private string ping_addr;

		private double screen_height;
		private double screen_width;

		const float FONT_SIZE = 16.0F;
		const float PING_UPDATE_INTERVAL_MS = 3000.0F;

		public PingOverlay()
		{
			ping_addr = "pingtest-atl.brawlhalla.com";

			// Start a timer to update the ping number regularly
			UpdatePingNum(null, null);
			var ping_update_timer = new Timer(PING_UPDATE_INTERVAL_MS);
			ping_update_timer.Elapsed += UpdatePingNum;
			ping_update_timer.AutoReset = true;
			ping_update_timer.Enabled = true;

			screen_height = System.Windows.SystemParameters.PrimaryScreenHeight;
			screen_width = System.Windows.SystemParameters.PrimaryScreenWidth;

			_brushes = new Dictionary<string, SolidBrush>();
			_fonts = new Dictionary<string, Font>();
			_images = new Dictionary<string, Image>();

			var gfx = new Graphics()
			{
				MeasureFPS = true,
				PerPrimitiveAntiAliasing = true,
				TextAntiAliasing = true
			};

			_window = new GraphicsWindow(0, 0, (int)screen_width, (int)screen_height, gfx)
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
				PingReply reply = pingSender.Send(ping_addr, timeout, buffer, options);
				ping_num = reply.RoundtripTime.ToString();
			}
			catch (PingException)
			{
				ping_num = ">999";
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
				.Append("Ping: ").Append(ping_num).Append(" ms".PadRight(padding))
				.ToString();

			gfx.ClearScene(_brushes["background"]);

			// Determine where to draw the text
			var text_size = gfx.MeasureString(_fonts["consolas"], FONT_SIZE, infoText);
			float centered_text_x = ((float)screen_width / 2.0F) - (text_size.X / 2.0F);

			gfx.DrawTextWithBackground(_fonts["consolas"], _brushes["green"], _brushes["black"], centered_text_x, 20, infoText);
		}

		public void Run()
		{
			_window.Create();
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
