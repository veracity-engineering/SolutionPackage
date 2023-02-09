using DNVGL.Veracity.Services.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNVGL.Veracity.Services.Api.My.Abstractions
{
	public interface IMyWidgets
	{
		Task<IEnumerable<Widget>> Get();
	}
}
