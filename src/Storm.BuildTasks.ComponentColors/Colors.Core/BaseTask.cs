using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Storm.BuildTasks.Common.Extensions;
using Storm.BuildTasks.Common.Reader;

namespace Colors.Core
{
	public abstract class BaseTask : Task
	{
		[Required]
		public ITaskItem[] InputFiles { get; set; }

		[Output]
		public ITaskItem[] OutputCompileFiles { get; set; }

		[Output]
		public ITaskItem[] OutputResourceFiles { get; set; }

		[Required]
		public string DefaultNamespace { get; set; }

		public string Namespace { get; set; }

		public ITaskItem[] OverrideFiles { get; set; }

		private const string LINK_METADATA_NAME = "Link";

		protected readonly List<string> OutputCompileFilePath = new List<string>();

		protected readonly List<string> OutputResourceFilePath = new List<string>();

		protected string GenerationNamespace
		{
			get
			{
				if (!string.IsNullOrEmpty(Namespace))
				{
					return Namespace;
				}

				if (string.IsNullOrEmpty(DefaultNamespace))
				{
					return "Storm.ComponentColors";
				}

				int index = DefaultNamespace.IndexOf('.');
				if (index < 0)
				{
					return DefaultNamespace;
				}

				return DefaultNamespace.Substring(0, index);
			}
		}

		public override bool Execute()
		{
			if (InputFiles == null || InputFiles.Length == 0)
			{
				return true;
			}

			Log.LogMessage(MessageImportance.High, $"Storm.BuildTasks.ComponentColors: Processing colors file ({GetType().Name})");
			BeforeRead();
			Dictionary<string, List<ColorFile>> colorFiles = ReadInputFiles(InputFiles);
			Dictionary<string, List<ColorFile>> overrideFiles = ReadInputFiles(OverrideFiles);
			AfterRead();

			BeforeGeneration();
			Generate(colorFiles, overrideFiles);
			AfterGeneration();

			SetOutputVariables();

			OutputCompileFiles = OutputCompileFilePath.Select(x => new TaskItem(x)).ToArray<ITaskItem>();
			OutputResourceFiles = OutputResourceFilePath.Select(x => new TaskItem(x)).ToArray<ITaskItem>();

			return !Log.HasLoggedErrors;
		}

		private Dictionary<string, string> ReadResourceFile(string file)
		{
			IFileReader reader = new JsonFileReader();
			if (reader == null)
			{
				throw new InvalidOperationException($"File {file} is not a supported format");
			}

			return reader.ReadResourceFile(file);
		}

		private Dictionary<string, List<ColorFile>> ReadInputFiles(ITaskItem[] inputs)
		{
			Dictionary<string, List<ColorFile>> result = new Dictionary<string, List<ColorFile>>();
			if (inputs == null)
			{
				return result;
			}

			foreach (ITaskItem inputFile in inputs)
			{
				string absoluteFilePath = inputFile.ItemSpec;
				string projectFilePath = inputFile.GetOrDefaultMetadata(LINK_METADATA_NAME, absoluteFilePath);

				string directory = Path.GetDirectoryName(projectFilePath) ?? string.Empty;

				if (!result.ContainsKey(directory))
				{
					result.Add(directory, new List<ColorFile>());
				}

				Log.LogMessage(MessageImportance.High, $"\t- {projectFilePath}");
				result[directory].Add(new ColorFile
				{
					AbsoluteFilePath = absoluteFilePath,
					ProjectFilePath = projectFilePath,
					Directory = directory,
					Content = ReadResourceFile(absoluteFilePath)
				});
			}

			return result;
		}

		protected virtual void BeforeRead() { }

		protected virtual void AfterRead() { }

		protected virtual void BeforeGeneration() { }

		protected virtual void Generate(Dictionary<string, List<ColorFile>> files, Dictionary<string, List<ColorFile>> overrideFiles)
		{
			HashSet<string> keySet = new HashSet<string>();

			foreach (KeyValuePair<string, List<ColorFile>> item in files)
			{
				if (!overrideFiles.TryGetValue(item.Key, out List<ColorFile> overrideFile))
				{
					overrideFile = new List<ColorFile>();
				}

				var colors = GenerateColors(item.Key, item.Value, overrideFile, keySet);
				GenerateForDirectory(item.Key, colors);
			}

			List<string> keys = keySet.ToList();
			GenerateForProject(keys);
		}

		protected virtual Dictionary<string, string> GenerateColors(string directory, List<ColorFile> inputFiles, List<ColorFile> overrideFiles, HashSet<string> keys)
		{
			var privateContent = new Dictionary<string, string>();
			var referenceContent = new Dictionary<string, string>();

			var content = new Dictionary<string, string>();

			foreach (var file in inputFiles)
			{
				foreach (var keyValue in file.Content)
				{
					var key = keyValue.Key;
					var colorString = keyValue.Value;
					if (colorString.StartsWith("#"))
					{
						if (!privateContent.TryAdd(key, colorString))
						{
							Log.LogError($"Can't have duplicated key {key}, file {file.AbsoluteFilePath}");
						}
					}
					else
					{
						if (!referenceContent.TryAdd(key, colorString))
						{
							Log.LogError($"Can't have duplicated key {key}, file {file.AbsoluteFilePath}");
						}
					}
				}
			}

			foreach (var file in overrideFiles)
			{
				foreach (var keyValue in file.Content)
				{
					var key = keyValue.Key;
					var colorString = keyValue.Value;
					if (colorString.StartsWith("#"))
					{
						if (privateContent.ContainsKey(key))
						{
							privateContent[key] = colorString;
						}
						else
						{
							Log.LogError($"Can't find original key for override {key}, file {file.AbsoluteFilePath}");
						}
					}
					else
					{
						if (referenceContent.ContainsKey(key))
						{
							referenceContent[key] = colorString;
						}
						else
						{
							Log.LogError($"Can't find original key for override {key}, file {file.AbsoluteFilePath}");
						}
					}
				}
			}

			foreach (var reference in referenceContent)
			{
				if (privateContent.TryGetValue(reference.Value, out var color))
				{
					content.Add(reference.Key, color);
				}
				else
				{
					Log.LogError($"Can't find color for reference {reference.Key}");
				}
			}

			return content;
		}

		protected virtual void GenerateForDirectory(string directory, Dictionary<string, string> keyValues) { }

		protected virtual void GenerateForProject(List<string> keys) { }

		protected virtual void AfterGeneration() { }

		protected virtual void SetOutputVariables() { }
	}
}