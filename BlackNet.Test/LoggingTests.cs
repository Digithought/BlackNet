using System;
using NUnit.Framework;
using Digithought.BlackNet;

namespace BlackNet.Test
{
	[TestFixture]
	public class LoggingTests
	{
		private string _category;
		private object _message;

		[Test]
		public void TraceTest()
		{
			_category = null;
			_message = null;
			Logging.LogTrace += TraceCallback;
			try
			{
				Logging.Trace("Test", "Test message");
			}
			finally
			{
				Logging.LogTrace -= TraceCallback;
			}
			Assert.NotNull(_category);
			Assert.AreEqual("Test", _category);
			Assert.NotNull(_message);
			Assert.AreEqual("Test message", _message.ToString());
		}

		private void TraceCallback(string category, object message)
		{
			_category = category;
			_message = message;
		}

		[Test]
		public void TraceWithFilterTest()
		{
			_category = null;
			_message = null;
			Logging.LogTrace += TraceCallback;
			Logging.TraceFilter = TraceFilter;
			try
			{
				Logging.Trace("Test", "Test message");
				Logging.Trace("Other", "Another message");	// Filter this
			}
			finally
			{
				Logging.LogTrace -= TraceCallback;
				Logging.TraceFilter = null;
			}
			Assert.NotNull(_category);
			Assert.AreEqual("Test", _category);
			Assert.NotNull(_message);
			Assert.AreEqual("Test message", _message.ToString());
		}

		private bool TraceFilter(string category, object message)
		{
			return category == "Test";
		}

		private Exception _error;

		[Test]
		public void ErrorTest()
		{
			_error = null;
			Logging.LogError += TraceError;
			try
			{
				Logging.Error(new Exception("Test"));
			}
			finally
			{
				Logging.LogError -= TraceError;
			}
			Assert.NotNull(_error);
			Assert.AreEqual("Test", _error.Message);
		}

		private void TraceError(Exception e)
		{
			_error = e;
		}
	}
}
