using SIO = System.IO;

namespace Alias.Test.Fixture {
	public class FakeStream: SIO.MemoryStream {
		bool awaitingCopy = true;
		public SIO.Stream Stream { get; }
		public FakeStream(SIO.Stream stream) {
			stream.CopyTo(this);
			Stream = stream;
			Position = 0;
		}
		public override void Close() {
			if (awaitingCopy) {
				awaitingCopy = false;
				Position = Stream.Position = 0;
				Stream.SetLength(0);
				CopyTo(Stream);
			}
			base.Close();
		}
	}
}
