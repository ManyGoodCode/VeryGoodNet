using CleanArchitecture.Blazor.Application.Features.Sites.DTOs;
using CleanArchitecture.Blazor.Application.Features.Sites.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Blazor.Server.UI.Pages.Identity.Users;

public class AssignSiteAutocomplete : MudAutocomplete<string?>
{
    [Inject]
    private ISender mediator { get; set; } = default!;

    private List<SiteDto> sites = new List<SiteDto>();

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
            sites = (await mediator.Send(new GetAllSitesQuery())).ToList();
            ForceRender(true);
        }
    }

    private Task<IEnumerable<string?>> Search(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(sites.Select(x => x.Name).ToList().AsEnumerable());
        }
        else
        {
            List<string> result = sites.Where(x => x.Name.StartsWith(value)).Select(x => x.Name).ToList();
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
            return sites.Find(b => b.Name == txt)?.Name;
        }
    }
}