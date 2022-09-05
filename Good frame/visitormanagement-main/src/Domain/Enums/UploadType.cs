using System.ComponentModel;

/// <summary>
/// Product/VisitorPricture/VisitHistoryPricture/ProfilePicture/EmployeePicture/Document
/// </summary>
namespace CleanArchitecture.Blazor.Domain
{
    public enum UploadType : byte
    {
        [Description(@"Products")]
        Product,
        [Description(@"VisitorPrictures")]
        VisitorPricture,
        [Description(@"VisitHistoryPrictures")]
        VisitHistoryPricture,
        [Description(@"ProfilePictures")]
        ProfilePicture,
        [Description(@"EmployeePictures")]
        EmployeePicture,
        [Description(@"Documents")]
        Document
    }
}
