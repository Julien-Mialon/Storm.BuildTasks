using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Storm.BuildTasks.AndroidColors
{
	public class InputFile
	{
		public string AbsoluteFilePath { get; set; }

		public string ProjectFilePath { get; set; }

		public string Directory { get; set; }

		public Dictionary<string, string> Content { get; set; }
	}

	public static class TaskItemExtensions
	{
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
	}

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
			Dictionary<string, List<InputFile>> resxFiles = new Dictionary<string, List<InputFile>>();

		    if (InputFiles == null || InputFiles.Length == 0)
		    {
			    return true;
		    }

		    Log.LogMessage(MessageImportance.High, $"StormCrossLocalization: Processing strings file ({this.GetType().Name})");
		    foreach (ITaskItem inputFile in InputFiles)
		    {
			    string absoluteFilePath = inputFile.ItemSpec;
			    string projectFilePath = inputFile.GetOrDefaultMetadata(LINK_METADATA_NAME, absoluteFilePath);

			    string directory = Path.GetDirectoryName(projectFilePath) ?? string.Empty;

			    if (!resxFiles.ContainsKey(directory))
			    {
				    resxFiles.Add(directory, new List<InputFile>());
			    }
			    Log.LogMessage(MessageImportance.High, $"\t- {projectFilePath}");
			    resxFiles[directory].Add(new InputFile
				{
				    AbsoluteFilePath = absoluteFilePath,
				    ProjectFilePath = projectFilePath,
				    Directory = directory,
				    Content = ReadResourceFile(absoluteFilePath)
			    });
		    }
		    AfterRead();

		    BeforeGeneration();
		    Generate(resxFiles);
		    AfterGeneration();

		    SetOutputVariables();

		    OutputCompileFiles = OutputCompileFilePath.Select(x => (ITaskItem)new TaskItem(x)).ToArray();
		    OutputResourceFiles = OutputResourceFilePath.Select(x => (ITaskItem)new TaskItem(x)).ToArray();

		    return !Log.HasLoggedErrors;
		}
    }
}
