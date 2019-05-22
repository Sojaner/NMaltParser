using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.concurrent.test
{
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class Experiment
	{
		private readonly string modelName;
		private readonly URL modelURL;
		private readonly URL dataFormatURL;
		private readonly string charSet;
		private readonly IList<URL> inURLs;
		private readonly IList<File> outFiles;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Experiment(String _modelName, java.net.URL _modelURL, java.net.URL _dataFormatURL, String _charSet, java.util.List<java.net.URL> _inURLs, java.util.List<java.io.File> _outFiles) throws ExperimentException
		public Experiment(string _modelName, URL _modelURL, URL _dataFormatURL, string _charSet, IList<URL> _inURLs, IList<File> _outFiles)
		{
			this.modelName = _modelName;
			this.modelURL = _modelURL;
			this.dataFormatURL = _dataFormatURL;
			if (string.ReferenceEquals(_charSet, null) || _charSet.Length == 0)
			{
				this.charSet = "UTF-8";
			}
			else
			{
				this.charSet = _charSet;
			}
			if (_inURLs.Count != _outFiles.Count)
			{
				throw new ExperimentException("The lists of in-files and out-files must match in size.");
			}
			this.inURLs = Collections.synchronizedList(new List<URL>(_inURLs));
			this.outFiles = Collections.synchronizedList(new List<File>(_outFiles));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Experiment(String _modelName, String _modelFileName, String _dataFormatFileName, String _charSet, java.util.List<String> _inFileNames, java.util.List<String> _outFileNames) throws ExperimentException
		public Experiment(string _modelName, string _modelFileName, string _dataFormatFileName, string _charSet, IList<string> _inFileNames, IList<string> _outFileNames)
		{
			this.modelName = _modelName;

			try
			{
				this.modelURL = (new File(_modelFileName)).toURI().toURL();
			}
			catch (MalformedURLException e)
			{
				throw new ExperimentException("The model file name is malformed", e);
			}

			try
			{
				this.dataFormatURL = (new File(_dataFormatFileName)).toURI().toURL();
			}
			catch (MalformedURLException e)
			{
				throw new ExperimentException("The data format file name is malformed", e);
			}

			if (string.ReferenceEquals(_charSet, null) || _charSet.Length == 0)
			{
				this.charSet = "UTF-8";
			}
			else
			{
				this.charSet = _charSet;
			}

			if (_inFileNames.Count != _outFileNames.Count)
			{
				throw new ExperimentException("The lists of in-files and out-files must match in size.");
			}

			this.inURLs = Collections.synchronizedList(new List<URL>());
			for (int i = 0; i < _inFileNames.Count; i++)
			{
				try
				{
					this.inURLs.Add((new File(_inFileNames[i])).toURI().toURL());
				}
				catch (MalformedURLException e)
				{
					throw new ExperimentException("The in file name is malformed", e);
				}
			}

			this.outFiles = Collections.synchronizedList(new List<File>());
			for (int i = 0; i < _outFileNames.Count; i++)
			{
				this.outFiles.Add(new File(_outFileNames[i]));
			}
		}

		public virtual string ModelName
		{
			get
			{
				return modelName;
			}
		}

		public virtual URL ModelURL
		{
			get
			{
				return modelURL;
			}
		}

		public virtual URL DataFormatURL
		{
			get
			{
				return dataFormatURL;
			}
		}

		public virtual string CharSet
		{
			get
			{
				return charSet;
			}
		}

		public virtual IList<URL> InURLs
		{
			get
			{
				return Collections.synchronizedList(new List<URL>(inURLs));
			}
		}

		public virtual IList<File> OutFiles
		{
			get
			{
				return Collections.synchronizedList(new List<File>(outFiles));
			}
		}

		public virtual int nInURLs()
		{
			return inURLs.Count;
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + ((string.ReferenceEquals(charSet, null)) ? 0 : charSet.GetHashCode());
			result = prime * result + ((dataFormatURL == null) ? 0 : dataFormatURL.GetHashCode());
			result = prime * result + ((inURLs == null) ? 0 : inURLs.GetHashCode());
			result = prime * result + ((string.ReferenceEquals(modelName, null)) ? 0 : modelName.GetHashCode());
			result = prime * result + ((modelURL == null) ? 0 : modelURL.GetHashCode());
			result = prime * result + ((outFiles == null) ? 0 : outFiles.GetHashCode());
			return result;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			Experiment other = (Experiment) obj;
			if (string.ReferenceEquals(charSet, null))
			{
				if (!string.ReferenceEquals(other.charSet, null))
				{
					return false;
				}
			}
			else if (!charSet.Equals(other.charSet))
			{
				return false;
			}
			if (dataFormatURL == null)
			{
				if (other.dataFormatURL != null)
				{
					return false;
				}
			}
			else if (!dataFormatURL.Equals(other.dataFormatURL))
			{
				return false;
			}
			if (inURLs == null)
			{
				if (other.inURLs != null)
				{
					return false;
				}
			}
//JAVA TO C# CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
//ORIGINAL LINE: else if (!inURLs.equals(other.inURLs))
			else if (!inURLs.SequenceEqual(other.inURLs))
			{
				return false;
			}
			if (string.ReferenceEquals(modelName, null))
			{
				if (!string.ReferenceEquals(other.modelName, null))
				{
					return false;
				}
			}
			else if (!modelName.Equals(other.modelName))
			{
				return false;
			}
			if (modelURL == null)
			{
				if (other.modelURL != null)
				{
					return false;
				}
			}
			else if (!modelURL.Equals(other.modelURL))
			{
				return false;
			}
			if (outFiles == null)
			{
				if (other.outFiles != null)
				{
					return false;
				}
			}
//JAVA TO C# CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
//ORIGINAL LINE: else if (!outFiles.equals(other.outFiles))
			else if (!outFiles.SequenceEqual(other.outFiles))
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append("#STARTEXP");
			sb.Append('\n');
			sb.Append("MODELNAME:");
			sb.Append(modelName);
			sb.Append('\n');
			sb.Append("MODELURL:");
			sb.Append(modelURL);
			sb.Append('\n');
			sb.Append("DATAFORMATURL:");
			sb.Append(dataFormatURL);
			sb.Append('\n');
			sb.Append("CHARSET:");
			sb.Append(charSet);
			sb.Append('\n');
			sb.Append("INURLS");
			sb.Append('\n');
			for (int i = 0; i < inURLs.Count; i++)
			{
				sb.Append(inURLs[i].toExternalForm());
				sb.Append('\n');
			}
			sb.Append("OUTFILES");
			sb.Append('\n');
			for (int i = 0; i < outFiles.Count; i++)
			{
				sb.Append(outFiles[i]);
				sb.Append('\n');
			}
			sb.Append("#ENDEXP");
			sb.Append('\n');
			return sb.ToString();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.List<Experiment> loadExperiments(String experimentsFileName) throws java.net.MalformedURLException, java.io.IOException, ExperimentException
		public static IList<Experiment> loadExperiments(string experimentsFileName)
		{
			return loadExperiments((new File(experimentsFileName)).toURI().toURL());
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.List<Experiment> loadExperiments(java.net.URL experimentsURL) throws java.io.IOException, ExperimentException
		public static IList<Experiment> loadExperiments(URL experimentsURL)
		{
			IList<Experiment> experiments = Collections.synchronizedList(new List<Experiment>());

			StreamReader reader = new StreamReader(experimentsURL.openStream(), Encoding.UTF8);
			string line;
			bool read_expdesc = false;
			int read_inouturls = 0;
			string modelName = null;
			URL modelURL = null;
			URL dataFormatURL = null;
			string charSet = null;
			IList<URL> inURLs = new List<URL>();
			IList<File> outFiles = new List<File>();
			while (!string.ReferenceEquals((line = reader.ReadLine()), null))
			{
	//			System.out.println(line);
				if (line.Trim().Equals("#STARTEXP"))
				{
					read_expdesc = true;
				}
				else if (line.Trim().ToUpper().StartsWith("MODELNAME") && read_expdesc)
				{
					modelName = line.Trim().Substring(line.Trim().IndexOf(':') + 1);
				}
				else if (line.Trim().ToUpper().StartsWith("MODELURL") && read_expdesc)
				{
					modelURL = new URL(line.Trim().Substring(line.Trim().IndexOf(':') + 1));
				}
				else if (line.Trim().ToUpper().StartsWith("MODELFILE") && read_expdesc)
				{
					modelURL = (new File(line.Trim().Substring(line.Trim().IndexOf(':') + 1))).toURI().toURL();
				}
				else if (line.Trim().ToUpper().StartsWith("DATAFORMATURL") && read_expdesc)
				{
					dataFormatURL = new URL(line.Trim().Substring(line.Trim().IndexOf(':') + 1));
				}
				else if (line.Trim().ToUpper().StartsWith("DATAFORMATFILE") && read_expdesc)
				{
					dataFormatURL = (new File(line.Trim().Substring(line.Trim().IndexOf(':') + 1))).toURI().toURL();
				}
				else if (line.Trim().ToUpper().StartsWith("CHARSET") && read_expdesc)
				{
					charSet = line.Trim().Substring(line.Trim().IndexOf(':') + 1);
				}
				else if (line.Trim().ToUpper().StartsWith("INURLS") && read_expdesc)
				{
					read_inouturls = 1;
				}
				else if (line.Trim().ToUpper().StartsWith("INFILES") && read_expdesc)
				{
					read_inouturls = 2;
				}
				else if (line.Trim().ToUpper().StartsWith("OUTFILES") && read_expdesc)
				{
					read_inouturls = 3;
				}
				else if (read_expdesc && !line.Trim().Equals("#ENDEXP"))
				{
					if (read_inouturls == 1)
					{
						inURLs.Add(new URL(line.Trim()));
					}
					else if (read_inouturls == 2)
					{
						inURLs.Add((new File(line.Trim())).toURI().toURL());
					}
					else if (read_inouturls == 3)
					{
						outFiles.Add(new File(line.Trim()));
					}
				}
				else if (line.Trim().Equals("#ENDEXP") && read_expdesc)
				{
	//				System.out.println(modelName);
	//				System.out.println(modelURL);
	//				System.out.println(dataFormatURL);
	//				System.out.println(charSet);
	//				System.out.println(inURLs);
	//				System.out.println(outURLs);
					if (inURLs.Count > 0 && outFiles.Count > 0)
					{
						experiments.Add(new Experiment(modelName, modelURL, dataFormatURL, charSet, inURLs, outFiles));
					}
					modelName = charSet = null;
					modelURL = dataFormatURL = null;
					inURLs.Clear();
					outFiles.Clear();
					read_expdesc = false;
					read_inouturls = 0;
				}
			}

			return experiments;
		}
	}

}