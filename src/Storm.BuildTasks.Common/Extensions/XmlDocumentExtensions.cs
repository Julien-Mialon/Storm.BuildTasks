using System.IO;
using System.Text;
using System.Xml;
using Storm.BuildTasks.Common.Helpers;

namespace Storm.BuildTasks.Common.Extensions
{
	public static class XmlDocumentExtensions
	{
		public static void SaveIfDifferent(this XmlDocument document, string file)
		{
			string content = document.WriteToString();
			FileHelper.WriteIfDifferent(file, content);
		}

		public static string WriteToString(this XmlDocument document)
		{
			string result;
			using (MemoryStream outputStream = new MemoryStream())
			{
				using (XmlWriter writer = XmlWriter.Create(outputStream, new XmlWriterSettings
				{
					OmitXmlDeclaration = false,
					ConformanceLevel = ConformanceLevel.Document,
					Encoding = Encoding.UTF8,
					Indent = true,
					IndentChars = "\t"
				}))
				{
					document.Save(writer);
				}
				result = Encoding.UTF8.GetString(outputStream.ToArray());
			}
			
			string bomMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
			if (result.StartsWith(bomMarkUtf8))
			{
				result = result.Remove(0, bomMarkUtf8.Length);
			}
			return result.Replace("\0", "");
		}
	}
}