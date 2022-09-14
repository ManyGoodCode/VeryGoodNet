using CleanArchitecture.Blazor.Application.Features.Departments.DTOs;
using CleanArchitecture.Blazor.Application.Features.Departments.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Blazor.Server.UI.Pages.Departments;

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
            return Task.FromResult(departments.Select(x =>new int?(x.Id)).AsEnumerable());
        }
        else
        {
            IEnumerable<int?> result = departments.Where(x => x.Name.ToLower().Contains(value.ToLower())).Select(x =>new int?(x.Id)).AsEnumerable();
            return Task.FromResult(result);
        }
        
    }

    private string GetName(int? id) => departments.Find(b => b.Id == id)?.Name ?? string.Empty;
}