using System.Collections.Generic;

namespace Colors.Core
{
	public class ColorFile
	{
		public string AbsoluteFilePath { get; set; }
		
		public string ProjectFilePath { get; set; }
		
		public string Directory { get; set; }
		
		public Dictionary<string, string> Content { get; set; }
	}
}