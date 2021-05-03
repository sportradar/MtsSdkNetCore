/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Newtonsoft.Json;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.Entities.Utils
{
    /// <summary>
    /// Helper class used to serialize tickets
    /// </summary>
    public static class JsonUtils
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor};

        static JsonUtils()
        {
            Settings.Converters.Add(new ConcreteConverter<ITicket, Ticket>());
            Settings.Converters.Add(new ConcreteConverter<IBet, Bet>());
            Settings.Converters.Add(new ConcreteConverter<IBetBonus, BetBonus>());
            Settings.Converters.Add(new ConcreteConverter<IStake, Stake>());
            Settings.Converters.Add(new ConcreteConverter<ISelection, Selection>());
            Settings.Converters.Add(new ConcreteConverter<ISender, Sender>());
            Settings.Converters.Add(new ConcreteConverter<IEndCustomer, EndCustomer>());
            Settings.Converters.Add(new ConcreteConverter<ITicketAck, TicketAck>());
            Settings.Converters.Add(new ConcreteConverter<ITicketCancel, TicketCancel>());
            Settings.Converters.Add(new ConcreteConverter<IBetCancel, BetCancel>());
            Settings.Converters.Add(new ConcreteConverter<ITicketCancelAck, TicketCancelAck>());
            Settings.Converters.Add(new ConcreteConverter<ITicketCashout, TicketCashout>());
            Settings.Converters.Add(new ConcreteConverter<IBetCashout, BetCashout>());
            Settings.Converters.Add(new ConcreteConverter<ITicketNonSrSettle, TicketNonSrSettle>());
            Settings.Converters.Add(new ConcreteConverter<ITicketReofferCancel, TicketReofferCancel>());
        }

        /// <summary>
        /// Serialize given ticket
        /// </summary>
        /// <param name="ticket">Ticket to be serialized</param>
        /// <returns>Json representation of the given ticket</returns>
        public static string Serialize(ISdkTicket ticket)
        {
            return JsonConvert.SerializeObject(ticket);
        }

        /// <summary>
        /// Deserialize given ticket
        /// </summary>
        /// <param name="json">Json to be deserialized</param>
        /// <returns>Created <see cref="ISdkTicket"/> instance</returns>
        public static T Deserialize<T>(string json) where T : ISdkTicket
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        private class ConcreteConverter<I, C> : JsonConverter where C : I
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(I);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return serializer.Deserialize<C>(reader);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }
    }
}
