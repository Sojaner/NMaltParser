using System.IO;

namespace NMaltParser.Core.Helper
{
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public sealed class NoOutputStream : Stream
	{
		public static readonly Stream DEVNULL = new NoOutputStream();

		private NoOutputStream()
		{
		}
		public void write(int b)
		{
		}
		public void write(sbyte[] b)
		{
		}
		public void write(sbyte[] b, int off, int len)
		{
		}
	}

}