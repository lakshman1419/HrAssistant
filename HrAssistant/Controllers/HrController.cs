using HrAssistant.Agents;
using Microsoft.AspNetCore.Mvc;

namespace HrAssistant.Controllers
{
    [ApiController]
    [Route("api/hr")]
    public class HrController : ControllerBase
    {
        private readonly HrAgent _agent;

        public HrController(HrAgent agent)
        {
            _agent = agent;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask(
            [FromBody] string question)
        {
            var answer = await _agent.Ask(question);

            return Ok(new
            {
                answer
            });
        }
    }
}
