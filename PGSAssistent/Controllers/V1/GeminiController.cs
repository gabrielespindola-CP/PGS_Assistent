using Microsoft.AspNetCore.Mvc;
using PGSAssistent.Configuration;
using PGSAssistent.Services;
using PGSAssistentAPI.DTOs;

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
        public async Task<IActionResult> Health() 
        {
            string result = await _geminiService.ExececutePrompt();
            return Ok(result);
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AskDto ask, CancellationToken cancelationToken)
        {
            if (string.IsNullOrEmpty(ask.Prompt)) return BadRequest("Por favor digite sua pergunta.");

            var result = await _geminiService.ExecutePromptAsync(
                prompt: ask.Prompt,
                ct: cancelationToken
            );
            return Ok(result);
        }
    }
}
