using System;
using System.Reactive.Linq;
using Klootzakken.Core.ApiModel;
using Klootzakken.Core.Model;

namespace Klootzakken.Server.InMemory
{
    public static class ViewExtensions
    {
        public static IObservable<LobbyView> AsView(this Lobby lobby) => Observable.Return(new LobbyView(lobby));

        public static IObservable<LobbyView> AsView(this IObservable<Lobby> game)
            => game.Select(l => new LobbyView(l));

        public static IObservable<GameView> AsViewFor(this Game lobby, User user)
            => Observable.Return(new GameView(lobby, user));

        public static IObservable<GameView> AsViewFor(this IObservable<Game> game, User user)
            => game.Select(g => new GameView(g, user));
    }
}