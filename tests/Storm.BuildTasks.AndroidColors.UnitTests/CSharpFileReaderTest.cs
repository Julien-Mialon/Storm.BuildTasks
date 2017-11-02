using NFluent;
using Xunit;

namespace Storm.BuildTasks.AndroidColors.UnitTests
{
	public class CSharpFileReaderTest
	{
		[Fact]
		public void TestIntValues()
		{
			string input = @"namespace X
{
	public static class Colors
	{
		public const int White = 0xFFFFFF;

		public const int Black = 0x000000;

		public const int Red = 0xFF0000;
		public const int Green = 0x00FF00;
		public const int Blue = 0x0000FF;
	}
}";

			CSharpFileReader reader = new CSharpFileReader();

			Check.ThatCode(() => reader.Read(input))
				.WhichResult()
				.ContainsPair("White", "#FFFFFF").And
				.ContainsPair("Black", "#000000").And
				.ContainsPair("Red", "#FF0000").And
				.ContainsPair("Green", "#00FF00").And
				.ContainsPair("Blue", "#0000FF").And
				.HasSize(5);
		}

		[Fact]
		public void TestUintValuesWithAlpha()
		{
			string input = @"namespace X
{
	public static class Colors
	{
		public const uint Blue = 0xFF0000FF;
		public const uint AlphaBlue = 0x800000FF;
		public const uint LightAlphaBlue = 0x400000FF;
		public const uint NearZeroAlphaBlue = 0x080000FF;
	}
}";

			CSharpFileReader reader = new CSharpFileReader();

			Check.ThatCode(() => reader.Read(input))
				.WhichResult()
				.ContainsPair("Blue", "#FF0000FF").And
				.ContainsPair("AlphaBlue", "#800000FF").And
				.ContainsPair("LightAlphaBlue", "#400000FF").And
				.ContainsPair("NearZeroAlphaBlue", "#080000FF").And
				.HasSize(4);
		}

		[Fact]
		public void TestUintValuesWithoutAlpha()
		{
			string input = @"namespace X
{
	public static class Colors
	{
		public const uint WithoutAlphaRed = 0xFF0000;
	}
}";

			CSharpFileReader reader = new CSharpFileReader();

			Check.ThatCode(() => reader.Read(input))
				.WhichResult()
				.ContainsPair("WithoutAlphaRed", "#FF0000").And
				.HasSize(1);
		}

		[Fact]
		public void TestVariableNamesOrdered()
		{
			string input = @"namespace X
{
	public static class Colors
	{
		public const int White = 0xFFFFFF;
		public const int OtherWhite = White;
	}
}";

			CSharpFileReader reader = new CSharpFileReader();

			Check.ThatCode(() => reader.Read(input))
				.WhichResult()
				.ContainsPair("White", "#FFFFFF").And
				.ContainsPair("OtherWhite", "@color/White").And
				.HasSize(2);
		}
	}
}