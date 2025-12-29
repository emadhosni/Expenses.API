using Expenses.API.Data.Services;
using Expenses.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Expenses.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    [Authorize]
    public class TransactionsController(ITransactionsService transactionService) : ControllerBase
    {
        [HttpGet("All")]
        public IActionResult GetAll()
        {
            var nameIdentifierClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifierClaim))
                return BadRequest("Could not find the user Id.");

            if (!int.TryParse(nameIdentifierClaim, out int userId) || userId <= 0)
                return BadRequest("Invalid user Id.");

            var allTransactions = transactionService.GetAll(userId);
            return Ok(allTransactions);
        }

        [HttpGet("Details/{id}")]
        public IActionResult Get(int id)
        {
            var transactionDb = transactionService.GetById(id);
            if (transactionDb == null)
                return NotFound();

            return Ok(transactionDb);
        }

        [HttpPost("Create")]
        public IActionResult CreateTransaction([FromBody]PostTransactionDto payload)
        {
            var nameIdentifierClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifierClaim))
                return BadRequest("Could not find the user Id.");

            if(!int.TryParse(nameIdentifierClaim, out int userId) || userId <= 0)
                return BadRequest("Invalid user Id.");

            var newTransaction = transactionService.Add(payload, userId);
            return Ok(newTransaction);
        }

        [HttpPut("Update/{id}")]
        public IActionResult UpdateTransaction(int id, [FromBody]PutTransactionDto payload)
        {
            var updatedTransaction = transactionService.Update(id, payload);
            if (updatedTransaction == null)
                return NotFound();

            return Ok(updatedTransaction);
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteTransaction(int id)
        {
            transactionService.Delete(id);
            return Ok();
        }
    }
}
