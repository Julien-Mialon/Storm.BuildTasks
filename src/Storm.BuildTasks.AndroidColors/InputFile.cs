using System.Collections.Generic;

namespace Storm.BuildTasks.AndroidColors
{
	public class InputFile
	{
		public string AbsoluteFilePath { get; set; }

		public string ProjectFilePath { get; set; }

		public string Directory { get; set; }

		public FileReaderResult Content { get; set; }
	}
}