using BusinessObjects.Models;

namespace Repositories.Helper
{
	public class SingletonBase<T> where T : class, new()
	{
		private static T _instance;
		private static readonly object _lock = new object();
		public static PostgresContext context = new PostgresContext();

		public static T Instance
		{
			get
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = new T();
					}
					return _instance;
				}
			}
		}
	}
}
