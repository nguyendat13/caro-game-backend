using backend.DTOs.Role;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // ✅ Lấy tất cả Role
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        // ✅ Lấy Role theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var role = await _roleService.GetByIdAsync(id);
                return Ok(role);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // ✅ Tạo mới Role
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleCreateDTO dto)
        {
            try
            {
                var role = await _roleService.CreateAsync(dto);
                return Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Cập nhật Role
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoleUpdateDTO dto)
        {
            if (id != dto.RoleId)
                return BadRequest(new { message = "ID không khớp." });

            try
            {
                var role = await _roleService.UpdateAsync(dto);
                return Ok(role);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // ✅ Xóa Role
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool result = await _roleService.DeleteAsync(id);
                return Ok(new { success = result, message = "Role đã được xóa." });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
