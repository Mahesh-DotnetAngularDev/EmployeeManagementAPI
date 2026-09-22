using EmployeeManagementApi.Models;
using EmployeeManagementAPI.Data;
using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeManagementAPI.Tests;

public class EmployeeServiceTests
{
    private readonly DbContextOptions<AppDbContext> _options;

    public EmployeeServiceTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsEmployee()
    {
        // Arrange
        await using var context = new AppDbContext(_options);

        var employee = new Employee
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            Department = "Engineering",
            Salary = 65000
        };

        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        var logger = new Mock<ILogger<EmployeeService>>();

        var service = new EmployeeService(
            context,
            logger.Object);

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("Engineering", result.Department);
    }
    [Fact]
    public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        // Arrange
        await using var context = new AppDbContext(_options);

        var logger = new Mock<ILogger<EmployeeService>>();

        var service = new EmployeeService(
            context,
            logger.Object);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }
    [Fact]
    public async Task CreateAsync_WhenEmailAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        await using var context = new AppDbContext(_options);

        context.Employees.Add(new Employee
        {
            FirstName = "Existing",
            LastName = "Employee",
            Email = "existing@test.com",
            Department = "Engineering",
            Salary = 60000
        });

        await context.SaveChangesAsync();

        var logger = new Mock<ILogger<EmployeeService>>();

        var service = new EmployeeService(
            context,
            logger.Object);

        var dto = new CreateEmployeeDto
        {
            FirstName = "New",
            LastName = "Employee",
            Email = "existing@test.com",
            Department = "Engineering",
            Salary = 65000
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateAsync(dto));

        Assert.Equal(
            "An employee with this email already exists.",
            exception.Message);
    }
    [Fact]
    public async Task CreateAsync_WhenValidEmployeeProvided_CreatesEmployee()
    {
        // Arrange
        await using var context = new AppDbContext(_options);

        var logger = new Mock<ILogger<EmployeeService>>();

        var service = new EmployeeService(
            context,
            logger.Object);

        var dto = new CreateEmployeeDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@test.com",
            Department = "HR",
            Salary = 55000
        };

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("Smith", result.LastName);
        Assert.Equal("jane.smith@test.com", result.Email);
        Assert.Equal("HR", result.Department);
        Assert.Equal(55000, result.Salary);

        var savedEmployee = await context.Employees
            .FirstOrDefaultAsync(e => e.Email == "jane.smith@test.com");

        Assert.NotNull(savedEmployee);
    }
    [Fact]
    public async Task GetAllAsync_WhenDepartmentProvided_ReturnsOnlyMatchingEmployees()
    {
        // Arrange
        await using var context = new AppDbContext(_options);

        context.Employees.AddRange(
            new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                Department = "Engineering",
                Salary = 65000
            },
            new Employee
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@test.com",
                Department = "HR",
                Salary = 55000
            },
            new Employee
            {
                FirstName = "Mike",
                LastName = "Brown",
                Email = "mike@test.com",
                Department = "Engineering",
                Salary = 70000
            });

        await context.SaveChangesAsync();

        var logger = new Mock<ILogger<EmployeeService>>();

        var service = new EmployeeService(
            context,
            logger.Object);

        // Act
        var result = await service.GetAllAsync(
            pageNumber: 1,
            pageSize: 10,
            search: null,
            department: "Engineering",
            sortBy: null,
            sortOrder: null);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Employees.Count);
        Assert.All(
            result.Employees,
            employee => Assert.Equal("Engineering", employee.Department));
    }

    [Fact]
public async Task GetAllAsync_WhenPagingIsUsed_ReturnsCorrectPage()
    {
        // Arrange
        await using var context = new AppDbContext(_options);

        for (int i = 1; i <= 5; i++)
        {
            context.Employees.Add(new Employee
            {
                FirstName = $"Employee{i}",
                LastName = "Test",
                Email = $"employee{i}@test.com",
                Department = "Engineering",
                Salary = 50000 + i
            });
        }

        await context.SaveChangesAsync();

        var logger = new Mock<ILogger<EmployeeService>>();
        var service = new EmployeeService(context, logger.Object);

        // Act
        var result = await service.GetAllAsync(
            pageNumber: 2,
            pageSize: 2,
            search: null,
            department: null,
            sortBy: null,
            sortOrder: null);

        // Assert
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(2, result.Employees.Count);
    }
    [Fact]
    public async Task GetAllAsync_WhenSortingBySalaryDescending_ReturnsHighestSalaryFirst()
    {
        // Arrange
        await using var context = new AppDbContext(_options);

        context.Employees.AddRange(
            new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.sort@test.com",
                Department = "Engineering",
                Salary = 50000
            },
            new Employee
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.sort@test.com",
                Department = "Engineering",
                Salary = 80000
            },
            new Employee
            {
                FirstName = "Mike",
                LastName = "Brown",
                Email = "mike.sort@test.com",
                Department = "Engineering",
                Salary = 60000
            });

        await context.SaveChangesAsync();

        var logger = new Mock<ILogger<EmployeeService>>();
        var service = new EmployeeService(context, logger.Object);

        // Act
        var result = await service.GetAllAsync(
            pageNumber: 1,
            pageSize: 10,
            search: null,
            department: null,
            sortBy: "salary",
            sortOrder: "desc");

        // Assert
        Assert.Equal(3, result.Employees.Count);
        Assert.Equal(80000, result.Employees[0].Salary);
        Assert.Equal(60000, result.Employees[1].Salary);
        Assert.Equal(50000, result.Employees[2].Salary);
    }
}