namespace HtawPyi.Application;

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);
public record AuthResponse(string AccessToken, string RefreshToken, string Email, string Role);

public record TicketDto(Guid Id, string DrawDate, string Number, decimal Price);

public record CreateOrderRequest(List<Guid> TicketIds);
public record CreateOrderResponse(Guid OrderId, decimal Total, string Provider, string RedirectUrl);

public record OrderItemDto(string Number, string DrawDate, decimal Price, List<WinDto> Wins);
public record WinDto(string Name, decimal Reward);
public record OrderDto(
    Guid Id, string Status, decimal Total, DateTime CreatedAt, List<OrderItemDto> Items);

public record AdminOrderDto(
    Guid Id, string CustomerEmail, string Status, decimal Total, DateTime CreatedAt,
    List<string> TicketNumbers);

public record UploadTicketRequest(string DrawDate, string Number, decimal Price);
public record UploadReportRow(int Row, string Number, string Error);
public record UploadReport(int Inserted, List<UploadReportRow> Rejected);

public record AdminTicketDto(
    Guid Id, string DrawDate, string Number, decimal Price, string Status);
