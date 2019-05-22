using System;
using System.Text;

namespace org.maltparser.core.exception
{
	/// <summary>
	/// MaltChainedException handles a chain of MaltParser specific exception. 
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	public class MaltChainedException : Exception
	{
		public const long serialVersionUID = 8045568022124816379L;
		private readonly Exception cause;

		/// <summary>
		/// Creates a MaltChainedException instance with a message
		/// </summary>
		/// <param name="message"> a message string </param>
		public MaltChainedException(string message) : this(message, null)
		{
		}

		/// <summary>
		/// Creates a MaltChainedException instance with a message and keeps track of the cause of the exception.
		/// </summary>
		/// <param name="message">	a message string </param>
		/// <param name="cause">		a cause </param>
		public MaltChainedException(string message, Exception cause) : base(message)
		{
			this.cause = cause;
		}


		/* (non-Javadoc)
		 * @see java.lang.Throwable#getCause()
		 */
		public virtual Exception Cause
		{
			get
			{
				return cause;
			}
		}

		/// <summary>
		/// Returns a string representation of the exception chain. Only MaltParser specific exception is included.
		/// </summary>
		/// <returns> a string representation of the exception chain </returns>
		public virtual string MessageChain
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
				StringBuilder sb = new StringBuilder();
				Exception t = this;
    
				while (t != null)
				{
				  if (t.Message != null && t is MaltChainedException)
				  {
					  sb.Append(t.Message + "\n");
				  }
				  t = t.InnerException;
				}
				return sb.ToString();
			}
		}

		/* (non-Javadoc)
		 * @see java.lang.Throwable#printStackTrace()
		 */
		public virtual void printStackTrace()
		{
			Console.WriteLine(base.ToString());
			Console.Write(base.StackTrace);
			if (cause != null)
			{
				Console.WriteLine(cause.ToString());
				Console.Write(cause.StackTrace);
			}
		}
	}

}