using System.IO;

namespace org.maltparser.parser
{

	using  org.maltparser.core.exception;

	public class Diagnostics
	{
	//	protected final boolean diagnostics;
		protected internal readonly StreamWriter diaWriter;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Diagnostics(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public Diagnostics(string fileName)
		{
			try
			{
				if (fileName.Equals("stdout"))
				{
					diaWriter = new StreamWriter(System.out);
				}
				else if (fileName.Equals("stderr"))
				{
					diaWriter = new StreamWriter(System.err);
				}
				else
				{
					diaWriter = new StreamWriter(fileName);
				}
			}
			catch (IOException e)
			{
				throw new MaltChainedException("Could not open the diagnostic file. ", e);
			}
	//		this.diagnostics = (Boolean)manager.getOptionValue("singlemalt", "diagnostics");
	//		openDiaWriter(manager.getOptionValue("singlemalt", "diafile").toString());
		}

	//	public boolean isDiagnostics() {
	//		return diagnostics;
	//	}

		public virtual StreamWriter DiaWriter
		{
			get
			{
				return diaWriter;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeToDiaFile(String message) throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeToDiaFile(string message)
		{
			try
			{
				DiaWriter.Write(message);
			}
			catch (IOException e)
			{
				throw new MaltChainedException("Could not write to the diagnostic file. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void closeDiaWriter() throws org.maltparser.core.exception.MaltChainedException
		public virtual void closeDiaWriter()
		{
			if (diaWriter != null)
			{
				try
				{
					diaWriter.Flush();
					diaWriter.Close();
				}
				catch (IOException e)
				{
					throw new MaltChainedException("Could not close the diagnostic file. ", e);
				}
			}
		}

	//	public void openDiaWriter(String fileName) throws MaltChainedException {
	//		if (diagnostics) {
	//			try {
	//				if (fileName.equals("stdout")) {
	//					diaWriter = new BufferedWriter(new OutputStreamWriter(System.out));
	//				} else if (fileName.equals("stderr")) {
	//					diaWriter = new BufferedWriter(new OutputStreamWriter(System.err));
	//				} else {
	//					diaWriter = new BufferedWriter(new FileWriter(fileName));
	//				}
	//			} catch (IOException e) {
	//				throw new MaltChainedException("Could not open the diagnostic file. ", e);
	//			}
	//		}
	//	}
	}

}