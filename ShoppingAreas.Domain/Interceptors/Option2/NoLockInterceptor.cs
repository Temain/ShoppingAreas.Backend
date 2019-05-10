using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ShoppingAreas.Domain.Interceptors
{
	public class NoLockInterceptor : IObserver<KeyValuePair<string, object>>
	{
		public void OnCompleted()
		{
		}

		public void OnError(Exception error)
		{
		}

		public void OnNext(KeyValuePair<string, object> value)
		{
			if (value.Key == RelationalEventId.CommandExecuting.Name)
			{
				var command = ((CommandEventData)value.Value).Command;

				// Do command.CommandText manipulation here
				//var query = command.CommandText;
				//query = query.Replace("areas", "AREA");
				//command.CommandText = query;
			}
		}
	}
}
