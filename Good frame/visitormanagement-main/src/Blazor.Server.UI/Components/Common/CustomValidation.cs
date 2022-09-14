using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Server.UI.Components.Common
{
    public class CustomModelValidation : ComponentBase
    {
        private ValidationMessageStore? messageStore;

        [CascadingParameter]
        private EditContext? CurrentEditContext { get; set; }

        protected override void OnInitialized()
        {
            if (CurrentEditContext is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(CustomModelValidation)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. " +
                    $"For example, you can use {nameof(CustomModelValidation)} " +
                    $"inside an {nameof(EditForm)}.");
            }

            messageStore = new(CurrentEditContext);

            CurrentEditContext.OnValidationRequested += (s, e) =>
                messageStore?.Clear();
            CurrentEditContext.OnFieldChanged += (s, e) =>
                messageStore?.Clear(e.FieldIdentifier);
        }

        public void DisplayErrors(IDictionary<string, ICollection<string>> errors)
        {
            if (CurrentEditContext is not null && errors is not null)
            {
                foreach (var err in errors)
                {
                    messageStore?.Add(CurrentEditContext.Field(err.Key), err.Value);
                }

                CurrentEditContext.NotifyValidationStateChanged();
            }
        }

        public void ClearErrors()
        {
            messageStore?.Clear();
            CurrentEditContext?.NotifyValidationStateChanged();
        }
    }
}