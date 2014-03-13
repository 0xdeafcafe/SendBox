using System.Collections.Generic;
using Blamite.Blam;

namespace SendBox.XboxCommunication.Models
{
	public class TagEntry
	{
		public string TagClass { get; set; }

		public string TagName { get; set; }

		public ITag Tag { get; set; }

		public IList<string> TagAliasNames { get; set; }
	}
}
