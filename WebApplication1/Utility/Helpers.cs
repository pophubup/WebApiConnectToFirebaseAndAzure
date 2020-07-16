using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Utility
{
    public enum OptionStatus
    {
        NoneValue,
        HasValue,
        Error
    }

    public struct Option<T>
    {
        public bool HasValue { get; private set; }
        public bool HasError { get; private set; }
        public Exception Error { get; private set; }
        public T Value { get; private set; }
        public OptionStatus Status { get; private set; }

        public Option(T value)
        {
            if (value == null)
            {
                HasValue = false;
                HasError = false;
                Error = null;
                Value = default;
                Status = OptionStatus.NoneValue;
            }
            else
            {
                HasValue = true;
                HasError = false;
                Value = value;
                Error = null;
                Status = OptionStatus.HasValue;
            }
        }

        public Option(Exception ex)
        {
            HasValue = false;
            HasError = true;
            Error = ex;
            Value = default;
            Status = OptionStatus.Error;
        }

        public static implicit operator Option<T>(T value) => new Option<T>(value);
        public static implicit operator T(Option<T> value) => value.Value;
        public static implicit operator Option<T>(Exception ex) => new Option<T>(ex);

    }
    public static class Helpers
    {
        public static Option<T> Some<T>(T value) => new Option<T>(value);
        public static Option<R> Select<T, R>(this Option<T> source, Func<Option<T>, R> func) =>
              func(source);
        public static Option<T> Where<T>(this Option<T> value, Func<T, bool> func)
        {
            if (func(value))
                return value;
            return default;
        }
        public static Option<R> Map<T, R>(this Option<T> source, Func<T, R> map) =>
         (source.HasValue && !source.HasError) ? Some(map(source.Value)) : new Option<R>();
        public static async Task<Option<R>> Map<T, R>(this Option<T> source, Func<T, Task<R>> func) =>
          source.HasValue ? Some(await func(source).ConfigureAwait(false)) : (source.Error != null ? new Option<R>(source.Error) : new Option<R>());

        public static async Task<Option<R>> Map<T, R>(this Task<Option<T>> source, Func<T, Task<R>> func)
        {
            var sourceValue = await source;
            return sourceValue.HasValue ? Some(await func(sourceValue).ConfigureAwait(false)) :
                 (sourceValue.Error != null ? new Option<R>(sourceValue.Error) : new Option<R>());
        }
        public static HttpClient Connect(this HttpClient client, string url)
        {
            client.BaseAddress = new Uri(url);
            return client;
        }
        public static HttpResponseMessage Get(this HttpClient client)
        {
            var response = Task.Run(async () => {
                return await client.GetAsync("api/resources/test");
            });
            
            return response.GetAwaiter().GetResult();
        }
        public static async Task<string> Error (this HttpResponseMessage source, Action<string> error)
        {
            if (!source.IsSuccessStatusCode)
                 await source.Error(error);
            return await source.Content.ReadAsStringAsync();
        }

        public static string parseJsonToStringLinq(this HttpResponseMessage httpResponseMessage)
        {
            return httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
        public static Task<string> parseJsonToStringAsync(this Task<HttpResponseMessage> httpResponseMessage)
        {
            return httpResponseMessage.GetAwaiter().GetResult().Content.ReadAsStringAsync();
        }
        public static async Task<string> parseJsonToString(this Task<HttpResponseMessage> httpResponseMessage)
        {
          return await httpResponseMessage.GetAwaiter().GetResult().Content.ReadAsStringAsync();
        }

        public static StockData parseStringToObjectLinq(this string input)
        {
            return JsonConvert.DeserializeObject<StockData>(input);
        }
        public static T parseStringToObject<T>(this Task<string> input, T output)
        {
            return JsonConvert.DeserializeObject<T>(input.GetAwaiter().GetResult());
        }
        public static async Task<StockData> parseStringToObjectAsync(this string input)
        {
            var data = Task.Run(() =>
            {
                return JsonConvert.DeserializeObject<StockData>(input);
            });
            return await data;
        }
        public static async Task<StockData> stockNameArrangeAsync(this StockData source)
        {
            var data = Task.Run(() =>
            {
                var i = 0;
                List<string> stockNameCombine = new List<string>();
                source.stockName.GroupBy(item => i++ / 2).ToList().ForEach(
                   x => stockNameCombine.Add($"{x.FirstOrDefault()} {x.LastOrDefault()}")
                );
                source.stockName = stockNameCombine;
                return source;
            });
            return await data;
        }
        public static StockData stockNameArrange(this StockData source)
        {
            var i = 0;
            List<string> stockNameCombine = new List<string>();
            source.stockName.GroupBy(item => i++ / 2).ToList().ForEach(
                x=> stockNameCombine.Add($"{x.FirstOrDefault()} {x.LastOrDefault()}")
             );
            source.stockName = stockNameCombine;
            return source;
        }
        public static StockData stockNameArrangeLinq(StockData source)
        {
            var i = 0;
            List<string> stockNameCombine = new List<string>();
            source.stockName.GroupBy(item => i++ / 2).ToList().ForEach(
                x => stockNameCombine.Add($"{x.FirstOrDefault()} {x.LastOrDefault()}")
             );
            source.stockName = stockNameCombine;
            return source;
        }
    }
}
