using MAuth.AspNetCore.Api.Swaggers;
using MAuth.AspNetCore.Models.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Minio;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Web;

namespace MAuth.AspNetCore.Api.Controllers
{
    /// <summary>
    /// 文件上传、下载
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ApiGroup(ApiGroupNames.USER)]
    public class FileController : ControllerBase
    {
        private readonly IOptions<FileSystemOptions> fsOption;
        private MinioClient minioClient;
        public FileController(
              IOptions<FileSystemOptions> fsOption,
              MinioClient minioClient
            )
        {
            this.fsOption = fsOption;
            this.minioClient = minioClient;
        }

        /// <summary>
        /// formData上传文件
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <returns></returns>
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> CreateFileOnFormdata([FromForm] IFormFile file)
        {
            var relatedDir = $"{DateTime.Now.ToString("yyyy/MM/dd")}/";
            var relatedPath = $"{relatedDir}{Guid.NewGuid().ToString()}{System.IO.Path.GetExtension(file.FileName)}";
            var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;
            var ossPath = string.Empty;
            if (string.IsNullOrEmpty(fsOption.Value.BasePath) == false)
            {
                ossPath = $"{fsOption.Value.BasePath.Trim('/')}/{relatedPath.Trim('/')}";
            }
            else
                ossPath = relatedPath;
            await minioClient.PutObjectAsync(
                                    new PutObjectArgs()
                                    .WithBucket(fsOption.Value.Bucket)
                                    .WithStreamData(ms)
                                    .WithObject(ossPath)
                                    .WithObjectSize(file.Length)
                                    .WithContentType(file.ContentType)
                                    );
            return Ok(relatedPath);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <returns></returns>
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> CreateFile([FromQuery] string name)
        {
            var relatedDir = $"{DateTime.Now.ToString("yyyy/MM/dd")}/";
            var relatedPath = $"{relatedDir}{Guid.NewGuid().ToString()}{System.IO.Path.GetExtension(name)}";
            var ossPath = string.Empty;
            if (string.IsNullOrEmpty(fsOption.Value.BasePath) == false)
            {
                ossPath = $"{fsOption.Value.BasePath.Trim('/')}/{relatedPath.Trim('/')}";
            }
            else
                ossPath = relatedPath;

            await minioClient.PutObjectAsync(
                                    new PutObjectArgs()
                                    .WithBucket(fsOption.Value.Bucket)
                                    .WithStreamData(Request.Body)
                                    .WithObject(ossPath)
                                    .WithObjectSize(Request.ContentLength.GetValueOrDefault())
                                    .WithContentType(Request.ContentType)
                                    );

            return Ok(relatedPath);
        }
        /// <summary>
        /// 获取文件内容
        /// </summary>
        /// <param name="Path">文件Id</param>
        /// <returns></returns>
        [HttpGet("{Path}")]
        public async Task GetFileContent([FromRoute] string Path)
        {
            var storePath = Path;
            if (string.IsNullOrEmpty(fsOption.Value.BasePath) == false)
            {
                storePath = $"{fsOption.Value.BasePath.Trim('/')}/{Path.Trim('/')}";
            }
            storePath = HttpUtility.UrlDecode(storePath);
            if (string.IsNullOrWhiteSpace(fsOption.Value.CdnPath) == false)
            {
                Response.StatusCode = StatusCodes.Status301MovedPermanently;
                Response.GetTypedHeaders().Location = new Uri($"{fsOption.Value.CdnPath}/{storePath}");
            }
            var meta = await minioClient.StatObjectAsync(new StatObjectArgs().WithBucket(fsOption.Value.Bucket).WithObject(storePath));
            if (meta != null)
            {
                //Response.ContentType = result.Type;
                Response.ContentLength = meta.Size;
            }
            await minioClient.GetObjectAsync(new GetObjectArgs().WithBucket(fsOption.Value.Bucket)
                .WithObject(storePath)
                .WithCallbackStream(stream =>
                {
                    stream.CopyToAsync(Response.Body).GetAwaiter().GetResult();
                }));
        }
    }
}
