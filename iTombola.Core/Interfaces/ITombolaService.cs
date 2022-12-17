using iTombola.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTombola.Core.Interfaces
{
	/// <summary>
	/// Interface for the iTombola service
	/// 
	/// This service has the responsibility to get an image, and return the list of the numbers with audio.
	/// </summary>
	/// <param name="imageData">The stream of the image to analyze</param>
	/// <param name="culture">Culture string indicates the country/culture for the process (e.g.it-IT)</param>
	/// <param name="dialect">Specific dialect for a culture (e.g. "rm" for roman or "na" for napolitan)</param>
	/// <param name="expectedConfidence">The expected confidence (between 0 abd 1) you want for the number contained in the image. The service will return only number with confidence more than this</param>
	/// <param name="maximumResultNumber">Maximum number of result returned. Default value is 1</param>
	public interface ITombolaService
	{
		Task<ExtractedNumbersResponse> ExtractNumberFromStream(Stream imageData, string culture, string dialect,
			double expectedConfidence,int maximumResultNumber=1, CancellationToken token=default);
	}
}
