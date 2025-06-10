using AppCreditosBackEnd.Application.DTOs;
using AppCreditosBackEnd.Application.Interfaces;
using AppCreditosBackEnd.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppCreditosBackEnd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditApplicationController : ControllerBase
    {
        private readonly ICreditApplicationService _service;

        public CreditApplicationController(ICreditApplicationService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Applicant")]
        public async Task<ActionResult<CreditApplicationResponseDto>> Create(CreateCreditApplicationDto request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _service.CreateApplicationAsync(userId, request);
            return Ok(result);
        }


        [HttpGet("my-applications")]
        [Authorize(Roles = "Applicant")]
        public async Task<ActionResult<List<CreditApplicationResponseDto>>> GetMyApplications()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _service.GetUserApplicationsAsync(userId);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult<List<CreditApplicationResponseDto>>> GetAll(
            [FromQuery] ApplicationStatus? status = null)
        {
            var result = await _service.GetAllApplicationsAsync(status);
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult<CreditApplicationResponseDto>> UpdateStatus(
            int id, UpdateStatusDto request)
        {
            var analystId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _service.UpdateStatusAsync(id, request, analystId);
            return Ok(result);
        }

        //[HttpPut("{id}/status-sp")]
        //[Authorize(Roles = "Analyst")]
        //public async Task<ActionResult> UpdateStatusViaStoredProcedure(
        //    int id, UpdateStatusDto request)
        //{
        //    var analystId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        //    await _service.UpdateStatusViaStoredProcedureAsync(id, request, analystId);
        //    return Ok(new { Message = "Status updated successfully via stored procedure" });
        //}
    }
}
