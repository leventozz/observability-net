﻿using System.Diagnostics;

namespace Observability.ConsoleApp
{
    internal class FirstService
    {
        static HttpClient httpClient = new();
        internal async Task<int> GetGoogleBytes()
        {
            using var activity = ActivitySourceProvider.Source.StartActivity(kind: ActivityKind.Producer, name:"CustomGetGoogleBytes");

            var eventTags = new ActivityTagsCollection();

            activity?.AddEvent(new("Google Request Started"));
            activity?.AddTag("request.scheme", "https");
            activity?.AddTag("request.method", "GET");
            var result = await httpClient.GetAsync("https://www.google.com");
            var responseContent = await result.Content.ReadAsStringAsync();
            activity?.AddTag("response.lenght", responseContent.Length); //This is the correct usage instead of tag.
            eventTags.Add("Google Body Lenght", responseContent.Length); //This is not best practice.
            activity?.AddEvent(new("Google Request Finished", tags: eventTags));

            var secondService = new SecondService();
            await secondService.WriteToFile(responseContent.Length.ToString());
            return responseContent.Length;
        }

        internal async Task<int> GetStackOverFlowBytes()
        {
            using var activity = ActivitySourceProvider.Source.StartActivity();
            var result = await httpClient.GetAsync("https://www.stackoverflow.com");
            var responseContent = await result.Content.ReadAsStringAsync();
            return responseContent.Length;
        }
    }
}
