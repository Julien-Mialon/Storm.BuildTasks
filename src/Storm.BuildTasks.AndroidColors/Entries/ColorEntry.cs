namespace Storm.BuildTasks.AndroidColors.Entries
{
	public class ColorEntry : IEntry
	{
		public ColorEntry(string name, int color)
		{
			Name = name;
			Color = color;
		}

		public string Name { get; }

		public int Color { get; }

		public override bool Equals(object obj)
		{
			return obj is ColorEntry entry &&
				   Name == entry.Name &&
				   Color == entry.Color;
		}

		public string ToAndroidColor()
		{
			return $"#{Color:X6}";
		}
	}
}