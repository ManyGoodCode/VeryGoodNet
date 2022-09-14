using CleanArchitecture.Blazor.Application.Features.CheckinPoints.DTOs;
using CleanArchitecture.Blazor.Application.Features.CheckinPoints.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Blazor.Server.UI.Pages.Devices;

public class CheckinPointAutocomplete : MudAutocomplete<int?>
{
    [Inject]
    private ISender mediator { get; set; } = default!;
    private List<CheckinPointDto> checkinpoints = new();

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
            checkinpoints = (await mediator.Send(new GetAllCheckinPointsQuery())).ToList();
            ForceRender(true);
        }
    }

    private Task<IEnumerable<int?>> Search(string value)
    {
        List<int?> list = new List<int?>();
        if (string.IsNullOrEmpty(value))
        {
            foreach (int i in checkinpoints.Select(x => x.Id))
            {
                list.Add(i);
            }
        }
        else
        {
            IEnumerable<int> result = checkinpoints.Where(x => value.Contains(x.Name)).Select(x => x.Id);
            foreach (int i in result)
            {
                list.Add(i);
            }
        }

        return Task.FromResult(list.AsEnumerable());
    }

    private string GetName(int? id)
    {
        CheckinPointDto chpoint = checkinpoints.Find(b => b.Id == id);
        if (chpoint is null)
        {
            return string.Empty;
        }
        else
        {
            return $"{chpoint.Site} - {chpoint.Name}";
        }
    }
}