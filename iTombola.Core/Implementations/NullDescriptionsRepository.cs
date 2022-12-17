using iTombola.Core.Interfaces;
using iTombola.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTombola.Core.Implementations
{
	public class NullDescriptionsRepository : IDescriptionsRepository
	{
		public Task<NumberDescriptionInfo> GetDescriptionAsync(string number, string culture, string dialect, CancellationToken token = default)
		{
			return Task.FromResult<NumberDescriptionInfo>(null);
		}
	}
}
