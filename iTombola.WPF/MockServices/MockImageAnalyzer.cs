using iTombola.Core.Interfaces;
using iTombola.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace iTombola.MockServices
{
	internal class MockImageAnalyzer : IImageAnalyzer
	{
		
		public Task<ImageAnalyzerResponse> ExtractNumbersImageFromStream(Stream imageData, CancellationToken token)
		{
			var result = new ImageAnalyzerResponse();

			result.Numbers = new List<ExtractedNumberInfo>
			{
				new ExtractedNumberInfo() { Confidence = 0.9, Number =  "23"}
			};

			return Task.FromResult(result);
		}

        public Task RecognizePrintedTextLocal(Stream imageData)
        {
            throw new NotImplementedException();
        }
    }
}
