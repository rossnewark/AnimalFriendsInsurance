using CustomerAPI.Models;
using CustomerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(
            IRegistrationService registrationService,
            ILogger<RegistrationController> logger)
        {
            _registrationService = registrationService;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new customer for the Animla Friends Insurance Customer Portal
        /// </summary>
        /// <param name="registrationRequest">Customer registration details</param>
        /// <returns>A unique customer ID</returns>
        /// <response code="201">Returns the newly created customer ID</response>
        /// <response code="400">If the registration request is invalid</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RegistrationResponse>> Register(
            [FromBody] RegistrationRequest registrationRequest)
        {
            try
            {
                if (registrationRequest == null)
                {
                    _logger.LogWarning("Received null registration request.");
                    return BadRequest("Request body cannot be null.");
                }

                if (!TryValidateModel(registrationRequest))
                {
                    _logger.LogWarning("Model validation failed: {@ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(ModelState);
                }
                
                int customerId = await _registrationService.Register(registrationRequest);
                
                RegistrationResponse response = new RegistrationResponse 
                { 
                    CustomerId = customerId,
                    Message = "Customer registration successful!" 
                };
                
                return CreatedAtAction(nameof(Register), response);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error during customer registration");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during customer registration");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { error = "An error occurred processing your registration" });
            }
        }
    }
}