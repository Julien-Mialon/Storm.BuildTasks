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

				Dictionary<string, uint> strings = GenerateColors(item.Key, item.Value, overrideFile, keySet);
				GenerateForDirectory(item.Key, strings);
			}

			List<string> keys = keySet.ToList();
			GenerateForProject(keys);
		}

		protected virtual Dictionary<string, uint> GenerateColors(string directory, List<ColorFile> inputFiles, List<ColorFile> overrideFiles, HashSet<string> keys)
		{
			var content = new Dictionary<string, uint>();
			var platformSpecificContent = new Dictionary<string, uint>();

			foreach (var file in inputFiles)
			{
				foreach (var item in file.Content)
				{
					if (item.Key.IsPlatformSpecificString())
					{
						string simplifiedKey = item.Key.SimplifyKey();
						if (IsCurrentPlatformKey(item.Key))
						{
							if (!platformSpecificContent.TryAdd(simplifiedKey, ProcessValue(item.Value)) && !CanHaveDuplicatedKeys)
							{
								Log.LogError($"Duplicated key (key: {item.Key}, file: {file.AbsoluteFilePath})");
							}
						}

						keys.Add(simplifiedKey);
					}
					else
					{
						if (!content.TryAdd(item.Key, ProcessValue(item.Value)) && !CanHaveDuplicatedKeys)
						{
							Log.LogError($"Duplicated key (key: {item.Key}, file: {file.AbsoluteFilePath})");
						}

						keys.Add(item.Key);
					}
				}
			}

			foreach (var file in overrideFiles)
			{
				foreach (var item in file.Content)
				{
					if (item.Key.IsPlatformSpecificString())
					{
						string simplifiedKey = item.Key.SimplifyKey();
						if (keys.Add(simplifiedKey))
						{
							Log.LogError($"Can not add new key using override file (key: {item.Key}, file: {file.AbsoluteFilePath})");
						}

						if (IsCurrentPlatformKey(item.Key))
						{
							platformSpecificContent[simplifiedKey] = ProcessValue(item.Value);
						}
					}
					else
					{
						if (keys.Add(item.Key))
						{
							Log.LogError($"Can not add new key using override file (key: {item.Key}, file: {file.AbsoluteFilePath})");
						}

						content[item.Key] = ProcessValue(item.Value);
					}
				}
			}

			foreach (var item in platformSpecificContent)
			{
				string key = item.Key;
				if (content.ContainsKey(key))
				{
					content[key] = item.Value;
				}
				else
				{
					content.Add(key, item.Value);
				}
			}

			return content;
		}

		protected virtual void GenerateForDirectory(string directory, Dictionary<string, uint> keyValues) { }

		protected virtual void GenerateForProject(List<string> keys) { }

		protected virtual void AfterGeneration() { }

		protected virtual void SetOutputVariables() { }

		protected virtual uint ProcessValue(string value)
		{
			//TODO 
//			uint.Parse(value);
			return 0;
		}

		protected virtual bool IsCurrentPlatformKey(string key) => false;

		protected virtual bool CanHaveDuplicatedKeys => false;
	}
}