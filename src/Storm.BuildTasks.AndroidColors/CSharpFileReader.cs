using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Storm.BuildTasks.AndroidColors.Entries;

namespace Storm.BuildTasks.AndroidColors
{
	public class CSharpFileReader
	{
		public FileReaderResult Read(string content)
		{
			SyntaxNode rootNode = CSharpSyntaxTree.ParseText(content).GetRoot();

			Dictionary<string, bool> nameDependenciesSatisfied = new Dictionary<string, bool>();

			List<IEntry> resultEntries = new List<IEntry>();

			foreach (var fieldDeclarationSyntax in rootNode.DescendantNodes().OfType<FieldDeclarationSyntax>())
			{
				if (fieldDeclarationSyntax.Declaration.Type is PredefinedTypeSyntax type &&
					(type.Keyword.ValueText == "int" || type.Keyword.ValueText == "uint" || type.Keyword.ValueText == "long"))
				{
					VariableDeclaratorSyntax declaration = fieldDeclarationSyntax.Declaration.Variables.First();

					string name = declaration.Identifier.ValueText;
					if (declaration.Initializer.Value is LiteralExpressionSyntax literalValue)
					{
						string text = literalValue.Token.Text;
						if (text.StartsWith("0x"))
						{
							text = text.Substring(2);
							if (uint.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint colorValue))
							{
								if (text.Length == 8)
								{
									nameDependenciesSatisfied[name] = true;
									resultEntries.Add(new ColorWithAlphaEntry(name, colorValue));
								}
								else if (text.Length == 6)
								{
									nameDependenciesSatisfied[name] = true;
									resultEntries.Add(new ColorEntry(name, (int) colorValue));
								}
								else if (text.Length == 3)
								{
									int r = (int) (colorValue & 0xF00) >> 8;
									r = r << 4 | r;
									int g = (int) (colorValue & 0x0F0) >> 4;
									g = g << 4 | g;
									int b = (int) (colorValue & 0x00F);
									b = b << 4 | b;

									int expandedColorValue = r << 16 | g << 8 | b;

									nameDependenciesSatisfied[name] = true;
									resultEntries.Add(new ColorEntry(name, expandedColorValue));
								}
							}
						}
						else
						{
							string value = literalValue.Token.ValueText;
							if (uint.TryParse(value, out uint colorValue))
							{
								if (colorValue > 0xFFFFFF)
								{
									nameDependenciesSatisfied[name] = true;
									resultEntries.Add(new ColorWithAlphaEntry(name, colorValue));
								}
								else
								{
									nameDependenciesSatisfied[name] = true;
									resultEntries.Add(new ColorEntry(name, (int) colorValue)); //int cast is safe since value must be lower than 0xFFFFFF
								}
							}
						}
					}
					else if (declaration.Initializer.Value is IdentifierNameSyntax identifier)
					{
						string value = identifier.Identifier.ValueText;

						if (!nameDependenciesSatisfied.ContainsKey(value))
						{
							nameDependenciesSatisfied[value] = false;
						}
						resultEntries.Add(new VariableNameEntry(name, value));
					}
				}
			}
			
			return new FileReaderResult(resultEntries, nameDependenciesSatisfied);
		}
	}
}