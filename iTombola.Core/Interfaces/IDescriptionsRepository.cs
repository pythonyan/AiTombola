using iTombola.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTombola.Core.Interfaces
{
	public interface IDescriptionsRepository
	{
		Task<NumberDescriptionInfo> GetDescriptionAsync(string number,string culture, 
			string dialect,CancellationToken token = default);
	}
}
