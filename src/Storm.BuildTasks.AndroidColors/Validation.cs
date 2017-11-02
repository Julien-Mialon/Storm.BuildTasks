using System;
using System.Collections.Generic;
using System.Linq;
using Storm.BuildTasks.AndroidColors.Entries;

namespace Storm.BuildTasks.AndroidColors
{
    public static class Validation
    {
	    public static bool ValidateDependency(List<InputFile> files, Action<string> logError)
	    {
		    Dictionary<string, bool> overallDependencies = new Dictionary<string, bool>();
		    foreach (InputFile file in files)
		    {
			    foreach (KeyValuePair<string, bool> dependency in file.Content.Dependencies)
			    {
				    if (overallDependencies.TryGetValue(dependency.Key, out bool currentValue))
				    {
					    if (dependency.Value && !currentValue)
					    {
						    overallDependencies[dependency.Key] = true;
					    }
				    }
				    else
				    {
					    overallDependencies[dependency.Key] = dependency.Value;
				    }
			    }
		    }

		    List<KeyValuePair<string, bool>> missingDependencies = overallDependencies.Where(x => !x.Value).ToList();
		    if (missingDependencies.Count > 0)
		    {
			    logError($"Missing variables for files {string.Join(", ", files.Select(x => x.ProjectFilePath))}");

			    foreach (KeyValuePair<string, bool> missingDependency in missingDependencies)
			    {
				    logError($"\t Missing variable: {missingDependency.Key}");
			    }

			    return false;
		    }

		    return true;
	    }

	    public static bool ValidateCircularDependencies(List<InputFile> files, Action<string> logError)
	    {
		    HashSet<string> validatedNames = new HashSet<string>();
		    List<IEntry> entries = files.SelectMany(x => x.Content.Entries).ToList();
			List<VariableNameEntry> variableEntries = new List<VariableNameEntry>();

		    foreach (IEntry entry in entries)
		    {
			    if (entry is VariableNameEntry variableEntry)
			    {
				    variableEntries.Add(variableEntry);
			    }
			    else
			    {
				    validatedNames.Add(entry.Name);
			    }
		    }

		    if (variableEntries.Count == 0)
		    {
			    return true;
		    }

		    while (true)
		    {
			    int initialCount = variableEntries.Count;

			    for (int i = 0; i < variableEntries.Count; ++i)
			    {
				    if (validatedNames.Contains(variableEntries[i].Link))
				    {
					    validatedNames.Add(variableEntries[i].Name);
					    variableEntries.RemoveAt(i);
					    i--;
				    }
			    }

			    if (variableEntries.Count == 0)
			    {
				    return true;
			    }

			    if (initialCount == variableEntries.Count)
			    {
				    break;
			    }
		    }

		    if (variableEntries.Count > 0)
		    {
			    logError($"Circular dependency for files {string.Join(", ", files.Select(x => x.ProjectFilePath))}");

			    foreach (VariableNameEntry entry in variableEntries)
			    {
				    logError($"\t Circular dependency: {entry.Name} -> {entry.Link}");
			    }

			    return false;
		    }
		    return true;
	    }

	    public static bool ValidateUniqueNames(List<InputFile> files, Action<string> logError)
	    {
			HashSet<string> names = new HashSet<string>();
			HashSet<string> duplicatedNames = new HashSet<string>();

			foreach (InputFile file in files)
			{
				foreach (IEntry entry in file.Content.Entries)
				{
					if (names.Contains(entry.Name))
					{
						if (!duplicatedNames.Contains(entry.Name))
						{
							duplicatedNames.Add(entry.Name);
						}
					}
					else
					{
						names.Add(entry.Name);
					}
				}
			}

		    if (duplicatedNames.Count > 0)
		    {
			    logError($"Duplicated variable names for files {string.Join(", ", files.Select(x => x.ProjectFilePath))}");

			    foreach (string duplicatedName in duplicatedNames)
			    {
				    logError($"\t Duplicated name: {duplicatedName}");
			    }

			    return false;
		    }

		    return true;
	    }
	}
}
