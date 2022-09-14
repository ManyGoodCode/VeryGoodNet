using CleanArchitecture.Blazor.Application.Features.Departments.DTOs;
using CleanArchitecture.Blazor.Application.Features.Departments.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Blazor.Server.UI.Pages.Employees;

public class DepartmentAutocomplete : MudAutocomplete<int?>
{
    [Inject]
    private ISender mediator { get; set; } = default!;
    private List<DepartmentDto> departments = new();

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
            departments = (await mediator.Send(new GetAllDepartmentsQuery())).ToList();
            ForceRender(true);
        }
    }

    private Task<IEnumerable<int?>> Search(string value)
    {
        List<int?> list = new List<int?>();
        if (string.IsNullOrEmpty(value))
        {
            IEnumerable<int> result = departments.Select(x => x.Id);
            foreach (int i in result)
            {
                list.Add(i);
            }
        }
        else
        {
            IEnumerable<int> result = departments.Where(x => x.Name.ToLower().Contains(value.ToLower())).Select(x => x.Id);
            foreach (int i in result)
            {
                list.Add(i);
            }
        }

        return Task.FromResult(list.AsEnumerable());
    }

    private string GetName(int? id) => departments.Find(b => b.Id == id)?.Name ?? string.Empty;
}