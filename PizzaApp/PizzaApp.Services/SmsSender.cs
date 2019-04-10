using System;
using PizzaApp.Services.Abstract;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace PizzaApp.Services
{
    public class SmsSender : ISender
    {
        public string SendMessage(string path)
        {
            Random random = new Random();
            string smsCode = random.Next(1111, 9999).ToString();

            const string accountSid = "AC7ee9ce3104b8f87e7caf3d1b17eb899e";
            const string authToken = "7585663101907a2ac2f550e48213d465";

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: $"Ваш код подтверждения: {smsCode}",
                from: new Twilio.Types.PhoneNumber("+1 971 407 1753"),
                to: new Twilio.Types.PhoneNumber("+77027779674")
            );

            return smsCode;
        }
    }
}
