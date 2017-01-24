using System;
using System.Collections.Concurrent;
using Klootzakken.Server.ApiModel;

namespace Klootzakken.Server.InMemory
{
    public class MainApi
    {
        ConcurrentDictionary<string,UserApi> _userApiDictionary = new ConcurrentDictionary<string, UserApi>();
        public IObservable<IUserApi> GetUserApi(string userId)
        {
            //return _userApiDictionary.GetOrAdd(userId, id => new UserApi());
            throw new NotImplementedException();
        }
    }
}