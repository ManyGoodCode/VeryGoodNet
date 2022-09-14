using CleanArchitecture.Blazor.Application.Features.Sites.DTOs;
using CleanArchitecture.Blazor.Application.Features.Sites.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Blazor.Server.UI.Pages.Identity.Users;

public class SelectSiteAutocomplete : MudAutocomplete<SiteDto?>
{
    [Inject]
    private ISender mediator { get; set; } = default!;

    [Parameter]
    public EventCallback<string> SiteChanged { get; set; }

    private List<SiteDto?> sites = new();

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
    
    private Task<IEnumerable<SiteDto?>> Search(string value)
    {
        List<SiteDto?> list = new List<SiteDto?>();
        foreach(SiteDto item in sites)
        {
            list.Add(item);
        }
        
        return Task.FromResult(list.AsEnumerable());
    }

    private string GetName(SiteDto? item)
    {
        return item?.Name;
    }
}