using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blamite.Blam;
using Blamite.Flexibility;
using Blamite.Flexibility.Settings;
using Blamite.IO;
using Blamite.Util;
using SendBox.XboxCommunication.Models;
using XBDMCommunicator;

namespace SendBox.XboxCommunication
{
	public class MemoryManager
	{
		// XBox Stuff
		private Xbdm _xbdm;

		// Relevant Stuff
		private const string ConsoleIp = "192.168.1.86";
		private const string MapFilePath = @"A:\Xbox\Games\Halo 3\Maps\Clean\riverworld.map";
		private IList<TagEntry> _tagEntries;
		private string _appLocation;

		// Blamite Stuff
		private ICacheFile _cacheFile;
		private EngineDescription _engineDescription;
		private IStreamManager _streamManager;
		private EngineDatabase _engineDatabase;

		// Blam Engine (Halo's Game Engine) Stuff

		public MemoryManager(string appLocation)
		{
			_xbdm = new Xbdm(ConsoleIp, true);
			_appLocation = appLocation;

			LoadMap();
			InitalizeTags();
		}

		private void LoadMap()
		{
			using (var fileStream = File.OpenRead(MapFilePath))
			{
				_engineDatabase = XMLEngineDatabaseLoader.LoadDatabase(Path.Combine(_appLocation, @"bin/Formats/Engines.xml"));
				var reader = new EndianReader(fileStream, Endian.BigEndian);
				_cacheFile = CacheFileLoader.LoadCacheFile(reader, _engineDatabase, out _engineDescription);
				_streamManager = new FileStreamManager(MapFilePath, reader.Endianness);

				var weapons =
					_cacheFile.Tags.Where(t => t.Class.Magic == CharConstant.FromString("weap"))
						.Select(t => _cacheFile.FileNames.GetTagName(t.Index));
				var projectiles =
					_cacheFile.Tags.Where(t => t.Class.Magic == CharConstant.FromString("proj"))
						.Select(t => _cacheFile.FileNames.GetTagName(t.Index));
			}
		}

		private void InitalizeTags()
		{
			_tagEntries = new List<TagEntry>
			{
				new TagEntry
				{
					TagClass = "weap",
					TagName = @"objects\weapons\rifle\assault_rifle\assault_rifle",
					Tag = _cacheFile.Tags.FirstOrDefault(
						t => t.Class.Magic == CharConstant.FromString("weap") && _cacheFile.FileNames.GetTagName(t.Index) == @"objects\weapons\rifle\assault_rifle\assault_rifle"),
					TagAliasNames = new List<string>
					{
						"assault rifle",
						"assault gun"
					}
				},
				new TagEntry
				{
					TagClass = "proj",
					TagName = @"objects\weapons\support_high\rocket_launcher\projectiles\rocket",
					Tag = _cacheFile.Tags.FirstOrDefault(
						t => t.Class.Magic == CharConstant.FromString("proj") && _cacheFile.FileNames.GetTagName(t.Index) == @"objects\weapons\support_high\rocket_launcher\projectiles\rocket"),
					TagAliasNames = new List<string>
					{
						"rockets",
						"rpg",
						"missile",
						"rocket"
					}
				},
				new TagEntry
				{
					TagClass = "proj",
					TagName = @"objects\vehicles\banshee\weapon\banshee_bomb",
					Tag = _cacheFile.Tags.FirstOrDefault(
						t => t.Class.Magic == CharConstant.FromString("proj") && _cacheFile.FileNames.GetTagName(t.Index) == @"objects\vehicles\banshee\weapon\banshee_bomb"),
					TagAliasNames = new List<string>
					{
						"banshee bomb"
					}
				},
				new TagEntry
				{
					TagClass = "proj",
					TagName = @"objects\vehicles\wraith\turrets\mortar\mortar_turret_charged",
					Tag = _cacheFile.Tags.FirstOrDefault(
						t => t.Class.Magic == CharConstant.FromString("proj") && _cacheFile.FileNames.GetTagName(t.Index) == @"objects\vehicles\wraith\turrets\mortar\mortar_turret_charged"),
					TagAliasNames = new List<string>
					{
						"wraith mortar",
						"wraith bomb",
						"mortar"
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
			var advCommand = CleanUpTextCommand(command);
			
			if (advCommand.Length != 1) return;

			BasicProjectileSwap(advCommand[0]);
		}

		#region Commands

		private void BasicProjectileSwap(string command)
		{
			command = command.ToLowerInvariant();

			var targetWeaponTag = _tagEntries.FirstOrDefault(t => t.TagClass == "weap" && t.TagAliasNames.Contains("assault rifle"));
			if (targetWeaponTag == null) return;

			var newProjectileTag = _tagEntries.FirstOrDefault(t => t.TagClass == "proj" && t.TagAliasNames.Contains(command));
			if (newProjectileTag == null) return;

			const uint baseOffset = 0xbf126690 + 0x98; // Tag Block Address + Initial Projectile Offset Gain
			var writer = new EndianWriter(_xbdm.MemoryStream, Endian.BigEndian);
			writer.SeekTo(baseOffset);
			writer.WriteUInt32((UInt32) newProjectileTag.Tag.Class.Magic);
			writer.WriteBlock(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
			writer.WriteUInt32(newProjectileTag.Tag.Index.Value);
		}

		#endregion

		/// <summary>
		/// Removes stuff from commands that shouldn't be there. (also sets it to lower, to make processing easier)
		/// </summary>
		/// <param name="command">The command to clean up</param>
		private static string[] CleanUpTextCommand(string command)
		{
			// remove tabs
			command = command.Replace("\t", "");

			// remove weird new-lines
			command = command.Replace("\r", "");

			// Trim Whitespace
			var advCommands = command.Split('\n');
			for (var i = 0; i < advCommands.Length; i++)
				advCommands[0] = advCommands[0].Trim();

			// return cleaned command
			return advCommands;
		}
	}
}
