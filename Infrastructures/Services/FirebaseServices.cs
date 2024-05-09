using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder.Extensions;
using System.Threading.Tasks;

namespace Infrastructures.Services
{


    public class FirebaseServices
    {
        private readonly FirebaseApp _app;

        public FirebaseServices()
        {
            _app = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("C:\\Users\\Mayung Hang Rai\\Downloads\\pushnotification-75624-firebase-adminsdk-6xvjd-b74d136bb7.json")
            });
        }

        public async Task SendNotificationAsync(string token, string title, string body)
        {
            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }

}
