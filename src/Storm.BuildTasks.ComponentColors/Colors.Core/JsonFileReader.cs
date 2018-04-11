using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Colors.Core
{
	public interface IFileReader
	{
		bool HasSupportForFile(string file);

		Dictionary<string, string> ReadResourceFile(string file);
	}
	public class JsonFileReader : IFileReader
	{
		public bool HasSupportForFile(string file)
		{
			string extension = Path.GetExtension(file)?.TrimStart('.').ToLowerInvariant() ?? string.Empty;
			return extension == "json";
		}

		public Dictionary<string, string> ReadResourceFile(string file)
		{
			if (JToken.Parse(File.ReadAllText(file)) is JObject result)
			{
				return result.Properties().ToDictionary(x => x.Name, x => x.Value.ToString());
			}
			else
			{
				return new Dictionary<string, string>();
			}
		}
	}
}