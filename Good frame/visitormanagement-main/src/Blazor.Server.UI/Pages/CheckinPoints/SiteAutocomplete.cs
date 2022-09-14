using CleanArchitecture.Blazor.Application.Features.Sites.DTOs;
using CleanArchitecture.Blazor.Application.Features.Sites.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Blazor.Server.UI.Pages.CheckinPoints;

public class SiteAutocomplete : MudAutocomplete<int?>
{

    [Inject]
    private ISender mediator { get; set; } = default!;

    private List<SiteDto> sites = new();

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
            sites = (await mediator.Send(new GetAllSitesQuery())).ToList();
            ForceRender(true);
        }
    }

    private Task<IEnumerable<int?>> Search(string value)
    {
        List<int?> list = new List<int?>();
        if (string.IsNullOrEmpty(value))
        {
            IEnumerable<int> result = sites.Select(x => x.Id);
            foreach (int i in result)
            {
                list.Add(i);
            }

        }
        else
        {
            IEnumerable<int> result = sites.Where(x => x.Name.Contains(value)).Select(x => x.Id);
            foreach (int i in result)
            {
                list.Add(i);
            }
        }

        return Task.FromResult(list.AsEnumerable());
    }

    private string GetName(int? id)
    {
        SiteDto site = sites.Find(b => b.Id == id);
        if (site is null)
        {
            return string.Empty;
        }
        else
        {
            return $"{site.Name} - {site.Address}";
        }
    }
}