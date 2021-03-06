using System;
using System.IO;
using System.Threading.Tasks;
using AcctOpeningImageValidationAPI.Models;
using AcctOpeningImageValidationAPI.Repository.Abstraction;
using AcctOpeningImageValidationAPI.Services;
using IdentificationValidationLib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace XFUploadFile.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChunkController : ControllerBase
    {
        private readonly ILogger<ChunkController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly AppSettings _setting;
        private readonly IFaceRepository _faceRepository;
        private readonly IconFluxOnebankIDCardsContext _context;
        private readonly IHubContext<NotificationHub> _hub;
        public ChunkController(ILogger<ChunkController> logger,IWebHostEnvironment environment, IOptions<AppSettings> options, IFaceRepository faceRepository, IconFluxOnebankIDCardsContext context, IHubContext<NotificationHub> hub)
        {
            _setting = options.Value;
            _logger = logger;
            _faceRepository = faceRepository;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _context = context;
            _hub = hub;
        }

        [HttpPost]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<IActionResult> Post([FromQuery] string userIdentification)
        {
            try
            {
                if(string.IsNullOrEmpty(userIdentification))
                {
                    return BadRequest();
                }

                var httpRequest = HttpContext.Request;

                if (httpRequest.Form.Files.Count > 0)
                {
                    foreach (var file in httpRequest.Form.Files)
                    {
                        var filePath = Path.Combine(_environment.ContentRootPath, userIdentification, "chunks");

                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);

                        await using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream); await System.IO.File.WriteAllBytesAsync(Path.Combine(filePath, $"{file.FileName}.mp4"), memoryStream.ToArray()); //file.FileName
                        }

                        return Ok();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");
                return new StatusCodeResult(500);
            }

            return BadRequest();
        }

        /// <summary>
        /// Start uploading a new file to the server.
        /// This method will allocate a unique file handle and create an empty file in the temporary upload folder.
        /// </summary>
        /// <param name="fileName">The name of the file to upload. This name will be used in the created file handle.</param>
        /// <returns>The created file handle when the file was successfully allocated. Or an error if a file with that name is already being uploaded.</returns>
        [HttpGet]
        [Route("BeginFileUpload")]
        public async Task<IActionResult> BeginFileUpload([FromQuery] string fileName, [FromQuery] string userIdentification)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("The fileName that was specified was null.");

            var filePath = Path.Combine(_environment.ContentRootPath, userIdentification, "temp");

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath); //Create the temp upload directory if it doesn't exist yet.

            //fileName = fileName.Substring(0, fileName.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase)); //Remove the extension.
            var tempFileName = $"{fileName}{Guid.NewGuid()}.{_setting.LivenessVideoFormat}"; //Build the temp filename. //{Path.GetExtension(fileName)}

            try
            {
                //Create a new empty file that will be filled later chunk by chunk.
                var fs = new FileStream(Path.Combine(filePath, tempFileName), FileMode.CreateNew);
                fs.Close();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");
                return new StatusCodeResult(500);
            }

            return Ok(tempFileName);
        }

        /// <summary>
        /// Upload a part of a media file.
        /// This method takes a part of the media file and appends it to the incomplete file. This method is to be called repeatedly until the upload is complete.
        /// </summary>
        /// <param name="mediaChunk">The chunk of the media file to upload.</param>
        /// <returns>Returns the Ok code if the chunk was uploaded and appended successfully. Or an error when it failed.</returns>
        [HttpPost]
        [Route("UploadChunk")]
        public async Task<IActionResult> UploadChunk(MediaChunk mediaChunk)
        {
            try
            {
                var path = Path.Combine(_environment.ContentRootPath, mediaChunk.UserIdentification, "temp", mediaChunk.FileHandle);
                var fileInfo = new FileInfo(path);
                var start = Convert.ToInt64(mediaChunk.StartAt);

                if (!fileInfo.Exists)
                    return NotFound(); //Temp file not found, maybe BeginFileUpload was not called?

                if (fileInfo.Length != start)
                    return BadRequest(); //The temp file is not the same length as the starting position of the next chunk, Maybe they are sent out of order?

                try
                {
                    using var fs = new FileStream(path, FileMode.Append);

                    var bytes = Convert.FromBase64String(mediaChunk.Data);
                    fs.Write(bytes, 0, bytes.Length);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error");
                    return new StatusCodeResult(500);
                }

                return Ok();

            }catch(Exception e) { return new StatusCodeResult(500);  }
        }

        /// <summary>
        /// Finish a file upload and copy the completed file to the upload folder so it can be streamed or retrieved.
        /// </summary>
        /// <param name="fileHandle">The file handle of the file that the upload is complete for.</param>
        /// <param name="quitUpload">If this is true the file upload will be aborted. The temporary file will be deleted.</param>
        /// <param name="fileSize">The size of the original file that was uploaded. This is used to check if the upload was successful.</param>
        /// <returns>Code Ok if the upload was successfully ended. Code 404 if the file handle was not found. Or code 500 if the file could not be moved or deleted.</returns>
        [HttpGet]
        [Route("EndFileUpload")]
        public async Task<IActionResult> EndFileUpload(string fileHandle, string userIdentification, bool quitUpload, long fileSize)
        {
            var fileInfo = new FileInfo(Path.Combine(_environment.ContentRootPath, userIdentification, "temp", fileHandle));
            if (!fileInfo.Exists)
                return NotFound(); //Temp file not found, maybe BeginFileUpload was not called?

            try
            {
                if (quitUpload)
                    fileInfo.Delete(); //Upload is being aborted, so the temp file is no longer needed.
                else
                {
                    if (fileInfo.Length != fileSize)
                        return Conflict(); //The local file does not have the same size as the file that was uploaded. This could indicate the upload was not completed properly.

                    var chunkPath = Path.Combine(_environment.ContentRootPath, userIdentification, "chunks");

                    if(!Directory.Exists(chunkPath))
                    {
                        Directory.CreateDirectory(chunkPath);
                    }

                    var newFile = new FileInfo(Path.Combine(chunkPath, fileHandle));

                    if (newFile.Exists)
                        newFile.Delete(); //Delete a file with the same name if it already exists, effectively overwriting it.

                    fileInfo.MoveTo(newFile.FullName); //Move the completed file to the main upload directory.

                    _ = Task.Run(async () =>
                      {
                          (var success, var message) = _faceRepository.ExtractFrameFromVideo(chunkPath, fileHandle);

                          if (success)
                          {
                              //TODO: Run Facial Recognition Algorithm
                              EyeBlinkResult faceGestureResults = await _faceRepository.RunEyeBlinkAlgorithm(chunkPath, userIdentification);

                              //TODO : Delete Folder
                              var chunkFiles = Directory.GetFiles(chunkPath);

                              var tempFiles = Directory.GetFiles(Path.Combine(_environment.ContentRootPath, userIdentification, "temp"));

                              foreach (var file in chunkFiles)
                              {
                                  if (System.IO.File.Exists(file))
                                  {
                                      System.IO.File.Delete(file);
                                  }
                              }

                              Directory.Delete(chunkPath);

                              foreach (var tempFile in tempFiles)
                              {
                                  if (System.IO.File.Exists(tempFile))
                                  {
                                      System.IO.File.Delete(tempFile);
                                  }
                              }

                              Directory.Delete(Path.Combine(_environment.ContentRootPath, userIdentification, "temp"));

                              var dataResponse = HelperLib.ReponseClass.ReponseMethodGeneric("Successful", faceGestureResults, true);

                              var result = Newtonsoft.Json.JsonConvert.SerializeObject(dataResponse);

                              await _hub.Clients.All.SendAsync(_setting.SignalrEventName, result);
                          } else {

                              var dataResponse = HelperLib.ReponseClass.ReponseMethod("Unsucessful", false);

                              var result = Newtonsoft.Json.JsonConvert.SerializeObject(dataResponse);

                              await _hub.Clients.All.SendAsync(_setting.SignalrEventName, result);
                          }
                      });

                    return Ok(HelperLib.ReponseClass.ReponseMethod("Successful", true));
                }
            }
            catch (Exception e)
            {
                _context.RequestLog.Add(new RequestLogs { Description = $"Message : {e.Message}    |   StackTrace  : {e.StackTrace} " });
                _context.SaveChanges();
                _logger.LogError(e, "Error");
                return BadRequest(HelperLib.ReponseClass.ReponseMethod("Unsucessful", false));

            }

            return Ok();
        }
    }
}