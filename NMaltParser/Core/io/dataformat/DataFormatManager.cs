using NMaltParser.Core.Helper;

namespace NMaltParser.Core.IO.DataFormat
{
    public class DataFormatManager
	{
		private DataFormatSpecification inputDataFormatSpec;
		private DataFormatSpecification outputDataFormatSpec;
		private readonly HashMap<string, DataFormatSpecification> fileNameDataFormatSpecs;
		private readonly HashMap<string, DataFormatSpecification> nameDataFormatSpecs;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DataFormatManager(java.net.URL inputFormatUrl, java.net.URL outputFormatUrl) throws org.maltparser.core.exception.MaltChainedException
		public DataFormatManager(URL inputFormatUrl, URL outputFormatUrl)
		{
			fileNameDataFormatSpecs = new HashMap<string, DataFormatSpecification>();
			nameDataFormatSpecs = new HashMap<string, DataFormatSpecification>();
			inputDataFormatSpec = loadDataFormat(inputFormatUrl);
			outputDataFormatSpec = loadDataFormat(outputFormatUrl);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DataFormatSpecification loadDataFormat(java.net.URL dataFormatUrl) throws org.maltparser.core.exception.MaltChainedException
		public virtual DataFormatSpecification loadDataFormat(URL dataFormatUrl)
		{
			if (dataFormatUrl == null)
			{
				return null;
			}
			DataFormatSpecification dataFormat = fileNameDataFormatSpecs[dataFormatUrl.ToString()];
			if (dataFormat == null)
			{
				dataFormat = new DataFormatSpecification();
				dataFormat.parseDataFormatXMLfile(dataFormatUrl);
				fileNameDataFormatSpecs[dataFormatUrl.ToString()] = dataFormat;
				nameDataFormatSpecs[dataFormat.DataFormatName] = dataFormat;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.helper.URLFinder f = new org.maltparser.core.helper.URLFinder();
				UrlFinder f = new UrlFinder();

				foreach (Dependency dep in dataFormat.Dependencies)
				{
					loadDataFormat(f.FindUrLinJars(dep.UrlString));
				}
			}
			return dataFormat;
		}

		public virtual DataFormatSpecification InputDataFormatSpec
		{
			get
			{
				return inputDataFormatSpec;
			}
			set
			{
				inputDataFormatSpec = value;
			}
		}

		public virtual DataFormatSpecification OutputDataFormatSpec
		{
			get
			{
				return outputDataFormatSpec;
			}
			set
			{
				outputDataFormatSpec = value;
			}
		}



		public virtual DataFormatSpecification getDataFormatSpec(string name)
		{
			return nameDataFormatSpecs[name];
		}
	}

}