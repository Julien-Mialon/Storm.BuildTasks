using NFluent;
using Storm.BuildTasks.AndroidColors.Entries;
using Xunit;

namespace Storm.BuildTasks.AndroidColors.UnitTests
{
	public class ColorEntryTest
	{
		[Theory]
		[InlineData(0xFF0000, "#FF0000")]
		[InlineData(0xA00000, "#A00000")]
		[InlineData(0x070000, "#070000")]
		[InlineData(0x004400, "#004400")]
		[InlineData(0x002300, "#002300")]
		[InlineData(0x000065, "#000065")]
		[InlineData(0x123456, "#123456")]
		[InlineData(0xABCDEF, "#ABCDEF")]
		public void TestToAndroidColor(int colorCode, string colorValue)
		{
			ColorEntry color = new ColorEntry("Color", colorCode);
			Check.ThatCode(() => color.ToAndroidColor())
				.WhichResult().IsEqualTo(colorValue);
		}
	}
}