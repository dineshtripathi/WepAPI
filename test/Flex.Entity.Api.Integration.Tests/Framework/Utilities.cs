using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Flex.Entity.Api.Integration.Tests.Framework
{
    public static class Utilities
    {
        public static IContainer Container { get; set; }
        public static T DeserializeJson<T>(string json)
        {
            var result = default(T);
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.MissingMemberHandling = MissingMemberHandling.Ignore;

                result = JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch (JsonSerializationException)
            {
                //Eatup exception
            }
            catch (Exception )
            {                
                //Eatup exception
            }

            return result;
        }

        public static object DeserializeJson(string json)
        {
            var result = default(object);
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.MissingMemberHandling = MissingMemberHandling.Ignore;

                result = JsonConvert.DeserializeObject(json, settings);
            }
            catch (JsonSerializationException)
            {
                //Eatup exception
            }
            catch (Exception)
            {
                //Eatup exception
            }

            return result;
        }

        public static T DeserializeSimpleJson<T>(string json)
        {
            var result = default(T);
            try
            {
                result = JsonConvert.DeserializeObject<T>(json);
            }
            catch (JsonSerializationException )
            {
                //Eatup exception
            }
            return result;
        }

        public static string SerializeJson<T>(T content)
        {
            return JsonConvert.SerializeObject(content);
        }
    }
}
