using CleanArchitecture.Blazor.Application.Features.Sites.DTOs;
using CleanArchitecture.Blazor.Application.Features.Sites.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Blazor.Server.UI.Components.AutoComplete
{
    public class AssignSiteIdAutocomplete : MudAutocomplete<int?>
    {
        [Inject]
        private ISender mediator { get; set; } = default!;
        private List<SiteDto> sites = new();

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

        private Task<IEnumerable<int?>> Search(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Task.FromResult(sites.Select(x => new int?(x.Id)).ToList().AsEnumerable());
            }
            else
            {
                List<int?> result = sites.Where(x => x.Name.StartsWith(value)).Select(x => new int?(x.Id)).ToList();
                return Task.FromResult(result.AsEnumerable());
            }
        }

        private string GetName(int? id)
        {
            if (id is null || id <= 0)
            {
                return string.Empty;
            }
            else
            {
                return sites.Find(b => b.Id == id)?.Name;
            }
        }
    }
}