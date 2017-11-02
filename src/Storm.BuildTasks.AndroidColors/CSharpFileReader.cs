using System.Collections.Generic;
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
						string value = literalValue.Token.ValueText;
						if (uint.TryParse(value, out uint colorWithAlpha))
						{
							nameDependenciesSatisfied[name] = true;
							if (colorWithAlpha > 0xFFFFFF)
							{
								resultEntries.Add(new ColorWithAlphaEntry(name, colorWithAlpha));
							}
							else
							{
								resultEntries.Add(new ColorEntry(name, (int) colorWithAlpha)); //int cast is safe since value must be lower than 0xFFFFFF
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