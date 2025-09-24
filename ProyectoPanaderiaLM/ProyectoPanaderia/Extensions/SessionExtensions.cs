// Crea una carpeta Extensions y un archivo SessionExtensions.cs
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json; // Necesitarás instalar el paquete NuGet Newtonsoft.Json
namespace ProyectoPanaderia.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
