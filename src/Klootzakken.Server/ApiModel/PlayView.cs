using Klootzakken.Server.Model;

namespace Klootzakken.Server.ApiModel
{
    public class PlayView
    {
        public PlayView()
        {
        }

        public PlayView( Play play)
        {
            Cards = play.PlayedCards.Serialized();
        }

        public string[] Cards { get; set; }
    }
}