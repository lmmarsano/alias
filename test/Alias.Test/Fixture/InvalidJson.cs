using S = System;
using System.Linq;
using Xunit;

namespace Alias.Test.Fixture {
	public class InvalidJson: TheoryData<string> {
		readonly string[] _data
		= new []
			{ string.Empty
			, "}"
			, "{"
			};
		public InvalidJson() {
			foreach (var item in _data) {
				Add(item);
			}
		}
	}
}