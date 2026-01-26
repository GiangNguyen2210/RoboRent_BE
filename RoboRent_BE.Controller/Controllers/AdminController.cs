using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOS.Admin;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IModifyIdentityUserService  _modifyIdentityUserService;
    private readonly IAccountService _accountService;
    

    public AdminController(IHttpClientFactory httpClientFactory, IHostEnvironment hostEnvironment, IModifyIdentityUserService modifyIdentityUserService,  IAccountService accountService)
    {
        _httpClientFactory = httpClientFactory;
        _hostEnvironment = hostEnvironment;
        _modifyIdentityUserService = modifyIdentityUserService;
        _accountService = accountService;
    }

    //them URL: https://provinces.open-api.vn/api/v2/p/
    [HttpGet("address/province")]
    public async Task<IActionResult> GetProvinces([FromQuery] string? URL)
    {
        if (string.IsNullOrWhiteSpace(URL))
        {
            return BadRequest("URL is required");
        }

        if (!Uri.TryCreate(URL, UriKind.Absolute, out Uri? uriResult) 
            || (uriResult.Scheme == Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
        {
            return BadRequest("Invalid URL");
        }

        try
        {
            // ✅ Ignore SSL certificate
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = 
                    (sender, cert, chain, sslPolicyErrors) => true
            };
            
            using var client = new HttpClient(handler);

            var response = await client.GetAsync(URL);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to fetch from URL");
            }
            
            string jsonString = await response.Content.ReadAsStringAsync();
            return Content(jsonString, "application/json");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error fetching data: {e.Message}");
        }
    }
    
    //them URL: https://provinces.open-api.vn/api/v2/w/
    [HttpGet("address/ward")]
    public async Task<IActionResult> GetWards([FromQuery] string? URL)
    {
        if (string.IsNullOrWhiteSpace(URL))
        {
            return BadRequest("URL is required");
        }

        if (!Uri.TryCreate(URL, UriKind.Absolute, out Uri? uriResult) 
            || (uriResult.Scheme == Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
        {
            return BadRequest("Invalid URL");
        }

        try
        {
            // ✅ Ignore SSL certificate
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = 
                    (sender, cert, chain, sslPolicyErrors) => true
            };
            
            using var client = new HttpClient(handler);

            var response = await client.GetAsync(URL);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to fetch from URL");
            }
            
            string jsonString = await response.Content.ReadAsStringAsync();
            return Content(jsonString, "application/json");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error fetching data: {e.Message}");
        }
    }

    [HttpPost("user")]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest createUserRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }
        
        var result = await _modifyIdentityUserService.CreateUserAsync(createUserRequest);

        if (result.Succeeded)
        {
            var user = await _modifyIdentityUserService.GetUserByEmailAsync(createUserRequest.Email);
            var account = await _accountService.CreatePendingAccountAsync(user.Id, "");
            return Ok(new { message = "User created successfully", account});
        }

        return BadRequest("can not create account");
    }

    [HttpPut("user")]
    public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserRequest updateUserRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }
        
        var result  = await _modifyIdentityUserService.UpdateUserAsync(updateUserRequest);

        if (result == null) return BadRequest("User or account not found");
        
        return Ok(new { message = "User updated successfully", account = result});
    }
    
    // AdminController - Thêm endpoint này
    /// <summary>
    /// [MANAGER] Get list of staff for delivery assignment
    /// </summary>
    [HttpGet("staff")]
    public async Task<IActionResult> GetStaffList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] string? searchTerm = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await _modifyIdentityUserService.GetStaffListAsync(page, pageSize, status, searchTerm);
        return Ok(result);
    }
    
    [HttpGet("technical-staff")]
    public async Task<IActionResult> GetTechnicalStaffList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? searchTerm = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;
        
        var result = await _modifyIdentityUserService.GetTechnicalStaffListAsync(page, pageSize, status, searchTerm);
        return Ok(result);
    }
    
    /// <summary>
    /// [STAFF] Get list of managers
    /// </summary>
    [HttpGet("manager")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> GetManagerList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] string? searchTerm = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await _modifyIdentityUserService.GetManagerListAsync(page, pageSize, status, searchTerm);
        return Ok(result);
    }

    [HttpGet("accounts")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAccounts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] string? searchTerm = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;
        var result = await _modifyIdentityUserService.GetAllAccountsAsync(page, pageSize, status, searchTerm);
        return Ok(result);
    }

    [HttpPut("accounts/{accountId}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserStatus(int accountId, [FromBody] UpdateStatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        var result = await _modifyIdentityUserService.UpdateUserStatusAsync(accountId, request.Status);
        if (result)
        {
            return Ok(new { message = "User status updated successfully" });
        }

        return NotFound(new { message = "Account not found" });
    }
}