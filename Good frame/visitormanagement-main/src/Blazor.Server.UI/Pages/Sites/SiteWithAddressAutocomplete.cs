using CleanArchitecture.Blazor.Application.Features.Departments.DTOs;
using CleanArchitecture.Blazor.Application.Features.Departments.Queries.GetAll;
using CleanArchitecture.Blazor.Application.Features.Sites.DTOs;
using CleanArchitecture.Blazor.Application.Features.Sites.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Blazor.Server.UI.Pages.Sites;

public class SiteWithAddressAutocomplete : MudAutocomplete<int?>
{
    [Inject]
    private ISender mediator { get; set; } = default!;

    private List<SiteDto> sites = new List<SiteDto>();

    public override Task SetParametersAsync(ParameterView parameters)
    {
        Dense = true;
        SearchFunc = Search;
        ToStringFunc = GetName;
        Clearable = true;
        return base.SetParametersAsync(parameters);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            sites = (await mediator.Send(new GetAllSitesQuery())).ToList();
            ForceRender(true);
        }
    }

    private Task<IEnumerable<int?>> Search(string value)
    {
        List<int?> list = new List<int?>();
        if (string.IsNullOrEmpty(value))
        {
            IEnumerable<int?> result = sites.Select(x => new int?(x.Id)).AsEnumerable();
            return Task.FromResult(result);

        }
        else
        {
            IEnumerable<int?> result = sites.Where(x => value.Contains(x.Name)).Select(x => new int?(x.Id)).AsEnumerable();
            return Task.FromResult(result);
        }
    }

    private string GetName(int? id)
    {
        SiteDto site = sites.Find(b => b.Id == id);
        if (site is null)
            return string.Empty;
        return $"{site.Name} - {site.Address}";
    }
}