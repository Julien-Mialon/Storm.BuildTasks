﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Storm.BuildTasks.Common.Extensions;

namespace Storm.BuildTasks.AndroidColors
{
	public class AndroidColorsTask : Task
    {
	    private const string LINK_METADATA_NAME = "Link";
		protected readonly List<string> OutputResourceFilePath = new List<string>();

		[Required]
	    public ITaskItem[] InputFiles { get; set; }
		
	    [Output]
	    public ITaskItem[] OutputResourceFiles { get; set; }

		public override bool Execute()
	    {
			Dictionary<string, List<InputFile>> files = new Dictionary<string, List<InputFile>>();

		    if (InputFiles == null || InputFiles.Length == 0)
		    {
			    return true;
		    }

		    Log.LogMessage(MessageImportance.High, $"Storm.BuildTasks.AndroidColors: Processing file ({this.GetType().Name})");
		    foreach (ITaskItem inputFile in InputFiles)
		    {
			    string absoluteFilePath = inputFile.ItemSpec;
			    string projectFilePath = inputFile.GetOrDefaultMetadata(LINK_METADATA_NAME, absoluteFilePath);

			    string directory = Path.GetDirectoryName(projectFilePath) ?? string.Empty;

			    if (!files.ContainsKey(directory))
			    {
				    files.Add(directory, new List<InputFile>());
			    }
			    Log.LogMessage(MessageImportance.High, $"\t- {projectFilePath}");
			    files[directory].Add(new InputFile
				{
				    AbsoluteFilePath = absoluteFilePath,
				    ProjectFilePath = projectFilePath,
				    Directory = directory,
				    Content = new CSharpFileReader().Read(File.ReadAllText(absoluteFilePath))
			    });
		    }

			foreach (string directory in files.Keys)
			{
				List<InputFile> inputs = files[directory];
				if (Validation.ValidateDependency(inputs, LogError) && 
					Validation.ValidateUniqueNames(inputs, LogError) &&
				    Validation.ValidateCircularDependencies(inputs, LogError))
				{
					GenerateColors(directory, files[directory]);
				}
			}
		    
		    OutputResourceFiles = OutputResourceFilePath.Select(x => (ITaskItem)new TaskItem(x)).ToArray();

		    return !Log.HasLoggedErrors;
		}

	    protected void LogError(string errorMessage)
	    {
		    Log.LogError(errorMessage);
	    }

	    protected virtual void GenerateColors(string directory, List<InputFile> files)
	    {
			XmlDocument document = new XmlDocument();
		    document.AppendChild(document.CreateXmlDeclaration("1.0", "utf-8", null));
		    XmlNode rootNode = document.CreateElement("resources");
		    rootNode.AppendChild(document.CreateComment("This file was generated by Colors task for Android"));
		    document.AppendChild(rootNode);

		    foreach (var entries in files.SelectMany(x => x.Content.Entries))
		    {
			    XmlNode elementNode = document.CreateElement("color");
			    elementNode.InnerText = entries.ToAndroidColor();

			    XmlAttribute attributeName = document.CreateAttribute("name");
			    attributeName.Value = entries.Name;
			    elementNode.Attributes.Append(attributeName);
				
			    rootNode.AppendChild(elementNode);
		    }

		    string filepath = Path.Combine(directory, "colors.xml");
		    document.SaveIfDifferent(filepath);
		    OutputResourceFilePath.Add(filepath);
		}
    }
}
