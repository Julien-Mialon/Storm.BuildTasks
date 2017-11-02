using NFluent;
using Xunit;

namespace Storm.BuildTasks.AndroidColors.UnitTests
{
	public class CSharpFileReaderTest
	{
		[Fact]
		public void TestSimpleFileWithConstants()
		{
			string input = @"namespace X
{
	public static class Colors
	{
		public const int White = 0xFFFFFF;

		public const int Black = 0x000000;

		public const int Red = 0xFF0000;
		public const int Blue = 0x0000FF;

		public const uint AlphaBlue = 0x800000FF;
		public const uint WithoutAlphaRed = 0xFF0000;

		public const int PseudoWhite = White;
		public const uint PseudoBlue = AlphaBlue;
	}
}";

			CSharpFileReader reader = new CSharpFileReader();

			Check.ThatCode(() => reader.Read(input))
				.WhichResult()
				.ContainsPair("White", "#FFFFFF").And
				.ContainsPair("Black", "#000000").And
				.ContainsPair("Red", "#FF0000").And
				.ContainsPair("Blue", "#0000FF").And
				.ContainsPair("AlphaBlue", "#800000FF").And
				.ContainsPair("WithoutAlphaRed", "#FF0000").And
				 .ContainsPair("PseudoWhite", "@color/White").And
				 .ContainsPair("PseudoBlue", "@color/AlphaBlue").And
				.HasSize(8);
		}
	}
}