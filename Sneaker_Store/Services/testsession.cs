using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Sneaker_Store.Services
{
    public class NoSessionObjectException : ArgumentException
    {
        public NoSessionObjectException()
        {
        }

        public NoSessionObjectException(string? message) : base(message)
        {
        }
    }

    public static class Testsession
    {
        public static T Get<T>(HttpContext context)
        {
            string sessionName = typeof(T).Name;
            string? s = context.Session.GetString(sessionName);
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new NoSessionObjectException($"No session {sessionName}");
            }
            return JsonSerializer.Deserialize<T>(s);
        }

        public static void Set<T>(T t, HttpContext context)
        {
            string sessionName = typeof(T).Name;
            string s = JsonSerializer.Serialize(t);
            context.Session.SetString(sessionName, s);
        }

        public static void Clear<T>(HttpContext context)
        {
            context.Session.Remove(typeof(T).Name);
        }
    }
}