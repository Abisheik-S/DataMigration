using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LimsMigration.Services;

namespace LimsMigration.Controllers
{
    [ApiController]
    [Route("api/v1/migration")]
    public class MigrationController : ControllerBase
    {
        private readonly MigrationService _migrationService;
        public MigrationController(MigrationService migrationService)
        {
            _migrationService = migrationService;
        }

        [HttpPost("run/{mappingGroup}")]
        public async Task<IActionResult> RunMappingGroup(string mappingGroup)
        {
            try
            {
                var txn = await _migrationService.RunMigrationGroupAsync(mappingGroup, User?.Identity?.Name ?? "api");
                return Ok(new { success = true, txn.TransactionId, txn.ExecutionStatus, txn.RecordsAffected, txn.ErrorMessage });
            }
            catch(System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
