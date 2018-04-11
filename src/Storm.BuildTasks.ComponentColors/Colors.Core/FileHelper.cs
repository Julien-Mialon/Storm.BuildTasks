using System;
using System.Collections.Generic;
using System.IO;

namespace Colors.Core
{
	public static class FileHelper
	{
		public static void WriteIfDifferent(string file, string content)
		{
			if (File.Exists(file))
			{
				using (StreamReader reader = new StreamReader(file))
				{
					string actualContent = reader.ReadToEnd();
					if (actualContent == content)
					{
						return;
					}
				}
				File.Delete(file);
			}

			using (StreamWriter writer = new StreamWriter(File.OpenWrite(file)))
			{
				writer.Write(content);
			}
		}
		
		public static void WriteIfDifferent(string file, IEnumerable<string> contentLines)
		{
			WriteIfDifferent(file, string.Join(Environment.NewLine, contentLines));
		}		
	}
}