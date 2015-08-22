﻿using System;
using System.ComponentModel.Design.Serialization;
using System.Threading.Tasks;

namespace GroundJobs.ServiceBus.Services
{
    public class GooglePlacesService : BaseHtmlScrapingService<GetEateriesRequest, GetEateriesResponse>
    {
        public override GetEateriesResponse Execute(GetEateriesRequest request)
        {
            var encodedPostcode = request.Command.Postcode.Replace(" ", string.Empty);
            var postcodeData = GetHTMLString($"http://api.postcodes.io/postcodes/{encodedPostcode}");
            postcodeData.Wait();
            var postcode = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(postcodeData.Result);

            var latitude = postcode.result.latitude.ToString();
            var longitude = postcode.result.longitude.ToString();
            var storesData = GetHTMLString($"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={latitude},{longitude}&radius=500&types=food&key=AIzaSyCM_pHgWShJwu4sYj_M79lDQ6Tpw9zV_9k");
            storesData.Wait();
            var googlePlaces = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(storesData.Result);

            if (googlePlaces.results.Count > 0)
                return new GetEateriesResponse
                {
                    LocationName = googlePlaces.results[0].name.ToString(),
                    Latitude = googlePlaces.results[0].geometry.location.lat.ToString(),
                    Longitude = googlePlaces.results[0].geometry.location.lng.ToString(),
                    Distance = float.Parse("123")
                };
            return new GetEateriesResponse();
        }
    }
}