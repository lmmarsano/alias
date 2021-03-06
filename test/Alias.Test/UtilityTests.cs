using SIO = System.IO;
using Xunit;
using A = Alias;
using ST = LMMarsano.SumType;

namespace Alias.Test {
	public class UtilityTests {
		public static TheoryData<string> ValidateFileNameAcceptsData { get; }
		= new TheoryData<string>
		  { @"-"
		  , @"name"
		  };
		public static TheoryData<string?> ValidateFileNameRejectsData { get; }
		= new TheoryData<string?>
		  { (string?)null
		  , @""
		  , Utility.DirectorySeparator
		  , @"."
		  , @".."
		  };
		public static TheoryData<string> ValidatePathAcceptsData { get; }
		= new TheoryData<string>
		  { @"-"
		  , @"name"
		  , SIO.Path.Combine(@"path", @"name")
		  , @"."
		  , @".."
		  };
		public static TheoryData<string?> ValidatePathRejectsData { get; }
		= new TheoryData<string?>
		  { (string?)null
		  , @""
		  };
		[ Theory
		, MemberData(nameof(ValidateFileNameAcceptsData))
		]
		public void ValidateFileNameAccepts(string fileName)
		=> Assert.IsType<ST.Ok<string>>(A.Utility.ValidateFileName(fileName));
		[ Theory
		, MemberData(nameof(ValidateFileNameRejectsData))
		]
		public void ValidateFileNameRejects(string fileName)
		=> Assert.IsType<ST.Error<string>>(A.Utility.ValidateFileName(fileName));
		[ Theory
		, MemberData(nameof(ValidatePathAcceptsData))
		]
		public void ValidatePathAccepts(string fileName)
		=> Assert.IsType<ST.Ok<string>>(A.Utility.ValidatePath(fileName));
		[ Theory
		, MemberData(nameof(ValidatePathRejectsData))
		]
		public void ValidatePathRejects(string fileName)
		=> Assert.IsType<ST.Error<string>>(A.Utility.ValidatePath(fileName));
		public static TheoryData<string, string> SafeQuoteData { get; }
		= new TheoryData<string, string>
		  { {@""" """, @" "}
		  , {@"\""", @""""}
		  , {@"\", @"\"}
		  , {@""" \""""", @" """}
		  , {@"\\\""", @"\"""}
		  , {@""" \\""", @" \"}
		  , {@"""\ \\""", @"\ \"}
		  };
		[ Theory
		, MemberData(nameof(SafeQuoteData))
		]
		public void SafeQuoteTest(string expected, string input) => Assert.Equal(expected, A.Utility.SafeQuote(input));
	}
}
