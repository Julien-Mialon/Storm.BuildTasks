using NFluent;
using Storm.BuildTasks.AndroidColors.Entries;
using Xunit;

namespace Storm.BuildTasks.AndroidColors.UnitTests
{
	public class ColorWithAlphaEntryTest
	{
		[Theory]
		[InlineData(0x12ABCDEF, "#12ABCDEF")]
		[InlineData(0x08ABCDEF, "#08ABCDEF")]
		[InlineData(0x80ABCDEF, "#80ABCDEF")]
		[InlineData(0xFFABCDEF, "#FFABCDEF")]
		[InlineData(0x00ABCDEF, "#00ABCDEF")]
		public void TestToAndroidColor(uint colorCode, string colorValue)
		{
			ColorWithAlphaEntry color = new ColorWithAlphaEntry("Color", colorCode);
			Check.ThatCode(() => color.ToAndroidColor())
				.WhichResult().IsEqualTo(colorValue);
		}
	}
}