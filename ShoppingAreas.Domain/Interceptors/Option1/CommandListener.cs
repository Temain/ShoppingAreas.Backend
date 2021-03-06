﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DiagnosticAdapter;

namespace ShoppingAreas.Domain.Interceptors
{
	public class CommandListener
	{
		[DiagnosticName("Microsoft.EntityFrameworkCore.Database.Command.CommandExecuting")]
		public void OnCommandExecuting(DbCommand command, DbCommandMethod executeMethod, Guid commandId, Guid connectionId, bool async, DateTimeOffset startTime)
		{
		}

		[DiagnosticName("Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted")]
		public void OnCommandExecuted(object result, bool async)
		{
		}

		[DiagnosticName("Microsoft.EntityFrameworkCore.Database.Command.CommandError")]
		public void OnCommandError(Exception exception, bool async)
		{
		}
	}
}
