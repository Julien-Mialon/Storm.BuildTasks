namespace Storm.BuildTasks.AndroidColors.Entries
{
	public class VariableNameEntry : IEntry
	{
		public string Name { get; }

		public string Link { get; }

		public VariableNameEntry(string name, string link)
		{
			Name = name;
			Link = link;
		}

		public string ToAndroidColor()
		{
			return $"@color/{Link}";
		}

		public override bool Equals(object obj)
		{
			return obj is VariableNameEntry entry &&
				   Name == entry.Name &&
				   Link == entry.Link;
		}
	}
}