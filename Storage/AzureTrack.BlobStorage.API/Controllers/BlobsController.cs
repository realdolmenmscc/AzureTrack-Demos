using Microsoft.AspNetCore.Mvc;

namespace AzureTrack.BlobStorage.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobsController : ControllerBase
    {
        private BlobService BlobService { get; }

        public BlobsController(BlobService blobService)
        {
            BlobService = blobService;
        }

        [HttpGet("{category}")]
        public IActionResult Find(string category)
        {
            BlobModel[] data = BlobService.GetContainerItems($"{category}-thumbs");
            return Ok(data);
        }

        [HttpGet("{category}/{name}")]
        public IActionResult Get(string category, string name)
        {
            name = name.Replace("_", "/");
            return Redirect(BlobService.GetSasUri(category, name));
        }
    }
}
