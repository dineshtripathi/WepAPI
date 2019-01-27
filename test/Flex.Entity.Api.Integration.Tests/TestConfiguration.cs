using System;

namespace Flex.Entity.Api.Integration.Tests
{
    public static class TestConfiguration
    {
        public static bool UseAuthentication = true;
        public static Uri ApplicationUnderTestBaseUrl { get; } = new Uri("http://flex-entity-api.azurewebsites.net");
        //public static Uri ApplicationUnderTestBaseUrl { get; } = new Uri("http://localhost:13659");
        public static string Issuer = "https://openenergi.eu.auth0.com/";
        public static Uri Auth0Url { get; } = new Uri("https://openenergi.eu.auth0.com/oauth/ro");


        //public static string audience_clientId = "osyAakHl8YMARR5IHtvr41wqivOfxzeI";
        //public static string audience_secretKey = "7HmG-p3-3InGpmGgQjqMJDJNWROcanRJ0GCeZ42Jmgm_AhaVJJijaAmuAFHoAGBD";


        public static string audience_clientId = "n38r4dOstj8HEWyAxIDbAtYhC65Nh7bs";
        public static string audience_secretKey = "NEViF1O5MDaMLgc5kvmAb7fzplkRKrfzadYmhaq1UeZds_V8rDtze6HH7WCJLr8U";

        public static string audience_issuer= "https://openenergi.eu.auth0.com/";
        public static object Auth0Json { get; } = new
        {
            client_id = "n38r4dOstj8HEWyAxIDbAtYhC65Nh7bs",
            username = @"nooruddin.kapasi@gmail.com",
            password = "Password1234",
            id_token = "",
            connection = "Username-Password-Authentication",
            grant_type = "password",
            scope = "openid name email app_metadata",
            device = ""
        };
    }
}