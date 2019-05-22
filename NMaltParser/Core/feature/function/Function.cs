using System;

namespace org.maltparser.core.feature.function
{
	using  org.maltparser.core.exception;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface Function
	{
		/// <summary>
		/// Initialize the feature function
		/// </summary>
		/// <param name="arguments"> an array of arguments with the type returned by getParameterTypes() </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException;
		void initialize(object[] arguments);
		/// <summary>
		/// Returns an array of class types used by the feature extraction system to invoke initialize with
		/// correct arguments.
		/// </summary>
		/// <returns> an array of class types </returns>
		Type[] ParameterTypes {get;}
		/// <summary>
		/// Cause the feature function to update the feature value.
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update() throws org.maltparser.core.exception.MaltChainedException;
		void update();
	}

}