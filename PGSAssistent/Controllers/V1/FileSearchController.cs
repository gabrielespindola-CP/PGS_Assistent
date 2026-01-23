using Microsoft.AspNetCore.Mvc;
using PGSAssistent.Services;
using PGSAssistentAPI.DTOs.ConversationalDTOs;
using PGSAssistentAPI.DTOs.FileSearchDTOs;
using PGSAssistentAPI.Services;

namespace PGSAssistentAPI.Controllers.V1
{
    [ApiController]
    [Route("filesearch/v1")]
    public class FileSearchController : ControllerBase
    {
        private readonly FileSearchService _fileSearch;

        public FileSearchController(FileSearchService fileSearch)
        {
            _fileSearch = fileSearch;
        }

        [HttpGet("create")]
        public async Task<IActionResult> CreateFileSearch()
        {
            var result = await _fileSearch.Create();
            return Ok(result);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles([FromBody] UploadFolderDto uploader, CancellationToken cancelationToken)
        {
            await _fileSearch.UploadAllPdfsFromFolder(uploader.Url);
            return NoContent();
        }
    }
}
