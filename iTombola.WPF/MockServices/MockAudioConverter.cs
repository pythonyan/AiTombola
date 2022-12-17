using iTombola.Core.Interfaces;
using iTombola.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace iTombola.MockServices
{
	internal class MockAudioConverter : IAudioConverter
	{
		public Task<AudioConverterResponse> ConvertDescriptionToAudio(NumberDescriptionInfo description, CancellationToken token = default)
		{
			var result = new AudioConverterResponse
			{
				Audio = null,
				Culture = description!.Culture,
				InputText = description!.Content
			};

			return Task.FromResult(result);
		}

		public Task<AudioConverterResponse> ConvertTextToAudio(string text, string culture, CancellationToken token)
		{
			var result = new AudioConverterResponse
			{
				Audio = null,
				Culture = culture,
				InputText = text
			};

			return Task.FromResult(result);
		}
	}
}
