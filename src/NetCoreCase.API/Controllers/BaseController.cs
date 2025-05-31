using Microsoft.AspNetCore.Mvc;

namespace NetCoreCase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Başarılı response döndürür
    /// </summary>
    protected ActionResult SuccessResult<T>(T data, string? message = null)
    {
        var response = new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "İşlem başarılı.",
            Timestamp = DateTime.UtcNow
        };

        return Ok(response);
    }

    /// <summary>
    /// Başarılı response döndürür (data olmadan)
    /// </summary>
    protected ActionResult SuccessResult(string? message = null)
    {
        var response = new ApiResponse<object>
        {
            Success = true,
            Data = null,
            Message = message ?? "İşlem başarılı.",
            Timestamp = DateTime.UtcNow
        };

        return Ok(response);
    }

    /// <summary>
    /// Not Found response döndürür
    /// </summary>
    protected ActionResult NotFoundResult(string? message = null)
    {
        var response = new ApiResponse<object>
        {
            Success = false,
            Data = null,
            Message = message ?? "Kayıt bulunamadı.",
            Timestamp = DateTime.UtcNow
        };

        return NotFound(response);
    }

    /// <summary>
    /// Bad Request response döndürür
    /// </summary>
    protected ActionResult BadRequestResult(string? message = null)
    {
        var response = new ApiResponse<object>
        {
            Success = false,
            Data = null,
            Message = message ?? "Geçersiz istek.",
            Timestamp = DateTime.UtcNow
        };

        return BadRequest(response);
    }

    /// <summary>
    /// Created response döndürür
    /// </summary>
    protected ActionResult CreatedResult<T>(T data, string? message = null)
    {
        var response = new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Kayıt başarıyla oluşturuldu.",
            Timestamp = DateTime.UtcNow
        };

        return Created(string.Empty, response);
    }

    /// <summary>
    /// Created response döndürür (location ile)
    /// </summary>
    protected ActionResult CreatedResult<T>(string location, T data, string? message = null)
    {
        var response = new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Kayıt başarıyla oluşturuldu.",
            Timestamp = DateTime.UtcNow
        };

        return Created(location, response);
    }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? TraceId { get; set; }
}

public class PagedResponse<T> : ApiResponse<IEnumerable<T>>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
} 