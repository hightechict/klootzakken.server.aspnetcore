using Klootzakken.Core.Model;

namespace Klootzakken.Core.ApiModel
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