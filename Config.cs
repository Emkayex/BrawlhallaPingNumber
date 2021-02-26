using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BrawlhallaPingNumber
{
	public class Config
	{
		public string PingAddr { get; set; }
		public float PingUpdateIntervalMs { get; set; }

		private string _configPath;

		public Config()
		{
			string[] paths = { Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BrawlhallaPingNumber", "config.json" };
			_configPath = Path.Join(paths);
		}

		public void SaveConfig()
		{
			// Ensure the necessary directory exists
			Directory.CreateDirectory(Path.GetDirectoryName(_configPath));

			// Serialize the config as JSON and write it to the _configPath
			string jsonText = JsonSerializer.Serialize(this);
			File.WriteAllText(_configPath, jsonText);
		}

		public void LoadConfig()
		{
			// Read the config from the disk
			if (File.Exists(_configPath))
			{
				string jsonText = File.ReadAllText(_configPath);
				var tempConfig = JsonSerializer.Deserialize<Config>(jsonText);

				// Set the members from the tempConfig
				PingAddr = tempConfig.PingAddr;
				PingUpdateIntervalMs = tempConfig.PingUpdateIntervalMs;
			}
			// If a previous config hadn't been written, write defaults
			else
			{
				PingAddr = "pingtest-atl.brawlhalla.com";
				PingUpdateIntervalMs = 3000.0F;
				SaveConfig();
			}
		}
	}
}
