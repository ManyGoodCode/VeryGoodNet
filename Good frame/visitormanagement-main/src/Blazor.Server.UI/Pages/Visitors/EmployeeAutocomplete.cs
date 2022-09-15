using CleanArchitecture.Blazor.Application.Features.Employees.DTOs;
using CleanArchitecture.Blazor.Application.Features.Employees.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Blazor.Server.UI.Pages.Visitors;

public class EmployeeAutocomplete : MudAutocomplete<int?>
{
    [Inject]
    private ISender mediator { get; set; } = default!;

    private List<EmployeeDto> employees = new List<EmployeeDto>();

    public override Task SetParametersAsync(ParameterView parameters)
    {
        Dense = true;
        ResetValueOnEmptyText = true;
        SearchFunc = Search;
        ToStringFunc = GetName;
        Clearable = true;
        return base.SetParametersAsync(parameters);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            employees = (await mediator.Send(new GetAllEmployeesQuery())).ToList();
            ForceRender(true);
        }
    }

    private Task<IEnumerable<int?>> Search(string value)
    {
        List<int?> list = new List<int?>();
        if (string.IsNullOrEmpty(value))
        {
            IEnumerable<int> result = employees.Select(x => x.Id);
            foreach (int i in result)
            {
                list.Add(i);
            }
        }
        else
        {
            IEnumerable<int> result = employees.Where(x => value.Contains(x.Name)).Select(x => x.Id);
            foreach (int i in result)
            {
                list.Add(i);
            }
        }

        return Task.FromResult(list.AsEnumerable());
    }

    private string GetName(int? id)
    {
        EmployeeDto emp = employees.Find(b => b.Id == id);
        if (emp is null)
        {
            return string.Empty;
        }
        else
        {
            return $"{emp.Name} ({emp.Designation})";
        }
    }
}