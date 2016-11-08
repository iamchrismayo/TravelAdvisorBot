using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.FormFlow;

using TravelAdvisorBot.Models;
using System.Globalization;

#pragma warning disable 1998

namespace TravelAdvisorBot.Dialogs
{
    [Serializable]
    [LuisModel("d53866b9-50fd-4832-b436-d239ddb59d7b", "f0392b343da4490f9bc91197d6a5eda2")]
    public class RootDialog : LuisDialog<object>
    {

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi! Try asking me things like 'search hotels in Seattle', 'search hotels near LAX airport' or 'show me the reviews of The Bot Resort'");

            context.Wait(this.MessageReceived);
        }

        #region Search Flights

        private const string EntityDepartureCity = "City::DepartureCity";
        private const string EntityArrivalCity = "City::ReturnCity";
        private const string EntityDepartureDate = "Date::DepartureDate";
        private const string EntityReturnDate = "Date::ReturnDate";

        [LuisIntent("SearchFlights")]
        public async Task SearchFlights(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;

            var flightsQuery = new FlightsQuery();

            EntityRecommendation departureCityEntityRecommendation;
            if (result.TryFindEntity(EntityDepartureCity, out departureCityEntityRecommendation))
            {
                departureCityEntityRecommendation.Type = "DepartureCity";
            }

            EntityRecommendation arrivalCityEntityRecommendation;
            if (result.TryFindEntity(EntityArrivalCity, out arrivalCityEntityRecommendation))
            {
                arrivalCityEntityRecommendation.Type = "ReturnCity";
            }

            EntityRecommendation departureDateEntityRecommendation;
            if (result.TryFindEntity(EntityDepartureDate, out departureDateEntityRecommendation))
            {
                departureDateEntityRecommendation.Type = "DepartureDate";
            }

            EntityRecommendation returnDateEntityRecommendation;
            if (result.TryFindEntity(EntityReturnDate, out returnDateEntityRecommendation))
            {
                returnDateEntityRecommendation.Type = "ReturnDate";
            }

            var searchFlightsFormDialog = new FormDialog<FlightsQuery>(flightsQuery, this.BuildSearchFlightsForm, FormOptions.PromptInStart, result.Entities);

            context.Call(searchFlightsFormDialog, this.ResumeAfterSearchFlightsFormDialog);
        }

        private IForm<FlightsQuery> BuildSearchFlightsForm()
        {
            OnCompletionAsyncDelegate<FlightsQuery> processFlightsSearch = async (context, state) =>
            {

            };

            return new FormBuilder<FlightsQuery>()
                .Message("Great, I can help you find flights.")
                .Field(nameof(FlightsQuery.DepartureCity))
                .Field(nameof(FlightsQuery.ReturnCity))
                .Field(nameof(FlightsQuery.DepartureDate),
                    validate: async (state, value) =>
                    {
                        var result = new ValidateResult { IsValid = true, Value = value };

                        var resultString = (value as string).Trim();
                        DateTime date;

                        if (DateTime.TryParse(resultString, out date))
                        {
                            result.Value = date.ToShortDateString();
                        }
                        else
                        {
                            result.Feedback = $"I don't understand '{resultString}'. Try '11/22/2017', '11/22', Jan 22, etc.";
                            result.IsValid = false;
                        }

                        return result;
                    })
                .Field(nameof(FlightsQuery.ReturnDate),
                    validate: async (state, value) =>
                    {
                        var result = new ValidateResult { IsValid = true, Value = value };

                        var resultString = (value as string).Trim();
                        DateTime date;

                        if (DateTime.TryParse(resultString, out date))
                        {
                            result.Value = date.ToShortDateString();
                        }
                        else
                        {
                            result.Feedback = $"I don't understand '{resultString}'. Try '11/22/2017', '11/22', Jan 22, etc.";
                            result.IsValid = false;
                        }

                        return result;
                    })
                .Confirm(async (state) =>
                {   
                    return new PromptAttribute("Great, so I'm looking for flights from {DepartureCity} to {ReturnCity}, leaving {DepartureDate}, and returning {ReturnDate}. {||}");
                })
                .OnCompletion(processFlightsSearch)
                .Build();
        }

        private async Task ResumeAfterSearchFlightsFormDialog(IDialogContext context, IAwaitable<FlightsQuery> result)
        {
            try
            {
                var searchQuery = await result;

                var flights = await this.GetFlightsAsync(searchQuery);

                await context.PostAsync($"I found {flights.Count()} flights.");

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
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "Cancelled.";
                }
                else
                {
                    reply = $"Exception: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
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
                    DepartureAirline = "Alaska",
                    DepartureDateTime = DateTime.Parse(searchQuery.DepartureDate),
                    DepartureFlightNumber = random.Next(101, 9999).ToString(),
                    ReturnAirport = $"{searchQuery.ReturnCity}",
                    ReturnAirline = "United",
                    ReturnDateTime = DateTime.Parse(searchQuery.ReturnDate),
                    ReturnFlightNumber = random.Next(101, 9999).ToString(),
                    Price = random.Next(80, 450),
                    Image = $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Flight+{i + 1}&w=200&h=100"
                };

                flights.Add(flight);
            }

            flights.Sort((f1, f2) => f1.Price.CompareTo(f2.Price));

            return flights;
        }

        #endregion
    }
}