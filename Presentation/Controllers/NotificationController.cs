using Infrastructures.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly FirebaseServices _fcmService;

        public NotificationController(FirebaseServices fcmService)
        {
            _fcmService = fcmService;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification(string token, string title, string body)
        {
            await _fcmService.SendNotificationAsync(token, title, body);
            return Ok();
        }
    }
}
