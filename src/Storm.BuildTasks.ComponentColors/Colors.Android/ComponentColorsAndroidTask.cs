﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Colors.Core;

namespace Colors.Android
{
	public class ComponentColorsAndroidTask : BaseTask
	{
		protected override void GenerateForDirectory(string directory, Dictionary<string, string> keyValues)
		{
			//write colors.xml file
			XmlDocument document = new XmlDocument();
			document.AppendChild(document.CreateXmlDeclaration("1.0", "utf-8", null));
			XmlNode rootNode = document.CreateElement("resources");
			rootNode.AppendChild(document.CreateComment("This file was generated by ComponentColors task for Android"));
			document.AppendChild(rootNode);

			foreach (var pair in keyValues)
			{
				XmlNode elementNode = document.CreateElement("color");
				elementNode.InnerText = pair.Value;

				XmlAttribute attributeName = document.CreateAttribute("name");
				attributeName.Value = pair.Key;
				elementNode.Attributes.Append(attributeName);

				rootNode.AppendChild(elementNode);
			}

			string filepath = Path.Combine(directory, "colors.xml");
			document.SaveIfDifferent(filepath);
			OutputResourceFilePath.Add(filepath);

			base.GenerateForDirectory(directory, keyValues);
		}

		protected override void GenerateForProject(List<string> keys)
		{
			GenerateColorService(keys);
			GenerateColors(keys);

			base.GenerateForProject(keys);
		}

		protected virtual void GenerateColorService(List<string> keys)
		{
			Log.LogMessage($"Generate Color Service {keys.Count}");

			CodeCompileUnit codeUnit = new CodeCompileUnit();

			// add namespace
			var codeNamespace = new CodeNamespace(GenerationNamespace);
			codeUnit.Namespaces.Add(codeNamespace);

			codeNamespace.Imports.Add(new CodeNamespaceImport(DefaultNamespace));
			codeNamespace.Imports.Add(new CodeNamespaceImport("Android.Content"));
			codeNamespace.Imports.Add(new CodeNamespaceImport("Android.Graphics"));

			// create class
			var classDeclaration = new CodeTypeDeclaration(ColorConstants.IMPLEMENTATION_SERVICE_NAME)
			{
				IsClass = true,
				TypeAttributes = TypeAttributes.Public,
			};
			classDeclaration.BaseTypes.Add(ColorConstants.INTERFACE_SERVICE_NAME);
			codeNamespace.Types.Add(classDeclaration);

			//field
			var field = new CodeMemberField("Context", ColorConstants.CONTEXT_FIELD_NAME)
			{
				Attributes = MemberAttributes.Private
			};
			classDeclaration.Members.Add(field);

			//constructor
			var constructor = new CodeConstructor
			{
				Attributes = MemberAttributes.Public
			};
			constructor.Parameters.Add(new CodeParameterDeclarationExpression("Context", ColorConstants.CONTEXT_PARAMETER_NAME));
			constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), ColorConstants.CONTEXT_FIELD_NAME), new CodeVariableReferenceExpression(ColorConstants.CONTEXT_PARAMETER_NAME)));
			classDeclaration.Members.Add(constructor);

			//methode
			var method = new CodeMemberMethod
			{
				Name = ColorConstants.SERVICE_METHOD_NAME,
				ReturnType = new CodeTypeReference(typeof(uint)),
				Attributes = MemberAttributes.Public
			};
			method.Parameters.Add(new CodeParameterDeclarationExpression(ColorConstants.ENUM_NAME, "key"));
			classDeclaration.Members.Add(method);

			var methodParam = new CodeVariableReferenceExpression("key");
			var contextReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), ColorConstants.CONTEXT_FIELD_NAME);
			var getColor = new CodeMethodReferenceExpression(contextReference, "GetColor");

			var androidColorId = new CodeTypeReferenceExpression("Resource.Color");

			foreach (string key in keys)
			{
				CodeConditionStatement condition = new CodeConditionStatement(
					new CodeBinaryOperatorExpression(
						methodParam,
						CodeBinaryOperatorType.IdentityEquality,
						new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(ColorConstants.ENUM_NAME), key)
					),
					new CodeMethodReturnStatement(
						new CodeCastExpression(typeof(uint), new CodeMethodInvokeExpression(new CodeObjectCreateExpression("Color", new CodeMethodInvokeExpression(getColor, new CodePropertyReferenceExpression(androidColorId, key))), "ToArgb"))
					)
				);

				method.Statements.Add(condition);
			}

			method.Statements.Add(new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(ArgumentOutOfRangeException))));

			codeUnit.WriteToFile(ColorConstants.IMPLEMENTATION_SERVICE_FILE_PATH, "This file was generated by ComponentColors task for Android");
			OutputCompileFilePath.Add(ColorConstants.IMPLEMENTATION_SERVICE_FILE_PATH);
		}

		protected virtual void GenerateColors(List<string> keys)
		{
			var codeUnit = new CodeCompileUnit();

			// ajout namespace
			var codeNamespace = new CodeNamespace(GenerationNamespace);
			codeUnit.Namespaces.Add(codeNamespace);

			codeNamespace.Imports.Add(new CodeNamespaceImport(DefaultNamespace));
			codeNamespace.Imports.Add(new CodeNamespaceImport("Android.Content"));
			codeNamespace.Imports.Add(new CodeNamespaceImport("Android.Graphics"));

			// create class
			var classDeclaration = new CodeTypeDeclaration(ColorConstants.COLORS_NAME)
			{
				IsClass = true,
				TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed,
			};
			codeNamespace.Types.Add(classDeclaration);

			//field
			var field = new CodeMemberField("Context", ColorConstants.CONTEXT_FIELD_NAME)
			{
				Attributes = MemberAttributes.Private | MemberAttributes.Static
			};
			classDeclaration.Members.Add(field);

			//initialize method
			var initializeMethod = new CodeMemberMethod
			{
				Name = "Initialize",
				Attributes = MemberAttributes.Public | MemberAttributes.Static
			};
			initializeMethod.Parameters.Add(new CodeParameterDeclarationExpression("Context", ColorConstants.CONTEXT_PARAMETER_NAME));
			initializeMethod.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(ColorConstants.COLORS_NAME), ColorConstants.CONTEXT_FIELD_NAME), new CodeVariableReferenceExpression(ColorConstants.CONTEXT_PARAMETER_NAME)));
			classDeclaration.Members.Add(initializeMethod);

			//constructor
			var constructor = new CodeConstructor
			{
				Attributes = MemberAttributes.Private
			};
			classDeclaration.Members.Add(constructor);

			var contextReference = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(ColorConstants.COLORS_NAME), ColorConstants.CONTEXT_FIELD_NAME);
			var getColorMethod = new CodeMethodReferenceExpression(contextReference, "GetColor");
			var androidColorId = new CodeTypeReferenceExpression("Resource.Color");

			//properties
			foreach (string key in keys)
			{
				var property = new CodeMemberProperty
				{
					Name = key,
					Type = new CodeTypeReference("Color"),
					Attributes = MemberAttributes.Public | MemberAttributes.Static
				};

				property.GetStatements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression("Color", new CodeMethodInvokeExpression(getColorMethod, new CodePropertyReferenceExpression(androidColorId, key)))));
				classDeclaration.Members.Add(property);
			}

			codeUnit.WriteToFile(ColorConstants.COLORS_FILE_PATH, "This file was generated by ComponentColors task for Android");
			OutputCompileFilePath.Add(ColorConstants.COLORS_FILE_PATH);
		}
	}
}