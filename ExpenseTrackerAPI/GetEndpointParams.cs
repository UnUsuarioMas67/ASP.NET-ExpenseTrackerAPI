using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerAPI;

public class GetEndpointParams
{
    [FromQuery(Name = "search_string")] 
    public string SearchString { get; set; } = string.Empty;
    
    [FromQuery(Name = "sort_by")] 
    public SortOption SortBy { get; set; } = SortOption.Date;
    
    [FromQuery(Name = "date_filter")] 
    public DateFilter DateFilter { get; set; } = DateFilter.Default;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SortOption
{
    Amount,
    Date,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DateFilter
{
    Default,
    PastWeek,
    PastMonth,
    Last3Months,
}