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

        #region Search Hotels

        private const string EntityGeographyCity = "builtin.geography.city";

        [LuisIntent("SearchHotels")]
        public async Task SearchHotels(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Welcome to the Hotels finder! we are analyzing your message: '{message.Text}'...");

            var hotelsQuery = new HotelsQuery();

            EntityRecommendation cityEntityRecommendation;

            if (result.TryFindEntity(EntityGeographyCity, out cityEntityRecommendation))
            {
                cityEntityRecommendation.Type = "Destination";
            }

            var hotelsFormDialog = new FormDialog<HotelsQuery>(hotelsQuery, this.BuildHotelsForm, FormOptions.PromptInStart, result.Entities);

            context.Call(hotelsFormDialog, this.ResumeAfterHotelsFormDialog);
        }

        private IForm<HotelsQuery> BuildHotelsForm()
        {
            OnCompletionAsyncDelegate<HotelsQuery> processHotelsSearch = async (context, state) =>
            {
                var message = "Searching for hotels";
                if (!string.IsNullOrEmpty(state.Destination))
                {
                    message += $" in {state.Destination}...";
                }
                else if (!string.IsNullOrEmpty(state.AirportCode))
                {
                    message += $" near {state.AirportCode.ToUpperInvariant()} airport...";
                }

                await context.PostAsync(message);
            };

            return new FormBuilder<HotelsQuery>()
                .Field(nameof(HotelsQuery.Destination), (state) => string.IsNullOrEmpty(state.AirportCode))
                .Field(nameof(HotelsQuery.AirportCode), (state) => string.IsNullOrEmpty(state.Destination))
                .OnCompletion(processHotelsSearch)
                .Build();
        }

        private async Task ResumeAfterHotelsFormDialog(IDialogContext context, IAwaitable<HotelsQuery> result)
        {
            try
            {
                var searchQuery = await result;

                var hotels = await this.GetHotelsAsync(searchQuery);

                await context.PostAsync($"I found {hotels.Count()} hotels:");

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                foreach (var hotel in hotels)
                {
                    HeroCard heroCard = new HeroCard()
                    {
                        Title = hotel.Name,
                        Subtitle = $"{hotel.Rating} starts. {hotel.NumberOfReviews} reviews. From ${hotel.PriceStarting} per night.",
                        Images = new List<CardImage>()
                        {
                            new CardImage() { Url = hotel.Image }
                        },
                        Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "More details",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/search?q=hotels+in+" + HttpUtility.UrlEncode(hotel.Location)
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
                    reply = "You have canceled the operation.";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }

        private async Task<IEnumerable<Hotel>> GetHotelsAsync(HotelsQuery searchQuery)
        {
            var hotels = new List<Hotel>();

            // Filling the hotels results manually just for demo purposes
            for (int i = 1; i <= 5; i++)
            {
                var random = new Random(i);
                Hotel hotel = new Hotel()
                {
                    Name = $"{searchQuery.Destination ?? searchQuery.AirportCode} Hotel {i}",
                    Location = searchQuery.Destination ?? searchQuery.AirportCode,
                    Rating = random.Next(1, 5),
                    NumberOfReviews = random.Next(0, 5000),
                    PriceStarting = random.Next(80, 450),
                    Image = $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Hotel+{i}&w=500&h=260"
                };

                hotels.Add(hotel);
            }

            hotels.Sort((h1, h2) => h1.PriceStarting.CompareTo(h2.PriceStarting));

            return hotels;
        }
        #endregion

        #region Show Hotel Reviews
        private const string EntityHotelName = "Hotel";

        private IList<string> titleOptions = new List<string> { "“Very stylish, great stay, great staff”", "“good hotel awful meals”", "“Need more attention to little things”", "“Lovely small hotel ideally situated to explore the area.”", "“Positive surprise”", "“Beautiful suite and resort”" };

        [LuisIntent("ShowHotelsReviews")]
        public async Task ShowHotelReviews(IDialogContext context, LuisResult result)
        {
            EntityRecommendation hotelEntityRecommendation;

            if (result.TryFindEntity(EntityHotelName, out hotelEntityRecommendation))
            {
                await context.PostAsync($"Looking for reviews of '{hotelEntityRecommendation.Entity}'...");

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                for (int i = 0; i < 5; i++)
                {
                    var random = new Random(i);
                    ThumbnailCard thumbnailCard = new ThumbnailCard()
                    {
                        Title = this.titleOptions[random.Next(0, this.titleOptions.Count - 1)],
                        Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris odio magna, sodales vel ligula sit amet, vulputate vehicula velit. Nulla quis consectetur neque, sed commodo metus.",
                        Images = new List<CardImage>()
                        {
                            new CardImage() { Url = "https://upload.wikimedia.org/wikipedia/en/e/ee/Unknown-person.gif" }
                        },
                    };

                    resultMessage.Attachments.Add(thumbnailCard.ToAttachment());
                }

                await context.PostAsync(resultMessage);
            }

            context.Wait(this.MessageReceived);
        }
        #endregion

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
                // Script: Error on exception, Confirm on success.
                var message = "OK, I'll look for flights";

                if (!string.IsNullOrEmpty(this.ToTitleCase(state.DepartureCity)))
                {
                    message += $" from {state.DepartureCity}";
                }

                if (!string.IsNullOrEmpty(this.ToTitleCase(state.ReturnCity)))
                {
                    message += $" to {state.ReturnCity}";
                }

                if (!string.IsNullOrEmpty(state.DepartureDate))
                {
                    message += $" leaving {state.DepartureDate}";
                }

                if (!string.IsNullOrEmpty(state.ReturnDate))
                {
                    message += $" returning {state.ReturnDate}";
                }

                message += "...";

                await context.PostAsync(message);
            };

            return new FormBuilder<FlightsQuery>()
                .Message("Great, I can help you find flights!")
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
                .Confirm(async (State) =>
                {
                    return new PromptAttribute("FormFlow Confirm? {||}");
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

                await context.PostAsync($"I found {flights.Count()} flights:");

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
                    reply = "You have canceled the operation.";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
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

        private const string TravelAdviceOption = "Travel Advice";
        private const string SearchFlightsOption = "Search Flights";
        private const string SearchHotelsOption = "Search Hotels";

        private static Attachment CreateRootDialogHeroCard()
        {
            var heroCard = new HeroCard
            {
                Title = "Welcome to Travel Advisor",
                Text = "Hi, I'm the Travel Advisor Bot and I'm here to find the best hotels and flights for your upcoming travel. Here are some things I can help with:",
                Images = new List<CardImage> { new CardImage("https://placeholdit.imgix.net/~text?txtsize=20&txt=Travel%20Advisor%20Bot&w=200&h=100") },
                Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.ImBack, TravelAdviceOption, value: TravelAdviceOption),
                    new CardAction(ActionTypes.ImBack, SearchHotelsOption, value: SearchHotelsOption),
                    new CardAction(ActionTypes.ImBack, SearchFlightsOption, value: SearchFlightsOption)}
            };

            return heroCard.ToAttachment();
        }
    }
}