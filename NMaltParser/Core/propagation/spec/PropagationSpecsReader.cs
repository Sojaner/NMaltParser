namespace org.maltparser.core.propagation.spec
{


	using  org.maltparser.core.exception;
	using  org.w3c.dom;
	using  org.w3c.dom;
	using  org.xml.sax;

	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class PropagationSpecsReader
	{
		public PropagationSpecsReader()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(java.net.URL url, PropagationSpecs propagationSpecs) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(URL url, PropagationSpecs propagationSpecs)
		{
			try
			{
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				DocumentBuilder db = dbf.newDocumentBuilder();
				Element root = null;

				root = db.parse(url.openStream()).DocumentElement;

				if (root == null)
				{
					throw new PropagationException("The propagation specification file '" + url.File + "' cannot be found. ");
				}

				readPropagationSpecs(root, propagationSpecs);
			}
			catch (IOException e)
			{
				throw new PropagationException("The propagation specification file '" + url.File + "' cannot be found. ", e);
			}
			catch (ParserConfigurationException e)
			{
				throw new PropagationException("Problem parsing the file " + url.File + ". ", e);
			}
			catch (SAXException e)
			{
				throw new PropagationException("Problem parsing the file " + url.File + ". ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readPropagationSpecs(org.w3c.dom.Element propagations, PropagationSpecs propagationSpecs) throws org.maltparser.core.exception.MaltChainedException
		private void readPropagationSpecs(Element propagations, PropagationSpecs propagationSpecs)
		{
			NodeList propagationList = propagations.getElementsByTagName("propagation");
			for (int i = 0; i < propagationList.Length; i++)
			{
				readPropagationSpec((Element)propagationList.item(i), propagationSpecs);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readPropagationSpec(org.w3c.dom.Element propagation, PropagationSpecs propagationSpecs) throws org.maltparser.core.exception.MaltChainedException
		private void readPropagationSpec(Element propagation, PropagationSpecs propagationSpecs)
		{
			int nFrom = propagation.getElementsByTagName("from").Length;
			if (nFrom < 1 && nFrom > 1)
			{
				throw new PropagationException("Propagation specification wrongly formatted: Number of 'from' elements is '" + nFrom + "', must be 1.");
			}

			int nTo = propagation.getElementsByTagName("to").Length;
			if (nTo < 1 && nTo > 1)
			{
				throw new PropagationException("Propagation specification wrongly formatted: Number of 'to' elements is '" + nTo + "', must be 1.");
			}

			int nFor = propagation.getElementsByTagName("for").Length;
			if (nFor > 1)
			{
				throw new PropagationException("Propagation specification wrongly formatted: Number of 'for' elements is '" + nFor + "', at most 1.");
			}

			int nOver = propagation.getElementsByTagName("over").Length;
			if (nOver > 1)
			{
				throw new PropagationException("Propagation specification wrongly formatted: Number of 'over' elements is '" + nOver + "',at most 1.");
			}
			string fromText = ((Element)propagation.getElementsByTagName("from").item(0)).TextContent.Trim();
			if (fromText.Length == 0)
			{
				throw new PropagationException("Propagation specification wrongly formatted: The 'from' element is empty");
			}
			string toText = ((Element)propagation.getElementsByTagName("to").item(0)).TextContent.Trim();
			if (toText.Length == 0)
			{
				throw new PropagationException("Propagation specification wrongly formatted: The 'to' element is empty");
			}
			string forText = "";
			if (nFor != 0)
			{
				forText = ((Element)propagation.getElementsByTagName("for").item(0)).TextContent.Trim();
			}
			string overText = "";
			if (nOver != 0)
			{
				overText = ((Element)propagation.getElementsByTagName("over").item(0)).TextContent.Trim();
			}
			propagationSpecs.Add(new PropagationSpec(fromText, toText, forText, overText));
		}
	}

}