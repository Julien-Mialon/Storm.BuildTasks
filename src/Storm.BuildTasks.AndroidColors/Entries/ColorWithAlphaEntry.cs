namespace Storm.BuildTasks.AndroidColors.Entries
{
	public class ColorWithAlphaEntry : IEntry
	{
		public ColorWithAlphaEntry(string name, uint color)
		{
			Name = name;
			Color = color;
		}

		public string Name { get; }

		public uint Color { get; }

		public override bool Equals(object obj)
		{
			return obj is ColorWithAlphaEntry entry &&
				   Name == entry.Name &&
				   Color == entry.Color;
		}

		public string ToAndroidColor()
		{
			return $"#{Color:X8}";
		}
	}
}