using EmployeeManagementApi.Models;
using EmployeeManagementAPI.Data;
using EmployeeManagementAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.Globalization;

namespace EmployeeManagementAPI.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(
    AppDbContext context,
    ILogger<EmployeeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(List<Employee> Employees, int TotalCount)> GetAllAsync(
    int pageNumber,
    int pageSize,
    string? search,
    string? department,
        string? sortBy,
    string? sortOrder)
    {
        var query = _context.Employees
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e =>
                e.FirstName.Contains(search) ||
                e.LastName.Contains(search) ||
                e.Email.Contains(search) ||
                e.Department.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(department))
        {
            query = query.Where(e => e.Department == department);
        }

        var totalCount = await query.CountAsync();
        query = sortBy?.ToLower() switch
        {
            "firstname" => sortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(e => e.FirstName)
                : query.OrderBy(e => e.FirstName),

            "lastname" => sortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(e => e.LastName)
                : query.OrderBy(e => e.LastName),

            "salary" => sortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(e => e.Salary)
                : query.OrderBy(e => e.Salary),

            "department" => sortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(e => e.Department)
                : query.OrderBy(e => e.Department),

            _ => query.OrderBy(e => e.Id)
        };

        var employees = await query
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

        return (employees, totalCount);
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee> CreateAsync(CreateEmployeeDto dto)
    {
        var emailExists = await _context.Employees
            .AnyAsync(e => e.Email == dto.Email);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "An employee with this email already exists.");
        }

        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Department = dto.Department,
            Salary = dto.Salary
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        _logger.LogInformation(
    "Employee created successfully. EmployeeId: {EmployeeId}, Email: {Email}",
    employee.Id,
    employee.Email);

        return employee;
    }

    public async Task<bool> UpdateAsync(int id, UpdateEmployeeDto dto)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return false;
        }

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;
        employee.Department = dto.Department;
        employee.Salary = dto.Salary;

        await _context.SaveChangesAsync();
        _logger.LogInformation(
    "Employee updated successfully. EmployeeId: {EmployeeId}",
    id);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return false;
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        _logger.LogInformation(
    "Employee deleted successfully. EmployeeId: {EmployeeId}",
    id);

        return true;
    }
}