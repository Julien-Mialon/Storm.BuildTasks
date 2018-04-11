using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Build.Framework;

namespace Colors.Core
{
	public static class Extensions
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

		public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
		{
			if (source.ContainsKey(key))
			{
				return false;
			}

			source.Add(key, value);
			return true;
		}

		public static string GetOrDefaultMetadata(this ITaskItem item, string key, string defaultValue = null)
		{
			foreach (string metadataKey in item.MetadataNames)
			{
				if (key.Equals(metadataKey, StringComparison.InvariantCultureIgnoreCase))
				{
					return item.GetMetadata(metadataKey);
				}
			}
			return defaultValue;
		}

		public static void WriteToFile(this CodeCompileUnit code, string file, string comment)
		{
			CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
			CodeGeneratorOptions options = new CodeGeneratorOptions
			{
				BlankLinesBetweenMembers = false,
				BracingStyle = "C",
				IndentString = "\t"
			};

			string contentString;
			using (StringWriter stringWriter = new StringWriter())
			{
				provider.GenerateCodeFromCompileUnit(code, stringWriter, options);

				string content = stringWriter.GetStringBuilder().ToString();

				Regex commentRegex = new Regex("<auto-?generated>.*</auto-?generated>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				contentString = commentRegex.Replace(content, comment);
			}

			FileHelper.WriteIfDifferent(file, contentString);
		}
	}
}