using Microsoft.AspNetCore.Mvc;
using PGSAssistent.Configuration;
using PGSAssistent.Services;

namespace PGSAssistent.Controllers.V1
{
    [ApiController]
    [Route("PGSAssistent/v1/")]
    public class GeminiController : ControllerBase
    {
        private readonly GeminiService _geminiService;

        public GeminiController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpGet("health")]
        public async Task<IActionResult> Get() 
        {
            string result = await _geminiService.ExececutePrompt();
            return Ok(result);
        }
    }
}
