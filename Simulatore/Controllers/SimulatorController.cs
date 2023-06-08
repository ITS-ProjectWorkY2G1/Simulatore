using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Simulatore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimulatorController : ControllerBase
    {
        private readonly ISmartWatchService _smartWatchService;
        public SimulatorController(ISmartWatchService smartWatchService)
        {
            _smartWatchService = smartWatchService;
        }
        [HttpPost]
        public async Task<IActionResult> RunSession(short SessionTime, Guid userId, CancellationToken cancellationToken = default)
        {
            Guid watchId = Guid.NewGuid();

            try
            {
                await _smartWatchService.CreateSessionAsync(watchId, userId, TimeSpan.FromMinutes(SessionTime), cancellationToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}
