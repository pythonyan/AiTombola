using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models
{
	internal static class ReadInStreamHeadersExtensions
	{
		const int numberOfCharsInOperationId = 36;
		public static string GetOperationId(this ReadInStreamHeaders streamHeaders)
		{
			string operationLocation = streamHeaders.OperationLocation;
			string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);
			return operationId;
		}
	}
}
