﻿namespace GustoHub.Services.Services
{
    using System;
    using GustoHub.Data.Common;
    using GustoHub.Data.Models;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using GustoHub.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using GustoHub.Data.ViewModels.POST;
    using GustoHub.Data.ViewModels.GET;
    using GustoHub.Data.ViewModels.PUT;

    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository repository;

        public EmployeeService(IRepository repository)
        {
            this.repository = repository;
        }
        public async Task<string> AddAsync(POSTEmployeeDto employeeDto, Guid userId)
        {
            var user = await repository.GetByIdAsync<User>(userId);

            Employee employee = new Employee()
            {
                Name = employeeDto.Name,
                Title = employeeDto.Title,
                HireDate = DateTime.Now,
                IsActive = true,
                EmployeeUserId = userId,
                User = user
            };

            user.EmployeeId = employee.Id;

            await repository.AddAsync(employee);
            await repository.SaveChangesAsync();

            return "Employee added Successfully!";
        }

        public async Task<IEnumerable<GETEmployeeDto>> AllActiveAsync()
        {
            List<GETEmployeeDto> employeeDtos = await repository.AllAsReadOnly<Employee>()
                .Where(e => e.IsActive)
                .Select(e => new GETEmployeeDto()
                {
                    Id = e.Id.ToString(),
                    Name = e.Name,
                    Title = e.Title,
                    HireDate = e.HireDate.ToShortDateString(),
                })
                .ToListAsync();

            return employeeDtos;
        }
        public async Task<IEnumerable<GETEmployeeDto>> AllDeactivatedAsync()
        {
            List<GETEmployeeDto> employeeDtos = await repository.AllAsReadOnly<Employee>()
                .Where(e => !e.IsActive)
                .Select(e => new GETEmployeeDto()
                {
                    Id = e.Id.ToString(),
                    Name = e.Name,
                    Title = e.Title,
                    HireDate = e.HireDate.ToShortDateString(),
                })
                .ToListAsync();

            return employeeDtos;
        }
        public async Task<bool> ExistsByIdAsync(Guid employeeId)
        {
            return await repository.AllAsReadOnly<Employee>().AnyAsync(e => e.Id == employeeId);
        }

        public async Task<GETEmployeeDto> GetByIdAsync(Guid employeeId)
        {
            Employee employee = await repository.AllAsReadOnly<Employee>().FirstOrDefaultAsync(e => e.Id == employeeId);

            GETEmployeeDto employeeDto = new GETEmployeeDto()
            {
                Id = employee.Id.ToString(),
                Name = employee.Name,
                Title = employee.Title,
                HireDate = employee.HireDate.ToShortDateString(),
            };

            return employeeDto;
        }

        public async Task<GETEmployeeDto> GetByNameAsync(string employeeName)
        {
            Employee employee = await repository.AllAsReadOnly<Employee>().FirstOrDefaultAsync(e => e.Name == employeeName);

            GETEmployeeDto employeeDto = new GETEmployeeDto()
            {
                Id = employee.Id.ToString(),
                Name = employee.Name,
                Title = employee.Title,
                HireDate = employee.HireDate.ToShortDateString(),
            };

            return employeeDto;
        }
        public async Task<string> ActivateAsync(Guid employeeId)
        {
            Employee employee = await repository.GetByIdAsync<Employee>(employeeId);
            employee.IsActive = true;

            await repository.SaveChangesAsync();
            return "Employee activated!";
        }

        public async Task<string> DeactivateAsync(Guid employeeId)
        {
            Employee employee = await repository.GetByIdAsync<Employee>(employeeId);
            employee.IsActive = false;

            await repository.SaveChangesAsync();
            return "Employee deactivated!";
        }

        public async Task<string> UpdateAsync(PUTEmployeeDto employeeDto, string employeeId)
        {
            Employee? employee = await repository.GetByIdAsync<Employee>(Guid.Parse(employeeId));

            employee.Name = employeeDto.Name;
            employee.Title = employeeDto.Title;
            employee.HireDate = employee.HireDate;

            await repository.SaveChangesAsync();

            return "Employee updated Successfully!";
        }

        public async Task<bool> IsEmployeeActiveAsync(Guid employeeId)
        {
            Employee? employee = await repository.GetByIdAsync<Employee>(employeeId);

            return employee.IsActive;
        }
    }
}
