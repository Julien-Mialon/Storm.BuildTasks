using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text.RegularExpressions;
using Storm.BuildTasks.Common.Helpers;

namespace Storm.BuildTasks.Common.Extensions
{
	public static class CodeCompileUnitExtensions
	{
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