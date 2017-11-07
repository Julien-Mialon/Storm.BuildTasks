using NFluent;
using Storm.BuildTasks.AndroidColors.Entries;
using Xunit;

namespace Storm.BuildTasks.AndroidColors.UnitTests
{
	public class VariableNameEntryTest
	{
		[Theory]
		[InlineData("Blue", "@color/Blue")]
		[InlineData("Red_ish", "@color/Red_ish")]
		[InlineData("green", "@color/green")]
		[InlineData("000000", "@color/000000")]
		public void TestToAndroidColor(string colorName, string colorValue)
		{
			VariableNameEntry color = new VariableNameEntry("Color", colorName);
			Check.ThatCode(() => color.ToAndroidColor())
				.WhichResult().IsEqualTo(colorValue);
		}
	}
}