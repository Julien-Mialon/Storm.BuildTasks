namespace Storm.BuildTasks.AndroidColors.Entries
{
	public interface IEntry
	{
		string Name { get; }

		string ToAndroidColor();
	}
}