namespace ProductApi.Contracts;
using ProductApi.DTOs;
using ProductApi.Models;

public interface IUserService
{
    Task<ApiResponse<IEnumerable<UserDto>>> GetAllAsync();
    Task<ApiResponse<UserDto>> GetByIdAsync(int id);
    Task<ApiResponse<UserDto>> CreateAsync(UserCreateDto dto);
    Task<ApiResponse<UserDto>> UpdateAsync(
        int id,
        UserUpdateDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<PagedResultDto<UserDto>>> GetPagedAsync(
        UserFilterDto filter);
    Task<UserDto?> ValidateUserAsync(
        string userName,
        string password);
}