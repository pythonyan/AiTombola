using iTombola.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTombola.Core.Interfaces
{
	public interface IAudioConverter
	{
		Task<AudioConverterResponse> ConvertDescriptionToAudio(NumberDescriptionInfo description, 
			CancellationToken token = default);

		Task<AudioConverterResponse> ConvertTextToAudio(string text, 
			string culture, CancellationToken token = default);
	}
}
