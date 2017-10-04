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
		private class Entry
		{
			public Entry(string name, int color)
			{
				Name = name;
				Color = color;
			}

			public string Name { get; }

			public int Color { get; }
		}

		public Dictionary<string, string> Read(string content)
		{
			SyntaxNode rootNode = CSharpSyntaxTree.ParseText(content).GetRoot();

			return rootNode.DescendantNodes()
				.OfType<FieldDeclarationSyntax>()
				.Where(x =>
				{
					if (x.Declaration.Type is PredefinedTypeSyntax type && type.Keyword.ValueText == "int")
					{
						return true;
					}

					return false;
				}).Select(x =>
				{
					var declaration = x.Declaration.Variables.First();

					string name = declaration.Identifier.ValueText;
					if (declaration.Initializer.Value is LiteralExpressionSyntax literalValue)
					{
						string value = literalValue.Token.ValueText;
						if (int.TryParse(value, out int colorCode))
						{
							return new Entry(name, colorCode);
						}
					}

					return null;
				}).Where(x => x != null)
				.ToDictionary(x => x.Name, x => $"#{x.Color:X6}");
		}
	}
}