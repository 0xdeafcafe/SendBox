using System.Collections.Generic;
using System.IO;
using Blamite.Blam;
using Blamite.Flexibility;
using Blamite.Flexibility.Settings;
using Blamite.IO;
using SendBox.XboxCommunication.Models;
using XBDMCommunicator;

namespace SendBox.XboxCommunication
{
	public class MemoryManager
	{
		// XBox Stuff
		private Xbdm _xbdm;

		// Relevant Stuff
		private const string ConsoleIp = "192.168.1.69";
		private const string MapFilePath = @"A:\Xbox\Games\Halo 3\Maps\Clean\riverworld.map";
		private IList<TagEntry> _tagEntries;

		// Blamite Stuff
		private ICacheFile _cacheFile;
		private EngineDescription _engineDescription;
		private IStreamManager _streamManager;
		private EngineDatabase _engineDatabase;

		public MemoryManager()
		{
			_xbdm = new Xbdm(ConsoleIp, true);

			InitalizeTags();
			LoadMap();
		}

		private void LoadMap()
		{
			using (var fileStream = File.OpenRead(MapFilePath))
			{
				_engineDatabase = XMLEngineDatabaseLoader.LoadDatabase("Formats/Engines.xml");
				var reader = new EndianReader(fileStream, Endian.BigEndian);
				_cacheFile = CacheFileLoader.LoadCacheFile(reader, _engineDatabase, out _engineDescription);
				_streamManager = new FileStreamManager(MapFilePath, reader.Endianness);
			}
		}

		private void InitalizeTags()
		{
			_tagEntries = new List<TagEntry>
			{
				new TagEntry
				{
					TagClass = "weap",
					TagName = "",
					TagAliasNames = new List<string>
					{
						"assault rifle",
						"assault gun"
					}
				}
			};
		}

		/// <summary>
		/// Parses the text command, into the memory object
		/// </summary>
		/// <param name="command">The command read from the inbound parse api</param>
		public void ParseTextCommand(string command)
		{
			command = CleanUpTextCommand(command);
		}

		/// <summary>
		/// Removes stuff from commands that shouldn't be there. (also sets it to lower, to make processing easier)
		/// </summary>
		/// <param name="command">The command to clean up</param>
		private static string CleanUpTextCommand(string command)
		{
			// remove tabs
			command = command.Replace("\t", "");

			// remove weird new-lines
			command = command.Replace("\r", "");

			// return cleaned command
			return command;
		}
	}
}
