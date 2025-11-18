namespace CleanArch.Common;

/// <summary>
/// Options for paging through data
/// </summary>
/// <param name="PageIndex">Page index (1-based)</param>
/// <param name="PageSize">Max number of items per request</param>
public record PaginationOptions(int PageIndex, int PageSize);

/// <summary>
/// The results of a pagination request
/// </summary>
/// <param name="Items">Items returned</param>
/// <param name="Total">Total items matching request</param>
/// <param name="PageIndex">Current page index (1-based)</param>
/// <param name="PageSize">Current page size</param>
/// <typeparam name="T"></typeparam>
public record PaginationResult<T>(IEnumerable<T> Items, int Total, int PageIndex, int PageSize);

/// <summary>
/// A pagination request where the total count is indeterminant. 
/// </summary>
/// <param name="PageSize">Max number of items per page</param>
/// <param name="ContinueFrom">Token to continue paging. Optional</param>
/// <typeparam name="TokenType"></typeparam>
public record IndeterminantPagnationOptions<TokenType>(int PageSize, TokenType? ContinueFrom);

/// <summary>
/// A pagination result where the total count is indeterminant. Gives the token to continue from on the next request.
/// </summary>
/// <param name="Items">Items returned</param>
/// <param name="PageSize">Current page size</param>
/// <param name="ContinueFrom">Token to use in the next request to continue paging through the data</param>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TokenType"></typeparam>
public record IndeterminantPaginationResult<T, TokenType>(IEnumerable<T> Items, int PageSize, TokenType? ContinueFrom);
