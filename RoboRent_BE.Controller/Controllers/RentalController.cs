using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOS.RentalOrder;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Model.Enums;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Controller]
[Route("api/[controller]")]
public class RentalController : ControllerBase
{
    private readonly IRentalService _rentalService;
    private readonly INotificationService _notificationService;
    private readonly IAccountService _accountService;

    public RentalController(
        IRentalService rentalService,
        INotificationService notificationService,
        IAccountService accountService)
    {
        _rentalService = rentalService;
        _notificationService = notificationService;
        _accountService = accountService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> AddRental([FromBody] CreateOrderRequest createOrderRequest)
    {
        try
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

            var result = await _rentalService.CreateRentalAsync(createOrderRequest);

            if (result == null)
            {
                return BadRequest("Could not create new rental");
            }

            return Ok(result);
        }
        catch (ArgumentException ex)   // ⬅⬅⬅ CATCH VALIDATION ERRORS PROPERLY
        {
            return BadRequest(new
            {
                success = false,
                errors = new List<string> { ex.Message }
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateRental([FromBody] UpdateOrderRequest updateOrderRequest)
    {
        try
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

            var result = await _rentalService.UpdateRentalAsync(updateOrderRequest);

            if (result == null)
            {
                return BadRequest("Could not find valid data");
            }

            return Ok(result);
        }
        catch (ArgumentException ex)   // ⬅⬅⬅ CATCH VALIDATION ERRORS PROPERLY
        {
            return BadRequest(new
            {
                success = false,
                errors = new List<string> { ex.Message }
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRentalsById(int id)
    {
        var result = await _rentalService.GetRentalAsync(id);

        if (result == null) return BadRequest("Could not find rental");

        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllRentalsAsync()
    {
        var result = await _rentalService.GetAllRentalsAsync();

        if (result == null) return BadRequest("There are no rentals exist");

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRentalById(int id)
    {
        var result = await _rentalService.DeleteRentalAsync(id);

        if (result == null) return BadRequest("Could not find rental");

        return Ok(result);
    }

    // [Authorize]
    [HttpGet("my-rentals/{accountId}")]
    public async Task<IActionResult> GetRentalsByCustomer(int accountId)
    {
        var result = await _rentalService.GetRentalsByCustomerAsync(accountId);
        return Ok(result ?? new List<OrderResponse>());
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetRentalByCustomerId(int customerId, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        try
        {
            var result = await _rentalService.GetRentalByCustomerIdAsync(customerId, page, pageSize, search);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("customer/send/{rentalId}")]
    public async Task<IActionResult> SendRequest(int rentalId)
    {
        try
        {
            var result = await _rentalService.CustomerSendRentalAsync(rentalId);

            if (result == null) return NotFound("Could not found rental");

            // 🔔 Notify all Staff about new request
            var staffAccounts = await _accountService.GetAllStaffAccountsAsync();
            if (staffAccounts != null)
            {
                await _notificationService.CreateNotificationsAsync(
                    staffAccounts.Select(s => s.Id),
                    NotificationType.NewRequest,
                    $"📥 Yêu cầu mới #{rentalId} từ khách hàng. Vui lòng nhận xử lý.",
                    rentalId,
                    rentalId,
                    isRealTime: true);
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("staff/get/pending/rentals")]
    public async Task<IActionResult> GetAllPendingRentals()
    {
        try
        {
            var res = await _rentalService.GetAllPendingRentalsAsync();
            return Ok(new
            {
                success = true,
                data = res
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPut("staff/receive/{rentalId}/{staffId}")]
    public async Task<IActionResult> ReceiveRequest(int rentalId, int staffId)
    {
        try
        {
            var res = await _rentalService.ReceiveRequestAsync(rentalId, staffId);

            if (res == null) return BadRequest("Could not find rental");

            // 🔔 Notify Customer that Staff received their request
            if (res.AccountId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(
                    res.AccountId.Value,
                    NotificationType.RequestReceived,
                    $"✅ Yêu cầu #{rentalId} đã được nhân viên tiếp nhận. Chúng tôi sẽ liên hệ sớm.",
                    rentalId,
                    rentalId,
                    isRealTime: true);
            }

            return Ok(new
            {
                success = true,
                data = res
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpGet("staff/get/received/rentals/{staffId}")]
    public async Task<IActionResult> GetReceivedRequest(int staffId)
    {
        try
        {
            var res = await _rentalService.GetAllReceivedRentalsByStaffId(staffId);
            
            return Ok(new
            {
                success = true,
                data = res
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPut("staff/update/rental/{rentalId}")]
    public async Task<IActionResult> StaffUpdateRentalInfo(int rentalId, [FromBody] StaffUpdateRequest request)
    {
        try
        {
            var res = await _rentalService.StaffUpdateRentalInfoAsync(rentalId, request);
            
            if (res == null) return NotFound(new
            {
                success = false,
                message = "Could not find rental"
            });
            
            return Ok(new
            {
                success = true,
                data = res
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPut("customer/cancel/rental/{rentalId}")]
    public async Task<IActionResult> CustomerCancelRental(int rentalId)
    {
        try
        {
            var res = await _rentalService.CustomerCancelRentalAsync(rentalId);

            if (res == null) return NotFound(new
            {
                success = false,
                message = "Could not find rental"
            });

            // 🔔 Notify Staff that Customer cancelled
            if (res.StaffId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(
                    res.StaffId.Value,
                    NotificationType.RequestCancelled,
                    $"❌ Khách hàng đã hủy yêu cầu #{rentalId}.",
                    rentalId,
                    rentalId,
                    isRealTime: true);
            }

            return Ok(new
            {
                success = true,
                data = res
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPut("customer/delete/rental/{rentalId}")]
    public async Task<IActionResult> CustomerDeleteRental(int rentalId)
    {
        try
        {
            var res = await _rentalService.CustomerDeleteRentalAsync(rentalId);

            if (res == null) return NotFound(new
            {
                success = false,
                message = "Could not find rental"
            });

            return Ok(new
            {
                success = true,
                data = res
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPut("staff/request/update/rental/{rentalId}")]
    public async Task<IActionResult> StaffUpdateRequest(int rentalId)
    {
        try
        {
            var res = await _rentalService.StaffRequestRentalUpdateAsync(rentalId);

            if (res == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Could not find rental."
                });
            }

            // 🔔 Notify Customer that Staff requests update
            if (res.AccountId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(
                    res.AccountId.Value,
                    NotificationType.RequestUpdate,
                    $"📝 Nhân viên yêu cầu cập nhật thông tin cho yêu cầu #{rentalId}. Vui lòng kiểm tra.",
                    rentalId,
                    rentalId,
                    isRealTime: true);
            }

            return Ok(new
            {
                success = true,
                data = res
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }
    
    /// <summary>
    /// [STAFF/MANAGER] Mark rental as completed after event finished
    /// This will trigger Full payment creation
    /// </summary>
    [HttpPut("{rentalId}/complete")]
    public async Task<IActionResult> CompleteRental(int rentalId)
    {
        try
        {
            var result = await _rentalService.CompleteRentalAsync(rentalId);
        
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Rental not found"
                });
            }
        
            return Ok(new
            {
                success = true,
                message = "Rental completed successfully",
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }
}