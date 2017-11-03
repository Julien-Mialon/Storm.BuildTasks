using System.Collections.Generic;
using Storm.BuildTasks.AndroidColors.Entries;

namespace Storm.BuildTasks.AndroidColors
{
	public class FileReaderResult
	{
		public List<IEntry> Entries { get; }

		public Dictionary<string, bool> Dependencies { get; }

		public FileReaderResult(List<IEntry> entries, Dictionary<string, bool> dependencies)
		{
			Entries = entries;
			Dependencies = dependencies;
		}
	}
}