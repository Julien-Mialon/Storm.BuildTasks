using System.Collections.Generic;
using NFluent;
using Storm.BuildTasks.AndroidColors.Entries;
using Xunit;

namespace Storm.BuildTasks.AndroidColors.UnitTests
{
	public class ValidationTest
	{
		[Fact]
		public void TestUniqueNames()
		{
			Check.ThatCode(() => Validation.ValidateUniqueNames(new List<InputFile>
			{
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new ColorEntry("White", 0xFFFFFF),
						new ColorEntry("Black", 0x000000)
					}, new Dictionary<string, bool>
					{
						["White"] = true,
						["Black"] = true
					})
				}
			}, _ => { })).WhichResult().IsTrue();
		}

		[Fact]
		public void TestInvalidUniqueNamesFromOneFile()
		{
			Check.ThatCode(() => Validation.ValidateUniqueNames(new List<InputFile>
			{
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new ColorEntry("White", 0xFFFFFF),
						new ColorEntry("White", 0x000000)
					}, new Dictionary<string, bool>
					{
						["White"] = true
					})
				}
			}, _ => { })).WhichResult().IsFalse();
		}

		[Fact]
		public void TestInvalidUniqueNamesFromMultipleFiles()
		{
			Check.ThatCode(() => Validation.ValidateUniqueNames(new List<InputFile>
			{
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new ColorEntry("White", 0xFFFFFF),
						new ColorEntry("Black", 0x000000)
					}, new Dictionary<string, bool>
					{
						["White"] = true,
						["Black"] = true
					})
				},
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new ColorEntry("White", 0xFFFFFF),
					}, new Dictionary<string, bool>
					{
						["White"] = true
					})
				}
			}, _ => { })).WhichResult().IsFalse();
		}

		[Fact]
		public void TestDependency()
		{
			Check.ThatCode(() => Validation.ValidateDependency(new List<InputFile>
			{
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new ColorEntry("White", 0xFFFFFF),
						new ColorEntry("Black", 0x000000)
					}, new Dictionary<string, bool>
					{
						["White"] = true,
						["Black"] = true
					})
				}
			}, _ => { })).WhichResult().IsTrue();
		}

		[Fact]
		public void TestDependencyWithValidLink()
		{
			Check.ThatCode(() => Validation.ValidateDependency(new List<InputFile>
			{
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new ColorEntry("White", 0xFFFFFF),
						new VariableNameEntry("OtherWhite", "White")
					}, new Dictionary<string, bool>
					{
						["White"] = true,
						["OtherWhite"] = true
					})
				}
			}, _ => { })).WhichResult().IsTrue();
		}

		[Fact]
		public void TestDependencyWithInvalidLink()
		{
			Check.ThatCode(() => Validation.ValidateDependency(new List<InputFile>
			{
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new VariableNameEntry("OtherWhite", "White")
					}, new Dictionary<string, bool>
					{
						["White"] = false,
						["OtherWhite"] = true
					})
				}
			}, _ => { })).WhichResult().IsFalse();
		}

		[Fact]
		public void TestDependencyWithValidLinkFromMultipleFiles()
		{
			Check.ThatCode(() => Validation.ValidateDependency(new List<InputFile>
			{
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new VariableNameEntry("OtherWhite", "White")
					}, new Dictionary<string, bool>
					{
						["White"] = false,
						["OtherWhite"] = true
					})
				},
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new ColorEntry("White", 0xFFFFFF),
					}, new Dictionary<string, bool>
					{
						["White"] = true,
					})
				}
			}, _ => { })).WhichResult().IsTrue();
		}

		[Fact]
		public void TestDependencyWithInvalidLinkFromMultipleFiles()
		{
			Check.ThatCode(() => Validation.ValidateDependency(new List<InputFile>
			{
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new VariableNameEntry("OtherWhite", "White")
					}, new Dictionary<string, bool>
					{
						["White"] = false,
						["OtherWhite"] = true
					})
				},
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new ColorEntry("Black", 0x000000),
					}, new Dictionary<string, bool>
					{
						["Black"] = true,
					})
				}
			}, _ => { })).WhichResult().IsFalse();
		}

		[Fact]
		public void TestDependencyWithValidLinkFromMultipleFilesUnordered()
		{
			Check.ThatCode(() => Validation.ValidateDependency(new List<InputFile>
			{
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new ColorEntry("White", 0xFFFFFF),
					}, new Dictionary<string, bool>
					{
						["White"] = true,
					})
				},
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new VariableNameEntry("OtherWhite", "White")
					}, new Dictionary<string, bool>
					{
						["White"] = false,
						["OtherWhite"] = true
					})
				}
			}, _ => { })).WhichResult().IsTrue();
		}

		[Fact]
		public void TestInvalidCircularDependency()
		{
			Check.ThatCode(() => Validation.ValidateCircularDependencies(new List<InputFile>
			{
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new VariableNameEntry("White", "OtherWhite"),
						new VariableNameEntry("OtherWhite", "White")
					}, new Dictionary<string, bool>
					{
						["OtherWhite"] = true,
						["White"] = true,
					})
				}
			}, _ => { })).WhichResult().IsFalse();
		}

		[Fact]
		public void TestValidDepthDependency()
		{
			Check.ThatCode(() => Validation.ValidateCircularDependencies(new List<InputFile>
			{
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new ColorEntry("White", 0xFFFFFF),
						new VariableNameEntry("OtherWhite", "White"),
						new VariableNameEntry("O1therWhite", "OtherWhite"),
						new VariableNameEntry("O2therWhite", "O1therWhite"),
						new VariableNameEntry("O3therWhite", "O2therWhite"),
						new VariableNameEntry("O4therWhite", "O3therWhite")
					}, new Dictionary<string, bool>
					{
						["White"] = true,
						["OtherWhite"] = true,
						["O1therWhite"] = true,
						["O2therWhite"] = true,
						["O3therWhite"] = true,
						["O4therWhite"] = true,
					})
				}
			}, _ => { })).WhichResult().IsTrue();
		}

		[Fact]
		public void TestInvalidCircularDependencyAccrossFiles()
		{
			Check.ThatCode(() => Validation.ValidateCircularDependencies(new List<InputFile>
			{
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new VariableNameEntry("White", "OtherWhite")
					}, new Dictionary<string, bool>
					{
						["OtherWhite"] = false,
						["White"] = true,
					})
				},
				new InputFile
				{
					Content = new FileReaderResult(new List<IEntry>
					{
						new VariableNameEntry("OtherWhite", "White")
					}, new Dictionary<string, bool>
					{
						["OtherWhite"] = true,
						["White"] = false,
					})
				}
			}, _ => { })).WhichResult().IsFalse();
		}
	}
}