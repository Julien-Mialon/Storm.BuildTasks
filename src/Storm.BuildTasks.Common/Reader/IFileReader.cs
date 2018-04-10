using System.Collections.Generic;

namespace Storm.BuildTasks.Common.Reader
{
	public interface IFileReader
	{
		bool HasSupportForFile(string file);

		Dictionary<string, string> ReadResourceFile(string file);
	}
}