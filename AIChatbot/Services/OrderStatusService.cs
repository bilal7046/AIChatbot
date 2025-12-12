using System.Text.RegularExpressions;

namespace AIChatbot.Services;

public class OrderStatusService
{
    private readonly ILogger<OrderStatusService> _logger;
    private readonly Dictionary<string, OrderStatus> _orderStatuses;

    public OrderStatusService(ILogger<OrderStatusService> logger)
    {
        _logger = logger;
        _orderStatuses = InitializeMockOrders();
    }

    public string? ExtractOrderNumber(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return null;

        // Common order number patterns in Absher
        // Pattern 1: REQ-XXXXXX format
        var pattern1 = @"REQ-?\s*(\d{6,})";
        var match1 = Regex.Match(message, pattern1, RegexOptions.IgnoreCase);
        if (match1.Success)
        {
            return $"REQ-{match1.Groups[1].Value}";
        }

        // Pattern 2: APP-XXXXXX format
        var pattern2 = @"APP-?\s*(\d{6,})";
        var match2 = Regex.Match(message, pattern2, RegexOptions.IgnoreCase);
        if (match2.Success)
        {
            return $"APP-{match2.Groups[1].Value}";
        }

        // Pattern 3: Just numbers (6-12 digits)
        var pattern3 = @"\b(\d{6,12})\b";
        var match3 = Regex.Match(message, pattern3);
        if (match3.Success)
        {
            return match3.Groups[1].Value;
        }

        // Pattern 4: Arabic numerals or mixed
        var pattern4 = @"[\u0660-\u0669\u06F0-\u06F9\d]{6,}";
        var match4 = Regex.Match(message, pattern4);
        if (match4.Success)
        {
            return match4.Value;
        }

        return null;
    }

    public OrderStatus? GetOrderStatus(string orderNumber)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            return null;

        // Normalize order number (remove spaces, convert to uppercase)
        var normalizedOrder = orderNumber.Trim().ToUpper().Replace(" ", "");

        // Try exact match first
        if (_orderStatuses.TryGetValue(normalizedOrder, out var status))
        {
            return status;
        }

        // Try partial match (in case user entered just the number part)
        foreach (var kvp in _orderStatuses)
        {
            if (kvp.Key.Contains(normalizedOrder) || normalizedOrder.Contains(kvp.Key.Replace("REQ-", "").Replace("APP-", "")))
            {
                return kvp.Value;
            }
        }

        return null;
    }

    public string GetStatusResponse(string orderNumber, OrderStatus? status)
    {
        if (status == null)
        {
            return $"Sorry, I couldn't find order '{orderNumber}' in the system. " +
                   "Double-check the number and try again. " +
                   "It's usually something like REQ-123456 or APP-123456. " +
                   "You can find it in the confirmation email or SMS you got when you submitted your request.";
        }

        var statusMessage = status.Status switch
        {
            "Submitted" => "We got your request and it's in the queue now.",
            "Under Review" => "Your application's being looked at by our team right now.",
            "Approved" => "Great news! Your request was approved. You might need to do a few more things like pay fees or pick up documents.",
            "Rejected" => $"Unfortunately, your request was rejected. {status.Notes ?? "Check your Absher account for more details on why."}",
            "Completed" => "All done! Your request is complete and ready for pickup or delivery.",
            "Pending" => "Your request is waiting for some info or documents from you. Check your Absher account to see what's needed.",
            _ => $"Your request status is: {status.Status}"
        };

        var response = $"Order {orderNumber} is currently {status.Status.ToLower()}.\n\n" +
                      $"{statusMessage}";

        if (!string.IsNullOrWhiteSpace(status.SubmittedDate))
        {
            response += $"\n\nYou submitted it on {status.SubmittedDate}.";
        }

        if (!string.IsNullOrWhiteSpace(status.LastUpdated))
        {
            response += $" Last update was {status.LastUpdated}.";
        }

        if (!string.IsNullOrWhiteSpace(status.Notes) && status.Status != "Rejected")
        {
            response += $"\n\n{status.Notes}";
        }

        response += "\n\nYou can also check this anytime by logging into Absher and going to 'My Services'.";

        return response;
    }

    private Dictionary<string, OrderStatus> InitializeMockOrders()
    {
        // Mock order data for demonstration
        // In production, this would come from a database or API
        return new Dictionary<string, OrderStatus>
        {
            {
                "REQ-123456",
                new OrderStatus
                {
                    OrderNumber = "REQ-123456",
                    Status = "Under Review",
                    SubmittedDate = "2024-01-15",
                    LastUpdated = "2024-01-18",
                    Notes = "Your ID renewal application is being processed. Expected completion: 2-3 business days."
                }
            },
            {
                "REQ-789012",
                new OrderStatus
                {
                    OrderNumber = "REQ-789012",
                    Status = "Approved",
                    SubmittedDate = "2024-01-10",
                    LastUpdated = "2024-01-16",
                    Notes = "Your passport application has been approved. Please schedule an appointment for biometrics."
                }
            },
            {
                "APP-345678",
                new OrderStatus
                {
                    OrderNumber = "APP-345678",
                    Status = "Completed",
                    SubmittedDate = "2024-01-05",
                    LastUpdated = "2024-01-12",
                    Notes = "Your driving license renewal is complete. Your new license is ready for pickup."
                }
            },
            {
                "REQ-901234",
                new OrderStatus
                {
                    OrderNumber = "REQ-901234",
                    Status = "Pending",
                    SubmittedDate = "2024-01-20",
                    LastUpdated = "2024-01-21",
                    Notes = "Additional documents are required. Please upload the requested documents in your Absher account."
                }
            },
            {
                "123456",
                new OrderStatus
                {
                    OrderNumber = "REQ-123456",
                    Status = "Under Review",
                    SubmittedDate = "2024-01-15",
                    LastUpdated = "2024-01-18",
                    Notes = "Your ID renewal application is being processed."
                }
            }
        };
    }
}

public class OrderStatus
{
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? SubmittedDate { get; set; }
    public string? LastUpdated { get; set; }
    public string? Notes { get; set; }
}

