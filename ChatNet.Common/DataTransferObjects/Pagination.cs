using System.Text.Json.Serialization;

namespace ChatNet.Common.DataTransferObjects; 

/// <summary>
/// Pagination DTO component
/// </summary>
public class Pagination<T> {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="items"></param>
    /// <param name="currentPage"></param>
    /// <param name="pageSize"></param>
    /// <param name="pagesAmount"></param>
    public Pagination(List<T> items, int currentPage, int pageSize, int pagesAmount) {
        Items = items;
        CurrentPage = currentPage;
        PageSize = pageSize;
        PagesAmount = pagesAmount;
    }

    /// <summary>
    /// List of items
    /// </summary>
    [JsonPropertyName("items")]
    public List<T> Items { get; set; }

    /// <summary>
    /// Page of list (natural number)
    /// </summary>
    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }

    /// <summary>
    /// Count of items on page (natural number)
    /// </summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>
    /// Amount of pages
    /// </summary>
    [JsonPropertyName("pages_amount")]
    public int PagesAmount { get; set; }
}