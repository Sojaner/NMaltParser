using System.IO;

namespace org.maltparser.core.helper
{
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public sealed class NoPrintStream : PrintStream
	{
		public static readonly Stream NO_OUTPUTSTREAM = NoOutputStream.DEVNULL;
		public static readonly PrintStream NO_PRINTSTREAM = new NoPrintStream();
		private NoPrintStream() : base(NO_OUTPUTSTREAM)
		{
		}
		public override PrintStream append(char c)
		{
			return base.append(c);
		}

		public override PrintStream append(CharSequence csq, int start, int end)
		{
			return base.append(csq, start, end);
		}

		public override PrintStream append(CharSequence csq)
		{
			return base.append(csq);
		}

		public override bool checkError()
		{
			return base.checkError();
		}

		public override void close()
		{
			base.close();
		}

		public override void flush()
		{
			base.flush();
		}

		public override PrintStream format(Locale l, string format, params object[] args)
		{
			return base.format(l, format, args);
		}

		public override PrintStream format(string format, params object[] args)
		{
			return base.format(format, args);
		}

		public override void print(bool b)
		{
		}
		public override void print(char c)
		{
		}
		public override void print(char[] s)
		{
		}
		public override void print(double d)
		{
		}
		public override void print(float f)
		{
		}
		public override void print(int i)
		{
		}
		public override void print(long l)
		{
		}
		public override void print(object obj)
		{
		}
		public override void print(string s)
		{
		}
		public override PrintStream printf(Locale l, string format, params object[] args)
		{
			return base.printf(l, format, args);
		}

		public override PrintStream printf(string format, params object[] args)
		{
			return base.printf(format, args);
		}

		public override void println()
		{
		}
		public override void println(bool x)
		{
		}
		public override void println(char x)
		{
		}
		public override void println(char[] x)
		{
		}
		public override void println(double x)
		{
		}
		public override void println(float x)
		{
		}
		public override void println(int x)
		{
		}
		public override void println(long x)
		{
		}
		public override void println(object x)
		{
		}
		public override void println(string x)
		{
		}
		protected internal override void setError()
		{
			base.setError();
		}

		public override void write(sbyte[] buf, int off, int len)
		{
		}
		public override void write(int b)
		{
		}
	}


}