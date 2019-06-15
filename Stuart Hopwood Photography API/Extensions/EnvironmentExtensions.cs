using Microsoft.AspNetCore.Hosting;

namespace Stuart_Hopwood_Photography_API.Extensions
{
	public static class EnvironmentExtensions
	{
		public static bool IsTest(this IHostingEnvironment env)
		{
			return env.IsEnvironment(StandardEnvironment.Test);
		}
	}
}
