using CleanArchitecture.Blazor.Application.Features.CheckinPoints.DTOs;
using CleanArchitecture.Blazor.Application.Features.CheckinPoints.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Blazor.Server.UI.Pages.CheckinPoints
{
    public class CheckinPointWithSiteIdAutocomplete : MudAutocomplete<int?>
    {

        [Inject]
        private ISender mediator { get; set; } = default!;

        [Parameter]
        [EditorRequired]
        public int? SiteId { get; set; }

        private List<CheckinPointDto> checkinpoints = new();

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
                checkinpoints = (await mediator.Send(new GetAllCheckinPointsQuery())).ToList();
                ForceRender(true);
            }
        }

        private Task<IEnumerable<int?>> Search(string value)
        {
            List<int?> list = new List<int?>();
            if (string.IsNullOrEmpty(value))
            {
                IEnumerable<int?> result = checkinpoints.Where(x => x.SiteId == SiteId).Select(x => new int?(x.Id)).AsEnumerable();
                return Task.FromResult(result);
            }
            else
            {
                IEnumerable<int?> result = checkinpoints.Where(x => value.Contains(x.Name) && x.SiteId == SiteId).Select(x => new int?(x.Id)).AsEnumerable();
                return Task.FromResult(result);
            }
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
}