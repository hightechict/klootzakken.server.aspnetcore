using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using Klootzakken.Server.ApiModel;

namespace Klootzakken.Server.InMemory
{
    public class MainApi
    {
        readonly ConcurrentDictionary<string,UserApi> _userApiDictionary = new ConcurrentDictionary<string, UserApi>();
        public IObservable<IUserApi> GetUserApi(string userId)
        {
            return Observable.Return(_userApiDictionary.GetOrAdd(userId, id => new UserApi()));
        }
    }
}