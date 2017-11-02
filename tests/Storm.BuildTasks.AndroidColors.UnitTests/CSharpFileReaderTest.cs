using NFluent;
using Storm.BuildTasks.AndroidColors.Entries;
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

			var readerResult = reader.Read(input);
			Check.That(readerResult.Entries).HasSize(5);
			Check.That(readerResult.Entries).ContainsExactly(
				new ColorEntry("White", 0xFFFFFF),
				new ColorEntry("Black", 0x000000),
				new ColorEntry("Red", 0xFF0000),
				new ColorEntry("Green", 0x00FF00),
				new ColorEntry("Blue", 0x0000FF));
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

			var readerResult = reader.Read(input);
			Check.That(readerResult.Entries).HasSize(4);
			Check.That(readerResult.Entries).ContainsExactly(
				new ColorWithAlphaEntry("Blue", 0xFF0000FF),
				new ColorWithAlphaEntry("AlphaBlue", 0x800000FF),
				new ColorWithAlphaEntry("LightAlphaBlue", 0x400000FF),
				new ColorWithAlphaEntry("NearZeroAlphaBlue", 0x080000FF));
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

			var readerResult = reader.Read(input);
			Check.That(readerResult.Entries).HasSize(1);
			Check.That(readerResult.Entries).ContainsExactly(
				new ColorEntry("WithoutAlphaRed", 0xFF0000));
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

			var readerResult = reader.Read(input);
			Check.That(readerResult.Entries).HasSize(2);
			Check.That(readerResult.Entries).ContainsExactly<IEntry>(
				new ColorEntry("White", 0xFFFFFF),
				new VariableNameEntry("OtherWhite", "White"));
		}

		[Fact]
		public void TestVariableNamesUnordered()
		{
			string input = @"namespace X
{
	public static class Colors
	{
		public const int OtherWhite = White;
		public const int White = 0xFFFFFF;
	}
}";

			CSharpFileReader reader = new CSharpFileReader();

			var readerResult = reader.Read(input);
			Check.That(readerResult.Entries).HasSize(2);
			Check.That(readerResult.Entries).ContainsExactly<IEntry>(
				new VariableNameEntry("OtherWhite", "White"),
				new ColorEntry("White", 0xFFFFFF));
		}
	}
}