using Application.Features.Employees.Dtos.Requests;
using Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Employees;

public interface IEmployeeService
{
    public Task<Employee> AddAsync(Employee employee,CancellationToken cancellationToken=default);
    public Task<Employee> UpdateAsync(UpdateEmployeeReqeust reqeust,CancellationToken cancellationToken=default);
    public Task<Employee> DeleteAsync(DeleteEmployeeReqeust reqeust,bool permament = false,CancellationToken cancellationToken=default);
    public Task<Employee> GetAsync(
        Expression<Func<Employee, bool>> predicate,
        Func<IQueryable<Employee>,IIncludableQueryable<Employee,object>>? include=null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);
    public Task<IPaginate<Employee>> GetAllAsync(
        Expression<Func<Employee,bool>>? predicate = null,
        Func<IQueryable<Employee>,IIncludableQueryable<Employee,object>>? include = null,
        Func<IQueryable<Employee>,IOrderedQueryable<Employee>>? orderBy = null,
        bool enableTracking = true,
        bool withDeleted = false,
        int index = 0,
        int size = 10,
        CancellationToken cancellationToken = default);

    public Task<bool> AnyAsync(
        Expression<Func<Employee, bool>>? predicate = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);
}
