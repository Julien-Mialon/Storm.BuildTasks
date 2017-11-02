using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Storm.BuildTasks.AndroidColors
{
	public class CSharpFileReader
	{
		public Dictionary<string, string> Read(string content)
		{
			SyntaxNode rootNode = CSharpSyntaxTree.ParseText(content).GetRoot();

			var keyList = new List<string>();

			return rootNode.DescendantNodes()
				.OfType<FieldDeclarationSyntax>()
				.Where(x =>
				{
					if (x.Declaration.Type is PredefinedTypeSyntax type && (type.Keyword.ValueText == "int" || type.Keyword.ValueText = "uint")
					{
						return true;
					}

					return false;
				})
				.Select<FieldDeclarationSyntax, IEntry>(x =>
				{
					var declaration = x.Declaration.Variables.First();

					string name = declaration.Identifier.ValueText;
					if (declaration.Initializer.Value is LiteralExpressionSyntax literalValue)
					{
						string value = literalValue.Token.ValueText;
						if (int.TryParse(value, out int colorCode))
						{
							keyList.Add(name);
							return new Entry(name, colorCode);
						}
						else if (int.TryParse(value, out uint colorCode))
						{
							keyList.Add(name);
							return new AlphaEntry(name, colorCode);
						}
					}
					else if (declaration.Initializer.Value is IdentifierNameSyntax identifier)
					{
						string value = identifier.Identifier.ValueText;

						if (keyList.Contains(value))
						{
							keyList.Add(name);
							return new LinkEntry(name, value);
						}
					}

					return null;
				})
				.Where(x => x != null)
				.ToDictionary(x => x.Name, x => x.ToAndroidColor());
		}
	}
}