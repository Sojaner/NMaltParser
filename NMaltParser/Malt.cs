namespace NMaltParser
{

	/// <summary>
	/// Application class in the MaltParser library.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class Malt
	{
		/// <summary>
		/// The starting point of MaltParser
		/// </summary>
		/// <param name="args"> command-line arguments </param>
		public static void Main(string[] args)
		{
			MaltConsoleEngine engine = new MaltConsoleEngine();
			engine.startEngine(args);
		}
	}

}