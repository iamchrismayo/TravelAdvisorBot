namespace TravelAdvisorBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Threading.Tasks;
    using Models;
    using Microsoft.Bot.Connector;
    using System.Globalization;

#pragma warning disable 1998

    [Serializable]
    public class SearchFlightsDialog : IDialog<string>
    {
        private FlightsQuery flightsQuery;

        public async Task StartAsync(IDialogContext context)
        {
            this.flightsQuery = new FlightsQuery();

            PromptDialog.Text(context, this.AfterPromptDepartureCity, "Where are you flying from?", "I'm sorry, I don't understand. Please try again.", 3);
        }

        private async Task AfterPromptDepartureCity(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.flightsQuery.DepartureCity = await result;

                PromptDialog.Text(context, this.AfterPromptReturnCity, "Where do you want to fly to?", "I'm sorry, I don't understand. Please try again.", 3);
            }
            catch (TooManyAttemptsException ex)
            {
                context.Fail(ex);
            }
        }

        private async Task AfterPromptReturnCity(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.flightsQuery.ReturnCity = await result;

                var departureDateDialog = new DateDialog("When do you want to leave ?");
                context.Call(departureDateDialog, this.AfterDepartureDateDialog);
            }
            catch (TooManyAttemptsException ex)
            {
                context.Fail(ex);
            }
        }

        private async Task AfterDepartureDateDialog(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.flightsQuery.DepartureDate = await result;

                var returnDateDialog = new DateDialog("When would you like to return?");
                context.Call(returnDateDialog, this.AfterReturnDateDialog);
            }
            catch (TooManyAttemptsException ex)
            {
                context.Fail(ex);
            }
        }

        private async Task AfterReturnDateDialog(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.flightsQuery.ReturnDate = await result;

                var flights = await this.GetFlightsAsync(flightsQuery);

                await context.PostAsync($"OK, I found {flights.Count()} flights.");

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                foreach (var flight in flights)
                {
                    HeroCard heroCard = new HeroCard()
                    {
                        Title = $"{this.ToTitleCase(flight.DepartureAirport)} to {this.ToTitleCase(flight.ReturnAirport)}",
                        Subtitle = $"${flight.Price}",
                        Images = new List<CardImage>()
                        {
                            new CardImage() { Url = flight.Image }
                        },
                        Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "More details",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/search?q=flights+from+" + HttpUtility.UrlEncode(flight.DepartureAirport) + "+to+" + HttpUtility.UrlEncode(flight.DepartureAirport)
                            }
                        }
                    };

                    resultMessage.Attachments.Add(heroCard.ToAttachment());
                }

                await context.PostAsync(resultMessage);

            }
            catch (TooManyAttemptsException ex)
            {
                context.Fail(ex);
            }
            finally
            {
                context.Done("Done");
            }
        }

        private string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        private async Task<IEnumerable<Flight>> GetFlightsAsync(FlightsQuery searchQuery)
        {
            var flights = new List<Flight>();

            // Filling in the flight details manually for demo purposes...
            for (int i = 0; i < 5; i++)
            {
                var random = new Random(i);
                Flight flight = new Flight()
                {
                    DepartureAirport = $"{searchQuery.DepartureCity}",
                    DepartureDateTime = DateTime.Parse(searchQuery.DepartureDate),
                    ReturnAirport = $"{searchQuery.ReturnCity}",
                    ReturnDateTime = DateTime.Parse(searchQuery.ReturnDate),
                    Price = random.Next(80, 450),
                    Image = $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Flight+{i + 1}&w=200&h=100"
                };

                flights.Add(flight);
            }

            flights.Sort((f1, f2) => f1.Price.CompareTo(f2.Price));

            return flights;
        }
    }
}