using Application.Features.Employees.Dtos.Requests;
using AutoMapper;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Employees;

public class EmplooyeManager : IEmployeeService
{
    private readonly IEmpoyeeRepository _repository;
    private readonly IMapper _mapper;

    public EmplooyeManager(IEmpoyeeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        employee.Id = new Guid();
        if (await AnyAsync(p => p.Id == employee.Id))
            await _repository.AddAsync(employee);
        employee.IsActive = true;
        Employee addedEmplooye = await _repository.AddAsync(employee);
        return addedEmplooye;
    }

    public async Task<bool> AnyAsync(Expression<Func<Employee, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        return await _repository.AnyAsync
            (
            predicate: predicate,
            withDeleted: withDeleted,
            enableTracking: enableTracking,
            cancellationToken: cancellationToken
            );
    }

    public async Task<Employee> DeleteAsync(DeleteEmployeeReqeust reqeust, bool permament = false, CancellationToken cancellationToken = default)
    {
        Employee? employee = await _repository.GetAsync(p=>p.Id==reqeust.Id,cancellationToken:cancellationToken);
        if (employee == null)
            throw new BusinessException("Employee not exists");
        employee.IsActive= false;
        Employee deletedEmployee = await _repository.DeleteAsync(employee);

        return deletedEmployee;
    }

    public async Task<IPaginate<Employee>> GetAllAsync(Expression<Func<Employee, bool>>? predicate = null, Func<IQueryable<Employee>, IIncludableQueryable<Employee, object>>? include = null, Func<IQueryable<Employee>, IOrderedQueryable<Employee>>? orderBy = null, bool enableTracking = true, bool withDeleted = false, int index = 0, int size = 10, CancellationToken cancellationToken = default)
    {
        Paginate<Employee> employees = await _repository.GetListAsync
            (
            predicate: predicate,
            include: include,
            orderBy: orderBy,
            withDeleted: withDeleted,
            enableTracking: enableTracking,
            cancellationToken: cancellationToken
            );
        return employees;
    }

    public async Task<Employee> GetAsync(Expression<Func<Employee, bool>> predicate, Func<IQueryable<Employee>, IIncludableQueryable<Employee, object>>? include, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        Employee? employee = await _repository.GetAsync
            (
            predicate:predicate,
            include:include,
            withDeleted:withDeleted,
            enableTracking:enableTracking,
            cancellationToken:cancellationToken
            );
        return employee ?? throw new BusinessException("Employee not deleted");
    }

    public async Task<Employee> UpdateAsync(UpdateEmployeeReqeust reqeust, CancellationToken cancellationToken = default)
    {
        Employee? employee = await _repository.GetAsync(p=>p.Id==reqeust.Id,cancellationToken:cancellationToken);
        employee = _mapper.Map(reqeust,employee);
        await _repository.UpdateAsync(employee!);
        return employee;
    }
}
