using CleanArchitecture.Blazor.Application.Features.Departments.DTOs;
using CleanArchitecture.Blazor.Application.Features.Departments.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Blazor.Server.UI.Pages.Identity.Users;

public class AssignDepartmentAutocomplete : MudAutocomplete<string?>
{
    [Inject]
    private ISender mediator { get; set; } = default!;

    private List<DepartmentDto> list = new List<DepartmentDto>();

    public override Task SetParametersAsync(ParameterView parameters)
    {
        Dense = true;
        SearchFunc = Search;
        ToStringFunc = GetName;
        return base.SetParametersAsync(parameters);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            list = (await mediator.Send(new GetAllDepartmentsQuery())).ToList();
            ForceRender(true);
        }
    }

    private Task<IEnumerable<string?>> Search(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(list.Select(x => x.Name).ToList().AsEnumerable());
        }
        else
        {
            List<string> result = list.Where(x => x.Name.StartsWith(value)).Select(x => x.Name).ToList();
            return Task.FromResult(result.AsEnumerable());
        }
    }

    private string GetName(string? txt)
    {
        if (string.IsNullOrEmpty(txt))
        {
            return string.Empty;
        }
        else
        {
            return list.Find(b => b.Name == txt)?.Name;
        }
    }
}