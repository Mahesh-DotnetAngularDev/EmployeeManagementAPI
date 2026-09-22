using EmployeeManagementApi.Models;
using EmployeeManagementAPI.DTOs;

namespace EmployeeManagementAPI.Services;

public interface IEmployeeService
{
    Task<(List<Employee> Employees, int TotalCount)> GetAllAsync(
    int pageNumber,
    int pageSize,
    string? search,
    string? department,
    string? sortBy,
    string? sortOrder);

    Task<Employee?> GetByIdAsync(int id);

    Task<Employee> CreateAsync(CreateEmployeeDto dto);

    Task<bool> UpdateAsync(int id, UpdateEmployeeDto dto);

    Task<bool> DeleteAsync(int id);
}