using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Intefaces;

namespace Simulatore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SimulatorController : ControllerBase
    {
        private readonly ISmartWatchService _smartWatchService;
        public SimulatorController(ISmartWatchService smartWatchService)
        {
            _smartWatchService = smartWatchService;
        }
        [HttpPost]
        public async Task<IActionResult> RunSession(TimeSpan SessionTime, CancellationToken cancellationToken = default)
        {
            Guid watchId = Guid.NewGuid();
            var userId = Guid.Parse(User.Claims.First(x => x.Type == "id").Value);

            await _smartWatchService.CreateSessionAsync(watchId, userId, SessionTime, cancellationToken);

            return Ok();
        }
    }
}
