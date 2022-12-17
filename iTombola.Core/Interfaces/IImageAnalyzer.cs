using iTombola.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTombola.Core.Interfaces
{
	public interface IImageAnalyzer
	{
		Task<ImageAnalyzerResponse> ExtractNumbersImageFromStream(Stream imageData,CancellationToken token);
    }
}
