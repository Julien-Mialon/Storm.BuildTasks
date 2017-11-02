namespace Storm.BuildTasks.AndroidColors
{
	public interface IEntry
	{
		string Name { get; }
		string ToAndroidColor();
	}

	public class Entry : IEntry
	{
		public Entry(string name, int color)
		{
			Name = name;
			Color = color;
		}

		public string Name { get; }

		public int Color { get; }

		public string ToAndroidColor()
		{
			return $"#{Color:X6}";
		}
	}

	public class AlphaEntry : IEntry
	{
		public AlphaEntry(string name, uint color)
		{
			Name = name;
			Color = color;
		}

		public string Name { get; }

		public uint Color { get; }

		public string ToAndroidColor()
		{
			return $"#{Color:X8}";
		}
	}

	public class LinkEntry : IEntry
	{
		public string Name { get; }

		public string Link { get; }

		public LinkEntry(string name, string link)
		{
			Name = name;
			Link = link;
		}

		public string ToAndroidColor()
		{
			return $"@color/{Link}";
		}
	}
}