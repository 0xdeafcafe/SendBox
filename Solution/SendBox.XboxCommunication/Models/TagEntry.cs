using System.Collections.Generic;

namespace SendBox.XboxCommunication.Models
{
	public class TagEntry
	{
		public string TagClass { get; set; }

		public string TagName { get; set; }

		public IList<string> TagAliasNames { get; set; }
	}
}
