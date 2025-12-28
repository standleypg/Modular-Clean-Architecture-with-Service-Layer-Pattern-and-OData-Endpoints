namespace RetailPortal.Model.DTOs.Common;

public class ODataResponse<T>
{
    public IEnumerable<T>? Value { get; set; }
    public string? Count { get; set; }
    public string? NextPage { get; set; }
}